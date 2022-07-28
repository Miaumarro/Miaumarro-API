using MiauDatabase;
using MiauDatabase.Entities;
using MiauDatabase.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MiauAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProductControllerAnterior : ControllerBase
{
    private readonly MiauDbContext _db;

    public ProductControllerAnterior(MiauDbContext db)
    {
        _db = db;
    }


    // GET api/<ValuesController>/5
    [HttpGet("{id}")]
    public ActionResult<ProductEntity> Get(int id)
    {
        try
        {
            var product = _db.Products.Find(id);
            return Ok(product);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // GET: api/products/OrderBy{option}  Options: PriceDesc, PriceAsc, DiscountDesc, DiscountAsc
    [HttpGet("OrderBy{option}")]
    public ActionResult GetOrder(string option)
    {
        try
        {
            var products = _db.Products.ToList();

            switch (option.ToLower())
            {
                case "pricedesc":
                    products = products
                                .OrderByDescending(p => (p.Price * (1 - p.Discount)))
                                .ToList();
                    break;
                case "priceasc":
                    products = products
                                .OrderBy(p => (p.Price * (1 - p.Discount)))
                                .ToList();
                    break;
                case "discountdesc":
                    products = products
                                .OrderByDescending(p => p.Discount)
                                .ToList();
                    break;
                case "discountasc":
                    products = products
                                .OrderBy(p => p.Discount)
                                .ToList();
                    break;
            }

            products = products
                    .OrderByDescending(p => p.IsActive)
                    .ToList();

            return Ok(products);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }


    // POST api/<ValuesController>
    [HttpPost("register")]
    public ActionResult Register(ProductEntity product)
    {
        try
        {
            _db.Products.Add(product);
            _db.SaveChanges();
            return Ok(product);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // POST api/<ValuesController>
    [HttpPost("create")]
    public ActionResult Create()
    {
        try
        {
            var product = new ProductEntity()
            {
                Description = "ProductTag.SmallPet, ProductTag.Clothing e ProductTag.Accessory",
                Price = 75.90M,
                IsActive = false,
                Amount = 10,
                Tags = ProductTag.SmallPet | ProductTag.Clothing | ProductTag.Accessory,
                Brand = "Test",
                Discount = 0.1M
            };
            _db.Products.Add(product);
            _db.SaveChanges();
            return Ok(product);
        }
        catch (Exception er)
        {
            return BadRequest(er.Message);
        }
    }

    // PUT api/<ValuesController>/5
    [HttpPost("{id}")]
    public ActionResult Put(int id, ProductEntity product)
    {
        try
        {
            var productUpdate = new ProductEntity();
            productUpdate = product;
            _db.Products.Update(productUpdate);
            _db.SaveChanges();
            return Ok(product);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE api/<ValuesController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }


}
