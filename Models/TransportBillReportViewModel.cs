namespace PayrollToyHRD.Models
{
    public class TransportBillReportViewModel
    {
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string EmpDesig { get; set; }
        public string StoppageName { get; set; }
        public string BusName { get; set; }
        public int PresentDays { get; set; }
        public decimal AllowanceRate { get; set; }
        public decimal TotalBill { get; set; }
    }
}
