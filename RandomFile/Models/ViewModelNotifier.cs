using System.Collections.Specialized;
using System.ComponentModel;

namespace RandomFile.Models
{
    public class ViewModelNotifier : INotifyPropertyChanged, INotifyCollectionChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnNotifyCollectionChanged(NotifyCollectionChangedAction collectionAction)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(collectionAction));
        }
    }
}
