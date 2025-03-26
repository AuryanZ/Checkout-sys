using Microsoft.AspNetCore.Mvc;
using ShopCheckOut.API.Data.Services.Orders;
using ShopCheckOut.API.Dtos.Orders;

namespace ShopCheckOut.API.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController(IOrderService orderService) : ControllerBase
{
    [HttpPost("new", Name = "NewOrder")]
    public async Task<ActionResult<OrderCreateDto>> NewOrder([FromQuery] string? CustomerId)
    {
        try
        {
            var result = await orderService.NewOrder(CustomerId);
            return Ok(result);
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

    [HttpPost("item", Name = "AddItemToOrder")]
    public async Task<ActionResult<OrderUpdateDto>> AddItemToOrder([FromQuery] string orderId, [FromBody] AddItemRequest request)
    {
        try
        {
            var result = await orderService.AddItemToOrder(orderId, request);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponse("Order not found.", ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse("Invalid request data.", ex.Message));
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

    [HttpDelete("item", Name = "RemoveItemFromOrder")]
    public async Task<ActionResult<OrderUpdateDto>> RemoveItemFromOrder([FromQuery] string orderId, [FromBody] RemoveItemRequest request)
    {
        try
        {
            var order = await orderService.RemoveItemFromOrder(orderId, request);
            return Ok(order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse("Invalid request data.", ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponse("Order not found.", ex.Message));
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

    [HttpGet("checkout", Name = "CheckOutOrder")]
    public async Task<ActionResult<OrderCheckoutDto>> CheckOutOrder([FromQuery] string orderId)
    {
        try
        {
            var order = await orderService.OrderCheckOut(orderId);

            return Ok(order);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponse("Order not found.", ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse("Invalid request data.", ex.Message));
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
