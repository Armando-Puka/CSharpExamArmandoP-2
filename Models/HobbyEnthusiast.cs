#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CSharpExamArmandoP.Models;

namespace CSharpExamArmandoP.Models;

public class HobbyEnthusiast {
    [Key]
    public int HobbyEnthusiastId { get; set; }
    public string Proficiency { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public int? UserId { get; set; }
    public User? User { get; set; }
    public int? HobbyId { get; set; }
    public Hobby? Hobbies { get; set; }
}