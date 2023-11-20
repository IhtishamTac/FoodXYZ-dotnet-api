namespace FoodXYZ.Contracts.Invoice
{
    public record ItemEntity(
        int ProductId,
        int SubTotal,
        int Qty
    );
}