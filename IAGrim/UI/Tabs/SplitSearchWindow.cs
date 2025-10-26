using IAGrim.Database.Dto;
using IAGrim.Database.Interfaces;
using IAGrim.Parsers.Arz;
using IAGrim.Settings;
using IAGrim.Theme;
using IAGrim.UI.Controller;
using IAGrim.UI.Misc.CEF;
using IAGrim.UI.Tabs.Util;
using IAGrim.Utilities;
using log4net;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace IAGrim.UI.Tabs {
    internal sealed class SplitSearchWindow : Form {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SplitSearchWindow));
        private readonly Action<string> _setStatus;
        private readonly SearchController _searchController;
        private readonly IItemTagDao _itemTagDao;
        private System.Windows.Forms.Timer _delayedTextChangedTimer;
        private DesiredSkills _filterWindow;
        private TextBox _searchBox;
        private CheckBox _orderByLevel;
        private ComboBox _modFilter;
        private ComboBox _slotFilter;
        private SplitContainer _mainSplitter;
        private ComboBox _itemQuality;
        private ComboBoxItem _selectedSlot;
        private TextBox _minLevel;
        private TextBox _maxLevel;
        private FlowLayoutPanel _flowPanelFilter;
        private GroupBox _levelRequirementGroup;
        private ComboBoxItemQuality _selectedItemQuality;
        private ScrollPanelMessageFilter _scrollableFilterView;
        private ToolStripContainer _toolStripContainer;
        private readonly SettingsService _settings;
        private ToolTip toolTip1;
        private System.ComponentModel.IContainer components;
        private const int FilterPanelMinSize = 250;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private bool _hasCheckedModFilterNotEmpty = false;

        /// <summary>
        /// ModSelectionHandler
        /// </summary>
        public ModSelectionHandler ModSelectionHandler { get; }
        public WebView2 Browser => this.webView21;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="setStatus"></param>
        /// <param name="playerItemDao"></param>
        /// <param name="searchController"></param>
        /// <param name="itemTagDao"></param>
        public SplitSearchWindow(Microsoft.Web.WebView2.WinForms.WebView2 browser,
            Action<string> setStatus,
            IPlayerItemDao playerItemDao,
            SearchController searchController,
            IItemTagDao itemTagDao, SettingsService settings) {
            _setStatus = setStatus;
            _searchController = searchController;
            _itemTagDao = itemTagDao;
            _settings = settings;
            InitializeComponent();

            Dock = DockStyle.Fill;

            _mainSplitter.SplitterDistance = FilterPanelMinSize;
            _mainSplitter.SplitterWidth = 5;
            _mainSplitter.BorderStyle = BorderStyle.None;
            _mainSplitter.SplitterMoved += MainSplitterOnSplitterMoved;

            ModSelectionHandler = new ModSelectionHandler(_modFilter, playerItemDao, UpdateListViewDelayed, setStatus, _settings);

            _toolStripContainer.ContentPanel.Controls.Add(browser);

            Activated += SplitSearchWindow_Activated;
            Deactivate += SplitSearchWindow_Deactivate;
            var conf = CoreWebView2Environment.CreateAsync(null, GlobalPaths.EdgeCacheLocation).Result;
            webView21.EnsureCoreWebView2Async(conf);
            webView21.Source = new Uri(GetSiteUri());

            InitializeFilterPanel();
        }


        private string GetSiteUri() {
#if DEBUG
            /*
            var client = new WebClient();

            try {
                Logger.Debug("Checking if NodeJS is running...");
                client.DownloadString("http://localhost:3000/");
                Logger.Debug("NodeJS running");
                return "http://localhost:3000/";
            }
            catch (System.Net.WebException) {
                Logger.Debug("NodeJS not running, defaulting to standard view");
            }*/
#endif
            return GlobalPaths.ItemsHtmlFile;
        }

        public void SelectModFilterIfNotSelected() {
            if (!_hasCheckedModFilterNotEmpty) {
                if (InvokeRequired) {
                    Invoke((MethodInvoker)delegate {
                        this.SelectModFilterIfNotSelected();
                    });
                }
                else {
                    // Should only happen for the very first item ever looted, but it's a fairly poor user experience when it does happen.
                    if (_modFilter.SelectedItem == null) {
                        ModSelectionHandler.ConfigureModFilter();
                    }

                    if (_modFilter.SelectedItem == null && _modFilter.Items.Count > 0) {
                        _modFilter.SelectedItem = _modFilter.Items[0];
                        _hasCheckedModFilterNotEmpty = true;
                    }
                }

            }
        }

        /// <summary>
        /// Clear all filters
        /// </summary>
        public void ClearFilters() {
            _filterWindow.ClearFilters();
            _searchBox.Text = string.Empty;
            _itemQuality.SelectedIndex = 0;
            _slotFilter.SelectedIndex = 0;
            _minLevel.Text = "0";
            _maxLevel.Text = "110";

            UpdateListViewDelayed();
        }

        /// <summary>
        /// Update interface
        /// </summary>
        public void UpdateInterface() {
            InitializeFilterPanel();
        }

        /// <summary>
        /// Update view
        /// </summary>
        public void UpdateListView() {
            if (InvokeRequired) {
                Invoke((MethodInvoker)delegate { UpdateListView(_filterWindow.Filters); });
            }
            else {
                UpdateListView(_filterWindow.Filters);
            }
        }

        /// <summary>
        /// Update view
        /// </summary>
        /// <param name="filters">Filters</param>
        public void UpdateListView(FilterEventArgs filters) {
            var transferFile = ModSelectionHandler.SelectedMod;

            if (transferFile == null) {
                Logger.Warn("Attempting to update item view, but no mod selection has been made");
                return;
            }

            var rarity = _selectedItemQuality;
            var slot = _selectedSlot;
            var query = new ItemSearchRequest {
                Wildcard = _searchBox.Text,
                Filters = filters.Filters,
                MinimumLevel = ParseNumeric(_minLevel),
                MaximumLevel = ParseNumeric(_maxLevel),
                Rarity = rarity?.Rarity,
                PrefixRarity = rarity?.PrefixRarity ?? 0,
                Slot = slot?.Filter,
                SlotInverse = slot?.Inverse ?? false,
                PetBonuses = filters.PetBonuses,
                IsRetaliation = filters.IsRetaliation,
                DuplicatesOnly = filters.DuplicatesOnly,
                Mod = transferFile.Mod,
                IsHardcore = transferFile.IsHardcore,
                Classes = filters.DesiredClass,
                SocketedOnly = filters.SocketedOnly,
                RecentOnly = filters.RecentOnly,
                WithGrantSkillsOnly = filters.GrantsSkill,
                WithSummonerSkillOnly = filters.WithSummonerSkillOnly
            };


            bool includeBuddyItems = !filters.DuplicatesOnly; // If we're looking for duplicates, we're probably doing a cleanup, not caring about buddyitems
            var message = _searchController.Search(query, includeBuddyItems, _orderByLevel.Checked);

            Logger.Info("Updating UI...");

            if (!string.IsNullOrEmpty(message)) {
                _setStatus(message);
            }

            Logger.Info("Done");
        }

        /// <summary>
        /// Update view with delay
        /// </summary>
        public void UpdateListViewDelayed() {
            UpdateListViewDelayed(_settings.GetLocal().PreferDelayedSearch ? 200 : 0);
        }

        private void BeginSearchOnAutoSearch(object sender, EventArgs e) {
            UpdateListViewDelayed();
        }

        private void HandleDelayedTextChangedTimerTick(object? sender, EventArgs e) {
            if (_delayedTextChangedTimer != null) {
                _delayedTextChangedTimer.Stop();
                _delayedTextChangedTimer = null;
            }

            if (InvokeRequired) {
                Invoke((MethodInvoker) delegate { UpdateListView(_filterWindow.Filters); });
            }
            else {
                UpdateListView(_filterWindow.Filters);
            }
        }

        private void InitializeFilterPanel() {
            if (_filterWindow != null) {
                _filterWindow.OnChanged -= BeginSearchOnAutoSearch;
                _filterWindow.Close();
                _mainSplitter.Panel1.Controls.Remove(_filterWindow);
            }

            _filterWindow = new DesiredSkills(_itemTagDao) {
                TopLevel = false
            };
            _filterWindow.OnChanged += BeginSearchOnAutoSearch;
            _mainSplitter.Panel1.Controls.Add(_filterWindow);
            _filterWindow.Show();
        }

        private void MainSplitterOnSplitterMoved(object sender, SplitterEventArgs e) {
            if (_mainSplitter.SplitterDistance < FilterPanelMinSize) {
                _mainSplitter.SplitterDistance = FilterPanelMinSize;
            }
        }

        private void MaxLevel_MouseWheel(object sender, MouseEventArgs e) {
            if (sender is Control c) {
                if (e.Delta > 0) {
                    c.Text = Math.Min(ParseNumeric(c) + 1, 110).ToString();
                }
                else if (e.Delta < 0) {
                    var newValue = Math.Min(Math.Max(0, ParseNumeric(c) - 1), 110);
                    if (ParseNumeric(_minLevel) > newValue) {
                        _minLevel.Text = newValue.ToString();
                    }

                    c.Text = Math.Min(Math.Max(0, ParseNumeric(c) - 1), 110).ToString();
                }
            }

            UpdateListViewDelayed(1200);
        }

        private void MinLevel_MouseWheel(object sender, MouseEventArgs e) {
            if (sender is Control c) {
                if (e.Delta > 0) {
                    var newValue = Math.Min(ParseNumeric(c) + 1, 110);
                    if (ParseNumeric(_maxLevel) < newValue) {
                        _maxLevel.Text = newValue.ToString();
                    }

                    c.Text = newValue.ToString();
                }
                else if (e.Delta < 0) {
                    c.Text = Math.Min(Math.Max(0, ParseNumeric(c) - 1), 110).ToString();
                }
            }

            UpdateListViewDelayed(1200);
        }

        private void MinLevel_KeyPress(object sender, KeyPressEventArgs e) {
            e.Handled = !(char.IsDigit(e.KeyChar) || ParseNumeric(_minLevel) > 105 || e.KeyChar == '\b');
            UpdateListViewDelayed(1200);
        }

        private int ParseNumeric(Control tb) {
            return int.TryParse(tb.Text, out var val) ? val : 0;
        }

        private void SplitSearchWindow_Load(object sender, EventArgs e) {
            ModSelectionHandler.ConfigureModFilter();

            _minLevel.KeyPress += MinLevel_KeyPress;
            _minLevel.MouseWheel += MinLevel_MouseWheel;

            _maxLevel.KeyPress += MinLevel_KeyPress;
            _maxLevel.MouseWheel += MaxLevel_MouseWheel;

            _itemQuality.Items.AddRange(UIHelper.QualityFilter.ToArray<object>());
            _itemQuality.SelectedIndex = 0;
            _selectedItemQuality = _itemQuality.SelectedItem as ComboBoxItemQuality;
            _itemQuality.SelectedIndexChanged += (s, ev) => { _selectedItemQuality = _itemQuality.SelectedItem as ComboBoxItemQuality; };
            _itemQuality.SelectedIndexChanged += BeginSearchOnAutoSearch;

            _slotFilter.Items.AddRange(UIHelper.SlotFilter.ToArray<object>());
            _slotFilter.SelectedIndex = 0;
            _selectedSlot = _slotFilter.SelectedItem as ComboBoxItem;
            _slotFilter.SelectedIndexChanged += (s, ev) => { _selectedSlot = _slotFilter.SelectedItem as ComboBoxItem; };
            _slotFilter.SelectedIndexChanged += BeginSearchOnAutoSearch;

            FormClosing += SplitSearchWindow_FormClosing;

            _searchBox.TextChanged += SearchBox_TextChanged;

            _orderByLevel.CheckStateChanged += delegate { UpdateListViewDelayed(); };

            _flowPanelFilter.SizeChanged += FlowPanelFilter_Resize;
            _mainSplitter.SizeChanged += FlowPanelFilter_Resize;

            LocalizationLoader.ApplyTooltipLanguage(toolTip1, Controls, RuntimeSettings.Language);
        }

        private void SearchBox_TextChanged(object sender, EventArgs e) {
            UpdateListViewDelayed(600);
        }

        private void SplitSearchWindow_Activated(object sender, EventArgs e) {
            _scrollableFilterView = new ScrollPanelMessageFilter(_filterWindow);
            Application.AddMessageFilter(_scrollableFilterView);
        }

        private void SplitSearchWindow_Deactivate(object sender, EventArgs e) {
            Application.RemoveMessageFilter(_scrollableFilterView);
        }

        private void SplitSearchWindow_FormClosing(object sender, FormClosingEventArgs e) {
            _filterWindow.OnChanged -= BeginSearchOnAutoSearch;
            Activated -= SplitSearchWindow_Activated;
            Deactivate -= SplitSearchWindow_Deactivate;

            _toolStripContainer.ContentPanel.Controls.Clear();
        }

        private void UpdateListViewDelayed(int delay) {
            _delayedTextChangedTimer?.Stop();

            if (delay > 0) {
                _delayedTextChangedTimer = new System.Windows.Forms.Timer();
                _delayedTextChangedTimer.Tick += HandleDelayedTextChangedTimerTick;
                _delayedTextChangedTimer.Interval = delay;
                _delayedTextChangedTimer.Start();
            }
            else {
                HandleDelayedTextChangedTimerTick(this, EventArgs.Empty);
            }
        }

        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
            _mainSplitter = new SplitContainer();
            _toolStripContainer = new ToolStripContainer();
            webView21 = new WebView2();
            _flowPanelFilter = new FlowLayoutPanel();
            _searchBox = new TextBox();
            _orderByLevel = new CheckBox();
            _itemQuality = new ComboBox();
            _slotFilter = new ComboBox();
            _modFilter = new ComboBox();
            _levelRequirementGroup = new GroupBox();
            _minLevel = new TextBox();
            _maxLevel = new TextBox();
            toolTip1 = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)_mainSplitter).BeginInit();
            _mainSplitter.Panel2.SuspendLayout();
            _mainSplitter.SuspendLayout();
            _toolStripContainer.ContentPanel.SuspendLayout();
            _toolStripContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
            _flowPanelFilter.SuspendLayout();
            _levelRequirementGroup.SuspendLayout();
            SuspendLayout();
            // 
            // _mainSplitter
            // 
            _mainSplitter.Dock = DockStyle.Fill;
            _mainSplitter.Location = new Point(0, 0);
            _mainSplitter.Margin = new Padding(6, 5, 6, 5);
            _mainSplitter.Name = "_mainSplitter";
            // 
            // _mainSplitter.Panel2
            // 
            _mainSplitter.Panel2.Controls.Add(_toolStripContainer);
            _mainSplitter.Panel2.Controls.Add(_flowPanelFilter);
            _mainSplitter.Size = new Size(2189, 1250);
            _mainSplitter.SplitterDistance = 340;
            _mainSplitter.SplitterWidth = 6;
            _mainSplitter.TabIndex = 0;
            _mainSplitter.TabStop = false;
            // 
            // _toolStripContainer
            // 
            _toolStripContainer.BottomToolStripPanelVisible = false;
            // 
            // _toolStripContainer.ContentPanel
            // 
            _toolStripContainer.ContentPanel.Controls.Add(webView21);
            _toolStripContainer.ContentPanel.Margin = new Padding(6, 5, 6, 5);
            _toolStripContainer.ContentPanel.Size = new Size(1843, 1132);
            _toolStripContainer.Dock = DockStyle.Fill;
            _toolStripContainer.LeftToolStripPanelVisible = false;
            _toolStripContainer.Location = new Point(0, 93);
            _toolStripContainer.Margin = new Padding(6, 5, 6, 5);
            _toolStripContainer.Name = "_toolStripContainer";
            _toolStripContainer.RightToolStripPanelVisible = false;
            _toolStripContainer.Size = new Size(1843, 1157);
            _toolStripContainer.TabIndex = 48;
            _toolStripContainer.Text = "toolStripContainer1";
            // 
            // webView21
            // 
            webView21.AllowExternalDrop = true;
            webView21.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = Color.White;
            webView21.Location = new Point(6, 5);
            webView21.Margin = new Padding(6, 5, 6, 5);
            webView21.Name = "webView21";
            webView21.Size = new Size(1831, 1122);
            webView21.TabIndex = 0;
            webView21.ZoomFactor = 1D;
            // 
            // _flowPanelFilter
            // 
            _flowPanelFilter.AutoSize = true;
            _flowPanelFilter.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _flowPanelFilter.Controls.Add(_searchBox);
            _flowPanelFilter.Controls.Add(_orderByLevel);
            _flowPanelFilter.Controls.Add(_itemQuality);
            _flowPanelFilter.Controls.Add(_slotFilter);
            _flowPanelFilter.Controls.Add(_modFilter);
            _flowPanelFilter.Controls.Add(_levelRequirementGroup);
            _flowPanelFilter.Dock = DockStyle.Top;
            _flowPanelFilter.Location = new Point(0, 0);
            _flowPanelFilter.Margin = new Padding(6, 5, 6, 5);
            _flowPanelFilter.Name = "_flowPanelFilter";
            _flowPanelFilter.Size = new Size(1843, 93);
            _flowPanelFilter.TabIndex = 52;
            // 
            // _searchBox
            // 
            _searchBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _searchBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            _searchBox.AutoCompleteSource = AutoCompleteSource.RecentlyUsedList;
            _searchBox.Location = new Point(6, 33);
            _searchBox.Margin = new Padding(6, 33, 6, 5);
            _searchBox.MaximumSize = new Size(730, 0);
            _searchBox.MaxLength = 255;
            _searchBox.MinimumSize = new Size(0, 8);
            _searchBox.Name = "_searchBox";
            _searchBox.Size = new Size(177, 31);
            _searchBox.TabIndex = 41;
            _searchBox.Tag = "iatag_ui_searchbox_tooltip";
            toolTip1.SetToolTip(_searchBox, "The item name, partially works fine.");
            // 
            // _orderByLevel
            // 
            _orderByLevel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _orderByLevel.AutoSize = true;
            _orderByLevel.Checked = true;
            _orderByLevel.CheckState = CheckState.Checked;
            _orderByLevel.Location = new Point(195, 37);
            _orderByLevel.Margin = new Padding(6, 37, 6, 5);
            _orderByLevel.Name = "_orderByLevel";
            _orderByLevel.Size = new Size(152, 29);
            _orderByLevel.TabIndex = 42;
            _orderByLevel.Tag = "iatag_ui_orderbylevel";
            _orderByLevel.Text = "Order By Level";
            toolTip1.SetToolTip(_orderByLevel, "If items should be ordered by level, instead of alphabetically.");
            _orderByLevel.UseVisualStyleBackColor = true;
            // 
            // _itemQuality
            // 
            _itemQuality.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _itemQuality.DropDownStyle = ComboBoxStyle.DropDownList;
            _itemQuality.FormattingEnabled = true;
            _itemQuality.Location = new Point(359, 33);
            _itemQuality.Margin = new Padding(6, 33, 6, 5);
            _itemQuality.Name = "_itemQuality";
            _itemQuality.Size = new Size(147, 33);
            _itemQuality.TabIndex = 43;
            _itemQuality.Tag = "iatag_ui_itemquality_tooltip";
            toolTip1.SetToolTip(_itemQuality, "The minimum item quality");
            // 
            // _slotFilter
            // 
            _slotFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _slotFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            _slotFilter.FormattingEnabled = true;
            _slotFilter.Location = new Point(518, 33);
            _slotFilter.Margin = new Padding(6, 33, 6, 5);
            _slotFilter.Name = "_slotFilter";
            _slotFilter.Size = new Size(197, 33);
            _slotFilter.TabIndex = 44;
            _slotFilter.Tag = "iatag_ui_slotfilter_tooltip";
            toolTip1.SetToolTip(_slotFilter, "Slot/Type");
            // 
            // _modFilter
            // 
            _modFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _modFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            _modFilter.FormattingEnabled = true;
            _modFilter.Location = new Point(727, 33);
            _modFilter.Margin = new Padding(6, 33, 6, 5);
            _modFilter.Name = "_modFilter";
            _modFilter.Size = new Size(167, 33);
            _modFilter.TabIndex = 45;
            _modFilter.Tag = "iatag_ui_modfilter_tooltip";
            toolTip1.SetToolTip(_modFilter, "Mod / Hardcore / Vanilla");
            // 
            // _levelRequirementGroup
            // 
            _levelRequirementGroup.Controls.Add(_minLevel);
            _levelRequirementGroup.Controls.Add(_maxLevel);
            _levelRequirementGroup.Location = new Point(906, 5);
            _levelRequirementGroup.Margin = new Padding(6, 5, 6, 5);
            _levelRequirementGroup.Name = "_levelRequirementGroup";
            _levelRequirementGroup.Padding = new Padding(6, 5, 6, 5);
            _levelRequirementGroup.Size = new Size(130, 83);
            _levelRequirementGroup.TabIndex = 50;
            _levelRequirementGroup.TabStop = false;
            _levelRequirementGroup.Tag = "iatag_ui_level_requirement";
            _levelRequirementGroup.Text = "Level";
            toolTip1.SetToolTip(_levelRequirementGroup, "Level requirements for the item");
            // 
            // _minLevel
            // 
            _minLevel.Location = new Point(9, 28);
            _minLevel.Margin = new Padding(6, 5, 6, 5);
            _minLevel.MaxLength = 3;
            _minLevel.Name = "_minLevel";
            _minLevel.Size = new Size(47, 31);
            _minLevel.TabIndex = 46;
            _minLevel.Tag = "iatag_ui_minlevel_tooltip";
            _minLevel.Text = "0";
            _minLevel.TextAlign = HorizontalAlignment.Center;
            toolTip1.SetToolTip(_minLevel, "The minimum level required to use this item");
            _minLevel.WordWrap = false;
            // 
            // _maxLevel
            // 
            _maxLevel.Location = new Point(67, 28);
            _maxLevel.Margin = new Padding(6, 5, 6, 5);
            _maxLevel.MaxLength = 3;
            _maxLevel.Name = "_maxLevel";
            _maxLevel.Size = new Size(47, 31);
            _maxLevel.TabIndex = 47;
            _maxLevel.Tag = "iatag_ui_maxlevel_tooltip";
            _maxLevel.Text = "110";
            _maxLevel.TextAlign = HorizontalAlignment.Center;
            toolTip1.SetToolTip(_maxLevel, "The maximum level required to use this item");
            _maxLevel.WordWrap = false;
            // 
            // toolTip1
            // 
            toolTip1.ToolTipTitle = "This is:";
            // 
            // SplitSearchWindow
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2189, 1250);
            Controls.Add(_mainSplitter);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(6, 5, 6, 5);
            Name = "SplitSearchWindow";
            Text = "SearchWindow";
            Load += SplitSearchWindow_Load;
            _mainSplitter.Panel2.ResumeLayout(false);
            _mainSplitter.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_mainSplitter).EndInit();
            _mainSplitter.ResumeLayout(false);
            _toolStripContainer.ContentPanel.ResumeLayout(false);
            _toolStripContainer.ResumeLayout(false);
            _toolStripContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            _flowPanelFilter.ResumeLayout(false);
            _flowPanelFilter.PerformLayout();
            _levelRequirementGroup.ResumeLayout(false);
            _levelRequirementGroup.PerformLayout();
            ResumeLayout(false);

        }

        private void FlowPanelFilter_Resize(object sender, EventArgs e) {
            _searchBox.Width = Math.Max(300, _flowPanelFilter.Width - 500);
            _searchBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }
    }
}