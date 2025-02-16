using Microsoft.EntityFrameworkCore;
using Out_Source_Data.Models;

namespace Out_Source_Data.Datas
{
    public class StudentDB : DbContext
    {
        public StudentDB(DbContextOptions<StudentDB> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<TimeTable> TimeTables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map the Student entity to the existing table 'SinhVien'
            modelBuilder.Entity<Student>().ToTable("SinhVien");

            // Configure the TimeTable entity
            modelBuilder.Entity<TimeTable>().ToTable("thoikhoabieu");
        }
    }
}
