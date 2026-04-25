public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }

    public int? MenuItemId { get; set; }
    public decimal Price { get; set; }
    public string DrinkName { get; set; }
    public string Size { get; set; }
    public int Quantity { get; set; }
}