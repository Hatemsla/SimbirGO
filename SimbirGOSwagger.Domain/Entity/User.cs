using System.ComponentModel.DataAnnotations.Schema;

namespace SimbirGOSwagger.Domain.Entity;

[Table("Users")]
public class User
{
    public int Id { get; set; }
    
    public string Username { get; set; }
    
    public string Password { get; set; }
    public bool IsAdmin { get; set; }
    public double Balance { get; set; }
}