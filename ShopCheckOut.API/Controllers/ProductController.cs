using Microsoft.AspNetCore.Mvc;
using ShopCheckOut.API.Data.Services.Products;
using ShopCheckOut.API.Dtos.Products;

namespace ShopCheckOut.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController(IProductServices productServices) : ControllerBase
{
    [HttpGet(Name = "Get ALL Products")]
    public async Task<ActionResult<List<ProductReadDto>>> GetProducts()
    {
        try
        {
            var products = await productServices.GetProducts();
            return Ok(products);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponse("Product Not Found", ex.Message));
        }
        catch (ApplicationException ex)
        {
            return StatusCode(500, new ErrorResponse("Internal server error.", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse("An unexpected error occurred.", ex.Message));
        }
    }

    [HttpPost(Name = "AddProduct")]
    public async Task<IActionResult> AddProduct([FromBody] ProductCreateDto productCreateDto)
    {
        if (productCreateDto == null)
        {
            return BadRequest(new ErrorResponse("Product Add not Success", "Requset data missing"));
        }
        try
        {
            await productServices.AddProduct(productCreateDto);

            return Ok(new { message = "Add product success" });

        }
        catch (ApplicationException ex)
        {
            return StatusCode(500, new ErrorResponse("Internal server error.", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse("An unexpected error occurred.", ex.Message));
        }

    }

    [HttpGet("category/{category}", Name = "GetProductByCategory")]
    public async Task<ActionResult<List<ProductReadDto>>> GetProductsByCategory(string category)
    {
        if (string.IsNullOrEmpty(category))
        {
            return BadRequest(new ErrorResponse("Category must entry", "Request category"));
        }
        try
        {
            var products = await productServices.GetProductsByCategory(category);
            return Ok(products);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponse("Product Not Found", ex.Message));
        }
        catch (ApplicationException ex)
        {

            return StatusCode(500, new ErrorResponse("Internal server error.", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse("An unexpected error occurred.", ex.Message));

        }
    }

    [HttpGet("sku/{sku}", Name = "GetProductBySKU")]
    public async Task<ActionResult<ProductReadDto>> GetProductBySKU(string sku)
    {
        if (string.IsNullOrEmpty(sku))
        {
            return BadRequest(new ErrorResponse("Cannot Get Products", "Request SKU"));
        }
        try
        {
            var product = await productServices.GetProductBySKU(sku);
            return Ok(product);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponse("Product Not Found", ex.Message));
        }
        catch (ApplicationException ex)
        {
            return StatusCode(500, new ErrorResponse("Internal server error.", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse("An unexpected error occurred.", ex.Message));
        }
    }


}
