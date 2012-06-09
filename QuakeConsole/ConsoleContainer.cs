using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace QuakeConsole
{
    class ConsoleContainer : SplitContainer, FocusableTerminal
    {

        // Temp variable to store a previously focused control
        private Control focused = null;
        public ConsoleContainer(Orientation orientation)
            : base()
        {
            Panel1.ControlRemoved += new ControlEventHandler(TerminalRemoved);
            Panel2.ControlRemoved += new ControlEventHandler(TerminalRemoved);

            Dock = DockStyle.Fill;

            Orientation = orientation;
            var size = Orientation == Orientation.Horizontal ? Height : Width;
            SplitterDistance = (int)(size / 2);

            MouseUp += new MouseEventHandler(OnMouseUp);
        }

        void TerminalRemoved(object sender, ControlEventArgs e)
        {
            var count = Panel1.Controls.Count + Panel2.Controls.Count;
            if (count == 1)
            {
                var panel = Panel1.Controls.Count == 1 ? Panel1 : Panel2;
                var terminal = (FocusableTerminal)panel.Controls[0];
                panel.Controls.RemoveAt(0);
                Parent.Controls.Add((Control)terminal);
                terminal.FocusTerminal();
                Parent.Controls.Remove(this);
            }
        }

        public void FocusTerminal()
        {
            var terminal = (QuakeTerminal)Panel1.Controls[0];
            terminal.FocusTerminal();
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            var main = (QuakeConsole.Main) Application.OpenForms[0];
            main.RefocusLastFocusedTerminal();
        }
    }
}
