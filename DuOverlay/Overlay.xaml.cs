using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Diagnostics;

namespace DuOverlay
{
    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>
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



        KeyboardHook keyboardHook;

        void SetFocusToExternalApp(string strProcessName)
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

        public Overlay(KeyboardHook keyboardHook1)
        {
            InitializeComponent();

            var hwnd = new WindowInteropHelper(this).Handle;
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);


            //InterceptKeys interceptKeys = new InterceptKeys();
            //interceptKeys.start(this);

            //KEYBOARD HOOKS
            // Create the Keyboard Hook
            //KeyboardHook keyboardHook = new KeyboardHook();
            keyboardHook = keyboardHook1;

            //Installing the Keyboard Hooks
            keyboardHook.Install();
            // Capture the events
            keyboardHook.KeyDown += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyDown);
            keyboardHook.KeyUp += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyUp);


            //this.Closed += new EventHandler(this.OnApplicationExit);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //this.Hide();
            MyLabel.Content = MyBox.Text;

            SetFocusToExternalApp(MyBox.Text);
        }

        public void SetThisText(String incString)
        {
            MyLabel.Content = incString;
        }

        private void keyboardHook_KeyUp(KeyboardHook.VKeys key)
        {
            //SetThisText(key.ToString());
            //MessageBox.Show("HELLO THERE");

            if(key == KeyboardHook.VKeys.F4)
            {
                if (this.IsVisible)
                    this.Hide();
                else
                    this.Show();
            }
        }
        private void keyboardHook_KeyDown(KeyboardHook.VKeys key)
        {
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] KeyDown Event {" + key.ToString() + "}");
            //MessageBox.Show("HELLO THERE2");
        }

        public void OnApplicationExit()
        {
            keyboardHook.KeyDown -= new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyDown);
            keyboardHook.KeyUp -= new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyUp);
            keyboardHook.Uninstall();
        }
    }
}
