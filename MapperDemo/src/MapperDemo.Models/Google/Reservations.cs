namespace MapperDemo.Models.Google
{
    public class Reservation
    {
        public string? Id { get; set; }
        public string? CheckIn { get; set; }
        public string? CheckOut { get; set; }
        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestPhone { get; set; }
        public GuestAddress? Address { get; set; }
        public List<RoomBooking> Rooms { get; set; } = new List<RoomBooking>();
        public string? TotalPrice { get; set; }
        public string? ReservationStatus { get; set; }
        public string? Source { get; set; }
        public string? CreateTime { get; set; }
        public string? UpdateTime { get; set; }
    }

    public class GuestAddress
    {
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? CountryCode { get; set; }
    }

    public class RoomBooking
    {
        public string? RoomTypeId { get; set; }
        public string? RoomCategory { get; set; }
        public int Quantity { get; set; }
        public string? NightlyRate { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
    }
}