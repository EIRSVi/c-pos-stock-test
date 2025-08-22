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

            // Form properties - Full Desktop with no margins
            this.Text = "Stock Room Management System";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.Size = Screen.PrimaryScreen.Bounds.Size;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.KeyPreview = true;

            // Background Panel - Full screen gradient
            backgroundPanel = new Panel();
            backgroundPanel.Dock = DockStyle.Fill;
            backgroundPanel.BackColor = Color.FromArgb(45, 45, 48);
            backgroundPanel.Paint += BackgroundPanel_Paint;

            // Get screen dimensions
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            // Left Panel - For branding and welcome
            leftPanel = new Panel();
            leftPanel.Size = new Size(screenWidth / 2, screenHeight);
            leftPanel.Location = new Point(0, 0);
            leftPanel.BackColor = Color.FromArgb(25, 118, 210);
            leftPanel.Paint += LeftPanel_Paint;

            // Right Panel - For login controls
            rightPanel = new Panel();
            rightPanel.Size = new Size(screenWidth / 2, screenHeight);
            rightPanel.Location = new Point(screenWidth / 2, 0);
            rightPanel.BackColor = Color.FromArgb(250, 250, 250);

            // Logo Panel - Large and centered on left side
            logoPanel = new Panel();
            logoPanel.Size = new Size(600, 300);
            logoPanel.Location = new Point((leftPanel.Width - 600) / 2, screenHeight / 4);
            logoPanel.BackColor = Color.Transparent;

            // Logo PictureBox - Much larger
            logoPictureBox = new PictureBox();
            logoPictureBox.Size = new Size(250, 200);
            logoPictureBox.Location = new Point(175, 20);
            logoPictureBox.BackColor = Color.FromArgb(33, 150, 243);
            logoPictureBox.BorderStyle = BorderStyle.None;
            logoPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            
            // Enhanced logo graphics
            logoPictureBox.Paint += (s, e) =>
            {
                // Draw a modern logo design
                using (Font font = new Font("Arial Black", 24, FontStyle.Bold))
                using (Brush whiteBrush = new SolidBrush(Color.White))
                using (Brush blueBrush = new SolidBrush(Color.FromArgb(13, 71, 161)))
                {
                    e.Graphics.FillRectangle(whiteBrush, 0, 0, 250, 200);
                    e.Graphics.DrawString("??", new Font("Arial", 40), blueBrush, new PointF(100, 30));
                    e.Graphics.DrawString("STOCK", font, blueBrush, new PointF(60, 90));
                    e.Graphics.DrawString("ROOM", font, blueBrush, new PointF(75, 125));
                }
            };

            // Title Label - Large and prominent
            titleLabel = new Label();
            titleLabel.Text = "STOCK ROOM\nMANAGEMENT SYSTEM";
            titleLabel.Font = new Font("Arial", 28, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.Size = new Size(500, 120);
            titleLabel.Location = new Point(50, 240);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;

            logoPanel.Controls.Add(logoPictureBox);
            logoPanel.Controls.Add(titleLabel);
            leftPanel.Controls.Add(logoPanel);

            // Login Panel - Centered on right side
            loginPanel = new Panel();
            loginPanel.Size = new Size(500, 450);
            loginPanel.Location = new Point((rightPanel.Width - 500) / 2, (screenHeight - 450) / 2);
            loginPanel.BackColor = Color.White;
            loginPanel.BorderStyle = BorderStyle.None;

            // Add shadow effect
            loginPanel.Paint += (s, e) =>
            {
                // Draw shadow
                using (Brush shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, 5, 5, loginPanel.Width - 5, loginPanel.Height - 5);
                }
            };

            // Login Title
            Label loginTitle = new Label();
            loginTitle.Text = "WELCOME BACK";
            loginTitle.Font = new Font("Arial", 24, FontStyle.Bold);
            loginTitle.ForeColor = Color.FromArgb(25, 118, 210);
            loginTitle.Size = new Size(450, 40);
            loginTitle.Location = new Point(25, 30);
            loginTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Username Label
            usernameLabel = new Label();
            usernameLabel.Text = "Username";
            usernameLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            usernameLabel.ForeColor = Color.FromArgb(66, 66, 66);
            usernameLabel.Size = new Size(150, 30);
            usernameLabel.Location = new Point(50, 100);

            // Username TextBox - Fixed for proper input
            usernameTextBox = new TextBox();
            usernameTextBox.Font = new Font("Arial", 16);
            usernameTextBox.Size = new Size(398, 38);
            usernameTextBox.Location = new Point(51, 136);
            usernameTextBox.BorderStyle = BorderStyle.FixedSingle;
            usernameTextBox.BackColor = Color.White;
            usernameTextBox.ForeColor = Color.Black;
            usernameTextBox.TabIndex = 0;
            usernameTextBox.Text = ""; // Ensure it's empty
            usernameTextBox.Enabled = true; // Ensure it's enabled
            usernameTextBox.ReadOnly = false; // Ensure it's not read-only
            usernameTextBox.Cursor = Cursors.IBeam; // Set proper cursor
            
            // Custom border for textbox - Add BEFORE the textbox
            Panel usernameBorder = new Panel();
            usernameBorder.Size = new Size(402, 42);
            usernameBorder.Location = new Point(49, 134);
            usernameBorder.BackColor = Color.FromArgb(25, 118, 210);
            usernameBorder.SendToBack(); // Ensure border stays behind
            loginPanel.Controls.Add(usernameBorder);

            // Password Label
            passwordLabel = new Label();
            passwordLabel.Text = "Password";
            passwordLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            passwordLabel.ForeColor = Color.FromArgb(66, 66, 66);
            passwordLabel.Size = new Size(150, 30);
            passwordLabel.Location = new Point(50, 200);

            // Password TextBox - Fixed for proper input
            passwordTextBox = new TextBox();
            passwordTextBox.Font = new Font("Arial", 16);
            passwordTextBox.Size = new Size(398, 38);
            passwordTextBox.Location = new Point(51, 236);
            passwordTextBox.BorderStyle = BorderStyle.FixedSingle;
            passwordTextBox.BackColor = Color.White;
            passwordTextBox.ForeColor = Color.Black;
            passwordTextBox.UseSystemPasswordChar = true;
            passwordTextBox.TabIndex = 1;
            passwordTextBox.Text = ""; // Ensure it's empty
            passwordTextBox.Enabled = true; // Ensure it's enabled
            passwordTextBox.ReadOnly = false; // Ensure it's not read-only
            passwordTextBox.Cursor = Cursors.IBeam; // Set proper cursor

            // Custom border for password textbox - Add BEFORE the textbox
            Panel passwordBorder = new Panel();
            passwordBorder.Size = new Size(402, 42);
            passwordBorder.Location = new Point(49, 234);
            passwordBorder.BackColor = Color.FromArgb(25, 118, 210);
            passwordBorder.SendToBack(); // Ensure border stays behind
            loginPanel.Controls.Add(passwordBorder);

            // Login Button - Large and modern
            loginButton = new Button();
            loginButton.Text = "LOGIN";
            loginButton.Font = new Font("Arial", 18, FontStyle.Bold);
            loginButton.Size = new Size(400, 55);
            loginButton.Location = new Point(50, 300);
            loginButton.BackColor = Color.FromArgb(25, 118, 210);
            loginButton.ForeColor = Color.White;
            loginButton.FlatStyle = FlatStyle.Flat;
            loginButton.FlatAppearance.BorderSize = 0;
            loginButton.Cursor = Cursors.Hand;
            loginButton.Click += LoginButton_Click;

            // Hover effect for login button
            loginButton.MouseEnter += (s, e) => loginButton.BackColor = Color.FromArgb(21, 101, 192);
            loginButton.MouseLeave += (s, e) => loginButton.BackColor = Color.FromArgb(25, 118, 210);

            // Exit Button - Styled
            exitButton = new Button();
            exitButton.Text = "EXIT APPLICATION";
            exitButton.Font = new Font("Arial", 14, FontStyle.Bold);
            exitButton.Size = new Size(400, 45);
            exitButton.Location = new Point(50, 370);
            exitButton.BackColor = Color.FromArgb(244, 67, 54);
            exitButton.ForeColor = Color.White;
            exitButton.FlatStyle = FlatStyle.Flat;
            exitButton.FlatAppearance.BorderSize = 0;
            exitButton.Cursor = Cursors.Hand;
            exitButton.Click += ExitButton_Click;

            // Hover effect for exit button
            exitButton.MouseEnter += (s, e) => exitButton.BackColor = Color.FromArgb(229, 57, 53);
            exitButton.MouseLeave += (s, e) => exitButton.BackColor = Color.FromArgb(244, 67, 54);

            loginPanel.Controls.Add(loginTitle);
            loginPanel.Controls.Add(usernameLabel);
            loginPanel.Controls.Add(usernameTextBox);
            loginPanel.Controls.Add(passwordLabel);
            loginPanel.Controls.Add(passwordTextBox);
            loginPanel.Controls.Add(loginButton);
            loginPanel.Controls.Add(exitButton);

            // Ensure textboxes are brought to front for input
            usernameTextBox.BringToFront();
            passwordTextBox.BringToFront();

            rightPanel.Controls.Add(loginPanel);

            // Credentials Panel - Bottom right corner
            credentialsPanel = new Panel();
            credentialsPanel.Size = new Size(450, 280);
            credentialsPanel.Location = new Point(screenWidth - 470, screenHeight - 300);
            credentialsPanel.BackColor = Color.FromArgb(255, 243, 224);
            credentialsPanel.BorderStyle = BorderStyle.None;

            // Custom border for credentials
            credentialsPanel.Paint += (s, e) =>
            {
                using (Pen borderPen = new Pen(Color.FromArgb(255, 193, 7), 3))
                {
                    e.Graphics.DrawRectangle(borderPen, 0, 0, credentialsPanel.Width - 1, credentialsPanel.Height - 1);
                }
            };

            // Credentials Title
            credentialsTitle = new Label();
            credentialsTitle.Text = "?? DEMO CREDENTIALS";
            credentialsTitle.Font = new Font("Arial", 16, FontStyle.Bold);
            credentialsTitle.Size = new Size(400, 35);
            credentialsTitle.Location = new Point(20, 15);
            credentialsTitle.ForeColor = Color.FromArgb(255, 143, 0);
            credentialsTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Credentials Information - Better formatted
            credentialsInfo = new Label();
            credentialsInfo.Text = "????? ADMINISTRATOR\n" +
                                 "   Username: admin\n" +
                                 "   Password: admin123\n\n" +
                                 "?? REGULAR USER\n" +
                                 "   Username: user1\n" +
                                 "   Password: user123\n\n" +
                                 "????? MANAGER\n" +
                                 "   Username: manager\n" +
                                 "   Password: manager123";
            credentialsInfo.Font = new Font("Arial", 13, FontStyle.Regular);
            credentialsInfo.Size = new Size(400, 210);
            credentialsInfo.Location = new Point(25, 55);
            credentialsInfo.ForeColor = Color.FromArgb(101, 87, 74);

            credentialsPanel.Controls.Add(credentialsTitle);
            credentialsPanel.Controls.Add(credentialsInfo);

            // Status Label - Positioned better
            statusLabel = new Label();
            statusLabel.Text = "";
            statusLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            statusLabel.Size = new Size(screenWidth, 40);
            statusLabel.Location = new Point(0, screenHeight - 50);
            statusLabel.ForeColor = Color.White;
            statusLabel.BackColor = Color.FromArgb(100, 244, 67, 54);
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            statusLabel.Visible = false;

            // Add all panels to background
            backgroundPanel.Controls.Add(leftPanel);
            backgroundPanel.Controls.Add(rightPanel);
            backgroundPanel.Controls.Add(credentialsPanel);
            backgroundPanel.Controls.Add(statusLabel);

            // Add background to form
            this.Controls.Add(backgroundPanel);

            // Form events
            this.KeyDown += LoginForm_KeyDown;
            this.Load += LoginForm_Load;

            // Add click handlers to help with focus issues
            loginPanel.Click += (s, e) =>
            {
                // If user clicks on login panel, focus the username textbox
                usernameTextBox.Focus();
            };
            
            // Add debugging click handler for textboxes
            usernameTextBox.Click += (s, e) =>
            {
                usernameTextBox.Focus();
                usernameTextBox.Select();
            };
            
            passwordTextBox.Click += (s, e) =>
            {
                passwordTextBox.Focus();
                passwordTextBox.Select();
            };

            this.ResumeLayout();
        }

        private void BackgroundPanel_Paint(object sender, PaintEventArgs e)
        {
            // Create gradient background
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                backgroundPanel.ClientRectangle,
                Color.FromArgb(45, 45, 48),
                Color.FromArgb(33, 33, 36),
                System.Drawing.Drawing2D.LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, backgroundPanel.ClientRectangle);
            }
        }

        private void LeftPanel_Paint(object sender, PaintEventArgs e)
        {
            // Create gradient for left panel
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                leftPanel.ClientRectangle,
                Color.FromArgb(25, 118, 210),
                Color.FromArgb(13, 71, 161),
                System.Drawing.Drawing2D.LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, leftPanel.ClientRectangle);
            }

            // Add decorative elements
            using (var pen = new Pen(Color.FromArgb(100, 255, 255, 255), 2))
            {
                // Draw some decorative lines
                for (int i = 0; i < 5; i++)
                {
                    e.Graphics.DrawLine(pen, 0, i * 100, leftPanel.Width, i * 100);
                }
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Test database connection
            if (!dbHelper.TestConnection())
            {
                ShowStatus("Demo Mode: Database not connected. Using test credentials below.", Color.FromArgb(255, 193, 7));
            }
            else
            {
                ShowStatus("Connected to database successfully.", Color.FromArgb(76, 175, 80));
            }
            
            // Ensure textboxes are properly set up for input
            usernameTextBox.Focus();
            usernameTextBox.Select();
            usernameTextBox.TabIndex = 0;
            passwordTextBox.TabIndex = 1;
            loginButton.TabIndex = 2;
            exitButton.TabIndex = 3;
            
            // Add Enter key handling for textboxes
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

            // Add focus events for visual feedback
            usernameTextBox.Enter += (s, e) =>
            {
                usernameTextBox.BackColor = Color.FromArgb(240, 248, 255);
            };
            
            usernameTextBox.Leave += (s, e) =>
            {
                usernameTextBox.BackColor = Color.White;
            };
            
            passwordTextBox.Enter += (s, e) =>
            {
                passwordTextBox.BackColor = Color.FromArgb(240, 248, 255);
            };
            
            passwordTextBox.Leave += (s, e) =>
            {
                passwordTextBox.BackColor = Color.White;
            };
        }

        private void ShowStatus(string message, Color color)
        {
            statusLabel.Text = message;
            statusLabel.BackColor = Color.FromArgb(200, color.R, color.G, color.B);
            statusLabel.Visible = true;
            
            // Auto-hide after 3 seconds
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 3000;
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
            else if (e.KeyCode == Keys.F11)
            {
                // F11 to toggle (currently always fullscreen)
                MessageBox.Show("Application is running in full desktop mode", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowStatus("Please enter both username and password.", Color.FromArgb(244, 67, 54));
                return;
            }

            ShowStatus("Authenticating user...", Color.FromArgb(33, 150, 243));

            try
            {
                if (dbHelper.ValidateUser(username, password))
                {
                    UserInfo userInfo = dbHelper.GetUserInfo(username);
                    
                    ShowStatus($"Welcome {userInfo.FullName}! Loading dashboard...", Color.FromArgb(76, 175, 80));
                    
                    // Add delay for better UX
                    System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                    timer.Interval = 1000;
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
                    ShowStatus("Invalid credentials. Please check username and password.", Color.FromArgb(244, 67, 54));
                    passwordTextBox.Clear();
                    usernameTextBox.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowStatus("Login failed. Please try again.", Color.FromArgb(244, 67, 54));
                MessageBox.Show($"Error: {ex.Message}", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit?", "Exit Application", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}