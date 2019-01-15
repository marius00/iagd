using IAGrim.Database.Dto;
using IAGrim.Database.Interfaces;
using IAGrim.Theme;
using IAGrim.UI.Controller;
using IAGrim.UI.Tabs.Util;
using log4net;
using System;
using System.Linq;
using System.Windows.Forms;

namespace IAGrim.UI.Tabs
{
    internal sealed class SplitSearchWindow : Form
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SplitSearchWindow));
        private readonly Action<string> _setStatus;
        private readonly SearchController _searchController;
        private readonly IItemTagDao _itemTagDao;
        private Timer _delayedTextChangedTimer;
        private DesiredSkills _filterWindow;
        private TextBox searchBox;
        private CheckBox orderByLevel;
        private ComboBox modFilter;
        private ComboBox slotFilter;
        private SplitContainer mainSplitter;
        private ComboBox itemQuality;
        private ComboBoxItem _selectedSlot;
        private TextBox minLevel;
        private TextBox maxLevel;
        private FlowLayoutPanel flowPanelFilter;
        private GroupBox levelRequirementGroup;
        private ComboBoxItemQuality _selectedItemQuality;
        private ScrollPanelMessageFilter _scrollableFilterView;
        private ToolStripContainer toolStripContainer;

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
            IItemTagDao itemTagDao)
        {
            _setStatus = setStatus;
            _searchController = searchController;
            _itemTagDao = itemTagDao;
            InitializeComponent();

            Dock = DockStyle.Fill;

            mainSplitter.SplitterDistance = FilterPanelMinSize;
            mainSplitter.SplitterWidth = 5;
            mainSplitter.BorderStyle = BorderStyle.Fixed3D;
            mainSplitter.SplitterMoved += MainSplitterOnSplitterMoved;

            ModSelectionHandler = new ModSelectionHandler(modFilter, playerItemDao, UpdateListViewDelayed, setStatus);

            toolStripContainer.ContentPanel.Controls.Add(browser);

            Activated += SplitSearchWindow_Activated;
            Deactivate += SplitSearchWindow_Deactivate;

            InitializeFilterPanel();
        }

        /// <summary>
        /// Clear all filters
        /// </summary>
        public void ClearFilters()
        {
            _filterWindow.ClearFilters();
            searchBox.Text = string.Empty;
            itemQuality.SelectedIndex = 0;
            slotFilter.SelectedIndex = 0;
            minLevel.Text = "0";
            maxLevel.Text = "110";

            UpdateListViewDelayed();
        }

        /// <summary>
        /// Update interface
        /// </summary>
        public void UpdateInterface()
        {
            InitializeFilterPanel();
        }

        /// <summary>
        /// Update view
        /// </summary>
        public void UpdateListView()
        {
            UpdateListView(_filterWindow.Filters);
        }

        /// <summary>
        /// Update view
        /// </summary>
        /// <param name="filters">Filters</param>
        public void UpdateListView(FilterEventArgs filters)
        {
            var transferFile = ModSelectionHandler.SelectedMod;

            if (transferFile == null)
            {
                return;
            }

            var rarity = _selectedItemQuality;
            var slot = _selectedSlot;
            var query = new ItemSearchRequest
            {
                Wildcard = searchBox.Text,
                Filters = filters.Filters,
                MinimumLevel = ParseNumeric(minLevel),
                MaximumLevel = ParseNumeric(maxLevel),
                Rarity = rarity?.Rarity,
                Slot = slot?.Filter,
                PetBonuses = filters.PetBonuses,
                IsRetaliation = filters.IsRetaliation,
                Mod = transferFile.Mod,
                IsHardcore = transferFile.IsHardcore,
                Classes = filters.DesiredClass,
                SocketedOnly = filters.SocketedOnly,
                RecentOnly = filters.RecentOnly
            };

            var includeBuddyItems = Properties.Settings.Default.BuddySyncEnabled;
            var message = _searchController.Search(query, filters.DuplicatesOnly, includeBuddyItems, orderByLevel.Checked);

            _logger.Info("Updating UI...");

            if (!string.IsNullOrEmpty(message))
            {
                _setStatus(message);
            }
            _logger.Info("Done");
        }

        /// <summary>
        /// Update view with delay
        /// </summary>
        public void UpdateListViewDelayed()
        {
            UpdateListViewDelayed(200);
        }

        private void BeginSearchOnAutoSearch(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.AutoSearch)
            {
                UpdateListViewDelayed();
            }
        }

        private void HandleDelayedTextChangedTimerTick(object sender, EventArgs e)
        {
            if (_delayedTextChangedTimer != null)
            {
                _delayedTextChangedTimer.Stop();
                _delayedTextChangedTimer = null;
            }

            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    UpdateListView(_filterWindow.Filters);
                });
            }
            else
            {
                UpdateListView(_filterWindow.Filters);
            }
        }

        private void InitializeFilterPanel()
        {
            if (_filterWindow != null)
            {
                _filterWindow.OnChanged -= BeginSearchOnAutoSearch;
                _filterWindow.Close();
                mainSplitter.Panel1.Controls.Remove(_filterWindow);
            }

            _filterWindow = new DesiredSkills(_itemTagDao)
            {
                TopLevel = false
            };
            _filterWindow.OnChanged += BeginSearchOnAutoSearch;
            mainSplitter.Panel1.Controls.Add(_filterWindow);
            _filterWindow.Show();
        }

        private void MainSplitterOnSplitterMoved(object sender, SplitterEventArgs e)
        {
            if (mainSplitter.SplitterDistance < FilterPanelMinSize)
            {
                mainSplitter.SplitterDistance = FilterPanelMinSize;
            }
        }

        private void MaxLevel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (sender is Control c)
            {
                if (e.Delta > 0)
                {
                    c.Text = Math.Min(ParseNumeric(c) + 1, 110).ToString();
                }
                else if (e.Delta < 0)
                {
                    var newValue = Math.Min(Math.Max(0, ParseNumeric(c) - 1), 110);
                    if (ParseNumeric(minLevel) > newValue)
                    {
                        minLevel.Text = newValue.ToString();
                    }
                    c.Text = Math.Min(Math.Max(0, ParseNumeric(c) - 1), 110).ToString();
                }
            }

            UpdateListViewDelayed(1200);
        }

        private void MinLevel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (sender is Control c)
            {
                if (e.Delta > 0)
                {
                    var newValue = Math.Min(ParseNumeric(c) + 1, 110);
                    if (ParseNumeric(maxLevel) < newValue)
                    {
                        maxLevel.Text = newValue.ToString();
                    }
                    c.Text = newValue.ToString();
                }
                else if (e.Delta < 0)
                {
                    c.Text = Math.Min(Math.Max(0, ParseNumeric(c) - 1), 110).ToString();
                }
            }

            UpdateListViewDelayed(1200);
        }

        private void MinLevel_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar) || ParseNumeric(minLevel) > 105 || e.KeyChar == '\b');
            UpdateListViewDelayed(1200);
        }

        private int ParseNumeric(Control tb)
        {
            return int.TryParse(tb.Text, out var val) ? val : 0;
        }

        private void SplitSearchWindow_Load(object sender, EventArgs e)
        {
            ModSelectionHandler.ConfigureModFilter();

            minLevel.KeyPress += MinLevel_KeyPress;
            minLevel.MouseWheel += MinLevel_MouseWheel;

            maxLevel.KeyPress += MinLevel_KeyPress;
            maxLevel.MouseWheel += MaxLevel_MouseWheel;

            itemQuality.Items.AddRange(UIHelper.QualityFilter.ToArray<object>());
            itemQuality.SelectedIndex = 0;
            _selectedItemQuality = itemQuality.SelectedItem as ComboBoxItemQuality;
            itemQuality.SelectedIndexChanged += (s, ev) =>
            {
                _selectedItemQuality = itemQuality.SelectedItem as ComboBoxItemQuality;
            };
            itemQuality.SelectedIndexChanged += BeginSearchOnAutoSearch;

            slotFilter.Items.AddRange(UIHelper.SlotFilter.ToArray<object>());
            slotFilter.SelectedIndex = 0;
            _selectedSlot = slotFilter.SelectedItem as ComboBoxItem;
            slotFilter.SelectedIndexChanged += (s, ev) =>
            {
                _selectedSlot = slotFilter.SelectedItem as ComboBoxItem;
            };
            slotFilter.SelectedIndexChanged += BeginSearchOnAutoSearch;

            FormClosing += SplitSearchWindow_FormClosing;

            searchBox.KeyDown += SearchBox_KeyDown;
            searchBox.TextChanged += SearchBox_TextChanged;

            orderByLevel.CheckStateChanged += delegate
            {
                UpdateListViewDelayed();
            };
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!Properties.Settings.Default.AutoSearch && e.KeyCode == Keys.Enter)
            {
                UpdateListView();
                e.Handled = true;
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.AutoSearch)
            {
                UpdateListViewDelayed(600);
            }
        }

        private void SplitSearchWindow_Activated(object sender, EventArgs e)
        {
            _scrollableFilterView = new ScrollPanelMessageFilter(_filterWindow);
            Application.AddMessageFilter(_scrollableFilterView);
        }

        private void SplitSearchWindow_Deactivate(object sender, EventArgs e)
        {
            Application.RemoveMessageFilter(_scrollableFilterView);
        }

        private void SplitSearchWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _filterWindow.OnChanged -= BeginSearchOnAutoSearch;
            Activated -= SplitSearchWindow_Activated;
            Deactivate -= SplitSearchWindow_Deactivate;

            toolStripContainer.ContentPanel.Controls.Clear();
        }

        private void UpdateListViewDelayed(int delay)
        {
            _delayedTextChangedTimer?.Stop();

            _delayedTextChangedTimer = new Timer();
            _delayedTextChangedTimer.Tick += HandleDelayedTextChangedTimerTick;
            _delayedTextChangedTimer.Interval = delay;
            _delayedTextChangedTimer.Start();
        }

        private void InitializeComponent()
        {
            this.mainSplitter = new System.Windows.Forms.SplitContainer();
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.flowPanelFilter = new System.Windows.Forms.FlowLayoutPanel();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.orderByLevel = new System.Windows.Forms.CheckBox();
            this.itemQuality = new System.Windows.Forms.ComboBox();
            this.slotFilter = new System.Windows.Forms.ComboBox();
            this.modFilter = new System.Windows.Forms.ComboBox();
            this.levelRequirementGroup = new System.Windows.Forms.GroupBox();
            this.minLevel = new System.Windows.Forms.TextBox();
            this.maxLevel = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitter)).BeginInit();
            this.mainSplitter.Panel2.SuspendLayout();
            this.mainSplitter.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.flowPanelFilter.SuspendLayout();
            this.levelRequirementGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainSplitter
            // 
            this.mainSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitter.Location = new System.Drawing.Point(0, 0);
            this.mainSplitter.Name = "mainSplitter";
            // 
            // mainSplitter.Panel2
            // 
            this.mainSplitter.Panel2.Controls.Add(this.toolStripContainer);
            this.mainSplitter.Panel2.Controls.Add(this.flowPanelFilter);
            this.mainSplitter.Size = new System.Drawing.Size(1313, 650);
            this.mainSplitter.SplitterDistance = 204;
            this.mainSplitter.TabIndex = 0;
            // 
            // toolStripContainer
            // 
            this.toolStripContainer.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(1105, 576);
            this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer.LeftToolStripPanelVisible = false;
            this.toolStripContainer.Location = new System.Drawing.Point(0, 49);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.RightToolStripPanelVisible = false;
            this.toolStripContainer.Size = new System.Drawing.Size(1105, 601);
            this.toolStripContainer.TabIndex = 48;
            this.toolStripContainer.Text = "toolStripContainer1";
            // 
            // flowPanelFilter
            // 
            this.flowPanelFilter.AutoSize = true;
            this.flowPanelFilter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowPanelFilter.Controls.Add(this.searchBox);
            this.flowPanelFilter.Controls.Add(this.orderByLevel);
            this.flowPanelFilter.Controls.Add(this.itemQuality);
            this.flowPanelFilter.Controls.Add(this.slotFilter);
            this.flowPanelFilter.Controls.Add(this.modFilter);
            this.flowPanelFilter.Controls.Add(this.levelRequirementGroup);
            this.flowPanelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowPanelFilter.Location = new System.Drawing.Point(0, 0);
            this.flowPanelFilter.Name = "flowPanelFilter";
            this.flowPanelFilter.Size = new System.Drawing.Size(1105, 49);
            this.flowPanelFilter.TabIndex = 52;
            // 
            // searchBox
            // 
            this.searchBox.Location = new System.Drawing.Point(3, 17);
            this.searchBox.Margin = new System.Windows.Forms.Padding(3, 17, 3, 3);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(250, 20);
            this.searchBox.TabIndex = 41;
            // 
            // orderByLevel
            // 
            this.orderByLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.orderByLevel.AutoSize = true;
            this.orderByLevel.Location = new System.Drawing.Point(259, 19);
            this.orderByLevel.Margin = new System.Windows.Forms.Padding(3, 19, 3, 3);
            this.orderByLevel.Name = "orderByLevel";
            this.orderByLevel.Size = new System.Drawing.Size(96, 17);
            this.orderByLevel.TabIndex = 42;
            this.orderByLevel.Tag = "iatag_ui_orderbylevel";
            this.orderByLevel.Text = "Order By Level";
            this.orderByLevel.UseVisualStyleBackColor = true;
            // 
            // itemQuality
            // 
            this.itemQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.itemQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.itemQuality.FormattingEnabled = true;
            this.itemQuality.Location = new System.Drawing.Point(361, 17);
            this.itemQuality.Margin = new System.Windows.Forms.Padding(3, 17, 3, 3);
            this.itemQuality.Name = "itemQuality";
            this.itemQuality.Size = new System.Drawing.Size(59, 21);
            this.itemQuality.TabIndex = 43;
            // 
            // slotFilter
            // 
            this.slotFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.slotFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.slotFilter.FormattingEnabled = true;
            this.slotFilter.Location = new System.Drawing.Point(426, 17);
            this.slotFilter.Margin = new System.Windows.Forms.Padding(3, 17, 3, 3);
            this.slotFilter.Name = "slotFilter";
            this.slotFilter.Size = new System.Drawing.Size(120, 21);
            this.slotFilter.TabIndex = 44;
            // 
            // modFilter
            // 
            this.modFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.modFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modFilter.FormattingEnabled = true;
            this.modFilter.Location = new System.Drawing.Point(552, 17);
            this.modFilter.Margin = new System.Windows.Forms.Padding(3, 17, 3, 3);
            this.modFilter.Name = "modFilter";
            this.modFilter.Size = new System.Drawing.Size(102, 21);
            this.modFilter.TabIndex = 45;
            // 
            // levelRequirementGroup
            // 
            this.levelRequirementGroup.Controls.Add(this.minLevel);
            this.levelRequirementGroup.Controls.Add(this.maxLevel);
            this.levelRequirementGroup.Location = new System.Drawing.Point(660, 3);
            this.levelRequirementGroup.Name = "levelRequirementGroup";
            this.levelRequirementGroup.Size = new System.Drawing.Size(78, 43);
            this.levelRequirementGroup.TabIndex = 50;
            this.levelRequirementGroup.TabStop = false;
            this.levelRequirementGroup.Text = "Level reqs.";
            // 
            // minLevel
            // 
            this.minLevel.Location = new System.Drawing.Point(5, 15);
            this.minLevel.MaxLength = 3;
            this.minLevel.Name = "minLevel";
            this.minLevel.Size = new System.Drawing.Size(30, 20);
            this.minLevel.TabIndex = 46;
            this.minLevel.Text = "0";
            this.minLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.minLevel.WordWrap = false;
            // 
            // maxLevel
            // 
            this.maxLevel.Location = new System.Drawing.Point(40, 15);
            this.maxLevel.MaxLength = 3;
            this.maxLevel.Name = "maxLevel";
            this.maxLevel.Size = new System.Drawing.Size(30, 20);
            this.maxLevel.TabIndex = 47;
            this.maxLevel.Text = "110";
            this.maxLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.maxLevel.WordWrap = false;
            // 
            // SplitSearchWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1313, 650);
            this.Controls.Add(this.mainSplitter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplitSearchWindow";
            this.Text = "SearchWindow";
            this.Load += new System.EventHandler(this.SplitSearchWindow_Load);
            this.mainSplitter.Panel2.ResumeLayout(false);
            this.mainSplitter.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitter)).EndInit();
            this.mainSplitter.ResumeLayout(false);
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.flowPanelFilter.ResumeLayout(false);
            this.flowPanelFilter.PerformLayout();
            this.levelRequirementGroup.ResumeLayout(false);
            this.levelRequirementGroup.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
