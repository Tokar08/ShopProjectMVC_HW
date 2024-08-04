using Microsoft.AspNetCore.Mvc;
using ShopProjectMVC.Core.Access;
using ShopProjectMVC.Core.Exceptions;
using ShopProjectMVC.Core.Interfaces;
using ShopProjectMVC.Core.Models;

namespace ShopProjectMVC.Controllers;

public class ProductController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly IProductService _productService;

    public ProductController(IProductService productService, IWebHostEnvironment env)
    {
        _productService = productService;
        _env = env;
    }

    public IActionResult Index()
    {
        try
        {
            var products = _productService.GetAll();
            return View(products);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Ошибка при получении продуктов: " + ex.Message;
            return View("Error");
        }
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
        try
        {
            var hash = Guid.NewGuid().ToString();
            var name = Path.GetFileNameWithoutExtension(file.FileName) + hash + Path.GetExtension(file.FileName);
            var path = Path.Combine(_env.WebRootPath, "pictures", name);
            using var fileStream = new MemoryStream();
            await file.CopyToAsync(fileStream);
            await System.IO.File.WriteAllBytesAsync(path, fileStream.ToArray());
            
            product.Image = name;
            product.Category = _productService.GetAllCategories().First(x => x.Id == category);
            await _productService.AddProduct(product);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Ошибка при добавлении продукта: " + ex.Message;
            return View("Error");
        }
    }


    [RequireLogin]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var product = await _productService.GetProductById(id)
                          ?? throw new EntityNotFoundException(typeof(Product), id);

            return View(product);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Ошибка при получении продукта для удаления: " + ex.Message;
            return View("Error");
        }
    }

    [HttpPost]
    [RequireLogin]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var product = await _productService.GetProductById(id)
                          ?? throw new EntityNotFoundException(typeof(Product), id);

            var path = Path.Combine(_env.WebRootPath, "pictures", product.Image);
            await _productService.DeleteProduct(id);
            System.IO.File.Delete(path);

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Ошибка при удалении продукта: " + ex.Message;
            return View("Error");
        }
    }

    [HttpPost]
    [RequireLogin]
    public async Task<IActionResult> Buy(int id)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32("id")
                         ?? throw new InvalidOperationException("В сессии такой Id не найден");

            await _productService.BuyProduct(userId, id);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Ошибка при покупке продукта: " + ex.Message;
            return View("Error");
        }
    }

    [RequireLogin]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var product = await _productService.GetProductById(id)
                          ?? throw new EntityNotFoundException(typeof(Product), id);

            return View(product);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Ошибка при получении продукта для редактирования: " + ex.Message;
            return View("Error");
        }
    }

    [HttpPost]
    [RequireLogin]
    [ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(Product product, int id)
    {
        try
        {
            var productFromDb = await _productService.GetProductById(id)
                                ?? throw new EntityNotFoundException(typeof(Product), id);

            productFromDb.Name = product.Name;
            productFromDb.Description = product.Description;
            productFromDb.Price = product.Price;
            productFromDb.Count = product.Count;

            await _productService.UpdateProduct(productFromDb);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Ошибка при обновлении продукта: " + ex.Message;
            return View("Error");
        }
    }
}