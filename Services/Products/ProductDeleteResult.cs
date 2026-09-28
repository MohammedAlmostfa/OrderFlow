namespace OrderFlow.Api.Services.Products;


public enum ProductDeleteResult
{
    

    NotFound,
    HasOrderItems,
    Deleted
}