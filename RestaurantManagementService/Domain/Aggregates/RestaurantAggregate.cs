using RestaurantManagementService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantManagementService.Domain.Aggregates
{
    public class RestaurantAggregate
    {
        private readonly List<MenuItem> _menuItems; // Internal list for menu items

        public Guid RestaurantId { get; private set; } // Unique identifier for the restaurant
        public string Name { get; private set; } // Restaurant name
        public Address Address { get; private set; } // Restaurant address (Value Object)

        // EF Core requires a parameterless constructor for materialization
        private RestaurantAggregate()
        {
            _menuItems = new List<MenuItem>();
        }

        // Constructor
        public RestaurantAggregate(Guid id, string name, Address address)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Restaurant name cannot be empty.", nameof(name));

            if (address == null)
                throw new ArgumentNullException(nameof(address), "Address cannot be null.");

            RestaurantId = id;
            Name = name;
            Address = address;
            _menuItems = new List<MenuItem>();
        }

        // Read-only access to menu items
        public IReadOnlyCollection<MenuItem> MenuItems => _menuItems.AsReadOnly();

        // Factory method for creating a new restaurant
        public static RestaurantAggregate CreateNewRestaurant(Guid id, string name, Address address)
        {
            return new RestaurantAggregate(id, name, address);
        }

        // Add a new menu item
        public void AddMenuItem(MenuItem menuItem)
        {
            if (menuItem == null)
                throw new ArgumentNullException(nameof(menuItem), "Menu item cannot be null.");

            if (_menuItems.Any(m => m.Id == menuItem.Id))
                throw new InvalidOperationException($"A menu item with the ID '{menuItem.Id}' already exists.");

            if (_menuItems.Any(m => m.Name.Equals(menuItem.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"A menu item with the name '{menuItem.Name}' already exists.");

            _menuItems.Add(menuItem);
        }

        // Remove a menu item
        public void RemoveMenuItem(Guid menuItemId)
        {
            var menuItem = _menuItems.FirstOrDefault(m => m.Id == menuItemId);

            if (menuItem == null)
                throw new InvalidOperationException("The menu item does not exist.");

            _menuItems.Remove(menuItem);
        }

        // Update restaurant details (name and address)
        public void UpdateRestaurantDetails(string newName, Address newAddress)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("New restaurant name cannot be empty.", nameof(newName));

            if (newAddress == null)
                throw new ArgumentNullException(nameof(newAddress), "New address cannot be null.");

            Name = newName;
            Address = newAddress;
        }

        // Update an existing menu item
        public void UpdateMenuItem(Guid menuItemId, string newName, decimal newPrice)
        {
            var menuItem = _menuItems.FirstOrDefault(m => m.Id == menuItemId);

            if (menuItem == null)
                throw new InvalidOperationException("The menu item does not exist.");

            menuItem.Update(newName, newPrice);
        }
    }
}
