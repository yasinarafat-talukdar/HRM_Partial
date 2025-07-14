using Microsoft.EntityFrameworkCore;
using PayrollToyHRD.Models;
using System.Collections.Generic;

namespace PayrollToyHRD.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeSettlement> EmployeeSettlements { get; set; }
        public DbSet<IncrementLetter> IncrementLetters { get; set; }
        public DbSet<LayOffSalaryAdvice> LayOffSalaryAdvices { get; set; }
        public DbSet<tblAttendanceTransport> tblAttendanceTransports { get; set; }
        public DbSet<tblTransportAllowanceRate> tblTransportAllowanceRates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<EmployeeSettlement>().HasNoKey();
            modelBuilder.Entity<EmployeeSettlement>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("EmployeeSettlement"); // <-- Your actual table name here
            });

            modelBuilder.Entity<IncrementLetter>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("tblEmployeeIncrementLetter"); // <-- Your actual table name here
            });
            modelBuilder.Entity<LayOffSalaryAdvice>().HasNoKey();
            modelBuilder.Entity<tblAttendanceTransport>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("tblAttendanceTransport");
            });

            modelBuilder.Entity<tblTransportAllowanceRate>().HasNoKey();
        }
    }


}
