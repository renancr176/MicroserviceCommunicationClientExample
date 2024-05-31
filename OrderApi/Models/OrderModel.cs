namespace OrderApi.Models;

public class OrderModel
{
    public int Id { get; set; }
    public Guid CustomerId { get; set; }
    public IEnumerable<ProductModel> Products { get; set; }
    public decimal Total { get; set; }
}