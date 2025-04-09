using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace ViewModel
{
    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        private readonly SynchronizationContext synchronizationContext;

        public AsyncObservableCollection()
        {
            synchronizationContext = SynchronizationContext.Current;
        }

        public AsyncObservableCollection(IEnumerable<T> list) : base(list)
        {
            synchronizationContext = SynchronizationContext.Current;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
        {
            if (SynchronizationContext.Current == synchronizationContext)
            {
                // Execute event on current thread
                RaiseCollectionChanged(eventArgs);
            }
            else
            {
                // Raise on creator thread
                synchronizationContext.Send(RaiseCollectionChanged, eventArgs);
            }
        }

        private void RaiseCollectionChanged(object param)
        {
            // In creator thread - call base implementation directly
            base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
        {
            if (SynchronizationContext.Current == synchronizationContext)
            {
                RaisePropertyChanged(eventArgs);
            }
            else
            {
                synchronizationContext.Send(RaisePropertyChanged, eventArgs);
            }
        }

        private void RaisePropertyChanged(object param)
        {
            base.OnPropertyChanged((PropertyChangedEventArgs)param);
        }

        public void AddRange(IEnumerable<T> elements)
        {
            foreach (T element in elements)
            {
                Add(element);
            }
        }
    }
}
