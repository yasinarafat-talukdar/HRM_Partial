using PayrollToyHRD.Models;
using Microsoft.AspNetCore.Hosting;
using System.Data.SqlClient;

namespace PayrollToyHRD.Services
{
    public class FireSafetyServices
    {
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FireSafetyServices(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Employee> GetEmployees()
        {
            var employees = new List<Employee>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT EmpCode, EmpCodeB, EmpName, EmpNameB, dtJoin, BloodGroup, SectName, EmpPerAdd, VoterNo, EmpPicLocation, DesigNameB, EmpMobile FROM EmployeeData";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        string imageDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "EmpPic");

                        while (reader.Read())
                        {
                            var empPicPath = Path.Combine("file:///", imageDirectory, reader["EmpPicLocation"].ToString())
                                                    .Replace("\\", "/");

                            employees.Add(new Employee
                            {
                                EmpCode = reader["EmpCode"].ToString(),
                                EmpCodeB = reader["EmpCodeB"].ToString(),
                                EmpName = reader["EmpName"].ToString(),
                                EmpNameB = reader["EmpNameB"].ToString(),
                                dtJoin = Convert.ToDateTime(reader["dtJoin"]),
                                BloodGroup = reader["BloodGroup"].ToString(),
                                SectName = reader["SectName"].ToString(),
                                EmpPerAdd = reader["EmpPerAdd"].ToString(),
                                VoterNo = reader["VoterNo"].ToString(),
                                EmpPicLocation = empPicPath, // Updated path
                                DesigNameB = reader["DesigNameB"].ToString(),
                                EmpMobile = reader["EmpMobile"].ToString()
                            });

                            // Log for debugging
                            Console.WriteLine($"Constructed Path: {empPicPath}");
                        }
                    }
                }
            }

            return employees;
        }
    }
}
