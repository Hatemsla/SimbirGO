using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SimbirGOSwagger.Domain.ViewModels.User;

public class UserDetailViewModel
{
    
    [Required]
    public string Username { get; set; }
    
    [Required]
    public string Password { get; set; }

    public double Balance { get; set; } = 0;
}