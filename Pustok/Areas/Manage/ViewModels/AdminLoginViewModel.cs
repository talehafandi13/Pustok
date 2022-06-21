using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.ViewModels
{
    public class AdminLoginViewModel
    {
        [Required]
        [MaxLength(25)]
        [MinLength(4)]
        public string Username { get; set; }
        [Required]
        [MaxLength(15)]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
