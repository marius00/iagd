using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using IAGrim.Parsers.GameDataParsing.Model;
using IAGrim.Parsers.GameDataParsing.Service;
using IAGrim.Utilities;
using log4net;
using StatTranslator;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IAGrim.UI
{
    public partial class LanguagePackPicker : Form
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LanguagePackPicker));
        private readonly IEnumerable<string> _paths;
        private readonly List<FirefoxRadioButton> _checkboxes = new List<FirefoxRadioButton>();
        private readonly IItemTagDao _itemTagDao;
        private readonly IPlayerItemDao _playerItemDao;
        private readonly ParsingService _parsingService;

        public LanguagePackPicker(
            IItemTagDao itemTagDao,
            IPlayerItemDao playerItemDao,
            IEnumerable<string> paths,
            ParsingService parsingService
        )
        {
            InitializeComponent();

            _paths = paths;
            _parsingService = parsingService;
            _itemTagDao = itemTagDao;
            _playerItemDao = playerItemDao;

            LocalizationLoader.ApplyLanguage(Controls, GlobalSettings.Language);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                buttonSelect_Click(null, null);

                return true;
            }

            if (keyData == Keys.Escape)
            {
                Close();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            // find selected checkbox
            // if selected != Properties.Settings.Default.LocalizationFile
            var cb = _checkboxes.FirstOrDefault(m => m.Checked);
            if (cb != null)
            {
                var package = cb.Tag.ToString();
                if (package != Properties.Settings.Default.LocalizationFile)
                {
                    Properties.Settings.Default.LocalizationFile = package;
                    Properties.Settings.Default.Save();

                    if (!string.IsNullOrEmpty(Properties.Settings.Default.LocalizationFile))
                    {
                        var loader = new LocalizationLoader();
                        GlobalSettings.Language = loader.LoadLanguage(package, new EnglishLanguage(new Dictionary<string, string>()));

                        // TODO: Grab tags from loader and save to sql
                        _itemTagDao.Save(loader.GetItemTags(), new ProgressTracker());

                        var x = new UpdatingPlayerItemsScreen(_playerItemDao);
                        x.ShowDialog();
                    }
                    // Load the GD one
                    else
                    {
                        // Override timestamp to force an update
                        GlobalSettings.InitializeLanguage(string.Empty, new Dictionary<string, string>()); // TODO: Not ideal, will need a restart

                        foreach (var location in _paths)
                        {
                            if (!string.IsNullOrEmpty(location) && Directory.Exists(location))
                            {
                                _parsingService.Update(location, string.Empty);
                                _parsingService.Execute();
                                break;
                            }

                            Logger.Warn("Could not find the Grim Dawn install location");

                        }

                        // Update item stats as well
                        var x = new UpdatingPlayerItemsScreen(_playerItemDao);
                        x.ShowDialog();
                    }
                }
            }
            Close();
        }

        private void LanguagePackPicker_Load(object sender, EventArgs e)
        {
            var loc = new LocalizationLoader();

            var n = 0;

            // Default language: English
            {
                var cb = new FirefoxRadioButton
                {
                    Location = new Point(10, 25 + n * 33),
                    Text = "English (Official)",
                    Tag = string.Empty,
                    Checked = string.IsNullOrEmpty(Properties.Settings.Default.LocalizationFile),
                    TabIndex = n,
                    TabStop = true
                };

                groupBox1.Controls.Add(cb);
                _checkboxes.Add(cb);
                n++;
            }

            foreach (var path in _paths)
            {
                if (Directory.Exists(Path.Combine(path, "localization")))
                {
                    foreach (var file in Directory.EnumerateFiles(Path.Combine(path, "localization"), "*.zip"))
                    {
                        loc.CheckLanguage(file, out var author, out var language);

                        var cb = new FirefoxRadioButton
                        {
                            Location = new Point(10, 25 + n * 33),
                            Text = GlobalSettings.Language.GetTag("iatag_ui_language_by_author", language, author),
                            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                            Width = groupBox1.Width - pictureBox1.Width,
                            Tag = file,
                            Checked = file == Properties.Settings.Default.LocalizationFile,
                            TabIndex = n,
                            TabStop = true
                        };

                        groupBox1.Controls.Add(cb);
                        _checkboxes.Add(cb);
                        n++;
                    }
                }
            }

            Height += n * 33;
        }

        private void LanguagePackPicker_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.MainWindow?.UpdateLanguage();
        }
    }
}
