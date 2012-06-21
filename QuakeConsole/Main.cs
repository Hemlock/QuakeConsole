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
        private QuakeApplication 
            FocusedApplication, 
            LastFocusedTerminal;
        private GlobalHotKey[] ApplicationHotKeys;

        public Main()
        {
            HotKeys = new GlobalHotKeys();
            HotKeys.Register(Keys.Oemtilde, Modifiers.Ctrl, this.Toggle, true);
            var ctrlShift = Modifiers.Ctrl | Modifiers.Shift;
            ApplicationHotKeys = new GlobalHotKey[] { 
                HotKeys.Register(Keys.E, ctrlShift, SplitApplicationHorizontally),
                HotKeys.Register(Keys.O, ctrlShift, SplitApplicationVertically),
                HotKeys.Register(Keys.W, ctrlShift, CloseApplication)
            };
    
            InitializeComponent();
            Height = Properties.Settings.Default.Height;
            Opacity = Properties.Settings.Default.Opacity;
            Width = Screen.PrimaryScreen.Bounds.Width;
            var terminal = CreateNewApplication();
            Controls.Add(terminal);
            ControlRemoved += new ControlEventHandler(ApplicationRemoved);

            Padding = new Padding(0, 0, 0, 4);
        }
        
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTBOTTOM = 15;

            if (m.Msg == WM_NCHITTEST)
            {
                int x = (int)(m.LParam.ToInt64() & 0xFFFF);
                int y = (int)((m.LParam.ToInt64() & 0xFFFF0000) >> 16);
                Point pt = PointToClient(new Point(x, y));
                Size clientSize = ClientSize;

                if ( pt.Y >= ClientSize.Height - Padding.Bottom && ClientSize.Height >= Padding.Bottom)
                {
                    m.Result = (IntPtr)HTBOTTOM;
                    RefocusLastFocusedApplication();
                    return;
                }
            }
            base.WndProc(ref m);
        }

        void ApplicationRemoved(object sender, ControlEventArgs e)
        {
            if (Controls.Count == 0)
            {
                Close();
            }
        }
        
        public void RefocusLastFocusedApplication()
        {
            if (LastFocusedTerminal != null && LastFocusedTerminal != FocusedApplication)
            {
                LastFocusedTerminal.FocusApplication();
            }
        }

        QuakeApplication CreateNewApplication()
        {
            var terminal = new QuakeApplication();
            terminal.ApplicationBlurred += new EventHandler(OnApplicationBlurred);
            terminal.ApplicationFocused += new EventHandler(OnApplicationFocused);
            return terminal;
        }

        void OnApplicationFocused(object sender, EventArgs e)
        {
            FocusedApplication = (QuakeApplication)sender;
            EnableOrDisableApplicationHotKeys(true);
            LastFocusedTerminal = FocusedApplication;
        }

        void OnApplicationBlurred(object sender, EventArgs e)
        {
            var application = (QuakeApplication)sender;
            if (application == FocusedApplication)
            {
                EnableOrDisableApplicationHotKeys(false);
                FocusedApplication = null;
            }
        }

        void EnableOrDisableApplicationHotKeys(bool enable)
        {
            foreach (GlobalHotKey hotkey in ApplicationHotKeys)
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

        private void SplitApplicationHorizontally()
        {
            SplitApplication(Orientation.Vertical);
        }

        private void SplitApplicationVertically()
        {
            SplitApplication(Orientation.Horizontal);
        }

        private void SplitApplication(Orientation orientation)
        {
            if (FocusedApplication != null) 
            {
                var parent = FocusedApplication.Parent;
                var container = new ConsoleContainer(orientation);
                
                parent.Controls.Add(container);
                parent.Controls.Remove(FocusedApplication);
                container.Panel1.Controls.Add(FocusedApplication);
                var application = CreateNewApplication();
                container.Panel2.Controls.Add(application);
                application.HasFocus = true;
            }
        }

        private void CloseApplication()
        {
            if (FocusedApplication != null) 
            {
                FocusedApplication.ExitApplication();
            }
        }

        private void OnVisibleChange(object sender, EventArgs e)
        {
            if (Visible)
            {
                RefocusLastFocusedApplication();
            }

        }

        private void OnShown(object sender, EventArgs e)
        {
            var application = (QuakeApplication)Controls[0];
            application.FocusApplication();
        }

    }
}