using System.Security.Claims;
using BeeMart.Api.Dtos;
using BeeMart.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeeMart.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/invoices")]
public class InvoicesController : ControllerBase
{
    private readonly InvoiceService _invoices;

    public InvoicesController(InvoiceService invoices)
    {
        _invoices = invoices;
    }

    [HttpPost("lock")]
    public async Task<ActionResult<InvoiceLockResponse>> Lock(CancellationToken cancellationToken)
    {
        var idUser = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? string.Empty;
        var response = await _invoices.EnsureLockAsync(idUser, cancellationToken);
        return Ok(response);
    }
}
