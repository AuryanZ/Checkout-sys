using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShopCheckOut.API.Data.Products;
using ShopCheckOut.API.Dtos.Products;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductsService _iProductsService;
    private readonly IMapper _mapper;
    public ProductController(IProductsService iProductsService, IMapper mapper)
    {
        _iProductsService = iProductsService;
        _mapper = mapper;
    }
    [HttpGet("all")]
    public async Task<ActionResult<List<ProductReadDto>>> GetProducts()
    {
        try
        {
            var products = await _iProductsService.GetProducts();
            var result = _mapper.Map<List<ProductReadDto>>(products);
            return Ok(result);
        }
        catch (Exception ex)
        {

            return BadRequest(new ErrorResponse("Cannot Get Products", ex.Message));
        }
    }

    [HttpGet(Name = "GetProductByCategory")]
    public async Task<ActionResult<List<ProductReadDto>>> GetProductsByCategory([FromQuery] string category)
    {
        if (string.IsNullOrEmpty(category))
        {
            return BadRequest(new ErrorResponse("Cannot Get Products", "Request category"));
        }
        try
        {
            var products = await _iProductsService.GetProductsByCategory(category);
            var result = _mapper.Map<List<ProductReadDto>>(products);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse($"Cannot Get Products in {category}", ex.Message));

        }
    }

    [HttpGet("{sku}", Name = "GetProductBySKU")]
    public async Task<ActionResult<ProductReadDto>> GetProductBySKU(string sku)
    {
        if (string.IsNullOrEmpty(sku))
        {
            return BadRequest(new ErrorResponse("Cannot Get Products", "Request SKU"));
        }
        try
        {
            var product = await _iProductsService.GetProductBySKU(sku);
            if (product == null)
            {
                return NotFound();
            }
            var result = _mapper.Map<ProductReadDto>(product);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse($"Cannot Get Products {sku}", ex.Message));
        }
    }

    [HttpPost(Name = "AddProduct")]
    public async Task<IActionResult> AddProduct([FromBody] ProductCreateDto productCreateDto)
    {
        if (productCreateDto != null)
        {
            try
            {
                var product = _mapper.Map<ProductsModel>(productCreateDto);
                var result = await _iProductsService.AddProduct(product);
                if (result)
                {
                    return Ok(result);
                }
                return BadRequest(new ErrorResponse("Product Add not Success", "Serviec return null"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse("Prodcut not created", ex.Message));
            }
        }
        else
        {
            return BadRequest(new ErrorResponse("Product Add not Success", "Requset data missing"));
        }
    }
}
