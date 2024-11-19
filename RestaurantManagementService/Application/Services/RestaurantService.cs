using RestaurantManagementService.Application.Ports;
using RestaurantManagementService.Domain.Aggregates;
using RestaurantManagementService.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace RestaurantManagementService.Application.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IDatabaseRepository _repository;

        public RestaurantService(IDatabaseRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public RestaurantAggregate CreateRestaurant(string name, Address address)
        {
            var restaurant = RestaurantAggregate.CreateNewRestaurant(Guid.NewGuid(), name, address);
            _repository.Save(restaurant);
            return restaurant;
        }

        public void AddMenuItem(Guid restaurantId, MenuItem menuItem)
        {
            _repository.AddMenuItem(restaurantId, menuItem);
        }

        public void UpdateRestaurantDetails(Guid restaurantId, string newName, Address newAddress)
        {
            var restaurant = _repository.GetById(restaurantId);
            if (restaurant == null)
                throw new InvalidOperationException("Restaurant not found.");

            restaurant.UpdateRestaurantDetails(newName, newAddress);
            _repository.Save(restaurant);
        }

        public RestaurantAggregate GetRestaurantById(Guid restaurantId)
        {
            return _repository.GetById(restaurantId);
        }

        public IEnumerable<RestaurantAggregate> GetRestaurantsByAddress(string city)
        {
            return _repository.FindByCity(city);
        }
    }
}
