using MiauAPI.Common;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Services;
using Microsoft.AspNetCore.Mvc;
using OneOf;

namespace MiauAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductController : ControllerBase
{
    private readonly ProductService _service;

    public ProductController(ProductService service)
        => _service = service;

    
    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<CreatedProductResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedProductRequest product)
        => await _service.CreateProductAsync(product, base.Request.Path.Value!);
}