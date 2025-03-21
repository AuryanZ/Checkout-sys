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
        if (!string.IsNullOrEmpty(CustomerId))
        {
            // todo: 
            // 1. Verify CustomerId
            // 2. Create Order and link to Customer
        }
        else
        {
            try
            {
                var newOrder = await _orderService.NewOrder(CustomerId);
                var result = _mapper.Map<OrderCreateDto>(newOrder);

                if (result != null)
                {
                    return Ok(result);
                }
                return BadRequest("New Order Not Created");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        return BadRequest("no action found");
    }

    [HttpPost("item", Name = "AddItemToOrder")]
    public async Task<ActionResult<OrderUpdateDto>> AddItemToOrder([FromQuery] string orderId, [FromBody] AddItemRequest request)
    {
        int _orderId = 0;
        int _quantity = 0;
        if (string.IsNullOrEmpty(request.Quantity) || string.IsNullOrEmpty(request.ItemSKU))
        {
            return BadRequest("No order id or SKu");
        }
        try
        {
            _orderId = int.Parse(orderId);
            _quantity = int.Parse(request.Quantity);
        }
        catch (Exception ex)
        {
            return BadRequest("Illegal ID. Error: " + ex.Message);
        }

        ProductsModel product = await _productsService.GetProductBySKU(request.ItemSKU);
        if (product == null)
        {
            return NotFound();
        }
        try
        {
            var order = await _orderService.AddItemToOrder(_orderId, product, _quantity);
            var result = _mapper.Map<OrderUpdateDto>(order);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

}
