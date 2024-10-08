using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSecureCoding1.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string? Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public bool RememberLogin { get; set; }

        public string? ReturnUrl { get; set; } = null!;
        
    }
}