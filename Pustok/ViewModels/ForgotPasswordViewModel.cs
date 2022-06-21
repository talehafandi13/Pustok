using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
    }
}
