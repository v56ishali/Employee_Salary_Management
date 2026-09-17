using System;

namespace EmployeeSalaryManagement.Models
{
    /// <summary>
    /// Represents a saved salary calculation record for an employee.
    /// </summary>
    public class SalaryRecord
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HRA { get; set; }
        public decimal DA { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal PF { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal NetSalary { get; set; }
        public string SalaryDate { get; set; }
    }
}
