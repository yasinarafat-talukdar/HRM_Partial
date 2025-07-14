using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace PayrollToyHRD.Controllers
{
    public class BackupController : Controller
    {
        private readonly string? _connectionString;


        public BackupController(IConfiguration configuration)
        {

            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BackupDatabase()
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("prcDBBackup", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@dbId", SqlDbType.TinyInt).Value = 1;
                        cmd.Parameters.Add("@Date", SqlDbType.VarChar, 50).Value = "";
                        cmd.Parameters.Add("@Time", SqlDbType.VarChar, 50).Value = "";
                        cmd.Parameters.Add("@LUserId", SqlDbType.VarChar, 50).Value = "1";

                        // Replace with actual User ID

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                ViewBag.Message = "Database backup completed successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error: " + ex.Message;
            }

            return View("Index");
        }
    }
}
