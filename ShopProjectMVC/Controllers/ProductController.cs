using Microsoft.AspNetCore.Mvc;
using ShopProjectMVC.Core.Interfaces;

namespace ShopProjectMVC.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    public IActionResult Index()
    {
        return View(_productService.GetAll());
    }
}