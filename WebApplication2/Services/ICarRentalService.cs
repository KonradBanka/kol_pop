using WebApplication2.DTOs;

namespace WebApplication2.Services;

public interface ICarRentalService
{
    Task<NewClientDto> GetClientWithRentalsAsync(int clientId);
    Task<bool> CreateRentalAsync(NewClientRentalRequest request);
}