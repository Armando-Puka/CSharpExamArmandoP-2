using System.ComponentModel.DataAnnotations;
using CSharpExamArmandoP.Models;

public class UniqueUsernameAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
        if (value == null) {
            return new ValidationResult("Username is required.");
        }

        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));
        if (_context.Users.Any(e => e.Username == value.ToString())) {
            return new ValidationResult("This username is already registered.");
        } else {
            return ValidationResult.Success;
        }
    }
}