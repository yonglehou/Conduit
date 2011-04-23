using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Conduit
{
    public class OptimizedObservableCollection<t>
        : ObservableCollection<t>
    {
        private bool suppressOnCollectionChanged;

        public void AddRange(IEnumerable<t> items)
        {
            if (null == items)
            {
                throw new ArgumentNullException("items");
            }

            if (items.Any())
            {
                try
                {
                    suppressOnCollectionChanged = true;
                    foreach (var item in items)
                    {
                        Add(item);
                    }
                }
                finally
                {
                    suppressOnCollectionChanged = false;
                    OnCollectionChanged(
                        new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Reset));
                }
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!suppressOnCollectionChanged)
            {
                base.OnCollectionChanged(e);
            }
        }
    }
}
