using System;
using System.Text;
using EmployeeSalaryManagement.Models;

namespace EmployeeSalaryManagement.Services
{
    /// <summary>
    /// Service class responsible for all salary calculations and payslip formatting.
    /// Uses standard statutory rules:
    /// - HRA = 20% of Basic Salary
    /// - DA  = 10% of Basic Salary
    /// - PF  = 8% of Basic Salary
    /// - Tax = 5% of Basic Salary
    /// - Gross Salary = Basic + HRA + DA
    /// - Total Deduction = PF + Tax
    /// - Net Salary = Gross Salary - Total Deduction
    /// </summary>
    public static class SalaryCalculator
    {
        // Percentage constants
        public const decimal HraPercentage = 0.20m;
        public const decimal DaPercentage = 0.10m;
        public const decimal PfPercentage = 0.08m;
        public const decimal TaxPercentage = 0.05m;

        /// <summary>
        /// Calculates HRA (House Rent Allowance) = 20% of Basic Salary.
        /// </summary>
        public static decimal CalculateHra(decimal basicSalary)
        {
            return Math.Round(basicSalary * HraPercentage, 2);
        }

        /// <summary>
        /// Calculates DA (Dearness Allowance) = 10% of Basic Salary.
        /// </summary>
        public static decimal CalculateDa(decimal basicSalary)
        {
            return Math.Round(basicSalary * DaPercentage, 2);
        }

        /// <summary>
        /// Calculates Gross Salary = Basic Salary + HRA + DA.
        /// </summary>
        public static decimal CalculateGrossSalary(decimal basicSalary, decimal hra, decimal da)
        {
            return Math.Round(basicSalary + hra + da, 2);
        }

        /// <summary>
        /// Calculates PF (Provident Fund) = 8% of Basic Salary.
        /// </summary>
        public static decimal CalculatePf(decimal basicSalary)
        {
            return Math.Round(basicSalary * PfPercentage, 2);
        }

        /// <summary>
        /// Calculates Tax = 5% of Basic Salary.
        /// </summary>
        public static decimal CalculateTax(decimal basicSalary)
        {
            return Math.Round(basicSalary * TaxPercentage, 2);
        }

        /// <summary>
        /// Calculates Total Deduction = PF + Tax.
        /// </summary>
        public static decimal CalculateTotalDeduction(decimal pf, decimal tax)
        {
            return Math.Round(pf + tax, 2);
        }

        /// <summary>
        /// Calculates Net Salary = Gross Salary - Total Deduction.
        /// </summary>
        public static decimal CalculateNetSalary(decimal grossSalary, decimal totalDeduction)
        {
            return Math.Round(grossSalary - totalDeduction, 2);
        }

        /// <summary>
        /// Performs full salary breakdown calculation for an employee.
        /// </summary>
        public static SalaryRecord ComputeSalaryRecord(Employee employee, DateTime? calculationDate)
        {
            if (employee == null)
                throw new ArgumentNullException("employee");

            decimal basic = employee.BasicSalary;
            decimal hra = CalculateHra(basic);
            decimal da = CalculateDa(basic);
            decimal gross = CalculateGrossSalary(basic, hra, da);

            decimal pf = CalculatePf(basic);
            decimal tax = CalculateTax(basic);
            decimal deduction = CalculateTotalDeduction(pf, tax);
            decimal net = CalculateNetSalary(gross, deduction);

            DateTime date = calculationDate.HasValue ? calculationDate.Value : DateTime.Now;

            return new SalaryRecord
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.Name,
                Department = employee.Department,
                Designation = employee.Designation,
                BasicSalary = basic,
                HRA = hra,
                DA = da,
                GrossSalary = gross,
                PF = pf,
                Tax = tax,
                TotalDeduction = deduction,
                NetSalary = net,
                SalaryDate = date.ToString("yyyy-MM-dd")
            };
        }

        /// <summary>
        /// Formats an amount as Indian Rupee currency string.
        /// </summary>
        public static string FormatCurrency(decimal amount)
        {
            return string.Format("₹{0:N2}", amount);
        }

        /// <summary>
        /// Generates a clean, professional text payslip for preview, printing, or file export.
        /// </summary>
        public static string GeneratePayslipText(SalaryRecord record, string companyName)
        {
            if (record == null)
                return string.Empty;

            if (string.IsNullOrEmpty(companyName))
                companyName = "APEX GLOBAL TECHNOLOGIES PVT. LTD.";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("========================================================================");
            sb.AppendLine(string.Format("                 {0}", companyName.ToUpper()));
            sb.AppendLine("                 EMPLOYEE SALARY SLIP / PAYSLIP");
            sb.AppendLine("========================================================================");
            sb.AppendLine(string.Format(" Pay Period: {0,-20} Generated: {1:dd-MMM-yyyy HH:mm}", record.SalaryDate, DateTime.Now));
            sb.AppendLine("------------------------------------------------------------------------");
            sb.AppendLine(string.Format(" Employee ID   : {0,-20} Department  : {1}", record.EmployeeId, record.Department));
            sb.AppendLine(string.Format(" Employee Name : {0,-20} Designation : {1}", record.EmployeeName, record.Designation));
            sb.AppendLine("========================================================================");
            sb.AppendLine(string.Format(" {0,-32} | {1,-32}", "EARNINGS & ALLOWANCES", "DEDUCTIONS"));
            sb.AppendLine("------------------------------------------------------------------------");
            sb.AppendLine(string.Format(" {0,-20} : {1,10} | {2,-20} : {3,10}",
                "Basic Salary", FormatCurrency(record.BasicSalary),
                "Provident Fund (8%)", FormatCurrency(record.PF)));
            sb.AppendLine(string.Format(" {0,-20} : {1,10} | {2,-20} : {3,10}",
                "HRA (20%)", FormatCurrency(record.HRA),
                "Income Tax (5%)", FormatCurrency(record.Tax)));
            sb.AppendLine(string.Format(" {0,-20} : {1,10} | {2,-20} : {3,10}",
                "DA (10%)", FormatCurrency(record.DA),
                "", ""));
            sb.AppendLine("------------------------------------------------------------------------");
            sb.AppendLine(string.Format(" {0,-20} : {1,10} | {2,-20} : {3,10}",
                "GROSS SALARY", FormatCurrency(record.GrossSalary),
                "TOTAL DEDUCTIONS", FormatCurrency(record.TotalDeduction)));
            sb.AppendLine("========================================================================");
            sb.AppendLine(string.Format("                          NET PAYABLE SALARY : {0}", FormatCurrency(record.NetSalary)));
            sb.AppendLine("========================================================================");
            sb.AppendLine(" Note: This is a computer generated salary slip and does not require");
            sb.AppendLine(" a physical signature.");
            sb.AppendLine("========================================================================");

            return sb.ToString();
        }

        public static string GeneratePayslipText(SalaryRecord record)
        {
            return GeneratePayslipText(record, "APEX GLOBAL TECHNOLOGIES PVT. LTD.");
        }
    }
}
