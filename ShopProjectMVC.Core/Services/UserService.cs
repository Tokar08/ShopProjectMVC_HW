using ShopProjectMVC.Core.Exceptions;
using ShopProjectMVC.Core.Interfaces;
using ShopProjectMVC.Core.Models;

namespace ShopProjectMVC.Core.Services;

public class UserService : IUserService
{
    private readonly IRepository _repository;

    public UserService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<User> Login(string email, string password)
    {
        var user = _repository.GetAll<User>()
            .SingleOrDefault(u => u.Email == email && u.Password == password);

        return user ?? throw new EntityNotFoundException(typeof(User), -1); 
    }
    
    public async Task<User> Register(User user)
    {
        try
        {
            return await _repository.Add(user);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Ошибка при регистрации пользователя: " + ex.Message, ex);
        }
    }
}