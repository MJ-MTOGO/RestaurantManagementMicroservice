using RestaurantManagementService.Application.Ports;
using RestaurantManagementService.Domain.Aggregates;
using RestaurantManagementService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantManagementService.Infrastructure.Adapters.Repositories
{
    public class RestaurantRepository : IDatabaseRepository
    {
        private readonly RestaurantManagementServiceDbContext _context;

        public RestaurantRepository(RestaurantManagementServiceDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Save(RestaurantAggregate restaurant)
        {
            if (restaurant == null)
                throw new ArgumentNullException(nameof(restaurant));

            var existingRestaurant = _context.Restaurants
                .Include(r => r.MenuItems)
                .FirstOrDefault(r => r.RestaurantId == restaurant.RestaurantId);

            if (existingRestaurant == null)
            {
                _context.Restaurants.Add(restaurant);
            }
            else
            {
                // Update restaurant details
                _context.Entry(existingRestaurant).CurrentValues.SetValues(restaurant);

                // Sync MenuItems
                SyncMenuItems(existingRestaurant, restaurant.MenuItems);
            }

            _context.SaveChanges();
        }

        public RestaurantAggregate GetById(Guid restaurantId)
        {
            return _context.Restaurants
                .Include(r => r.MenuItems)
                .FirstOrDefault(r => r.RestaurantId == restaurantId);
        }

        public void AddMenuItem(Guid restaurantId, MenuItem menuItem)
        {
            var restaurant = _context.Restaurants
                .Include(r => r.MenuItems)
                .FirstOrDefault(r => r.RestaurantId == restaurantId);

            if (restaurant == null)
                throw new InvalidOperationException("Restaurant not found.");

            // Check if the MenuItem already exists
            var existingMenuItem = _context.MenuItems.FirstOrDefault(m => m.Id == menuItem.Id);
            if (existingMenuItem != null)
            {
                // Update existing menu item
                existingMenuItem.Update(menuItem.Name, menuItem.Price);
            }
            else
            {
                // Add new menu item
                restaurant.AddMenuItem(menuItem); // Use aggregate method
                _context.MenuItems.Add(menuItem); // Explicitly track new menu item
            }

            _context.SaveChanges();
        }

        public IEnumerable<RestaurantAggregate> FindByCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be null or empty.", nameof(city));

            return _context.Restaurants
                .Include(r => r.MenuItems)
                .Where(r => r.Address.City.ToLower() == city.ToLower())
                .ToList();
        }

        private void SyncMenuItems(RestaurantAggregate existingRestaurant, IReadOnlyCollection<MenuItem> newMenuItems)
        {
            // Add or update menu items
            foreach (var menuItem in newMenuItems)
            {
                var existingMenuItem = existingRestaurant.MenuItems.FirstOrDefault(m => m.Id == menuItem.Id);
                if (existingMenuItem == null)
                {
                    existingRestaurant.AddMenuItem(menuItem);
                }
                else
                {
                    existingMenuItem.Update(menuItem.Name, menuItem.Price);
                }
            }

            // Remove menu items that no longer exist in the updated list
            var removedMenuItems = existingRestaurant.MenuItems
                .Where(m => !newMenuItems.Any(updated => updated.Id == m.Id))
                .ToList();

            foreach (var removed in removedMenuItems)
            {
                existingRestaurant.RemoveMenuItem(removed.Id);
            }
        }
    }
}
