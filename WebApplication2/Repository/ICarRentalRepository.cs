using WebApplication2.DTOs;

namespace WebApplication2.Repository;

public interface ICarRentalRepository
{
    Task<NewClientDto> GetClientWithRentalsAsync(int clientId);
    Task<bool> CarExistsAsync(int carId);
    Task<int> AddClientAsync(Client client);
    Task AddRentalAsync(int clientId, int carId, DateTime dateFrom, DateTime dateTo, int totalPrice);
    Task<int> GetPricePerDayAsync(int carId);
}
