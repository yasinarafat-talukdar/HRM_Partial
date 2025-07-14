using PayrollToyHRD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using OfficeOpenXml;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace PayrollToyHRD.Controllers
{
    public class LeaveController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public LeaveController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        // Method to get the client machine name
        private string GetClientMachineName(HttpContext context)
        {
            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;
                if (remoteIpAddress != null)
                {
                    IPHostEntry hostEntry = Dns.GetHostEntry(remoteIpAddress);
                    return hostEntry.HostName ?? remoteIpAddress.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "Unknown Device";
        }

        [HttpGet]
        public IActionResult LeavePage()
        {
            string? remoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            // Normalize IPv6 loopback and IPv4-mapped IPv6 addresses
            if (remoteIpAddress == "::1")
            {
                remoteIpAddress = "127.0.0.1";
            }
            else if (remoteIpAddress != null && remoteIpAddress.StartsWith("::ffff:"))
            {
                remoteIpAddress = remoteIpAddress.Substring(7);
            }

            Console.WriteLine("Client IP: " + remoteIpAddress);

            var allowedIPs = new List<string>
    {
        "127.0.0.1",       // Local dev
        "192.168.1.101",
        "192.168.1.110",
        "192.168.1.200"
    };

            bool isAllowed = allowedIPs.Contains(remoteIpAddress);
            Console.WriteLine("Is Allowed: " + isAllowed);

            ViewBag.ClientIP = remoteIpAddress;
            ViewBag.IsAllowed = isAllowed;
            return View();
        }

        [HttpPost]
        public IActionResult GenerateLeaveForm(string empCode, string leaveType, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrEmpty(empCode) || string.IsNullOrEmpty(leaveType))
            {
                ViewData["ErrorMessage"] = "Employee code(s), leave type, start date, and end date are required.";
                return View("LeavePage");
            }

            var rdlcPath = Path.Combine(_webHostEnvironment.WebRootPath, "Reports", "LeaveFormRptNew.rdlc");
            string deviceName = GetClientMachineName(HttpContext);

            var empCodes = empCode.Split(',').Select(e => e.Trim()).ToList();
            DataTable leaveTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();

                    foreach (var code in empCodes)
                    {
                        SqlCommand leaveCommand = new SqlCommand("SELECT EmpCode, EmployeeName, SectionName, DeptName, Reason, EmpCodeB, BalanceCL, BalanceSL, DesigName, dtJoin FROM EmployeeLeave WHERE EmpCode = @empCode", connection);
                        leaveCommand.Parameters.AddWithValue("@empCode", code);

                        SqlDataAdapter leaveAdapter = new SqlDataAdapter(leaveCommand);
                        DataTable tempTable = new DataTable();
                        leaveAdapter.Fill(tempTable);

                        if (tempTable.Rows.Count > 0)
                        {
                            tempTable.Columns.Add("StartDate", typeof(DateTime));
                            tempTable.Columns.Add("EndDate", typeof(DateTime));
                            tempTable.Columns.Add("LeaveType", typeof(string));

                            foreach (DataRow row in tempTable.Rows)
                            {
                                row["StartDate"] = startDate;
                                row["EndDate"] = endDate;
                                row["LeaveType"] = leaveType;
                            }

                            leaveTable.Merge(tempTable);
                        }
                    }

                    if (leaveTable.Rows.Count == 0)
                    {
                        ViewData["ErrorMessage"] = "No leave records found for the provided employee codes.";
                        return View("LeavePage");
                    }

                    var report = new LocalReport
                    {
                        ReportPath = rdlcPath
                    };
                    report.DataSources.Clear();
                    report.DataSources.Add(new ReportDataSource("LeaveData", leaveTable));

                    var parameters = new ReportParameter[] { new ReportParameter("GeneratedBy", deviceName) };
                    report.SetParameters(parameters);

                    string deviceInfo = "<DeviceInfo>" +
                                        "<OutputFormat>PDF</OutputFormat>" +
                                        "<PageWidth>8.5in</PageWidth>" +
                                        "<PageHeight>11in</PageHeight>" +
                                        "<MarginTop>0.2in</MarginTop>" +
                                        "<MarginLeft>0.5in</MarginLeft>" +
                                        "<MarginRight>0.3in</MarginRight>" +
                                        "<MarginBottom>0.2in</MarginBottom>" +
                                        "</DeviceInfo>";

                    byte[] bytes = report.Render("PDF", deviceInfo);
                    string fileName = $"LeaveForm_{DateTime.Now:yyyyMMdd}.pdf";
                    Response.Headers.Append("Content-Disposition", $"inline; filename={fileName}");

                    return File(bytes, "application/pdf");
                }
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"An error occurred while generating the report: {ex.Message}";
                return View("LeavePage");
            }
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a valid Excel file.";
                return RedirectToAction("Upload");
            }

            var leaveRecords = new List<EmployeeLeave>();

            try
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            if (string.IsNullOrEmpty(worksheet.Cells[row, 1]?.Text)) continue;

                            var record = new EmployeeLeave
                            {
                                EmpCode = worksheet.Cells[row, 1]?.Text,
                                EmployeeName = worksheet.Cells[row, 2]?.Text,
                                SectionName = worksheet.Cells[row, 3]?.Text,
                                DeptName = worksheet.Cells[row, 4]?.Text,
                                LeaveType = worksheet.Cells[row, 5]?.Text,
                                StartDate = DateTime.TryParse(worksheet.Cells[row, 6]?.Text, out DateTime startDate) ? startDate : (DateTime?)null,
                                EndDate = DateTime.TryParse(worksheet.Cells[row, 7]?.Text, out DateTime endDate) ? endDate : (DateTime?)null,
                                Reason = worksheet.Cells[row, 8]?.Text,
                                DesigName = worksheet.Cells[row, 9]?.Text,
                                dtJoin = DateTime.TryParse(worksheet.Cells[row, 10]?.Text, out DateTime dtJoin) ? dtJoin : (DateTime?)null,
                                BalanceCL = worksheet.Cells[row, 11]?.Text,
                                BalanceSL = worksheet.Cells[row, 12]?.Text,
                                EmpCodeB = worksheet.Cells[row, 13]?.Text
                            };
                            leaveRecords.Add(record);
                        }
                    }
                }

                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();
                    foreach (var record in leaveRecords)
                    {
                        using (SqlCommand cmd = new SqlCommand("spLeaveStsUpdate", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            AddLeaveRecordParameters(cmd, record);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                TempData["Success"] = "Leave records uploaded successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while uploading the file: {ex.Message}";
            }

            return RedirectToAction("Upload");
        }

        private void AddLeaveRecordParameters(SqlCommand cmd, EmployeeLeave record)
        {
            cmd.Parameters.AddWithValue("@EmpCode", record.EmpCode ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@EmployeeName", record.EmployeeName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SectionName", record.SectionName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DeptName", record.DeptName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@LeaveType", record.LeaveType ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@StartDate", record.StartDate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@EndDate", record.EndDate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Reason", record.Reason ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DesigName", record.DesigName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@dtJoin", record.dtJoin ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@BalanceCL", record.BalanceCL ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@BalanceSL", record.BalanceSL ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@EmpCodeB", record.EmpCodeB ?? (object)DBNull.Value);
        }
    }
}
