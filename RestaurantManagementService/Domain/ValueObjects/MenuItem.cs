namespace RestaurantManagementService.Domain.ValueObjects
{
    public class MenuItem
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public Guid RestaurantId { get; private set; } // Foreign Key

        // Private parameterless constructor for EF Core
        private MenuItem() { }

        public MenuItem(Guid id, string name, decimal price, Guid restaurantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Menu item name cannot be empty.", nameof(name));

            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero.", nameof(price));

            Id = id;
            Name = name;
            Price = price;
            RestaurantId = restaurantId;
        }

        // Update menu item properties
        public void Update(string newName, decimal newPrice, Guid? restaurantId = null)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Menu item name cannot be empty.", nameof(newName));

            if (newPrice <= 0)
                throw new ArgumentException("Price must be greater than zero.", nameof(newPrice));

            Name = newName;
            Price = newPrice;

            if (restaurantId.HasValue)
            {
                RestaurantId = restaurantId.Value;
            }
        }
    }
}
