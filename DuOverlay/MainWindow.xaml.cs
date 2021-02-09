using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace DuOverlay
{
    public partial class MainWindow : Window
    {
        Overlay overlay;
        SettingsWindow settings;
        HelpWindow help;
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
            loadSettings();

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

        //Open settings window
        public void showSettings(object sender, RoutedEventArgs e)
        {
            settings = new SettingsWindow(this);
            settings.ShowDialog();
        }

        //Load user settings 
        private void loadSettings()
        {
            if(SingletonSettings.Instance.shouldUseOverlay())
            {
                if (overlay == null)
                    overlay = new Overlay(this);
                overlay.Show();
                overlayMenu.IsChecked = true;
            }

            if (keyboardHook == null)
            {
                keyboardHook = new KeyboardHook();

                //Installing the Keyboard Hooks
                keyboardHook.Install();
                keyboardHook.KeyUp += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyUp);
            }
        }

        //Called when textfield GotFocus gets fired - simulate hint text
        public void removePosPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Foreground = System.Windows.Media.Brushes.Black;
            if (tb.Text.Equals(POS_PLACEHOLDER))
            {
                tb.Text = "";
            }
        }

        //Called when textfield LostFocus gets fired - simulate hint text
        public void addPosPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = POS_PLACEHOLDER;
                tb.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        //Called when textfield GotFocus gets fired - simulate hint text
        public void removeDistPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Foreground = System.Windows.Media.Brushes.Black;
            if (tb.Text.Contains(DIST_PLACEHOLDER))
            {
                tb.Text = "";
            }
        }

        //Called when textfield LostFocus gets fired - simulate hint text
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

            switch (number)
            {
                case 1:
                    changeOreDistText(dist1, image, false);
                    image.Dispose();
                    graphics.Dispose();
                    if (Double.Parse(dist1.Text) < 10)
                        return false;
                    return true;
                case 2:
                    changeOreDistText(dist2, image, false);
                    image.Dispose();
                    graphics.Dispose();
                    if (Double.Parse(dist2.Text) < 10)
                        return false;
                    return true;
                case 3:
                    changeOreDistText(dist3, image, false);
                    image.Dispose();
                    graphics.Dispose();
                    if (Double.Parse(dist3.Text) < 10)
                        return false;
                    return true;
                case 4:
                    changeOreDistText(dist4, image, false);
                    image.Dispose();
                    graphics.Dispose();
                    if (Double.Parse(dist4.Text) < 10)
                        return false;
                    return true;
            }
            return false;
        }

        public void calculateDistanceImage(object sender, RoutedEventArgs e)
        {
            Bitmap image = new Bitmap((int)Math.Floor(SystemParameters.PrimaryScreenWidth), (int)Math.Floor(SystemParameters.PrimaryScreenHeight));
            Graphics graphics = Graphics.FromImage(image as System.Drawing.Image);
            graphics.CopyFromScreen(0, 0, 0, 0, image.Size);

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

            //Garbage collector is too slow
            image.Dispose();
            graphics.Dispose();
        }

        public void changeOreDistText(TextBox tb,Bitmap image, bool showWarning)
        {
            removeDistPlaceholder(tb, null);
            tb.Text = imageProcessing.getOreDistance(image,showWarning).ToString();
            addDistPlaceholder(tb, null);
        }

        //Called when get result btn is pressed - starts calculation
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

            //Check if it was called from overlay or not.
            bool shouldShowWarnings = sender != null;
            solver.startSolve(varList, this, shouldShowWarnings);
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

        //Called after clicking open help btn
        public void openHelp(object sender, RoutedEventArgs e)
        {
            help = new HelpWindow();
            help.ShowDialog();
        }


        private void keyboardHook_KeyUp(KeyboardHook.VKeys key)
        {
            //Used for recording button shortcuts - TODO better solution
            if(settings!=null)
            {
                settings.changeRecordingBtnText(key);
            }

            if (!SingletonSettings.Instance.shouldUseShortcuts())
                return;

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

        //Update UI for settings
        public void closeSettings(bool shouldUpdate)
        {
            //Disable button recording possibility
            settings = null;

            //Check if user used save settings or just closed window
            if (!shouldUpdate)
                return;

            if (SingletonSettings.Instance.shouldUseOverlay())
            {
                if (overlay == null)
                    overlay = new Overlay(this);
                overlay.Show();
                overlayMenu.IsChecked = true;
            }
            else
            {
                if (overlay != null)
                    overlay.Close();
                overlay = null;
                overlayMenu.IsChecked = false;
            }
        }

        //Remove keyboardhooks
        public void OnApplicationExit()
        {
            keyboardHook.KeyUp -= new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyUp);
            keyboardHook.Uninstall();
        }
    }
}
