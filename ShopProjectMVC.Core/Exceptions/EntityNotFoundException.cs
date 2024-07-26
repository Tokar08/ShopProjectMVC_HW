namespace ShopProjectMVC.Core.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(Type entityType, int id) 
        : base($"Entity of type {entityType.Name} with id {id} not found!")
    {
    }
}