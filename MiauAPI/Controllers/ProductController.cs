using MiauAPI.Common;
using MiauAPI.Models.Parameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Services;
using Microsoft.AspNetCore.Mvc;
using MiauAPI.Pagination;
using OneOf;

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

        var teste = productsPaged.Value;
        var teste2 = (GetProductResponse)productsPaged.Result;
        var teste3 = productsPaged.Result as productsPaged.Value;

        teste2.Products;
        teste3.Products;

        if (productsPaged is GetProductResponse response)
        {
            response.Products;
        }

        /*

            if (productsPaged.Result.Value.GetType() == typeof(GetProductResponse))
        {

            var metadata = new
            {
               
                productsPaged.TotalCount,
                productsPaged.PageSize,
                productsPaged.CurrentPage,
                productsPaged.TotalPages,
                productsPaged.HasNext,
                productsPaged.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            _logger.LogInfo($"Returned {dbProductsPaged.TotalCount} products from database.");
        }
        */
        Console.WriteLine(productsPaged);
        return productsPaged;
    }

    /*
    [HttpGet()]
    [ProducesResponseType(typeof(GetProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<GetProductResponse, ErrorResponse>>> GetAsync([FromQuery] ProductParameters productParameters)
        => await _service.GetProductAsync(productParameters);
    */
    

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OneOf<CreatedProductResponse, ErrorResponse>>> RegisterAsync([FromBody] CreatedProductRequest product)
        => await _service.CreateProductAsync(product, base.Request.Path.Value!);

}