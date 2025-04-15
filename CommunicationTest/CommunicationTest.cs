using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientData;
using ServerPresentation;

namespace CommunicationTest
{
    [TestClass]
    public class CommunicationTest
    {
        [TestMethod]
        public async Task WebSocketUsageTestMethod()
        {
            WebSocketConnection _client = null;
            WebSocketConnection _server = null;
            const int delay = 10;

            //create server
            Uri uri = new Uri("ws://localhost:6669");
            List<string> logOutput = new List<string>();
            Task server = Task.Run(async () => await WebSocketServer.StartServer(uri.Port,
                _ws =>
                {
                    _server = _ws; _server.OnMessage = (data) =>
                    {
                        logOutput.Add($"Received message from client: {data}");
                    };
                }));
            await Task.Delay(delay); 
            

            _client = await WebSocketClient.Connect(uri, message => logOutput.Add(message));

            Assert.IsNotNull(_server);
            Assert.IsNotNull(_client);

            Task clientSendTask = _client.SendAsync("test");
            Assert.IsTrue(clientSendTask.Wait(new TimeSpan(0, 0, 1)));

            await Task.Delay(delay); 
            Assert.AreEqual($"Received message from client: test", logOutput[1]);

            _client.OnMessage = (data) =>
            {
                logOutput.Add($"Received message from server: {data}");
            };
            Task serverSendTask = _server.SendAsync("test 2");
            Assert.IsTrue(serverSendTask.Wait(new TimeSpan(0, 0, 1)));

            await Task.Delay(delay); 
            Assert.AreEqual($"Received message from server: test 2", logOutput[2]); //test correctness of the response

            await _client?.DisconnectAsync();
            await _server?.DisconnectAsync();
        }
    }
}
