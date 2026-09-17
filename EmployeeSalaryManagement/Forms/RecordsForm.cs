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
    /// Displays saved salary calculation records in a DataGridView with search,
    /// delete, refresh, and payslip viewing capabilities.
    /// </summary>
    public class RecordsForm : Form
    {
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnRefresh;
        private Button btnDelete;
        private Button btnViewPayslip;
        private Button btnBack;

        private DataGridView dgvRecords;
        private Label lblTotalRecords;
        private Label lblTotalNetPaid;

        private List<SalaryRecord> currentRecordsList;

        public RecordsForm()
        {
            InitializeComponent();
            LoadRecords();
        }

        private void InitializeComponent()
        {
            this.Text = "Salary Records History - Employee Salary Management";
            this.Size = new Size(1100, 680);
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
                Text = "Salary History & Calculation Records",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                Location = new Point(25, 18),
                AutoSize = true
            };

            Label lblSubHeader = new Label
            {
                Text = "Audit trail of calculated salaries, gross pay, statutory deductions, and net payouts stored in SQLite",
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

            // 2. Search & Controls Bar
            Panel pnlToolBar = new Panel
            {
                Location = new Point(25, 95),
                Size = new Size(1035, 60),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblSearch = new Label
            {
                Text = "Search Record:",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(15, 18),
                AutoSize = true
            };

            txtSearch = new TextBox
            {
                Location = new Point(125, 16),
                Size = new Size(260, 26),
                Font = new Font("Segoe UI", 9.5f)
            };
            txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    SearchRecords();
                }
            };

            btnSearch = new Button
            {
                Text = "Search",
                Location = new Point(395, 14),
                Size = new Size(95, 30),
                BackColor = Color.FromArgb(37, 99, 235), // Royal Blue
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += (s, e) => SearchRecords();

            btnRefresh = new Button
            {
                Text = "🔄 Refresh",
                Location = new Point(500, 14),
                Size = new Size(95, 30),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(71, 85, 105),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnRefresh.Click += (s, e) =>
            {
                txtSearch.Clear();
                LoadRecords();
            };

            btnViewPayslip = new Button
            {
                Text = "📄 View / Print Slip",
                Location = new Point(720, 14),
                Size = new Size(140, 30),
                BackColor = Color.FromArgb(16, 185, 129), // Emerald
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnViewPayslip.FlatAppearance.BorderSize = 0;
            btnViewPayslip.Click += BtnViewPayslip_Click;

            btnDelete = new Button
            {
                Text = "🗑 Delete Record",
                Location = new Point(870, 14),
                Size = new Size(140, 30),
                BackColor = Color.FromArgb(239, 68, 68), // Red
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;

            pnlToolBar.Controls.AddRange(new Control[] {
                lblSearch, txtSearch, btnSearch, btnRefresh, btnViewPayslip, btnDelete
            });
            this.Controls.Add(pnlToolBar);

            // 3. DataGridView Panel
            Panel pnlGridCard = new Panel
            {
                Location = new Point(25, 165),
                Size = new Size(1035, 435),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            dgvRecords = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false
            };

            dgvRecords.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            dgvRecords.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(30, 41, 59);
            dgvRecords.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvRecords.ColumnHeadersHeight = 34;
            dgvRecords.RowTemplate.Height = 28;
            dgvRecords.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgvRecords.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 41, 59);

            // Columns as requested:
            // Employee ID, Employee Name, Department, Basic Salary, Gross Salary, Total Deduction, Net Salary, Date
            dgvRecords.Columns.Add("RecordId", "Record #");
            dgvRecords.Columns["RecordId"].FillWeight = 45;

            dgvRecords.Columns.Add("EmployeeId", "Employee ID");
            dgvRecords.Columns["EmployeeId"].FillWeight = 65;

            dgvRecords.Columns.Add("EmployeeName", "Employee Name");
            dgvRecords.Columns["EmployeeName"].FillWeight = 95;

            dgvRecords.Columns.Add("Department", "Department");
            dgvRecords.Columns["Department"].FillWeight = 85;

            dgvRecords.Columns.Add("BasicSalary", "Basic (₹)");
            dgvRecords.Columns["BasicSalary"].FillWeight = 65;
            dgvRecords.Columns["BasicSalary"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvRecords.Columns.Add("GrossSalary", "Gross (₹)");
            dgvRecords.Columns["GrossSalary"].FillWeight = 65;
            dgvRecords.Columns["GrossSalary"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvRecords.Columns.Add("TotalDeduction", "Deductions (₹)");
            dgvRecords.Columns["TotalDeduction"].FillWeight = 65;
            dgvRecords.Columns["TotalDeduction"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvRecords.Columns.Add("NetSalary", "Net Salary (₹)");
            dgvRecords.Columns["NetSalary"].FillWeight = 70;
            dgvRecords.Columns["NetSalary"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvRecords.Columns["NetSalary"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvRecords.Columns["NetSalary"].DefaultCellStyle.ForeColor = Color.FromArgb(6, 95, 70);

            dgvRecords.Columns.Add("SalaryDate", "Date");
            dgvRecords.Columns["SalaryDate"].FillWeight = 60;
            dgvRecords.Columns["SalaryDate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            pnlGridCard.Controls.Add(dgvRecords);
            this.Controls.Add(pnlGridCard);

            // 4. Bottom Summary Bar
            Panel pnlBottom = new Panel
            {
                Location = new Point(25, 608),
                Size = new Size(1035, 30),
                BackColor = Color.Transparent
            };

            lblTotalRecords = new Label
            {
                Text = "Total Records: 0",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(5, 5),
                AutoSize = true
            };

            lblTotalNetPaid = new Label
            {
                Text = "Total Net Payout: ₹0.00",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(16, 185, 129),
                Location = new Point(780, 5),
                Size = new Size(245, 20),
                TextAlign = ContentAlignment.MiddleRight
            };

            pnlBottom.Controls.AddRange(new Control[] { lblTotalRecords, lblTotalNetPaid });
            this.Controls.Add(pnlBottom);
        }

        private void LoadRecords()
        {
            try
            {
                currentRecordsList = DatabaseHelper.GetAllSalaryRecords();
                PopulateGrid(currentRecordsList);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading salary records: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchRecords()
        {
            string keyword = txtSearch.Text.Trim();
            try
            {
                currentRecordsList = DatabaseHelper.SearchSalaryRecords(keyword);
                PopulateGrid(currentRecordsList);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching records: " + ex.Message, "Search Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateGrid(List<SalaryRecord> list)
        {
            dgvRecords.Rows.Clear();
            decimal totalNet = 0m;

            foreach (var r in list)
            {
                dgvRecords.Rows.Add(
                    r.Id,
                    r.EmployeeId,
                    r.EmployeeName,
                    r.Department,
                    r.BasicSalary.ToString("N2"),
                    r.GrossSalary.ToString("N2"),
                    r.TotalDeduction.ToString("N2"),
                    r.NetSalary.ToString("N2"),
                    r.SalaryDate
                );
                totalNet += r.NetSalary;
            }

            lblTotalRecords.Text = string.Format("Total Salary Records: {0}", list.Count);
            lblTotalNetPaid.Text = string.Format("Total Net Payout: ₹{0:N2}", totalNet);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvRecords.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a salary record row to delete.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = dgvRecords.SelectedRows[0];
            int recordId = Convert.ToInt32(row.Cells["RecordId"].Value);
            string empId = row.Cells["EmployeeId"].Value != null ? row.Cells["EmployeeId"].Value.ToString() : string.Empty;
            string date = row.Cells["SalaryDate"].Value != null ? row.Cells["SalaryDate"].Value.ToString() : string.Empty;

            DialogResult confirm = MessageBox.Show(string.Format("Are you sure you want to delete Salary Record #{0} for Employee '{1}' dated {2}?", recordId, empId, date),
                "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    if (DatabaseHelper.DeleteSalaryRecord(recordId))
                    {
                        MessageBox.Show(string.Format("Salary Record #{0} deleted successfully.", recordId), "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadRecords();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete record.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database error: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnViewPayslip_Click(object sender, EventArgs e)
        {
            if (dgvRecords.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a salary record from the table to view its payslip.",
                    "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int index = dgvRecords.SelectedRows[0].Index;
            if (currentRecordsList != null && index >= 0 && index < currentRecordsList.Count)
            {
                var record = currentRecordsList[index];
                string slipText = SalaryCalculator.GeneratePayslipText(record);

                using (Form slipForm = new Form())
                {
                    slipForm.Text = string.Format("Payslip - {0} ({1})", record.EmployeeName, record.EmployeeId);
                    slipForm.Size = new Size(580, 520);
                    slipForm.StartPosition = FormStartPosition.CenterParent;
                    slipForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                    slipForm.MaximizeBox = false;

                    RichTextBox rtb = new RichTextBox
                    {
                        Dock = DockStyle.Fill,
                        Font = new Font("Consolas", 9.5f),
                        ReadOnly = true,
                        Text = slipText,
                        BackColor = Color.FromArgb(248, 250, 252)
                    };

                    Panel pnlActions = new Panel
                    {
                        Dock = DockStyle.Bottom,
                        Height = 50,
                        BackColor = Color.White
                    };

                    Button btnPrint = new Button
                    {
                        Text = "🖨 Print",
                        Location = new Point(20, 10),
                        Size = new Size(110, 30),
                        BackColor = Color.FromArgb(37, 99, 235),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat
                    };
                    btnPrint.Click += (s, ev) =>
                    {
                        PrintDocument doc = new PrintDocument();
                        doc.DocumentName = string.Format("Payslip_{0}", record.EmployeeId);
                        doc.PrintPage += (pSender, pEvent) =>
                        {
                            using (Font pf = new Font("Consolas", 10))
                            {
                                pEvent.Graphics.DrawString(slipText, pf, Brushes.Black, 50, 50);
                            }
                        };
                        using (PrintPreviewDialog ppd = new PrintPreviewDialog())
                        {
                            ppd.Document = doc;
                            ppd.ShowDialog();
                        }
                    };

                    Button btnSave = new Button
                    {
                        Text = "💾 Save to File",
                        Location = new Point(140, 10),
                        Size = new Size(130, 30),
                        BackColor = Color.FromArgb(16, 185, 129),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat
                    };
                    btnSave.Click += (s, ev) =>
                    {
                        using (SaveFileDialog sfd = new SaveFileDialog())
                        {
                            sfd.Filter = "Text Files (*.txt)|*.txt";
                            sfd.FileName = string.Format("Payslip_{0}_{1}.txt", record.EmployeeId, record.SalaryDate);
                            if (sfd.ShowDialog() == DialogResult.OK)
                            {
                                File.WriteAllText(sfd.FileName, slipText);
                                MessageBox.Show("Saved to " + sfd.FileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    };

                    Button btnClose = new Button
                    {
                        Text = "Close",
                        Location = new Point(440, 10),
                        Size = new Size(95, 30),
                        BackColor = Color.FromArgb(241, 245, 249),
                        FlatStyle = FlatStyle.Flat
                    };
                    btnClose.Click += (s, ev) => slipForm.Close();

                    pnlActions.Controls.AddRange(new Control[] { btnPrint, btnSave, btnClose });
                    slipForm.Controls.Add(rtb);
                    slipForm.Controls.Add(pnlActions);

                    slipForm.ShowDialog();
                }
            }
        }
    }
}
