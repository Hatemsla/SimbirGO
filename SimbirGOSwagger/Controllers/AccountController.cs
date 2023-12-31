﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimbirGOSwagger.Domain.ViewModels.User;
using SimbirGOSwagger.Service.Interfaces;

namespace SimbirGOSwagger.Controllers;

[ApiController]
[Route("/api/[controller]/[action]")]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(UserViewModel model)
    {
        var response = await _userService.SignUp(model);
        
        if (ModelState.IsValid)
        {
            if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Ok(new {Token = response.Data});
            }
        }

        return BadRequest(new { Response = response.Description });
    }
    
    [HttpPost]
    public async Task<IActionResult> SignIn(UserViewModel model)
    {
        var response = await _userService.SignIn(model);
        
        if (ModelState.IsValid)
        {
            if (response.StatusCode == Domain.Enum.StatusCode.Ok)
            {
                return Ok(new { Token = response.Data });
            }
        }

        return BadRequest(new { Response = response.Description });
    }

    [HttpPut, Authorize]
    public async Task<IActionResult> Update(UserViewModel model)
    {
        var response = await _userService.Update(model, HttpContext.User.Identity.Name);

        if (response.StatusCode == Domain.Enum.StatusCode.Ok)
        {
            return Ok(new { Result = "Данные пользователя обновлены" });
        }

        return BadRequest(response.Description);
    }
    
    [HttpPost, Authorize]
    public new IActionResult SignOut()
    {
        var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            return Ok(new { Message = "Выход из аккаунта выполнен" });
        }

        return BadRequest(new { Message = "Токен не найден" });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userName = HttpContext.User.Identity?.Name;
        if (userName != null)
        {
            var response = await _userService.GetUserByName(userName);

            if(response.StatusCode == Domain.Enum.StatusCode.Ok)
                return Ok(response.Data);
        }

        return BadRequest();
    }
}