namespace PayrollToyHRD.Models
{
    public class EmployeeLeave
    {
        public int Id { get; set; }
        public string EmpCode { get; set; }
        public string EmployeeName { get; set; }
        public string SectionName { get; set; }
        public string DeptName { get; set; }
        public string LeaveType { get; set; }
        public DateTime? StartDate { get; set; }  // Made nullable to match the database schema
        public DateTime? EndDate { get; set; }    // Made nullable to match the database schema
        public string Reason { get; set; }
        public string DesigName { get; set; }
        public DateTime? dtJoin { get; set; }     // Made nullable to match the database schema
        public string BalanceCL { get; set; }
        public string BalanceSL { get; set; }
        public string EmpCodeB { get; set; }
    }
}
