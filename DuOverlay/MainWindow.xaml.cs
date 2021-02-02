using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
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
            tb.Foreground = System.Windows.Media.Brushes.Black;
            if (tb.Text.Equals(POS_PLACEHOLDER))
            {
                tb.Text = "";
            }
        }

        //Called when textfield LostFocus gets fired
        public void addPosPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = POS_PLACEHOLDER;
                tb.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        //Called when textfield GotFocus gets fired
        public void removeDistPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Foreground = System.Windows.Media.Brushes.Black;
            if (tb.Text.Contains(DIST_PLACEHOLDER))
            {
                tb.Text = "";
            }
        }

        //Called when textfield LostFocus gets fired
        public void addDistPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = DIST_PLACEHOLDER;
                tb.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        //Opens overlay
        private void activateOverlay(object sender, RoutedEventArgs e)
        {
            if (keyboardHook == null)
                keyboardHook = new KeyboardHook();

            if (overlay == null)
                overlay = new Overlay(keyboardHook,this);

            if (overlayMenu.IsChecked)
            {
                overlay.Show();
            }
            else
            {
                overlay.Hide();
            }
        }

        //For overlay
        public bool setFieldText(int number)
        {
            string text = Clipboard.GetText();
            if(string.IsNullOrEmpty(text) || !text.Contains("::pos{"))
            {
                return false;
            }

            switch (number)
            {
                case 1:
                    removePosPlaceholder(field1, null);
                    field1.Text = text;
                    return true;
                case 2:
                    removePosPlaceholder(field2, null);
                    field2.Text = text;
                    return true;
                case 3:
                    removePosPlaceholder(field3, null);
                    field3.Text = text;
                    return true;
                case 4:
                    removePosPlaceholder(field4, null);
                    field4.Text = text;
                    return true;
            }

            return false;
        }

        //For overlay
        public bool setDistance(int number)
        {
            Bitmap image = new Bitmap((int)Math.Floor(SystemParameters.PrimaryScreenWidth), (int)Math.Floor(SystemParameters.PrimaryScreenHeight));
            Graphics graphics = Graphics.FromImage(image as System.Drawing.Image);
            graphics.CopyFromScreen(0, 0, 0, 0, image.Size);

            /*if (Clipboard.ContainsImage())
            {
                BitmapSource image = Clipboard.GetImage();*/

                switch (number)
                {
                    case 1:
                        changeOreDistText(dist1, image, false);
                        if (Double.Parse(dist1.Text) < 10)
                            return false;
                        return true;
                    case 2:
                        changeOreDistText(dist2, image, false);
                        if (Double.Parse(dist2.Text) < 10)
                            return false;
                        return true;
                    case 3:
                        changeOreDistText(dist3, image, false);
                        if (Double.Parse(dist3.Text) < 10)
                            return false;
                        return true;
                    case 4:
                        changeOreDistText(dist4, image, false);
                        if (Double.Parse(dist4.Text) < 10)
                            return false;
                        return true;
                }
            //}
            return false;
        }

        public void calculateDistanceImage(object sender, RoutedEventArgs e)
        {
            /*if (Clipboard.ContainsImage())
            {*/
            //BitmapSource source = Clipboard.GetImage();

            Bitmap image = new Bitmap((int)Math.Floor(SystemParameters.PrimaryScreenWidth), (int)Math.Floor(SystemParameters.PrimaryScreenHeight));
            Graphics graphics = Graphics.FromImage(image as System.Drawing.Image);
            graphics.CopyFromScreen(0, 0, 0, 0, image.Size);
            /*SaveControlImage(printscreen);

            BitmapSource image = new BitmapImage(new Uri(@".\image.bmp"));*/
            var btn = sender as Button;
            String code = btn.Tag.ToString();

            if (code.Equals("1"))
            {
                changeOreDistText(dist1, image, true);
            }
            if (code.Equals("2"))
            {
                changeOreDistText(dist2, image, true);
            }
            if (code.Equals("3"))
            {
                changeOreDistText(dist3, image, true);
            }
            if (code.Equals("4"))
            {
                changeOreDistText(dist4, image, true);
            }
            /* }
             else
             {
                 MessageBox.Show("No screenshot in clipboard!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
             }*/
        }

        public BitmapSource convertToSource(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgra32, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);

            return bitmapSource;
        }

        public void changeOreDistText(TextBox tb,Bitmap image, bool showWarning)
        {
            removeDistPlaceholder(tb, null);
            tb.Text = imageProcessing.getOreDistance(image,showWarning).ToString();
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

            if (sender!= null)
            {
                solver.startSolve(varList, this, true);
            }
            else
            {
                solver.startSolve(varList, this, false);
            }
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
            field.Foreground = System.Windows.Media.Brushes.Gray;

            dist.Text = "";
            dist.Text = DIST_PLACEHOLDER;
            dist.Foreground = System.Windows.Media.Brushes.Gray;
        }

        public string getResultText() { return resultBox.Text; }

        public void clearAllFields(object sender, RoutedEventArgs e)
        {
            resetField(field1, dist1);
            resetField(field2, dist2);
            resetField(field3, dist3);
            resetField(field4, dist4);
            resultBox.Text = "";
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
        }

        public void openHelp(object sender, RoutedEventArgs e)
        {
            /*try
            {
                Process.Start("notepad.exe", "help.txt");
            }
            catch(Exception)
            {
                MessageBox.Show("Couldn`t open help! Check file help.txt in application folder.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }*/
        }
    }
}
