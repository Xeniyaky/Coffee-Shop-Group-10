namespace CoffeeShopMVC.Models
{
    public class Customer
    {
        public int Id { get; set; } // PK
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Order> Orders { get; set; } = new();
    }
}