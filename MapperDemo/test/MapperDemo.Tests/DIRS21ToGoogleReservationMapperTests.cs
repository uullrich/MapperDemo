using MapperDemo.Mappers;
using MapperDemo.Models.DIRS21;
using Xunit;

namespace MapperDemo.Tests
{
    public class DIRS21ToGoogleReservationMapperTests
    {
        private readonly DIRS21ToGoogleReservationMapper _mapper = new DIRS21ToGoogleReservationMapper();

        [Fact]
        public void Map_ValidReservation_MapsAllFields()
        {
            // Arrange
            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                CheckInDate = new DateTime(2023, 6, 15),
                CheckOutDate = new DateTime(2023, 6, 20),
                Status = "CONFIRMED",
                TotalAmount = 500.50m,
                BookingChannel = "Website",
                CreatedAt = new DateTime(2023, 5, 10, 14, 30, 0),
                Guest = new Guest
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "john.smith@example.com",
                    Phone = "123-456-7890",
                    Address = "123 Main St",
                    City = "Berlin",
                    Country = "DE"
                },
                Rooms = new List<RoomReservation>
                {
                    new RoomReservation
                    {
                        RoomTypeId = "DBL",
                        RoomTypeName = "Double Room",
                        Quantity = 1,
                        RatePerNight = 100.10m,
                        Adults = 2,
                        Children = 0
                    }
                }
            };

            // Act
            var result = _mapper.Map(reservation);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reservation.Id.ToString(), result.Id);
            Assert.Equal("2023-06-15", result.CheckIn);
            Assert.Equal("2023-06-20", result.CheckOut);
            Assert.Equal("John Smith", result.GuestName);
            Assert.Equal("john.smith@example.com", result.GuestEmail);
            Assert.Equal("123-456-7890", result.GuestPhone);
            Assert.Equal("500.50", result.TotalPrice);
            Assert.Equal("CONFIRMED", result.ReservationStatus);

            // Check address
            Assert.NotNull(result.Address);
            Assert.Equal("123 Main St", result.Address.StreetAddress);
            Assert.Equal("Berlin", result.Address.City);
            Assert.Equal("DE", result.Address.CountryCode);

            // Check rooms
            Assert.Single(result.Rooms);
            Assert.Equal("DBL", result.Rooms[0].RoomTypeId);
            Assert.Equal("Double Room", result.Rooms[0].RoomCategory);
            Assert.Equal(1, result.Rooms[0].Quantity);
            Assert.Equal("100.10", result.Rooms[0].NightlyRate);
            Assert.Equal(2, result.Rooms[0].NumberOfAdults);
            Assert.Equal(0, result.Rooms[0].NumberOfChildren);
        }

        [Fact]
        public void Map_NullReservation_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null!));
        }

        [Fact]
        public void Map_ReservationWithoutGuest_HandlesNullGuestGracefully()
        {
            // Arrange
            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                CheckInDate = new DateTime(2023, 6, 15),
                CheckOutDate = new DateTime(2023, 6, 20),
                Guest = null,
                Rooms = new List<RoomReservation>()
            };

            // Act
            var result = _mapper.Map(reservation);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.GuestName);
            Assert.Null(result.GuestEmail);
            Assert.Null(result.GuestPhone);
            Assert.Null(result.Address);
        }
    }
}