
namespace SimbirGOSwagger.Domain.Entity;

public class Rent
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long TransportId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int RentId { get; set; }
    public double PriceOfUnit { get; set; }
    public double? FinalPrice { get; set; }
}