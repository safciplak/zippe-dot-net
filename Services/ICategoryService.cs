using ProductService.Models;

public interface ICategoryService
{
    Task<Category> GetCategoryByIdAsync(int id);
}