using MiauAPI.Common;
using MiauAPI.Models.Parameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Services;
using Microsoft.AspNetCore.Mvc;
using MiauAPI.Pagination;
using OneOf;
using Newtonsoft.Json;

namespace MiauAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductController : ControllerBase
{
    private readonly ProductService _service;

    public ProductController(ProductService service)
        => _service = service;

    
    [HttpGet()]
    [ProducesResponseType(typeof(GetProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<GetProductResponse, ErrorResponse>>> GetAsync([FromQuery] ProductParameters productParameters)
    {
        var productsPaged = await _service.GetProductAsync(productParameters);

        if (productsPaged.Result is OkObjectResult response && response.Value is GetProductResponse productResponse)
        {
            var metadata = new
            {
                productResponse.Products.TotalCount,
                productResponse.Products.PageSize,
                productResponse.Products.CurrentPage,
                productResponse.Products.TotalPages,
                productResponse.Products.HasNext,
                productResponse.Products.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

        }
        return productsPaged;
    }


    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<CreatedProductResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedProductRequest product)
        => await _service.CreateProductAsync(product, base.Request.Path.Value!);

}