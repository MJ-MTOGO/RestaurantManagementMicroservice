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

        public async Task PublishReadyToPickupAsync(ReadyToPickup readyToPickup)
        {
            var message = new
            {
                readyToPickup = new
                {
                    orderId = readyToPickup.OrderId,
                }
            };
            await _messageBus.PublishAsync("ready-to-pickup", message);

        }
    }
}
