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
        private readonly IDiscountRepo _discountRepo;
        private readonly IProductsRepo _productsRepo;
        private readonly IMapper _mapper;
        public DiscountController(IDiscountRepo discountService, IProductsRepo productsService, IMapper mapper)
        {
            _discountRepo = discountService;
            _productsRepo = productsService;
            _mapper = mapper;
        }

        [HttpGet(Name = "Get ALL Avaliable Discounts")]
        public async Task<ActionResult<List<DiscountsModel>>> GetDiscounts()
        {
            try
            {
                var discounts = await _discountRepo.GetAvailableDiscounts();
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
                var productId = await _productsRepo.GetProductIdBySku(request.ProductSKU);
                int _productId = int.Parse(productId);
                var discount = (DiscountsModel)_mapper.Map(request, request.GetType(), discoutType);

                await _discountRepo.AddNewDiscout(discount, _productId);
                return Ok(new { Message = "Discount added successfully" });
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
                await _discountRepo.DeleteDiscount(discountId);
                return Ok(new { message = $"Deleted discount {discountId}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse("Discount not deleted", ex.Message));
            }
        }
    }
}
