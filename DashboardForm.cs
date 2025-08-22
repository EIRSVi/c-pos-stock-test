using System;
using System.Drawing;
using System.Windows.Forms;

namespace Stock_Room
{
    public partial class DashboardForm : Form
    {
        private UserInfo currentUser;
        private Panel sidebarPanel;
        private Panel mainPanel;
        private Panel topPanel;
        private Label welcomeLabel;
        private Label userLabel;
        private Button logoutButton;
        private Button dashboardButton;
        private Button inventoryButton;
        private Button reportsButton;
        private Button settingsButton;
        private Panel contentPanel;
        private Label contentLabel;

        public DashboardForm(UserInfo userInfo)
        {
            currentUser = userInfo;
            InitializeComponent();
            LoadDashboardContent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties - Full Desktop with no margins
            this.Text = "Stock Room Management System - Dashboard";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.Size = Screen.PrimaryScreen.Bounds.Size;
            this.BackColor = Color.FromArgb(240, 248, 255);

            // Top Panel - Full width header
            topPanel = new Panel();
            topPanel.Size = new Size(Screen.PrimaryScreen.Bounds.Width, 100);
            topPanel.Location = new Point(0, 0);
            topPanel.BackColor = Color.FromArgb(25, 118, 210);
            topPanel.Dock = DockStyle.Top;

            // Create gradient for top panel
            topPanel.Paint += (s, e) =>
            {
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    topPanel.ClientRectangle,
                    Color.FromArgb(25, 118, 210),
                    Color.FromArgb(13, 71, 161),
                    System.Drawing.Drawing2D.LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, topPanel.ClientRectangle);
                }
            };

            // Welcome Label - Larger and more prominent
            welcomeLabel = new Label();
            welcomeLabel.Text = "?? STOCK ROOM MANAGEMENT SYSTEM";
            welcomeLabel.Font = new Font("Arial", 24, FontStyle.Bold);
            welcomeLabel.ForeColor = Color.White;
            welcomeLabel.Size = new Size(800, 45);
            welcomeLabel.Location = new Point(30, 15);

            // User Label - Enhanced styling
            userLabel = new Label();
            userLabel.Text = $"?? Welcome back, {currentUser?.FullName ?? "User"} | Role: {currentUser?.UserRole ?? "User"}";
            userLabel.Font = new Font("Arial", 16);
            userLabel.ForeColor = Color.FromArgb(200, 230, 255);
            userLabel.Size = new Size(800, 30);
            userLabel.Location = new Point(30, 55);

            // Logout Button - Enhanced styling
            logoutButton = new Button();
            logoutButton.Text = "?? LOGOUT";
            logoutButton.Font = new Font("Arial", 14, FontStyle.Bold);
            logoutButton.Size = new Size(150, 45);
            logoutButton.Location = new Point(Screen.PrimaryScreen.Bounds.Width - 180, 27);
            logoutButton.BackColor = Color.FromArgb(244, 67, 54);
            logoutButton.ForeColor = Color.White;
            logoutButton.FlatStyle = FlatStyle.Flat;
            logoutButton.FlatAppearance.BorderSize = 0;
            logoutButton.Cursor = Cursors.Hand;
            logoutButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            logoutButton.Click += LogoutButton_Click;

            // Hover effects
            logoutButton.MouseEnter += (s, e) => logoutButton.BackColor = Color.FromArgb(229, 57, 53);
            logoutButton.MouseLeave += (s, e) => logoutButton.BackColor = Color.FromArgb(244, 67, 54);

            topPanel.Controls.Add(welcomeLabel);
            topPanel.Controls.Add(userLabel);
            topPanel.Controls.Add(logoutButton);

            // Sidebar Panel - Full height with modern styling
            sidebarPanel = new Panel();
            sidebarPanel.Size = new Size(280, Screen.PrimaryScreen.Bounds.Height - 100);
            sidebarPanel.Location = new Point(0, 100);
            sidebarPanel.BackColor = Color.FromArgb(33, 150, 243);
            sidebarPanel.Dock = DockStyle.Left;

            // Sidebar gradient
            sidebarPanel.Paint += (s, e) =>
            {
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    sidebarPanel.ClientRectangle,
                    Color.FromArgb(33, 150, 243),
                    Color.FromArgb(21, 101, 192),
                    System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, sidebarPanel.ClientRectangle);
                }
            };

            // Dashboard Button
            dashboardButton = CreateSidebarButton("?? Dashboard", "Main overview and statistics", 30);
            dashboardButton.Click += DashboardButton_Click;

            // Inventory Button
            inventoryButton = CreateSidebarButton("?? Inventory", "Manage stock items", 110);
            inventoryButton.Click += InventoryButton_Click;

            // Reports Button
            reportsButton = CreateSidebarButton("?? Reports", "View analytics and reports", 190);
            reportsButton.Click += ReportsButton_Click;

            // Settings Button
            settingsButton = CreateSidebarButton("?? Settings", "System configuration", 270);
            settingsButton.Click += SettingsButton_Click;

            sidebarPanel.Controls.Add(dashboardButton);
            sidebarPanel.Controls.Add(inventoryButton);
            sidebarPanel.Controls.Add(reportsButton);
            sidebarPanel.Controls.Add(settingsButton);

            // Main Panel - Full remaining space
            mainPanel = new Panel();
            mainPanel.Location = new Point(280, 100);
            mainPanel.Size = new Size(Screen.PrimaryScreen.Bounds.Width - 280, Screen.PrimaryScreen.Bounds.Height - 100);
            mainPanel.BackColor = Color.FromArgb(250, 250, 250);
            mainPanel.Dock = DockStyle.Fill;

            // Content Panel - With margins for better appearance
            contentPanel = new Panel();
            contentPanel.Size = new Size(mainPanel.Width - 60, mainPanel.Height - 60);
            contentPanel.Location = new Point(30, 30);
            contentPanel.BackColor = Color.White;
            contentPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // Add shadow to content panel
            contentPanel.Paint += (s, e) =>
            {
                // Draw shadow
                using (Brush shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, 5, 5, contentPanel.Width, contentPanel.Height);
                }
                
                // Draw border
                using (Pen borderPen = new Pen(Color.FromArgb(200, 200, 200), 1))
                {
                    e.Graphics.DrawRectangle(borderPen, 0, 0, contentPanel.Width - 1, contentPanel.Height - 1);
                }
            };

            // Content Label - Main heading
            contentLabel = new Label();
            contentLabel.Text = "Dashboard Overview";
            contentLabel.Font = new Font("Arial", 28, FontStyle.Bold);
            contentLabel.Size = new Size(600, 50);
            contentLabel.Location = new Point(40, 30);
            contentLabel.ForeColor = Color.FromArgb(25, 118, 210);

            contentPanel.Controls.Add(contentLabel);
            mainPanel.Controls.Add(contentPanel);

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

        private Button CreateSidebarButton(string text, string tooltip, int yPosition)
        {
            Button button = new Button();
            button.Text = text;
            button.Font = new Font("Arial", 16, FontStyle.Bold);
            button.Size = new Size(260, 70);
            button.Location = new Point(10, yPosition);
            button.BackColor = Color.FromArgb(30, 136, 229);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.Cursor = Cursors.Hand;
            button.Padding = new Padding(20, 0, 0, 0);

            // Add tooltip
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(button, tooltip);

            // Hover effects with smooth transitions
            button.MouseEnter += (s, e) =>
            {
                button.BackColor = Color.FromArgb(25, 118, 210);
                button.Size = new Size(270, 70);
                button.Location = new Point(5, yPosition);
            };
            
            button.MouseLeave += (s, e) =>
            {
                button.BackColor = Color.FromArgb(30, 136, 229);
                button.Size = new Size(260, 70);
                button.Location = new Point(10, yPosition);
            };

            return button;
        }

        private void DashboardForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                LogoutButton_Click(sender, e);
            }
            else if (e.KeyCode == Keys.F1)
            {
                DashboardButton_Click(sender, e);
            }
            else if (e.KeyCode == Keys.F2)
            {
                InventoryButton_Click(sender, e);
            }
            else if (e.KeyCode == Keys.F3)
            {
                ReportsButton_Click(sender, e);
            }
            else if (e.KeyCode == Keys.F4)
            {
                SettingsButton_Click(sender, e);
            }
        }

        private void LoadDashboardContent()
        {
            ClearContent();
            contentLabel.Text = "?? Dashboard Overview";
            
            // Add welcome message
            Label welcomeMsg = new Label();
            welcomeMsg.Text = $"Welcome to the Stock Room Management System, {currentUser.FullName}!\n\n" +
                             "Here's your system overview:";
            welcomeMsg.Font = new Font("Arial", 16);
            welcomeMsg.Size = new Size(800, 80);
            welcomeMsg.Location = new Point(40, 100);
            welcomeMsg.ForeColor = Color.FromArgb(66, 66, 66);
            contentPanel.Controls.Add(welcomeMsg);

            // Add enhanced summary cards with animations
            AddSummaryCard("?? Total Items", "1,248", "Items in inventory", Color.FromArgb(76, 175, 80), 40, 200);
            AddSummaryCard("?? Low Stock", "23", "Items need restock", Color.FromArgb(255, 193, 7), 320, 200);
            AddSummaryCard("?? Out of Stock", "5", "Items unavailable", Color.FromArgb(244, 67, 54), 600, 200);
            AddSummaryCard("?? Orders Today", "47", "Orders processed", Color.FromArgb(33, 150, 243), 880, 200);

            // Add recent activity section
            AddRecentActivity();
            
            SetActiveButton(dashboardButton);
        }

        private void AddSummaryCard(string title, string value, string subtitle, Color color, int x, int y)
        {
            if (x + 260 > contentPanel.Width) return; // Don't add if it won't fit

            Panel card = new Panel();
            card.Size = new Size(260, 150);
            card.Location = new Point(x, y);
            card.BackColor = color;
            card.Cursor = Cursors.Hand;

            // Add gradient to card
            card.Paint += (s, e) =>
            {
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    card.ClientRectangle,
                    color,
                    Color.FromArgb(Math.Max(0, color.R - 30), Math.Max(0, color.G - 30), Math.Max(0, color.B - 30)),
                    System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, card.ClientRectangle);
                }
            };

            Label titleLabel = new Label();
            titleLabel.Text = title;
            titleLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.Size = new Size(220, 35);
            titleLabel.Location = new Point(20, 20);
            titleLabel.BackColor = Color.Transparent;

            Label valueLabel = new Label();
            valueLabel.Text = value;
            valueLabel.Font = new Font("Arial", 32, FontStyle.Bold);
            valueLabel.ForeColor = Color.White;
            valueLabel.Size = new Size(220, 60);
            valueLabel.Location = new Point(20, 50);
            valueLabel.BackColor = Color.Transparent;

            Label subtitleLabel = new Label();
            subtitleLabel.Text = subtitle;
            subtitleLabel.Font = new Font("Arial", 10);
            subtitleLabel.ForeColor = Color.FromArgb(240, 240, 240);
            subtitleLabel.Size = new Size(220, 25);
            subtitleLabel.Location = new Point(20, 115);
            subtitleLabel.BackColor = Color.Transparent;

            // Hover effects
            card.MouseEnter += (s, e) =>
            {
                card.Size = new Size(270, 160);
                card.Location = new Point(x - 5, y - 5);
            };
            
            card.MouseLeave += (s, e) =>
            {
                card.Size = new Size(260, 150);
                card.Location = new Point(x, y);
            };

            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);
            card.Controls.Add(subtitleLabel);

            contentPanel.Controls.Add(card);
        }

        private void AddRecentActivity()
        {
            Label activityTitle = new Label();
            activityTitle.Text = "?? Recent Activity";
            activityTitle.Font = new Font("Arial", 18, FontStyle.Bold);
            activityTitle.Size = new Size(300, 30);
            activityTitle.Location = new Point(40, 380);
            activityTitle.ForeColor = Color.FromArgb(25, 118, 210);
            contentPanel.Controls.Add(activityTitle);

            string[] activities = {
                "?? Stock updated: Office Supplies - Pens (+50 units)",
                "?? New item added: Wireless Mouse Model XZ-100",
                "?? Low stock alert: Printer Paper (5 units remaining)",
                "? Order completed: IT Equipment - Keyboards (25 units)",
                "?? User login: John Doe accessed inventory system"
            };

            for (int i = 0; i < activities.Length; i++)
            {
                Label activity = new Label();
                activity.Text = $"• {activities[i]}";
                activity.Font = new Font("Arial", 12);
                activity.Size = new Size(contentPanel.Width - 100, 25);
                activity.Location = new Point(60, 420 + (i * 30));
                activity.ForeColor = Color.FromArgb(66, 66, 66);
                contentPanel.Controls.Add(activity);
            }
        }

        private void DashboardButton_Click(object sender, EventArgs e)
        {
            LoadDashboardContent();
        }

        private void InventoryButton_Click(object sender, EventArgs e)
        {
            ClearContent();
            contentLabel.Text = "?? Inventory Management";
            
            Label infoLabel = new Label();
            infoLabel.Text = "Inventory management system is under development.\n\n" +
                            "Planned features include:\n\n" +
                            "• ? Add new inventory items\n" +
                            "• ?? Update existing item details\n" +
                            "• ?? Real-time stock level monitoring\n" +
                            "• ?? Advanced search and filtering\n" +
                            "• ?? Barcode scanning integration\n" +
                            "• ?? Automatic low-stock alerts\n" +
                            "• ?? Stock movement tracking";
            infoLabel.Font = new Font("Arial", 14);
            infoLabel.Size = new Size(800, 300);
            infoLabel.Location = new Point(40, 100);
            infoLabel.ForeColor = Color.FromArgb(66, 66, 66);
            
            contentPanel.Controls.Add(infoLabel);
            SetActiveButton(inventoryButton);
        }

        private void ReportsButton_Click(object sender, EventArgs e)
        {
            ClearContent();
            contentLabel.Text = "?? Reports & Analytics";
            
            Label infoLabel = new Label();
            infoLabel.Text = "Comprehensive reporting system coming soon.\n\n" +
                            "Available report types:\n\n" +
                            "• ?? Stock Level Reports\n" +
                            "• ?? Low Stock Alerts\n" +
                            "• ?? Inventory Movement History\n" +
                            "• ?? Cost Analysis Reports\n" +
                            "• ?? Daily/Weekly/Monthly Summaries\n" +
                            "• ?? Custom Report Builder\n" +
                            "• ?? Automated Email Reports";
            infoLabel.Font = new Font("Arial", 14);
            infoLabel.Size = new Size(800, 300);
            infoLabel.Location = new Point(40, 100);
            infoLabel.ForeColor = Color.FromArgb(66, 66, 66);
            
            contentPanel.Controls.Add(infoLabel);
            SetActiveButton(reportsButton);
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            ClearContent();
            contentLabel.Text = "?? System Settings";
            
            Label infoLabel = new Label();
            infoLabel.Text = "System configuration and preferences.\n\n" +
                            "Settings categories:\n\n" +
                            "• ?? User Account Management\n" +
                            "• ?? Security & Permissions\n" +
                            "• ?? User Interface Preferences\n" +
                            "• ?? Database Configuration\n" +
                            "• ?? Backup & Restore Options\n" +
                            "• ?? Notification Settings\n" +
                            "• ?? System Maintenance Tools";
            infoLabel.Font = new Font("Arial", 14);
            infoLabel.Size = new Size(800, 300);
            infoLabel.Location = new Point(40, 100);
            infoLabel.ForeColor = Color.FromArgb(66, 66, 66);
            
            contentPanel.Controls.Add(infoLabel);
            SetActiveButton(settingsButton);
        }

        private void ClearContent()
        {
            // Clear all controls except the main content label
            for (int i = contentPanel.Controls.Count - 1; i >= 0; i--)
            {
                if (contentPanel.Controls[i] != contentLabel)
                {
                    contentPanel.Controls.RemoveAt(i);
                }
            }
        }

        private void SetActiveButton(Button activeButton)
        {
            // Reset all buttons
            dashboardButton.BackColor = Color.FromArgb(30, 136, 229);
            inventoryButton.BackColor = Color.FromArgb(30, 136, 229);
            reportsButton.BackColor = Color.FromArgb(30, 136, 229);
            settingsButton.BackColor = Color.FromArgb(30, 136, 229);

            // Highlight active button
            activeButton.BackColor = Color.FromArgb(25, 118, 210);
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show($"Are you sure you want to logout, {currentUser.FullName}?", 
                "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void DashboardForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}