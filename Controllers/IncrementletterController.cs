using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollToyHRD.Data;
using PayrollToyHRD.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.Reporting.NETCore;

namespace PayrollToyHRD.Controllers
{
    public class IncrementLetterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public IncrementLetterController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
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
            ViewBag.IncTypeList = GetIncTypeList();

            return View();
        }




        //private string GetClientIp(HttpContext context)
        //{
        //    // First check for proxy headers (if applicable)
        //    if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
        //    {
        //        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        //        if (!string.IsNullOrWhiteSpace(forwardedFor))
        //            return forwardedFor.Split(',')[0];
        //    }

        //    // Fallback to remote IP address
        //    return context.Connection.RemoteIpAddress?.ToString();
        //}





        [HttpPost]
        public IActionResult IncrementLetterReport(string incType)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            DataTable dt = GetIncrementLetterDataByIncType(connectionString, incType);

            if (dt.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for this employee.";
                return RedirectToAction("Index");
            }

            string rdlcFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Reports", "rptIncrementLetter.rdlc");

            LocalReport report = new LocalReport();
            report.ReportPath = rdlcFilePath;
            report.DataSources.Add(new ReportDataSource("DataSet1", dt));

            byte[] bytes = report.Render("PDF");

            return File(bytes, "application/pdf");
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadExcel(IFormFile excelFile)
        {


            if (excelFile == null || excelFile.Length == 0)
            {
                TempData["Message"] = "Please select a valid Excel file.";
                return RedirectToAction("Index");
            }

            var dataTable = new DataTable();
            dataTable.Columns.AddRange(new[]
            {
                new DataColumn("EmpCode"),
                new DataColumn("EmpCodeB"),
                new DataColumn("SectNameB"),
                new DataColumn("EmpNameB"),
                new DataColumn("DesigNameB"),
                new DataColumn("Old_Basic", typeof(decimal)) { AllowDBNull = true },
                new DataColumn("Inc_Basic", typeof(decimal)) { AllowDBNull = true },
                new DataColumn("New_Basic", typeof(decimal)) { AllowDBNull = true },
                new DataColumn("New_HouseRent", typeof(decimal)) { AllowDBNull = true },
                new DataColumn("New_Gross", typeof(decimal)) { AllowDBNull = true },
                new DataColumn("ProcType"),
                new DataColumn("IncType")
            });

            try
            {
                using (var stream = excelFile.OpenReadStream())
                {
                    IWorkbook workbook = new XSSFWorkbook(stream);
                    ISheet sheet = workbook.GetSheetAt(0);

                    //for (int row = 1; row <= sheet.LastRowNum; row++)
                    //{
                    //    var currentRow = sheet.GetRow(row);
                    //    if (currentRow == null) continue;

                    //    var dr = dataTable.NewRow();
                    //    dr["EmpCode"] = currentRow.GetCell(0)?.ToString()?.Trim();
                    //    dr["EmpCodeB"] = currentRow.GetCell(1)?.ToString()?.Trim();
                    //    dr["SectNameB"] = currentRow.GetCell(2)?.ToString()?.Trim();
                    //    dr["EmpNameB"] = currentRow.GetCell(3)?.ToString()?.Trim();
                    //    dr["DesigNameB"] = currentRow.GetCell(4)?.ToString()?.Trim();
                    //    dr["Old_Basic"] = TryParseDecimal(currentRow.GetCell(5));
                    //    dr["Inc_Basic"] = TryParseDecimal(currentRow.GetCell(6));
                    //    dr["New_Basic"] = TryParseDecimal(currentRow.GetCell(7));
                    //    dr["New_HouseRent"] = TryParseDecimal(currentRow.GetCell(8));
                    //    dr["New_Gross"] = TryParseDecimal(currentRow.GetCell(9));
                    //    dr["ProcType"] = currentRow.GetCell(10)?.ToString()?.Trim();
                    //    dr["IncType"] = currentRow.GetCell(11)?.ToString()?.Trim();
                    //    dataTable.Rows.Add(dr);
                    //}


                    for (int row = 1; row <= sheet.LastRowNum; row++)
                    {
                        var currentRow = sheet.GetRow(row);
                        if (currentRow == null) continue;

                        string empCode = currentRow.GetCell(0)?.ToString()?.Trim();
                        if (string.IsNullOrEmpty(empCode)) continue; // Skip if EmpCode is missing

                        var dr = dataTable.NewRow();
                        dr["EmpCode"] = empCode;
                        dr["EmpCodeB"] = currentRow.GetCell(1)?.ToString()?.Trim();
                        dr["SectNameB"] = currentRow.GetCell(2)?.ToString()?.Trim();
                        dr["EmpNameB"] = currentRow.GetCell(3)?.ToString()?.Trim();
                        dr["DesigNameB"] = currentRow.GetCell(4)?.ToString()?.Trim();
                        dr["Old_Basic"] = TryParseDecimal(currentRow.GetCell(5));
                        dr["Inc_Basic"] = TryParseDecimal(currentRow.GetCell(6));
                        dr["New_Basic"] = TryParseDecimal(currentRow.GetCell(7));
                        dr["New_HouseRent"] = TryParseDecimal(currentRow.GetCell(8));
                        dr["New_Gross"] = TryParseDecimal(currentRow.GetCell(9));
                        dr["ProcType"] = currentRow.GetCell(10)?.ToString()?.Trim();
                        dr["IncType"] = currentRow.GetCell(11)?.ToString()?.Trim();

                        dataTable.Rows.Add(dr);
                    }
                }



                var connStr = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                    {
                        bulkCopy.DestinationTableName = "tblEmployeeIncrementLetter";
                        await conn.OpenAsync();
                        await bulkCopy.WriteToServerAsync(dataTable);
                    }
                }

                TempData["Message"] = "Increments uploaded successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        private List<string> GetIncTypeList()
        {
            return _context.IncrementLetters
                           .Select(x => x.IncType)
                           .Where(x => !string.IsNullOrEmpty(x))
                           .Distinct()
                           .OrderBy(x => x)
                           .ToList();
        }

        private DataTable GetIncrementLetterDataByIncType(string connectionString, string incType)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("rptIncrementLetter", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IncType", incType);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
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



        [HttpPost]
        public async Task<IActionResult> DeleteByIncType(string incType)
        {

            try
            {
                if (string.IsNullOrEmpty(incType))
                {
                    TempData["Message"] = "Error: Increment type not selected.";
                    return RedirectToAction("Index");
                }

                await _context.IncrementLetters
                              .Where(x => x.IncType == incType)
                              .ExecuteDeleteAsync(); // No primary key needed

                TempData["Message"] = $"Successfully deleted records for increment type: {incType}";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Error during deletion: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
