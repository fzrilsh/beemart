using System.Globalization;
using System.Security.Claims;
using BeeMart.Api.Dtos;
using BeeMart.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeeMart.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/history")]
public class HistoryController : ControllerBase
{
    private readonly HistoryService _history;

    public HistoryController(HistoryService history)
    {
        _history = history;
    }

    [HttpGet]
    public async Task<ActionResult<HistoryResponse>> Get([FromQuery] string? date, [FromQuery] int page, CancellationToken cancellationToken)
    {
        if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            throw new ValidationException("Format tanggal tidak valid.");
        }

        var idUser = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? string.Empty;
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "Cashier";
        var response = await _history.GetAsync(idUser, role, parsed, page, cancellationToken);
        return Ok(response);
    }
}
