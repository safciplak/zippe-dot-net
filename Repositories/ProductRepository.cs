using ProductService.Models;
using ProductService.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();
    private int _nextId = 1;
    private readonly ICategoryService _categoryService;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(ICategoryService categoryService, ILogger<ProductRepository> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    public IEnumerable<Product> GetAll()
    {
        _logger.LogInformation("fetching all products");
        return _products;
    }

    public Product GetById(int id)
    {
        _logger.LogInformation("fetching product with id {ProductId}", id);
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public async Task AddAsync(Product product)
    {
        if (product == null)
        {
            _logger.LogWarning("prouduct is null");
            throw new ArgumentNullException(nameof(product), "product cannot be null");
        }

        var category = await _categoryService.GetCategoryByIdAsync(product.CategoryId);
        if (category == null)
        {
            _logger.LogWarning("Category with ID {CategoryId} was not found", product.CategoryId);
            throw new Exception($"Category with ID {product.CategoryId} was not found.");
        }

        _logger.LogInformation("[EXTERNAL API DATA ADD] Category data retrieved successfully. category_id = {CategoryId} | category_name = {CategoryName}", category.Id, category.Name);
        
        product.Id = _nextId++;
        _products.Add(product);
        _logger.LogInformation("product with id {ProductId} added successfully", product.Id);
    }

    public async Task UpdateAsync(Product product)
    {
        if (product == null)
        {
            _logger.LogWarning("product is null");
            throw new ArgumentNullException(nameof(product), "product cannot be null");
        }

        var category = await _categoryService.GetCategoryByIdAsync(product.CategoryId);
        if (category == null)
        {
            _logger.LogWarning("category with ID {CategoryId} was not found", product.CategoryId);
            throw new Exception($"category with ID {product.CategoryId} was not found.");
        }
        
        _logger.LogInformation("[EXTERNAL API DATA UPDATE] Category data retrieved successfully. category_id = {CategoryId} | category_name = {CategoryName}", category.Id, category.Name);

        var existing = GetById(product.Id);
        if (existing != null)
        {
            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.CategoryId = product.CategoryId;
            _logger.LogInformation("product with id {ProductId} updated successfully", product.Id);
        }
        else
        {
            _logger.LogWarning("product with id {ProductId} not found for update", product.Id);
        }
    }

    public void Delete(int id)
    {
        var removedCount = _products.RemoveAll(p => p.Id == id);
        if (removedCount > 0)
        {
            _logger.LogInformation("product with id {ProductId} deleted successfully", id);
        }
        else
        {
            _logger.LogWarning("product with id {ProductId} not found for deletion", id);
        }
    }
}
