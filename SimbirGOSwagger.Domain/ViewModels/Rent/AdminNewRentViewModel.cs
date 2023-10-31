using System.ComponentModel.DataAnnotations;

namespace SimbirGOSwagger.Domain.ViewModels.Rent;

public class AdminNewRentViewModel
{
    [Required]
    public long TransportId { get; set; }
    
    [Required]
    public long UserId { get; set; }
    
    [Required]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss}")]
    public DateTime StartDate { get; set; }
    
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm:ss}")]
    public DateTime? EndDate { get; set; }
    
    [Required]
    public double PriceOfUnit { get; set; }
    
    [Required]
    public string PriceType { get; set; }
    
    public double? FinalPrice { get; set; }
}