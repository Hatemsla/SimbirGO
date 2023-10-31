using System.ComponentModel.DataAnnotations;

namespace SimbirGOSwagger.Domain.ViewModels.Rent;

public class RentTransportViewModel
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

    [StringLength(120)]
    public string Description { get; set; }

    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }

    [Range(0, double.MaxValue)]
    public double? MinutePrice { get; set; }

    [Range(0, double.MaxValue)]
    public double? DayPrice { get; set; }
}