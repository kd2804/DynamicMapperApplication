
# Dynamic Mapping Application

Dynamic Mapping Application is a tool built on .NET 7 that helps transform data between different formats or models. It makes it easy to add new partners and allows you to set up mapping rules using a configuration file, giving you flexibility and control.

---

## Features

- **Dynamic Mapping**: Enables data transformation between different external and internal data formats.
- **Configuration Driven**: Mappings and strategies are defined in the configuration file for flexibility.
- **Easily Extendable**: Add new partners with minimal code changes.
- **Swagger Integration**: API documentation and example requests are accessible via Swagger.

---

## System Architecture

The system is built on the following architectural components:

### Key Classes

1. **`MapHandler`**
   - Central class responsible for handling the mapping logic.
   - Dynamically resolves the correct `IMappingStrategy` for a given source and target type.
   - Provides methods for:
     - Mapping from internal to external formats.
     - Mapping from external to internal formats.

2. **`IMappingStrategy` Interface**
   - Defines the contract for mapping strategies.
   - Key methods:
     - `ConvertToExternalFormat`: Converts an internal model to an external format.
     - `ConvertToInternalModel`: Converts an external format to an internal model.

3. **Concrete Mapping Strategies**
   - Example: `GoogleMappingStrategy`, `BookingComMappingStrategy`.
   - Implements the `IMappingStrategy` interface for specific partners.

4. **`MappingRequest`**
   - Represents the mapping request payload.
   - Contains:
     - `Data` (type: `JsonElement`) - The input data to be mapped.
     - `SourceType` - The type of the source model.
     - `TargetType` - The type of the target model.

5. **Controller**
   - The `MappingController` processes mapping requests.
   - Parses `JsonElement` to `JObject` for flexible handling of JSON data.
   - Handles exceptions and provides meaningful error responses.

### Configuration

The application uses a configuration-driven approach for defining mappings, stored in `appsettings.json` under the `MappingConfigurations` section:

```json
"MappingConfigurations": [
  {
    "SourceType": "Model.Reservation",
    "TargetType": "Google.Reservation",
    "Strategy": "GoogleMappingStrategy"
  },
  {
    "SourceType": "Google.Reservation",
    "TargetType": "Model.Reservation",
    "Strategy": "GoogleMappingStrategy"
  },
  {
    "SourceType": "Model.Reservation",
    "TargetType": "Booking.Reservation",
    "Strategy": "BookingComMappingStrategy"
  },
  {
    "SourceType": "Booking.Reservation",
    "TargetType": "Model.Reservation",
    "Strategy": "BookingComMappingStrategy"
  }
]
```

### Data Flow

- **Incoming Request**: A JSON payload is received with the source type, target type, and data.
- **Mapping Resolution**: `MapHandler` identifies the appropriate mapping strategy based on the configuration.
- **Mapping Execution**: The selected strategy converts the data from the source format to the target format.
- **Response**: The mapped data is returned as a JSON response.
---

## Assumptions and Potential Limitations

### Assumptions

1. **Consistent Data Structure**  
   - The incoming JSON data is assumed to follow a consistent structure for each source type. Any deviation may result in mapping errors.

2. **Handling of Data with `JsonElement`**  
   - `JObject` was not directly accepting JSON input in certain scenarios. To address this, `JsonElement` was used in `MappingRequest`, and it is parsed to `JObject` in the controller for flexible handling.

3. **Configuration-Driven Approach**  
   - All mapping rules are defined in the configuration file (`appsettings.json`). Correct configurations are essential for smooth operation.

4. **Dependency Injection**  
   - Mapping strategies are resolved using dependency injection. All strategies must be properly registered in the DI container.

5. **Two-Way Mapping Support**  
   - Mapping strategies are expected to support conversions in both directions.

6. **JSON Format for Data Exchange**  
   - All incoming and outgoing data is expected to be in JSON format.

### Limitations

1. **Performance Impact of Parsing**  
   - Using `JsonElement` and converting it to `JObject` introduces minor performance overhead.

2. **Dependency on JSON Format**  
   - The solution is tightly coupled with JSON as the data exchange format.


---
## How the Mapping Works

### Case Study: `sourceType: Booking.Reservation` to `targetType: Model.Reservation`

When a request is made with the `sourceType` as `Booking.Reservation` and the `targetType` as `Model.Reservation`, the following process takes place:

1. **Configuration Lookup**:  
   The application reads the `MappingConfigurations` section in the configuration file. It matches the entry with:
   ```json
   {
     "SourceType": "Booking.Reservation",
     "TargetType": "Model.Reservation",
     "Strategy": "BookingComMappingStrategy"
   }
   ```
2. **Mapping Strategy Selection**:  
   Based on the `Strategy` field, the application identifies `BookingComMappingStrategy` as the strategy to use for this mapping.

3. **Mapping Execution**:  
   - The `MapHandler` invokes the `ConvertToInternalModel` method of `BookingComMappingStrategy`.  
   - The JSON input is processed to extract values and transform them into the internal `Model.Reservation` format.  
   - For example:
     ```json
     {
       "booking_id": "Booking786",
       "customer_name": "Kiran Dubey",
       "booking_start_date": "2023-12-01T14:00:00",
       "booking_end_date": "2023-12-07T12:00:00",
       "booking_amount_due": 600.25,
       "booking_notes": "Non-smoking room",
       "rooms": [ ... ]
     }
     ```
     is mapped into:
     ```json
     {
       "reservationId": "Booking786",
       "guestName": "Kiran Dubey",
       "checkIn": "2023-12-01T14:00:00",
       "checkOut": "2023-12-07T12:00:00",
       "totalAmount": 600.25,
       "specialRequests": "Non-smoking room",
       "rooms": [ ... ]
     }
     ```
4. **Result**:  
   The transformed `Model.Reservation` object is returned as the response.

---

## Adding a New Partner (e.g., Airbnb)

Follow these steps to integrate a new partner like Airbnb:

### Step 1: Define the Mapping Strategy
Implement a new strategy class, `AirbnbMappingStrategy`, that inherits from `IMappingStrategy`. Ensure you handle the `ConvertToInternalModel` and `ConvertToExternalFormat` methods to transform data between Airbnb's format and the internal model.

### Step 2: Update Configuration
Add entries in the `MappingConfigurations` section for the new partner:
```json
{
  "SourceType": "Airbnb.Reservation",
  "TargetType": "Model.Reservation",
  "Strategy": "AirbnbMappingStrategy"
},
{
  "SourceType": "Model.Reservation",
  "TargetType": "Airbnb.Reservation",
  "Strategy": "AirbnbMappingStrategy"
}
```

### Step 3: Register the Strategy
Ensure `AirbnbMappingStrategy` is registered in the `Startup` or `Program` file:
```csharp
builder.Services.AddScoped<IMappingStrategy, AirbnbMappingStrategy>();
```

### Step 4: Test the Integration
Use example requests for Airbnb mappings to verify the functionality.

---

## Example Request Format

### Google Reservation to Internal Model
```json
{
    "data": {
        "reservation_id": "Google786",
        "guest_name": "Kiran Dubey",
        "check_in_date": "2023-12-01T14:00:00",
        "check_out_date": "2023-12-07T12:00:00",
        "total_amount": 600.25,
        "special_requests": "Non-smoking room",
        "booking_source": "Google.com",
        "rooms": [
            {
                "RoomNumber": "101",
                "RoomType": "Suite",
                "NumberOfBeds": 1,
                "BedType": "Queen",
                "IsSmokingAllowed": false,
                "Price": 600.25
            }
        ]
    },
    "sourceType": "Google.Reservation",
    "targetType": "Model.Reservation"
}
```

### Booking Reservation to Internal Model
```json
{
    "data": {
        "booking_id": "Booking786",
        "customer_name": "Kiran Dubey",
        "booking_start_date": "2023-12-01T14:00:00",
        "booking_end_date": "2023-12-07T12:00:00",
        "booking_amount_due": 600.25,
        "booking_notes": "Non-smoking room",
        "booking_source": "Booking.com",
        "booking_room_details": [
            {
                "RoomNumber": "101",
                "RoomType": "Suite",
                "NumberOfBeds": 1,
                "BedType": "Queen",
                "IsSmokingAllowed": false,
                "Price": 600.25
            }
        ]
    },
    "sourceType": "Booking.Reservation",
    "targetType": "Model.Reservation"
}
```
