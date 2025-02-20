using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace CollisionControl
{
    /// <summary>
    /// Класс для хранения настроек
    /// </summary>
    public class PlaginSettings
    {
        /// <summary>
        /// Список профилей в файле настроек
        /// </summary>
        public ObservableCollection<Profile> Profiles { get; set; } = new ObservableCollection<Profile>();
    }

    /// <summary>
    /// Профиль
    /// </summary>
    public class Profile : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged(nameof(Name)); }
        }
        public List<Set> Sets1 { get; set; }
        public List<Set> Sets2 { get; set; }
        public List<Test> Tests { get; set; }
        public Profile()
        {

        }
        public Profile(string name, List<WrapSavedItem> sets1, List<WrapSavedItem> sets2, List<WrapClashTest> tests)
        {
            sets1 = sets1.Where(x => x.IsSelected).ToList();
            sets2 = sets2.Where(x => x.IsSelected).ToList();
            tests = tests.Where(x => x.IsSelected).ToList();
            Name = name;
            Sets1 = new List<Set>();
            foreach (WrapSavedItem setWrap in sets1)
            {
                Set set = new Set();
                set.Name = setWrap.DisplayName;
                set.Guid = setWrap.Set.Guid;
                Sets1.Add(set);
            }
            Sets2 = new List<Set>();
            foreach (WrapSavedItem setWrap in sets2)
            {
                Set set = new Set();
                set.Name = setWrap.DisplayName;
                set.Guid = setWrap.Set.Guid;
                Sets2.Add(set);
            }
            Tests = new List<Test>();
            foreach (WrapClashTest clashtest in tests)
            {
                Test test = new Test();
                test.Name = clashtest.DisplayName;
                Tests.Add(test);
            }
        }
      
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    
    /// <summary>
    /// Класс для хранения "наборов поиска"
    /// </summary>
    public class Set
    {
        public string Name { get; set; }
        public Guid Guid { get; set; }
    }

    /// <summary>
    /// Класс для хранения "проверок"
    /// </summary>
    public class Test
    {
        public string Name { get; set; }

    }
}
