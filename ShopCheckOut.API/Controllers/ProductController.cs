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
    private readonly IProductsRepo _iProductsRepo;
    private readonly IMapper _mapper;
    public ProductController(IProductsRepo iProductsService, IMapper mapper)
    {
        _iProductsRepo = iProductsService;
        _mapper = mapper;
    }
    [HttpGet(Name = "Get ALL Products")]
    public async Task<ActionResult<List<ProductReadDto>>> GetProducts()
    {
        try
        {
            var products = await _iProductsRepo.GetProducts();
            var result = _mapper.Map<List<ProductReadDto>>(products);
            return Ok(result);
        }
        catch (Exception ex)
        {

            return BadRequest(new ErrorResponse("Cannot Get Products", ex.Message));
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
                await _iProductsRepo.AddProduct(product);

                return Ok(new { message = "Add product success" });

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

    [HttpGet("category/{category}", Name = "GetProductByCategory")]
    public async Task<ActionResult<List<ProductReadDto>>> GetProductsByCategory(string category)
    {
        if (string.IsNullOrEmpty(category))
        {
            return BadRequest(new ErrorResponse("Cannot Get Products", "Request category"));
        }
        try
        {
            var products = await _iProductsRepo.GetProductsByCategory(category);
            var result = _mapper.Map<List<ProductReadDto>>(products);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse($"Cannot Get Products in {category}", ex.Message));

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
            var product = await _iProductsRepo.GetProductBySKU(sku);
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


}
