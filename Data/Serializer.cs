﻿namespace ClientData
{
    public abstract class Serializer
    {
        public abstract string Serialize<T>(T objToSerialize);
        public abstract T Deserialize<T>(string message);

        public abstract string? GetResponseHeader(string message);

        public static Serializer Create() { return new JsonSerializer(); }
    }
}
