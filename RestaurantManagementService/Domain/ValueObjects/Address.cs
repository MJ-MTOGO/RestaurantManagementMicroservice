namespace RestaurantManagementService.Domain.ValueObjects
{
    public class Address
    {
        public string Street { get; private set; }
        public string City { get; private set; }
        public string PostalCode { get; private set; }

        public Address(string street, string city, string postalCode)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("Street cannot be empty.", nameof(street));

            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be empty.", nameof(city));

            if (string.IsNullOrWhiteSpace(postalCode))
                throw new ArgumentException("Postal code cannot be empty.", nameof(postalCode));

            Street = street;
            City = city;
            PostalCode = postalCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Address other) return false;

            return Street == other.Street &&
                   City == other.City &&
                   PostalCode == other.PostalCode;
        }

        public override int GetHashCode() => HashCode.Combine(Street, City, PostalCode);

        public override string ToString() => $"{Street}, {City}, {PostalCode}";
    }
}
