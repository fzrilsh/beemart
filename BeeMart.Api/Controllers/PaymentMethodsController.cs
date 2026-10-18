using BeeMart.Api.Dtos;
using BeeMart.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeeMart.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/payment-methods")]
public class PaymentMethodsController : ControllerBase
{
    private readonly ProductRepository _products;

    public PaymentMethodsController(ProductRepository products)
    {
        _products = products;
    }

    [HttpGet]
    public async Task<ActionResult<List<PaymentMethodDto>>> Get(CancellationToken cancellationToken)
    {
        var methods = await _products.GetPaymentMethodsAsync(cancellationToken);
        var result = methods
            .Select(x => new PaymentMethodDto(x.IdPaymentMethod, x.MethodName))
            .ToList();
        return Ok(result);
    }
}
