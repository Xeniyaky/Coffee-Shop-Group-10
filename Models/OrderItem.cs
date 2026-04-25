namespace CoffeeShopMVC.Models
{
    public class OrderItem
    {
        public int Id { get; set; } // PK

        public int OrderId { get; set; } // FK
        public Order Order { get; set; }

        public int MenuItemId { get; set; } // FK
        public MenuItem MenuItem { get; set; }

        public int Quantity { get; set; }
    }
}