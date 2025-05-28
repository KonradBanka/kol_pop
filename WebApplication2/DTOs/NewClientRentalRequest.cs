namespace WebApplication2.DTOs;

public class NewClientRentalRequest
{
    public Client Client { get; set; }
    public int CarId { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}