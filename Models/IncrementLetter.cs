using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollToyHRD.Models
{
    [Keyless]
    public class IncrementLetter
    {
        public string EmpCode { get; set; }

        public string EmpCodeB { get; set; }
        public string SectNameB { get; set; }
        public string EmpNameB { get; set; }
        public string DesigNameB { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Old_Basic { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Inc_Basic { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? New_Basic { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? New_HouseRent { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? New_Gross { get; set; }

        public string ProcType { get; set; }
        public string IncType { get; set; }
    }
}
