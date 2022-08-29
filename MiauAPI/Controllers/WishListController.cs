using MiauAPI.Common;
using MiauAPI.Extensions;
using MiauAPI.Models.QueryObjects;
using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Services;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OneOf.Types;
using System.Text.Json;

namespace MiauAPI.Controllers;

[ApiController]
[Route(ApiConstants.MainEndpoint)]
public sealed class WishlistController : ControllerBase
{
    private readonly WishlistService _service;

    public WishlistController(WishlistService service)
        => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ProductObject[]>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<PagedResponse<ProductObject[]>, None>>> GetAsync([FromQuery] WishlistParameters wishListParameters)
    {
        var actionResult = await _service.GetWishlistAsync(wishListParameters);

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

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedWishlistResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OneOf<CreatedWishlistResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedWishlistRequest wishList)
        => await _service.CreateWishlistAsync(wishList, base.Request.Path.Value!);

    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteByIdAsync([FromQuery] DeleteWishlistRequest request)
        => await _service.DeleteWishlistAsync(request);
}