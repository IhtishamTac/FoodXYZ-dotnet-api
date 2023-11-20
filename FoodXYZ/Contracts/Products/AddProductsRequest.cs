namespace FoodXYZ.Contracts.Products
{
    public record AddProductsRequest(
        string Name,
        int Price,
        double Rating,
        IFormFile Image
    );
}