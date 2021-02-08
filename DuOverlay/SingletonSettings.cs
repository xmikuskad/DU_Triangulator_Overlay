using System;
using System.IO;
using System.Windows;

namespace DuOverlay
{
    public sealed class SingletonSettings
    {
        private static readonly SingletonSettings instance = new SingletonSettings();
        const string PATH = "settings.txt";
        const string OVERLAY_ON = "OVERLAY_ON";
        const string DISPLAY_MODE = "DISPLAY_MODE";
        const string ALLOW_SHORTCUTS = "ALLOW_SHORTCUTS";
        const string OPEN_SHORTCUT = "OPEN_SHORTCUT";
        const string POS_SHORTCUT = "POS_SHORTCUT";
        const string DIST_SHORTCUT = "DIST_SHORTCUT";
        const string RESULT_SHORTCUT = "RESULT_SHORTCUT";

        bool isOverlayOn = true;
        bool allowShortcuts = true;
        DISPLAY_MODES displayMode = DISPLAY_MODES.FULLSCREEN;
        KeyboardHook.VKeys openShortcut;
        KeyboardHook.VKeys posShortcut;
        KeyboardHook.VKeys disShortcut;
        KeyboardHook.VKeys resultShortcut;

        public enum DISPLAY_MODES
        {
            FULLSCREEN =1,
            WINDOWED = 2
        }
        
        // Explicit static constructor to tell C# compiler  
        // not to mark type as beforefieldinit  
        static SingletonSettings() {}
        private SingletonSettings() { updateSettings(); }
        public static SingletonSettings Instance
        {
            get
            {
                return instance;
            }
        }

        public void updateSettings()
        {
            if (File.Exists(PATH))
            {
                try
                { 
                    loadSettings();
                }
                catch(Exception)
                {
                    MessageBox.Show("Settings file corrupted, reverting to default!","ERROR",MessageBoxButton.OK,MessageBoxImage.Error);
                    createSettingsFile();
                }
            }
            else
            {
                createSettingsFile();
            }
            loadSettings();
        }

        private void createSettingsFile()
        {
            File.WriteAllText(PATH, OVERLAY_ON + "=True\n" + DISPLAY_MODE + "=FULLSCREEN\n"+ALLOW_SHORTCUTS+"=True\n"+
                OPEN_SHORTCUT+"=0\n"+POS_SHORTCUT+"=0\n"+DIST_SHORTCUT+"=0\n"+RESULT_SHORTCUT+"=0\n");
        }

        public void saveSettings(bool newOvelay, DISPLAY_MODES newDisplayMode, bool newShortcut, KeyboardHook.VKeys newOpen,
            KeyboardHook.VKeys newPos, KeyboardHook.VKeys newDist, KeyboardHook.VKeys newResult)
        {
            File.WriteAllText(PATH, OVERLAY_ON + "="+newOvelay.ToString()+"\n" + DISPLAY_MODE + "="+newDisplayMode.ToString()+"\n"+
                ALLOW_SHORTCUTS+"="+ newShortcut.ToString()+"\n"+OPEN_SHORTCUT+"="+newOpen.ToString()+"\n"+POS_SHORTCUT+"="+
                newPos.ToString()+"\n"+DIST_SHORTCUT+"="+newDist.ToString()+"\n"+RESULT_SHORTCUT+"="+newResult.ToString());
            
            loadSettings();
        }

        private void loadSettings()
        {
            string[] lines = File.ReadAllLines(PATH);

            if (lines.Length < 7)
                throw new Exception("Bad formatting of settings file!");

            foreach(string line in lines)
            {
                var splitted = line.Split("=");
                if (splitted[0].Equals(OVERLAY_ON))
                {
                    isOverlayOn = bool.Parse(splitted[1]);
                    continue;
                }
                else if(splitted[0].Equals(DISPLAY_MODE))
                {
                    displayMode = (DISPLAY_MODES)Enum.Parse(typeof(DISPLAY_MODES), splitted[1]);
                    continue;
                }
                else if(splitted[0].Equals(ALLOW_SHORTCUTS))
                {
                    allowShortcuts = bool.Parse(splitted[1]);
                    continue;
                }
                else if (splitted[0].Equals(OPEN_SHORTCUT))
                {
                    openShortcut = (KeyboardHook.VKeys)Enum.Parse(typeof(KeyboardHook.VKeys), splitted[1]);
                    continue;
                }
                else if (splitted[0].Equals(POS_SHORTCUT))
                {
                    posShortcut = (KeyboardHook.VKeys)Enum.Parse(typeof(KeyboardHook.VKeys), splitted[1]);
                    continue;
                }
                else if (splitted[0].Equals(DIST_SHORTCUT))
                {
                    disShortcut = (KeyboardHook.VKeys)Enum.Parse(typeof(KeyboardHook.VKeys), splitted[1]);
                    continue;
                }
                else if (splitted[0].Equals(RESULT_SHORTCUT))
                {
                    resultShortcut = (KeyboardHook.VKeys)Enum.Parse(typeof(KeyboardHook.VKeys), splitted[1]);
                    continue;
                }
            }
        }
    
        //Getters
        public DISPLAY_MODES getDisplayMode() { return displayMode; }
        public bool shouldUseOverlay() { return isOverlayOn; }
        public bool shouldUseShortcuts() { return allowShortcuts; }
        public KeyboardHook.VKeys getOpenShortcut() { return openShortcut; }
        public KeyboardHook.VKeys getPosShortcut() { return posShortcut; }
        public KeyboardHook.VKeys getDisShortcut() { return disShortcut; }
        public KeyboardHook.VKeys getResultShortcut() { return resultShortcut; }
    }
}
