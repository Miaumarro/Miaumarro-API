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
[Route(ApiConstants.MainEndpoint)]
public sealed class ProductReviewController : ControllerBase
{
    private readonly ProductReviewService _service;

    public ProductReviewController(ProductReviewService service)
        => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(GetProductReviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<GetProductReviewResponse, ErrorResponse>>> GetAsync([FromQuery] ProductReviewParameters productReviewParameters)
    {
        var productReviewPaged = await _service.GetProductReviewAsync(productReviewParameters);

        if (productReviewPaged.Result is OkObjectResult response && response.Value is GetProductReviewResponse productReviewResponse)
        {
            var metadata = new
            {
                productReviewResponse.ProductReviews.TotalCount,
                productReviewResponse.ProductReviews.PageSize,
                productReviewResponse.ProductReviews.CurrentPage,
                productReviewResponse.ProductReviews.TotalPages,
                productReviewResponse.ProductReviews.HasNext,
                productReviewResponse.ProductReviews.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
        }

        return productReviewPaged;
    }

    [HttpGet("details")]
    [ProducesResponseType(typeof(GetProductReviewByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<GetProductReviewByIdResponse, ErrorResponse>>> GetByIdAsync([FromQuery] int id)
    => await _service.GetProductReviewByIdAsync(id);

    [HttpPost("create")]
    [Authorize(Roles = $"{nameof(UserPermissions.Customer)}")]
    [ProducesResponseType(typeof(CreatedProductReviewResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<CreatedProductReviewResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedProductReviewRequest productReview)
        => await _service.CreateProductReviewAsync(productReview, base.Request.Path.Value!);

    [HttpDelete("delete")]
    [ProducesResponseType(typeof(DeleteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<DeleteResponse, ErrorResponse>>> DeleteByIdAsync([FromQuery] int id)
        => await _service.DeleteProductReviewByIdAsync(id);

    [HttpPut("update")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<UpdateResponse, ErrorResponse>>> UpdateByIdAsync([FromBody] UpdateProductReviewRequest productReview)
        => await _service.UpdateProductReviewByIdAsync(productReview);
}