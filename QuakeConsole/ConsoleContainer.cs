using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace QuakeConsole
{
    class ConsoleContainer : Panel
    {
        public enum Alignment 
        {
            Vertical,
            Horizontal
        }
        public event EventHandler Empty;

        private Alignment ChildAlignment;
        public ConsoleContainer(Alignment alignment)
        {
            ChildAlignment = alignment;
        }

        public void Add(Control child)
        {
            if (Controls.Count == 0)
            {
                child.Dock = DockStyle.Fill;
            }
            else
            {
                var dock = ChildAlignment == Alignment.Horizontal
                    ? DockStyle.Left
                    : DockStyle.Top;

                child.Dock = dock;
                var splitter = new ConsoleSplitter();
                splitter.Dock = dock;
                Controls.Add(splitter);
            }
            Controls.Add(child);
        }


        public void Remove(Control child)
        {
            Controls.Remove(child);

            if (Controls.Count > 0)
            {
                if (Controls[0] is ConsoleSplitter)
                {
                    Controls.RemoveAt(0);
                }
                else if (Controls[Controls.Count - 1] is ConsoleSplitter)
                {
                    Controls.RemoveAt(Controls.Count - 1);
                }
                else
                {
                    Control prev = null;
                    foreach (Control control in Controls)
                    {
                        if (control is ConsoleSplitter && prev is ConsoleSplitter)
                        {
                            Controls.Remove(control);
                            break;
                        }
                        prev = control;
                    }
                }
            }

            if (Controls.Count == 0)
            {
                Empty(this, new EventArgs());
            }
            else 
            {
                Controls[0].Dock = DockStyle.Fill;
            }
        }
    }
}
