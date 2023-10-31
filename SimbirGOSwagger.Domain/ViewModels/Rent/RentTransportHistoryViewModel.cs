using System.ComponentModel.DataAnnotations;

namespace SimbirGOSwagger.Domain.ViewModels.Rent;

public class RentTransportHistoryViewModel
{
    [Required]
    public string TransportType { get; set; }

    [Required]
    [StringLength(30)]
    public string Model { get; set; }

    [Required]
    [StringLength(30)]
    public string Color { get; set; }

    [Required]
    [StringLength(30)]
    public string Identifier { get; set; }
    
    public string RentType { get; set; }
    
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}