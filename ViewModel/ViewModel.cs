using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Model;

namespace ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        public enum TabEnum
        {
            Weapon = 0,
            Ammo = 1,
            Generator = 2,
            Spaceship = 3,
            All = 4,
            Available = 5
        }

        private TabEnum currentTab;
        public TabEnum CurrentTab
        {
            get => currentTab;

            private set
            {
                if (currentTab != value)
                {
                    currentTab = value;
                    OnPropertyChanged();
                }
            }
        }

        private Model.Model model;

        private ObservableCollection<ItemPresentation> items;
        public ObservableCollection<ItemPresentation> Items
        {
            get => items;

            private set
            {
                if (items != value)
                {
                    items = value;
                    OnPropertyChanged("Items");
                }
            }
        }

        private string reputationString;
        public string ReputationString
        {
            get => reputationString;

            private set
            {
                if (reputationString != value)
                {
                    reputationString = value;
                    OnPropertyChanged();
                }
            }
        }

        private string connectionString;
        public string ConnectionString
        {
            get => connectionString;
            private set
            {
                if (connectionString != value)
                {
                    connectionString = value;
                    OnPropertyChanged();
                }
            }
        }

        private string transactionString;
        public string TransactionString
        {
            get => transactionString;
            private set
            {
                if (transactionString != value)
                {
                    transactionString = value;
                    OnPropertyChanged();
                }
            }
        }

        public ViewModel()
        {
            model = new Model.Model(null);
            model.ModelConnectionService.OnConnectionStateChanged += OnConnectionStateChanged;
            model.ModelConnectionService.OnError += OnConnectionStateChanged;

            model.ModelConnectionService.Logger += Log;
            model.ModelConnectionService.OnMessage += OnMessage;

            model.DepotPresentation.ReputationChanged += HandleReputationChanged;
            model.DepotPresentation.OnItemsUpdated += HandleOnItemsUpdated;
            model.DepotPresentation.TransactionFinished += HandleTransactionFinish;

            OnConnectionStateChanged();

            CurrentTab = TabEnum.All;
            Items = new AsyncObservableCollection<ItemPresentation>();
            foreach (ItemPresentation item in model.DepotPresentation.GetItems())
            {
                Items.Add(item);
            }

            reputationString = "Reputation: 1.0";
            transactionString = "Transaction: Ready";

            OnAllCommand = new RelayCommand(() => HandleOnAllButton());
            OnAvailableCommand = new RelayCommand(() => HandleOnAvailableButton());
            OnWeaponCommand = new RelayCommand(() => HandleOnWeaponButton());
            OnAmmoCommand = new RelayCommand(() => HandleOnAmmoButton());
            OnGeneratorCommand = new RelayCommand(() => HandleOnGeneratorButton());
            OnSpaceshipCommand = new RelayCommand(() => HandleOnSpaceshipButton());

            OnItemCommand = new RelayCommand<Guid>((id) => HandleOnItemButton(id));
        }

        public async Task CloseConnection()
        {
            if (model.ModelConnectionService.IsConnected())
            {
                await model.ModelConnectionService.Disconnect();
            }
        }

        private void HandleTransactionFinish(bool succeeded)
        {
            string time = DateTime.Now.ToLongTimeString();
            TransactionString = succeeded ?
                $"Transaction finished succesfully! ({time})"
                : $"Transaction failed! ({time})";
        }

        private void Log(string message)
        {
            Console.WriteLine(message);
        }

        private void OnMessage(string message)
        {
            Log($"New Message: {message}");
        }

        private void OnConnectionStateChanged()
        {
            bool actualState = model.ModelConnectionService.IsConnected();
            ConnectionString = actualState ? "Connected" : "Disconnected";

            if (!actualState)
            {
                Task.Run(() => model.ModelConnectionService.Connect(new Uri(@"ws://localhost:11634")));
            }
            else
            {
                model.DepotPresentation.RequestUpdate();
            }
        }

        private void HandleReputationChanged(object sender, ModelReputationChangedEventArgs args)
        {
            ReputationString = $"Reputation multiplier: {args.NewReputation}";
            ReloadItems();
        }

        private void HandleOnItemsUpdated()
        {
            ReloadItems();
        }

        public ICommand OnAllCommand { get; private set; }
        private void HandleOnAllButton()
        {
            CurrentTab = TabEnum.All;
            ReloadItems();
        }
        
        public ICommand OnAvailableCommand { get; private set; }
        private void HandleOnAvailableButton()
        {
            CurrentTab = TabEnum.Available;
            ReloadItems();
        }
        
        public ICommand OnWeaponCommand { get; private set; }
        private void HandleOnWeaponButton()
        {
            CurrentTab = TabEnum.Weapon;
            ReloadItems();
        }
        
        public ICommand OnAmmoCommand { get; private set; }
        private void HandleOnAmmoButton()
        {
            CurrentTab = TabEnum.Ammo;
            ReloadItems();
        }
        
        public ICommand OnGeneratorCommand { get; private set; }
        private void HandleOnGeneratorButton()
        {
            CurrentTab = TabEnum.Generator;
            ReloadItems();
        }
        
        public ICommand OnSpaceshipCommand { get; private set; }
        private void HandleOnSpaceshipButton()
        {
            CurrentTab = TabEnum.Spaceship;
            ReloadItems();
        }

        public ICommand OnItemCommand { get; private set; }
        private void HandleOnItemButton(Guid id)
        {
            Task.Run(async () => await model.SellItem(id));
        }

        private void ReloadItems()
        {
            items.Clear();

            if (CurrentTab == TabEnum.All)
            {
                foreach (ItemPresentation item in model.DepotPresentation.GetItems())
                {
                    items.Add(item);
                }
            }
            else if (CurrentTab == TabEnum.Available)
            {
                foreach (ItemPresentation item in model.DepotPresentation.GetAvailableItems())
                {
                    items.Add(item);
                }
            }
            else
            {
                foreach (ItemPresentation item in model.DepotPresentation.GetItemsByType((PresentationItemType)CurrentTab))
                {
                    items.Add(item);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
