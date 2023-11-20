namespace FoodXYZ.Contracts.Invoice
{
    public record AddInvoiceRequest(
        int QtyTotal,
        int PriceTotal,
        List<ItemEntity> items
    );
}