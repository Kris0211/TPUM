using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using ClientApi;

namespace ServerPresentation
{
    internal static class WebSocketServer
    {
        private const int HTTP_BAD_REQUEST = 400;

        public static async Task StartServer(int port, Action<WebSocketConnection> onConnection)
        {
            Uri uri = new Uri($@"http://localhost:{port}/");
            await ServerLoop(uri, onConnection);
        }

        private static async Task ServerLoop(Uri uri, Action<WebSocketConnection> onConnection)
        {
            HttpListener server = new HttpListener();
            server.Prefixes.Add(uri.ToString());
            server.Start();
            while (true)
            {
                HttpListenerContext httpContext = await server.GetContextAsync();
                if (!httpContext.Request.IsWebSocketRequest)
                {
                    httpContext.Response.StatusCode = HTTP_BAD_REQUEST;
                    httpContext.Response.Close();
                }

                HttpListenerWebSocketContext context = await httpContext.AcceptWebSocketAsync(null);
                WebSocketConnection connection = new ServerWebSocketConnection(context.WebSocket, 
                    httpContext.Request.RemoteEndPoint);
                onConnection?.Invoke(connection);
            }

        }

        private class ServerWebSocketConnection : WebSocketConnection
        {
            private readonly IPEndPoint endPoint;
            private readonly WebSocket socket;

            public ServerWebSocketConnection(WebSocket socket, IPEndPoint endPoint)
            {
                this.socket = socket;
                this.endPoint = endPoint;
                Task.Factory.StartNew(() => ServerMessageLoop(socket));
            }

            public override string ToString()
            {
                return endPoint.ToString();
            }

            private void ServerMessageLoop(WebSocket ws)
            {
                byte[] buffer = new byte[1024];
                while (true)
                {
                    ArraySegment<byte> segments = new ArraySegment<byte>(buffer);
                    WebSocketReceiveResult receiveResult = 
                        ws.ReceiveAsync(segments, CancellationToken.None).Result;
                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        OnClose?.Invoke();
                        ws.CloseAsync(WebSocketCloseStatus.NormalClosure, 
                            "Closing connection", CancellationToken.None);
                        return;
                    }

                    int count = receiveResult.Count;
                    while (!receiveResult.EndOfMessage)
                    {
                        if (count >= buffer.Length)
                        {
                            OnClose?.Invoke();
                            ws.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, 
                                "Buffer length exceeded", CancellationToken.None);
                            return;
                        }

                        segments = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                        receiveResult = ws.ReceiveAsync(segments, CancellationToken.None).Result;
                        count += receiveResult.Count;
                    }

                    string message = Encoding.UTF8.GetString(buffer, 0, count);
                    OnMessage?.Invoke(message);
                }
            }

            protected override Task SendTask(string message)
            {
                return socket.SendAsync(message.GetArraySegment(), 
                    WebSocketMessageType.Text, true, CancellationToken.None);
            }

            public override Task DisconnectAsync()
            {
                return socket.CloseAsync(WebSocketCloseStatus.NormalClosure, 
                    "Shutdown procedure started", CancellationToken.None);
            }
        }
    }
}
