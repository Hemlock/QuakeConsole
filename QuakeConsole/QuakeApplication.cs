using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;

namespace QuakeConsole
{
    class QuakeApplication : CaptionPanel, FocusableApplication
    {
        [DllImport("user32.dll")]
        static extern int SetParent(IntPtr hWndChild, int hWndParent);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, int bRepaint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, EntryPoint = "SetWindowLong")]
        static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(HandleRef hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(HandleRef hWnd, StringBuilder lpString, int nMaxCount);

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

        const int 
            GWL_STYLE = -16,
            GWL_EXSTYLE = -20,
            WS_EX_LAYERED = 0x80000,
            LWA_ALPHA = 0x2;


        private IntPtr 
            GEHrender = (IntPtr)0,
            GEParentHrender = (IntPtr)0;

        internal const int
            WS_CHILD = 0x40000000,
            WS_VISIBLE = 0x10000000,
            LBS_NOTIFY = 0x00000001,
            HOST_ID = 0x00000002,
            LISTBOX_ID = 0x00000001,
            WS_VSCROLL = 0x00200000,
            WS_BORDER = 0x00800000;

        public const uint 
            EVENT_SYSTEM_FOREGROUND = 3,
            EVENT_OBJECT_NAMECHANGE = 0x800C,
            WINEVENT_OUTOFCONTEXT = 0;


        Process ApplicationProcess;
        public event EventHandler 
            ApplicationBlurred,
            ApplicationFocused;

        private bool _HasFocus = false;

        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        WinEventDelegate WindowsEventDelegate;
        IntPtr WindowsEventHook;
        EventHandler ApplicationExitedHandler;

        public QuakeApplication() : base()
        {
            Dock = DockStyle.Fill;
            Margin = new Padding(0);
            Resize += (object sender, EventArgs e) => Redraw();
            VisibleChanged += (object sender, EventArgs e) => Redraw();
            ApplicationExitedHandler = new EventHandler(ApplicationExited);
            Application.ApplicationExit += ApplicationExitedHandler;
            SelectApplication();
        }

        void ApplicationExited(object sender, EventArgs e)
        {
            ExitApplication();
        }

        private void SelectApplication()
        {
            var apps = (from app in
                            (from string app in Properties.Settings.Default.Applications select app.Split(','))
                        select new { Name = app[0], Executable = app[1], Arguments = app[2] }).ToArray();
            
            var list = new ListView();
            list.Dock = DockStyle.Fill;
            list.Items.Clear();
            list.Columns.Clear();            
            list.Columns.Add("Name", 0).AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            list.Columns.Add("Executable", 0).AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            list.Columns.Add("Arguments", 0).AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            list.LargeImageList = new ImageList();
            list.LargeImageList.ImageSize = new Size(32, 32);
            
            foreach (var app in apps) {
                var item = new ListViewItem(new string[] { app.Name, app.Executable, app.Arguments });
                if (!list.LargeImageList.Images.ContainsKey(app.Executable))
                {
                    list.LargeImageList.Images.Add(app.Executable, Icon.ExtractAssociatedIcon(app.Executable));
                }
                item.ImageKey = app.Executable;
                list.Items.Add(item);
            }

            list.DoubleClick += (sender, e) =>
            {
                var l = sender as ListView;
                if (l == null)
                {
                    return;
                }

                if (l.SelectedIndices.Count > 0)
                {
                    var index = l.SelectedIndices[0];
                    var app = apps[index];
                    Controls.Remove(list);
                    Start(app.Executable, app.Arguments);
                }
            };

            Controls.Add(list);
        }
        
        private void Start(string command, string arguments)
        {
            //var startInfo = new ProcessStartInfo(Properties.Settings.Default.Executable);
            var startInfo = new ProcessStartInfo(command);
            startInfo.Arguments = arguments;
            startInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            startInfo.LoadUserProfile = true;
            startInfo.RedirectStandardOutput = false;
            startInfo.RedirectStandardError = false;
            startInfo.UseShellExecute = false;

            ApplicationProcess = Process.Start(startInfo);
            ApplicationProcess.EnableRaisingEvents = true;
            ApplicationProcess.Exited +=
                (object sender, EventArgs e) => this.Invoke(new MethodInvoker(delegate { ApplicationExited(); }));
            ApplicationProcess.WaitForInputIdle();

            Style();
            Redraw();

            // setup up the hook to watch for all EVENT_SYSTEM_FOREGROUND events system wide
            WindowsEventDelegate = new WinEventDelegate(WinEventProc);
            WindowsEventHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_OBJECT_NAMECHANGE,
                IntPtr.Zero, WindowsEventDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);
        }
        
        public void FocusApplication()
        {
            if (GetForegroundWindow() != ChildHandle)
            {
                SetForegroundWindow(ChildHandle);
            }
            HasFocus = true;
        }

        public void ExitApplication()
        {
            if (!ApplicationProcess.HasExited)
            {
                ApplicationProcess.CloseMainWindow();
                ApplicationProcess.WaitForExit(1000);
                Application.ApplicationExit -= ApplicationExitedHandler;
            }
        }
        
        private void ApplicationExited()
        {
            Parent.Controls.Remove(this);
        }

        private void Style()
        {
            var style = new IntPtr((uint)WindowStyles.WS_VISIBLE | (uint)WindowStyles.WS_MAXIMIZE);
            SetWindowLongPtr(ChildHandle, GWL_STYLE, style);
            SetParent(ChildHandle, (int)Handle);

            var exstyle = new IntPtr(WS_EX_LAYERED);
            SetWindowLongPtr(ChildHandle, GWL_EXSTYLE, exstyle);
        }

        private void Redraw()
        {
            MoveWindow(ChildHandle, Margin.Left + Padding.Left, Margin.Top + Padding.Top,  
                ClientSize.Width - (Margin.Left + Padding.Left + Margin.Right + Padding.Right), 
                ClientSize.Height - (Margin.Top + Padding.Top + Margin.Bottom + Padding.Bottom), 1);
        }

        private IntPtr ChildHandle
        {
            get
            {
                return ApplicationProcess == null ? (IntPtr)null : (IntPtr)ApplicationProcess.MainWindowHandle;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }

        void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            // if we got the EVENT_SYSTEM_FOREGROUND, and the hwnd is the putty terminal hwnd (m_AppWin)
            // then bring the supperputty window to the foreground
            bool isThis = (hwnd == ChildHandle);
            if (eventType == EVENT_SYSTEM_FOREGROUND)
            {
                HasFocus = (GetForegroundWindow() == ChildHandle);
            }
            else if (isThis && eventType == EVENT_OBJECT_NAMECHANGE)
            {
                SetCaptionFromApplication();     
            }
        }

        private void SetCaptionFromApplication()
        {
            int capacity = GetWindowTextLength(new HandleRef(this, ChildHandle)) * 2;
            StringBuilder stringBuilder = new StringBuilder(capacity);
            GetWindowText(new HandleRef(this, ChildHandle), stringBuilder, stringBuilder.Capacity);
            Caption = stringBuilder.ToString();
        }

        public bool HasFocus
        {
            get 
            {
                return _HasFocus;
            }

            set
            {
                _HasFocus = value;
                var settings = Properties.Settings.Default;
                var e = new EventArgs();
                if (value)
                {
                    if (ApplicationFocused != null) ApplicationFocused(this, e);
                    BackColor = ColorTranslator.FromHtml(settings.FocusedCaptionBackgroundColor);
                    CaptionColor = ColorTranslator.FromHtml(settings.FocusedCaptionColor);
                }
                else
                {
                    if (ApplicationBlurred != null) ApplicationBlurred(this, e);
                    BackColor = ColorTranslator.FromHtml(settings.CaptionBackgroundColor);
                    CaptionColor = ColorTranslator.FromHtml(settings.CaptionColor);
                }

            }
        }

    }
}
