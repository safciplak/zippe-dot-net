using System.Text.Json;
using ProductService.Models;

public class CategoryService : ICategoryService
{
    private readonly HttpClient _httpClient;

    public CategoryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"http://localhost:8080/api/v1/categories/{id}");
        
            response.EnsureSuccessStatusCode();
        
            var category = await response.Content.ReadFromJsonAsync<Category>();
        
            if (category == null)
            {
                throw new Exception($"category with id {id} was not found.");
            }
        
            return category;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception("an error occurred while fetching the category.", ex);
        }
        catch (JsonException ex)
        {
            throw new Exception("an error occurred while deserializing the category response.", ex);
        }
    }
}