namespace RestaurantManagementService.Application.DTOs
{
    public class PubSubMessage
    {
        public Guid OrderId { get; set; }
        public DeliveryAddress DeliveryAddress { get; set; }
    }
}
