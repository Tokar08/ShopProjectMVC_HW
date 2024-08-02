using Microsoft.AspNetCore.Mvc;
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

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();  
    }

    [HttpPost]
    public async Task<IActionResult> Login(User user)
    {
        var userFromDb = await _userService.Login(user.Email, user.Password);
        // save user
        
        if(userFromDb == null)
        {
            return NotFound();
        }

        HttpContext.Session.SetString("user", userFromDb.Name);
        HttpContext.Session.SetInt32("role", (int)userFromDb.Role);
        return RedirectToAction("Index", "Product");
    }

    [HttpPost]
    public async Task<IActionResult> Register(User user)
    {
        user.Role = Role.Client;
        user.CreatedAt = DateTime.UtcNow;
        await _userService.Register(user);
        return RedirectToAction("Index", "Product");
    }
}
