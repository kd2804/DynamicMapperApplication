using DynamicMapperApplication.Models.Internal;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json;

namespace DynamicMapperApplication
{
    public class BookingComInternalToExternalMappingRequestExample : IExamplesProvider<MappingRequest>
    {
        public MappingRequest GetExamples()
        {
            string bookingComJson = @"
            {
                ""ReservationId"": ""Booking786"",
                ""GuestName"": ""Kiran Dubey"",
                ""CheckIn"": ""2023-12-01T14:00:00"",
                ""CheckOut"": ""2023-12-07T12:00:00"",
                ""Rooms"": [
                    {
                        ""RoomNumber"": ""101"",
                        ""RoomType"": ""Suite"",
                        ""NumberOfBeds"": 1,
                        ""BedType"": ""Queen"",
                        ""IsSmokingAllowed"": false,
                        ""Price"": 600.25
                    }
                ],
                ""NumberOfGuests"": 0,
                ""TotalAmount"": 600.25,
                ""SpecialRequests"": ""Non-smoking room"",
                ""BookingSource"": ""Booking.com""
    }";

            JsonElement bookingComData = JsonSerializer.Deserialize<JsonElement>(bookingComJson);

            return new MappingRequest
            {
                SourceType = "Model.Reservation",
                TargetType = "Booking.Reservation",
                Data = bookingComData
            };
        }
    }
}