using DynamicMapperApplication.Models.Internal;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace DynamicMapperApplication.Services
{
    public class GoogleMappingStrategy : IMappingStrategy
    {
        private const string ExpectedSourceType = "Google.Reservation";
        private const string ExpectedTargetType = "Google.Reservation";
        private const string GoogleSource = "Google.com";

        public Reservation ConvertToInternalModel(JObject data, string sourceType)
        {
            ValidateSourceType(sourceType);

            try
            {
                return new Reservation
                {
                    ReservationId = ExtractStringValue(data, "reservation_id"),
                    GuestName = ExtractStringValue(data, "guest_name"),
                    CheckIn = ExtractDateValue(data, "check_in_date"),
                    CheckOut = ExtractDateValue(data, "check_out_date"),
                    TotalAmount = ExtractDecimalValue(data, "total_amount"),
                    SpecialRequests = data["special_requests"]?.ToString(),
                    BookingSource = GoogleSource,
                    Rooms = data["rooms"]?.ToObject<List<Room>>() ?? new List<Room>()
                };
            }
            catch (Exception ex)
            {
                throw new MappingException("An error occurred while converting Google reservation data to the internal model.", ex);
            }
        }

        public JObject ConvertToExternalFormat(Reservation reservation, string targetType)
        {
            ValidateTargetType(targetType);

            try
            {
                var externalData = new JObject
                {
                    ["reservation_id"] = reservation.ReservationId ?? throw new MappingException("Missing Reservation ID."),
                    ["guest_name"] = reservation.GuestName ?? throw new MappingException("Guest name cannot be null."),
                    ["check_in_date"] = reservation.CheckIn.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                    ["check_out_date"] = reservation.CheckOut.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                    ["total_amount"] = reservation.TotalAmount,
                    ["special_requests"] = reservation.SpecialRequests,
                    ["booking_source"] = GoogleSource,
                    ["rooms"] = JArray.FromObject(reservation.Rooms ?? new List<Room>())
                };

                return externalData;
            }
            catch (Exception ex)
            {
                throw new MappingException("Error while converting internal reservation data to Google format.", ex);
            }
        }

        private static void ValidateSourceType(string sourceType)
        {
            if (!string.Equals(sourceType, ExpectedSourceType, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Invalid source type. Expected: {ExpectedSourceType}, Found: {sourceType}");
            }
        }

        private static void ValidateTargetType(string targetType)
        {
            if (!string.Equals(targetType, ExpectedTargetType, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Invalid target type. Expected: {ExpectedTargetType}, Found: {targetType}");
            }
        }

        private static string ExtractStringValue(JObject data, string key)
        {
            var value = data[key]?.ToString();
            if (string.IsNullOrEmpty(value))
            {
                throw new MappingException($"The field '{key}' is missing or empty.");
            }
            return value;
        }

        private static DateTime ExtractDateValue(JObject data, string key)
        {
            var dateValue = data[key]?.ToString();
            if (DateTime.TryParse(dateValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                return parsedDate;
            }
            throw new MappingException($"Invalid or missing date for '{key}'.");
        }

        private static decimal ExtractDecimalValue(JObject data, string key)
        {
            var decimalValue = data[key]?.ToString();
            if (decimal.TryParse(decimalValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedDecimal))
            {
                return parsedDecimal;
            }
            throw new MappingException($"Invalid or missing decimal value for '{key}'.");
        }
    }
}