using System.Net;
using RestaurantManagementService.Domain.ValueObjects;


namespace RestaurantManagementService.Domain.Entities
{
    public class Restaurant
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Address Address { get; private set; }
        public List<MenuItem> MenuItems { get; private set; }

        public Restaurant(Guid id, string name, Address address)
        {
            Id = id;
            Name = name;
            Address = address;
            MenuItems = new List<MenuItem>();
        }

        public void AddMenuItem(MenuItem menuItem)
        {
            MenuItems.Add(menuItem);
        }
    }
}
