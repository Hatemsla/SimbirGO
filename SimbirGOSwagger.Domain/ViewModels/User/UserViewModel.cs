using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SimbirGOSwagger.Domain.ViewModels.User;

public class UserViewModel
{
    
    [Required]
    public string Username { get; set; }
    
    [Required]
    public string Password { get; set; }
}