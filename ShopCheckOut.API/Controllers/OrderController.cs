using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShopCheckOut.API.Data.Orders;
using ShopCheckOut.API.Data.Products;
using ShopCheckOut.API.Dtos.Orders;
using ShopCheckOut.API.Models;

namespace ShopCheckOut.API.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IOrderService _orderService;
    private readonly IProductsService _productsService;
    public OrderController(IMapper mapper, IOrderService orderService, IProductsService productsService)
    {
        _mapper = mapper;
        _orderService = orderService;
        _productsService = productsService;
    }
    [HttpGet(Name = "GetOrderByOrderId")]
    public async Task<IActionResult> GetOrderByOrderId([FromQuery] string orderId)
    {
        return NoContent();
    }

    [HttpPost(Name = "NewOrder")]
    public async Task<ActionResult<OrderCreateDto>> NewOrder([FromQuery] string? CustomerId)
    {
        try
        {
            var newOrder = await _orderService.NewOrder(CustomerId);
            var result = _mapper.Map<OrderCreateDto>(newOrder);

            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest(new ErrorResponse("New Order Not Created", "No data found"));
        }
        catch (Exception ex)
        {

            return BadRequest(new ErrorResponse("New Order Not Created", ex.Message));
        }
    }

    [HttpPost("item", Name = "AddItemToOrder")]
    public async Task<ActionResult<OrderUpdateDto>> AddItemToOrder([FromQuery] string orderId, [FromBody] AddItemRequest request)
    {
        int _orderId = 0;
        int _quantity = 0;
        if (string.IsNullOrEmpty(request.Quantity) || string.IsNullOrEmpty(request.ItemSKU))
        {
            return BadRequest(new ErrorResponse("No order id or SKu", "Requset data missing"));
        }
        try
        {
            _orderId = int.Parse(orderId);
            _quantity = int.Parse(request.Quantity);

            ProductsModel product = await _productsService.GetProductBySKU(request.ItemSKU);
            if (product == null)
            {
                return NotFound();
            }
            var order = await _orderService.AddItemToOrder(_orderId, product, _quantity);
            if (order == null)
            {
                return BadRequest(new ErrorResponse($"Cannot add item to {orderId}", "Order not Found"));

            }
            var result = _mapper.Map<OrderUpdateDto>(order);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse("Illegal ID. Error: ", ex.Message));
        }
    }

    [HttpDelete("item", Name = "RemoveItemFromOrder")]
    public async Task<ActionResult<OrderUpdateDto>> RemoveItemFromOrder([FromQuery] string orderId, [FromBody] RemoveItemRequest request)
    {
        if (string.IsNullOrEmpty(request.Quantity)
            || string.IsNullOrEmpty(request.ItemSku)
            || string.IsNullOrEmpty(orderId))
        {
            return BadRequest(new ErrorResponse("Remove item Error: ", "No order id or Sku"));
        }
        try
        {
            int _orderId = int.Parse(orderId);
            int _quantity = int.Parse(request.Quantity);
            string productId = await _productsService.GetProductIdBySku(request.ItemSku);
            if (string.IsNullOrEmpty(productId))
            {
                return BadRequest(new ErrorResponse($"Connot find product {request.ItemSku}", "Invalid product Sku"));

            }
            int _productId = int.Parse(productId);
            var order = await _orderService.DeleteItemFromOrder(_orderId, _productId, _quantity);
            if (order == null)
            {
                return NotFound("Order not Found");

            }
            var result = _mapper.Map<OrderUpdateDto>(order);
            return Ok(result);
        }
        catch (Exception ex)
        {

            return BadRequest(new ErrorResponse("Item not removed", ex.Message));
        }
    }

    [HttpGet("checkout", Name = "CheckOutOrder")]
    public async Task<ActionResult<OrderCheckoutDto>> CheckOutOrder([FromQuery] string orderId)
    {
        if (string.IsNullOrEmpty(orderId))
        {
            return BadRequest(new ErrorResponse("Check out Error", "No order id"));
        }
        try
        {
            int _orderId = int.Parse(orderId);
            var order = await _orderService.OrderCheckOut(_orderId);
            if (order == null)
            {
                return NotFound("Order not Found");
            }
            var result = _mapper.Map<OrderCheckoutDto>(order);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse("Check out Error", ex.Message));
        }
    }
}
