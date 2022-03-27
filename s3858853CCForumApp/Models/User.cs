using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace s3858853CCForumApp.Models
{

    public class User
    {

        [Required, StringLength(50)]
        public string id { get; set; }

        [Required, StringLength(50)]
        public string user_name { get; set; }

        [Required, StringLength(50)]
        public string password { get; set; }

        public string image { get; set; }

    }
}
