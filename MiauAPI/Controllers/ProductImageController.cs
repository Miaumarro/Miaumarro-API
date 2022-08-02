using MiauAPI.Common;
using MiauAPI.Models.QueryObjects;
using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Services;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using System.Text.Json;

namespace MiauAPI.Controllers;

[ApiController]
[Route(ApiConstants.MainEndpoint)]
public sealed class ProductImageController : ControllerBase
{
    private readonly ProductImageService _service;

    public ProductImageController(ProductImageService service)
        => _service = service;

    [HttpPost("create")]
    [ProducesResponseType(typeof(ProductImageResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<ProductImageResponse, ErrorResponse>>> RegisterAsync([FromBody] ProductImageRequest productImage)
        => await _service.CreatedProductImageAsync(productImage, base.Request.Path.Value!);

}