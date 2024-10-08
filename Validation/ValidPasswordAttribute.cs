using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjectSecureCoding1.Validation
{
    public class ValidPasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult("Password is required.");

            string password = value.ToString().Trim();

            // Normalize multiple spaces to a single space
            string normalizedPassword = System.Text.RegularExpressions.Regex.Replace(password, @"\s+", " ");

            if (normalizedPassword.Length < 12)
            {
                return new ValidationResult("Password must be at least 12 characters long (after combining multiple spaces).");
            }

            // Check for at least one uppercase letter
            if (!Regex.IsMatch(normalizedPassword, "[A-Z]"))
            {
                return new ValidationResult("Password must contain at least one uppercase letter.");
            }

            if (!Regex.IsMatch(normalizedPassword, @"[!@#$%^&*(),.?""{}|<>]"))
            {
                return new ValidationResult("Password must contain at least one special character.");
            }

            return ValidationResult.Success;
        }
    }
}