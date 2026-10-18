using System.Security.Claims;
using BeeMart.Api.Dtos;
using BeeMart.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeeMart.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly TransactionService _transactions;

    public TransactionsController(TransactionService transactions)
    {
        _transactions = transactions;
    }

    [HttpPost("calculate")]
    public async Task<ActionResult<CalculateResponse>> Calculate(CalculateRequest request, CancellationToken cancellationToken)
    {
        var response = await _transactions.CalculateAsync(request.Items, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<SubmitTransactionResponse>> Submit(SubmitTransactionRequest request, CancellationToken cancellationToken)
    {
        var idUser = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? string.Empty;
        var response = await _transactions.SubmitAsync(idUser, request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }
}
