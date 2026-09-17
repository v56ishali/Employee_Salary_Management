using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using EmployeeSalaryManagement.Data;
using EmployeeSalaryManagement.Models;

namespace EmployeeSalaryManagement.Forms
{
    /// <summary>
    /// Form for managing employee details (CRUD: Create, Read, Update, Delete).
    /// </summary>
    public class EmployeeForm : Form
    {
        private TextBox txtEmployeeId;
        private TextBox txtName;
        private ComboBox cmbDepartment;
        private TextBox txtDesignation;
        private TextBox txtBasicSalary;
        private TextBox txtSearch;

        private Button btnSave;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnClear;
        private Button btnBack;

        private DataGridView dgvEmployees;
        private Label lblTotalBadge;

        public EmployeeForm()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void InitializeComponent()
        {
            this.Text = "Manage Employees - Employee Salary Management";
            this.Size = new Size(1000, 640);
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
                Text = "Employee Information Management",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                Location = new Point(25, 18),
                AutoSize = true
            };

            Label lblSubHeader = new Label
            {
                Text = "Register, update, view, and maintain employee records in the SQLite database",
                ForeColor = Color.FromArgb(191, 219, 254),
                Font = new Font("Segoe UI", 9f),
                Location = new Point(27, 46),
                AutoSize = true
            };

            btnBack = new Button
            {
                Text = "← Back to Dashboard",
                Location = new Point(810, 22),
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

            // 2. Left Panel: Input Form Card
            Panel pnlForm = new Panel
            {
                Location = new Point(25, 95),
                Size = new Size(360, 490),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblFormTitle = new Label
            {
                Text = "Employee Details",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 15),
                AutoSize = true
            };

            // Employee ID
            Label lblId = new Label { Text = "Employee ID *", Location = new Point(20, 50), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            txtEmployeeId = new TextBox { Location = new Point(20, 72), Size = new Size(318, 26), Font = new Font("Segoe UI", 9.5f) };

            // Employee Name
            Label lblName = new Label { Text = "Employee Name *", Location = new Point(20, 108), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            txtName = new TextBox { Location = new Point(20, 130), Size = new Size(318, 26), Font = new Font("Segoe UI", 9.5f) };

            // Department
            Label lblDept = new Label { Text = "Department *", Location = new Point(20, 166), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            cmbDepartment = new ComboBox
            {
                Location = new Point(20, 188),
                Size = new Size(318, 26),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f)
            };
            cmbDepartment.Items.AddRange(new object[] {
                "IT / Software",
                "Human Resources",
                "Finance & Accounts",
                "Marketing & Sales",
                "Operations",
                "Administration"
            });
            cmbDepartment.SelectedIndex = 0;

            // Designation
            Label lblDesig = new Label { Text = "Designation *", Location = new Point(20, 224), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            txtDesignation = new TextBox { Location = new Point(20, 246), Size = new Size(318, 26), Font = new Font("Segoe UI", 9.5f) };

            // Basic Salary
            Label lblSalary = new Label { Text = "Basic Salary (₹) *", Location = new Point(20, 282), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            txtBasicSalary = new TextBox { Location = new Point(20, 304), Size = new Size(318, 26), Font = new Font("Segoe UI", 9.5f) };

            // Buttons: Row 1
            btnSave = new Button
            {
                Text = "Save",
                Location = new Point(20, 350),
                Size = new Size(150, 36),
                BackColor = Color.FromArgb(16, 185, 129), // Emerald
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnUpdate = new Button
            {
                Text = "Update",
                Location = new Point(188, 350),
                Size = new Size(150, 36),
                BackColor = Color.FromArgb(37, 99, 235), // Royal Blue
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnUpdate.FlatAppearance.BorderSize = 0;
            btnUpdate.Click += BtnUpdate_Click;

            // Buttons: Row 2
            btnDelete = new Button
            {
                Text = "Delete",
                Location = new Point(20, 396),
                Size = new Size(150, 36),
                BackColor = Color.FromArgb(239, 68, 68), // Red
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;

            btnClear = new Button
            {
                Text = "Clear Fields",
                Location = new Point(188, 396),
                Size = new Size(150, 36),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(71, 85, 105),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnClear.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnClear.Click += (s, e) => ClearInputs();

            Label lblHint = new Label
            {
                Text = "Tip: Click any employee in table to edit or delete.",
                ForeColor = Color.FromArgb(100, 116, 139),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                Location = new Point(20, 448),
                Size = new Size(318, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnlForm.Controls.AddRange(new Control[] {
                lblFormTitle, lblId, txtEmployeeId, lblName, txtName,
                lblDept, cmbDepartment, lblDesig, txtDesignation,
                lblSalary, txtBasicSalary, btnSave, btnUpdate,
                btnDelete, btnClear, lblHint
            });
            this.Controls.Add(pnlForm);

            // 3. Right Panel: DataGridView Card
            Panel pnlTableCard = new Panel
            {
                Location = new Point(400, 95),
                Size = new Size(560, 490),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblTableTitle = new Label
            {
                Text = "Registered Employees List",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(15, 15),
                AutoSize = true
            };

            lblTotalBadge = new Label
            {
                Text = "Total: 0",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235),
                Location = new Point(230, 17),
                AutoSize = true
            };

            // Search Bar
            Label lblSearch = new Label { Text = "Search:", Location = new Point(310, 17), AutoSize = true, Font = new Font("Segoe UI", 9f) };
            txtSearch = new TextBox { Location = new Point(365, 14), Size = new Size(180, 24), Font = new Font("Segoe UI", 9f) };
            txtSearch.TextChanged += (s, e) => FilterEmployees();

            // DataGridView setup
            dgvEmployees = new DataGridView
            {
                Location = new Point(15, 50),
                Size = new Size(530, 422),
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

            dgvEmployees.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            dgvEmployees.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(30, 41, 59);
            dgvEmployees.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvEmployees.ColumnHeadersHeight = 32;
            dgvEmployees.RowTemplate.Height = 28;
            dgvEmployees.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgvEmployees.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 41, 59);

            // Columns
            dgvEmployees.Columns.Add("EmployeeId", "ID");
            dgvEmployees.Columns["EmployeeId"].FillWeight = 55;

            dgvEmployees.Columns.Add("Name", "Name");
            dgvEmployees.Columns["Name"].FillWeight = 95;

            dgvEmployees.Columns.Add("Department", "Department");
            dgvEmployees.Columns["Department"].FillWeight = 85;

            dgvEmployees.Columns.Add("Designation", "Designation");
            dgvEmployees.Columns["Designation"].FillWeight = 85;

            dgvEmployees.Columns.Add("BasicSalary", "Basic (₹)");
            dgvEmployees.Columns["BasicSalary"].FillWeight = 60;
            dgvEmployees.Columns["BasicSalary"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvEmployees.CellClick += DgvEmployees_CellClick;

            pnlTableCard.Controls.AddRange(new Control[] {
                lblTableTitle, lblTotalBadge, lblSearch, txtSearch, dgvEmployees
            });
            this.Controls.Add(pnlTableCard);
        }

        private void LoadEmployees()
        {
            try
            {
                List<Employee> list = DatabaseHelper.GetAllEmployees();
                PopulateGrid(list);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading employee records: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateGrid(List<Employee> list)
        {
            dgvEmployees.Rows.Clear();
            foreach (var emp in list)
            {
                dgvEmployees.Rows.Add(
                    emp.EmployeeId,
                    emp.Name,
                    emp.Department,
                    emp.Designation,
                    emp.BasicSalary.ToString("N2")
                );
            }
            lblTotalBadge.Text = string.Format("Total: {0}", list.Count);
        }

        private void FilterEmployees()
        {
            string query = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(query))
            {
                LoadEmployees();
                return;
            }

            var all = DatabaseHelper.GetAllEmployees();
            var filtered = all.FindAll(e =>
                e.EmployeeId.ToLower().Contains(query) ||
                e.Name.ToLower().Contains(query) ||
                e.Department.ToLower().Contains(query) ||
                e.Designation.ToLower().Contains(query));

            PopulateGrid(filtered);
        }

        private void DgvEmployees_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvEmployees.Rows.Count)
            {
                var row = dgvEmployees.Rows[e.RowIndex];
                txtEmployeeId.Text = row.Cells["EmployeeId"].Value != null ? row.Cells["EmployeeId"].Value.ToString() : string.Empty;
                txtName.Text = row.Cells["Name"].Value != null ? row.Cells["Name"].Value.ToString() : string.Empty;
                string dept = row.Cells["Department"].Value != null ? row.Cells["Department"].Value.ToString() : string.Empty;
                if (cmbDepartment.Items.Contains(dept))
                    cmbDepartment.SelectedItem = dept;
                else
                    cmbDepartment.SelectedIndex = 0;

                txtDesignation.Text = row.Cells["Designation"].Value != null ? row.Cells["Designation"].Value.ToString() : string.Empty;
                txtBasicSalary.Text = row.Cells["BasicSalary"].Value != null ? row.Cells["BasicSalary"].Value.ToString().Replace(",", "") : string.Empty;

                txtEmployeeId.ReadOnly = true; // Protect ID from modification when updating
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string empId;
            string name;
            string dept;
            string desig;
            decimal basicSalary;

            if (!ValidateInputs(out empId, out name, out dept, out desig, out basicSalary))
                return;

            if (DatabaseHelper.IsEmployeeIdExists(empId))
            {
                MessageBox.Show(string.Format("Employee ID '{0}' already exists in the system!\nPlease use a unique Employee ID.", empId),
                    "Duplicate Employee ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmployeeId.Focus();
                return;
            }

            Employee emp = new Employee(empId, name, dept, desig, basicSalary);
            try
            {
                if (DatabaseHelper.AddEmployee(emp))
                {
                    MessageBox.Show(string.Format("Employee '{0}' (ID: {1}) saved successfully!", name, empId),
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    LoadEmployees();
                }
                else
                {
                    MessageBox.Show("Failed to save employee. Please try again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            string empId;
            string name;
            string dept;
            string desig;
            decimal basicSalary;

            if (!ValidateInputs(out empId, out name, out dept, out desig, out basicSalary))
                return;

            if (!DatabaseHelper.IsEmployeeIdExists(empId))
            {
                MessageBox.Show(string.Format("Employee ID '{0}' does not exist! Save as new employee instead.", empId),
                    "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Employee emp = new Employee(empId, name, dept, desig, basicSalary);
            try
            {
                if (DatabaseHelper.UpdateEmployee(emp))
                {
                    MessageBox.Show(string.Format("Employee '{0}' details updated successfully!", name),
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    LoadEmployees();
                }
                else
                {
                    MessageBox.Show("Failed to update employee details.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            string empId = txtEmployeeId.Text.Trim();
            if (string.IsNullOrEmpty(empId))
            {
                MessageBox.Show("Please select or enter an Employee ID to delete.",
                    "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!DatabaseHelper.IsEmployeeIdExists(empId))
            {
                MessageBox.Show(string.Format("Employee ID '{0}' not found.", empId),
                    "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(string.Format("Are you sure you want to delete Employee '{0}'?\nAll associated salary records will also be removed.", empId),
                "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    if (DatabaseHelper.DeleteEmployee(empId))
                    {
                        MessageBox.Show(string.Format("Employee '{0}' deleted successfully.", empId),
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearInputs();
                        LoadEmployees();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete employee.", "Error",
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

        private bool ValidateInputs(out string empId, out string name, out string dept, out string desig, out decimal basicSalary)
        {
            empId = txtEmployeeId.Text.Trim();
            name = txtName.Text.Trim();
            dept = cmbDepartment.SelectedItem != null ? cmbDepartment.SelectedItem.ToString() : "General";
            desig = txtDesignation.Text.Trim();
            basicSalary = 0m;

            if (string.IsNullOrEmpty(empId))
            {
                MessageBox.Show("Employee ID cannot be empty.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmployeeId.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Employee Name cannot be empty.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(desig))
            {
                MessageBox.Show("Designation cannot be empty.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDesignation.Focus();
                return false;
            }

            if (!decimal.TryParse(txtBasicSalary.Text.Trim(), out basicSalary) || basicSalary <= 0)
            {
                MessageBox.Show("Basic Salary must be a valid positive number greater than 0.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBasicSalary.SelectAll();
                txtBasicSalary.Focus();
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            txtEmployeeId.Clear();
            txtEmployeeId.ReadOnly = false;
            txtName.Clear();
            if (cmbDepartment.Items.Count > 0)
                cmbDepartment.SelectedIndex = 0;
            txtDesignation.Clear();
            txtBasicSalary.Clear();
            txtEmployeeId.Focus();
        }
    }
}
