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
        try
        {
            var client = await _service.GetClientWithRentalsAsync(clientId);
            return Ok(client);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddClientWithRental([FromBody] NewClientRentalRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _service.CreateRentalAsync(request);
        

        return Ok(result);
    }
}