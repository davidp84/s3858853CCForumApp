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
        [StringLength(8)]
        public string id { get; init; }

        public virtual User User { get; init; }

        [Column(TypeName = "nchar")]
        [Required, StringLength(64)]
        public string password { get; init; }

        public Locked Locked { get; init; }

    }
}