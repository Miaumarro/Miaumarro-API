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
public sealed class WishlistController : ControllerBase
{
    private readonly WishlistService _service;

    public WishlistController(WishlistService service)
        => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(GetWishlistResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<GetWishlistResponse, ErrorResponse>>> GetAsync([FromQuery] WishlistParameters wishListParameters)
    {
        var wishListsPaged = await _service.GetWishlistAsync(wishListParameters);

        if (wishListsPaged.Result is OkObjectResult response && response.Value is GetWishlistResponse wishListResponse)
        {
            var metadata = new
            {
                wishListResponse.Wishlist.TotalCount,
                wishListResponse.Wishlist.PageSize,
                wishListResponse.Wishlist.CurrentPage,
                wishListResponse.Wishlist.TotalPages,
                wishListResponse.Wishlist.HasNext,
                wishListResponse.Wishlist.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
        }

        return wishListsPaged;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedWishlistResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<CreatedWishlistResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedWishlistRequest wishList)
        => await _service.CreateWishlistAsync(wishList, base.Request.Path.Value!);

    [HttpDelete("delete")]
    [ProducesResponseType(typeof(DeleteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<DeleteResponse, ErrorResponse>>> DeleteByIdAsync([FromQuery] int id)
        => await _service.DeleteWishlistByIdAsync(id);

}