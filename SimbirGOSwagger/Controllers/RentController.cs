using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimbirGOSwagger.Service.Interfaces;

namespace SimbirGOSwagger.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class RentController : ControllerBase
{
    private readonly IRentService _rentService;

    public RentController(IRentService rentService)
    {
        _rentService = rentService;
    }

    [HttpGet("Transport")]
    public async Task<IActionResult> GetRentTransport(double latitude, double longitude, double radius, string type)
    {
        var response = await _rentService.GetRentTransport(latitude, longitude, radius, type);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(response.Data);

        return BadRequest(new { Message = response.Description });
    }

    [HttpPost("[action]/{transportId}"), Authorize]
    public async Task<IActionResult> New(int transportId, string rentType)
    {
        var response = await _rentService.NewRent(HttpContext.User.Identity.Name, transportId, rentType);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(new { Message = response.Data });
        
        return BadRequest(new { Message = response.Description });
    }

    [HttpPost("[action]/{rentId}"), Authorize]
    public async Task<IActionResult> End(int rentId, double latitude, double longitude)
    {
        var response = await _rentService.EndRent(rentId, HttpContext.User.Identity.Name, latitude, longitude);
        
        if(response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(new { Message = response.Data });
        
        return BadRequest(new { Message = response.Description });
    }

    [HttpGet("[action]"), Authorize]
    public async Task<IActionResult> MyHistory()
    {
        var response = await _rentService.GetHistory(HttpContext.User.Identity.Name);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(response.Data);
        
        return BadRequest(new { Message = response.Description });
    }
    
    [HttpGet("[action]/{transportId}"), Authorize]
    public async Task<IActionResult> TransportHistory(int transportId)
    {
        var response = await _rentService.GetTransportHistory(transportId, HttpContext.User.Identity.Name);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(response.Data);
        
        return BadRequest(new { Message = response.Description });
    }
    
    [HttpGet("{id}"), Authorize]
    public async Task<IActionResult> GetRent(int id)
    {
        var response = await _rentService.GetRentInfo(HttpContext.User.Identity.Name, id);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(response.Data);
        
        return BadRequest(new { Message = response.Description });
    }
}