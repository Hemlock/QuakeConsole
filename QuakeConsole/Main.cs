using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuakeConsole
{
    public partial class Main : Form
    {




        private GlobalHotKeys hotkeys;
        private bool visible;
        private ChildApplication console;
        private const int WM_PRINT = 0x0317;
        private const int WM_PRINTCLIENT = 0x0318;
        public Main()
        {
            visible = true;
            hotkeys = new GlobalHotKeys();
            hotkeys.Register(Keys.Oemtilde, Modifiers.Ctrl, () => this.Toggle() );
                
            InitializeComponent();
            Height = 500;
            Width = Screen.PrimaryScreen.Bounds.Width;

            StartConsole();
        }

        private void Toggle()
        {
            if (visible)
            {
                WindowAnimation.FadeOut(this);
            }
            else
            {
                WindowAnimation.FadeIn(this);
            }
            visible = !visible;
            console.SizeToParent();
        }

        private void StartConsole()
        {
            console = new ChildApplication("c:\\console2\\console.exe", this);
            console.Launch();
        }

    }
}
