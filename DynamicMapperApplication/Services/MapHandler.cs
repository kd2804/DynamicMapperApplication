namespace DynamicMapperApplication.Services
{
    using DynamicMapperApplication.Models.Internal;
    using Newtonsoft.Json.Linq;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MapHandler
    {
        private readonly IDictionary<string, IMappingStrategy> _mappingStrategies;

        public MapHandler(IEnumerable<IMappingStrategy> strategies, IConfiguration configuration)
        {
            _mappingStrategies = new Dictionary<string, IMappingStrategy>();
            RegisterStrategies(strategies, configuration);
        }

        private void RegisterStrategies(IEnumerable<IMappingStrategy> strategies, IConfiguration configuration)
        {
            var mappingConfigurations = configuration.GetSection("MappingConfigurations").Get<List<MappingConfig>>();

            foreach (var config in mappingConfigurations)
            {
                // Dynamically find the strategy type by name
                var strategy = strategies.FirstOrDefault(s => s.GetType().Name == config.Strategy);
                if (strategy == null)
                {
                    throw new InvalidOperationException($"Strategy {config.Strategy} not found.");
                }

                //Register the mapping strategy for the provided source and target types
                RegisterStrategyMappings(config.SourceType, config.TargetType, strategy);
            }
        }

        private void RegisterStrategyMappings(string sourceType, string targetType, IMappingStrategy strategy)
        {
            var mappingKey = CreateMappingKey(sourceType, targetType);
            if (!_mappingStrategies.ContainsKey(mappingKey))
            {
                _mappingStrategies.Add(mappingKey, strategy);
            }
        }

        private string CreateMappingKey(string sourceType, string targetType)
        {
            return $"{sourceType}->{targetType}";
        }

        public object Map(JObject data, string sourceType, string targetType)
        {
            var mappingKey = CreateMappingKey(sourceType, targetType);
            if (!_mappingStrategies.TryGetValue(mappingKey, out var strategy))
            {
                throw new InvalidOperationException($"No mapping strategy found for {sourceType} -> {targetType}");
            }

            return sourceType.StartsWith("Model", StringComparison.OrdinalIgnoreCase)
                ? MapFromInternalToExternal(data, targetType, strategy)
                : MapFromExternalToInternal(data, sourceType, strategy);
        }

        private object MapFromInternalToExternal(JObject data, string targetType, IMappingStrategy strategy)
        {
            if (!IsReservationType(data))
            {
                throw new InvalidOperationException("The provided data does not match the expected internal reservation format.");
            }

            var reservation = data.ToObject<Reservation>();
            return strategy.ConvertToExternalFormat(reservation, targetType);
        }

        private object MapFromExternalToInternal(JObject data, string sourceType, IMappingStrategy strategy)
        {
            return strategy.ConvertToInternalModel(data, sourceType);
        }

        private bool IsReservationType(JObject obj)
        {
            var requiredProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "reservationId", "guestName", "checkIn", "checkOut", "rooms"
            };

            var actualProperties = obj.Properties().Select(p => p.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

            return requiredProperties.IsSubsetOf(actualProperties);
        }
    }
}