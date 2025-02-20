using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CollisionControl
{
    /// <summary>
    /// Логика взаимодействия для SettingView.xaml
    /// </summary>
    public partial class SettingsView : Window
    {
        public SettingsView(List<WrapSavedItem> set1, List<WrapSavedItem> set2, List<WrapClashTest> tests, PlaginSettings settings)
        {
            InitializeComponent();
            Debug.WriteLine("InitializeComponent SettingView");
            SettingViewModel vm = new SettingViewModel(set1,set2,tests, settings);
            vm.CloseRequest += (s, e) => this.Close();
            DataContext = vm;
        }
    }
}
