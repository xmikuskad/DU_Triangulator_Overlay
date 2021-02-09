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

namespace DuOverlay
{
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
            initUI();
        }

        private void initUI()
        {
            appText.Text = "1. Open scanner (tab + left click on it)" + Environment.NewLine
                + "2. Take screenshot and open Triangulator" + Environment.NewLine
                + "3. Click on the frame icon" + Environment.NewLine
                + "4. Go back to DU. Open map and press copy my position" + Environment.NewLine
                + "5. Paste position to Triangulator" + Environment.NewLine
                + "6. Repeat for 4 positions - you need to move  (not recommended walking all time in straight line, and all position can`t be done on one ground level) " + Environment.NewLine
                + "7. Click \"Get ore positions\"" + Environment.NewLine
                + "8. Paste to chat then right click and set as destination";


            overlayText.Text = "1. Open scanner (tab + left click on it)" + Environment.NewLine
                + "2. Click image icon or use Set next distance shortcut" + Environment.NewLine
                + "3. Open map and press copy my position" + Environment.NewLine
                + "4. Click position icon or use Set next position shortcut" + Environment.NewLine
                + "5. Repeat for 4 positions - you need to move  (not recommended walking all time in straight line, and all position can`t be done on one ground level) " + Environment.NewLine
                + "6. Click result button or use Get result shortcut" + Environment.NewLine
                + "7. Paste to chat then right click and set as destination";

            contactText.Text = "If you need more help you can contact us on" + Environment.NewLine
                + "\tDiscord - message AX3Lino#9013" + Environment.NewLine
                + "\tGithub - https://github.com/xmikuskad/DU_Triangulator_Overlay";
        }

    }
}
