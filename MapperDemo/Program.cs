using Microsoft.Extensions.DependencyInjection;
using MapperDemo.Core;
using MapperDemo.Mappers;
using MapperDemo.Models.DIRS21;

namespace MapperDemo.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set up dependency injection
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddMappingSystem()
                .AddMapper<DIRS21ToGoogleReservationMapper>()
                .AddMapper<GoogleToDIRS21ReservationMapper>()
                .BuildServiceProvider();

            // Get the map handler from DI
            var mapHandler = serviceProvider.GetRequiredService<IMapHandler>();
            
            // Create a sample DIRS21 reservation
            var dirs21Reservation = CreateSampleReservation();
            
            // Map DIRS21 reservation to Google format
            var googleReservation = mapHandler.Map<Models.Google.Reservation>(
                dirs21Reservation, 
                "Model.Reservation", 
                "Google.Reservation");
                
            Console.WriteLine($"Successfully mapped DIRS21 Reservation {dirs21Reservation.Id} to Google Reservation {googleReservation.Id}");
            
            // Map Google reservation back to DIRS21 format
            var roundTripReservation = mapHandler.Map<Reservation>(
                googleReservation, 
                "Google.Reservation", 
                "Model.Reservation");
                
            Console.WriteLine($"Successfully mapped Google Reservation back to DIRS21 Reservation {roundTripReservation.Id}");
        }
        
        static Reservation CreateSampleReservation()
        {
            return new Reservation
            {
                Id = Guid.NewGuid(),
                CheckInDate = DateTime.Now.Date.AddDays(7),
                CheckOutDate = DateTime.Now.Date.AddDays(10),
                TotalAmount = 450.00m,
                Status = "CONFIRMED",
                BookingChannel = "Website",
                CreatedAt = DateTime.UtcNow,
                Guest = new Guest
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Phone = "+1-555-123-4567",
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
                        RatePerNight = 150.00m,
                        Adults = 2,
                        Children = 0
                    }
                }
            };
        }
    }
}