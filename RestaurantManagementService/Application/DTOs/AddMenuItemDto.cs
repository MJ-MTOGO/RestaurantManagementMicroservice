namespace RestaurantManagementService.Application.DTOs
{
    public class AddMenuItemDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Menu item name cannot be null or empty.", nameof(Name));

            if (Price <= 0)
                throw new ArgumentException("Price must be greater than zero.", nameof(Price));
        }
    }
}
