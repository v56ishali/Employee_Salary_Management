using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using EmployeeSalaryManagement.Data;
using EmployeeSalaryManagement.Models;
using EmployeeSalaryManagement.Services;

namespace EmployeeSalaryManagement.Forms
{
    /// <summary>
    /// Form for selecting an employee, computing statutory salary components,
    /// generating salary slips, saving records to SQLite, and printing/exporting payslips.
    /// </summary>
    public class SalaryForm : Form
    {
        private ComboBox cmbEmployees;
        private TextBox txtEmpId;
        private TextBox txtName;
        private TextBox txtDept;
        private TextBox txtDesignation;
        private TextBox txtBasicSalary;
        private DateTimePicker dtpSalaryDate;

        // Breakdown display labels
        private Label lblHRA;
        private Label lblDA;
        private Label lblGross;
        private Label lblPF;
        private Label lblTax;
        private Label lblTotalDeductions;
        private Label lblNetSalary;

        // Payslip preview
        private RichTextBox rtbPayslip;

        // Action buttons
        private Button btnCalculate;
        private Button btnSaveRecord;
        private Button btnPrintPayslip;
        private Button btnExportPayslip;
        private Button btnClear;
        private Button btnBack;

        private List<Employee> employeeList;
        private SalaryRecord currentRecord;

        public SalaryForm()
        {
            InitializeComponent();
            LoadEmployeeDropdown();
        }

        private void InitializeComponent()
        {
            this.Text = "Salary Calculation & Payslip Generator";
            this.Size = new Size(1100, 720);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(243, 246, 250);
            this.Font = new Font("Segoe UI", 9.5f);

            // 1. Top Header Banner
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(26, 54, 93) // Navy
            };

            Label lblHeader = new Label
            {
                Text = "Salary Calculation & Payslip Generator",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                Location = new Point(25, 16),
                AutoSize = true
            };

            Label lblSubHeader = new Label
            {
                Text = "Calculate HRA (20%), DA (10%), PF (8%), Tax (5%), generate professional payslips & store records",
                ForeColor = Color.FromArgb(191, 219, 254),
                Font = new Font("Segoe UI", 9f),
                Location = new Point(27, 46),
                AutoSize = true
            };

            btnBack = new Button
            {
                Text = "← Back to Dashboard",
                Location = new Point(910, 22),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderColor = Color.FromArgb(100, 116, 139);
            btnBack.Click += (s, e) => this.Close();

            pnlHeader.Controls.AddRange(new Control[] { lblHeader, lblSubHeader, btnBack });
            this.Controls.Add(pnlHeader);

            // 2. Top Bar: Select Employee Card
            Panel pnlSelectCard = new Panel
            {
                Location = new Point(25, 95),
                Size = new Size(1035, 65),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblSelect = new Label
            {
                Text = "Select Employee:",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(15, 20),
                AutoSize = true
            };

            cmbEmployees = new ComboBox
            {
                Location = new Point(135, 17),
                Size = new Size(350, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f)
            };
            cmbEmployees.SelectedIndexChanged += CmbEmployees_SelectedIndexChanged;

            Label lblDate = new Label
            {
                Text = "Salary Date:",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(530, 20),
                AutoSize = true
            };

            dtpSalaryDate = new DateTimePicker
            {
                Location = new Point(620, 17),
                Size = new Size(180, 28),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 9.5f)
            };

            btnClear = new Button
            {
                Text = "Reset Form",
                Location = new Point(880, 15),
                Size = new Size(130, 32),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(71, 85, 105),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f),
                Cursor = Cursors.Hand
            };
            btnClear.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnClear.Click += (s, e) => ResetForm();

            pnlSelectCard.Controls.AddRange(new Control[] { lblSelect, cmbEmployees, lblDate, dtpSalaryDate, btnClear });
            this.Controls.Add(pnlSelectCard);

            // 3. Middle Left: Employee Details & Salary Breakdown Card
            Panel pnlCalcCard = new Panel
            {
                Location = new Point(25, 170),
                Size = new Size(495, 495),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblCalcTitle = new Label
            {
                Text = "Employee & Salary Breakdown",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(15, 12),
                AutoSize = true
            };

            // Readonly fields
            AddFormRow(pnlCalcCard, "Employee ID:", out txtEmpId, 42, 140, true);
            AddFormRow(pnlCalcCard, "Name:", out txtName, 74, 140, true);
            AddFormRow(pnlCalcCard, "Department:", out txtDept, 106, 140, true);
            AddFormRow(pnlCalcCard, "Designation:", out txtDesignation, 138, 140, true);
            AddFormRow(pnlCalcCard, "Basic Salary (₹):", out txtBasicSalary, 170, 140, false);

            // Divider
            Label lblDiv1 = new Label
            {
                Location = new Point(15, 205),
                Size = new Size(465, 1),
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlCalcCard.Controls.Add(lblDiv1);

            // Earnings Group
            Label lblEarnHeader = new Label
            {
                Text = "Earnings (Allowances)",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235),
                Location = new Point(15, 215),
                AutoSize = true
            };
            pnlCalcCard.Controls.Add(lblEarnHeader);

            AddValueRow(pnlCalcCard, "• HRA (20%):", out lblHRA, 235, 160);
            AddValueRow(pnlCalcCard, "• DA (10%):", out lblDA, 258, 160);
            AddValueRow(pnlCalcCard, "GROSS SALARY:", out lblGross, 282, 160, true, Color.FromArgb(30, 41, 59));

            // Deductions Group
            Label lblDedHeader = new Label
            {
                Text = "Deductions (Statutory)",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(239, 68, 68),
                Location = new Point(15, 310),
                AutoSize = true
            };
            pnlCalcCard.Controls.Add(lblDedHeader);

            AddValueRow(pnlCalcCard, "• PF (8%):", out lblPF, 330, 160);
            AddValueRow(pnlCalcCard, "• Income Tax (5%):", out lblTax, 353, 160);
            AddValueRow(pnlCalcCard, "TOTAL DEDUCTION:", out lblTotalDeductions, 377, 160, true, Color.FromArgb(185, 28, 28));

            // Net Salary Highlight Box
            Panel pnlNetBox = new Panel
            {
                Location = new Point(15, 405),
                Size = new Size(465, 42),
                BackColor = Color.FromArgb(236, 253, 245), // Emerald light
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblNetTitle = new Label
            {
                Text = "NET SALARY :",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(6, 95, 70),
                Location = new Point(12, 10),
                AutoSize = true
            };

            lblNetSalary = new Label
            {
                Text = "₹0.00",
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.FromArgb(6, 95, 70),
                Location = new Point(260, 9),
                Size = new Size(190, 24),
                TextAlign = ContentAlignment.MiddleRight
            };
            pnlNetBox.Controls.AddRange(new Control[] { lblNetTitle, lblNetSalary });
            pnlCalcCard.Controls.Add(pnlNetBox);

            // Calculation and Save Buttons
            btnCalculate = new Button
            {
                Text = "1. Calculate Salary",
                Location = new Point(15, 452),
                Size = new Size(225, 34),
                BackColor = Color.FromArgb(37, 99, 235), // Blue
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCalculate.FlatAppearance.BorderSize = 0;
            btnCalculate.Click += BtnCalculate_Click;

            btnSaveRecord = new Button
            {
                Text = "2. Save Record to Database",
                Location = new Point(255, 452),
                Size = new Size(225, 34),
                BackColor = Color.FromArgb(16, 185, 129), // Emerald
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSaveRecord.FlatAppearance.BorderSize = 0;
            btnSaveRecord.Click += BtnSaveRecord_Click;

            pnlCalcCard.Controls.AddRange(new Control[] { lblCalcTitle, btnCalculate, btnSaveRecord });
            this.Controls.Add(pnlCalcCard);

            // 4. Middle Right: Payslip Preview Card
            Panel pnlSlipCard = new Panel
            {
                Location = new Point(535, 170),
                Size = new Size(525, 495),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblSlipTitle = new Label
            {
                Text = "Salary Slip (Payslip) Preview",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(15, 12),
                AutoSize = true
            };

            rtbPayslip = new RichTextBox
            {
                Location = new Point(15, 42),
                Size = new Size(495, 400),
                Font = new Font("Consolas", 9f),
                BackColor = Color.FromArgb(248, 250, 252),
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true
            };

            btnPrintPayslip = new Button
            {
                Text = "🖨 Print / Preview Payslip",
                Location = new Point(15, 452),
                Size = new Size(240, 34),
                BackColor = Color.FromArgb(14, 165, 233), // Sky blue
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPrintPayslip.FlatAppearance.BorderSize = 0;
            btnPrintPayslip.Click += BtnPrintPayslip_Click;

            btnExportPayslip = new Button
            {
                Text = "💾 Save as Text File (.txt)",
                Location = new Point(270, 452),
                Size = new Size(240, 34),
                BackColor = Color.FromArgb(100, 116, 139), // Slate
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExportPayslip.FlatAppearance.BorderSize = 0;
            btnExportPayslip.Click += BtnExportPayslip_Click;

            pnlSlipCard.Controls.AddRange(new Control[] {
                lblSlipTitle, rtbPayslip, btnPrintPayslip, btnExportPayslip
            });
            this.Controls.Add(pnlSlipCard);
        }

        private void AddFormRow(Panel parent, string labelText, out TextBox txt, int top, int labelWidth, bool readOnly)
        {
            Label lbl = new Label
            {
                Text = labelText,
                Location = new Point(15, top + 2),
                Size = new Size(labelWidth, 22),
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                ForeColor = Color.FromArgb(71, 85, 105)
            };

            txt = new TextBox
            {
                Location = new Point(15 + labelWidth, top),
                Size = new Size(310, 24),
                Font = new Font("Segoe UI", 9.5f),
                ReadOnly = readOnly,
                BackColor = readOnly ? Color.FromArgb(248, 250, 252) : Color.White
            };

            parent.Controls.AddRange(new Control[] { lbl, txt });
        }

        private void AddValueRow(Panel parent, string labelText, out Label valLabel, int top, int labelWidth, bool isBold, Color? color)
        {
            Label lbl = new Label
            {
                Text = labelText,
                Location = new Point(15, top),
                Size = new Size(labelWidth, 20),
                Font = new Font("Segoe UI", 9f, isBold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = color.HasValue ? color.Value : Color.FromArgb(71, 85, 105)
            };

            valLabel = new Label
            {
                Text = "₹0.00",
                Location = new Point(290, top),
                Size = new Size(185, 20),
                Font = new Font("Segoe UI", 9.5f, isBold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = color.HasValue ? color.Value : Color.FromArgb(30, 41, 59),
                TextAlign = ContentAlignment.MiddleRight
            };

            parent.Controls.AddRange(new Control[] { lbl, valLabel });
        }

        private void AddValueRow(Panel parent, string labelText, out Label valLabel, int top, int labelWidth)
        {
            AddValueRow(parent, labelText, out valLabel, top, labelWidth, false, null);
        }

        private void LoadEmployeeDropdown()
        {
            try
            {
                employeeList = DatabaseHelper.GetAllEmployees();
                cmbEmployees.Items.Clear();

                if (employeeList.Count == 0)
                {
                    cmbEmployees.Items.Add("-- No Employees Found. Please add an employee first. --");
                    cmbEmployees.SelectedIndex = 0;
                    return;
                }

                foreach (var emp in employeeList)
                {
                    cmbEmployees.Items.Add(string.Format("{0} - {1} ({2})", emp.EmployeeId, emp.Name, emp.Department));
                }
                cmbEmployees.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching employees: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbEmployees_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = cmbEmployees.SelectedIndex;
            if (employeeList != null && index >= 0 && index < employeeList.Count)
            {
                var emp = employeeList[index];
                txtEmpId.Text = emp.EmployeeId;
                txtName.Text = emp.Name;
                txtDept.Text = emp.Department;
                txtDesignation.Text = emp.Designation;
                txtBasicSalary.Text = emp.BasicSalary.ToString("0.00");

                // Auto-calculate on employee select
                CalculateSalary(false);
            }
        }

        private bool CalculateSalary(bool showAlertOnSuccess)
        {
            if (string.IsNullOrWhiteSpace(txtEmpId.Text))
            {
                MessageBox.Show("Please select an employee first.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            decimal basicSalary;
            if (!decimal.TryParse(txtBasicSalary.Text.Trim(), out basicSalary) || basicSalary <= 0)
            {
                MessageBox.Show("Please enter a valid positive Basic Salary amount.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBasicSalary.SelectAll();
                txtBasicSalary.Focus();
                return false;
            }

            var emp = new Employee(
                txtEmpId.Text.Trim(),
                txtName.Text.Trim(),
                txtDept.Text.Trim(),
                txtDesignation.Text.Trim(),
                basicSalary
            );

            currentRecord = SalaryCalculator.ComputeSalaryRecord(emp, dtpSalaryDate.Value);

            // Display in breakdown
            lblHRA.Text = SalaryCalculator.FormatCurrency(currentRecord.HRA);
            lblDA.Text = SalaryCalculator.FormatCurrency(currentRecord.DA);
            lblGross.Text = SalaryCalculator.FormatCurrency(currentRecord.GrossSalary);

            lblPF.Text = SalaryCalculator.FormatCurrency(currentRecord.PF);
            lblTax.Text = SalaryCalculator.FormatCurrency(currentRecord.Tax);
            lblTotalDeductions.Text = SalaryCalculator.FormatCurrency(currentRecord.TotalDeduction);

            lblNetSalary.Text = SalaryCalculator.FormatCurrency(currentRecord.NetSalary);

            // Generate Payslip preview
            rtbPayslip.Text = SalaryCalculator.GeneratePayslipText(currentRecord);

            if (showAlertOnSuccess)
            {
                string msg = string.Format("Salary calculated successfully for {0}!\n\nGross Salary: {1}\nTotal Deductions: {2}\nNet Salary: {3}",
                    currentRecord.EmployeeName,
                    SalaryCalculator.FormatCurrency(currentRecord.GrossSalary),
                    SalaryCalculator.FormatCurrency(currentRecord.TotalDeduction),
                    SalaryCalculator.FormatCurrency(currentRecord.NetSalary));

                MessageBox.Show(msg, "Calculation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return true;
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            CalculateSalary(true);
        }

        private void BtnSaveRecord_Click(object sender, EventArgs e)
        {
            if (currentRecord == null)
            {
                if (!CalculateSalary(false))
                    return;
            }

            try
            {
                currentRecord.SalaryDate = dtpSalaryDate.Value.ToString("yyyy-MM-dd");
                bool saved = DatabaseHelper.SaveSalaryRecord(currentRecord);
                if (saved)
                {
                    string msg = string.Format("Salary record for '{0}' (ID: {1}) for date {2} has been saved into SQLite database successfully!",
                        currentRecord.EmployeeName, currentRecord.EmployeeId, currentRecord.SalaryDate);

                    MessageBox.Show(msg, "Record Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to save salary record to the database.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPrintPayslip_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rtbPayslip.Text))
            {
                MessageBox.Show("Please calculate salary to generate a payslip before printing.",
                    "Payslip Empty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PrintDocument doc = new PrintDocument();
            doc.DocumentName = string.Format("Payslip_{0}", currentRecord != null ? currentRecord.EmployeeId : "Employee");
            doc.PrintPage += (s, ev) =>
            {
                using (Font printFont = new Font("Consolas", 10))
                {
                    ev.Graphics.DrawString(rtbPayslip.Text, printFont, Brushes.Black, 50, 50);
                }
            };

            using (PrintPreviewDialog preview = new PrintPreviewDialog())
            {
                preview.Document = doc;
                preview.Width = 800;
                preview.Height = 600;
                preview.StartPosition = FormStartPosition.CenterParent;
                preview.ShowDialog();
            }
        }

        private void BtnExportPayslip_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rtbPayslip.Text))
            {
                MessageBox.Show("Please calculate salary to generate a payslip before exporting.",
                    "Payslip Empty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                string empTag = currentRecord != null ? currentRecord.EmployeeId : "EMP";
                sfd.FileName = string.Format("Payslip_{0}_{1:yyyyMMdd}.txt", empTag, DateTime.Now);
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(sfd.FileName, rtbPayslip.Text);
                        MessageBox.Show("Payslip exported successfully!\nLocation: " + sfd.FileName,
                            "Export Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error exporting payslip: " + ex.Message, "Export Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ResetForm()
        {
            if (cmbEmployees.Items.Count > 0)
                cmbEmployees.SelectedIndex = 0;
            currentRecord = null;
            lblHRA.Text = "₹0.00";
            lblDA.Text = "₹0.00";
            lblGross.Text = "₹0.00";
            lblPF.Text = "₹0.00";
            lblTax.Text = "₹0.00";
            lblTotalDeductions.Text = "₹0.00";
            lblNetSalary.Text = "₹0.00";
            rtbPayslip.Clear();
        }
    }
}
