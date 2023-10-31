using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimbirGOSwagger.Domain.ViewModels.Transport;
using SimbirGOSwagger.Service.Interfaces;

namespace SimbirGOSwagger.Controllers;

[ApiController]
[Route("/api/Admin/Transport")]
[Authorize(Roles = "Admin")]
public class AdminTransportController : ControllerBase
{
    private readonly IAdminTransportService _adminTransportService;

    public AdminTransportController(IAdminTransportService adminTransportService)
    {
        _adminTransportService = adminTransportService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTransports(int start, int count, string transportType)
    {
        var response = await _adminTransportService.GetTransports(start, count, transportType);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(response.Data);

        return BadRequest(new { Message = response.Description });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransport(int id)
    {
        var response = await _adminTransportService.GetTransport(id);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(response.Data);

        return BadRequest(new { Message = response.Description });
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransport(AdminTransportViewModel model)
    {
        var response = await _adminTransportService.AddTransport(model);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(new { Message = response.Data });
                
        return BadRequest(new { Message = response.Description });
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransport(int id, AdminTransportViewModel model)
    {
        var response = await _adminTransportService.Update(id, model);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(new { Message = response.Data });
                
        return BadRequest(new { Message = response.Description });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTransport(int id)
    {
        var response = await _adminTransportService.Delete(id);
        
        if(response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(new { Message = response.Data });
                
        return BadRequest(new { Message = response.Description });
    }
}