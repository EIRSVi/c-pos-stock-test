using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using Stock_Room.Models;
using Stock_Room.UI;

namespace Stock_Room
{
    public partial class DashboardForm : Form
    {
        private UserInfo currentUser;
        private DatabaseHelper databaseHelper;
        
        // Main layout panels
        private Panel sidebarPanel;
        private Panel mainPanel;
        private Panel topPanel;
        private Panel contentPanel;
        
        // Header controls
        private Label welcomeLabel;
        private Label userLabel;
        private ModernButton logoutButton;
        
        // Sidebar navigation
        private ModernButton dashboardButton;
        private ModernButton inventoryButton;
        private ModernButton reportsButton;
        private ModernButton settingsButton;
        
        // Content controls
        private Label contentHeaderLabel;
        private ModernPanel currentContentPanel;
        
        // Dashboard stats
        private DashboardStats? dashboardStats;

        public DashboardForm(UserInfo userInfo)
        {
            currentUser = userInfo;
            databaseHelper = new DatabaseHelper();
            InitializeComponent();
            LoadDashboardContentAsync();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Stock Room Management System";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1400, 900);
            this.BackColor = ResourceManager.Theme.Background;
            this.Font = ResourceManager.Typography.BodyMedium;

            // Top Panel
            CreateTopPanel();
            
            // Sidebar Panel
            CreateSidebarPanel();
            
            // Main Panel
            CreateMainPanel();

            // Add panels to form
            this.Controls.Add(mainPanel);
            this.Controls.Add(sidebarPanel);
            this.Controls.Add(topPanel);

            // Form events
            this.FormClosing += DashboardForm_FormClosing;
            this.KeyPreview = true;
            this.KeyDown += DashboardForm_KeyDown;

            this.ResumeLayout();
        }

        private void CreateTopPanel()
        {
            topPanel = new Panel
            {
                Size = new Size(this.Width, 80),
                Location = new Point(0, 0),
                BackColor = ResourceManager.Theme.Primary,
                Dock = DockStyle.Top
            };

            // Gradient background
            topPanel.Paint += (s, e) =>
            {
                using var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    topPanel.ClientRectangle,
                    ResourceManager.Theme.Primary,
                    ResourceManager.Theme.PrimaryDark,
                    System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                
                e.Graphics.FillRectangle(brush, topPanel.ClientRectangle);

                // App icon
                var appIcon = ResourceManager.DashboardIcons.GetAppIcon();
                if (appIcon != null)
                {
                    e.Graphics.DrawImage(appIcon, new Rectangle(20, 24, 32, 32));
                }
            };

            // Welcome label
            welcomeLabel = new Label
            {
                Text = "STOCK ROOM MANAGEMENT SYSTEM",
                Font = ResourceManager.Typography.HeaderMedium,
                ForeColor = Color.White,
                Size = new Size(600, 35),
                Location = new Point(65, 12),
                BackColor = Color.Transparent
            };

            // User label
            userLabel = new Label
            {
                Text = $"Welcome back, {currentUser.FullName} | Role: {currentUser.UserRole}",
                Font = ResourceManager.Typography.BodyMedium,
                ForeColor = Color.FromArgb(219, 234, 254),
                Size = new Size(800, 25),
                Location = new Point(65, 47),
                BackColor = Color.Transparent
            };

            // Logout button
            logoutButton = new ModernButton
            {
                Text = "LOGOUT",
                Size = new Size(120, 36),
                Location = new Point(this.Width - 150, 22),
                BackColor = ResourceManager.Theme.Danger,
                HoverColor = Color.FromArgb(220, 38, 38),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Image = ResourceManager.DashboardIcons.GetLogoutIcon()
            };
            logoutButton.Click += LogoutButton_Click;

            topPanel.Controls.AddRange(new Control[] { welcomeLabel, userLabel, logoutButton });
        }

        private void CreateSidebarPanel()
        {
            sidebarPanel = new Panel
            {
                Size = new Size(280, this.Height - 80),
                Location = new Point(0, 80),
                BackColor = ResourceManager.Theme.Secondary,
                Dock = DockStyle.Left
            };

            // Gradient background
            sidebarPanel.Paint += (s, e) =>
            {
                using var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    sidebarPanel.ClientRectangle,
                    ResourceManager.Theme.Secondary,
                    ResourceManager.Theme.SecondaryLight,
                    System.Drawing.Drawing2D.LinearGradientMode.Vertical);
                
                e.Graphics.FillRectangle(brush, sidebarPanel.ClientRectangle);
            };

            // Navigation buttons
            dashboardButton = CreateSidebarButton("Dashboard", "Main overview and statistics", 
                ResourceManager.DashboardIcons.GetDashboardIcon(), 30);
            dashboardButton.Click += (s, e) => LoadDashboardContentAsync();

            inventoryButton = CreateSidebarButton("Inventory", "Manage stock items", 
                ResourceManager.DashboardIcons.GetInventoryIcon(), 100);
            inventoryButton.Click += (s, e) => LoadInventoryContent();

            reportsButton = CreateSidebarButton("Reports", "View analytics and reports", 
                ResourceManager.DashboardIcons.GetReportsIcon(), 170);
            reportsButton.Click += (s, e) => LoadReportsContent();

            settingsButton = CreateSidebarButton("Settings", "System configuration", 
                ResourceManager.DashboardIcons.GetSettingsIcon(), 240);
            settingsButton.Click += (s, e) => LoadSettingsContent();

            sidebarPanel.Controls.AddRange(new Control[] { 
                dashboardButton, inventoryButton, reportsButton, settingsButton 
            });
        }

        private ModernButton CreateSidebarButton(string text, string tooltip, Image icon, int yPosition)
        {
            var button = new ModernButton
            {
                Text = text,
                Font = ResourceManager.Typography.BodyLarge,
                Size = new Size(260, 60),
                Location = new Point(10, yPosition),
                BackColor = ResourceManager.Theme.SidebarInactive,
                HoverColor = ResourceManager.Theme.SidebarActive,
                ForeColor = ResourceManager.Theme.SidebarText,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(50, 0, 0, 0),
                Image = icon,
                ImageAlign = ContentAlignment.MiddleLeft
            };

            button.Paint += (s, e) =>
            {
                // Custom paint for sidebar button
                if (button.Image != null)
                {
                    var imageRect = new Rectangle(15, (button.Height - 24) / 2, 24, 24);
                    e.Graphics.DrawImage(button.Image, imageRect);
                }
            };

            var toolTip = new ToolTip();
            toolTip.SetToolTip(button, tooltip);

            return button;
        }

        private void CreateMainPanel()
        {
            mainPanel = new Panel
            {
                Location = new Point(280, 80),
                Size = new Size(this.Width - 280, this.Height - 80),
                BackColor = ResourceManager.Theme.Background,
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Content header
            contentHeaderLabel = new Label
            {
                Font = ResourceManager.Typography.HeaderLarge,
                ForeColor = ResourceManager.Theme.TextPrimary,
                Size = new Size(600, 50),
                Location = new Point(20, 20),
                BackColor = Color.Transparent
            };

            mainPanel.Controls.Add(contentHeaderLabel);
        }

        private async void LoadDashboardContentAsync()
        {
            try
            {
                ClearContent();
                contentHeaderLabel.Text = "Dashboard Overview";
                SetActiveButton(dashboardButton);

                // Load dashboard statistics
                dashboardStats = await databaseHelper.GetDashboardStatsAsync();

                // Create dashboard content
                CreateDashboardContent();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Failed to load dashboard", ex.Message);
            }
        }

        private void CreateDashboardContent()
        {
            var contentPanel = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(mainPanel.Width - 40, mainPanel.Height - 100),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            // Statistics cards
            var cardWidth = 280;
            var cardHeight = 160;
            var cardMargin = 20;
            var cardsPerRow = (contentPanel.Width - cardMargin) / (cardWidth + cardMargin);
            
            var cards = new[]
            {
                new { Title = "Total Items", Value = dashboardStats?.TotalItems.ToString() ?? "0", 
                      Color = ResourceManager.Theme.Primary, Icon = "??" },
                new { Title = "Low Stock Items", Value = dashboardStats?.LowStockItems.ToString() ?? "0", 
                      Color = ResourceManager.Theme.Warning, Icon = "??" },
                new { Title = "Categories", Value = dashboardStats?.TotalCategories.ToString() ?? "0", 
                      Color = ResourceManager.Theme.Success, Icon = "??" },
                new { Title = "Total Value", Value = $"${dashboardStats?.TotalInventoryValue:N2}" ?? "$0.00", 
                      Color = ResourceManager.Theme.Info, Icon = "??" }
            };

            for (int i = 0; i < cards.Length; i++)
            {
                var card = cards[i];
                var x = cardMargin + (i % cardsPerRow) * (cardWidth + cardMargin);
                var y = cardMargin + (i / cardsPerRow) * (cardHeight + cardMargin);
                
                CreateStatCard(contentPanel, card.Title, card.Value, card.Color, x, y, cardWidth, cardHeight);
            }

            // Recent activity section
            var activityY = cardMargin + (cardHeight + cardMargin) * 2;
            CreateRecentActivitySection(contentPanel, activityY);

            mainPanel.Controls.Add(contentPanel);
            currentContentPanel = null; // Dashboard doesn't use ModernPanel
        }

        private void CreateStatCard(Panel parent, string title, string value, Color accentColor, int x, int y, int width, int height)
        {
            var card = new ModernPanel
            {
                Size = new Size(width, height),
                Location = new Point(x, y),
                HasShadow = true,
                ShadowDepth = 6
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = ResourceManager.Typography.BodyMedium,
                ForeColor = ResourceManager.Theme.TextSecondary,
                Size = new Size(width - 40, 30),
                Location = new Point(20, 20),
                BackColor = Color.Transparent
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = accentColor,
                Size = new Size(width - 40, 60),
                Location = new Point(20, 50),
                BackColor = Color.Transparent
            };

            var trendLabel = new Label
            {
                Text = "? +12% from last month",
                Font = ResourceManager.Typography.BodySmall,
                ForeColor = ResourceManager.Theme.Success,
                Size = new Size(width - 40, 20),
                Location = new Point(20, height - 40),
                BackColor = Color.Transparent
            };

            card.Controls.AddRange(new Control[] { titleLabel, valueLabel, trendLabel });
            parent.Controls.Add(card);
        }

        private void CreateRecentActivitySection(Panel parent, int y)
        {
            var activityLabel = new Label
            {
                Text = "Recent Activity",
                Font = ResourceManager.Typography.HeaderSmall,
                ForeColor = ResourceManager.Theme.TextPrimary,
                Size = new Size(300, 30),
                Location = new Point(20, y),
                BackColor = Color.Transparent
            };

            var activityPanel = new ModernPanel
            {
                Size = new Size(parent.Width - 40, 200),
                Location = new Point(20, y + 40),
                HasShadow = true
            };

            // Sample activities
            var activities = new[]
            {
                "? Added 50 units of Product A",
                "?? Updated pricing for Category B", 
                "?? Low stock alert: Product C",
                "?? Generated monthly report",
                "?? New user account created"
            };

            for (int i = 0; i < activities.Length; i++)
            {
                var activityItem = new Label
                {
                    Text = activities[i],
                    Font = ResourceManager.Typography.BodyMedium,
                    ForeColor = ResourceManager.Theme.TextSecondary,
                    Size = new Size(activityPanel.Width - 40, 25),
                    Location = new Point(20, 20 + i * 30),
                    BackColor = Color.Transparent
                };
                activityPanel.Controls.Add(activityItem);
            }

            parent.Controls.AddRange(new Control[] { activityLabel, activityPanel });
        }

        private void LoadInventoryContent()
        {
            ClearContent();
            contentHeaderLabel.Text = "Inventory Management";
            SetActiveButton(inventoryButton);

            currentContentPanel = new ModernPanel
            {
                Location = new Point(20, 80),
                Size = new Size(mainPanel.Width - 40, mainPanel.Height - 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            // Inventory management form will be created here
            CreateInventoryManagementContent();
            
            mainPanel.Controls.Add(currentContentPanel);
        }

        private void CreateInventoryManagementContent()
        {
            // Toolbar
            var toolbar = new Panel
            {
                Size = new Size(currentContentPanel.Width - 40, 60),
                Location = new Point(20, 20),
                BackColor = Color.Transparent
            };

            var addButton = new ModernButton
            {
                Text = "Add Item",
                Size = new Size(100, 40),
                Location = new Point(0, 10),
                BackColor = ResourceManager.Theme.Success,
                Image = ResourceManager.DashboardIcons.GetAddIcon()
            };

            var editButton = new ModernButton
            {
                Text = "Edit",
                Size = new Size(80, 40),
                Location = new Point(110, 10),
                BackColor = ResourceManager.Theme.Info,
                Image = ResourceManager.DashboardIcons.GetEditIcon()
            };

            var deleteButton = new ModernButton
            {
                Text = "Delete",
                Size = new Size(80, 40),
                Location = new Point(200, 10),
                BackColor = ResourceManager.Theme.Danger,
                Image = ResourceManager.DashboardIcons.GetDeleteIcon()
            };

            var refreshButton = new ModernButton
            {
                Text = "Refresh",
                Size = new Size(90, 40),
                Location = new Point(290, 10),
                BackColor = ResourceManager.Theme.TextSecondary,
                Image = ResourceManager.DashboardIcons.GetRefreshIcon()
            };

            // Search box
            var searchBox = new ModernTextBox
            {
                Placeholder = "Search inventory items...",
                Size = new Size(250, 40),
                Location = new Point(toolbar.Width - 250, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            toolbar.Controls.AddRange(new Control[] { addButton, editButton, deleteButton, refreshButton, searchBox });

            // DataGridView for inventory items
            var inventoryGrid = new ModernDataGridView
            {
                Location = new Point(20, 100),
                Size = new Size(currentContentPanel.Width - 40, currentContentPanel.Height - 140),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            // Configure columns
            inventoryGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "ItemID", HeaderText = "ID", Width = 60 },
                new DataGridViewTextBoxColumn { Name = "ItemName", HeaderText = "Item Name", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "SKU", HeaderText = "SKU", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Category", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Quantity", HeaderText = "Quantity", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "CostPrice", HeaderText = "Cost Price", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "SellingPrice", HeaderText = "Selling Price", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 100 }
            });

            currentContentPanel.Controls.AddRange(new Control[] { toolbar, inventoryGrid });

            // Load inventory data
            LoadInventoryDataAsync(inventoryGrid);

            // Event handlers
            addButton.Click += (s, e) => ShowAddItemDialog();
            editButton.Click += (s, e) => ShowEditItemDialog(inventoryGrid);
            deleteButton.Click += (s, e) => DeleteSelectedItem(inventoryGrid);
            refreshButton.Click += (s, e) => LoadInventoryDataAsync(inventoryGrid);
            searchBox.TextChanged += (s, e) => FilterInventoryItems(inventoryGrid, searchBox.GetValue());
        }

        private async void LoadInventoryDataAsync(ModernDataGridView grid)
        {
            try
            {
                var items = await databaseHelper.GetInventoryItemsAsync();
                
                grid.Rows.Clear();
                foreach (var item in items)
                {
                    grid.Rows.Add(
                        item.ItemID,
                        item.ItemName,
                        item.SKU,
                        item.CategoryName ?? "N/A",
                        item.Quantity,
                        $"${item.CostPrice:N2}",
                        $"${item.SellingPrice:N2}",
                        item.Status
                    );
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Failed to load inventory", ex.Message);
            }
        }

        private void LoadReportsContent()
        {
            ClearContent();
            contentHeaderLabel.Text = "Reports & Analytics";
            SetActiveButton(reportsButton);

            currentContentPanel = new ModernPanel
            {
                Location = new Point(20, 80),
                Size = new Size(mainPanel.Width - 40, mainPanel.Height - 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            var comingSoonLabel = new Label
            {
                Text = "?? Reports & Analytics\n\nComing Soon!\n\nFeatures will include:\n• Inventory Reports\n• Sales Analytics\n• Stock Movement History\n• Low Stock Alerts\n• Custom Report Builder",
                Font = ResourceManager.Typography.BodyLarge,
                ForeColor = ResourceManager.Theme.TextSecondary,
                Size = new Size(400, 300),
                Location = new Point(50, 50),
                BackColor = Color.Transparent
            };

            currentContentPanel.Controls.Add(comingSoonLabel);
            mainPanel.Controls.Add(currentContentPanel);
        }

        private void LoadSettingsContent()
        {
            ClearContent();
            contentHeaderLabel.Text = "System Settings";
            SetActiveButton(settingsButton);

            currentContentPanel = new ModernPanel
            {
                Location = new Point(20, 80),
                Size = new Size(mainPanel.Width - 40, mainPanel.Height - 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            var comingSoonLabel = new Label
            {
                Text = "?? System Settings\n\nComing Soon!\n\nFeatures will include:\n• User Management\n• System Configuration\n• Database Settings\n• Backup & Restore\n• Security Settings",
                Font = ResourceManager.Typography.BodyLarge,
                ForeColor = ResourceManager.Theme.TextSecondary,
                Size = new Size(400, 300),
                Location = new Point(50, 50),
                BackColor = Color.Transparent
            };

            currentContentPanel.Controls.Add(comingSoonLabel);
            mainPanel.Controls.Add(currentContentPanel);
        }

        #region Helper Methods

        private void ClearContent()
        {
            // Remove existing content panel
            if (currentContentPanel != null)
            {
                mainPanel.Controls.Remove(currentContentPanel);
                currentContentPanel.Dispose();
                currentContentPanel = null;
            }

            // Remove other dynamic controls
            var controlsToRemove = new List<Control>();
            foreach (Control control in mainPanel.Controls)
            {
                if (control != contentHeaderLabel)
                {
                    controlsToRemove.Add(control);
                }
            }

            foreach (var control in controlsToRemove)
            {
                mainPanel.Controls.Remove(control);
                control.Dispose();
            }
        }

        private void SetActiveButton(ModernButton activeButton)
        {
            // Reset all buttons
            var buttons = new[] { dashboardButton, inventoryButton, reportsButton, settingsButton };
            foreach (var button in buttons)
            {
                button.BackColor = ResourceManager.Theme.SidebarInactive;
                button.HoverColor = ResourceManager.Theme.SidebarActive;
            }

            // Set active button
            activeButton.BackColor = ResourceManager.Theme.SidebarActive;
            activeButton.HoverColor = ResourceManager.Theme.Primary;
        }

        private void ShowAddItemDialog()
        {
            try
            {
                using var addItemForm = new Stock_Room.Forms.InventoryItemForm();
                if (addItemForm.ShowDialog() == DialogResult.OK)
                {
                    // Refresh the inventory grid if we're on the inventory page
                    var inventoryGrid = FindInventoryGrid();
                    if (inventoryGrid != null)
                    {
                        LoadInventoryDataAsync(inventoryGrid);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Failed to open add item dialog", ex.Message);
            }
        }

        private void ShowEditItemDialog(ModernDataGridView grid)
        {
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item to edit.", "Edit Item", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var itemId = Convert.ToInt32(grid.SelectedRows[0].Cells["ItemID"].Value);
                
                // Load the full item details
                Task.Run(async () =>
                {
                    var item = await databaseHelper.GetInventoryItemAsync(itemId);
                    if (item != null)
                    {
                        this.Invoke(() =>
                        {
                            using var editItemForm = new Stock_Room.Forms.InventoryItemForm(item);
                            if (editItemForm.ShowDialog() == DialogResult.OK)
                            {
                                LoadInventoryDataAsync(grid);
                            }
                        });
                    }
                    else
                    {
                        this.Invoke(() =>
                        {
                            MessageBox.Show("Item not found.", "Error", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Failed to open edit item dialog", ex.Message);
            }
        }

        private ModernDataGridView? FindInventoryGrid()
        {
            if (currentContentPanel == null) return null;
            
            foreach (Control control in currentContentPanel.Controls)
            {
                if (control is ModernDataGridView grid)
                {
                    return grid;
                }
            }
            return null;
        }

        private void ShowErrorMessage(string title, string message)
        {
            MessageBox.Show($"{title}\n\n{message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region Event Handlers

        private void DashboardForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    LogoutButton_Click(sender, e);
                    break;
                case Keys.F1:
                    LoadDashboardContentAsync();
                    break;
                case Keys.F2:
                    LoadInventoryContent();
                    break;
                case Keys.F3:
                    LoadReportsContent();
                    break;
                case Keys.F4:
                    LoadSettingsContent();
                    break;
            }
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show($"Are you sure you want to logout, {currentUser.FullName}?", 
                "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void DashboardForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            databaseHelper?.Dispose();
            Application.Exit();
        }

        private async void DeleteSelectedItem(ModernDataGridView grid)
        {
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item to delete.", "Delete Item", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var itemId = Convert.ToInt32(grid.SelectedRows[0].Cells["ItemID"].Value);
                    await databaseHelper.DeleteInventoryItemAsync(itemId);
                    LoadInventoryDataAsync(grid);
                    MessageBox.Show("Item deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("Failed to delete item", ex.Message);
                }
            }
        }

        private async void FilterInventoryItems(ModernDataGridView grid, string searchTerm)
        {
            try
            {
                var searchParams = new InventorySearchParams();
                
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchParams.SearchTerm = searchTerm;
                }

                var items = await databaseHelper.GetInventoryItemsAsync(searchParams);
                
                grid.Rows.Clear();
                foreach (var item in items)
                {
                    grid.Rows.Add(
                        item.ItemID,
                        item.ItemName,
                        item.SKU,
                        item.CategoryName ?? "N/A",
                        item.Quantity,
                        $"${item.CostPrice:N2}",
                        $"${item.SellingPrice:N2}",
                        item.Status
                    );
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Failed to filter inventory", ex.Message);
            }
        }

        #endregion
    }
}