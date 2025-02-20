using Autodesk.Navisworks.Api;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollisionControl
{
    public class WrapSavedItem : INotifyPropertyChanged
    {
        private SavedItem savedItem;

       
        public SavedItem SavedItem { get { return savedItem; } }

        public event PropertyChangedEventHandler PropertyChanged;

        public ModelItemCollection Collection { get; set; }

        public SelectionSet Set { get { return (savedItem as SelectionSet); } }

        public ObservableCollection<WrapSavedItem> Nodes { get; set; } = new ObservableCollection<WrapSavedItem>();

        public string DisplayName { get { return savedItem.DisplayName; } }

        public bool IsGroup { get { return savedItem is FolderItem; } }


        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; NotifyPropertyChanged(nameof(IsSelected)); }
        }

        public WrapSavedItem(SavedItem item)
        {
            savedItem = item;
            IsSelected = false;
            SetNodes();
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IEnumerable<WrapSavedItem> GetSelectNodes()
        {
            foreach (var child in Nodes)
            {
                if (child.IsSelected)
                {
                    yield return child;
                }

                if (IsGroup)
                {
                    foreach (var descendant in child.GetSelectNodes())
                    {
                        yield return descendant;
                    }
                }
            }
        }

        public IEnumerable<WrapSavedItem> GetNodes()
        {
            foreach (var child in Nodes)
            {
                yield return child;

                if (IsGroup)
                {
                    foreach (var descendant in child.GetNodes())
                    {
                        yield return descendant;
                    }
                }
            }
        }


        private void SetNodes()
        {
            if (IsGroup)
            {
                FolderItem group = savedItem as FolderItem;

                foreach (var item in group.Children)
                {
                    Nodes.Add(new WrapSavedItem(item));
                }

            }
        }

    }
}
