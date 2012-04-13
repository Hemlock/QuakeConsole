using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace QuakeConsole
{
    public class WindowAnimation
    {
        /// <summary>
        /// Animates the window from left to right. 
        /// This flag can be used with roll or slide animation.
        /// </summary>
        public const int AW_HOR_POSITIVE = 0X1;
        /// <summary>
        /// Animates the window from right to left. 
        /// This flag can be used with roll or slide animation.
        /// </summary>
        public const int AW_HOR_NEGATIVE = 0X2;
        /// <summary>
        /// Animates the window from top to bottom. 
        /// This flag can be used with roll or slide animation.
        /// </summary>
        public const int AW_VER_POSITIVE = 0X4;
        /// <summary>
        /// Animates the window from bottom to top. 
        /// This flag can be used with roll or slide animation.
        /// </summary>
        public const int AW_VER_NEGATIVE = 0X8;
        /// <summary>
        /// Makes the window appear to collapse inward 
        /// if AW_HIDE is used or expand outward if the AW_HIDE is not used.
        /// </summary>
        public const int AW_CENTER = 0X10;
        /// <summary>
        /// Hides the window. By default, the window is shown.
        /// </summary>
        public const int AW_HIDE = 0X10000;
        /// <summary>
        /// Activates the window.
        /// </summary>
        public const int AW_ACTIVATE = 0X20000;
        /// <summary>
        /// Uses slide animation. By default, roll animation is used.
        /// </summary>
        public const int AW_SLIDE = 0X40000;
        /// <summary>
        /// Uses a fade effect. 
        /// This flag can be used only if hwnd is a top-level window.
        /// </summary>
        public const int AW_BLEND = 0X80000;
        /// <summary>
        /// Animates a window.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int AnimateWindow(IntPtr hwand, int dwTime, int dwFlags);

        public static void SlideOut(Form form)
        {
            AnimateWindow(form.Handle, 200, AW_HIDE | AW_SLIDE | AW_VER_NEGATIVE);
        }

        public static void SlideIn(Form form)
        {
            AnimateWindow(form.Handle, 200, AW_SLIDE | AW_VER_POSITIVE);
        }

        public static void FadeIn(Form form)
        {
            AnimateWindow(form.Handle, 200, AW_CENTER);
        }
     
        public static void FadeOut(Form form)
        {
            AnimateWindow(form.Handle, 200, AW_CENTER | AW_HIDE);
        }
    }

}
