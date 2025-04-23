using ClientData;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using ServerPresentation;
using ClientApi;

namespace ACommunicationTest
{
    
    [TestClass]
    public class CommunicationTest
    {
        internal JSchema LoadSchema()
        {
            string schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "schema.json");
            string schemaJson = File.ReadAllText(schemaPath);
            return JSchema.Parse(schemaJson);
        }

        [TestMethod]
        public void JsonSchemaTest()
        {
            ItemDTO item = new ItemDTO
            (
                Guid.NewGuid(),
                "Item A",
                "Description A",
                "Weapon",
                12.5f,
                false
            );

            string json = JsonConvert.SerializeObject(item);
            JObject jObject = JObject.Parse(json);

            JSchema schema = LoadSchema();
            bool isValid = jObject.IsValid(schema, out IList<string> errors);

            Assert.IsTrue(isValid, $"JSON is invalid:\n{string.Join("\n", errors)}");
        }

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
            Assert.AreEqual($"Received message from server: test 2", logOutput[2]);

            await _client?.DisconnectAsync();
            await _server?.DisconnectAsync();
        }
    }
}
