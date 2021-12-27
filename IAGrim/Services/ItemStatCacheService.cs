﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EvilsoftCommons.Exceptions;
using IAGrim.Database.Interfaces;
using IAGrim.UI.Controller.dto;
using IAGrim.Utilities;
using log4net;
using Newtonsoft.Json;

namespace IAGrim.Services {
    class ItemStatCacheService : IDisposable {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemStatCacheService));
        private readonly IPlayerItemDao _playerItemDao;
        private readonly IDatabaseItemDao _databaseItemDao;
        private readonly ItemStatService _itemStatService;
        private Thread t = null;

        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Culture = System.Globalization.CultureInfo.InvariantCulture,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public ItemStatCacheService(IPlayerItemDao playerItemDao, ItemStatService itemStatService, IDatabaseItemDao databaseItemDao) {
            _playerItemDao = playerItemDao;
            _itemStatService = itemStatService;
            _databaseItemDao = databaseItemDao;
        }

        public void Start() {
            if (t != null) {
                throw new ArgumentException("Attempting to a second thread");
            }

            t = new Thread(ThreadStart);
            t.Start();
        }

        private void ThreadStart() {
            ExceptionReporter.EnableLogUnhandledOnThread();
            t.Name = "ItemStatCache";
            t.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            // Sleeping a bit in the start, we don't wanna keep locking the DB (SQLite mode) on startup
            try {
                Thread.Sleep(1000 * 15);
            }
            catch (ThreadInterruptedException) {
                // Don't care
            }

            while (t?.IsAlive ?? false) {
                // Only perform action if we got plenty in the DB, otherwise we might not be parsed yet.
                bool result = false;
                if (_databaseItemDao.GetRowCount() > 20000) {
                    result = Tick();
                }

                try {
                    Thread.Sleep(result ? 1500 : 120 * 1000);
                }
                catch (ThreadInterruptedException) {
                    // Don't care
                }
            }
        }

        private string ToString(JsonStat stat) {
            return stat.Text?.Replace("{0}", stat.Param0?.ToString(System.Globalization.CultureInfo.InvariantCulture))
                .Replace("{1}", stat.Param1?.ToString(System.Globalization.CultureInfo.InvariantCulture))
                .Replace("{2}", stat.Param2?.ToString(System.Globalization.CultureInfo.InvariantCulture))
                .Replace("{3}", stat.Param3)
                .Replace("{4}", stat.Param4?.ToString(System.Globalization.CultureInfo.InvariantCulture))
                .Replace("{5}", stat.Param5)
                .Replace("{6}", stat.Param6);
        }

        bool Tick() {
            var items = _playerItemDao.ListWithMissingStatCache();
            if (items.Count <= 10) return true;

            Logger.Debug($"Updated cache for {items.Count} items");
            _itemStatService.ApplyStats(items);

            foreach (var item in items) {
                var rendered = ItemHtmlWriter.GetJsonItem(item);

                List<string> searchableText = new List<string>();
                searchableText.AddRange(rendered.HeaderStats.Select(ToString));
                searchableText.AddRange(rendered.BodyStats.Select(ToString));
                searchableText.AddRange(rendered.PetStats.Select(ToString));
                if (rendered.Skill != null) {
                    searchableText.AddRange(rendered.Skill.HeaderStats.Select(ToString).ToList());
                    searchableText.AddRange(rendered.Skill.BodyStats.Select(ToString).ToList());
                    searchableText.AddRange(rendered.Skill.PetStats.Select(ToString).ToList());
                    searchableText.Add(rendered.Skill.Name);
                    searchableText.Add(rendered.Skill.Description);
                    searchableText.Add(rendered.Skill.Trigger);
                }

                searchableText.Add(rendered.Extras);
                item.SearchableText = string.Join("\n", searchableText);

                if (string.IsNullOrEmpty(item.SearchableText)) {
                    if (string.IsNullOrEmpty(item.Name)) {
                        continue;
                    }
                    else {
                        // Typically potions and writs, they wont have any stats listed, so set a dummy value to prevent an infinite loop here.
                        item.SearchableText = "-";
                    }
                }
            }

            _playerItemDao.UpdateCachedStats(items);
            return true;
        }

        public void Dispose() {
            t?.Abort();
            t = null;
        }
    }
}