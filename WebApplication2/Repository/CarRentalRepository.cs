using Microsoft.Data.SqlClient;
using WebApplication2.DTOs;

namespace WebApplication2.Repository;

public class CarRentalRepository : ICarRentalRepository
{
    private readonly string _connectionString;

    public CarRentalRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<NewClientDto> GetClientWithRentalsAsync(int clientId)
    {
        var client = new NewClientDto { Rentals = new List<RentalDto>() };
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT c.ID, c.FirstName, c.LastName, c.Address,
                   r.VIN, co.Name AS Color, m.Name AS Model, cr.DateFrom, cr.DateTo, cr.TotalPrice
            FROM clients c
            LEFT JOIN car_rentals cr ON c.ID = cr.ClientID
            LEFT JOIN cars r ON cr.CarID = r.ID
            LEFT JOIN colors co ON r.ColorID = co.ID
            LEFT JOIN models m ON r.ModelID = m.ID
            WHERE c.ID = @ClientId";

        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@ClientId", clientId);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            if (client.Id == 0)
            {
                client.Id = reader.GetInt32(0);
                client.FirstName = reader.GetString(1);
                client.LastName = reader.GetString(2);
                client.Address = reader.GetString(3);
            }

            if (!reader.IsDBNull(4))
            {
                client.Rentals.Add(new RentalDto
                {
                    Vin = reader.GetString(4),
                    Color = reader.GetString(5),
                    Model = reader.GetString(6),
                    DateFrom = reader.GetDateTime(7),
                    DateTo = reader.GetDateTime(8),
                    TotalPrice = reader.GetInt32(9)
                });
            }
        }

        return client;
    }

    public async Task<bool> CarExistsAsync(int carId)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = "SELECT COUNT(*) FROM cars WHERE ID = @CarId";
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@CarId", carId);

        var count = (int)await command.ExecuteScalarAsync();
        return count > 0;
    }

    public async Task<int> AddClientAsync(Client client)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = "INSERT INTO clients (FirstName, LastName, Address) OUTPUT INSERTED.ID VALUES (@FirstName, @LastName, @Address)";
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@FirstName", client.FirstName);
        command.Parameters.AddWithValue("@LastName", client.LastName);
        command.Parameters.AddWithValue("@Address", client.Address);

        return (int)await command.ExecuteScalarAsync();
    }

    public async Task AddRentalAsync(int clientId, int carId, DateTime dateFrom, DateTime dateTo, int totalPrice)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = "INSERT INTO car_rentals (ClientID, CarID, DateFrom, DateTo, TotalPrice) VALUES (@ClientId, @CarId, @DateFrom, @DateTo, @TotalPrice)";
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@ClientId", clientId);
        command.Parameters.AddWithValue("@CarId", carId);
        command.Parameters.AddWithValue("@DateFrom", dateFrom);
        command.Parameters.AddWithValue("@DateTo", dateTo);
        command.Parameters.AddWithValue("@TotalPrice", totalPrice);

        await command.ExecuteNonQueryAsync();
    }
    
    public async Task<int> GetPricePerDayAsync(int carId)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = "SELECT PricePerDay FROM cars WHERE ID = @CarId";
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@CarId", carId);
        var ret = (int)await command.ExecuteScalarAsync();
        return ret;

    }
}