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

namespace DuOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Overlay overlay;
        KeyboardHook keyboardHook;

        bool test = true;
        public MainWindow()
        {
            InitializeComponent();

            //To close the process
            this.Closed += (sender, e) =>
            {
                if(keyboardHook!=null)
                    overlay.OnApplicationExit();
                
                this.Dispatcher.InvokeShutdown();
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (keyboardHook == null)
                keyboardHook = new KeyboardHook();

            if (overlay == null)
                overlay = new Overlay(keyboardHook);

            if (test)
            {
                overlay.Show();         
            }
            else
            {
                overlay.Hide();
            }

            test = !test;
        }
    }
}
