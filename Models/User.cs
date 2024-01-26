#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CSharpExamArmandoP.Models;

public class User {
    [Key]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [MinLength(2, ErrorMessage = "Name must be at least 2 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Last Name is required.")]
    [MinLength(2, ErrorMessage = "Last Name must be at least 2 characters.")]
    public string LastName { get; set; }

    [Required]
    [StringLength(15, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 15 characters.")]    [UniqueUsername]
    public string Username { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    public string Password { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [NotMapped]
    [Required(ErrorMessage = "Confirm Password is required.")]
    [Compare("Password")]
    [DataType(DataType.Password)]
    public string PasswordConfirm { get; set; }

    public List<HobbyEnthusiast>? HobbyEnthusiasts { get; set; }
}