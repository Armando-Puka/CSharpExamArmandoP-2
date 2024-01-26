#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace CSharpExamArmandoP.Models;

public class LoginUser {
    [Required(ErrorMessage = "Username is required.")]
    public string LoginUsername { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string LoginPassword { get; set; }
}