using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace QuakeConsole
{
    class CaptionPanel : Panel
    {
        private Color _CaptionColor = Color.White;
        private string _Caption = "";
        
        private Font _CaptionFont = new Font("Segoe UI", 10, FontStyle.Regular);
        public CaptionPanel() : base()
        {
            Padding = new Padding(0, 20, 0, 0);
            Paint += new PaintEventHandler(OnPaint);
        }

        void OnPaint(object sender, PaintEventArgs e)
        {
            DrawCaption(e.Graphics);
        }

        public Color CaptionColor
        {
            get
            {
                return _CaptionColor;
            }
            set
            {
                _CaptionColor = value;
                Invalidate();
            }
        }

        public Font CaptionFont
        {
            get
            {
                return _CaptionFont;
            }

            set
            {
                _CaptionFont = value;
                Invalidate();
            }
        }

        public string Caption
        {
            get
            {
                return _Caption;
            }
            set
            {
                _Caption = value == null ? "" : value;
                Invalidate();
            }
        }

        private void DrawCaption(Graphics graphics)
        {
            SizeF textSize = graphics.MeasureString(Caption, CaptionFont);
            Brush brush = new SolidBrush(CaptionColor);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            graphics.DrawString(Caption, CaptionFont, brush, 2, (Padding.Top - textSize.Height) / 2);
        }
    }
}
