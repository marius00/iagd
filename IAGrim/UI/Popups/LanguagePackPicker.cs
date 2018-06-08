using IAGrim.Database;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using IAGrim.Utilities;
using log4net;
using StatTranslator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IAGrim.Parsers.GameDataParsing.Model;
using IAGrim.Parsers.GameDataParsing.Service;

namespace IAGrim.UI {
    public partial class LanguagePackPicker : Form {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LanguagePackPicker));
        private readonly IEnumerable<string> _paths;
        private readonly List<FirefoxRadioButton> _checkboxes = new List<FirefoxRadioButton>();
        private readonly IItemTagDao _itemTagDao;
        private readonly IDatabaseSettingDao _databaseSettingDao;
        private readonly IPlayerItemDao _playerItemDao;
        private readonly ArzParser _parser;
        private readonly ParsingService _parsingService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Path to Grim Dawn install</param>
        public LanguagePackPicker(
            IItemTagDao itemTagDao, 
            IDatabaseSettingDao databaseSettingDao,
            IPlayerItemDao playerItemDao,
            ArzParser parser,
            IEnumerable<string> paths, 
            ParsingService parsingService
        ) {
            InitializeComponent();
            this._paths = paths;
            _parsingService = parsingService;
            this._itemTagDao = itemTagDao;
            this._databaseSettingDao = databaseSettingDao;
            this._playerItemDao = playerItemDao;
            this._parser = parser;

            var buttonTag = GlobalSettings.Language.GetTag("iatag_ui_lang_button_change_language");
            if (!string.IsNullOrEmpty(buttonTag)) {
                buttonSelect.Text += $" ({buttonTag})";
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Enter) {
                buttonSelect_Click(null, null);

                return true;
            }
            else if (keyData == Keys.Escape)
                this.Close();

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void buttonSelect_Click(object sender, EventArgs e) {
            // find selected checkbox
            // if selected != Properties.Settings.Default.LocalizationFile
            var cb = _checkboxes.FirstOrDefault(m => m.Checked);
            if (cb != null) {
                var package = cb.Tag.ToString();
                if (package != Properties.Settings.Default.LocalizationFile) {
                    Properties.Settings.Default.LocalizationFile = package;
                    Properties.Settings.Default.Save();

                    if (!string.IsNullOrEmpty(Properties.Settings.Default.LocalizationFile)) {
                        var loader = new LocalizationLoader();
                        GlobalSettings.Language = loader.LoadLanguage(package);

                        // TODO: Grab tags from loader and save to sql
                        _itemTagDao.Save(loader.GetItemTags(), new ProgressTracker());

                        UpdatingPlayerItemsScreen x = new UpdatingPlayerItemsScreen(_playerItemDao);
                        x.ShowDialog();
                    }
                    // Load the GD one
                    else {
                        // Override timestamp to force an update
                        GlobalSettings.Language = new EnglishLanguage();

                        foreach (var location in _paths) {
                            if (!string.IsNullOrEmpty(location) && Directory.Exists(location)) {
                                _parsingService.Update(location, string.Empty);
                                _parsingService.Execute();
                                break;
                            }
                            else {
                                Logger.Warn("Could not find the Grim Dawn install location");
                            }

                        }

                        // Update item stats as well
                        UpdatingPlayerItemsScreen x = new UpdatingPlayerItemsScreen(_playerItemDao);
                        x.ShowDialog();

                    }
                }
            }
            this.Close();
        }

        private void LanguagePackPicker_Load(object sender, EventArgs e) {
            LocalizationLoader loc = new LocalizationLoader();

            int n = 0;

            // Default language: English
            {
                FirefoxRadioButton cb = new FirefoxRadioButton {
                    Location = new Point(10, 25 + n * 33),
                    Text = "English (Official)",
                    Tag = string.Empty,
                    Checked = string.IsNullOrEmpty(Properties.Settings.Default.LocalizationFile)
                };

                cb.TabIndex = n;
                cb.TabStop = true;
                groupBox1.Controls.Add(cb);
                _checkboxes.Add(cb);
                n++;
            }

            foreach (var path in _paths) {
                if (Directory.Exists(Path.Combine(path, "localization"))) {
                    foreach (var file in Directory.EnumerateFiles(Path.Combine(path, "localization"), "*.zip")) {
                        string author;
                        string language;
                        loc.CheckLanguage(file, out author, out language);

                        FirefoxRadioButton cb = new FirefoxRadioButton {
                            Location = new Point(10, 25 + n * 33),
                            Text = string.Format("{0} by {1}", language, author),
                            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                            Width = groupBox1.Width - pictureBox1.Width,
                            Tag = file,
                            Checked = file == Properties.Settings.Default.LocalizationFile
                        };

                        cb.TabIndex = n;
                        cb.TabStop = true;
                        groupBox1.Controls.Add(cb);
                        _checkboxes.Add(cb);
                        n++;
                    }
                }
            }

            this.Height += n * 33;
        }
    }
}
