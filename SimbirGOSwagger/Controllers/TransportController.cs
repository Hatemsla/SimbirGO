using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimbirGOSwagger.Domain.ViewModels.Transport;
using SimbirGOSwagger.Service.Interfaces;

namespace SimbirGOSwagger.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class TransportController : ControllerBase
{
    private readonly ITransportService _transportService;

    public TransportController(ITransportService transportService)
    {
        _transportService = transportService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransport(int id)
    {
        var response = await _transportService.GetTransport(id);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(response.Data);

        return BadRequest(new { Message = response.Description });
    }

    [HttpPost, Authorize]
    public async Task<IActionResult> Create(TransportViewModel model)
    {
        var response = await _transportService.AddTransport(HttpContext.User.Identity.Name, model);

        if (ModelState.IsValid)
        {
            if (response.StatusCode == Domain.Enum.StatusCode.Ok)
                return Ok(new { Message = response.Data });
        }

        return BadRequest(new { Message = response.Description });
    }
    
    [HttpPut("{id}"), Authorize]
    public async Task<IActionResult> Update(int id, TransportViewModel model)
    {
        var response = await _transportService.Update(HttpContext.User.Identity.Name, id, model);

        if (ModelState.IsValid)
        {
            if (response.StatusCode == Domain.Enum.StatusCode.Ok)
                return Ok(new { Message = response.Data });
        }

        return BadRequest(new { Message = response.Description });
    }

    [HttpDelete("{id}"), Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _transportService.Delete(HttpContext.User.Identity.Name, id);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            return Ok(new { Message = response.Data });

        return BadRequest(new { Response = response.Description });
    }
}