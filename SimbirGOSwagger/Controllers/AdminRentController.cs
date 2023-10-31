using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimbirGOSwagger.Domain.ViewModels.Rent;
using SimbirGOSwagger.Service.Interfaces;

namespace SimbirGOSwagger.Controllers;

[ApiController]
[Route("/api/Admin")]
[Authorize(Roles = "Admin")]
public class AdminRentController : ControllerBase
{
    private readonly IAdminRentService _adminRentService;

    public AdminRentController(IAdminRentService adminRentService)
    {
        _adminRentService = adminRentService;
    }

    [HttpGet("Rent/{transportId}")]
    public async Task<IActionResult> RentHistory(int transportId)
    {
        var response = await _adminRentService.GetRentHistory(transportId);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(response.Data);
        
        return BadRequest(new { Message = response.Description });
    }
    
    [HttpGet("[action]/{userId}")]
    public async Task<IActionResult> UserHistory(int userId)
    {
        var response = await _adminRentService.GetUserHistory(userId);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(response.Data);
        
        return BadRequest(new { Message = response.Description });
    }
    
    [HttpGet("[action]/{transportId}")]
    public async Task<IActionResult> TransportHistory(int transportId)
    {
        var response = await _adminRentService.GetTransportHistory(transportId);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(response.Data);
        
        return BadRequest(new { Message = response.Description });
    }
    
    [HttpPost("Rent")]
    public async Task<IActionResult> NewRent(AdminNewRentViewModel model)
    {
        var response = await _adminRentService.NewRent(model);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(response.Data);
        
        return BadRequest(new { Message = response.Description });
    }
    
    [HttpPost("Rent/End/{rentId}")]
    public async Task<IActionResult> EndRent(int rentId, double latitude, double longitude)
    {
        var response = await _adminRentService.EndRent(rentId, latitude, longitude);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(response.Data);
        
        return BadRequest(new { Message = response.Description });
    }
    
    [HttpPut("Rent/{id}")]
    public async Task<IActionResult> EndRent(int id, AdminNewRentViewModel model)
    {
        var response = await _adminRentService.EditRent(id, model);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(response.Data);
        
        return BadRequest(new { Message = response.Description });
    }
    
    [HttpDelete("Rent/{renId}")]
    public async Task<IActionResult> EndRent(int renId)
    {
        var response = await _adminRentService.DeleteRent(renId);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(response.Data);
        
        return BadRequest(new { Message = response.Description });
    }
}