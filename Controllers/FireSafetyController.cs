using PayrollToyHRD.Models;
using PayrollToyHRD.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using System.Linq;
using System.IO;

namespace PayrollToyHRD.Controllers
{
    public class FireSafetyController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FireSafetyServices _fireSafetyServices;

        // Constructor uses Dependency Injection to inject services
        public FireSafetyController(IWebHostEnvironment webHostEnvironment, FireSafetyServices fireSafetyServices)
        {
            _webHostEnvironment = webHostEnvironment;
            _fireSafetyServices = fireSafetyServices;
        }

        [HttpPost]
        public IActionResult GenerateSafetyBoardCard(string option, string empCodes = null)
        {
            try
            {
                // Get employee data based on option and employee codes
                var employees = GetEmployees(option, empCodes);

                if (employees == null || !employees.Any())
                {
                    return BadRequest("No matching employees found.");
                }

                // Update image paths for employees
                UpdateEmployeeImagePaths(employees);

                // Generate and return the report as PDF
                return GeneratePdfReport(option, employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private List<Employee> GetEmployees(string option, string empCodes)
        {
            List<Employee> employees;

            if (option == "employee" && !string.IsNullOrEmpty(empCodes))
            {
                var empCodeList = empCodes.Split(',')
                                          .Select(e => e.Trim())
                                          .Where(e => !string.IsNullOrEmpty(e))
                                          .ToList();

                employees = _fireSafetyServices.GetEmployees()
                                               .Where(e => empCodeList.Contains(e.EmpCode))
                                               .OrderBy(e => e.EmpCode)
                                               .ToList();
            }
            else
            {
                employees = _fireSafetyServices.GetEmployees()
                                               .OrderBy(e => e.EmpCode)
                                               .ToList();
            }

            return employees;
        }

        private void UpdateEmployeeImagePaths(List<Employee> employees)
        {
            string imageDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "EmpPic");

            foreach (var employee in employees)
            {
                employee.EmpPicLocation = Path.Combine("file:///", imageDirectory, employee.EmpPicLocation)
                                            .Replace("\\", "/");
            }
        }

        private IActionResult GeneratePdfReport(string option, List<Employee> employees)
        {
            string rdlcPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Reports", "FireSafetyBanner.rdlc");

            var report = new LocalReport
            {
                ReportPath = rdlcPath,
                EnableExternalImages = true
            };

            var rds = new ReportDataSource("EmployeeData", employees);
            report.DataSources.Clear();
            report.DataSources.Add(rds);

            string deviceInfo = "<DeviceInfo><OutputFormat>PDF</OutputFormat><ColorDepth>24</ColorDepth></DeviceInfo>";
            byte[] reportBytes = report.Render("PDF", deviceInfo);

            // Generate file names for each employee
            var fileNames = employees.Select(e => $"{e.EmpCode}_SafetyCard.pdf").ToList();

            // If option is "employee", send one file per employee
            if (option == "employee")
            {
                return File(reportBytes, "application/pdf", fileNames.First());
            }
            else
            {
                // If option is "all", create a single PDF for all employees
                string allEmployeesFileName = "All_Employees_SafetyCards.pdf";
                return File(reportBytes, "application/pdf", allEmployeesFileName);
            }
        }
    }
}
