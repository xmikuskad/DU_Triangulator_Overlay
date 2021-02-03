using System;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace DuOverlay
{
    public partial class Overlay : Window
    {
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);


        MainWindow mainWindow;
        //KeyboardHook keyboardHook;

        public Overlay(MainWindow mainWindow1)
        {
            InitializeComponent();

            //Settings for overlay window
            var hwnd = new WindowInteropHelper(this).Handle;
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);

            //Initialize variables
            mainWindow = mainWindow1;

            //Set up UI
            setUpPositions();
        }

        //dual.exe
        void setFocusToExternalApp(string strProcessName)
        {
            Process[] arrProcesses = Process.GetProcessesByName(strProcessName);
            if (arrProcesses.Length > 0)
            {
                //Use the very first process (if there is many of them)
                IntPtr ipHwnd = arrProcesses[0].MainWindowHandle;
                bool fSuccess = SetForegroundWindow(ipHwnd);
                if (!fSuccess)
                    MessageBox.Show("Couldn't set the focus to app \"" + strProcessName + "\".");
            }
            else
                MessageBox.Show("Couldn't find app \"" + strProcessName + "\".");
        }

        private void setUpPositions()
        {
            Canvas.SetLeft(canvas1, 100);
            Canvas.SetLeft(canvas2, 100);
            Canvas.SetLeft(canvas3, 100);
            Canvas.SetLeft(canvas4, 100);
            Canvas.SetLeft(canvas5, 100);
        }

        public void setPosition(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int code = Int32.Parse(btn.Tag.ToString());

            if (mainWindow.setFieldText(code))
            {
                //Done
                btn.Background = Brushes.Green;
            }
            else
            {
                //Error
                btn.Background = Brushes.Red;
            }
        }
        public void setDistance(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int code = Int32.Parse(btn.Tag.ToString());
            if (mainWindow.setDistance(code))
            {
                //Done
                btn.Background = Brushes.Green;
            }
            else
            {
                //Error
                btn.Background = Brushes.Red;
            }
        }

        public void setDistanceColor(int number, Brush color)
        {
            switch (number)
            {
                case 1:
                    dis1.Background = color;
                    return;
                case 2:
                    dis2.Background = color;
                    return;
                case 3:
                    dis3.Background = color;
                    return;
                case 4:
                    dis4.Background = color;
                    return;
            }
        }

        public void setPositionColor(int number, Brush color)
        {
            switch (number)
            {
                case 1:
                    pos1.Background = color;
                    return;
                case 2:
                    pos2.Background = color;
                    return;
                case 3:
                    pos3.Background = color;
                    return;
                case 4:
                    pos4.Background = color;
                    return;
            }
        }

        public void setResultColor(Brush color)
        {
            resultBtn.Background = color;
        }

        public void clearFields(object sender, RoutedEventArgs e)
        {
            pos1.Background = Brushes.White;
            pos2.Background = Brushes.White;
            pos3.Background = Brushes.White;
            pos4.Background = Brushes.White;
            dis1.Background = Brushes.White;
            dis2.Background = Brushes.White;
            dis3.Background = Brushes.White;
            dis4.Background = Brushes.White;
            resultBtn.Background = Brushes.White;

            mainWindow.clearAllFields(null,null);
        }
        public void getResult(object sender, RoutedEventArgs e)
        {
            mainWindow.calculateOrePos(null,null);
            if (string.IsNullOrEmpty(mainWindow.getResultText()))
            {
                resultBtn.Background = Brushes.Red;
            }
            else
            {
                resultBtn.Background = Brushes.Green;
            }
        }
    }
}
