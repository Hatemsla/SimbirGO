using System.ComponentModel.DataAnnotations;

namespace SimbirGOSwagger.Domain.ViewModels.User;

public class AdminUserViewModel
{
    [Required]
    public string Username { get; set; }
    
    [Required]
    public string Password { get; set; }

    public bool IsAdmin { get; set; }
    
    public double Balance { get; set; } = 0;
}