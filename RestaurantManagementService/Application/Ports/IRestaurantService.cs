using RestaurantManagementService.Domain.Aggregates;
using RestaurantManagementService.Domain.ValueObjects;

namespace RestaurantManagementService.Application.Ports
{
    public interface IRestaurantService
    {
        RestaurantAggregate CreateRestaurant(string name, Address address);
        void AddMenuItem(Guid restaurantId, MenuItem menuItem);
        void UpdateRestaurantDetails(Guid restaurantId, string newName, Address newAddress);
        RestaurantAggregate GetRestaurantById(Guid restaurantId);
        IEnumerable<RestaurantAggregate> GetRestaurantsByAddress(string city);
    }
}
