namespace OrderManagement.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemModel> Items { get; set; }
    }

    public class OrderItemModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
