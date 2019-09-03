using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace inCRM
{
    public class ObservableList<T> : ObservableCollection<T>, IEnumerable<T>
    {
        public ObservableList() : base()
        {

        }

        public ObservableList(List<T> l) : base(EmptyOnNull(l))
        {
            
        }

        private static List<T> EmptyOnNull(List<T> p)
        {
            if (p is null)
                return new List<T>();
            else
                return p;
        }

        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

    }
}
