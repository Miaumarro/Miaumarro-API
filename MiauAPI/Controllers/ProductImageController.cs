using MiauAPI.Common;
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
public sealed class ProductImageController : ControllerBase
{
    private readonly ProductImageService _service;

    public ProductImageController(ProductImageService service)
        => _service = service;

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GetProductImageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<GetProductImageResponse, None>>> GetAsync([FromQuery] ProductImageParameters productImageParameters)
        => await _service.GetProductImagesAsync(productImageParameters);

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedProductImageResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<CreatedProductImageResponse, None>>> RegisterAsync([FromBody] CreatedProductImageRequest productImage)
        => await _service.CreatedProductImageAsync(productImage, base.Request.Path.Value!);

    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteByIdAsync([FromQuery] int id)
        => await _service.DeleteProductImageByIdAsync(id);
}