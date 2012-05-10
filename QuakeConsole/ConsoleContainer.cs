using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuakeConsole
{
    class ConsoleContainer : Panel
    {
        public enum Alignment 
        {
            Vertical,
            Horizontal
        }

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
                var splitter = new Splitter();
                splitter.Dock = dock;
                Controls.Add(splitter);
            }
            Controls.Add(child);
        }

        public void Remove(Control child)
        {
        }
    }
}
