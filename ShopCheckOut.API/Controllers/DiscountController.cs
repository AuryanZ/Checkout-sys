using Microsoft.AspNetCore.Mvc;
using ShopCheckOut.API.Data.Services.Discounts;
using ShopCheckOut.API.Dtos.Discounts;

namespace ShopCheckOut.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DiscountController(IDiscountService discountService) : ControllerBase
    {
        [HttpGet(Name = "Get ALL Avaliable Discounts")]
        public async Task<IActionResult> GetDiscounts()
        {
            try
            {
                var discounts = await discountService.GetDiscounts();
                return Ok(discounts);
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

        [HttpPost(Name = "Add Discount")]
        public async Task<IActionResult> AddDiscount([FromBody] AddDiscountRequest request)
        {
            try
            {
                await discountService.AddNewDiscout(request);
                return Ok(new { Message = "Discount added successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new ErrorResponse("Discount not created", ex.Message));
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

        [HttpDelete("{discountId}", Name = "Delete Discount")]
        public async Task<IActionResult> DeleteDiscount(int discountId)
        {
            try
            {
                await discountService.DeleteDiscount(discountId);
                return Ok(new { message = $"Deleted discount {discountId}" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse("Discount not deleted", ex.Message));
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
}
