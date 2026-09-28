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

        if (product is null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateProductRequest request)
    {
        var product = await _productService.CreateAsync(request);

        if (product is null)
        {
            return BadRequest(new
            {
                message = "The specified category does not exist."
            });
        }

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

        if (product is null)
        {
            return BadRequest(new
            {
                message = "The product or category does not exist."
            });
        }

        return Ok(product);
    }
[HttpDelete("{id:int}")]
public async Task<IActionResult> Delete(int id)
{
    var result = await _productService.DeleteAsync(id);

    return result switch
    {
        ProductDeleteResult.NotFound => NotFound(),

        ProductDeleteResult.HasOrderItems =>
            Conflict(new
            {
                message = "Cannot delete a product that belongs to an order."
            }),

        ProductDeleteResult.Deleted => NoContent(),

        _ => StatusCode(500)
    };
}
}