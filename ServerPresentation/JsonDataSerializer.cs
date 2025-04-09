﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ServerPresentation
{
    internal class JsonDataSerializer : Serializer
    {
        public override string Serialize<T>(T objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize);
        }

        public override T Deserialize<T>(string message)
        {
            return JsonConvert.DeserializeObject<T>(message);
        }

        public override string? GetCommandHeader(string message)
        {
            JObject jObject = JObject.Parse(message);
            if (jObject.ContainsKey("Header"))
            {
                return (string)jObject["Header"];
            }

            return null;
        }
    }
}
