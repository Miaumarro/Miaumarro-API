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
    public async Task<ActionResult<OneOf<GetPurchaseResponse, ErrorResponse>>> GetAsync([FromQuery] PurchaseParameters purchaseParameters)
    {
        var purchasesPaged = await _service.GetPurchaseAsync(purchaseParameters);

        if (purchasesPaged.Result is OkObjectResult response && response.Value is GetPurchaseResponse purchaseResponse)
        {
            var metadata = new
            {
                purchaseResponse.Purchases.TotalCount,
                purchaseResponse.Purchases.PageSize,
                purchaseResponse.Purchases.CurrentPage,
                purchaseResponse.Purchases.TotalPages,
                purchaseResponse.Purchases.HasNext,
                purchaseResponse.Purchases.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
        }

        return purchasesPaged;
    }

    [HttpGet("detail")]
    [ProducesResponseType(typeof(GetPurchaseByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<GetPurchasedProductByIdResponse, ErrorResponse>>> GetByIdAsync([FromQuery] int id)
    => await _service.GetPurchasedProductByIdAsync(id);

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedPurchaseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<CreatedPurchaseResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedPurchaseRequest purchase)
        => await _service.CreatePurchaseAsync(purchase, base.Request.Path.Value!);

    [HttpPut("update")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<UpdateResponse, ErrorResponse>>> UpdateByIdAsync([FromBody] UpdatePurchaseRequest purchase)
        => await _service.UpdatePurchaseByIdAsync(purchase);
}