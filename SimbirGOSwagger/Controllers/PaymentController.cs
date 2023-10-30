using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimbirGOSwagger.Service.Interfaces;

namespace SimbirGOSwagger.Controllers;

[ApiController]
[Route("/api/[controller]/Hesoyam")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("{accountId}")]
    public async Task<IActionResult> AddBalance(int accountId)
    {
        var userName = HttpContext.User.Identity?.Name;
        if (userName != null)
        {
            var response = await _paymentService.AddBalance(accountId, userName);

            if(response.StatusCode == Domain.Enum.StatusCode.Ok)
                return Ok(new { Message = response.Description });
        }

        return BadRequest();
    }
}