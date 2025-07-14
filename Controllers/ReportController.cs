using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.NETCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using PayrollToyHRD.Data;
using PayrollToyHRD.Models;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Emit;


namespace PayrollToyHRD.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;


        public ReportController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            List<string> sections = GetSections();
            ViewBag.Sections = sections;
            return View();

        }

        public IActionResult LayOffSalarySheet()
        {
            List<string> sections = GetSections();
            ViewBag.Sections = sections;
            return View();

        }

        public IActionResult PostalAdCardList()
        {
           
            return View();

        }


        [HttpPost]
        public IActionResult LayOffSalaryReport(string sectName)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            DataTable dt;

            if (sectName == "ALL")
            {
                // Fetch data for all sections
                dt = GetLayOffSalaryData(connectionString, null); // passing null for SectName
            }
            else
            {
                // Fetch data for the selected section
                dt = GetLayOffSalaryData(connectionString, sectName);
            }

            if (dt.Rows.Count == 0)
            {
                TempData["Message"] = "No records found for the selected section.";
                return RedirectToAction("Index");
            }

            string rdlcFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Reports", "rptSalaryDisbusment.rdlc");

            LocalReport report = new LocalReport();
            report.ReportPath = rdlcFilePath;
            report.DataSources.Add(new ReportDataSource("DataSet1", dt));

            byte[] bytes = report.Render("PDF");
            //string fileName = !string.IsNullOrEmpty(sectName) ? $"{sectName.Replace(",", "_")}_PF_Application.pdf" : "All_Employees_PF_Applications.pdf";
            //Response.Headers.Append("Content-Disposition", $"inline; filename={fileName}");

            //return File(bytes, "application/pdf", $"{sectName}.pdf");
            return File(bytes, "application/pdf");
        }


        private DataTable GetLayOffSalaryData(string connectionString, string sectName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetLayOffSalaryAdvice", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // If sectName is "ALL" or null, pass NULL to the stored procedure
                    if (string.IsNullOrEmpty(sectName) || sectName == "ALL")
                    {
                        cmd.Parameters.AddWithValue("@SectName", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SectName", sectName);
                    }

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }


        private List<string> GetSections()
        {
            List<string> sections = new List<string>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT DISTINCT SectName FROM LayOffSalaryAdvice2025 ORDER BY SectName";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sections.Add(reader["SectName"].ToString());
                        }
                    }
                }
            }
            return sections;
        }













        //SalarySheetFinal-Lay-off Salary March-2025

        [HttpPost]
        public IActionResult LayOffSalarySheetReport(string sectName)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            DataTable dt;

            if (sectName == "ALL")
            {
                // Fetch data for all sections
                dt = GetLayOffSalarySheetData(connectionString, null); // passing null for SectName
            }
            else
            {
                // Fetch data for the selected section
                dt = GetLayOffSalarySheetData(connectionString, sectName);
            }

            if (dt.Rows.Count == 0)
            {
                TempData["Message"] = "No records found for the selected section.";
                return RedirectToAction("Index");
            }

            string rdlcFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Reports", "rptLayOffSalarySheet.rdlc");

            LocalReport report = new LocalReport();
            report.ReportPath = rdlcFilePath;
            report.DataSources.Add(new ReportDataSource("DataSet1", dt));

            byte[] bytes = report.Render("PDF");
            //string fileName = !string.IsNullOrEmpty(sectName) ? $"{sectName.Replace(",", "_")}_PF_Application.pdf" : "All_Employees_PF_Applications.pdf";
            //Response.Headers.Append("Content-Disposition", $"inline; filename={fileName}");

            //return File(bytes, "application/pdf", $"{sectName}.pdf");
            return File(bytes, "application/pdf");
        }


        private DataTable GetLayOffSalarySheetData(string connectionString, string sectName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("rptLayOffSalarySheet", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // If sectName is "ALL" or null, pass NULL to the stored procedure
                    if (string.IsNullOrEmpty(sectName) || sectName == "ALL")
                    {
                        cmd.Parameters.AddWithValue("@SectName", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SectName", sectName);
                    }

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }













        //////Final Salary Sheet and Final Advice

        [HttpPost]
        public IActionResult LayOffSalaryFinalAdvice(string sectName)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            DataTable dt;

            if (sectName == "ALL")
            {
                // Fetch data for all sections
                dt = GetLayOffSalaryFinalAdviceData(connectionString, null); // passing null for SectName
            }
            else
            {
                // Fetch data for the selected section
                dt = GetLayOffSalaryFinalAdviceData(connectionString, sectName);
            }

            if (dt.Rows.Count == 0)
            {
                TempData["Message"] = "No records found for the selected section.";
                return RedirectToAction("Index");
            }

            string rdlcFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Reports", "rptSalaryDisbusment.rdlc");

            LocalReport report = new LocalReport();
            report.ReportPath = rdlcFilePath;
            report.DataSources.Add(new ReportDataSource("DataSet1", dt));

            byte[] bytes = report.Render("PDF");
            //string fileName = !string.IsNullOrEmpty(sectName) ? $"{sectName.Replace(",", "_")}_PF_Application.pdf" : "All_Employees_PF_Applications.pdf";
            //Response.Headers.Append("Content-Disposition", $"inline; filename={fileName}");

            //return File(bytes, "application/pdf", $"{sectName}.pdf");
            return File(bytes, "application/pdf");
        }


        private DataTable GetLayOffSalaryFinalAdviceData(string connectionString, string sectName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("rptLayOffSalaryFinalAdvice", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // If sectName is "ALL" or null, pass NULL to the stored procedure
                    if (string.IsNullOrEmpty(sectName) || sectName == "ALL")
                    {
                        cmd.Parameters.AddWithValue("@SectName", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SectName", sectName);
                    }

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }











        ///AD CARD for POST OFFICE
        ///

        [HttpPost]
        public IActionResult PostalAdCardList(string sectName)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            DataTable dt;

            if (sectName == "ALL")
            {
                // Fetch data for all sections
                dt = GetPostalAdCardList(connectionString, null); // passing null for SectName
            }
            else
            {
                // Fetch data for the selected section
                dt = GetPostalAdCardList(connectionString, sectName);
            }

            if (dt.Rows.Count == 0)
            {
                TempData["Message"] = "No records found for the selected section.";
                return RedirectToAction("Index");
            }

            string rdlcFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Reports", "rptPostAdToy.rdlc");

            LocalReport report = new LocalReport();
            report.ReportPath = rdlcFilePath;
            report.DataSources.Add(new ReportDataSource("DataSet1", dt));

            byte[] bytes = report.Render("PDF");
            //string fileName = !string.IsNullOrEmpty(sectName) ? $"{sectName.Replace(",", "_")}_PF_Application.pdf" : "All_Employees_PF_Applications.pdf";
            //Response.Headers.Append("Content-Disposition", $"inline; filename={fileName}");

            //return File(bytes, "application/pdf", $"{sectName}.pdf");
            return File(bytes, "application/pdf");
        }


        private DataTable GetPostalAdCardList(string connectionString, string sectName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("rptPostAd20Toy", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // If sectName is "ALL" or null, pass NULL to the stored procedure
                    if (string.IsNullOrEmpty(sectName) || sectName == "ALL")
                    {
                        cmd.Parameters.AddWithValue("@SectName", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SectName", sectName);
                    }

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }







        ///AD CARD for POST OFFICE
        ///

        [HttpPost]
        public IActionResult PostalAdCardListMNC(string sectName)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            DataTable dt;

            if (sectName == "ALL")
            {
                // Fetch data for all sections
                dt = GetPostalAdCardListMNC(connectionString, null); // passing null for SectName
            }
            else
            {
                // Fetch data for the selected section
                dt = GetPostalAdCardListMNC(connectionString, sectName);
            }

            if (dt.Rows.Count == 0)
            {
                TempData["Message"] = "No records found for the selected section.";
                return RedirectToAction("Index");
            }

            string rdlcFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Reports", "rptPostAdMNC.rdlc");

            LocalReport report = new LocalReport();
            report.ReportPath = rdlcFilePath;
            report.DataSources.Add(new ReportDataSource("DataSet1", dt));

            byte[] bytes = report.Render("PDF");
            //string fileName = !string.IsNullOrEmpty(sectName) ? $"{sectName.Replace(",", "_")}_PF_Application.pdf" : "All_Employees_PF_Applications.pdf";
            //Response.Headers.Append("Content-Disposition", $"inline; filename={fileName}");

            //return File(bytes, "application/pdf", $"{sectName}.pdf");
            return File(bytes, "application/pdf");
        }


        private DataTable GetPostalAdCardListMNC(string connectionString, string sectName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("rptPostAdMNC", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // If sectName is "ALL" or null, pass NULL to the stored procedure
                    if (string.IsNullOrEmpty(sectName) || sectName == "ALL")
                    {
                        cmd.Parameters.AddWithValue("@SectName", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SectName", sectName);
                    }

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }





        //public IActionResult FinalSettlementReport()
        //{
        //    List<string> processTypes = new List<string>();
        //    string connectionString = _configuration.GetConnectionString("DefaultConnection");

        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        string query = "SELECT DISTINCT ProssType FROM tblEmployeeSettlement WHERE ProssType IS NOT NULL";
        //        using (SqlCommand cmd = new SqlCommand(query, conn))
        //        {
        //            conn.Open();
        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    processTypes.Add(reader.GetString(0));
        //                }
        //            }
        //        }
        //    }

        //    ViewBag.ProcessTypes = new SelectList(processTypes);
        //    return View();
        //}

        //[HttpPost]
        //public IActionResult FinalSettlementReports(string empCode, string processType)
        //{
        //    try
        //    {
        //        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        //        DataTable dt = GetFinalSettlementData(connectionString, empCode, processType);

        //        if (dt.Rows.Count == 0)
        //        {
        //            TempData["Message"] = "No data found for this employee and process type.";
        //            return RedirectToAction("FinalSettlementReport");
        //        }

        //        string rdlcFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Reports", "rptFinalSettlement.rdlc");

        //        if (!System.IO.File.Exists(rdlcFilePath))
        //        {
        //            TempData["Message"] = "Report file not found.";
        //            return RedirectToAction("FinalSettlementReport");
        //        }

        //        LocalReport report = new LocalReport();
        //        report.ReportPath = rdlcFilePath;
        //        report.DataSources.Add(new ReportDataSource("DataSet1", dt));

        //        byte[] reportBytes = report.Render("PDF");

        //        return File(reportBytes, "application/pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Message"] = "Error generating report: " + ex.Message;
        //        return RedirectToAction("FinalSettlementReport");
        //    }
        //}

        //private DataTable GetFinalSettlementData(string connectionString, string empCode, string processType)
        //{
        //    DataTable dt = new DataTable();

        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("rptFinalSettlement", conn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@EmpCode", empCode);
        //            cmd.Parameters.AddWithValue("@ProssType", processType);

        //            SqlDataAdapter da = new SqlDataAdapter(cmd);
        //            da.Fill(dt);
        //        }
        //    }

        //    return dt;
        //}

































        [HttpGet]
        public IActionResult FinalSettlementReport()
        {
            string? remoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (remoteIpAddress == "::1")
                remoteIpAddress = "127.0.0.1";
            else if (remoteIpAddress?.StartsWith("::ffff:") == true)
                remoteIpAddress = remoteIpAddress.Substring(7);

            var allowedIPs = new List<string> { "127.0.0.1", "192.168.1.101", "192.168.1.110", "192.168.1.200" };
            ViewBag.ClientIP = remoteIpAddress;
            ViewBag.IsAllowed = allowedIPs.Contains(remoteIpAddress);
            ViewBag.ProcessTypeList = GetProcessTypeList();

            if (TempData["Message"] != null)
                ViewBag.Message = TempData["Message"];

            return View();
        }

        [HttpPost]
        public IActionResult FinalSettlementReport(string processType, string empCode)
        {
            if (string.IsNullOrEmpty(processType) && string.IsNullOrEmpty(empCode))
            {
                TempData["Message"] = "Please select a Process Type or enter an Employee Code.";
                return RedirectToAction("FinalSettlementReport");
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            DataTable dt = GetSettlementDataByProcessType(connectionString, processType, empCode);

            if (dt.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the given criteria.";
                return RedirectToAction("FinalSettlementReport");
            }

            string rdlcFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Reports", "rptFinalSettlement.rdlc");

            LocalReport report = new LocalReport();
            report.ReportPath = rdlcFilePath;
            report.DataSources.Add(new ReportDataSource("DataSet1", dt));

            byte[] bytes = report.Render("PDF");

            return File(bytes, "application/pdf");
        }




        private List<string> GetProcessTypeList()
        {
            return _context.EmployeeSettlements
                           .Select(x => x.ProcessType)
                           .Where(x => !string.IsNullOrEmpty(x))
                           .Distinct()
                           .OrderBy(x => x)
                           .ToList();
        }

        private DataTable GetSettlementDataByProcessType(string connectionString, string processType, string empCode)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("rptFinalSettlement", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProcessType", processType);
                    cmd.Parameters.AddWithValue("@EmpCode", string.IsNullOrEmpty(empCode) ? (object)DBNull.Value : empCode);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadExcel(IFormFile excelFile)
        {


            if (excelFile == null || excelFile.Length == 0)
            {
                TempData["Message"] = "Please select a valid Excel file.";
                return RedirectToAction("FinalSettlementReport");
            }

            var dataTable = new DataTable();
            dataTable.Columns.AddRange(new[]
{
    new DataColumn("EmpCode"),
    new DataColumn("EmpCodeB"),
    new DataColumn("EmpNameB"),
    new DataColumn("DesigNameB"),
    new DataColumn("SectNameB"),
    new DataColumn("LineNameB"),
    new DataColumn("dtJoin", typeof(DateTime)) { AllowDBNull = true },
    new DataColumn("dtReleased", typeof(DateTime)) { AllowDBNull = true },
    new DataColumn("dtResigned", typeof(DateTime)) { AllowDBNull = true },
    new DataColumn("ServiceLength"),
    new DataColumn("PresentDayY1", typeof(int)) { AllowDBNull = true },
    new DataColumn("PresentDayY2", typeof(int)) { AllowDBNull = true },
    new DataColumn("ELYear1", typeof(int)) { AllowDBNull = true },
    new DataColumn("ELYear2", typeof(int)) { AllowDBNull = true },
    new DataColumn("ServBenefitDays", typeof(int)) { AllowDBNull = true },
    new DataColumn("Stamp", typeof(int)) { AllowDBNull = true },
    new DataColumn("GS", typeof(decimal)) { AllowDBNull = true },
    new DataColumn("BS", typeof(decimal)) { AllowDBNull = true },
    new DataColumn("HR", typeof(decimal)) { AllowDBNull = true },
    new DataColumn("MA", typeof(decimal)) { AllowDBNull = true },
    new DataColumn("LastMonthSalary", typeof(decimal)) { AllowDBNull = true },
    new DataColumn("NoticePeriodDeduct", typeof(decimal)) { AllowDBNull = true },
    new DataColumn("ExOtAmount", typeof(decimal)) { AllowDBNull = true },
    new DataColumn("IsLefty", typeof(bool)) { AllowDBNull = true },
    new DataColumn("IsResigned", typeof(bool)) { AllowDBNull = true },
    new DataColumn("ProcessType", typeof(string))
});


            try
            {
                using (var stream = excelFile.OpenReadStream())
                {
                    IWorkbook workbook = new XSSFWorkbook(stream);
                    ISheet sheet = workbook.GetSheetAt(0);

                    for (int row = 1; row <= sheet.LastRowNum; row++)
                    {
                        var currentRow = sheet.GetRow(row);
                        if (currentRow == null) continue;

                        string empCode = currentRow.GetCell(0)?.ToString()?.Trim();
                        if (string.IsNullOrEmpty(empCode)) continue; // Skip if EmpCode is missing

                        var dr = dataTable.NewRow();
                        dr["EmpCode"] = empCode;
                        dr["EmpCodeB"] = currentRow.GetCell(1)?.ToString()?.Trim();
                        dr["EmpNameB"] = currentRow.GetCell(2)?.ToString()?.Trim();
                        dr["DesigNameB"] = currentRow.GetCell(3)?.ToString()?.Trim();
                        dr["SectNameB"] = currentRow.GetCell(4)?.ToString()?.Trim();
                        dr["LineNameB"] = currentRow.GetCell(5)?.ToString()?.Trim();
                        dr["dtJoin"] = TryParseDate(currentRow.GetCell(6));
                        dr["dtReleased"] = TryParseDate(currentRow.GetCell(7));
                        dr["dtResigned"] = TryParseDate(currentRow.GetCell(8));
                        dr["ServiceLength"] = currentRow.GetCell(9)?.ToString()?.Trim();
                        dr["PresentDayY1"] = TryParseInt(currentRow.GetCell(10));
                        dr["PresentDayY2"] = TryParseInt(currentRow.GetCell(11));
                        dr["ELYear1"] = TryParseInt(currentRow.GetCell(12));
                        dr["ELYear2"] = TryParseInt(currentRow.GetCell(13));
                        dr["ServBenefitDays"] = TryParseInt(currentRow.GetCell(14));
                        dr["Stamp"] = TryParseInt(currentRow.GetCell(15));
                        dr["GS"] = TryParseDecimal(currentRow.GetCell(16));
                        dr["BS"] = TryParseDecimal(currentRow.GetCell(17));
                        dr["HR"] = TryParseDecimal(currentRow.GetCell(18));
                        dr["MA"] = TryParseDecimal(currentRow.GetCell(19));
                        dr["LastMonthSalary"] = TryParseDecimal(currentRow.GetCell(20));
                        dr["NoticePeriodDeduct"] = TryParseDecimal(currentRow.GetCell(21));
                        dr["ExOtAmount"] = TryParseDecimal(currentRow.GetCell(22));
                        dr["IsLefty"] = TryParseBool(currentRow.GetCell(23));
                        dr["IsResigned"] = TryParseBool(currentRow.GetCell(24));
                        dr["ProcessType"] = currentRow.GetCell(25)?.ToString()?.Trim();

                        dataTable.Rows.Add(dr);

                    }
                }



                var connStr = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                    {
                        bulkCopy.DestinationTableName = "EmployeeSettlement";
                        await conn.OpenAsync();
                        await bulkCopy.WriteToServerAsync(dataTable);
                    }
                }

                TempData["Message"] = "Increments uploaded successfully!";
                return RedirectToAction("FinalSettlementReport");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error: " + ex.Message;
                return RedirectToAction("FinalSettlementReport");
            }
        }


        private object TryParseDecimal(ICell cell)
        {
            if (cell == null || cell.CellType == CellType.Blank)
                return DBNull.Value;

            if (cell.CellType == CellType.Numeric)
                return cell.NumericCellValue;

            if (decimal.TryParse(cell.ToString(), out decimal value))
                return value;

            return DBNull.Value;
        }

        private static DateTime? TryParseDate(ICell cell)
        {
            if (cell == null) return null;
            if (DateTime.TryParse(cell.ToString(), out var result))
                return result;
            return null;
        }

        private static int? TryParseInt(ICell cell)
        {
            if (cell == null) return null;
            if (int.TryParse(cell.ToString(), out var result))
                return result;
            return null;
        }

        private static bool? TryParseBool(ICell cell)
        {
            if (cell == null) return null;
            if (bool.TryParse(cell.ToString(), out var result))
                return result;
            // You can also handle numeric true/false (1/0)
            if (int.TryParse(cell.ToString(), out var intVal))
                return intVal != 0;
            return null;
        }


        [HttpPost]
        public async Task<IActionResult> DeleteByProcessType(string processType)
        {

            try
            {
                if (string.IsNullOrEmpty(processType))
                {
                    TempData["Message"] = "Error: Increment type not selected.";
                    return RedirectToAction("FinalSettlementReport");
                }

                await _context.EmployeeSettlements
                              .Where(x => x.ProcessType == processType)
                              .ExecuteDeleteAsync(); // No primary key needed

                TempData["Message"] = $"Successfully deleted records for increment type: {processType}";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Error during deletion: {ex.Message}";
            }

            return RedirectToAction("FinalSettlementReport");
        }














































        ///Settlement Program for MNC Apparel Limited
        ///

        [HttpPost]
        public IActionResult FinalSettlementReportMNC(string empCode)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            DataTable dt = GetFinalSettlementDataMNC(connectionString, empCode);

            if (dt.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for this employee.";
                return RedirectToAction("FinalSettlementReport");
            }

            string rdlcFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Reports", "rptFinalSettlement-retrenchment - MNC.rdlc");

            LocalReport report = new LocalReport();
            report.ReportPath = rdlcFilePath;
            report.DataSources.Add(new ReportDataSource("DataSet1", dt));

            byte[] bytes = report.Render("PDF");

            return File(bytes, "application/pdf");
        }

        private DataTable GetFinalSettlementDataMNC(string connectionString, string empCode)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("rptFinalSettlementMNC", conn)) // your SP or inline query
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmpCode", empCode);


                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }





        //SalarySheetFinal-Lay-off Salary April-2025

        //[HttpPost]
        //public IActionResult LayOffSalarySheetApril25Report(string sectName)
        //{
        //    string connectionString = _configuration.GetConnectionString("DefaultConnection");
        //    DataTable dt;

        //    if (sectName == "ALL")
        //    {
        //        // Fetch data for all sections
        //        dt = GetLayOffSalarySheetApril25Data(connectionString, null); // passing null for SectName
        //    }
        //    else
        //    {
        //        // Fetch data for the selected section
        //        dt = GetLayOffSalarySheetApril25Data(connectionString, sectName);
        //    }

        //    if (dt.Rows.Count == 0)
        //    {
        //        TempData["Message"] = "No records found for the selected section.";
        //        return RedirectToAction("Index");
        //    }

        //    string rdlcFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Reports", "rptLayOffSalarySheet-april.rdlc");

        //    LocalReport report = new LocalReport();
        //    report.ReportPath = rdlcFilePath;
        //    report.DataSources.Add(new ReportDataSource("DataSet1", dt));

        //    byte[] bytes = report.Render("PDF");
        //    //string fileName = !string.IsNullOrEmpty(sectName) ? $"{sectName.Replace(",", "_")}_PF_Application.pdf" : "All_Employees_PF_Applications.pdf";
        //    //Response.Headers.Append("Content-Disposition", $"inline; filename={fileName}");

        //    //return File(bytes, "application/pdf", $"{sectName}.pdf");
        //    return File(bytes, "application/pdf");
        //}



        [HttpPost]
        public IActionResult LayOffSalarySheetApril25Report(string sectName)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            DataTable dt;

            if (sectName == "ALL")
            {
                // Fetch data for all sections
                dt = GetLayOffSalarySheetApril25Data(connectionString, null);
            }
            else
            {
                // Fetch data for the selected section
                dt = GetLayOffSalarySheetApril25Data(connectionString, sectName);
            }

            if (dt.Rows.Count == 0)
            {
                TempData["Message"] = "No records found for the selected section.";
                return RedirectToAction("Index");
            }

            string rdlcFileName;

            // Logic to choose RDLC based on sectName
            if (sectName == "OFFICE" || sectName == "OFFICE-P")
            {
                rdlcFileName = "rptLayOffSalarySheet-april.rdlc";
            }
            else
            {
                rdlcFileName = "rptLayOffSalarySheet-april-Worker.rdlc";
            }

            string rdlcFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Reports", rdlcFileName);

            LocalReport report = new LocalReport();
            report.ReportPath = rdlcFilePath;
            report.DataSources.Add(new ReportDataSource("DataSet1", dt));

            byte[] bytes = report.Render("PDF");

            return File(bytes, "application/pdf");
        }



        private DataTable GetLayOffSalarySheetApril25Data(string connectionString, string sectName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("rptLayOffSalarySheetApril25", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // If sectName is "ALL" or null, pass NULL to the stored procedure
                    if (string.IsNullOrEmpty(sectName) || sectName == "ALL")
                    {
                        cmd.Parameters.AddWithValue("@SectName", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SectName", sectName);
                    }

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }



        //////Final Salary Sheet and Final Advice

        [HttpPost]
        public IActionResult LayOffSalaryApril25FinalAdvice(string sectName)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            DataTable dt;

            if (sectName == "ALL")
            {
                // Fetch data for all sections
                dt = GetLayOffSalaryApril25FinalAdviceData(connectionString, null); // passing null for SectName
            }
            else
            {
                // Fetch data for the selected section
                dt = GetLayOffSalaryApril25FinalAdviceData(connectionString, sectName);
            }

            if (dt.Rows.Count == 0)
            {
                TempData["Message"] = "No records found for the selected section.";
                return RedirectToAction("Index");
            }

            string rdlcFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Reports", "rptSalaryDisbusment.rdlc");

            LocalReport report = new LocalReport();
            report.ReportPath = rdlcFilePath;
            report.DataSources.Add(new ReportDataSource("DataSet1", dt));

            byte[] bytes = report.Render("PDF");
            //string fileName = !string.IsNullOrEmpty(sectName) ? $"{sectName.Replace(",", "_")}_PF_Application.pdf" : "All_Employees_PF_Applications.pdf";
            //Response.Headers.Append("Content-Disposition", $"inline; filename={fileName}");

            //return File(bytes, "application/pdf", $"{sectName}.pdf");
            return File(bytes, "application/pdf");
        }


        private DataTable GetLayOffSalaryApril25FinalAdviceData(string connectionString, string sectName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("rptLayOffSalaryApril25FinalAdvice", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // If sectName is "ALL" or null, pass NULL to the stored procedure
                    if (string.IsNullOrEmpty(sectName) || sectName == "ALL")
                    {
                        cmd.Parameters.AddWithValue("@SectName", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SectName", sectName);
                    }

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }




































        [HttpPost]
        public IActionResult LayOffSalarySheetMay25Report(string sectName)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            DataTable dt;

            if (sectName == "ALL")
            {
                // Fetch data for all sections
                dt = GetLayOffSalarySheetMay25Data(connectionString, null);
            }
            else
            {
                // Fetch data for the selected section
                dt = GetLayOffSalarySheetMay25Data(connectionString, sectName);
            }

            if (dt.Rows.Count == 0)
            {
                TempData["Message"] = "No records found for the selected section.";
                return RedirectToAction("Index");
            }

            string rdlcFileName;

            // Logic to choose RDLC based on sectName
            if (sectName == "OFFICE" || sectName == "OFFICE-P")
            {
                rdlcFileName = "rptLayOffSalarySheet-May.rdlc";
            }
            else
            {
                rdlcFileName = "rptLayOffSalarySheet-May-Worker.rdlc";
            }

            string rdlcFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Reports", rdlcFileName);

            LocalReport report = new LocalReport();
            report.ReportPath = rdlcFilePath;
            report.DataSources.Add(new ReportDataSource("DataSet1", dt));

            byte[] bytes = report.Render("PDF");

            return File(bytes, "application/pdf");
        }



        private DataTable GetLayOffSalarySheetMay25Data(string connectionString, string sectName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("rptLayOffSalarySheetMay25", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // If sectName is "ALL" or null, pass NULL to the stored procedure
                    if (string.IsNullOrEmpty(sectName) || sectName == "ALL")
                    {
                        cmd.Parameters.AddWithValue("@SectName", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SectName", sectName);
                    }

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }



        //////Final Salary Sheet and Final Advice

        [HttpPost]
        public IActionResult LayOffSalaryMay25FinalAdvice(string sectName)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            DataTable dt;

            if (sectName == "ALL")
            {
                // Fetch data for all sections
                dt = GetLayOffSalaryMay25FinalAdviceData(connectionString, null); // passing null for SectName
            }
            else
            {
                // Fetch data for the selected section
                dt = GetLayOffSalaryMay25FinalAdviceData(connectionString, sectName);
            }

            if (dt.Rows.Count == 0)
            {
                TempData["Message"] = "No records found for the selected section.";
                return RedirectToAction("Index");
            }

            string rdlcFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Reports", "rptSalaryDisbusment.rdlc");

            LocalReport report = new LocalReport();
            report.ReportPath = rdlcFilePath;
            report.DataSources.Add(new ReportDataSource("DataSet1", dt));

            byte[] bytes = report.Render("PDF");
            //string fileName = !string.IsNullOrEmpty(sectName) ? $"{sectName.Replace(",", "_")}_PF_Application.pdf" : "All_Employees_PF_Applications.pdf";
            //Response.Headers.Append("Content-Disposition", $"inline; filename={fileName}");

            //return File(bytes, "application/pdf", $"{sectName}.pdf");
            return File(bytes, "application/pdf");
        }


        private DataTable GetLayOffSalaryMay25FinalAdviceData(string connectionString, string sectName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("rptLayOffSalaryMay25Advice", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // If sectName is "ALL" or null, pass NULL to the stored procedure
                    if (string.IsNullOrEmpty(sectName) || sectName == "ALL")
                    {
                        cmd.Parameters.AddWithValue("@SectName", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SectName", sectName);
                    }

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }





















        //Transport Bill System///



        // ===============================
        // Upload Transport Attendance
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadTrasportAttn(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Message"] = "Please select a valid Excel file.";
                return RedirectToAction("TransportBillGenerate");
            }

            var dataTable = new DataTable();
            dataTable.Columns.AddRange(new[]
            {
        new DataColumn("EmpCode"),
        new DataColumn("EmpName"),
        new DataColumn("EmpDesig"),
        new DataColumn("StoppageName"),
        new DataColumn("BusName"),
        new DataColumn("dtPresent", typeof(DateTime)),
        new DataColumn("Section"),   // <-- New
    new DataColumn("Floor")      // <-- New
    });

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    IWorkbook workbook = new XSSFWorkbook(stream);
                    ISheet sheet = workbook.GetSheetAt(0);

                    for (int row = 1; row <= sheet.LastRowNum; row++) // row 0 is header
                    {
                        var currentRow = sheet.GetRow(row);
                        if (currentRow == null) continue;

                        string empCode = currentRow.GetCell(0)?.ToString()?.Trim();
                        if (string.IsNullOrEmpty(empCode)) continue;

                        var dr = dataTable.NewRow();
                        dr["EmpCode"] = empCode;
                        dr["EmpName"] = currentRow.GetCell(1)?.ToString()?.Trim();
                        dr["EmpDesig"] = currentRow.GetCell(2)?.ToString()?.Trim();
                        dr["StoppageName"] = currentRow.GetCell(3)?.ToString()?.Trim();
                        dr["BusName"] = currentRow.GetCell(4)?.ToString()?.Trim();
                        dr["dtPresent"] = TryParseDate(currentRow.GetCell(5));
                        dr["Section"] = currentRow.GetCell(6)?.ToString()?.Trim(); // Assuming column 6 is Section
                        dr["Floor"] = currentRow.GetCell(7)?.ToString()?.Trim();   // Assuming column 7 is Floor


                        dataTable.Rows.Add(dr);
                    }
                }

                var connStr = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                    {
                        bulkCopy.DestinationTableName = "tblAttendanceTransport";
                        await conn.OpenAsync();
                        await bulkCopy.WriteToServerAsync(dataTable);
                    }
                }

                TempData["Message"] = "Transport Attendance uploaded successfully!";
                return RedirectToAction("TransportBillGenerate");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error: " + ex.Message;
                return RedirectToAction("TransportBillGenerate");
            }
        }


        // ===============================
        // View Upload + Report Form Page
        // ===============================
        [HttpGet]
        public IActionResult TransportBillGenerate()
        {
            var floorList = _context.tblAttendanceTransports
        .Select(x => x.Floor)
        .Distinct()
        .OrderBy(f => f)
        .ToList();

            ViewBag.FloorList = new SelectList(floorList);
            return View();
        }

        // ===============================
        // Generate RDLC Transport Bill Report using Stored Procedure
        // ===============================
        [HttpPost]
        public IActionResult TransportBillReportViewer(DateTime fromDate, DateTime toDate, string empCode, bool isBothWay, string floor)
        {
            if (fromDate == default || toDate == default)
            {
                TempData["Message"] = "Please select From Date and To Date.";
                return RedirectToAction("TransportBillGenerate");
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            DataTable dt = GetTransportBillData(connectionString, fromDate, toDate, empCode, isBothWay, floor);

            if (dt.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the given criteria.";
                return RedirectToAction("TransportBillGenerate");
            }

            string rdlcFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Reports", "TransportBillReport.rdlc");

            LocalReport report = new LocalReport();
            report.ReportPath = rdlcFilePath;
            report.DataSources.Add(new ReportDataSource("DataSet1", dt)); // This name must match RDLC dataset name

            ReportParameter paramIsBothWay = new ReportParameter("IsBothWay", isBothWay ? "1" : "0");
            report.SetParameters(paramIsBothWay);

            byte[] bytes = report.Render("PDF");

            return File(bytes, "application/pdf", "TransportBill.pdf");
        }

        // ===============================
        // Helper Method to Fetch Data from Stored Procedure
        // ===============================
        private DataTable GetTransportBillData(string connectionString, DateTime fromDate, DateTime toDate, string empCode, bool isBothWay, string floor)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_TransportBillReport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);
                    cmd.Parameters.AddWithValue("@EmpCode", string.IsNullOrEmpty(empCode) ? DBNull.Value : (object)empCode);
                    cmd.Parameters.AddWithValue("@IsBothWay", isBothWay ? 1 : 0);
                    cmd.Parameters.AddWithValue("@Floor", string.IsNullOrEmpty(floor) ? DBNull.Value : (object)floor);


                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

    }
}