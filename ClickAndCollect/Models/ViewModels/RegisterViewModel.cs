using System.ComponentModel.DataAnnotations;

namespace ClickAndCollect.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(64, MinimumLength = 12, ErrorMessage = "Password must be between 12 and 64 characters.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$", ErrorMessage = "Password must contain at least one letter and one number.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Display(Name = "First name")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "First name must be 3-255 characters.")]
        [RegularExpression(@"^\D+$", ErrorMessage = "First name must not contain digits.")]
        public string Firstname { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last name")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Last name must be 3-255 characters.")]
        [RegularExpression(@"^\D+$", ErrorMessage = "Last name must not contain digits.")]
        public string Lastname { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Phone number")]
        public string Phonenumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Postal code")]
        public int Postalcode { get; set; }

        [Required]
        [Display(Name = "City")]
        public string Cityname { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Street")]
        public string Streetname { get; set; } = string.Empty;

        [Required]
        [Display(Name = "House number")]
        public int Housenumber { get; set; }
    }
}
