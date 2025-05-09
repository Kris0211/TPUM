﻿using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClientApi;
using ServerPresentation;

namespace ClientData
{
    public class WebSocketClient
    {
        public static async Task<WebSocketConnection> Connect(Uri uri, Action<string>? logger)
        {
            ClientWebSocket clientWebSocket = new ClientWebSocket();
            await clientWebSocket.ConnectAsync(uri, CancellationToken.None);
            switch (clientWebSocket.State)
            {
                case WebSocketState.Open:
                    logger?.Invoke($"Opening WebSocket connection to remote server {uri}");
                    WebSocketConnection socket = new ClientWebSocketConnection(clientWebSocket, uri, logger);
                    return socket;
                default:
                    logger?.Invoke($"Cannot connect to remote: status {clientWebSocket.State}");
                    throw new WebSocketException($"Cannot connect to remote: status {clientWebSocket.State}");
            }
        }

        private class ClientWebSocketConnection : WebSocketConnection
        {
            private readonly ClientWebSocket clientWebSocket;
            private readonly Action<string> log;
            private readonly Uri uri;

            public ClientWebSocketConnection(ClientWebSocket clientWebSocket, Uri uri, Action<string> log)
            {
                this.clientWebSocket = clientWebSocket;
                this.uri = uri;
                this.log = log;
                Task.Factory.StartNew(ClientMessageLoop);
            }

            protected override Task SendTask(string message)
            {
                return clientWebSocket.SendAsync(message.GetArraySegment(), 
                    WebSocketMessageType.Text, true, CancellationToken.None);
            }

            public override Task DisconnectAsync()
            {
                return clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, 
                    "Starting shutdown procedure", CancellationToken.None);
            }

            public override string ToString()
            {
                return uri.ToString();
            }

            private void ClientMessageLoop()
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    while (true)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
                        WebSocketReceiveResult result = clientWebSocket.ReceiveAsync(segment, CancellationToken.None).Result;
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            OnClose?.Invoke();
                            clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                                "Closing connection", CancellationToken.None).Wait();
                            return;
                        }

                        int count = result.Count;
                        while (!result.EndOfMessage)
                        {
                            if (count >= buffer.Length)
                            {
                                OnClose?.Invoke();
                                clientWebSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData,
                                    "Buffer length exceeded", CancellationToken.None)
                                    .Wait();
                                return;
                            }

                            segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                            result = clientWebSocket.ReceiveAsync(segment, CancellationToken.None).Result;
                            count += result.Count;
                        }

                        string message = Encoding.UTF8.GetString(buffer, 0, count);
                        OnMessage?.Invoke(message);
                    }
                }
                catch (Exception ex)
                {
                    log($"Connection broken. An exception occured: {ex}");
                    clientWebSocket.CloseAsync(WebSocketCloseStatus.InternalServerError,
                        "Connection broken. An exception occured.",
                        CancellationToken.None).Wait();
                }
            }
        }
    }
}
