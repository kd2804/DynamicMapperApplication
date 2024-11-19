using DynamicMapperApplication.Models.Internal;
using DynamicMapperApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.Swagger.Annotations;

namespace DynamicMapperApplication
{
    /// <summary>
    /// Controller for handling dynamic mappings between different models.
    /// </summary>
    [ApiController]
    [Route("api/mapping")]
    public class MappingController : ControllerBase
    {
        private readonly MapHandler _mapHandler;

        public MappingController(MapHandler mapHandler)
        {
            _mapHandler = mapHandler ?? throw new ArgumentNullException(nameof(mapHandler), "MapHandler cannot be null.");
        }

        /// <summary>
        /// Maps data from the source type to the target type using the appropriate mapping strategy.
        /// </summary>
        /// <param name="mappingRequest">The mapping request containing data, source type, and target type.</param>
        /// <returns>A JSON response containing the mapped data or an error message.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [SwaggerRequestExample(typeof(MappingRequest), typeof(GoogleInternalToExternalMappingRequestExample))]
        [SwaggerRequestExample(typeof(MappingRequest), typeof(GoogleExternalToInternalMappingRequestExample))]
        [SwaggerRequestExample(typeof(MappingRequest), typeof(BookingComInternalToExternalMappingRequestExample))]
        [SwaggerRequestExample(typeof(MappingRequest), typeof(BookingComExternalToInternalMappingRequestExample))]
        public IActionResult Map([FromBody] MappingRequest mappingRequest)
        {
            if (mappingRequest == null)
            {
                return BadRequest(new { error = "Request body cannot be null." });
            }

            try
            {
                string inputJsonString = mappingRequest.Data.ToString();
                JObject inputData = JObject.Parse(inputJsonString);

                var mappedResult = _mapHandler.Map(inputData, mappingRequest.SourceType, mappingRequest.TargetType);
                string responseJson = JsonConvert.SerializeObject(mappedResult);

                return Ok(responseJson);
            }
            catch (JsonReaderException jsonEx)
            {
                return BadRequest(new { error = "Invalid JSON format.", details = jsonEx.Message });
            }
            catch (InvalidOperationException invalidOpEx)
            {
                return BadRequest(new { error = "Mapping error.", details = invalidOpEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
