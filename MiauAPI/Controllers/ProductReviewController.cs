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
public sealed class ProductReviewController : ControllerBase
{
    private readonly ProductReviewService _service;

    public ProductReviewController(ProductReviewService service)
        => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ProductReviewObject[]>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<PagedResponse<ProductReviewObject[]>, None>>> GetAsync([FromQuery] ProductReviewParameters productReviewParameters)
    {
        var actionResult = await _service.GetProductReviewsAsync(productReviewParameters);

        if (actionResult.Result.TryUnwrap<PagedResponse>(out var response))
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

        return actionResult;
    }

    [HttpGet("details")]
    [ProducesResponseType(typeof(ProductReviewObject), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<ProductReviewObject, None>>> GetByIdAsync([FromQuery] GetProductReviewRequest request)
        => await _service.GetProductReviewByIdAsync(request);

    [HttpPost("create")]
    [Authorize(Roles = $"{nameof(UserPermissions.Customer)}")]
    [ProducesResponseType(typeof(CreatedProductReviewResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<CreatedProductReviewResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedProductReviewRequest productReview)
        => await _service.CreateProductReviewAsync(productReview, base.Request.Path.Value!);

    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteByIdAsync([FromQuery] int id)
        => await _service.DeleteProductReviewByIdAsync(id);

    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateByIdAsync([FromBody] UpdateProductReviewRequest productReview)
        => await _service.UpdateProductReviewByIdAsync(productReview);
}