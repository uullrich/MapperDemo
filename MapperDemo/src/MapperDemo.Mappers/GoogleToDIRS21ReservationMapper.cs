using MapperDemo.Core;

namespace MapperDemo.Mappers
{
    /// <summary>
    /// Maps Google reservation models to DIRS21 reservation models.
    /// </summary>
    public class GoogleToDIRS21ReservationMapper : BaseMapper<Models.Google.Reservation, Models.DIRS21.Reservation>
    {
        public GoogleToDIRS21ReservationMapper() 
            : base("Google.Reservation", "Model.Reservation")
        {
        }

        public override Models.DIRS21.Reservation Map(Models.Google.Reservation source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var target = new Models.DIRS21.Reservation
            {
                // Parse ID or create a new one if invalid
                Id = Guid.TryParse(source.Id, out var id) ? id : Guid.NewGuid(),
                
                // Parse dates
                CheckInDate = string.IsNullOrEmpty(source.CheckIn) ? null : DateTime.Parse(source.CheckIn),
                CheckOutDate = string.IsNullOrEmpty(source.CheckOut) ? null : DateTime.Parse(source.CheckOut),
                
                // Parse total amount
                TotalAmount = decimal.TryParse(source.TotalPrice, out var total) ? total : 0m,
                
                // Map status
                Status = string.IsNullOrEmpty(source.ReservationStatus) ? null: MapReservationStatus(source.ReservationStatus),
                
                BookingChannel = source.Source ?? "Google",
                
                // Parse timestamps
                CreatedAt = string.IsNullOrEmpty(source.CreateTime) 
                    ? DateTime.UtcNow 
                    : DateTime.Parse(source.CreateTime),
                    
                ModifiedAt = string.IsNullOrEmpty(source.UpdateTime) 
                    ? null
                    : (DateTime?)DateTime.Parse(source.UpdateTime)
            };

            // Parse and create guest information
            target.Guest = new Models.DIRS21.Guest
            {
                // Parse name
                FirstName = string.IsNullOrEmpty(source.GuestName) ? null: ParseFirstName(source.GuestName),
                LastName = string.IsNullOrEmpty(source.GuestName) ? null : ParseLastName(source.GuestName),
                Email = source.GuestEmail,
                Phone = source.GuestPhone
            };
            
            // Add address if available
            if (source.Address != null)
            {
                target.Guest.Address = source.Address.StreetAddress;
                target.Guest.City = source.Address.City;
                target.Guest.Country = source.Address.CountryCode;
            }

            // Map rooms
            foreach (var room in source.Rooms)
            {
                target.Rooms.Add(new Models.DIRS21.RoomReservation
                {
                    RoomTypeId = room.RoomTypeId,
                    RoomTypeName = room.RoomCategory,
                    Quantity = room.Quantity,
                    RatePerNight = decimal.TryParse(room.NightlyRate, out var rate) ? rate : 0m,
                    Adults = room.NumberOfAdults,
                    Children = room.NumberOfChildren
                });
            }

            return target;
        }
        
        private string MapReservationStatus(string googleStatus)
        {
            // Map Google status values to DIRS21's expected values
            return googleStatus?.ToUpper() switch
            {
                "CONFIRMED" => "CONFIRMED",
                "CANCELLED" => "CANCELED", // Note the spelling difference
                "PENDING" => "PENDING",
                _ => "UNKNOWN"
            };
        }
        
        private string ParseFirstName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return string.Empty;
                
            var parts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[0] : string.Empty;
        }
        
        private string ParseLastName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return string.Empty;
                
            var parts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 1 ? string.Join(" ", parts, 1, parts.Length - 1) : string.Empty;
        }
    }
}