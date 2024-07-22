using ProductService.Models;

namespace ProductService.Repositories
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        Product GetById(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        void Delete(int id);
    }
}