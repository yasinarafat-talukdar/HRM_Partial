using Microsoft.EntityFrameworkCore;

namespace PayrollToyHRD.Models
{
    [Keyless]
    public class LayOffSalaryAdvice
    {
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string SectName { get; set; }
        public string BankAcNo { get; set; }
        public decimal NetPayable { get; set; }
    }
}
