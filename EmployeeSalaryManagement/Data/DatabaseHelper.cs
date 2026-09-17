using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using EmployeeSalaryManagement.Models;

namespace EmployeeSalaryManagement.Data
{
    /// <summary>
    /// Manages SQLite database connection, table initialization, and CRUD operations
    /// for Employees and SalaryRecords.
    /// </summary>
    public static class DatabaseHelper
    {
        private static readonly string DatabaseFolder;
        private static readonly string DatabaseFilePath;
        private static readonly string ConnectionString;

        static DatabaseHelper()
        {
            // Point to Database folder inside assembly's directory
            string appDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(appDir))
            {
                appDir = AppDomain.CurrentDomain.BaseDirectory;
            }
            DatabaseFolder = Path.Combine(appDir, "Database");
            DatabaseFilePath = Path.Combine(DatabaseFolder, "salary.db");
            ConnectionString = string.Format("Data Source={0};Version=3;", DatabaseFilePath);
        }

        /// <summary>
        /// Gets an open SQLite database connection.
        /// </summary>
        public static SQLiteConnection GetConnection()
        {
            var conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// Automatically creates the SQLite database file and tables if they do not exist.
        /// Also seeds initial sample data if the table is empty.
        /// </summary>
        public static void InitializeDatabase()
        {
            if (!Directory.Exists(DatabaseFolder))
            {
                Directory.CreateDirectory(DatabaseFolder);
            }

            using (var conn = GetConnection())
            {
                // Create Employees table
                string createEmployeesTable = @"
                    CREATE TABLE IF NOT EXISTS Employees (
                        EmployeeId TEXT PRIMARY KEY,
                        Name TEXT NOT NULL,
                        Department TEXT NOT NULL,
                        Designation TEXT NOT NULL,
                        BasicSalary REAL NOT NULL
                    );";

                // Create SalaryRecords table
                string createSalaryRecordsTable = @"
                    CREATE TABLE IF NOT EXISTS SalaryRecords (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        EmployeeId TEXT NOT NULL,
                        BasicSalary REAL NOT NULL,
                        HRA REAL NOT NULL,
                        DA REAL NOT NULL,
                        GrossSalary REAL NOT NULL,
                        PF REAL NOT NULL,
                        Tax REAL NOT NULL,
                        TotalDeduction REAL NOT NULL,
                        NetSalary REAL NOT NULL,
                        SalaryDate TEXT NOT NULL,
                        FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId) ON DELETE CASCADE
                    );";

                using (var cmd = new SQLiteCommand(createEmployeesTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SQLiteCommand(createSalaryRecordsTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Seed sample data if empty
                SeedSampleData(conn);
            }
        }

        private static void SeedSampleData(SQLiteConnection conn)
        {
            string checkCountQuery = "SELECT COUNT(*) FROM Employees;";
            using (var cmd = new SQLiteCommand(checkCountQuery, conn))
            {
                long count = (long)cmd.ExecuteScalar();
                if (count == 0)
                {
                    string seedQuery = @"
                        INSERT INTO Employees (EmployeeId, Name, Department, Designation, BasicSalary) VALUES
                        ('EMP101', 'Rajesh Sharma', 'IT / Software', 'Senior Developer', 45000.00),
                        ('EMP102', 'Priya Verma', 'Human Resources', 'HR Executive', 30000.00),
                        ('EMP103', 'Amitabh Sen', 'Finance & Accounts', 'Accounts Manager', 55000.00),
                        ('EMP104', 'Sneha Patel', 'Marketing', 'Marketing Specialist', 35000.00);

                        INSERT INTO SalaryRecords (EmployeeId, BasicSalary, HRA, DA, GrossSalary, PF, Tax, TotalDeduction, NetSalary, SalaryDate) VALUES
                        ('EMP102', 30000.00, 6000.00, 3000.00, 39000.00, 2400.00, 1500.00, 3900.00, 35100.00, date('now', '-5 days'));
                    ";
                    using (var seedCmd = new SQLiteCommand(seedQuery, conn))
                    {
                        seedCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        #region Employee CRUD Operations

        /// <summary>
        /// Retrieves all employees from the database.
        /// </summary>
        public static List<Employee> GetAllEmployees()
        {
            var list = new List<Employee>();
            using (var conn = GetConnection())
            {
                string query = "SELECT EmployeeId, Name, Department, Designation, BasicSalary FROM Employees ORDER BY EmployeeId ASC;";
                using (var cmd = new SQLiteCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Employee
                        {
                            EmployeeId = reader["EmployeeId"].ToString(),
                            Name = reader["Name"].ToString(),
                            Department = reader["Department"].ToString(),
                            Designation = reader["Designation"].ToString(),
                            BasicSalary = Convert.ToDecimal(reader["BasicSalary"])
                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Retrieves a single employee by Employee ID.
        /// </summary>
        public static Employee GetEmployeeById(string employeeId)
        {
            using (var conn = GetConnection())
            {
                string query = "SELECT EmployeeId, Name, Department, Designation, BasicSalary FROM Employees WHERE EmployeeId = @Id LIMIT 1;";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", employeeId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Employee
                            {
                                EmployeeId = reader["EmployeeId"].ToString(),
                                Name = reader["Name"].ToString(),
                                Department = reader["Department"].ToString(),
                                Designation = reader["Designation"].ToString(),
                                BasicSalary = Convert.ToDecimal(reader["BasicSalary"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Checks whether an Employee ID already exists in the database.
        /// </summary>
        public static bool IsEmployeeIdExists(string employeeId)
        {
            using (var conn = GetConnection())
            {
                string query = "SELECT COUNT(*) FROM Employees WHERE LOWER(EmployeeId) = LOWER(@Id);";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", employeeId.Trim());
                    long count = (long)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Inserts a new employee into the database.
        /// </summary>
        public static bool AddEmployee(Employee emp)
        {
            using (var conn = GetConnection())
            {
                string query = @"
                    INSERT INTO Employees (EmployeeId, Name, Department, Designation, BasicSalary)
                    VALUES (@Id, @Name, @Dept, @Desig, @Basic);";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", emp.EmployeeId.Trim());
                    cmd.Parameters.AddWithValue("@Name", emp.Name.Trim());
                    cmd.Parameters.AddWithValue("@Dept", emp.Department.Trim());
                    cmd.Parameters.AddWithValue("@Desig", emp.Designation.Trim());
                    cmd.Parameters.AddWithValue("@Basic", emp.BasicSalary);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Updates an existing employee's details.
        /// </summary>
        public static bool UpdateEmployee(Employee emp)
        {
            using (var conn = GetConnection())
            {
                string query = @"
                    UPDATE Employees
                    SET Name = @Name, Department = @Dept, Designation = @Desig, BasicSalary = @Basic
                    WHERE EmployeeId = @Id;";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", emp.EmployeeId.Trim());
                    cmd.Parameters.AddWithValue("@Name", emp.Name.Trim());
                    cmd.Parameters.AddWithValue("@Dept", emp.Department.Trim());
                    cmd.Parameters.AddWithValue("@Desig", emp.Designation.Trim());
                    cmd.Parameters.AddWithValue("@Basic", emp.BasicSalary);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Deletes an employee and their salary records from the database.
        /// </summary>
        public static bool DeleteEmployee(string employeeId)
        {
            using (var conn = GetConnection())
            {
                // Delete related salary records first (if foreign keys disabled)
                string delRecords = "DELETE FROM SalaryRecords WHERE EmployeeId = @Id;";
                using (var cmdRecords = new SQLiteCommand(delRecords, conn))
                {
                    cmdRecords.Parameters.AddWithValue("@Id", employeeId);
                    cmdRecords.ExecuteNonQuery();
                }

                string query = "DELETE FROM Employees WHERE EmployeeId = @Id;";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", employeeId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region SalaryRecord CRUD Operations

        /// <summary>
        /// Inserts a newly generated salary record into the database.
        /// </summary>
        public static bool SaveSalaryRecord(SalaryRecord record)
        {
            using (var conn = GetConnection())
            {
                string query = @"
                    INSERT INTO SalaryRecords 
                    (EmployeeId, BasicSalary, HRA, DA, GrossSalary, PF, Tax, TotalDeduction, NetSalary, SalaryDate)
                    VALUES 
                    (@EmpId, @Basic, @HRA, @DA, @Gross, @PF, @Tax, @Deduction, @Net, @Date);";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmpId", record.EmployeeId);
                    cmd.Parameters.AddWithValue("@Basic", record.BasicSalary);
                    cmd.Parameters.AddWithValue("@HRA", record.HRA);
                    cmd.Parameters.AddWithValue("@DA", record.DA);
                    cmd.Parameters.AddWithValue("@Gross", record.GrossSalary);
                    cmd.Parameters.AddWithValue("@PF", record.PF);
                    cmd.Parameters.AddWithValue("@Tax", record.Tax);
                    cmd.Parameters.AddWithValue("@Deduction", record.TotalDeduction);
                    cmd.Parameters.AddWithValue("@Net", record.NetSalary);
                    cmd.Parameters.AddWithValue("@Date", record.SalaryDate);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Retrieves all salary records with joined Employee Name and Department.
        /// </summary>
        public static List<SalaryRecord> GetAllSalaryRecords()
        {
            return QuerySalaryRecords("SELECT s.Id, s.EmployeeId, COALESCE(e.Name, 'Unknown') AS Name, COALESCE(e.Department, 'N/A') AS Department, COALESCE(e.Designation, 'N/A') AS Designation, s.BasicSalary, s.HRA, s.DA, s.GrossSalary, s.PF, s.Tax, s.TotalDeduction, s.NetSalary, s.SalaryDate FROM SalaryRecords s LEFT JOIN Employees e ON s.EmployeeId = e.EmployeeId ORDER BY s.Id DESC;");
        }

        /// <summary>
        /// Searches salary records by Employee ID or Employee Name.
        /// </summary>
        public static List<SalaryRecord> SearchSalaryRecords(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAllSalaryRecords();

            string query = @"
                SELECT s.Id, s.EmployeeId, COALESCE(e.Name, 'Unknown') AS Name, COALESCE(e.Department, 'N/A') AS Department, COALESCE(e.Designation, 'N/A') AS Designation,
                       s.BasicSalary, s.HRA, s.DA, s.GrossSalary, s.PF, s.Tax, s.TotalDeduction, s.NetSalary, s.SalaryDate
                FROM SalaryRecords s
                LEFT JOIN Employees e ON s.EmployeeId = e.EmployeeId
                WHERE LOWER(s.EmployeeId) LIKE @Key OR LOWER(e.Name) LIKE @Key
                ORDER BY s.Id DESC;";

            using (var conn = GetConnection())
            using (var cmd = new SQLiteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Key", "%" + keyword.Trim().ToLower() + "%");
                return ReadSalaryRecords(cmd);
            }
        }

        private static List<SalaryRecord> QuerySalaryRecords(string query)
        {
            using (var conn = GetConnection())
            using (var cmd = new SQLiteCommand(query, conn))
            {
                return ReadSalaryRecords(cmd);
            }
        }

        private static List<SalaryRecord> ReadSalaryRecords(SQLiteCommand cmd)
        {
            var list = new List<SalaryRecord>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new SalaryRecord
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        EmployeeId = reader["EmployeeId"].ToString(),
                        EmployeeName = reader["Name"].ToString(),
                        Department = reader["Department"].ToString(),
                        Designation = reader["Designation"].ToString(),
                        BasicSalary = Convert.ToDecimal(reader["BasicSalary"]),
                        HRA = Convert.ToDecimal(reader["HRA"]),
                        DA = Convert.ToDecimal(reader["DA"]),
                        GrossSalary = Convert.ToDecimal(reader["GrossSalary"]),
                        PF = Convert.ToDecimal(reader["PF"]),
                        Tax = Convert.ToDecimal(reader["Tax"]),
                        TotalDeduction = Convert.ToDecimal(reader["TotalDeduction"]),
                        NetSalary = Convert.ToDecimal(reader["NetSalary"]),
                        SalaryDate = reader["SalaryDate"].ToString()
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// Deletes a specific salary record by ID.
        /// </summary>
        public static bool DeleteSalaryRecord(int recordId)
        {
            using (var conn = GetConnection())
            {
                string query = "DELETE FROM SalaryRecords WHERE Id = @Id;";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", recordId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region Statistics for Dashboard

        public static int GetEmployeeCount()
        {
            using (var conn = GetConnection())
            {
                string query = "SELECT COUNT(*) FROM Employees;";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static int GetSalaryRecordCount()
        {
            using (var conn = GetConnection())
            {
                string query = "SELECT COUNT(*) FROM SalaryRecords;";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        #endregion
    }
}
