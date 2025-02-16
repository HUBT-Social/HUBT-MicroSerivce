using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Out_Source_Data.Models
{
    [Table("thoikhoabieu")]
    [PrimaryKey(nameof(ClassName), nameof(Day), nameof(Session))] // Khai báo composite key
    public class TimeTable
    {
        [Required]
        [Column("ClassName")]
        public string ClassName { get; set; } = string.Empty;

        [Required]
        [Column("Day")]
        public string Day { get; set; } = string.Empty;

        [Required]
        [Column("Session")]
        public string Session { get; set; } = string.Empty;

        [Required]
        [Column("Subject")]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [Column("Room")]
        public string Room { get; set; } = string.Empty;

        [Column("ZoomID")]
        public string? ZoomID { get; set; }
    }
}
