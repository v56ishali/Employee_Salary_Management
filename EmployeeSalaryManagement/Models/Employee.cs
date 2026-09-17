using System;

namespace EmployeeSalaryManagement.Models
{
    /// <summary>
    /// Represents an Employee entity with basic personal and job details.
    /// </summary>
    public class Employee
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public decimal BasicSalary { get; set; }

        public Employee()
        {
        }

        public Employee(string employeeId, string name, string department, string designation, decimal basicSalary)
        {
            EmployeeId = employeeId;
            Name = name;
            Department = department;
            Designation = designation;
            BasicSalary = basicSalary;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} ({2})", EmployeeId, Name, Department);
        }
    }
}
