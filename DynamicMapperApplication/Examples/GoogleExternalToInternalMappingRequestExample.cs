using DynamicMapperApplication.Models.Internal;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json;

namespace DynamicMapperApplication
{
    public class GoogleExternalToInternalMappingRequestExample : IExamplesProvider<MappingRequest>
    {
        public MappingRequest GetExamples()
        {
            string googleJson = @"
            {
                    ""reservation_id"": ""Google786"",
                    ""guest_name"": ""Kiran Dubey"",
                    ""check_in_date"": ""2023-12-01T14:00:00"",
                    ""check_out_date"": ""2023-12-07T12:00:00"",
                    ""total_amount"": 600.25,
                    ""special_requests"": ""Non-smoking room"",
                    ""booking_source"": ""Google.com"",
                    ""rooms"": [
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

            JsonElement googleData = JsonSerializer.Deserialize<JsonElement>(googleJson);

            return new MappingRequest
            {
                SourceType = "Google.Reservation",
                TargetType = "Model.Reservation",
                Data = googleData
            };
        }
    }
}