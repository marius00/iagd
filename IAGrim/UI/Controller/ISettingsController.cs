﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.UI.Controller {
    [Obsolete]
    interface ISettingsController {
        bool MinimizeToTray { get; set; }
        bool ShowRecipesAsItems { get; set; }

        void LoadDefaults();

        void BindCheckbox(FirefoxCheckBox control);

        void BindCheckbox(System.Windows.Forms.Control control, string property);

        void BindText(System.Windows.Forms.Control control);

        void BindText(System.Windows.Forms.Control control, string property);

        void DonateNow();

        void OpenDataFolder();


        void OpenLogFolder();
    }
}
