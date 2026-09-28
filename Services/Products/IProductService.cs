using OrderFlow.Api.DTOs.Products;

namespace OrderFlow.Api.Services.Products;

public interface IProductService
{
    Task<List<ProductResponse>> GetAllAsync();

    Task<ProductResponse?> GetByIdAsync(int id);

    Task<ProductResponse?> CreateAsync(
        CreateProductRequest request);

    Task<ProductResponse?> UpdateAsync(
        int id,
        UpdateProductRequest request);

    Task<ProductDeleteResult> DeleteAsync(int id);}
    