using System;

namespace Core.Entities.OrderAggregate;

public class OrderItem : BaseEntity
{
    //Object to be ordered
    public ProductItemOrdered ItemOrdered { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
