using Newtonsoft.Json;
using SFML.System;
using System;
using System.Globalization;

namespace TheForestWaiter.Game.Essentials
{
    internal class Vector2fValueConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.IsAssignableTo(typeof(Vector2f));
        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var parts = reader.Value.ToString().Split(',');
            return new Vector2f(
                float.Parse(parts[0], CultureInfo.InvariantCulture), 
                float.Parse(parts[1], CultureInfo.InvariantCulture)
                );
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}