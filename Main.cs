using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Clash;
using Autodesk.Navisworks.Api.DocumentParts;
using Autodesk.Navisworks.Api.Plugins;
using Application = Autodesk.Navisworks.Api.Application;

namespace CollisionControl
{
    [Plugin("BasicPlugIn.ABasicPlugin11",                   
                    "ADSK1",                                       
                    ToolTip = "BasicPlugIn.ABasicPlugin tool tip1",
                    DisplayName = "CollisionControl")]
    public class Main : AddInPlugin
    {
        public static MainView mainView = null;
        public override int Execute(params string[] parameters)
        {
            try
            {
                Debug.Listeners.Clear();
                string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                string name = Assembly.GetExecutingAssembly().GetName().Name;
                Debug.Listeners.Add(new RbsLogger.Logger($"{name}_{version}"));
                Debug.WriteLine("Старт");
                Data.DocMain = Application.MainDocument;
                Debug.WriteLine(Data.DocMain.CurrentFileName);
                if (mainView == null)
                {
                    Debug.WriteLine(" Start!!! Create!!!");
                    mainView = new MainView();
                    System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(mainView);
                    WindowInteropHelper helper = new WindowInteropHelper(mainView);
                    helper.Owner = Application.Gui.MainWindow.Handle;
                    mainView.Show();
                }
                else
                {
                    Debug.WriteLine(" Create!!! Activate!!!");
                    mainView.Activate();
                    mainView.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Debug.WriteLine(ex.ToString());
            }
            return 0;

        }
    }
}
