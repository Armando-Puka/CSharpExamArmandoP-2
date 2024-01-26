#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
namespace CSharpExamArmandoP.Models;

public class MyContext : DbContext 
{   

    public MyContext(DbContextOptions options) : base(options) { }      
    public DbSet<User> Users { get; set; }
    public DbSet<Hobby> Hobbies {get;set;}
    public DbSet<HobbyEnthusiast> HobbyEnthusiasts {get;set;} 
}