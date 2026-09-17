using System;
using System.Drawing;
using System.Windows.Forms;
using EmployeeSalaryManagement.Data;

namespace EmployeeSalaryManagement.Forms
{
    /// <summary>
    /// Main Dashboard presenting project metrics, quick action cards,
    /// and navigation to all core modules.
    /// </summary>
    public class DashboardForm : Form
    {
        private Label lblTotalEmployeesCount;
        private Label lblTotalRecordsCount;
        private Label lblClock;
        private Timer clockTimer;

        public DashboardForm()
        {
            InitializeComponent();
            RefreshStats();
        }

        private void InitializeComponent()
        {
            this.Text = "Dashboard - Employee Salary Management and Payslip System";
            this.Size = new Size(950, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(243, 246, 250);
            this.Font = new Font("Segoe UI", 9.5f);

            // 1. Top Header Banner
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.FromArgb(26, 54, 93) // Navy
            };

            Label lblCollegeBadge = new Label
            {
                Text = "COLLEGE MINI PROJECT",
                ForeColor = Color.FromArgb(147, 197, 253),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Location = new Point(30, 18),
                AutoSize = true
            };

            Label lblTitle = new Label
            {
                Text = "Employee Salary Management & Payslip System",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                Location = new Point(30, 36),
                AutoSize = true
            };

            lblClock = new Label
            {
                Text = DateTime.Now.ToString("dddd, dd MMMM yyyy  HH:mm:ss"),
                ForeColor = Color.FromArgb(224, 231, 255),
                Font = new Font("Segoe UI", 9.5f),
                Location = new Point(30, 68),
                AutoSize = true
            };

            Label lblAdminBadge = new Label
            {
                Text = "Logged in: Administrator",
                ForeColor = Color.FromArgb(254, 240, 138),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Location = new Point(730, 38),
                AutoSize = true
            };

            pnlHeader.Controls.AddRange(new Control[] { lblCollegeBadge, lblTitle, lblClock, lblAdminBadge });
            this.Controls.Add(pnlHeader);

            // Clock timer
            clockTimer = new Timer { Interval = 1000 };
            clockTimer.Tick += (s, e) => { lblClock.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy  HH:mm:ss"); };
            clockTimer.Start();

            // 2. Metrics & Rules Row
            Panel pnlMetricsContainer = new Panel
            {
                Location = new Point(30, 115),
                Size = new Size(880, 110),
                BackColor = Color.Transparent
            };

            // Stat Card 1: Total Employees
            Panel cardEmployees = CreateMetricCard("Total Employees", "0", Color.FromArgb(37, 99, 235), 0, out lblTotalEmployeesCount);
            // Stat Card 2: Total Salary Records
            Panel cardRecords = CreateMetricCard("Salary Records Saved", "0", Color.FromArgb(16, 185, 129), 295, out lblTotalRecordsCount);

            // Stat Card 3: Formula overview
            Panel cardFormula = new Panel
            {
                Location = new Point(590, 0),
                Size = new Size(290, 105),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblRuleTitle = new Label
            {
                Text = "Statutory Calculation Rates",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(12, 10),
                AutoSize = true
            };

            Label lblRuleDetails = new Label
            {
                Text = "• HRA: 20% | DA: 10% of Basic\n• PF: 8% | Tax: 5% of Basic\n• Net = (Basic + HRA + DA) - (PF + Tax)",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(12, 34),
                Size = new Size(265, 60)
            };
            cardFormula.Controls.AddRange(new Control[] { lblRuleTitle, lblRuleDetails });

            pnlMetricsContainer.Controls.AddRange(new Control[] { cardEmployees, cardRecords, cardFormula });
            this.Controls.Add(pnlMetricsContainer);

            // 3. Action Cards Grid
            Label lblSection = new Label
            {
                Text = "System Modules & Actions",
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(30, 235),
                AutoSize = true
            };
            this.Controls.Add(lblSection);

            // Card 1: Add Employee
            Button btnAddEmployee = CreateActionCard("Add & Manage Employee",
                "Register new employees, update details, assign departments, and manage records.",
                "Manage Employees", Color.FromArgb(37, 99, 235), 30, 270);
            btnAddEmployee.Click += (s, e) =>
            {
                using (var empForm = new EmployeeForm())
                {
                    empForm.ShowDialog();
                }
                RefreshStats();
            };

            // Card 2: Calculate Salary
            Button btnCalculateSalary = CreateActionCard("Calculate Salary & Payslip",
                "Select employee, compute HRA, DA, PF, Tax, and generate professional printable payslip.",
                "Calculate Salary", Color.FromArgb(16, 185, 129), 325, 270);
            btnCalculateSalary.Click += (s, e) =>
            {
                using (var salForm = new SalaryForm())
                {
                    salForm.ShowDialog();
                }
                RefreshStats();
            };

            // Card 3: Salary Records
            Button btnRecords = CreateActionCard("Salary Records History",
                "Search, review, filter, and manage historical salary transactions and deductions.",
                "View Records", Color.FromArgb(139, 92, 246), 620, 270);
            btnRecords.Click += (s, e) =>
            {
                using (var recForm = new RecordsForm())
                {
                    recForm.ShowDialog();
                }
                RefreshStats();
            };

            // Card 4: About
            Button btnAbout = CreateActionCard("About This Project",
                "View project objectives, developer details, architecture, and technology stack info.",
                "View About", Color.FromArgb(14, 165, 233), 30, 420);
            btnAbout.Click += (s, e) => ShowAboutDialog();

            // Card 5: Logout
            Button btnLogout = CreateActionCard("Sign Out / Logout",
                "Securely close current administrator session and return to the login screen.",
                "Logout", Color.FromArgb(239, 68, 68), 325, 420);
            btnLogout.Click += (s, e) =>
            {
                var res = MessageBox.Show("Are you sure you want to log out?", "Confirm Logout",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    this.Close();
                }
            };

            // Card 6: Database Location Info
            Panel pnlDbInfo = new Panel
            {
                Location = new Point(620, 420),
                Size = new Size(280, 130),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblDbTitle = new Label
            {
                Text = "Database Status: Active",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(16, 185, 129),
                Location = new Point(15, 15),
                AutoSize = true
            };

            Label lblDbDesc = new Label
            {
                Text = "Engine: SQLite (System.Data.SQLite)\nStorage: Database/salary.db\nTables: Employees, SalaryRecords\nAuto-created on start.",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(15, 40),
                Size = new Size(250, 75)
            };
            pnlDbInfo.Controls.AddRange(new Control[] { lblDbTitle, lblDbDesc });

            this.Controls.AddRange(new Control[] {
                btnAddEmployee, btnCalculateSalary, btnRecords, btnAbout, btnLogout, pnlDbInfo
            });

            // Status Bar at Bottom
            Panel pnlFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 35,
                BackColor = Color.FromArgb(226, 232, 240)
            };

            Label lblFooter = new Label
            {
                Text = "Employee Salary Management and Payslip Generation System | Visual Studio & SQLite Compatible",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(20, 9),
                AutoSize = true
            };
            pnlFooter.Controls.Add(lblFooter);
            this.Controls.Add(pnlFooter);
        }

        private Panel CreateMetricCard(string title, string initialValue, Color accentColor, int left, out Label valueLabel)
        {
            Panel card = new Panel
            {
                Location = new Point(left, 0),
                Size = new Size(280, 105),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Panel accentBar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 6,
                BackColor = accentColor
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(18, 12),
                AutoSize = true
            };

            valueLabel = new Label
            {
                Text = initialValue,
                Font = new Font("Segoe UI", 24f, FontStyle.Bold),
                ForeColor = accentColor,
                Location = new Point(18, 35),
                AutoSize = true
            };

            card.Controls.AddRange(new Control[] { accentBar, lblTitle, valueLabel });
            return card;
        }

        private Button CreateActionCard(string title, string description, string buttonText, Color themeColor, int x, int y)
        {
            Button cardBtn = new Button
            {
                Location = new Point(x, y),
                Size = new Size(280, 130),
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.TopLeft,
                Padding = new Padding(12)
            };
            cardBtn.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            cardBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(248, 250, 252);

            // Sub-elements as non-interactive controls inside button
            Panel topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 4,
                BackColor = themeColor,
                Enabled = false
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(12, 14),
                AutoSize = true,
                Enabled = false
            };

            Label lblDesc = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(12, 38),
                Size = new Size(250, 48),
                Enabled = false
            };

            Label lblAction = new Label
            {
                Text = "→ " + buttonText,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = themeColor,
                Location = new Point(12, 95),
                AutoSize = true,
                Enabled = false
            };

            cardBtn.Controls.AddRange(new Control[] { topBar, lblTitle, lblDesc, lblAction });
            return cardBtn;
        }

        private void RefreshStats()
        {
            try
            {
                int empCount = DatabaseHelper.GetEmployeeCount();
                int recCount = DatabaseHelper.GetSalaryRecordCount();

                lblTotalEmployeesCount.Text = empCount.ToString();
                lblTotalRecordsCount.Text = recCount.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load statistics: " + ex.Message, "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ShowAboutDialog()
        {
            string message =
                "=====================================================\n" +
                "  EMPLOYEE SALARY MANAGEMENT & PAYSLIP SYSTEM\n" +
                "=====================================================\n\n" +
                "• Type: College Mini Project\n" +
                "• Technology: C#, .NET Framework, Windows Forms (WinForms)\n" +
                "• Database: SQLite (salary.db) via ADO.NET\n" +
                "• Architecture: 3-Tier Layered Architecture (UI, Logic, Data)\n\n" +
                "Key Features:\n" +
                "  1. Admin Authentication\n" +
                "  2. Employee Master Data Management (CRUD)\n" +
                "  3. Statutory Allowance (HRA, DA) & Deduction (PF, Tax) Calculation\n" +
                "  4. Formatted Professional Salary Slip Preview & Export\n" +
                "  5. Salary Records History & Search Filtering\n\n" +
                "Designed to be simple, clean, and easily explainable in project viva!";

            MessageBox.Show(message, "About - Employee Salary Management System",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            if (clockTimer != null)
            {
                clockTimer.Stop();
            }
        }
    }
}
