using RestaurantManagementService.Domain.ValueObjects;

namespace RestaurantManagementService.Domain.Entities
{
    public class OrderDetails
    {
        public Guid OrderId { get; private set; }
        public Guid RestaurantId { get; private set; }
        public List<MenuItem> SelectedItems { get; private set; }

        public OrderDetails(Guid orderId, Guid restaurantId)
        {
            OrderId = orderId;
            RestaurantId = restaurantId;
            SelectedItems = new List<MenuItem>();
        }

        public void AddMenuItem(MenuItem menuItem)
        {
            SelectedItems.Add(menuItem);
        }
    }
}
