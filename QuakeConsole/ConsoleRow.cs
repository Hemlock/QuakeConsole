using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuakeConsole 
{
    class ConsoleRow : ConsoleContainer
    {
        public ConsoleRow() : base(ConsoleContainer.Alignment.Horizontal)
        {
            Add(new QuakeTerminal());
        }
    }
}
