using MiauAPI.Common;
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
public sealed class PurchaseController : ControllerBase
{
    private readonly PurchaseService _service;

    public PurchaseController(PurchaseService service)
        => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(GetPurchaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<GetPurchaseResponse, ErrorResponse>>> GetAsync([FromQuery] PurchaseParameters wishListParameters)
    {
        var wishListsPaged = await _service.GetPurchaseAsync(wishListParameters);

        if (wishListsPaged.Result is OkObjectResult response && response.Value is GetPurchaseResponse wishListResponse)
        {
            var metadata = new
            {
                wishListResponse.Purchase.TotalCount,
                wishListResponse.Purchase.PageSize,
                wishListResponse.Purchase.CurrentPage,
                wishListResponse.Purchase.TotalPages,
                wishListResponse.Purchase.HasNext,
                wishListResponse.Purchase.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
        }

        return wishListsPaged;
    }

    [HttpGet("detail")]
    [ProducesResponseType(typeof(GetPurchaseByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<GetPurchaseByIdResponse, ErrorResponse>>> GetByIdAsync([FromQuery] int id)
    => await _service.GetPurchaseByIdAsync(id);

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedPurchaseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<CreatedPurchaseResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedPurchaseRequest wishList)
        => await _service.CreatePurchaseAsync(wishList, base.Request.Path.Value!);

    [HttpDelete("delete")]
    [ProducesResponseType(typeof(DeleteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<DeleteResponse, ErrorResponse>>> DeleteByIdAsync([FromQuery] int id)
        => await _service.DeletePurchaseByIdAsync(id);

    [HttpPut("update")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<UpdateResponse, ErrorResponse>>> UpdateByIdAsync([FromBody] UpdatePurchaseRequest wishList)
        => await _service.UpdatePurchaseByIdAsync(wishList);
}