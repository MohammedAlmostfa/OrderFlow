using Microsoft.AspNetCore.Mvc;
using OrderFlow.Api.DTOs.Products;
using OrderFlow.Api.Services.Products;

namespace OrderFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateProductRequest request)
    {
        var product = await _productService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateProductRequest request)
    {
        var product = await _productService.UpdateAsync(id, request);

        return Ok(product);
    }
[HttpDelete("{id:int}")]
public async Task<IActionResult> Delete(int id)
{
    await _productService.DeleteAsync(id);

    return NoContent();
}
}