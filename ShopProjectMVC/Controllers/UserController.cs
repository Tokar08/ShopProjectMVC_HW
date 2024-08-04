using Microsoft.AspNetCore.Mvc;
using ShopProjectMVC.Core.Exceptions;
using ShopProjectMVC.Core.Interfaces;
using ShopProjectMVC.Core.Models;

namespace ShopProjectMVC.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    public IActionResult Login() => View();

    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Login(User user)
    {
        try
        {
            var userFromDb = await _userService.Login(user.Email, user.Password) 
                             ?? throw new EntityNotFoundException(typeof(User), 0);
            
            HttpContext.Session.SetInt32("id", userFromDb.Id);
            HttpContext.Session.SetString("user", userFromDb.Name);
            HttpContext.Session.SetInt32("role", (int)userFromDb.Role);

            return RedirectToAction("Index", "Product");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"Ошибка при попытке входа: {ex.Message}";
            return View(user);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Register(User user)
    {
        try
        {
            user.Role = Role.Client;
            user.CreatedAt = DateTime.UtcNow;
            await _userService.Register(user);
            return RedirectToAction("Index", "Product");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"Ошибка при регистрации: {ex.Message}";
            return View(user);
        }
    }
}