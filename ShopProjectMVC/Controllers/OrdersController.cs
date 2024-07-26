using Microsoft.AspNetCore.Mvc;
using ShopProjectMVC.Core.Interfaces;

namespace ShopProjectMVC.Controllers;

public class OrdersController : Controller
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public IActionResult Index()
    {
        var orders = _orderService.GetOrders(1);
        return View(orders);
    }
}
