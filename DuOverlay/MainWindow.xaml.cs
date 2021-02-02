using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DuOverlay
{
    public partial class MainWindow : Window
    {
        Overlay overlay;
        KeyboardHook keyboardHook;
        Solver solver;
        ImageProcessing imageProcessing;

        const string POS_PLACEHOLDER = "::pos{0,a,x,y,z}";
        const string DIST_PLACEHOLDER = "200.11...";

        public MainWindow()
        {
            InitializeComponent();

            //Used to change , to . in decimal numbers
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            //Initialize variables
            imageProcessing = new ImageProcessing();

            //Setup placeholder text
            clearAllFields(null,null);

            //Disable hooks when closing app
            this.Closed += (sender, e) =>
            {
                if(keyboardHook!=null)
                    overlay.OnApplicationExit();
                
                this.Dispatcher.InvokeShutdown();
            };
        }

        //Called when textfield GotFocus gets fired
        public void removePosPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text.Equals(POS_PLACEHOLDER))
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }

        //Called when textfield LostFocus gets fired
        public void addPosPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = POS_PLACEHOLDER;
                tb.Foreground = Brushes.Gray;
            }
        }

        //Called when textfield GotFocus gets fired
        public void removeDistPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text.Equals(DIST_PLACEHOLDER))
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }

        //Called when textfield LostFocus gets fired
        public void addDistPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = DIST_PLACEHOLDER;
                tb.Foreground = Brushes.Gray;
            }
        }

        //Opens overlay
        private void activateOverlay(object sender, RoutedEventArgs e)
        {
            if (keyboardHook == null)
                keyboardHook = new KeyboardHook();

            if (overlay == null)
                overlay = new Overlay(keyboardHook);

            if (overlayMenu.IsChecked)
            {
                overlay.Show();
            }
            else
            {
                overlay.Hide();
            }
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
                    changeOreDistText(dist1, image);
                }
                if (code.Equals("2"))
                {
                    changeOreDistText(dist2, image);
                }
                if (code.Equals("3"))
                {
                    changeOreDistText(dist3, image);
                }
                if (code.Equals("4"))
                {
                    changeOreDistText(dist4, image);
                }
            }
            else
            {
                MessageBox.Show("No screenshot in clipboard!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void changeOreDistText(TextBox tb,BitmapSource image)
        {
            removeDistPlaceholder(tb, null);
            tb.Text = imageProcessing.getOreDistance(image).ToString();
            addDistPlaceholder(tb, null);
        }

        public void calculateOrePos(object sender, RoutedEventArgs e)
        {
            solver = new Solver();
            List<String> varList = new List<String>();
            varList.Add(field1.Text);
            varList.Add(dist1.Text);
            varList.Add(field2.Text);
            varList.Add(dist2.Text);
            varList.Add(field3.Text);
            varList.Add(dist3.Text);
            varList.Add(field4.Text);
            varList.Add(dist4.Text);

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
            clipboardText.Visibility = Visibility.Hidden;
        }

        public void showClipMsg()
        {
            clipboardText.Visibility = Visibility.Visible;
        }

        public void setResult(String result)
        {
            resultBox.Text = result;
            MessageBox.Show(result);
        }

        public void openHelp(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("notepad.exe", "help.txt");
            }
            catch(Exception)
            {
                MessageBox.Show("Couldn`t open help! Check file help.txt in application folder.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
