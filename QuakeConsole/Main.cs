using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace QuakeConsole
{
    public partial class Main : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        private GlobalHotKeys hotkeys;
        private bool visible;
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
                WindowAnimation.SlideOut(this);
            }
            else
            {
                WindowAnimation.SlideIn(this);
            }
            visible = !visible;
        }

        private void StartConsole()
        {
            Process notepad = new Process();
            ProcessStartInfo psi = new ProcessStartInfo("c:\\Console2\\Console.exe");
            psi.WindowStyle = ProcessWindowStyle.Normal;
            notepad.StartInfo = psi;

            notepad.Start();
            notepad.WaitForInputIdle(3000);

            SetParent(notepad.MainWindowHandle, this.Handle);
        }
    }
}
