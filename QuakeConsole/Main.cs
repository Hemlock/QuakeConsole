using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Forms.VisualStyles;

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
            var ctrlShift = Modifiers.Ctrl | Modifiers.Shift;
            TerminalHotKeys = new GlobalHotKey[] { 
                HotKeys.Register(Keys.E, ctrlShift, SplitTerminalHorizontally),
                HotKeys.Register(Keys.O, ctrlShift, SplitTerminalVertically),
                HotKeys.Register(Keys.W, ctrlShift, CloseTerminal)
            };
    
            InitializeComponent();
            Height = Properties.Settings.Default.Height;
            Width = Screen.PrimaryScreen.Bounds.Width;
            var terminal = CreateNewTerminal();
            Controls.Add(terminal);
            ControlRemoved += new ControlEventHandler(TerminalRemoved);
        }

        void TerminalRemoved(object sender, ControlEventArgs e)
        {
            if (Controls.Count == 0)
            {
                Close();
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
                terminal.HasFocus = true;
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
