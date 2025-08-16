using MapperDemo.Core;

namespace MapperDemo.Mappers
{
    /// <summary>
    /// Maps DIRS21 reservation models to Google reservation models.
    /// </summary>
    public class DIRS21ToGoogleReservationMapper : BaseMapper<Models.DIRS21.Reservation, Models.Google.Reservation>
    {
        public DIRS21ToGoogleReservationMapper() 
            : base("Model.Reservation", "Google.Reservation")
        {
        }

        public override Models.Google.Reservation Map(Models.DIRS21.Reservation source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var target = new Models.Google.Reservation
            {
                Id = source.Id.ToString(),
                CheckIn = source.CheckInDate?.ToString("yyyy-MM-dd"),
                CheckOut = source.CheckOutDate?.ToString("yyyy-MM-dd"),
                GuestName = $"{source.Guest?.FirstName} {source.Guest?.LastName}".Trim(),
                GuestEmail = source.Guest?.Email,
                GuestPhone = source.Guest?.Phone,
                TotalPrice = source.TotalAmount.ToString("F2"),
                ReservationStatus = MapReservationStatus(string.IsNullOrEmpty(source.Status) ? string.Empty : source.Status),
                Source = source.BookingChannel,
                CreateTime = source.CreatedAt.ToString("o"),
                UpdateTime = source.ModifiedAt?.ToString("o")
            };

            // Map guest address if available
            if (source.Guest != null)
            {
                target.Address = new Models.Google.GuestAddress
                {
                    StreetAddress = source.Guest.Address,
                    City = source.Guest.City,
                    CountryCode = source.Guest.Country
                };
            }

            // Map rooms
            foreach (var room in source.Rooms)
            {
                target.Rooms.Add(new Models.Google.RoomBooking
                {
                    RoomTypeId = room.RoomTypeId,
                    RoomCategory = room.RoomTypeName,
                    Quantity = room.Quantity,
                    NightlyRate = room.RatePerNight.ToString("F2"),
                    NumberOfAdults = room.Adults,
                    NumberOfChildren = room.Children
                });
            }

            return target;
        }

        private string MapReservationStatus(string dirs21Status)
        {
            // Map DIRS21 status values to Google's expected values
            return dirs21Status?.ToUpper() switch
            {
                "CONFIRMED" => "CONFIRMED",
                "CANCELED" => "CANCELLED", // Note the spelling difference
                "PENDING" => "PENDING",
                _ => "UNSPECIFIED"
            };
        }
    }
}