using OrderManagement.Models;

namespace OrderManagement.Services
{
    public interface IOrderService
    {
        IEnumerable<OrderModel> GetAllOrders();
        OrderModel GetOrderById(int id);
        void AddOrder(OrderModel order);
        void UpdateOrder(int id, OrderModel order);
        void DeleteOrder(int id);
    }

    public class OrderService : IOrderService
    {
        private readonly List<OrderModel> _orders = new List<OrderModel>();

        public IEnumerable<OrderModel> GetAllOrders()
        {
            return _orders;
        }

        public OrderModel GetOrderById(int id)
        {
            return _orders.FirstOrDefault(o => o.Id == id);
        }

        public void AddOrder(OrderModel order)
        {
            _orders.Add(order);
        }

        public void UpdateOrder(int id, OrderModel order)
        {
            var existingOrder = _orders.FirstOrDefault(o => o.Id == id);
            if (existingOrder != null)
            {
                existingOrder.CustomerName = order.CustomerName;
                existingOrder.OrderDate = order.OrderDate;
                existingOrder.Items = order.Items;
            }
        }

        public void DeleteOrder(int id)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id);
            if (order != null)
            {
                _orders.Remove(order);
            }
        }
    }
}
