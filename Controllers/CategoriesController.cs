
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Api.DTOs.Categories;
using OrderFlow.Api.Services.Categories;

namespace OrderFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();

        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        if (category is null)
            return NotFound();

        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCategoryRequest request)
    {
        var category = await _categoryService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = category.Id },
            category);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateCategoryRequest request)
    {
        var category = await _categoryService.UpdateAsync(id, request);

        if (category is null)
            return NotFound();

        return Ok(category);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _categoryService.DeleteAsync(id);

        return result switch
        {
            CategoryDeleteResult.NotFound => NotFound(),

            CategoryDeleteResult.HasProducts =>
                Conflict(new
                {
                    message = "Cannot delete a category that has products."
                }),

            CategoryDeleteResult.Deleted => NoContent(),

            _ => StatusCode(500)
        };
    }
}