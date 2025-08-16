using MapperDemo.Mappers;
using MapperDemo.Models.Google;
using Xunit;

namespace MapperDemo.Tests
{
    public class GoogleToDIRS21ReservationMapperTests
    {
        private readonly GoogleToDIRS21ReservationMapper _mapper = new GoogleToDIRS21ReservationMapper();

        [Fact]
        public void Map_ValidReservation_MapsAllFields()
        {
            // Arrange
            var source = new Reservation
            {
                Id = Guid.NewGuid().ToString(),
                CheckIn = "2023-06-15",
                CheckOut = "2023-06-20",
                TotalPrice = "500.50",
                ReservationStatus = "CANCELLED", // maps to CANCELED
                Source = "Google Hotels",
                CreateTime = "2023-05-10T14:30:00Z",
                UpdateTime = null,
                GuestName = "John Smith",
                GuestEmail = "john.smith@example.com",
                GuestPhone = "123-456-7890",
                Address = new GuestAddress
                {
                    StreetAddress = "123 Main St",
                    City = "Berlin",
                    CountryCode = "DE"
                },
                Rooms = new List<RoomBooking>
                {
                    new RoomBooking
                    {
                        RoomTypeId = "DBL",
                        RoomCategory = "Double Room",
                        Quantity = 1,
                        NightlyRate = "100.10",
                        NumberOfAdults = 2,
                        NumberOfChildren = 0
                    }
                }
            };

            // Act
            var result = _mapper.Map(source);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Guid.Parse(source.Id), result.Id);
            Assert.Equal(new DateTime(2023, 6, 15), result.CheckInDate);
            Assert.Equal(new DateTime(2023, 6, 20), result.CheckOutDate);
            Assert.Equal(500.50m, result.TotalAmount);
            Assert.Equal("CANCELED", result.Status);
            Assert.Equal("Google Hotels", result.BookingChannel);

            Assert.NotNull(result.Guest);
            Assert.Equal("John", result.Guest!.FirstName);
            Assert.Equal("Smith", result.Guest.LastName);
            Assert.Equal("john.smith@example.com", result.Guest.Email);
            Assert.Equal("123-456-7890", result.Guest.Phone);

            Assert.Equal("123 Main St", result.Guest.Address);
            Assert.Equal("Berlin", result.Guest.City);
            Assert.Equal("DE", result.Guest.Country);

            Assert.Single(result.Rooms);
            Assert.Equal("DBL", result.Rooms[0].RoomTypeId);
            Assert.Equal("Double Room", result.Rooms[0].RoomTypeName);
            Assert.Equal(1, result.Rooms[0].Quantity);
            Assert.Equal(100.10m, result.Rooms[0].RatePerNight);
            Assert.Equal(2, result.Rooms[0].Adults);
            Assert.Equal(0, result.Rooms[0].Children);
        }

        [Fact]
        public void Map_NullReservation_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null!));
        }

        [Fact]
        public void Map_ReservationWithoutAddress_HandlesNullAddressGracefully()
        {
            // Arrange
            var source = new Reservation
            {
                Id = Guid.NewGuid().ToString(),
                CheckIn = "2023-06-15",
                CheckOut = "2023-06-20",
                TotalPrice = "200.00",
                ReservationStatus = "CONFIRMED",
                Source = null,
                CreateTime = "2023-05-10T14:30:00Z",
                UpdateTime = null,
                GuestName = null,
                GuestEmail = null,
                GuestPhone = null,
                Address = null,
                Rooms = new List<RoomBooking>()
            };

            // Act
            var result = _mapper.Map(source);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Guest);
            Assert.Null(result.Guest!.FirstName);
            Assert.Null(result.Guest.LastName);
            Assert.Null(result.Guest.Email);
            Assert.Null(result.Guest.Phone);

            Assert.Null(result.Guest.Address);
            Assert.Null(result.Guest.City);
            Assert.Null(result.Guest.Country);

            Assert.Equal("Google", result.BookingChannel);
        }
    }
}
