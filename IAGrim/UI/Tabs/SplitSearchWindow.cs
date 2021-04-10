using IAGrim.Database.Dto;
using IAGrim.Database.Interfaces;
using IAGrim.Theme;
using IAGrim.UI.Controller;
using IAGrim.UI.Tabs.Util;
using log4net;
using System;
using System.Linq;
using System.Windows.Forms;
using IAGrim.Settings;
using IAGrim.Settings.Dto;

namespace IAGrim.UI.Tabs {
    internal sealed class SplitSearchWindow : Form {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SplitSearchWindow));
        private readonly Action<string> _setStatus;
        private readonly SearchController _searchController;
        private readonly IItemTagDao _itemTagDao;
        private Timer _delayedTextChangedTimer;
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

        /// <summary>
        /// ModSelectionHandler
        /// </summary>
        public ModSelectionHandler ModSelectionHandler { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="setStatus"></param>
        /// <param name="playerItemDao"></param>
        /// <param name="searchController"></param>
        /// <param name="itemTagDao"></param>
        public SplitSearchWindow(Control browser,
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

            InitializeFilterPanel();
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
            UpdateListView(_filterWindow.Filters);
        }

        /// <summary>
        /// Update view
        /// </summary>
        /// <param name="filters">Filters</param>
        public void UpdateListView(FilterEventArgs filters) {
            var transferFile = ModSelectionHandler.SelectedMod;

            if (transferFile == null) {
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
                PetBonuses = filters.PetBonuses,
                IsRetaliation = filters.IsRetaliation,
                Mod = transferFile.Mod,
                IsHardcore = transferFile.IsHardcore,
                Classes = filters.DesiredClass,
                SocketedOnly = filters.SocketedOnly,
                RecentOnly = filters.RecentOnly,
                WithGrantSkillsOnly = filters.GrantsSkill,
                WithSummonerSkillOnly = filters.WithSummonerSkillOnly
            };


            const bool includeBuddyItems = true;
            var message = _searchController.Search(query, filters.DuplicatesOnly, includeBuddyItems, _orderByLevel.Checked);

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
            UpdateListViewDelayed(200);
        }

        private void BeginSearchOnAutoSearch(object sender, EventArgs e) {
            UpdateListViewDelayed();
        }

        private void HandleDelayedTextChangedTimerTick(object sender, EventArgs e) {
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

            _delayedTextChangedTimer = new Timer();
            _delayedTextChangedTimer.Tick += HandleDelayedTextChangedTimerTick;
            _delayedTextChangedTimer.Interval = delay;
            _delayedTextChangedTimer.Start();
        }

        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this._mainSplitter = new System.Windows.Forms.SplitContainer();
            this._toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this._flowPanelFilter = new System.Windows.Forms.FlowLayoutPanel();
            this._searchBox = new System.Windows.Forms.TextBox();
            this._orderByLevel = new System.Windows.Forms.CheckBox();
            this._itemQuality = new System.Windows.Forms.ComboBox();
            this._slotFilter = new System.Windows.Forms.ComboBox();
            this._modFilter = new System.Windows.Forms.ComboBox();
            this._levelRequirementGroup = new System.Windows.Forms.GroupBox();
            this._minLevel = new System.Windows.Forms.TextBox();
            this._maxLevel = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this._mainSplitter)).BeginInit();
            this._mainSplitter.Panel2.SuspendLayout();
            this._mainSplitter.SuspendLayout();
            this._toolStripContainer.SuspendLayout();
            this._flowPanelFilter.SuspendLayout();
            this._levelRequirementGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // _mainSplitter
            // 
            this._mainSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mainSplitter.Location = new System.Drawing.Point(0, 0);
            this._mainSplitter.Name = "_mainSplitter";
            // 
            // _mainSplitter.Panel2
            // 
            this._mainSplitter.Panel2.Controls.Add(this._toolStripContainer);
            this._mainSplitter.Panel2.Controls.Add(this._flowPanelFilter);
            this._mainSplitter.Size = new System.Drawing.Size(1313, 650);
            this._mainSplitter.SplitterDistance = 204;
            this._mainSplitter.SplitterWidth = 3;
            this._mainSplitter.TabIndex = 0;
            this._mainSplitter.TabStop = false;
            // 
            // _toolStripContainer
            // 
            this._toolStripContainer.BottomToolStripPanelVisible = false;
            // 
            // _toolStripContainer.ContentPanel
            // 
            this._toolStripContainer.ContentPanel.Size = new System.Drawing.Size(1106, 576);
            this._toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._toolStripContainer.LeftToolStripPanelVisible = false;
            this._toolStripContainer.Location = new System.Drawing.Point(0, 49);
            this._toolStripContainer.Name = "_toolStripContainer";
            this._toolStripContainer.RightToolStripPanelVisible = false;
            this._toolStripContainer.Size = new System.Drawing.Size(1106, 601);
            this._toolStripContainer.TabIndex = 48;
            this._toolStripContainer.Text = "toolStripContainer1";
            // 
            // _flowPanelFilter
            // 
            this._flowPanelFilter.AutoSize = true;
            this._flowPanelFilter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._flowPanelFilter.Controls.Add(this._searchBox);
            this._flowPanelFilter.Controls.Add(this._orderByLevel);
            this._flowPanelFilter.Controls.Add(this._itemQuality);
            this._flowPanelFilter.Controls.Add(this._slotFilter);
            this._flowPanelFilter.Controls.Add(this._modFilter);
            this._flowPanelFilter.Controls.Add(this._levelRequirementGroup);
            this._flowPanelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this._flowPanelFilter.Location = new System.Drawing.Point(0, 0);
            this._flowPanelFilter.Name = "_flowPanelFilter";
            this._flowPanelFilter.Size = new System.Drawing.Size(1106, 49);
            this._flowPanelFilter.TabIndex = 52;
            // 
            // _searchBox
            // 
            this._searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._searchBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this._searchBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.RecentlyUsedList;
            this._searchBox.Location = new System.Drawing.Point(3, 17);
            this._searchBox.Margin = new System.Windows.Forms.Padding(3, 17, 3, 3);
            this._searchBox.MaxLength = 255;
            this._searchBox.Name = "_searchBox";
            this._searchBox.Size = new System.Drawing.Size(304, 20);
            this._searchBox.TabIndex = 41;
            this.toolTip1.SetToolTip(this._searchBox, "The item name, partially works fine.");
            // 
            // _orderByLevel
            // 
            this._orderByLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._orderByLevel.AutoSize = true;
            this._orderByLevel.Checked = true;
            this._orderByLevel.CheckState = System.Windows.Forms.CheckState.Checked;
            this._orderByLevel.Location = new System.Drawing.Point(313, 19);
            this._orderByLevel.Margin = new System.Windows.Forms.Padding(3, 19, 3, 3);
            this._orderByLevel.Name = "_orderByLevel";
            this._orderByLevel.Size = new System.Drawing.Size(96, 17);
            this._orderByLevel.TabIndex = 42;
            this._orderByLevel.Tag = "iatag_ui_orderbylevel";
            this._orderByLevel.Text = "Order By Level";
            this.toolTip1.SetToolTip(this._orderByLevel, "If items should be ordered by level, instead of alphabetically.");
            this._orderByLevel.UseVisualStyleBackColor = true;
            // 
            // _itemQuality
            // 
            this._itemQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._itemQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._itemQuality.FormattingEnabled = true;
            this._itemQuality.Location = new System.Drawing.Point(415, 17);
            this._itemQuality.Margin = new System.Windows.Forms.Padding(3, 17, 3, 3);
            this._itemQuality.Name = "_itemQuality";
            this._itemQuality.Size = new System.Drawing.Size(59, 21);
            this._itemQuality.TabIndex = 43;
            this.toolTip1.SetToolTip(this._itemQuality, "The minimum item quality");
            // 
            // _slotFilter
            // 
            this._slotFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._slotFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._slotFilter.FormattingEnabled = true;
            this._slotFilter.Location = new System.Drawing.Point(480, 17);
            this._slotFilter.Margin = new System.Windows.Forms.Padding(3, 17, 3, 3);
            this._slotFilter.Name = "_slotFilter";
            this._slotFilter.Size = new System.Drawing.Size(120, 21);
            this._slotFilter.TabIndex = 44;
            this.toolTip1.SetToolTip(this._slotFilter, "Slot/Type");
            // 
            // _modFilter
            // 
            this._modFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._modFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._modFilter.FormattingEnabled = true;
            this._modFilter.Location = new System.Drawing.Point(606, 17);
            this._modFilter.Margin = new System.Windows.Forms.Padding(3, 17, 3, 3);
            this._modFilter.Name = "_modFilter";
            this._modFilter.Size = new System.Drawing.Size(102, 21);
            this._modFilter.TabIndex = 45;
            this.toolTip1.SetToolTip(this._modFilter, "Mod / Hardcore / Vanilla");
            // 
            // _levelRequirementGroup
            // 
            this._levelRequirementGroup.Controls.Add(this._minLevel);
            this._levelRequirementGroup.Controls.Add(this._maxLevel);
            this._levelRequirementGroup.Location = new System.Drawing.Point(714, 3);
            this._levelRequirementGroup.Name = "_levelRequirementGroup";
            this._levelRequirementGroup.Size = new System.Drawing.Size(78, 43);
            this._levelRequirementGroup.TabIndex = 50;
            this._levelRequirementGroup.TabStop = false;
            this._levelRequirementGroup.Tag = "iatag_ui_level_requirement";
            this._levelRequirementGroup.Text = "Level";
            this.toolTip1.SetToolTip(this._levelRequirementGroup, "Level requirements for the item");
            // 
            // _minLevel
            // 
            this._minLevel.Location = new System.Drawing.Point(5, 15);
            this._minLevel.MaxLength = 3;
            this._minLevel.Name = "_minLevel";
            this._minLevel.Size = new System.Drawing.Size(30, 20);
            this._minLevel.TabIndex = 46;
            this._minLevel.Text = "0";
            this._minLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this._minLevel, "The minimum level required to use this item");
            this._minLevel.WordWrap = false;
            // 
            // _maxLevel
            // 
            this._maxLevel.Location = new System.Drawing.Point(40, 15);
            this._maxLevel.MaxLength = 3;
            this._maxLevel.Name = "_maxLevel";
            this._maxLevel.Size = new System.Drawing.Size(30, 20);
            this._maxLevel.TabIndex = 47;
            this._maxLevel.Text = "110";
            this._maxLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this._maxLevel, "The maximum level required to use this item");
            this._maxLevel.WordWrap = false;
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipTitle = "This is:";
            // 
            // SplitSearchWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1313, 650);
            this.Controls.Add(this._mainSplitter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplitSearchWindow";
            this.Text = "SearchWindow";
            this.Load += new System.EventHandler(this.SplitSearchWindow_Load);
            this._mainSplitter.Panel2.ResumeLayout(false);
            this._mainSplitter.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._mainSplitter)).EndInit();
            this._mainSplitter.ResumeLayout(false);
            this._toolStripContainer.ResumeLayout(false);
            this._toolStripContainer.PerformLayout();
            this._flowPanelFilter.ResumeLayout(false);
            this._flowPanelFilter.PerformLayout();
            this._levelRequirementGroup.ResumeLayout(false);
            this._levelRequirementGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        private void FlowPanelFilter_Resize(object sender, EventArgs e) {
            _searchBox.Width = Math.Max(300, _flowPanelFilter.Width - 500);
            _searchBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }
    }
}