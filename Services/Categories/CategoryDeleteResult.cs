namespace OrderFlow.Api.Services.Categories;

public enum CategoryDeleteResult
{
    NotFound,
    HasProducts,
    Deleted
}