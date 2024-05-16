using System.ComponentModel.DataAnnotations;

namespace CourseWork_WebStore.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string ?Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string ?Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ?ConfirmPassword { get; set; }

        [Required]
        public string ?Username { get; set; }
        public string ?FirstName { get; set; }
        public string ?LastName { get; set; }

        [Required]
        public string ?Role { get; set; }
    }
}
