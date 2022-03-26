using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace s3858853CCForumApp.Models
{
    public enum Locked
    {
        unlocked = 0,
        locked = 1
    }

    public record Login
    {
        [Column(TypeName = "nchar")]
        [Required, StringLength(64)]
        public string subject { get; init; }

        public virtual User User { get; init; }

        [Column(TypeName = "nchar")]
        [Required]
        public string messageText { get; init; }

        public DateTime postTimeUTC { get; init; }

        public string Image { get; init; } //Maybe save objects???

    }
}