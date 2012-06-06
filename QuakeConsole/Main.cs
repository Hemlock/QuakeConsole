using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace QuakeConsole
{
    public partial class Main : SlidingForm
    {
        private GlobalHotKeys HotKeys;
        private QuakeTerminal 
            FocusedTerminal, 
            LastFocusedTerminal;
        private GlobalHotKey[] TerminalHotKeys;

        public Main()
        {
            HotKeys = new GlobalHotKeys();
            HotKeys.Register(Keys.Oemtilde, Modifiers.Ctrl, this.Toggle, true);
            TerminalHotKeys = new GlobalHotKey[] { 
                HotKeys.Register(Keys.E, Modifiers.Ctrl | Modifiers.Shift, this.SplitTerminalHorizontally),
                HotKeys.Register(Keys.O, Modifiers.Ctrl | Modifiers.Shift, this.SplitTerminalVertically),
                HotKeys.Register(Keys.W, Modifiers.Ctrl | Modifiers.Shift, this.CloseTerminal) 
            };
    
            InitializeComponent();
            Height = 500;
            Width = Screen.PrimaryScreen.Bounds.Width;
            Controls.Add(CreateNewTerminal());
            ControlRemoved += new ControlEventHandler(TerminalRemoved);
        }

        void TerminalRemoved(object sender, ControlEventArgs e)
        {
            if (Controls.Count == 0)
            {
                Application.Exit();
            }
        }
        
        QuakeTerminal CreateNewTerminal()
        {
            var terminal = new QuakeTerminal();
            terminal.TerminalBlurred += new EventHandler(OnTerminalBlurred);
            terminal.TerminalFocused += new EventHandler(OnTerminalFocused);
            return terminal;
        }

        void OnTerminalFocused(object sender, EventArgs e)
        {
            Debug.WriteLine(sender);
            FocusedTerminal = (QuakeTerminal)sender;
            EnableOrDisableTerminalHotKeys(true);
            LastFocusedTerminal = FocusedTerminal;
        }

        void OnTerminalBlurred(object sender, EventArgs e)
        {
            var terminal = (QuakeTerminal)sender;
            if (terminal == FocusedTerminal)
            {
                EnableOrDisableTerminalHotKeys(false);
                FocusedTerminal = null;
            }
        }

        void EnableOrDisableTerminalHotKeys(bool enable)
        {
            foreach (GlobalHotKey hotkey in TerminalHotKeys)
            {
                if (enable)
                {
                    hotkey.Enable();
                }
                else
                {
                    hotkey.Disable();
                }
            }
        }

        private void Toggle()
        {
            if (!Visible || Sliding)
            {
                SlideIn();
            }
            else
            {
                SlideOut();
            }
        }

        private void SplitTerminalHorizontally()
        {
            SplitTerminal(Orientation.Vertical);
        }

        private void SplitTerminalVertically()
        {
            SplitTerminal(Orientation.Horizontal);
        }

        private void SplitTerminal(Orientation orientation)
        {
            if (FocusedTerminal != null) 
            {
                var parent = FocusedTerminal.Parent;
                var container = new ConsoleContainer(orientation);
                parent.Controls.Add(container);
                parent.Controls.Remove(FocusedTerminal);
                container.Panel1.Controls.Add(FocusedTerminal);
                var terminal = CreateNewTerminal();
                container.Panel2.Controls.Add(terminal);
                terminal.FocusTerminal();               
            }
        }

        private void CloseTerminal()
        {
            if (FocusedTerminal != null) 
            {
                FocusedTerminal.ExitTerminal();
            }
        }

        private void OnVisibleChange(object sender, EventArgs e)
        {
            if (Visible)
            {
                if (LastFocusedTerminal != null) {
                    LastFocusedTerminal.FocusTerminal();
                }
            }

        }

        private void OnShown(object sender, EventArgs e)
        {
            var terminal = (QuakeTerminal)Controls[0];
            terminal.FocusTerminal();
        }

    }
}
