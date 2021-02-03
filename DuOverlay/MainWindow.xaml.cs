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

            //Initialize variables and load settings
            imageProcessing = new ImageProcessing();
            //CUSTOM SETTINGS FOR DEBUG
            /*SingletonSettings.Instance.changeSettings(true, SingletonSettings.DISPLAY_MODES.FULLSCREEN,true, KeyboardHook.VKeys.KEY_H,
                KeyboardHook.VKeys.KEY_J, KeyboardHook.VKeys.KEY_K, KeyboardHook.VKeys.KEY_L);*/

            loadSettings();

            //START WINDOW AS MODAL WINDOW
            /*
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();*/

            //Setup placeholder text
            clearAllFields(null,null);

            //Disable hooks when closing app
            this.Closed += (sender, e) =>
            {
                if(keyboardHook!=null)
                    OnApplicationExit();
                
                this.Dispatcher.InvokeShutdown();
            };
        }

        private void loadSettings()
        {
            if(SingletonSettings.Instance.shouldUseOverlay())
            {
                if (overlay == null)
                    overlay = new Overlay(this);
                overlay.Show();
                overlayMenu.IsChecked = true;
            }
            if (SingletonSettings.Instance.shouldUseShortcuts())
            {
                if (keyboardHook == null)
                    keyboardHook = new KeyboardHook();

                //Installing the Keyboard Hooks
                keyboardHook.Install();
                // Capture the events
                //keyboardHook.KeyDown += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyDown);
                keyboardHook.KeyUp += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyUp);
            }
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
            if (overlay == null)
                overlay = new Overlay(this);

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
            //TODO make window
            /*try
            {
                Process.Start("notepad.exe", "help.txt");
            }
            catch(Exception)
            {
                MessageBox.Show("Couldn`t open help! Check file help.txt in application folder.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }*/
        }


        private void keyboardHook_KeyUp(KeyboardHook.VKeys key)
        {
            //MessageBox.Show("[" + DateTime.Now.ToLongTimeString() + "] KeyDown Event {" + key.ToString() + "}");
            if (key == SingletonSettings.Instance.getOpenShortcut())
            {
                if (overlay != null)
                {
                    if (overlay.IsVisible)
                        overlay.Hide();
                    else
                        overlay.Show();
                }
            }
            if (key == SingletonSettings.Instance.getDisShortcut())
            {
                TextBox[] obj = new TextBox[] { dist1, dist2, dist3, dist4 };
                for(int i=0;i<obj.Length;i++)
                {
                    if (isEmptyPlace(obj[i], DIST_PLACEHOLDER))
                    {
                        setOverlayDistance(i+1);
                        break;
                    }
                }
            }

            if (key == SingletonSettings.Instance.getPosShortcut())
            {
                TextBox[] obj = new TextBox[] { field1, field2, field3, field4 };
                for (int i = 0; i < obj.Length; i++)
                {
                    if (isEmptyPlace(obj[i], POS_PLACEHOLDER))
                    {
                        setOverlayPositions(i + 1);
                        break;
                    }
                }
            }

            if (key == SingletonSettings.Instance.getResultShortcut())
            {
                calculateOrePos(null, null);
                if (string.IsNullOrEmpty(getResultText()))
                {
                    overlay.setResultColor(System.Windows.Media.Brushes.Red);
                }
                else
                {
                    overlay.setResultColor(System.Windows.Media.Brushes.Green);
                }
            }
            /*private void keyboardHook_KeyDown(KeyboardHook.VKeys key)
            {
                //Debug.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] KeyDown Event {" + key.ToString() + "}");

                KeyboardHook.VKeys gotKey = (KeyboardHook.VKeys)Enum.Parse(typeof(KeyboardHook.VKeys), key.ToString());
                Int32 num = (Int32)gotKey;
                //MessageBox.Show(num.ToString("X"));
            }*/
        }
        private void setOverlayDistance(int number)
        {
            if (setDistance(number))
                overlay.setDistanceColor(number, System.Windows.Media.Brushes.Green);
            else
                overlay.setDistanceColor(number, System.Windows.Media.Brushes.Red);
        }        
        private void setOverlayPositions(int number)
        {
            if (setFieldText(number))
                overlay.setPositionColor(number, System.Windows.Media.Brushes.Green);
            else
                overlay.setPositionColor(number, System.Windows.Media.Brushes.Red);
        }

        private bool isEmptyPlace(TextBox tb, string placeholder)
        {
            return string.IsNullOrEmpty(tb.Text) || tb.Text.Equals(placeholder) || tb.Text.Length < 2;
        }

        public void OnApplicationExit()
        {
            //keyboardHook.KeyDown -= new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyDown);
            keyboardHook.KeyUp -= new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyUp);
            keyboardHook.Uninstall();
        }
    }
}
