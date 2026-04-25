using System;
using System.Collections.Generic;

namespace CoffeeShopMVC.Models;

public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public DateTime OrderDate { get; set; }

    // Total can stay if you want to store the grand total here
    public decimal Total { get; set; }

    // This is where the drink info lives now!
    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}