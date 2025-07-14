using PayrollToyHRD.Models;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using System.Configuration;

namespace PayrollToyHRD.Services
{
    public class EmployeeService
    {
        //private readonly string _connectionString = "Server=YASIN;Database=IDCardGenerator;Trusted_Connection=True;";
        private readonly string _connectionString;
        private readonly string _webRootPath;

        public EmployeeService(string webRootPath, IConfiguration configuration)
        {
            _webRootPath = webRootPath;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Employee> GetEmployees()
        {
            var employees = new List<Employee>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                //var query = "SELECT EmpCode, EmpCodeB, EmpName, EmpNameB, dtJoin, BloodGroup, SectName, EmpPerAdd, VoterNo, EmpPicLocation, DesigNameB, EmpMobile FROM EmployeeData";
                var query = "SELECT EmpCode, EmpCodeB, EmpName, EmpNameB, dtJoin, BloodGroup, SectName, EmpPerAdd, VoterNo, EmpPicLocation, DesigNameB, EmpMobile FROM EmployeeData ORDER BY EmpCode";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var empPicPath = Path.Combine(
                                "file:///",
                                _webRootPath,
                                "EmpPic",
                                reader["EmpPicLocation"].ToString()
                            ).Replace("\\", "/");

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
