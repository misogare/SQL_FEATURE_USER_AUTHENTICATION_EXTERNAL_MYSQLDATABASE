using System.ComponentModel.DataAnnotations;

public class ApplicationUser
{
    public int Id { get; set; } // Unique identifier for the user

    [Required(ErrorMessage = "Username is required")]
    [MaxLength(50)]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one numeric digit, and one special character.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [MaxLength(100)]
    public string Email { get; set; }

    [Required(ErrorMessage = "Role is required")]
    [MaxLength(10)]
    public string Role { get; set; }
}
