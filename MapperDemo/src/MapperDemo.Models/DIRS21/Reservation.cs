namespace MapperDemo.Models.DIRS21
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public Guest? Guest { get; set; }
        public List<RoomReservation> Rooms { get; set; } = new List<RoomReservation>();
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
        public string? BookingChannel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    public class Guest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    public class RoomReservation
    {
        public string? RoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }
        public int Quantity { get; set; }
        public decimal RatePerNight { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
    }
}