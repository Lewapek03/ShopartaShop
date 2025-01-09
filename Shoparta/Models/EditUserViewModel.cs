using System.ComponentModel.DataAnnotations;

namespace Shoparta.ViewModels
{
    public class EditUserViewModel : IValidatableObject
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(NewPassword))
            {
                if (string.IsNullOrEmpty(ConfirmPassword))
                {
                    yield return new ValidationResult("The 'Confirm new password' field is required when you enter a new password.", new[] { "ConfirmPassword" });
                }
                else if (NewPassword != ConfirmPassword)
                {
                    yield return new ValidationResult("Password must be the same.", new[] { "ConfirmPassword" });
                }
            }
        }
    }
}