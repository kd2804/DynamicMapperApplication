using DynamicMapperApplication.Models.Internal;
using Newtonsoft.Json.Linq;

namespace DynamicMapperApplication.Services
{
    public interface IMappingStrategy
    {
        JObject ConvertToExternalFormat(Reservation reservation, string targetFormat);

        Reservation ConvertToInternalModel(JObject externalData, string sourceFormat);
    }
}