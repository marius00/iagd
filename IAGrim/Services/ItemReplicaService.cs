using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EvilsoftCommons.Exceptions;
using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Services.ItemReplica;
using IAGrim.Services.MessageProcessor;
using IAGrim.Utilities;
using log4net;
using log4net.Repository.Hierarchy;

namespace IAGrim.Services {
    // TODO: Class does too much, and is somewhat of a mess.
    class ItemReplicaService : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemReplicaService));
        private readonly IPlayerItemDao _playerItemDao;
        private volatile bool _isGrimDawnRunning = false;
        private volatile bool _isShuttingDown = false;
        private Thread _t = null;
        private readonly ActionCooldown _cooldown = new ActionCooldown(2500);
        private ReplicaCache _cache = new ReplicaCache();

        public ItemReplicaService(IPlayerItemDao playerItemDao) {
            _playerItemDao = playerItemDao;
        }

        private bool DispatchItemSeedInfoRequest(PlayerItem pi) {
            List<byte> buffer = Serialize(pi);
            if (buffer == null) {
                return false;
            }

            using (NamedPipeClientStream pipeStream = new NamedPipeClientStream(".", "gdiahook", PipeDirection.InOut, PipeOptions.Asynchronous)) {
                try {
                    pipeStream.Connect(250);
                }
                catch (TimeoutException) {
                    // Typically: The pipe does not exist
                    Logger.Debug("Timed out connecting to GD");
                    _isGrimDawnRunning = false;
                    return false;
                }
                catch (IOException ex) {
                    // Typical scenario: The pipe exists, but nobody are accepting connections.
                    Logger.Warn("IOException connecting to GD", ex);
                    _isGrimDawnRunning = false;
                    return false;

                }
                catch (Exception ex) {
                    Logger.Warn("Exception connecting to GD", ex);
                    return false;
                }

                pipeStream.Write(buffer.ToArray(), 0, buffer.Count);
#if DEBUG
                Logger.Debug("Wrote item to pipe");
#endif
            }

            return true;
        }


        private static List<byte> Serialize(PlayerItem pi) {
            List<byte> buffer = new List<byte>();

            if (pi.BaseRecord?.Length > 255 || pi.SuffixRecord?.Length > 255 || pi.PrefixRecord?.Length > 255
                || pi.MateriaRecord?.Length > 255 || pi.ModifierRecord?.Length > 255 || pi.EnchantmentRecord?.Length > 255
                || pi.TransmuteRecord?.Length > 255) {
                Logger.Warn("Received a player item with one or more records having a length of >255. Stat reproduction not possible.");
                return null;
            }

#if DEBUG
            StringBuilder sb = new StringBuilder();
            sb.Append(pi.Id + ";");
            sb.Append(pi.Seed + ";");
            sb.Append(pi.RelicSeed + ";");
            sb.Append(pi.EnchantmentSeed + ";");
            sb.Append(pi.BaseRecord + ";");
            sb.Append(pi.PrefixRecord + ";");
            sb.Append(pi.SuffixRecord + ";");
            sb.Append(pi.ModifierRecord + ";");
            sb.Append(pi.MateriaRecord + ";");
            sb.Append(pi.EnchantmentRecord + ";");
            sb.Append(pi.TransmuteRecord + ";");
            Logger.Debug($"Dispatching: {sb.ToString()}");
#endif

            buffer.AddRange(BitConverter.GetBytes((long)pi.Id));
            buffer.AddRange(BitConverter.GetBytes((int)pi.Seed));
            buffer.AddRange(BitConverter.GetBytes((int)pi.RelicSeed));
            buffer.AddRange(BitConverter.GetBytes((int)pi.EnchantmentSeed));

            buffer.AddRange(BitConverter.GetBytes(pi.BaseRecord.Length));
            buffer.AddRange(Encoding.ASCII.GetBytes(pi.BaseRecord));

            buffer.AddRange(BitConverter.GetBytes(pi.PrefixRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.PrefixRecord))
                buffer.AddRange(Encoding.ASCII.GetBytes(pi.PrefixRecord));

            buffer.AddRange(BitConverter.GetBytes(pi.SuffixRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.SuffixRecord))
                buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(pi.SuffixRecord));

            buffer.AddRange(BitConverter.GetBytes(pi.ModifierRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.ModifierRecord))
                buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(pi.ModifierRecord));

            buffer.AddRange(BitConverter.GetBytes(pi.MateriaRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.MateriaRecord))
                buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(pi.MateriaRecord));

            buffer.AddRange(BitConverter.GetBytes(pi.EnchantmentRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.EnchantmentRecord))
                buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(pi.EnchantmentRecord));

            buffer.AddRange(BitConverter.GetBytes(pi.TransmuteRecord?.Length ?? 0));
            if (!string.IsNullOrEmpty(pi.TransmuteRecord))
                buffer.AddRange(System.Text.Encoding.ASCII.GetBytes(pi.TransmuteRecord));

            return buffer;
        }

        public void SetIsGrimDawnRunning(bool b) {
            _isGrimDawnRunning = b;
        }

        public void Start() {
            if (_t != null)
                throw new ArgumentException("Max one thread running per instance");

            _t = new Thread(() => {
                ExceptionReporter.EnableLogUnhandledOnThread();

                while (!_isShuttingDown) {
                    if (_cooldown.IsReady) {
                        Process();
                        _cooldown.Reset();
                    }
                    
                    Thread.Sleep(1);
                }
            });

            _t.Start();
        }

        private bool Process() {
            if (!_isGrimDawnRunning)
                return false;

            var items = _playerItemDao.ListMissingReplica(300);
            if (items.Count == 0) {
                return false;
            }

            foreach (var item in items) {
                var hash = ItemReplicaProcessor.GetHash(item);
                if (_cache.Exists(hash)) // Don't ask for the same item twice. Esp if the user somehow gets two identical items in, this would infinitely loop.
                    continue;

                if (!DispatchItemSeedInfoRequest(item))
                    return false; //

                _cache.Add(hash);
                Thread.Sleep(1);
            }

            return true;
        }

        public void Dispose() {
            _isShuttingDown = true;
        }


    }
}
