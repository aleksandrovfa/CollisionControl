using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для SettingNameView.xaml
    /// </summary>
    public partial class SettingNameView : Window
    {
        public string Name { get; set; }
        public SettingNameView(string name = null, string title = null)
        {
            InitializeComponent();
            if (name != null)
            {
                this.NameProfile.Text = name;
            }
            if (title != null)
            {
                this.Title = title;
            }
            this.NameProfile.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Name = this.NameProfile.Text;
            DialogResult = true;
            this.Close();
        }
    }
}
