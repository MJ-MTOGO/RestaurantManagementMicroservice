using RestaurantManagementService.Application.DTOs;
using RestaurantManagementService.Application.Ports;

namespace RestaurantManagementService.Infrastructure.Publishers
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IMessageBus _messageBus;

        public MessagePublisher(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        //Ex. på en "task" fra interface 
        //public async Task PublishOrderCreatedAsync(Guid orderId, DeliveryAddress deliveryAddress)
        //{
        //    var message = new
        //    {
        //        OrderId = orderId,
        //        DeliveryAddress = new
        //        {
        //            deliveryAddress.Street,
        //            deliveryAddress.City,
        //            deliveryAddress.PostalCode
        //        }
        //    };

        //    await _messageBus.PublishAsync("OrderCreated", message);
        //}
    }
}
