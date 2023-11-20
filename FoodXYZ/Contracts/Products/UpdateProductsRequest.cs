namespace FoodXYZ.Contracts.Products
{
    public record UpdateProductsRequest(
        string Name,
        int Price,
        double Rating,
        IFormFile Image
    );
}