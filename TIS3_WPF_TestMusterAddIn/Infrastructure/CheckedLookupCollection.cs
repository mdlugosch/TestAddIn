using BFZ_WPF_Lib.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIS3_Base;

namespace TIS3_WPF_TestMusterAddIn.Infrastructure
{
    public class CheckedLookupCollection<T> : NotifyPropertyChangedCollectionBase<T> where T : ICheckable
    {
        public CheckedLookupCollection() : base()
        {
            this.CollectionChanged += items_CollectionChanged;
        }

        public CheckedLookupCollection(IEnumerable<T> collection) : base(collection) 
        {
            this.CollectionChanged += items_CollectionChanged;
            foreach (INotifyPropertyChanged item in this.Items)
                item.PropertyChanged += item_PropertyChanged;
        }
        /*
         * Beim entfernen und hinzufügen eines Items aus der ObservableCollection wird items_CollectionChanged
         * ausgeführt und je nachdem das PropertyChangedEvent des Items angehangen bzw. entfernt.
         */
        private void items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e != null)
            {
                if (e.OldItems != null)
                    foreach (INotifyPropertyChanged item in e.OldItems)
                        item.PropertyChanged -= item_PropertyChanged;

                if (e.NewItems != null)
                    foreach (INotifyPropertyChanged item in e.NewItems)
                        item.PropertyChanged += item_PropertyChanged;
            }
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                if (this.ItemCheckedChanged != null)
                {
                    this.ItemCheckedChanged((T)sender);
                }
            }
        }

        public event ItemCheckedDelegate ItemCheckedChanged;

        public delegate void ItemCheckedDelegate(T item);

        public IEnumerable<T> CheckedItems
        {
            get
            {
                return this.Items.Where(itm => itm.IsChecked == true);
            }
        }
    }
}
