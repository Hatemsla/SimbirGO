using System.ComponentModel.DataAnnotations.Schema;

namespace SimbirGOSwagger.Domain.Entity;

[Table("Transport")]
public class Transport
{
    public long Id { get; set; }
    public long Owner { get; set; }

    public bool CanBeRented { get; set; }

    public int TransportType { get; set; }

    public string Model { get; set; }

    public string Color { get; set; }

    public string Identifier { get; set; }

    public string Description { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public double MinutePrice { get; set; }

    public double DayPrice { get; set; }
}