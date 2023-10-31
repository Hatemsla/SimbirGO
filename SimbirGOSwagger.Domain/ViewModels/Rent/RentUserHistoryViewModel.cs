using System.ComponentModel.DataAnnotations;

namespace SimbirGOSwagger.Domain.ViewModels.Rent;

public class RentUserHistoryViewModel
{
    [Required]
    public string Username { get; set; }
    
    public string RentType { get; set; }
    
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}