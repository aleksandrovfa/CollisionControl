using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            //VirtualizingPanel.SetIsVirtualizing(this.listCheck, false);
            Debug.WriteLine("InitializeComponent MainView");
            MainViewModel vm = new MainViewModel();
            this.Closed += MainView_Closed;
            //vm.CloseRequest += (s, e) => this.Close();
            DataContext = vm;
            this.Title = this.Title + $" (ver.{Assembly.GetExecutingAssembly().GetName().Version.ToString()})";
        }

        private void MainView_Closed(object sender, EventArgs e)
        {
            Main.mainView = null;
            try
            {
                Data.DocActive.Models.ResetAllHidden();
                Data.DocActive.ActiveView.FocusOnCurrentSelection();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
           
        }
    }
}
