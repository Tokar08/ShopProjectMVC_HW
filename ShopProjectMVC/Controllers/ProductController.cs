using Microsoft.AspNetCore.Mvc;
using ShopProjectMVC.Core.Access;
using ShopProjectMVC.Core.Interfaces;
using ShopProjectMVC.Core.Models;

namespace ShopProjectMVC.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly IWebHostEnvironment _env;

    public ProductController(IProductService productService, IWebHostEnvironment env)
    {
        _productService = productService;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        //HttpContext.Session.SetString("user", "Name");
        //HttpContext.Session.SetInt32("role", (int)Role.Admin);
        //HttpContext.Session.SetInt32("id", 1);

        var products = await Task.Run(() => _productService.GetAll());
        return View(products);
    }
    
    [RequireLogin]
    public IActionResult Create()
    {
        ViewBag.Categories = _productService.GetAllCategories().ToList();
        return View();
    }

    [HttpPost]
    [RequireLogin]
    public async Task<IActionResult> Create(Product product, int category, IFormFile file)
    {
        string hash = Guid.NewGuid().ToString();
        string name = Path.GetFileNameWithoutExtension(file.FileName) + hash + Path.GetExtension(file.FileName);
        string path = Path.Combine(_env.WebRootPath, "pictures", name);
        using (var fileStream = new MemoryStream())
        {
            file.CopyTo(fileStream);
            await System.IO.File.WriteAllBytesAsync(path, fileStream.ToArray());
        }

        product.Image = name;
        product.Category = _productService.GetAllCategories().First(x => x.Id == category);
        await _productService.AddProduct(product);
        return RedirectToAction("Index");
    }
    
    [RequireLogin]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productService.GetProductById(id);
        return View(product);
    }

    [HttpPost]
    [RequireLogin]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _productService.GetProductById(id);
        string path = Path.Combine(_env.WebRootPath, "pictures", product.Image);
        await _productService.DeleteProduct(id);
        System.IO.File.Delete(path);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [RequireLogin]
    public async Task<IActionResult> Buy(int id)
    {
        int userId = HttpContext.Session.GetInt32("id").Value;
        await _productService.BuyProduct(userId, id);
        return RedirectToAction("Index");
    }
    
    [RequireLogin]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetProductById(id);
        return View(product);
    }

    [HttpPost]
    [RequireLogin]
    [ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(Product product, int id)
    {
        var productFromDb = await _productService.GetProductById(id);
        productFromDb.Name = product.Name;
        productFromDb.Description = product.Description;
        productFromDb.Price = product.Price;
        productFromDb.Count = product.Count;
        await _productService.UpdateProduct(productFromDb);
        return RedirectToAction("Index");
    }
}
