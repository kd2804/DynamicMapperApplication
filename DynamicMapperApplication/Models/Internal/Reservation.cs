namespace DynamicMapperApplication.Models.Internal
{
    public class Reservation
    {
        public string ReservationId { get; set; }
        public string GuestName { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public List<Room> Rooms { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalAmount { get; set; }
        public string SpecialRequests { get; set; }
        public string BookingSource { get; set; }

        public Reservation()
        {
            Rooms = new List<Room>(); // Initialize the Rooms list
        }
    }
}
