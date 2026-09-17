using System;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeSalaryManagement.Forms
{
    /// <summary>
    /// Login window allowing administrators to authenticate into the system.
    /// Demo credentials: admin / admin123
    /// </summary>
    public class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnClear;
        private Button btnExit;
        private CheckBox chkShowPassword;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Login - Employee Salary Management";
            this.Size = new Size(460, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(243, 246, 250);
            this.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

            // Header Banner
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = Color.FromArgb(26, 54, 93) // Navy Blue
            };

            Label lblAppTitle = new Label
            {
                Text = "APEX PAYROLL SYSTEM",
                ForeColor = Color.FromArgb(219, 234, 254),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            Label lblProjectName = new Label
            {
                Text = "Salary Management & Payslip System",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
                Location = new Point(20, 42),
                AutoSize = true
            };

            Label lblSubtitle = new Label
            {
                Text = "Administrator Authentication",
                ForeColor = Color.FromArgb(191, 219, 254),
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                Location = new Point(22, 73),
                AutoSize = true
            };

            pnlHeader.Controls.AddRange(new Control[] { lblAppTitle, lblProjectName, lblSubtitle });
            this.Controls.Add(pnlHeader);

            // Card Panel
            Panel pnlCard = new Panel
            {
                Location = new Point(30, 130),
                Size = new Size(385, 330),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblInstruction = new Label
            {
                Text = "Please enter your administrator credentials to sign in.",
                ForeColor = Color.FromArgb(71, 85, 105),
                Font = new Font("Segoe UI", 9f),
                Location = new Point(20, 15),
                Size = new Size(340, 30)
            };

            // Username
            Label lblUser = new Label
            {
                Text = "Username",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 52),
                AutoSize = true
            };

            txtUsername = new TextBox
            {
                Location = new Point(20, 75),
                Size = new Size(340, 28),
                Font = new Font("Segoe UI", 10.5f),
                Text = "admin"
            };

            // Password
            Label lblPass = new Label
            {
                Text = "Password",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 115),
                AutoSize = true
            };

            txtPassword = new TextBox
            {
                Location = new Point(20, 138),
                Size = new Size(340, 28),
                Font = new Font("Segoe UI", 10.5f),
                PasswordChar = '●',
                Text = "admin123"
            };

            // Show password checkbox
            chkShowPassword = new CheckBox
            {
                Text = "Show Password",
                Location = new Point(20, 172),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(100, 116, 139)
            };
            chkShowPassword.CheckedChanged += (s, e) =>
            {
                txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '●';
            };

            // Buttons
            btnLogin = new Button
            {
                Text = "Sign In",
                Location = new Point(20, 205),
                Size = new Size(340, 38),
                BackColor = Color.FromArgb(37, 99, 235), // Royal Blue
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            btnClear = new Button
            {
                Text = "Clear",
                Location = new Point(20, 252),
                Size = new Size(165, 32),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(71, 85, 105),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnClear.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnClear.Click += (s, e) =>
            {
                txtUsername.Clear();
                txtPassword.Clear();
                txtUsername.Focus();
            };

            btnExit = new Button
            {
                Text = "Exit System",
                Location = new Point(195, 252),
                Size = new Size(165, 32),
                BackColor = Color.FromArgb(254, 242, 242),
                ForeColor = Color.FromArgb(185, 28, 28),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnExit.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
            btnExit.Click += (s, e) => Application.Exit();

            Label lblDemoHint = new Label
            {
                Text = "Default Login: admin  |  Password: admin123",
                ForeColor = Color.FromArgb(100, 116, 139),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                Location = new Point(20, 295),
                Size = new Size(340, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnlCard.Controls.AddRange(new Control[] {
                lblInstruction, lblUser, txtUsername, lblPass, txtPassword,
                chkShowPassword, btnLogin, btnClear, btnExit, lblDemoHint
            });

            this.Controls.Add(pnlCard);

            // Enter key triggers login
            this.AcceptButton = btnLogin;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (username == "admin" && password == "admin123")
            {
                this.Hide();
                using (var dashboard = new DashboardForm())
                {
                    dashboard.ShowDialog();
                }
                // When dashboard closes, reset fields and show login again
                txtPassword.Text = "admin123";
                this.Show();
            }
            else
            {
                MessageBox.Show("Invalid Username or Password!\n\nPlease use demo credentials:\nUsername: admin\nPassword: admin123",
                    "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.SelectAll();
                txtPassword.Focus();
            }
        }
    }
}
