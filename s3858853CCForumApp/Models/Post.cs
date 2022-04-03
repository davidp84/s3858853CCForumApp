using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace s3858853CCForumApp.Models
{
    public class Post
    {
        [Column(TypeName = "nchar")]
        [Required, StringLength(64)]
        public string subject { get; set; }

        public string UserID { get; set; }

        [Column(TypeName = "nchar")]
        [Required]
        public string messageText { get; set; }

        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss:fff }", ApplyFormatInEditMode = true)]
        public string postTimeUTC { get; set; }

        public string Image { get; set; }

    }
}
