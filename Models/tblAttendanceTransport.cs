namespace PayrollToyHRD.Models
{
    public class tblAttendanceTransport
    {
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string EmpDesig { get; set; }
        public string StoppageName { get; set; }
        public string BusName { get; set; }
        public DateTime dtPresent { get; set; }

        // ✅ New properties
        public string Section { get; set; }
        public string Floor { get; set; }
    }
}
