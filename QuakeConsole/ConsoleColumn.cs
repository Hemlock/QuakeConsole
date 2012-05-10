using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace QuakeConsole
{
    class ConsoleColumn: ConsoleContainer
    {
        public ConsoleColumn() : base(ConsoleContainer.Alignment.Vertical)
        {
            Add(new ConsoleRow());
            Add(new ConsoleRow());
        }

        public void Add(ConsoleContainer row)
        {
            base.Add(row);
            row.Empty += (object sender, EventArgs e) => Remove((Control)sender);
        }
    }
}
