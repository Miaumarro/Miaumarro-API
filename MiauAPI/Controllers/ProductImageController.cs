using MiauAPI.Common;
using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Services;
using MiauDatabase.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using System.Text.Json;

namespace MiauAPI.Controllers;

[ApiController]
[Authorize(Roles = $"{nameof(UserPermissions.Administrator)},{nameof(UserPermissions.Clerk)}")]
[Route(ApiConstants.MainEndpoint)]
public sealed class ProductImageController : ControllerBase
{
    private readonly ProductImageService _service;

    public ProductImageController(ProductImageService service)
        => _service = service;

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetProductImageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<GetProductImageResponse, ErrorResponse>>> GetAsync([FromQuery] ProductImageParameters productImageParameters)
    {
        var productImagesPaged = await _service.GetProductImageAsync(productImageParameters);

        if (productImagesPaged.Result is OkObjectResult response && response.Value is GetProductImageResponse productImageResponse)
        {
            var metadata = new
            {
                productImageResponse.ProductImages.TotalCount,
                productImageResponse.ProductImages.PageSize,
                productImageResponse.ProductImages.CurrentPage,
                productImageResponse.ProductImages.TotalPages,
                productImageResponse.ProductImages.HasNext,
                productImageResponse.ProductImages.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
        }

        return productImagesPaged;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedProductImageResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<CreatedProductImageResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedProductImageRequest productImage)
        => await _service.CreatedProductImageAsync(productImage, base.Request.Path.Value!);

    [HttpDelete("delete")]
    [ProducesResponseType(typeof(DeleteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<DeleteResponse, ErrorResponse>>> DeleteByIdAsync([FromQuery] int id)
        => await _service.DeleteProductImageByIdAsync(id);
}