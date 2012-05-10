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
        }
    }
}
