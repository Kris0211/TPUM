using System;
using System.Threading.Tasks;

namespace ClientApi
{
    public abstract class WebSocketConnection
    {
        public virtual Action<string>? OnMessage { protected get; set; } = x => { };
        public virtual Action? OnClose { protected get; set; } = () => { };
        public virtual Action? OnError { protected get; set; } = () => { };

        public async Task SendAsync(string message)
        {
            await SendTask(message);
        }

        public abstract Task DisconnectAsync();

        protected abstract Task SendTask(string message);
    }
}
