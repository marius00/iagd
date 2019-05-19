using IAGrim.Database.Interfaces;
using IAGrim.Utilities;
using IAGrim.Utilities.HelperClasses;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using IAGrim.Settings;
using IAGrim.Settings.Dto;

namespace IAGrim.UI.Tabs.Util
{
    class ModSelectionHandler : IDisposable
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ModSelectionHandler));
        private readonly SettingsService _settings;
        private long _numAvailableModFiltersLastUpdate = -1;
        private ComboBox _cbModFilter;
        private readonly Action _updateView;
        private readonly Action<string> _setStatus;
        private readonly IPlayerItemDao _playerItemDao;
        private string _lastMod;
        private bool _lastHardcoreSetting;

        public GDTransferFile SelectedMod { private set; get; }

        public ModSelectionHandler(ComboBox cbModFilter, IPlayerItemDao playerItemDao, Action updateView, Action<string> setStatus, SettingsService settings)
        {
            _cbModFilter = cbModFilter;
            _cbModFilter.DropDown += modFilter_DropDown;
            _cbModFilter.SelectedIndexChanged += cbModFilter_SelectedIndexChanged;
            _updateView = updateView;
            _playerItemDao = playerItemDao;
            _setStatus = setStatus;
            _settings = settings;

            SelectedMod = _cbModFilter.SelectedItem as GDTransferFile;
        }

        ~ModSelectionHandler()
        {
            Dispose();
        }

        private void cbModFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedMod = _cbModFilter.SelectedItem as GDTransferFile;
            _updateView();
            _settings.Save(LocalSetting.LastSelectedMod, SelectedMod);
        }

        private void UpdateModSelection(string mod, bool isHardcore)
        {
            if (mod != _lastMod || isHardcore != _lastHardcoreSetting)
            {
                var options = GetAvailableModSelection();

                _cbModFilter.Items.Clear();
                _cbModFilter.Items.AddRange(options);

                var query = options.Where(m => m.Mod.Equals(mod) && m.IsHardcore == isHardcore);

                if (query.Any())
                {
                    _cbModFilter.SelectedItem = query.FirstOrDefault();
                }
            }

            _lastMod = mod;
            _lastHardcoreSetting = isHardcore;
        }

        public void ConfigureModFilter()
        {
            _cbModFilter.DropDown += modFilter_DropDown;
            modFilter_DropDown(null, null);

            if (_cbModFilter.Items.Count == 0)
            {
                _setStatus(RuntimeSettings.Language.GetTag("iatag_stash_not_found"));
                Logger.Warn("Could not locate any stash files");
            }
            else
            {
                
                var lastSelectedMod = _settings.Get<GDTransferFile>(LocalSetting.LastSelectedMod);

                if (lastSelectedMod != null)
                {
                    _cbModFilter.SelectedIndex = _cbModFilter.Items.IndexOf(lastSelectedMod);
                }

                if (_cbModFilter.SelectedIndex == -1)
                {
                    foreach (var elem in _cbModFilter.Items)
                    {
                        if (elem is IComboBoxItemToggle item && item.Enabled)
                        {
                            _cbModFilter.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
        }

        public bool HasMods => GetAvailableModSelection().Any(m => !string.IsNullOrEmpty(m.Mod));

        public GDTransferFile[] GetAvailableModSelection()
        {
            var mods = GlobalPaths.TransferFiles;

            foreach (var entry in _playerItemDao.GetModSelection())
            {
                if (!mods.Any(m => m.IsHardcore == entry.IsHardcore && m.Mod?.ToLowerInvariant() == entry.Mod?.ToLowerInvariant()))
                {
                    mods.Add(new GDTransferFile
                    {
                        Mod = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(entry.Mod ?? string.Empty),
                        IsHardcore = entry.IsHardcore,
                        Enabled = true
                    });
                }
            }

            if (mods.Count != _numAvailableModFiltersLastUpdate)
            {
                List<GDTransferFile> result = new List<GDTransferFile>();
                foreach (var item in mods)
                {
                    item.Enabled = true;
                    result.Add(item);
                }

                mods = result;
            }

            _numAvailableModFiltersLastUpdate = mods.Count;

            return mods.ToArray();
        }

        public void Dispose()
        {
            if (_cbModFilter != null)
            {
                _cbModFilter.DropDown -= modFilter_DropDown;
                _cbModFilter = null;
            }
        }

        public void UpdateModSelection(string mod)
        {
            if (_cbModFilter.SelectedItem is GDTransferFile selected)
            {
                // Survival mode is the crucible, which shares items with campaign.
                UpdateModSelection(mod.Replace("survivalmode", ""), selected.IsHardcore);
            }
        }

        public void UpdateModSelection(bool isHardcore)
        {
            if (_cbModFilter.SelectedItem is GDTransferFile selected)
            {
                UpdateModSelection(selected.Mod, isHardcore);
            }
        }

        private void modFilter_DropDown(object sender, EventArgs e)
        {
            var entries = GetAvailableModSelection();

            if (entries.Length != _cbModFilter.Items.Count)
            {
                _cbModFilter.Items.Clear();
                _cbModFilter.Items.AddRange(entries.ToArray<object>());
            }
        }
    }
}
