using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollToyHRD.Data;
using PayrollToyHRD.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Globalization;

namespace PayrollToyHRD.Controllers
{
    public class SettlementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public SettlementController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Settlement/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Settlement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeSettlement model)
        {
            if (ModelState.IsValid)
            {
                _context.EmployeeSettlements.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Settlement/Edit/5
        public async Task<IActionResult> Edit(int empCode)
        {
            var settlement = await _context.EmployeeSettlements.FindAsync(empCode);
            if (settlement == null)
            {
                return NotFound();
            }
            return View(settlement);
        }

        // POST: Settlement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string empCode, EmployeeSettlement model)
        {
            if (empCode != model.EmpCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }


        // GET: Settlement/Delete/5
        public async Task<IActionResult> Delete(int empCode)
        {
            var settlement = await _context.EmployeeSettlements.FindAsync(empCode);
            if (settlement == null)
            {
                return NotFound();
            }

            return View(settlement);
        }

        // POST: Settlement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int empCode)
        {
            var settlement = await _context.EmployeeSettlements.FindAsync(empCode);
            _context.EmployeeSettlements.Remove(settlement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Settlement/Index
        public async Task<IActionResult> Index()
        {
            var data = await _context.EmployeeSettlements.ToListAsync();
            return View(data);
        }

        // GET: Settlement/PreviewReport
        public IActionResult PreviewReport(int empCode)
        {
            return RedirectToAction("RDLCReport", "ReportViewer", new { empCode });
        }







        ////////////////////////////////////////////
        ///



        public IActionResult UploadExcel()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ViewBag.Message = "No file selected.";
                return View();
            }

            var dataTable = new DataTable();
            dataTable.Columns.AddRange(new[]
            {
        new DataColumn("EmpCode"),
        new DataColumn("EmpName"),
        new DataColumn("DesigName"),
        new DataColumn("DeptName"),
        new DataColumn("LineName"),
        new DataColumn("dtJoin", typeof(DateTime)) { AllowDBNull = true },
        new DataColumn("dtReleased", typeof(DateTime)) { AllowDBNull = true },
        new DataColumn("ServiceLength", typeof(decimal)) { AllowDBNull = true },
        new DataColumn("GS", typeof(decimal)) { AllowDBNull = true },
        new DataColumn("BS", typeof(decimal)) { AllowDBNull = true },
        new DataColumn("MA", typeof(decimal)) { AllowDBNull = true },
        new DataColumn("HR", typeof(decimal)) { AllowDBNull = true },
        new DataColumn("PayableSalary", typeof(decimal)) { AllowDBNull = true },
        new DataColumn("ServiceBenefits", typeof(decimal)) { AllowDBNull = true },
        new DataColumn("NoticePeriodBenefit", typeof(decimal)) { AllowDBNull = true },
        new DataColumn("SalaryDeduction", typeof(decimal)) { AllowDBNull = true },
        new DataColumn("ExtraOtAmount", typeof(decimal)) { AllowDBNull = true },
        new DataColumn("ELYear1", typeof(int)) { AllowDBNull = true },
        new DataColumn("ELYear2", typeof(int)) { AllowDBNull = true },
        new DataColumn("PresentDayY1", typeof(int)) { AllowDBNull = true },
        new DataColumn("PresentDayY2", typeof(int)) { AllowDBNull = true },
        new DataColumn("TotalPresentDays", typeof(int)) { AllowDBNull = true },
        new DataColumn("ElDays", typeof(decimal)) { AllowDBNull = true },
        new DataColumn("NetPayableAmount", typeof(decimal)) { AllowDBNull = true },
        new DataColumn("IsLefty", typeof(bool)) { AllowDBNull = true },
        new DataColumn("IsResigned", typeof(bool)) { AllowDBNull = true },
        new DataColumn("dtResignSubmit", typeof(DateTime)) { AllowDBNull = true },
        new DataColumn("ProcessType"),
        new DataColumn("ProssType")
    });

            try
            {
                using (var stream = excelFile.OpenReadStream())
                {
                    IWorkbook workbook = new XSSFWorkbook(stream);
                    ISheet sheet = workbook.GetSheetAt(0);

                    for (int row = 1; row <= sheet.LastRowNum; row++)
                    {
                        IRow currentRow = sheet.GetRow(row);
                        if (currentRow == null) continue;

                        try
                        {
                            var dr = dataTable.NewRow();

                            dr["EmpCode"] = currentRow.GetCell(0)?.ToString()?.Trim();
                            dr["EmpName"] = currentRow.GetCell(1)?.ToString()?.Trim();
                            dr["DesigName"] = currentRow.GetCell(2)?.ToString()?.Trim();
                            dr["DeptName"] = currentRow.GetCell(3)?.ToString()?.Trim();
                            dr["LineName"] = currentRow.GetCell(4)?.ToString()?.Trim();

                            dr["dtJoin"] = TryParseDate(currentRow.GetCell(5));
                            dr["dtReleased"] = TryParseDate(currentRow.GetCell(6));
                            dr["ServiceLength"] = TryParseDecimal(currentRow.GetCell(7));
                            dr["GS"] = TryParseDecimal(currentRow.GetCell(8));
                            dr["BS"] = TryParseDecimal(currentRow.GetCell(9));
                            dr["MA"] = TryParseDecimal(currentRow.GetCell(10));
                            dr["HR"] = TryParseDecimal(currentRow.GetCell(11));
                            dr["PayableSalary"] = TryParseDecimal(currentRow.GetCell(12));
                            dr["ServiceBenefits"] = TryParseDecimal(currentRow.GetCell(13));
                            dr["NoticePeriodBenefit"] = TryParseDecimal(currentRow.GetCell(14));
                            dr["SalaryDeduction"] = TryParseDecimal(currentRow.GetCell(15));
                            dr["ExtraOtAmount"] = TryParseDecimal(currentRow.GetCell(16));
                            dr["ELYear1"] = TryParseInt(currentRow.GetCell(17));
                            dr["ELYear2"] = TryParseInt(currentRow.GetCell(18));
                            dr["PresentDayY1"] = TryParseInt(currentRow.GetCell(19));
                            dr["PresentDayY2"] = TryParseInt(currentRow.GetCell(20));
                            dr["TotalPresentDays"] = TryParseInt(currentRow.GetCell(21));
                            dr["ElDays"] = TryParseDecimal(currentRow.GetCell(22));
                            dr["NetPayableAmount"] = TryParseDecimal(currentRow.GetCell(23));
                            dr["IsLefty"] = currentRow.GetCell(24)?.ToString() == "1";
                            dr["IsResigned"] = currentRow.GetCell(25)?.ToString() == "1";
                            dr["dtResignSubmit"] = TryParseDate(currentRow.GetCell(26));
                            dr["ProcessType"] = currentRow.GetCell(27)?.ToString()?.Trim();
                            dr["ProssType"] = currentRow.GetCell(28)?.ToString()?.Trim();

                            dataTable.Rows.Add(dr);
                        }
                        catch (Exception rowEx)
                        {
                            ViewBag.Message = $"Error at row {row + 1}: {rowEx.Message}";
                            return View();
                        }
                    }
                }

                var connStr = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                    {
                        bulkCopy.DestinationTableName = "tblEmployeeSettlement";
                        await conn.OpenAsync();
                        await bulkCopy.WriteToServerAsync(dataTable);
                    }
                }

                ViewBag.Message = "Excel uploaded and processed successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error: " + ex.Message;
            }

            return View();
        }


        private object TryParseDate(ICell cell)
        {
            if (cell == null || cell.CellType == CellType.Blank) return DBNull.Value;

            if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                return cell.DateCellValue;

            if (DateTime.TryParse(cell.ToString(), out var dateValue))
                return dateValue;

            return DBNull.Value;
        }

        private object TryParseDecimal(ICell cell)
        {
            if (cell == null || cell.CellType == CellType.Blank) return DBNull.Value;

            if (decimal.TryParse(cell.ToString(), out var decimalValue))
                return decimalValue;

            return DBNull.Value;
        }

        private object TryParseInt(ICell cell)
        {
            if (cell == null || cell.CellType == CellType.Blank) return DBNull.Value;

            if (int.TryParse(cell.ToString(), out var intValue))
                return intValue;

            return DBNull.Value;
        }


    }

}
