
using OrderFlow.Api.DTOs.Categories;

namespace OrderFlow.Api.Services.Categories;

public interface ICategoryService
{
    Task<List<CategoryResponse>> GetAllAsync();

    Task<CategoryResponse?> GetByIdAsync(int id);

    Task<CategoryResponse> CreateAsync(
        CreateCategoryRequest request);

    Task<CategoryResponse?> UpdateAsync(
        int id,
        UpdateCategoryRequest request);

    Task<CategoryDeleteResult> DeleteAsync(int id);
}