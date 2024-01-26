#pragma warning disable CS8618
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using CSharpExamArmandoP.Models;

namespace CSharpExamArmandoP.Models;

public class Hobby {
    [Key]
    public int HobbyId { get; set; }
    public int? UserId { get; set; }

    [Required(ErrorMessage = "Hobby name is required")]
    public string HobbyName { get; set; }

    public bool IsUnique(IEnumerable<Hobby> existingHobbies, int? currentHobbyId)
    {
        return !existingHobbies.Any(h => h.HobbyName == HobbyName && h.HobbyId != currentHobbyId);
    }

    [Required(ErrorMessage = "Hobby description is required")]
    public string HobbyDescription { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public User? Creator { get; set; }
    public List<HobbyEnthusiast>? Enthusiasts { get; set; }
}