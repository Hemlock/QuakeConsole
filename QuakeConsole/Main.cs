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
            HotKeys.Register(Keys.Oemtilde, Modifiers.Ctrl, this.Toggle);
                
            InitializeComponent();
            Height = 500;
            Width = Screen.PrimaryScreen.Bounds.Width;

            var column = new ConsoleColumn();
            column.Dock = DockStyle.Fill;
            column.Empty += (object sender, EventArgs e) => Application.Exit();
            Controls.Add(column);            
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Shift | Keys.E))
            {
                MessageBox.Show("C+S+e");
                return true;
            }
            else if (keyData == (Keys.Control | Keys.Shift | Keys.O))
            {
                MessageBox.Show("C+S+o");
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
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
