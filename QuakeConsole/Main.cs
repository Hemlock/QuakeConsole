using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuakeConsole
{
    public partial class Main : SlidingForm
    {
        private GlobalHotKeys HotKeys;
        public Main()
        {
            HotKeys = new GlobalHotKeys();
            HotKeys.Register(Keys.Oemtilde, Modifiers.Ctrl, () => this.Toggle() );
                
            InitializeComponent();
            Height = 500;
            Width = Screen.PrimaryScreen.Bounds.Width;

            var column = new ConsoleColumn();
            column.Dock = DockStyle.Fill;
            Controls.Add(column);            
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
    }
}
