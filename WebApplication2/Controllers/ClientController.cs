using Microsoft.AspNetCore.Mvc;
using WebApplication2.DTOs;
using WebApplication2.Services;

[Route("api/clients")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly ICarRentalService _service;

    public ClientsController(ICarRentalService service)
    {
        _service = service;
    }

    [HttpGet("{clientId}")]
    public async Task<IActionResult> GetClientWithRentals(int clientId)
    {
            var client = await _service.GetClientWithRentalsAsync(clientId);
            return Ok(client);
    }

    [HttpPost]
    public async Task<IActionResult> AddClientWithRental([FromBody] NewClientRentalRequest request)
    {
        var result = await _service.CreateRentalAsync(request);
        return Ok(result);
    }
}