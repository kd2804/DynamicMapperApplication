namespace DynamicMapperApplication.Models.Internal
{
    public class Room
    {
        public string RoomNumber { get; set; } 
        public string RoomType { get; set; }
        public int NumberOfBeds { get; set; }
        public string BedType { get; set; }
        public bool IsSmokingAllowed { get; set; }
        public decimal Price { get; set; }
    }
}
