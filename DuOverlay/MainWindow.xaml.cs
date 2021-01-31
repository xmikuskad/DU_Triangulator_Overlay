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
        Solver solver;
        ImageProcessing imageProcessing;

        const string POS_PLACEHOLDER = "::pos{0,a,x,y,z}";
        const string DIST_PLACEHOLDER = "200.11...";

        bool test = true;
        public MainWindow()
        {
            InitializeComponent();

            solver = new Solver();
            imageProcessing = new ImageProcessing();

            clearAllFields(null,null);

            //To close the process
            this.Closed += (sender, e) =>
            {
                if(keyboardHook!=null)
                    overlay.OnApplicationExit();
                
                this.Dispatcher.InvokeShutdown();
            };
        }

        public void removePosPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text.Equals(POS_PLACEHOLDER))
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }

        public void addPosPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = POS_PLACEHOLDER;
                tb.Foreground = Brushes.Gray;
            }
        }

        public void removeDistPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text.Equals(DIST_PLACEHOLDER))
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }

        public void addDistPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = DIST_PLACEHOLDER;
                tb.Foreground = Brushes.Gray;
            }
        }

        private void activateOverlay(object sender, RoutedEventArgs e)
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

        public void testBtn(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            MessageBox.Show(btn.Tag.ToString());
        }

        public void calculateDistanceImage(object sender, RoutedEventArgs e)
        {

            if (Clipboard.ContainsImage())
            {
                BitmapSource image = Clipboard.GetImage();

                var btn = sender as Button;
                String code = btn.Tag.ToString();

                if (code.Equals("1"))
                {
                    dist1.Text = imageProcessing.GetOreDistance(image).ToString();
                }
                if (code.Equals("2"))
                {
                    dist2.Text = imageProcessing.GetOreDistance(image).ToString();
                }
                if (code.Equals("3"))
                {
                    dist3.Text = imageProcessing.GetOreDistance(image).ToString();
                }
                if (code.Equals("4"))
                {
                    dist4.Text = imageProcessing.GetOreDistance(image).ToString();
                }
            }
            else
            {
                MessageBox.Show("No screenshot in clipboard!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void calculateOrePos(object sender, RoutedEventArgs e)
        {
            List<String> varList = new List<String>();
            varList.Add(field1.Text);
            varList.Add(dist1.Text);
            varList.Add(field2.Text);
            varList.Add(dist1.Text);
            varList.Add(field3.Text);
            varList.Add(dist1.Text);
            varList.Add(field4.Text);
            varList.Add(dist1.Text);

            solver.startSolve(varList, this);
        }

        public void clearField(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            String code = btn.Tag.ToString();

            if (code.Equals("1"))
            {
                resetField(field1, dist1);
            }
            if (code.Equals("2"))
            {
                resetField(field2, dist2);
            }
            if (code.Equals("3"))
            {
                resetField(field3, dist3);
            }
            if (code.Equals("4"))
            {
                resetField(field4, dist4);
            }
        }

        public void resetField(TextBox field, TextBox dist)
        {
            field.Text = "";
            field.Text = POS_PLACEHOLDER;
            field.Foreground = Brushes.Gray;

            dist.Text = "";
            dist.Text = DIST_PLACEHOLDER;
            dist.Foreground = Brushes.Gray;
        }

        public void clearAllFields(object sender, RoutedEventArgs e)
        {
            resetField(field1, dist1);
            resetField(field2, dist2);
            resetField(field3, dist3);
            resetField(field4, dist4);
        }

        public void hideClipMsg()
        {
            Console.WriteLine("Hiding clipboard msg");
            clipboardText.Visibility = Visibility.Hidden;
        }

        public void showClipMsg()
        {
            Console.WriteLine("Showing clipboard msg");
            clipboardText.Visibility = Visibility.Visible;
        }

        public void setResult(String result)
        {
            Console.WriteLine("Result is "+result);
            resultBox.Text = result;
        }
    }
}
