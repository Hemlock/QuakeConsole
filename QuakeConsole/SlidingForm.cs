using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace QuakeConsole
{
    public class SlidingForm : Form
    {
        private Timer AnimationTimer;
        private int AnimationCompleteAt;
        private int Goal;
        private const int ANIMATION_DURATION = 250;
        public SlidingForm() : base()
        {
            AnimationTimer = new Timer();
            AnimationTimer.Interval = 10;
            AnimationTimer.Tick += (object sender, EventArgs e) => Step();
        }

        private void Step()
        {
            int distance = Goal - Top;
            int time = AnimationCompleteAt - Now;
            int interval = AnimationTimer.Interval;
            int steps = (int)(time / interval);
            if (steps <= 1) 
            {
                Top = Goal;
                AnimationTimer.Enabled = false;
                if (Goal != 0)
                {
                    Hide();
                }
            }
            else 
            {
                int step = (int)(distance / steps);
                Top += step;
            }
        }

        public void SlideOut()
        {
            Goal = -Height;
            Start();
        }

        public void SlideIn()
        {
            if (!Visible)
            {
                Show();
            }
            Goal = 0;
            Start();
        }

        private void Start()
        {
            AnimationTimer.Enabled = true;
            AnimationCompleteAt = Now + ANIMATION_DURATION;
        }

        public bool Sliding
        {
            get
            {
                return AnimationTimer.Enabled;
            }
        }

        private int Now
        {
            get
            {
                return System.Environment.TickCount;
            }
        }
    }
}
