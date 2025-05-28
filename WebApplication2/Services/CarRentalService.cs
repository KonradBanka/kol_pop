using WebApplication2.DTOs;
using WebApplication2.Exceptions;
using WebApplication2.Repository;
using WebApplication2.Services;

public class CarRentalService : ICarRentalService
{
    private readonly ICarRentalRepository _repository;

    public CarRentalService(ICarRentalRepository repository, IConfiguration configuration)
    {
        _repository = repository;
    }

    public async Task<NewClientDto> GetClientWithRentalsAsync(int clientId)
    {
            var client = await _repository.GetClientWithRentalsAsync(clientId);
            return client;
        
    }

    public async Task<bool> CreateRentalAsync(NewClientRentalRequest dto)
    {
        if (dto.DateFrom >= dto.DateTo)
            return false;

        if (!await _repository.CarExistsAsync(dto.CarId))
            throw new NotFoundException("Samochod nie istnieje");

        var days = (dto.DateTo - dto.DateFrom).Days;
        var pricePerDay = await _repository.GetPricePerDayAsync(dto.CarId);
        var totalPrice = days * pricePerDay;

        var clientId = await _repository.AddClientAsync(dto.Client);
        await _repository.AddRentalAsync(clientId, dto.CarId, dto.DateFrom, dto.DateTo, totalPrice);
        return true;
    }

   
}