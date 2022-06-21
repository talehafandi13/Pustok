using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels
{
    public class MemberLoginViewModel
    {
        [Required]
        [MaxLength(25)]
        [MinLength(4)]
        public string LoginUsername { get; set; }
        [Required]
        [MaxLength(25)]
        [MinLength(4)]
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }
    }
}
