using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels
{
    public class MemberRegisterViewModel
    {
        [Required]
        [MaxLength(25)]
        [MinLength(4)]
        public string Username { get; set; }
        [Required]
        [MaxLength(30)]
        [MinLength(6)]
        public string Fullname { get; set; }
        [Required]
        [MaxLength(25)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [MaxLength(25)]
        [MinLength(4)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [MaxLength(25)]
        [MinLength(4)]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
