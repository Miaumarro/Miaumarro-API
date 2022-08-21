using MiauAPI.Common;
using MiauAPI.Extensions;
using MiauAPI.Models.QueryObjects;
using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Services;
using MiauDatabase.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OneOf.Types;
using System.Text.Json;

namespace MiauAPI.Controllers;

[ApiController, Route(ApiConstants.MainEndpoint)]
[Authorize(Roles = $"{nameof(UserPermissions.Administrator)},{nameof(UserPermissions.Clerk)}")]
public sealed class ProductController : ControllerBase
{
    private readonly ProductService _service;

    public ProductController(ProductService service)
        => _service = service;

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResponse<ProductObject[]>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<PagedResponse<ProductObject[]>, ErrorResponse, None>>> GetAsync([FromQuery] ProductParameters productParameters)
    {
        var productsPaged = await _service.GetProductsAsync(productParameters);

        if (productsPaged.Result.TryUnwrap<PagedResponse>(out var response))
        {
            var metadata = new
            {
                response.PageNumber,
                response.PageSize,
                response.PreviousCount,
                response.NextCount,
                response.Amount
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
        }

        return productsPaged;
    }

    [HttpGet("detail")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProductObject), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<ProductObject, ErrorResponse>>> GetByIdAsync([FromQuery] int id)
        => await _service.GetProductByIdAsync(id);

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<CreatedProductResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedProductRequest product)
        => await _service.CreateProductAsync(product, base.Request.Path.Value!);

    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteByIdAsync([FromQuery] int id)
        => await _service.DeleteProductByIdAsync(id);

    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateByIdAsync([FromBody] UpdateProductRequest product)
        => await _service.UpdateProductByIdAsync(product);
}