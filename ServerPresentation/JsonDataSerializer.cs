using System.Text.Json;

namespace ServerPresentation
{
    internal class JsonDataSerializer : Serializer
    {
        public override string Serialize<T>(T objectToSerialize)
        {
            return JsonSerializer.Serialize(objectToSerialize);
        }

        public override T Deserialize<T>(string message)
        {
            return JsonSerializer.Deserialize<T>(message);
        }

        public override string? GetCommandHeader(string message)
        {
            using JsonDocument doc = JsonDocument.Parse(message);
            if (doc.RootElement.TryGetProperty("Header", out JsonElement header))
            {
                return header.GetString();
            }

            return null;
        }
    }
}
