using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EvilsoftCommons.Exceptions;
using IAGrim.Backup.Cloud.Dto;
using IAGrim.Backup.Cloud.Util;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Settings;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IAGrim.Backup.Cloud.Service {
    /// <summary>
    /// Live sync over a websocket, active only for users on the "multiple PCs" (dual computer) setting.
    ///
    /// This is an additive fast-path on top of the regular REST backup, which remains the source of
    /// truth and owns the sync timestamp. This service:
    ///  * pushes newly looted items and in-game transfers (deletions) the instant they happen, and
    ///  * applies the same events received from the user's other machines, deduplicating by cloud id
    ///    and never touching the REST sync timestamp.
    ///
    /// A dropped connection is harmless: the regular REST sync reconciles anything missed. The
    /// connection is retried with backoff for as long as the setting is enabled and a token exists.
    /// </summary>
    public class WebSocketSyncService : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WebSocketSyncService));
        private const int MaxBatchSize = 100;
        private const int InitialBackoffMs = 2000;
        private const int MaxBackoffMs = 60000;

        private readonly AuthenticationProvider _authenticationProvider;
        private readonly SettingsService _settings;
        private readonly IPlayerItemDao _playerItemDao;

        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private volatile ClientWebSocket? _socket;
        private Thread? _thread;
        private volatile bool _running;

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore,
            Culture = System.Globalization.CultureInfo.InvariantCulture,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        /// <summary>Raised (on a background thread) after items are added or removed via live sync.</summary>
        public event EventHandler? OnItemsChanged;

        public WebSocketSyncService(
            AuthenticationProvider authenticationProvider,
            SettingsService settings,
            IPlayerItemDao playerItemDao) {
            _authenticationProvider = authenticationProvider;
            _settings = settings;
            _playerItemDao = playerItemDao;
        }

        public void Start() {
            if (_running) return;
            _running = true;
            _thread = new Thread(ConnectionLoop) { IsBackground = true, Name = "WebSocketSync" };
            _thread.Start();
        }

        // Continuously maintains a connection while the setting is enabled and a token exists.
        private void ConnectionLoop() {
            ExceptionReporter.EnableLogUnhandledOnThread();
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

            var backoff = InitialBackoffMs;
            while (_running && !_cts.IsCancellationRequested) {
                if (!ShouldConnect()) {
                    CloseSocket();
                    Sleep(2000);
                    continue;
                }

                try {
                    Connect();
                    Logger.Info("Websocket live sync connected");
                    backoff = InitialBackoffMs;
                    SuperviseConnection();
                }
                catch (OperationCanceledException) {
                    // Shutting down
                }
                catch (Exception ex) {
                    Logger.Warn($"Websocket live sync error: {ex.Message}", ex);
                }
                finally {
                    CloseSocket();
                }

                if (_running && !_cts.IsCancellationRequested) {
                    Sleep(backoff);
                    backoff = Math.Min(backoff * 2, MaxBackoffMs);
                }
            }
        }

        // Live sync runs only while BOTH conditions hold: the user has enabled "multiple PCs"
        // (UsingDualComputer) and is logged into online backups (a cloud auth token exists).
        private bool ShouldConnect() {
            return _settings.GetPersistent().UsingDualComputer
                   && _authenticationProvider.HasToken();
        }

        // Runs the receive loop while watching the enabling conditions. If the user disables the
        // "multiple PCs" setting or logs out mid-session, the connection is torn down promptly
        // rather than lingering until the socket happens to error.
        private void SuperviseConnection() {
            var receiveTask = Task.Run(ReceiveLoop);

            while (_running && !_cts.IsCancellationRequested) {
                if (receiveTask.IsCompleted) {
                    break; // connection closed or dropped
                }

                if (!ShouldConnect()) {
                    Logger.Info("Websocket live sync no longer enabled (dual-computer off or logged out), disconnecting");
                    break;
                }

                Sleep(1000);
            }

            // Abort any in-flight ReceiveAsync so the receive task can unwind, then wait for it.
            CloseSocket();
            try {
                receiveTask.Wait(2000);
            }
            catch (Exception) {
                // ignored -- the task faults with the expected abort exception
            }
        }

        private void Connect() {
            var socket = new ClientWebSocket();
            socket.Options.SetRequestHeader("Authorization", _authenticationProvider.GetToken());
            socket.Options.SetRequestHeader("X-Api-User", _authenticationProvider.GetUser());
            socket.ConnectAsync(new Uri(Uris.WebSocketUrl), _cts.Token).GetAwaiter().GetResult();
            _socket = socket;
        }

        private void ReceiveLoop() {
            var socket = _socket;
            if (socket == null) return;

            var buffer = new byte[16 * 1024];
            using var ms = new MemoryStream();

            try {
                while (_running && socket.State == WebSocketState.Open && !_cts.IsCancellationRequested) {
                    ms.SetLength(0);
                    WebSocketReceiveResult result;
                    do {
                        result = socket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token).GetAwaiter().GetResult();
                        if (result.MessageType == WebSocketMessageType.Close) {
                            socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None)
                                .GetAwaiter().GetResult();
                            return;
                        }

                        ms.Write(buffer, 0, result.Count);
                    } while (!result.EndOfMessage);

                    var json = Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                    HandleMessage(json);
                }
            }
            catch (Exception) {
                // Socket aborted (teardown) or a network error -- the supervisor will reconnect if appropriate.
            }
        }

        private void HandleMessage(string json) {
            WsEnvelope? env;
            try {
                env = JsonConvert.DeserializeObject<WsEnvelope>(json, _jsonSettings);
            }
            catch (Exception ex) {
                Logger.Warn($"Received malformed websocket message: {ex.Message}");
                return;
            }

            if (env == null) return;

            switch (env.Type) {
                case "item":
                    HandleIncomingItems(env.Items);
                    break;
                case "delete":
                    HandleIncomingDeletions(env.Removed);
                    break;
                default:
                    Logger.Warn($"Received unknown websocket message type: {env.Type}");
                    break;
            }
        }

        private void HandleIncomingItems(List<CloudItemDto>? items) {
            if (items == null || items.Count == 0) return;

            // Skip anything we already own (dedupe by cloud id) or have locally deleted -- the same
            // item will also arrive via the regular REST download, so this prevents duplication.
            var known = new HashSet<string>(_playerItemDao.GetOnlineIds().Where(id => !string.IsNullOrEmpty(id)));
            var deleted = new HashSet<string>(_playerItemDao.GetItemsMarkedForOnlineDeletion().Select(d => d.Id));

            var toStore = items
                .Where(i => !string.IsNullOrEmpty(i.Id))
                .Where(i => !known.Contains(i.Id) && !deleted.Contains(i.Id))
                .Select(ItemConverter.ToPlayerItem) // marks IsCloudSynchronized = true, so we never re-upload it
                .ToList();

            if (toStore.Count == 0) return;

            _playerItemDao.Save(toStore);
            Logger.Info($"Websocket live sync stored {toStore.Count} item(s)");
            OnItemsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void HandleIncomingDeletions(List<DeleteItemDto>? removed) {
            if (removed == null || removed.Count == 0) return;

            var toDelete = removed.Where(d => !string.IsNullOrEmpty(d.Id)).ToList();
            if (toDelete.Count == 0) return;

            _playerItemDao.Delete(toDelete);
            Logger.Info($"Websocket live sync removed {toDelete.Count} item(s)");
            OnItemsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Push newly acquired items to the user's other machines. No-op if not connected.</summary>
        public void SendItems(IList<PlayerItem> items) {
            var withCloudId = items.Where(i => !string.IsNullOrEmpty(i.CloudId)).ToList();
            if (withCloudId.Count == 0) return;

            foreach (var batch in Batch(withCloudId)) {
                Send(new WsEnvelope {
                    Type = "item",
                    Items = batch.Select(ItemConverter.ToUpload).ToList()
                });
            }
        }

        /// <summary>Push item deletions (transfers back in-game) to the user's other machines. No-op if not connected.</summary>
        public void SendDeletions(IList<string> cloudIds) {
            var ids = cloudIds.Where(id => !string.IsNullOrEmpty(id)).ToList();
            if (ids.Count == 0) return;

            foreach (var batch in Batch(ids)) {
                Send(new WsEnvelope {
                    Type = "delete",
                    Removed = batch.Select(id => new DeleteItemDto { Id = id }).ToList()
                });
            }
        }

        private void Send(WsEnvelope envelope) {
            var socket = _socket;
            if (socket == null || socket.State != WebSocketState.Open) {
                // Not connected; the regular REST sync will carry this instead.
                return;
            }

            byte[] bytes;
            try {
                bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(envelope, _jsonSettings));
            }
            catch (Exception ex) {
                Logger.Warn($"Failed to serialize websocket message: {ex.Message}", ex);
                return;
            }

            // WebSocket requires sends to be serialized.
            _sendLock.Wait();
            try {
                socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _cts.Token)
                    .GetAwaiter().GetResult();
            }
            catch (Exception ex) {
                Logger.Warn($"Failed to push websocket message: {ex.Message}");
            }
            finally {
                _sendLock.Release();
            }
        }

        private static IEnumerable<List<T>> Batch<T>(IReadOnlyList<T> items) {
            for (var i = 0; i < items.Count; i += MaxBatchSize) {
                yield return items.Skip(i).Take(MaxBatchSize).ToList();
            }
        }

        private void Sleep(int ms) {
            try {
                _cts.Token.WaitHandle.WaitOne(ms);
            }
            catch (Exception) {
                // ignored
            }
        }

        private void CloseSocket() {
            var socket = _socket;
            _socket = null;
            if (socket == null) return;

            try {
                // Abort (rather than a graceful CloseAsync) so any in-flight ReceiveAsync is
                // unblocked immediately; a graceful close awaits the close handshake on the
                // receive side we already occupy, which would hang.
                socket.Abort();
            }
            catch (Exception) {
                // ignored -- best effort
            }
            finally {
                socket.Dispose();
            }
        }

        public void Dispose() {
            _running = false;
            try {
                _cts.Cancel();
            }
            catch (Exception) {
                // ignored
            }
            CloseSocket();
        }

        private class WsEnvelope {
            public string Type { get; set; } = string.Empty;
            public List<CloudItemDto>? Items { get; set; }
            public List<DeleteItemDto>? Removed { get; set; }
        }
    }
}
