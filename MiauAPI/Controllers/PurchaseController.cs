using MiauAPI.Common;
using MiauAPI.Models.QueryObjects;
using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Responses;
using MiauAPI.Services;
using Microsoft.AspNetCore.Mvc;
using OneOf.Types;
using OneOf;
using MiauAPI.Extensions;
using System.Text.Json;
using MiauAPI.Models.Requests;

namespace MiauAPI.Controllers;

[ApiController, Route(ApiConstants.MainEndpoint)]
public class PurchaseController : ControllerBase
{
    private readonly PurchaseService _service;

    public PurchaseController(PurchaseService service)
        => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<PurchaseObject[]>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<PagedResponse<PurchaseObject[]>, None>>> GetAsync([FromQuery] PurchaseParameters request)
    {
        var actionResult = await _service.GetPurchasesAsync(request);

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

    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateAsync([FromBody] UpdatePurchaseRequest request)
        => await _service.UpdatePurchaseAsync(request);
}
