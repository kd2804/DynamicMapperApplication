using System.Text.Json;

namespace DynamicMapperApplication.Models.Internal
{
    public class MappingRequest
    {
        public JsonElement Data { get; set; }
        public string SourceType { get; set; }
        public string TargetType { get; set; }
    }
}
