using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace QuakeConsole
{
    class QuakeTerminal : Panel
    {
        [DllImport("user32.dll")]
        static extern int SetParent(IntPtr hWndChild, int hWndParent);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, int bRepaint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetWindowLong")]
        static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, EntryPoint = "DestroyWindow")]
        static extern IntPtr DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetLayeredWindowAttributes")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);


        //PInvoke declarations
        [DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Auto)]
        internal static extern IntPtr CreateWindowEx(int dwExStyle,
        string lpszClassName,
        string lpszWindowName,
        int style,
        int x, int y,
        int width, int height,
        IntPtr hwndParent,
        IntPtr hMenu,
        IntPtr hInst,
        [MarshalAs(UnmanagedType.AsAny)] object pvParam);


        readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        readonly IntPtr HWND_TOP = new IntPtr(0);
        readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly UInt32 SWP_NOSIZE = 1;
        static readonly UInt32 SWP_NOMOVE = 2;
        static readonly UInt32 SWP_NOZORDER = 4;
        static readonly UInt32 SWP_NOREDRAW = 8;
        static readonly UInt32 SWP_NOACTIVATE = 16;
        static readonly UInt32 SWP_FRAMECHANGED = 32;
        static readonly UInt32 SWP_SHOWWINDOW = 64;
        static readonly UInt32 SWP_HIDEWINDOW = 128;
        static readonly UInt32 SWP_NOCOPYBITS = 256;
        static readonly UInt32 SWP_NOOWNERZORDER = 512;
        static readonly UInt32 SWP_NOSENDCHANGING = 1024;
        static readonly Int32 WM_CLOSE = 0xF060;
        static readonly Int32 WM_QUIT = 0x0012;

        enum WindowStyles : long
        {
            WS_OVERLAPPED = 0x00000000L,
            WS_POPUP = 0x80000000L,
            WS_CHILD = 0x40000000L,
            WS_MINIMIZE = 0x20000000L,
            WS_VISIBLE = 0x10000000L,
            WS_DISABLED = 0x08000000L,
            WS_CLIPSIBLINGS = 0x04000000L,
            WS_CLIPCHILDREN = 0x02000000L,
            WS_MAXIMIZE = 0x01000000L,
            WS_CAPTION = 0x00C00000L,
            WS_BORDER = 0x00800000L,
            WS_DLGFRAME = 0x00400000L,
            WS_VSCROLL = 0x00200000L,
            WS_HSCROLL = 0x00100000L,
            WS_SYSMENU = 0x00080000L,
            WS_THICKFRAME = 0x00040000L,
            WS_GROUP = 0x00020000L,
            WS_TABSTOP = 0x00010000L,
            WS_MINIMIZEBOX = 0x00020000L,
            WS_MAXIMIZEBOX = 0x00010000L
        }
        const int GWL_STYLE = -16;
        const int GWL_EXSTYLE = -20;
        const int WS_EX_LAYERED = 0x80000;
        const int LWA_ALPHA = 0x2;


        private IntPtr GEHrender = (IntPtr)0;
        private IntPtr GEParentHrender = (IntPtr)0;

        internal const int
        WS_CHILD = 0x40000000,
        WS_VISIBLE = 0x10000000,
        LBS_NOTIFY = 0x00000001,
        HOST_ID = 0x00000002,
        LISTBOX_ID = 0x00000001,
        WS_VSCROLL = 0x00200000,
        WS_BORDER = 0x00800000;

        Process TerminalProcess;

        public QuakeTerminal() : base()
        {
            Dock = DockStyle.Fill;
            Start();
            Style();
            Redraw();

            Resize += (object sender, EventArgs e) => Redraw();
            VisibleChanged += (object sender, EventArgs e) => Redraw();
        }

        
        private void Start()
        {
            var startInfo = new ProcessStartInfo("c:\\cygwin\\bin\\mintty.exe");
            startInfo.Arguments = " - ";
            startInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            startInfo.LoadUserProfile = true;
            TerminalProcess = Process.Start(startInfo);
            TerminalProcess.EnableRaisingEvents = true;
            TerminalProcess.WaitForInputIdle();
        }

        private void Style()
        {
            var style = new IntPtr((uint)WindowStyles.WS_VISIBLE | (uint)WindowStyles.WS_MAXIMIZE);
            SetWindowLongPtr(ChildHandle, GWL_STYLE, style);
            SetParent(ChildHandle, (int)Handle);

            var exstyle = new IntPtr(WS_EX_LAYERED);
            SetWindowLongPtr(ChildHandle, GWL_EXSTYLE, exstyle);
            SetLayeredWindowAttributes(ChildHandle, 0, 28, LWA_ALPHA);
        }

        private void Redraw()
        {
            MoveWindow(ChildHandle, 0, 0, (int)this.Width, (int)this.Height, 1);
        }

        private IntPtr ChildHandle
        {
            get
            {
                return (IntPtr)TerminalProcess.MainWindowHandle;
            }
        }
    }
}
