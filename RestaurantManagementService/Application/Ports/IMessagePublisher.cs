using RestaurantManagementService.Application.DTOs;

namespace RestaurantManagementService.Application.Ports
{
    public interface IMessagePublisher
    {
        // Lav her det vi ønsker at publish
        Task PublishReadyToPickupAsync(ReadyToPickup readyToPickup);
    }
}
    