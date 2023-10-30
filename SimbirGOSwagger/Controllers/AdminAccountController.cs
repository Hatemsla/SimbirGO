using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimbirGOSwagger.Domain.ViewModels.User;
using SimbirGOSwagger.Service.Interfaces;

namespace SimbirGOSwagger.Controllers;

[ApiController]
[Route("/api/Admin/Account")]
[Authorize(Roles = "Admin")]
public class AdminAccountController : ControllerBase
{
    private readonly IAdminUserService _adminUserService;

    public AdminAccountController(IAdminUserService adminUserService)
    {
        _adminUserService = adminUserService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAccounts(int start, int count)
    {
        var response = await _adminUserService.GetUsers(start, count);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
        {
            return Ok(response.Data);
        }

        return BadRequest(new { Response = response.Description });
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccount(int id)
    {
        var response = await _adminUserService.GetUser(id);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
        {
            return Ok(response.Data);
        }

        return BadRequest(new { Response = response.Description });
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(AdminUserViewModel model)
    {
        var response = await _adminUserService.Create(model);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
        {
            return Ok(new { Message = response.Data });
        }

        return BadRequest(new { Response = response.Description });
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, AdminUserViewModel model)
    {
        var response = await _adminUserService.Update(id, model);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
        {
            return Ok(new { Message = response.Data });
        }

        return BadRequest(new { Response = response.Description });
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _adminUserService.Delete(id);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
        {
            return Ok(new { Message = response.Data });
        }

        return BadRequest(new { Response = response.Description });
    }
}