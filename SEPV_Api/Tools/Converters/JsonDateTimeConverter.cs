using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gp_Api.Tools.Converters
{
    public class JsonDateTimeConverter : DateTimeConverterBase
    {
        private static IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            return dtConverter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var time = (DateTime)value;
            if (time.Hour == 0 && time.Minute == 0 && time.Second == 0) dtConverter.DateTimeFormat = "yyyy-MM-dd";
            else dtConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            dtConverter.WriteJson(writer, value, serializer);
        }
    }
}
