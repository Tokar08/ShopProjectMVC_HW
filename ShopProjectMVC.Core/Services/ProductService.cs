using ShopProjectMVC.Core.Exceptions;
using ShopProjectMVC.Core.Interfaces;
using ShopProjectMVC.Core.Models;

namespace ShopProjectMVC.Core.Services;

public class ProductService : IProductService
{
    private readonly IRepository _repository;

    public ProductService(IRepository repository)
    {
        _repository = repository;
    }

    public Task<Product> AddProduct(Product product)
    {
        return _repository.Add(product);
    }

    public Task<Product> UpdateProduct(Product product)
    {
        return _repository.Update(product);
    }

    public Task DeleteProduct(int id)
    {
        return _repository.Delete<Product>(id);
    }

    public Task<Product> GetProductById(int id)
    {
        return _repository.GetById<Product>(id);
    }

    public IEnumerable<Product> GetAll()
    {
        // По вашему совету добавлена фильтрация товаров, количество которых больше нуля,
        // чтобы пользователи видели только доступные товары
        return _repository.GetAll<Product>().Where(product => product.Count > 0).ToList();
    }

    public async Task<Order> BuyProduct(int userId, int productId)
    {
        var user = await _repository.GetById<User>(userId) ?? throw new EntityNotFoundException(typeof(User), userId);
        var product = await _repository.GetById<Product>(productId) ??
                      throw new EntityNotFoundException(typeof(Product), productId);

        if (product.Count <= 0) 
            throw new InvalidOperationException("Product is out of stock!");

        var order = new Order()
        {
            User = user,
            Product = product,
            CreatedAt = DateTime.UtcNow
        };
        
        product.Count--;
        
        await _repository.Add(order);
        await _repository.Update(product);

        return order;
    }
}