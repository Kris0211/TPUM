using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ClientData
{
    internal class JsonSerializer : Serializer
    {
        public override string Serialize<T>(T objToSerialize)
        {
            return JsonConvert.SerializeObject(objToSerialize);
        }

        public override T Deserialize<T>(string message)
        {
            return JsonConverter.DeseralizeObject<T>(message);
        }

        public override string? GetResponseHeader(string message)
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
