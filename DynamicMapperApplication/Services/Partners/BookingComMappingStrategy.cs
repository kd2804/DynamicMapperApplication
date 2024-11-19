using DynamicMapperApplication.Models.Internal;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace DynamicMapperApplication.Services
{
    public class BookingComMappingStrategy : IMappingStrategy
    {
        private const string BookingSource = "Booking.com";
        private const string ExpectedSourceType = "Booking.Reservation";
        private const string ExpectedTargetType = "Booking.Reservation";

        public Reservation ConvertToInternalModel(JObject data, string sourceType)
        {
            ValidateSourceType(sourceType);

            try
            {
                return new Reservation
                {
                    ReservationId = GetStringValue(data, "booking_id"),
                    GuestName = GetStringValue(data, "customer_name"),
                    CheckIn = GetDateValue(data, "booking_start_date"),
                    CheckOut = GetDateValue(data, "booking_end_date"),
                    TotalAmount = GetDecimalValue(data, "booking_amount_due"),
                    SpecialRequests = data["booking_notes"]?.ToString(),
                    BookingSource = BookingSource,
                    Rooms = data["booking_room_details"]?.ToObject<List<Room>>() ?? new List<Room>()
                };
            }
            catch (Exception ex)
            {
                throw new MappingException("Error mapping Booking.com data to internal reservation.", ex);
            }
        }

        public JObject ConvertToExternalFormat(Reservation reservation, string targetType)
        {
            ValidateTargetType(targetType);

            try
            {
                var externalData = new JObject
                {
                    ["booking_id"] = reservation.ReservationId ?? throw new ArgumentNullException("ReservationId", "Reservation ID is required."),
                    ["customer_name"] = reservation.GuestName ?? throw new ArgumentNullException("GuestName", "Guest name is required."),
                    ["booking_start_date"] = reservation.CheckIn.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                    ["booking_end_date"] = reservation.CheckOut.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                    ["booking_amount_due"] = reservation.TotalAmount,
                    ["booking_notes"] = reservation.SpecialRequests,
                    ["booking_source"] = BookingSource,
                    ["booking_room_details"] = JArray.FromObject(reservation.Rooms ?? new List<Room>())
                };

                return externalData;
            }
            catch (Exception ex)
            {
                throw new MappingException("Error mapping internal reservation data to Booking.com format.", ex);
            }
        }

        private static void ValidateSourceType(string sourceType)
        {
            if (sourceType != ExpectedSourceType)
            {
                throw new InvalidOperationException($"Invalid source type for Booking mapping. Expected: {ExpectedSourceType}, Found: {sourceType}");
            }
        }

        private static void ValidateTargetType(string targetType)
        {
            if (targetType != ExpectedTargetType)
            {
                throw new InvalidOperationException($"Invalid target type for Booking mapping. Expected: {ExpectedTargetType}, Found: {targetType}");
            }
        }

        private static string GetStringValue(JObject data, string key)
        {
            return data[key]?.ToString() ?? throw new ArgumentNullException(key, $"{key} is required.");
        }

        private static DateTime GetDateValue(JObject data, string key)
        {
            if (DateTime.TryParse(data[key]?.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue))
            {
                return dateValue;
            }

            throw new ArgumentException($"Invalid date format for {key}.", key);
        }

        private static decimal GetDecimalValue(JObject data, string key)
        {
            if (decimal.TryParse(data[key]?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
            {
                return decimalValue;
            }

            throw new ArgumentException($"Invalid decimal format for {key}.", key);
        }
    }
}