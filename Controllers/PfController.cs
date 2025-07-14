using PayrollToyHRD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using System.Data;
using System.Data.SqlClient;

namespace PayrollToyHRD.Controllers
{
    public class PfController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _connectionString;

        public PfController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public IActionResult GeneratePfApplicationView() => View();

        [HttpPost]
        public IActionResult GeneratePfApplication(string option, string empCode = null)
        {
            try
            {
                var employees = FetchEmployeesFromDatabase(empCode);
                if (employees == null || !employees.Any())
                {
                    return BadRequest("No matching employees found.");
                }

                return GeneratePdfReport(employees, empCode);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private List<Employee> FetchEmployeesFromDatabase(string empCodes)
        {
            var employees = new List<Employee>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("rptPFapplication", connection) { CommandType = CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@EmpCodes", string.IsNullOrEmpty(empCodes) ? DBNull.Value : empCodes);

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                employees.Add(new Employee
                {
                    EmpCode = reader["EmpCode"].ToString(),
                    EmpCodeB = reader["EmpCodeB"].ToString(),
                    //EmpName = reader["EmpName"].ToString(),
                    EmpNameB = reader["EmpNameB"].ToString(),
                    //dtJoin = reader.GetDateTime(reader.GetOrdinal("dtJoin")),
                    //BloodGroup = reader["BloodGroup"].ToString(),
                    SectName = reader["SectName"].ToString(),
                    //EmpPerAdd = reader["EmpPerAdd"].ToString(),
                    //VoterNo = reader["VoterNo"].ToString(),
                    //EmpPicLocation = reader["EmpPicLocation"].ToString(),
                    DesigNameB = reader["DesigNameB"].ToString(),
                    //EmpMobile = reader["EmpMobile"].ToString()
                });
            }

            return employees;
        }

        private IActionResult GeneratePdfReport(List<Employee> employees, string empCode)
        {
            string reportPath1 = Path.Combine(_webHostEnvironment.WebRootPath, "Reports", "rptPFapplication.rdlc");
            string reportPath2 = Path.Combine(_webHostEnvironment.WebRootPath, "Reports", "rptPFapplication2.rdlc");

            var report1 = new LocalReport { ReportPath = reportPath1, EnableExternalImages = true };
            var report2 = new LocalReport { ReportPath = reportPath2, EnableExternalImages = true };

            var rds = new ReportDataSource("EmployeeData", employees);
            report1.DataSources.Add(rds);
            report2.DataSources.Add(rds);

            string deviceInfo = "<DeviceInfo><OutputFormat>PDF</OutputFormat><MarginTop>0.2in</MarginTop><ColorDepth>24</ColorDepth></DeviceInfo>";

            byte[] reportBytes1 = report1.Render("PDF", deviceInfo);
            byte[] reportBytes2 = report2.Render("PDF", deviceInfo);

            byte[] mergedPdf = MergePdfReports(reportBytes1, reportBytes2);

            string fileName = !string.IsNullOrEmpty(empCode) ? $"{empCode.Replace(",", "_")}_PF_Application.pdf" : "All_Employees_PF_Applications.pdf";
            Response.Headers.Append("Content-Disposition", $"inline; filename={fileName}");

            return File(mergedPdf, "application/pdf");
        }


        private byte[] MergePdfReports(params byte[][] reports)
        {
            using var memoryStream = new MemoryStream();
            using var document = new iTextSharp.text.Document();
            using var pdfCopy = new iTextSharp.text.pdf.PdfCopy(document, memoryStream);
            document.Open();

            foreach (var report in reports)
            {
                using var reader = new iTextSharp.text.pdf.PdfReader(report);
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    pdfCopy.AddPage(pdfCopy.GetImportedPage(reader, i));
                }
            }

            document.Close();
            return memoryStream.ToArray();
        }

    }
}
