using Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

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

        public ViewModel()
        {
            this.model = new Model.Model(null);
            model.ReputationChanged += HandleReputationChanged;

            CurrentTab = TabEnum.All;
            Items = new ObservableCollection<ItemPresentation>(model.depotPresentation.GetItems());

            OnAllCommand = new RelayCommand(() => HandleOnAllButton());
            OnAvailableCommand = new RelayCommand(() => HandleOnAvailableButton());
            OnWeaponCommand = new RelayCommand(() => HandleOnWeaponButton());
            OnAmmoCommand = new RelayCommand(() => HandleOnAmmoButton());
            OnGeneratorCommand = new RelayCommand(() => HandleOnGeneratorButton());
            OnSpaceshipCommand = new RelayCommand(() => HandleOnSpaceshipButton());

            OnItemCommand = new RelayCommand<Guid>((id) => HandleOnItemButton(id));
        }

        private void HandleReputationChanged(object sender, ModelReputationChangedEventArgs args)
        {
            ReputationString = $"Reputation multiplier: {args.NewReputation}";
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
            model.SellItem(id);
            ReloadItems();
        }

        private void ReloadItems()
        {
            items.Clear();

            if (CurrentTab == TabEnum.All)
            {
                foreach (ItemPresentation item in model.depotPresentation.GetItems())
                {
                    items.Add(item);
                }
            }
            else if (CurrentTab == TabEnum.Available)
            {
                foreach (ItemPresentation item in model.depotPresentation.GetAvailableItems())
                {
                    items.Add(item);
                }
            }
            else
            {
                foreach (ItemPresentation item in model.depotPresentation.GetItemsByType((PresentationItemType)CurrentTab))
                {
                    items.Add(item);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new  PropertyChangedEventArgs(propertyName));
        }
    }
}
