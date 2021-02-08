using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DuOverlay
{
    public partial class SettingsWindow : Window
    {
        const string NONE_KEY = "0";
        const string NONE_TEXT = "None";

        MainWindow mainWindow;
        ToggleButton recordingBtn = null;

        //Changes were saved
        bool shouldUpdate = false;

        public SettingsWindow(MainWindow mainWindow1)
        {
            InitializeComponent();

            mainWindow = mainWindow1;
            getDataFromSettings();

            //Disable recording when closing app
            this.Closed += (sender, e) =>
            {
                //TODO
                mainWindow.closeSettings(shouldUpdate);
            };
        }

        //Init UI with actual settings
        private void getDataFromSettings()
        {
            SingletonSettings.Instance.updateSettings();

            overlayCheckbox.IsChecked = SingletonSettings.Instance.shouldUseOverlay();
            shortcutCheckbox.IsChecked = SingletonSettings.Instance.shouldUseShortcuts();
            shortcutCheckboxChanged(null, null);

            SingletonSettings.DISPLAY_MODES mode = SingletonSettings.Instance.getDisplayMode();
            if (mode == SingletonSettings.DISPLAY_MODES.FULLSCREEN)
                fullscreenItem.IsSelected = true;
            if (mode == SingletonSettings.DISPLAY_MODES.WINDOWED)
                windowedItem.IsSelected = true;

            onOffShortcut.Content = getStringFromKey(SingletonSettings.Instance.getOpenShortcut());
            nextPosShortcut.Content = getStringFromKey(SingletonSettings.Instance.getPosShortcut());
            nextDisShortcut.Content = getStringFromKey(SingletonSettings.Instance.getDisShortcut());
            resultShortcut.Content = getStringFromKey(SingletonSettings.Instance.getResultShortcut());
        }

        private string getStringFromKey(KeyboardHook.VKeys key)
        {
            if (key.ToString().Equals(NONE_KEY))
                return NONE_TEXT;
            else
            {
                return key.ToString().Replace("_","__");
            }
        }

        private KeyboardHook.VKeys getKeyFromString(string key)
        {
            if (key.Equals(NONE_TEXT) || key.Equals("Press KEY")) //This is error which needs to be fixed
                return 0;
            else
            {
                return (KeyboardHook.VKeys)Enum.Parse(typeof(KeyboardHook.VKeys), key.Replace("__", "_"));
            }
        }

        public void shortcutCheckboxChanged(object sender, RoutedEventArgs e)
        {
            if (shortcutCheckbox.IsChecked.Value)
            {
                shortcutCanvas.IsEnabled = true;
            }
            else
            {
                shortcutCanvas.IsEnabled = false;
            }
        }

        public void setShortcut(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;

            if (btn.IsChecked.Value)
            {
                if (recordingBtn != null)
                {
                    MessageBox.Show("Can only record one shortcut at time!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    btn.IsChecked = false;
                    return;
                }

                recordingBtn = btn;
                btn.Content = "Press KEY";
            }
        }

        public void changeRecordingBtnText(KeyboardHook.VKeys key)
        {
            if (recordingBtn != null)
            {
                recordingBtn.Content = getStringFromKey(key);
                recordingBtn.IsChecked = false;
                recordingBtn = null;
            }
        }

        //Called by X buttons
        public void clearShortcut(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int code = Int32.Parse(btn.Tag.ToString());

            switch(code)
            {
                case 1:
                    onOffShortcut.Content = NONE_TEXT;
                    return;
                case 2:
                    nextPosShortcut.Content = NONE_TEXT;
                    return;
                case 3:
                    nextDisShortcut.Content = NONE_TEXT;
                    return;
                case 4:
                    resultShortcut.Content = NONE_TEXT;
                    return;
            }
        }

        //Save settings and close window
        public void saveSettings(object sender, RoutedEventArgs e)
        {
            try
            {
                KeyboardHook.VKeys onOffKey = getKeyFromString(onOffShortcut.Content.ToString());
                KeyboardHook.VKeys nextPosKey = getKeyFromString(nextPosShortcut.Content.ToString());
                KeyboardHook.VKeys nextDisKey = getKeyFromString(nextDisShortcut.Content.ToString());
                KeyboardHook.VKeys resultKey = getKeyFromString(resultShortcut.Content.ToString());

                SingletonSettings.DISPLAY_MODES mode = SingletonSettings.DISPLAY_MODES.FULLSCREEN;
                if (windowedItem.IsSelected)
                    mode = (SingletonSettings.DISPLAY_MODES)Enum.Parse(typeof(SingletonSettings.DISPLAY_MODES), windowedItem.Content.ToString());

                SingletonSettings.Instance.saveSettings(overlayCheckbox.IsChecked.Value, mode, shortcutCheckbox.IsChecked.Value,
                    onOffKey, nextPosKey, nextDisKey, resultKey);

                shouldUpdate = true;

            }
            catch(Exception)
            {
                MessageBox.Show("Saving settings failed!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.Close();
        }
    }
}
