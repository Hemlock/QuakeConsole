using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace QuakeConsole
{
    class ConsoleContainer : SplitContainer
    {
        public ConsoleContainer(Orientation orientation) : base()
        {
            Panel1.ControlRemoved += new ControlEventHandler(TerminalRemoved);
            Panel2.ControlRemoved += new ControlEventHandler(TerminalRemoved);
            Dock = DockStyle.Fill;

            Orientation = orientation;
            var size = Orientation == Orientation.Horizontal ? Height : Width;
            SplitterDistance = (int)(size / 2);
        }

        void TerminalRemoved(object sender, ControlEventArgs e)
        {
            var count = Panel1.Controls.Count + Panel2.Controls.Count;
            if (count == 1)
            {
                var panel = Panel1.Controls.Count == 1 ? Panel1 : Panel2;
                var terminal = panel.Controls[0];
                panel.Controls.RemoveAt(0);
                Parent.Controls.Add(terminal);
                Parent.Controls.Remove(this);
            }               
        }
    }
}
