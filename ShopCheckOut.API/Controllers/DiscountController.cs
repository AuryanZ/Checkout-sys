using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShopCheckOut.API.Data.Discounts;
using ShopCheckOut.API.Data.Products;
using ShopCheckOut.API.Dtos.Discounts;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;
        private readonly IProductsService _productsService;
        private readonly IMapper _mapper;
        public DiscountController(IDiscountService discountService, IProductsService productsService, IMapper mapper)
        {
            _discountService = discountService;
            _productsService = productsService;
            _mapper = mapper;
        }

        [HttpGet(Name = "Get ALL Avaliable Discounts")]
        public async Task<ActionResult<List<DiscountsModel>>> GetDiscounts()
        {
            try
            {
                var discounts = await _discountService.GetAvailableDiscounts();
                return Ok(discounts);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse("Cannot Get Discounts", ex.Message));
            }
        }

        [HttpPost(Name = "Add Discount")]
        public async Task<IActionResult> AddDiscount([FromBody] AddDiscountRequest request)
        {
            try
            {
                var discoutType = request.GetDiscountType();
                if (discoutType == null)
                {
                    return BadRequest(new ErrorResponse("Discount Type Not Found", "Request data missing"));
                }
                var productId = await _productsService.GetProductIdBySku(request.productSKU);
                int _productId = int.Parse(productId);
                var discount = (DiscountsModel)_mapper.Map(request, request.GetType(), discoutType);
                var result = await _discountService.AddNewDiscout(discount, _productId);
                if (result)
                {
                    return Ok(result);
                }
                return BadRequest(new ErrorResponse("Discount Add not Success", "Serviec return null"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse("Discount not created", ex.Message));
            }
        }

        [HttpDelete("{discountId}", Name = "Delete Discount")]
        public async Task<IActionResult> DeleteDiscount(int discountId)
        {
            try
            {
                var result = await _discountService.DeleteDiscount(discountId);
                if (result)
                {
                    return Ok(result);
                }
                return BadRequest(new ErrorResponse("Discount Delete not Success", "Serviec return null"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse("Discount not deleted", ex.Message));
            }
        }
    }
}
