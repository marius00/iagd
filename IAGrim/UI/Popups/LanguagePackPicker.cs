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

namespace IAGrim.UI {
    public partial class LanguagePackPicker : Form {
        private static ILog logger = LogManager.GetLogger(typeof(LanguagePackPicker));
        private readonly string path;
        private List<FirefoxRadioButton> Checkboxes = new List<FirefoxRadioButton>();
        private readonly IDatabaseItemDao databaseItemDao;
        private readonly IDatabaseSettingDao databaseSettingDao;
        private readonly IPlayerItemDao playerItemDao;
        private readonly ArzParser parser;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Path to Grim Dawn install</param>
        public LanguagePackPicker(
                IDatabaseItemDao databaseItemDao, 
                IDatabaseSettingDao databaseSettingDao,
                IPlayerItemDao playerItemDao,
                ArzParser parser, 
                string path
            
            ) {
            InitializeComponent();
            this.path = path;
            this.databaseItemDao = databaseItemDao;
            this.databaseSettingDao = databaseSettingDao;
            this.playerItemDao = playerItemDao;
            this.parser = parser;

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
            var cb = Checkboxes.Where(m => m.Checked).FirstOrDefault();
            if (cb != null) {
                var package = cb.Tag.ToString();
                if (package != Properties.Settings.Default.LocalizationFile) {
                    Properties.Settings.Default.LocalizationFile = package;
                    Properties.Settings.Default.Save();

                    if (!string.IsNullOrEmpty(Properties.Settings.Default.LocalizationFile)) {
                        var loader = new LocalizationLoader();
                        GlobalSettings.Language = loader.LoadLanguage(package);

                        // TODO: Grab tags from loader and save to sql
                        databaseItemDao.Save(loader.GetItemTags());

                        UpdatingPlayerItemsScreen x = new UpdatingPlayerItemsScreen(playerItemDao);
                        x.ShowDialog();
                    }
                    // Load the GD one
                    else {
                        // Override timestamp to force an update
                        GlobalSettings.Language = new EnglishLanguage();

                        string location = GrimDawnDetector.GetGrimLocation();
                        if (!string.IsNullOrEmpty(location) && Directory.Exists(location)) {
                            ParsingDatabaseScreen parserUi = new ParsingDatabaseScreen(databaseSettingDao, this.parser, location, string.Empty, true, true);
                            parserUi.ShowDialog();
                        }
                        else {
                            logger.Warn("Could not find the Grim Dawn install location");
                        }

                        // Update item stats as well
                        UpdatingPlayerItemsScreen x = new UpdatingPlayerItemsScreen(playerItemDao);
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
                Checkboxes.Add(cb);
                n++;
            }

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
                    Checkboxes.Add(cb);
                    n++;
                }
            }
            

            this.Height += n * 33;
        }
    }
}
