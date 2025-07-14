using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollToyHRD.Models
{
    [Keyless]
    [Table("EmployeeSettlement")]
    public class EmployeeSettlement
    {
        public string EmpCode { get; set; }
        public string EmpCodeB { get; set; }
        public string EmpNameB { get; set; }
        public string DesigNameB { get; set; }
        public string SectNameB { get; set; }
        public string LineNameB { get; set; }
        public DateTime? dtJoin { get; set; }
        public DateTime? dtReleased { get; set; }
        public DateTime? dtResigned { get; set; }
        public string ServiceLength { get; set; }
        public int? PresentDayY1 { get; set; }
        public int? PresentDayY2 { get; set; }
        public int? ELYear1 { get; set; }
        public int? ELYear2 { get; set; }
        public int? ServBenefitDays { get; set; }
        public float? Stamp { get; set; }
        public decimal? GS { get; set; }
        public decimal? BS { get; set; }
        public decimal? HR { get; set; }
        public decimal? MA { get; set; }
        public decimal? LastMonthSalary { get; set; }
        public decimal? NoticePeriodDeduct { get; set; }
        public decimal? ExOtAmount { get; set; }
        public bool? IsLefty { get; set; }
        public bool? IsResigned { get; set; }
        public string ProcessType { get; set; }
    }
}