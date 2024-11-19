using RestaurantManagementService.Domain.Aggregates;
using RestaurantManagementService.Domain.ValueObjects;

namespace RestaurantManagementService.Application.Ports
{
    public interface IDatabaseRepository
    {
        void Save(RestaurantAggregate restaurant);
        RestaurantAggregate GetById(Guid restaurantId);
        void AddMenuItem(Guid restaurantId, MenuItem menuItem);
        IEnumerable<RestaurantAggregate> FindByCity(string city);
    }
}
