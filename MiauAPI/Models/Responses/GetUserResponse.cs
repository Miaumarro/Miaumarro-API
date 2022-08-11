using MiauAPI.Models.QueryObjects;
using MiauAPI.Pagination;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when an user query is successfully executed.
/// </summary>
/// <param name="Users">The resulted list of users.</param>
public sealed record GetUserResponse(PagedList<UserObject> Users);
