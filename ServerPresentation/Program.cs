using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientApi;
using ServerLogic;

namespace ServerPresentation
{
    internal class Program
    {
        private readonly LogicAbstractApi logicAbstractApi;

        private WebSocketConnection? webSocketConnection;

        private Program(LogicAbstractApi logicAbstractApi)
        {
            this.logicAbstractApi = logicAbstractApi;
            logicAbstractApi.GetStore().ReputationChanged += HandleReputationChanged;
        }

        private async Task StartConnection()
        {
            while (true)
            {
                Console.WriteLine("Waiting for client...");
                await WebSocketServer.StartServer(11634, OnConnect);
            }
        }

        private void OnConnect(WebSocketConnection connection)
        {
            Console.WriteLine($"Connected to {connection}");

            connection.OnMessage = OnMessage;
            connection.OnError = OnError;
            connection.OnClose = OnClose;

            webSocketConnection = connection;
        }

        private async void OnMessage(string message)
        {
            if (webSocketConnection == null)
                return;

            Console.WriteLine($"New message: {message}");

            Serializer serializer = Serializer.Create();
            if (serializer.GetCommandHeader(message) == GetItemsCommand.StaticHeader)
            {
                GetItemsCommand getItemsCommand = serializer.Deserialize<GetItemsCommand>(message);
                Task task = Task.Run(async () => await SendItems());
            }
            else if (serializer.GetCommandHeader(message) == SellItemCommand.StaticHeader)
            {
                SellItemCommand sellItemCommand = serializer.Deserialize<SellItemCommand>(message);

                TransactionResponse transactionResponse = new TransactionResponse();
                transactionResponse.TransactionId = sellItemCommand.TransactionID;
                try
                {
                    logicAbstractApi.GetStore().SellItem(sellItemCommand.ItemID);
                    transactionResponse.Succeeded = true;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Exception \"{exception.Message}\" caught when selling item");
                    transactionResponse.Succeeded = false;
                }

                string transactionMessage = serializer.Serialize(transactionResponse);
                Console.WriteLine($"Send: {transactionMessage}");
                await webSocketConnection.SendAsync(transactionMessage);
            }
        }

        private async Task SendItems()
        {
            if (webSocketConnection == null)
                return;

            Console.WriteLine($"Sending items...");

            UpdateAllResponse serverResponse = new UpdateAllResponse();
            List<IStoreItem> items = logicAbstractApi.GetStore().GetItems();
            serverResponse.Items = new ItemDTO[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                serverResponse.Items[i] = items[i].ToDTO();
            }

            Serializer serializer = Serializer.Create();
            string responseJson = serializer.Serialize(serverResponse);
            Console.WriteLine(responseJson);

            await webSocketConnection.SendAsync(responseJson);
        }

        private async void HandleReputationChanged(object? sender, LogicReputationChangedEventArgs args)
        {
            if (webSocketConnection == null)
                return;

            Console.WriteLine($"New reputation: {args.NewReputation}");

            List<IStoreItem> items = logicAbstractApi.GetStore().GetItems();
            ReputationChangedResponse reputationChangedResponse = new ReputationChangedResponse();
            reputationChangedResponse.NewReputation = args.NewReputation;
            reputationChangedResponse.NewPrices = new NewPriceDTO[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                IStoreItem item = items[i];
                reputationChangedResponse.NewPrices[i] = new NewPriceDTO(item.Id, item.Price);
            }

            Serializer serializer = Serializer.Create();
            string responseJson = serializer.Serialize(reputationChangedResponse);
            Console.WriteLine(responseJson);

            await webSocketConnection.SendAsync(responseJson);
        }

        private void OnError()
        {
            Console.WriteLine($"Connection error");
        }

        private void OnClose()
        {
            Console.WriteLine($"Connection closed");
            webSocketConnection = null;
        }

        private static async Task Main(string[] args)
        {
            Program program = new Program(LogicAbstractApi.Create());
            await program.StartConnection();
        }
    }
}
