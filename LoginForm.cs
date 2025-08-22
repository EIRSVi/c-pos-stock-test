using System;
using System.Drawing;
using System.Windows.Forms;

namespace Stock_Room
{
    public partial class LoginForm : Form
    {
        private DatabaseHelper dbHelper;
        private Panel backgroundPanel;
        private Panel logoPanel;
        private PictureBox logoPictureBox;
        private Label titleLabel;
        private Panel loginPanel;
        private Label usernameLabel;
        private TextBox usernameTextBox;
        private Label passwordLabel;
        private TextBox passwordTextBox;
        private Button loginButton;
        private Button exitButton;
        private Label statusLabel;
        private Panel credentialsPanel;
        private Label credentialsTitle;
        private Label credentialsInfo;
        private Panel leftPanel;
        private Panel rightPanel;

        public LoginForm()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties - Full Desktop with modern design
            this.Text = "Stock Room Management System";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.Size = Screen.PrimaryScreen.Bounds.Size;
            this.BackColor = Color.FromArgb(15, 23, 42);
            this.KeyPreview = true;

            // Background Panel - Modern gradient
            backgroundPanel = new Panel();
            backgroundPanel.Dock = DockStyle.Fill;
            backgroundPanel.BackColor = Color.FromArgb(15, 23, 42);
            backgroundPanel.Paint += BackgroundPanel_Paint;

            // Get screen dimensions
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            // Left Panel - Enhanced branding
            leftPanel = new Panel();
            leftPanel.Size = new Size(screenWidth / 2, screenHeight);
            leftPanel.Location = new Point(0, 0);
            leftPanel.BackColor = Color.FromArgb(37, 99, 235);
            leftPanel.Paint += LeftPanel_Paint;

            // Right Panel - Clean login area
            rightPanel = new Panel();
            rightPanel.Size = new Size(screenWidth / 2, screenHeight);
            rightPanel.Location = new Point(screenWidth / 2, 0);
            rightPanel.BackColor = Color.FromArgb(248, 250, 252);

            // Enhanced Logo Panel
            logoPanel = new Panel();
            logoPanel.Size = new Size(600, 400);
            logoPanel.Location = new Point((leftPanel.Width - 600) / 2, (screenHeight - 400) / 2 - 50);
            logoPanel.BackColor = Color.Transparent;

            // Modern Logo Design
            logoPictureBox = new PictureBox();
            logoPictureBox.Size = new Size(200, 160);
            logoPictureBox.Location = new Point(200, 30);
            logoPictureBox.BackColor = Color.White;
            logoPictureBox.BorderStyle = BorderStyle.None;
            logoPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            
            // Enhanced modern logo
            logoPictureBox.Paint += (s, e) =>
            {
                // Create modern logo with shadow effect
                using (var shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
                {
                    e.Graphics.FillEllipse(shadowBrush, 5, 5, 190, 150);
                }
                
                using (var backgroundBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Rectangle(0, 0, 200, 160),
                    Color.White,
                    Color.FromArgb(248, 250, 252),
                    System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                {
                    e.Graphics.FillEllipse(backgroundBrush, 0, 0, 190, 150);
                }
                
                // Draw modern icon
                using (Font iconFont = new Font("Arial", 48, FontStyle.Bold))
                using (Brush iconBrush = new SolidBrush(Color.FromArgb(37, 99, 235)))
                {
                    e.Graphics.DrawString("??", iconFont, iconBrush, new PointF(70, 30));
                }
                
                using (Font titleFont = new Font("Arial", 14, FontStyle.Bold))
                using (Brush titleBrush = new SolidBrush(Color.FromArgb(15, 23, 42)))
                {
                    e.Graphics.DrawString("STOCK", titleFont, titleBrush, new PointF(70, 90));
                    e.Graphics.DrawString("ROOM", titleFont, titleBrush, new PointF(75, 110));
                }
            };

            // Enhanced Title Label
            titleLabel = new Label();
            titleLabel.Text = "?? STOCK ROOM\nMANAGEMENT SYSTEM";
            titleLabel.Font = new Font("Arial", 32, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.Size = new Size(500, 120);
            titleLabel.Location = new Point(50, 220);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;

            // Add subtitle
            Label subtitleLabel = new Label();
            subtitleLabel.Text = "?? Modern inventory management made simple";
            subtitleLabel.Font = new Font("Arial", 16);
            subtitleLabel.ForeColor = Color.FromArgb(219, 234, 254);
            subtitleLabel.Size = new Size(500, 30);
            subtitleLabel.Location = new Point(50, 340);
            subtitleLabel.TextAlign = ContentAlignment.MiddleCenter;

            logoPanel.Controls.Add(logoPictureBox);
            logoPanel.Controls.Add(titleLabel);
            logoPanel.Controls.Add(subtitleLabel);
            leftPanel.Controls.Add(logoPanel);

            // Modern Login Panel
            loginPanel = new Panel();
            loginPanel.Size = new Size(480, 520);
            loginPanel.Location = new Point((rightPanel.Width - 480) / 2, (screenHeight - 520) / 2);
            loginPanel.BackColor = Color.White;
            loginPanel.BorderStyle = BorderStyle.None;

            // Enhanced shadow and styling
            loginPanel.Paint += (s, e) =>
            {
                // Multi-layer shadow for depth
                for (int i = 1; i <= 8; i++)
                {
                    using (Brush shadowBrush = new SolidBrush(Color.FromArgb(5, 0, 0, 0)))
                    {
                        e.Graphics.FillRectangle(shadowBrush, i, i, loginPanel.Width, loginPanel.Height);
                    }
                }
                
                // Subtle border
                using (Pen borderPen = new Pen(Color.FromArgb(229, 231, 235), 1))
                {
                    e.Graphics.DrawRectangle(borderPen, 0, 0, loginPanel.Width - 1, loginPanel.Height - 1);
                }
            };

            // Welcome Header
            Label loginTitle = new Label();
            loginTitle.Text = "?? WELCOME BACK";
            loginTitle.Font = new Font("Arial", 28, FontStyle.Bold);
            loginTitle.ForeColor = Color.FromArgb(15, 23, 42);
            loginTitle.Size = new Size(420, 50);
            loginTitle.Location = new Point(30, 40);
            loginTitle.TextAlign = ContentAlignment.MiddleCenter;

            Label loginSubtitle = new Label();
            loginSubtitle.Text = "Please sign in to your account";
            loginSubtitle.Font = new Font("Arial", 14);
            loginSubtitle.ForeColor = Color.FromArgb(107, 114, 128);
            loginSubtitle.Size = new Size(420, 25);
            loginSubtitle.Location = new Point(30, 90);
            loginSubtitle.TextAlign = ContentAlignment.MiddleCenter;

            // Enhanced Username Section
            usernameLabel = new Label();
            usernameLabel.Text = "?? Username";
            usernameLabel.Font = new Font("Arial", 14, FontStyle.Regular);
            usernameLabel.ForeColor = Color.FromArgb(55, 65, 81);
            usernameLabel.SetBounds(40, 140, 150, 25);

            // Modern text input styling
            Panel usernameBorder = new Panel();
            usernameBorder.Size = new Size(402, 42);
            usernameBorder.Location = new Point(39, 169);
            usernameBorder.BackColor = Color.FromArgb(37, 99, 235);

            usernameTextBox = new TextBox();
            usernameTextBox.Font = new Font("Arial", 14);
            usernameTextBox.Size = new Size(398, 38);
            usernameTextBox.Location = new Point(41, 171);
            usernameTextBox.BorderStyle = BorderStyle.FixedSingle;
            usernameTextBox.BackColor = Color.White;
            usernameTextBox.ForeColor = Color.FromArgb(31, 41, 55);
            usernameTextBox.TabIndex = 0;
            usernameTextBox.Text = "";
            usernameTextBox.Enabled = true;
            usernameTextBox.ReadOnly = false;
            usernameTextBox.Cursor = Cursors.IBeam;

            // Enhanced Password Section
            passwordLabel = new Label();
            passwordLabel.Text = "?? Password";
            passwordLabel.Font = new Font("Arial", 14, FontStyle.Regular);
            passwordLabel.ForeColor = Color.FromArgb(55, 65, 81);
            passwordLabel.SetBounds(40, 230, 150, 25);

            Panel passwordBorder = new Panel();
            passwordBorder.Size = new Size(402, 42);
            passwordBorder.Location = new Point(39, 259);
            passwordBorder.BackColor = Color.FromArgb(37, 99, 235);

            passwordTextBox = new TextBox();
            passwordTextBox.Font = new Font("Arial", 14);
            passwordTextBox.Size = new Size(398, 38);
            passwordTextBox.Location = new Point(41, 261);
            passwordTextBox.BorderStyle = BorderStyle.FixedSingle;
            passwordTextBox.BackColor = Color.White;
            passwordTextBox.ForeColor = Color.FromArgb(31, 41, 55);
            passwordTextBox.UseSystemPasswordChar = true;
            passwordTextBox.TabIndex = 1;
            passwordTextBox.Text = "";
            passwordTextBox.Enabled = true;
            passwordTextBox.ReadOnly = false;
            passwordTextBox.Cursor = Cursors.IBeam;

            // Modern Login Button
            loginButton = new Button();
            loginButton.Text = "?? SIGN IN";
            loginButton.Font = new Font("Arial", 16, FontStyle.Bold);
            loginButton.Size = new Size(400, 50);
            loginButton.Location = new Point(40, 330);
            loginButton.BackColor = Color.FromArgb(37, 99, 235);
            loginButton.ForeColor = Color.White;
            loginButton.FlatStyle = FlatStyle.Flat;
            loginButton.FlatAppearance.BorderSize = 0;
            loginButton.Cursor = Cursors.Hand;
            loginButton.Click += LoginButton_Click;

            // Modern Exit Button
            exitButton = new Button();
            exitButton.Text = "? EXIT APPLICATION";
            exitButton.Font = new Font("Arial", 14);
            exitButton.Size = new Size(400, 45);
            exitButton.Location = new Point(40, 395);
            exitButton.BackColor = Color.FromArgb(239, 68, 68);
            exitButton.ForeColor = Color.White;
            exitButton.FlatStyle = FlatStyle.Flat;
            exitButton.FlatAppearance.BorderSize = 0;
            exitButton.Cursor = Cursors.Hand;
            exitButton.Click += ExitButton_Click;

            // Enhanced hover effects
            loginButton.MouseEnter += (s, e) => {
                loginButton.BackColor = Color.FromArgb(29, 78, 216);
            };
            loginButton.MouseLeave += (s, e) => {
                loginButton.BackColor = Color.FromArgb(37, 99, 235);
            };

            exitButton.MouseEnter += (s, e) => {
                exitButton.BackColor = Color.FromArgb(220, 38, 38);
            };
            exitButton.MouseLeave += (s, e) => {
                exitButton.BackColor = Color.FromArgb(239, 68, 68);
            };

            // Add controls to login panel
            loginPanel.Controls.Add(usernameBorder);
            loginPanel.Controls.Add(passwordBorder);
            loginPanel.Controls.Add(loginTitle);
            loginPanel.Controls.Add(loginSubtitle);
            loginPanel.Controls.Add(usernameLabel);
            loginPanel.Controls.Add(usernameTextBox);
            loginPanel.Controls.Add(passwordLabel);
            loginPanel.Controls.Add(passwordTextBox);
            loginPanel.Controls.Add(loginButton);
            loginPanel.Controls.Add(exitButton);

            usernameTextBox.BringToFront();
            passwordTextBox.BringToFront();

            rightPanel.Controls.Add(loginPanel);

            // Enhanced Credentials Panel
            credentialsPanel = new Panel();
            credentialsPanel.Size = new Size(440, 300);
            credentialsPanel.Location = new Point(screenWidth - 460, screenHeight - 320);
            credentialsPanel.BackColor = Color.White;
            credentialsPanel.BorderStyle = BorderStyle.None;

            credentialsPanel.Paint += (s, e) =>
            {
                // Shadow effect
                for (int i = 1; i <= 4; i++)
                {
                    using (Brush shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                    {
                        e.Graphics.FillRectangle(shadowBrush, i, i, credentialsPanel.Width, credentialsPanel.Height);
                    }
                }
                
                // Colored top border
                using (Brush borderBrush = new SolidBrush(Color.FromArgb(245, 158, 11)))
                {
                    e.Graphics.FillRectangle(borderBrush, 0, 0, credentialsPanel.Width, 4);
                }
                
                // Main border
                using (Pen borderPen = new Pen(Color.FromArgb(229, 231, 235), 1))
                {
                    e.Graphics.DrawRectangle(borderPen, 0, 0, credentialsPanel.Width - 1, credentialsPanel.Height - 1);
                }
            };

            credentialsTitle = new Label();
            credentialsTitle.Text = "?? DEMO CREDENTIALS";
            credentialsTitle.Font = new Font("Arial", 16, FontStyle.Bold);
            credentialsTitle.Size = new Size(400, 30);
            credentialsTitle.Location = new Point(20, 20);
            credentialsTitle.ForeColor = Color.FromArgb(245, 158, 11);
            credentialsTitle.TextAlign = ContentAlignment.MiddleCenter;

            credentialsInfo = new Label();
            credentialsInfo.Text = "?? ADMINISTRATOR\n" +
                                 "   Username: admin\n" +
                                 "   Password: admin123\n\n" +
                                 "?? REGULAR USER\n" +
                                 "   Username: user1\n" +
                                 "   Password: user123\n\n" +
                                 "?? MANAGER\n" +
                                 "   Username: manager\n" +
                                 "   Password: manager123";
            credentialsInfo.Font = new Font("Arial", 12);
            credentialsInfo.Size = new Size(400, 220);
            credentialsInfo.Location = new Point(25, 60);
            credentialsInfo.ForeColor = Color.FromArgb(75, 85, 99);

            credentialsPanel.Controls.Add(credentialsTitle);
            credentialsPanel.Controls.Add(credentialsInfo);

            // Enhanced Status Label
            statusLabel = new Label();
            statusLabel.Text = "";
            statusLabel.Font = new Font("Arial", 14, FontStyle.Regular);
            statusLabel.Size = new Size(screenWidth, 50);
            statusLabel.Location = new Point(0, screenHeight - 60);
            statusLabel.ForeColor = Color.White;
            statusLabel.BackColor = Color.FromArgb(180, 239, 68, 68);
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            statusLabel.Visible = false;

            // Add all panels to background
            backgroundPanel.Controls.Add(leftPanel);
            backgroundPanel.Controls.Add(rightPanel);
            backgroundPanel.Controls.Add(credentialsPanel);
            backgroundPanel.Controls.Add(statusLabel);

            this.Controls.Add(backgroundPanel);

            // Form events
            this.KeyDown += LoginForm_KeyDown;
            this.Load += LoginForm_Load;

            // Focus handling
            loginPanel.Click += (s, e) => usernameTextBox.Focus();
            usernameTextBox.Click += (s, e) => { usernameTextBox.Focus(); usernameTextBox.Select(); };
            passwordTextBox.Click += (s, e) => { passwordTextBox.Focus(); passwordTextBox.Select(); };

            this.ResumeLayout();
        }

        private void BackgroundPanel_Paint(object sender, PaintEventArgs e)
        {
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                backgroundPanel.ClientRectangle,
                Color.FromArgb(15, 23, 42),
                Color.FromArgb(30, 41, 59),
                System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(brush, backgroundPanel.ClientRectangle);
            }
        }

        private void LeftPanel_Paint(object sender, PaintEventArgs e)
        {
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                leftPanel.ClientRectangle,
                Color.FromArgb(37, 99, 235),
                Color.FromArgb(29, 78, 216),
                System.Drawing.Drawing2D.LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, leftPanel.ClientRectangle);
            }

            // Add modern geometric patterns
            using (var patternBrush = new SolidBrush(Color.FromArgb(20, 255, 255, 255)))
            {
                for (int i = 0; i < leftPanel.Height; i += 100)
                {
                    e.Graphics.FillEllipse(patternBrush, -50, i, 200, 200);
                    e.Graphics.FillEllipse(patternBrush, leftPanel.Width - 150, i + 50, 200, 200);
                }
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            if (!dbHelper.IsDatabaseAvailable())
            {
                ShowStatus("?? Demo Mode: Database not connected. Using demo credentials.", Color.FromArgb(245, 158, 11));
            }
            else
            {
                ShowStatus("? Connected to database successfully.", Color.FromArgb(34, 197, 94));
            }
            
            usernameTextBox.Focus();
            usernameTextBox.Select();
            usernameTextBox.TabIndex = 0;
            passwordTextBox.TabIndex = 1;
            loginButton.TabIndex = 2;
            exitButton.TabIndex = 3;
            
            // Enhanced keyboard navigation
            usernameTextBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    passwordTextBox.Focus();
                    e.Handled = true;
                }
            };
            
            passwordTextBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoginButton_Click(passwordTextBox, e);
                    e.Handled = true;
                }
            };

            // Enhanced visual feedback
            usernameTextBox.Enter += (s, e) => usernameTextBox.BackColor = Color.FromArgb(239, 246, 255);
            usernameTextBox.Leave += (s, e) => usernameTextBox.BackColor = Color.White;
            passwordTextBox.Enter += (s, e) => passwordTextBox.BackColor = Color.FromArgb(239, 246, 255);
            passwordTextBox.Leave += (s, e) => passwordTextBox.BackColor = Color.White;
        }

        private void ShowStatus(string message, Color color)
        {
            statusLabel.Text = message;
            statusLabel.BackColor = Color.FromArgb(220, color.R, color.G, color.B);
            statusLabel.Visible = true;
            
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 4000;
            timer.Tick += (s, e) =>
            {
                statusLabel.Visible = false;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoginButton_Click(sender, e);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                ExitButton_Click(sender, e);
            }
            else if (e.KeyCode == Keys.F1)
            {
                MessageBox.Show("?? Stock Room Management System\n\n" +
                              "?? Use the demo credentials shown in the bottom right corner\n" +
                              "?? Press Enter to login or Esc to exit\n" +
                              "??? Click on any input field to start typing",
                              "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowStatus("?? Please enter both username and password.", Color.FromArgb(239, 68, 68));
                return;
            }

            ShowStatus("?? Authenticating user...", Color.FromArgb(37, 99, 235));

            try
            {
                // Check if database is available first
                if (!dbHelper.IsDatabaseAvailable())
                {
                    // Demo mode - allow hardcoded credentials
                    if ((username == "admin" && password == "admin123") ||
                        (username == "user1" && password == "user123") ||
                        (username == "manager" && password == "manager123"))
                    {
                        var demoUser = new UserInfo
                        {
                            UserID = 1,
                            Username = username,
                            FullName = username == "admin" ? "Demo Administrator" :
                                      username == "manager" ? "Demo Manager" : "Demo User",
                            UserRole = username == "admin" ? "Admin" :
                                      username == "manager" ? "Manager" : "User",
                            Status = "Active"
                        };

                        ShowStatus($"? Welcome {demoUser.FullName}! (Demo Mode)", Color.FromArgb(34, 197, 94));
                        
                        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                        timer.Interval = 1500;
                        timer.Tick += (s, args) =>
                        {
                            timer.Stop();
                            timer.Dispose();
                            
                            this.Hide();
                            DashboardForm dashboard = new DashboardForm(demoUser);
                            dashboard.ShowDialog();
                            this.Close();
                        };
                        timer.Start();
                        return;
                    }
                    else
                    {
                        ShowStatus("? Invalid demo credentials. Check the demo panel for valid usernames/passwords.", Color.FromArgb(239, 68, 68));
                        passwordTextBox.Clear();
                        usernameTextBox.Focus();
                        return;
                    }
                }

                // Normal database authentication
                var userInfo = await dbHelper.ValidateUserAsync(username, password);
                
                if (userInfo != null)
                {
                    ShowStatus($"? Welcome {userInfo.FullName}! Loading dashboard...", Color.FromArgb(34, 197, 94));
                    
                    System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                    timer.Interval = 1500;
                    timer.Tick += (s, args) =>
                    {
                        timer.Stop();
                        timer.Dispose();
                        
                        this.Hide();
                        DashboardForm dashboard = new DashboardForm(userInfo);
                        dashboard.ShowDialog();
                        this.Close();
                    };
                    timer.Start();
                }
                else
                {
                    ShowStatus("? Invalid credentials. Please check username and password.", Color.FromArgb(239, 68, 68));
                    passwordTextBox.Clear();
                    usernameTextBox.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowStatus("?? Login failed. Please try again.", Color.FromArgb(239, 68, 68));
                MessageBox.Show($"Error: {ex.Message}\n\nIf database is not available, use demo credentials shown on screen.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("?? Are you sure you want to exit the application?", 
                "Exit Application", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}