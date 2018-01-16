using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IAGrim.Parsers.GameDataParsing.UI {
    public partial class ParsingDatabaseProgressView : Form {
        public ProgressBar LoadingTags => progressLoadingTags;
        public ProgressBar SavingTags => progressSavingTags;
        public ProgressBar LoadingItems => progressLoadingItems;
        public ProgressBar MappingItemNames => progressMappingItemNames;
        public ProgressBar MappingPetStats => progressMappingPetStats;
        public ProgressBar SavingItems => progressSaveItems;
        public ProgressBar GeneratingSpecialStats => progressGeneratingSpecialStats;
        public ProgressBar SavingSpecialStats => progressSavingSpecialStats;
        
        public ProgressBar GeneratingSkills => progressGeneratingSkills;
        public ProgressBar SkillCorrectnessCheck => progressSkillCorrectnessCheck;

        private bool closePermitted = false;
        public void OverrideClose() {
            closePermitted = true;
            Close();
        }

        public ParsingDatabaseProgressView() {
            InitializeComponent();
            this.FormClosing += OnFormClosing;
        }

        private void OnFormClosing(object sender, FormClosingEventArgs formClosingEventArgs) {
            if (formClosingEventArgs.CloseReason == CloseReason.UserClosing) {
                formClosingEventArgs.Cancel = !closePermitted;
            } else {
                formClosingEventArgs.Cancel = closePermitted;
            }
        }
    }
}
