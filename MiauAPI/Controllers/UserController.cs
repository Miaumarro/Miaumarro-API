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
public sealed class UserController : ControllerBase
{
    private readonly UserService _service;

    public UserController(UserService service)
        => _service = service;

    [HttpGet()]
    [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<GetUserResponse, ErrorResponse>>> GetAsync([FromQuery] UserParameters userParameters)
    {
        var usersPaged = await _service.GetUserAsync(userParameters);

        if (usersPaged.Result is OkObjectResult response && response.Value is GetUserResponse userResponse)
        {
            var metadata = new
            {
                userResponse.Users.TotalCount,
                userResponse.Users.PageSize,
                userResponse.Users.CurrentPage,
                userResponse.Users.TotalPages,
                userResponse.Users.HasNext,
                userResponse.Users.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

        }
        return usersPaged;
    }



    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<CreatedUserResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedUserRequest user)
        => await _service.CreateUserAsync(user, base.Request.Path.Value!);
}