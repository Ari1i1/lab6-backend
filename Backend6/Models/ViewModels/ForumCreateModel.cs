using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend6.Models.ViewModels
{
    public class ForumCreateModel
    {
        [Required]
        [MaxLength(200)]
        public String Name { get; set; }

        [Required]
        public String Description { get; set; }
    }
}
