namespace CoffeeShopMVC.Models
{
    public class Order
    {
        public int Id { get; set; } // Only ONE of these!
        public string CustomerName { get; set; }
        public string DrinkItem { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}