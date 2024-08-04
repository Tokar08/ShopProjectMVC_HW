using Microsoft.AspNetCore.Mvc;
using ShopProjectMVC.Core.Access;
using ShopProjectMVC.Core.Exceptions;
using ShopProjectMVC.Core.Interfaces;
using ShopProjectMVC.Core.Models;

namespace ShopProjectMVC.Controllers;

[RequireLogin]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = HttpContext.Session.GetInt32("id") ??
                         throw new InvalidOperationException("В сессии такой Id не найден");
            var orders = _orderService.GetOrders(userId).ToList();
            return View(orders);
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Home", new { message = $"Ошибка при получении заказов: {ex.Message}" });
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var order = await _orderService.GetOrderById(id)
                        ?? throw new EntityNotFoundException(typeof(Order), id);
            return View(order);
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Home",
                new { message = $"Ошибка при получении деталей заказа: {ex.Message}" });
        }
    }

    [RequireLogin]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var order = await _orderService.GetOrderById(id)
                        ?? throw new EntityNotFoundException(typeof(Order), id);
            return View(order);
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Home",
                new { message = $"Ошибка при получении заказа для удаления: {ex.Message}" });
        }
    }

    [HttpPost]
    [RequireLogin]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        try
        {
            var order = await _orderService.GetOrderById(id)
                        ?? throw new EntityNotFoundException(typeof(Order), id);
            
            order.Product.Count++;
            await _orderService.DeleteOrder(order.Id);
            
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Home", new { message = $"Ошибка при удалении заказа: {ex.Message}" });
        }
    }
}