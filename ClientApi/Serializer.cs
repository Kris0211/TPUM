﻿namespace ClientData
{
    public abstract class Serializer
    {
        public abstract string Serialize<T>(T objectToSerialize);
        public abstract T Deserialize<T>(string message);

        public abstract string? GetResponseHeader(string message);
        public abstract string? GetCommandHeader(string message);

        public static Serializer Create()
        {
            return new JsonDataSerializer();
        }
    }
}
