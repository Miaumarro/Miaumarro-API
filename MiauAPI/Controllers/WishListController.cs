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
public sealed class WishListController : ControllerBase
{
    private readonly WishListService _service;

    public WishListController(WishListService service)
        => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(GetWishListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<GetWishListResponse, ErrorResponse>>> GetAsync([FromQuery] WishListParameters wishListParameters)
    {
        var wishListsPaged = await _service.GetWishListAsync(wishListParameters);

        if (wishListsPaged.Result is OkObjectResult response && response.Value is GetWishListResponse wishListResponse)
        {
            var metadata = new
            {
                wishListResponse.WishList.TotalCount,
                wishListResponse.WishList.PageSize,
                wishListResponse.WishList.CurrentPage,
                wishListResponse.WishList.TotalPages,
                wishListResponse.WishList.HasNext,
                wishListResponse.WishList.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
        }

        return wishListsPaged;
    }

    [HttpGet("detail")]
    [ProducesResponseType(typeof(GetWishListByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<GetWishListByIdResponse, ErrorResponse>>> GetByIdAsync([FromQuery] int id)
    => await _service.GetWishListByIdAsync(id);

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedWishListResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<CreatedWishListResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedWishListRequest wishList)
        => await _service.CreateWishListAsync(wishList, base.Request.Path.Value!);

    [HttpDelete("delete")]
    [ProducesResponseType(typeof(DeleteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<DeleteResponse, ErrorResponse>>> DeleteByIdAsync([FromQuery] int id)
        => await _service.DeleteWishListByIdAsync(id);

    //[HttpPut("update")]
    //[ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    //public async Task<ActionResult<OneOf<UpdateResponse, ErrorResponse>>> UpdateByIdAsync([FromBody] UpdateWishListRequest wishList)
    //    => await _service.UpdateWishListByIdAsync(wishList);
}