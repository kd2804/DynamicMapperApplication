using DynamicMapperApplication.Models.Internal;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json;

namespace DynamicMapperApplication
{
    public class BookingComExternalToInternalMappingRequestExample : IExamplesProvider<MappingRequest>
    {
        public MappingRequest GetExamples()
        {
            string bookingComJson = @"
            {
                    ""booking_id"": ""Booking786"",
                    ""customer_name"": ""Kiran Dubey"",
                    ""booking_start_date"": ""2023-12-01T14:00:00"",
                    ""booking_end_date"": ""2023-12-07T12:00:00"",
                    ""booking_amount_due"": 600.25,
                    ""booking_notes"": ""Non-smoking room"",
                    ""booking_source"": ""Booking.com"",
                    ""booking_room_details"": [
                        {
                            ""RoomNumber"": ""101"",
                            ""RoomType"": ""Suite"",
                            ""NumberOfBeds"": 1,
                            ""BedType"": ""Queen"",
                            ""IsSmokingAllowed"": false,
                            ""Price"": 600.25
                        }
                    ]
            }";

            JsonElement bookingComData = JsonSerializer.Deserialize<JsonElement>(bookingComJson);

            return new MappingRequest
            {
                SourceType = "Booking.Reservation",
                TargetType = "Model.Reservation",
                Data = bookingComData
            };
        }
    }
}