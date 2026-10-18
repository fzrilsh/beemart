using BeeMart.Api.Dtos;
using BeeMart.Api.Repositories;
using BeeMart.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeeMart.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductRepository _products;

    public ProductsController(ProductRepository products)
    {
        _products = products;
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<ProductDto>>> Search([FromQuery] string? keyword, [FromQuery] int limit, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            throw new ValidationException("Keyword wajib diisi.");
        }

        var take = limit <= 0 || limit > 10 ? 10 : limit;
        var products = await _products.SearchAsync(keyword.Trim(), take, cancellationToken);
        var result = products
            .Select(x => new ProductDto(x.IdProduct, x.Name, x.Weight, x.Stock, x.Price, x.Discount))
            .ToList();
        return Ok(result);
    }
}
