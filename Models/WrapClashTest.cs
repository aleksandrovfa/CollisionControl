using Autodesk.Navisworks.Api.Clash;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CollisionControl
{
    public class WrapClashTest : INotifyPropertyChanged
    {
        private ClashTest _clashTest;

        public event PropertyChangedEventHandler PropertyChanged;

        public ClashTest ClashTest { get { return _clashTest; } }
        public string  DisplayName { get { return _clashTest.DisplayName; } }


        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; NotifyPropertyChanged(nameof(IsSelected)); }
        }
        public WrapClashTest(ClashTest clashTest)
        {
            _clashTest = clashTest;
            IsSelected = false;
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}