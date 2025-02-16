using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Out_Source_Data.Models
{
    [Table("SinhVien")] // Map to the existing table
    public class Student
    {
        [Key]
        [Required]
        [Column("MASV")]
        public string MASV { get; set; } = string.Empty;
        [Required]
        [Column("Hoten")]
        public string Name { get; set; } = string.Empty;
        [Required]
        [Column("NgaySinh")]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [Column("GioiTinh")]
        public string Gender { get; set; } = string.Empty;
        [Required]
        [Column("TenLop")]
        public string ClassName { get; set; } = string.Empty;
    }
}
