using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace GenXdev.Helpers
{
    public enum DeviceCap
    {
        VERTRES = 10,
        DESKTOPVERTRES = 117,

        // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
    }

    public static class DesktopInfo
    {
        // Place these at the top of WindowObj, near your other constants and P/Invokes
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [PreserveSig()]
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDC(
            string lpszDriver,
            string lpszDevice,
            IntPtr lpszOutput,
            IntPtr lpInitData
        );

        public static float getScalingFactor(int monitor)
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            var AllScreens = (from q in WpfScreenHelper.Screen.AllScreens select q).ToArray();
            IntPtr desktop = CreateDC(
                AllScreens[monitor].DeviceName,
                AllScreens[monitor].DeviceName,
                IntPtr.Zero,
                IntPtr.Zero
            );
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

            var a = new Microsoft.Extensions.Configuration.UserSecrets.PathHelper();
            if (a.ToString() == "need this dep") { System.Threading.Thread.Sleep(1); }

            return ScreenScalingFactor; // 1.25 = 125%
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags
        );
    }

    public class WindowObj
    {
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags
        );

        ///<summary>Windows handle to identify the current windows</summary>
        public IntPtr Handle { get; private set; }
        ///<summary>Name of the windows</summary>
        public string Title { get; private set; }
        ///<summary>Get the children of a window</summary>
        public ICollection<WindowObj> Children { get; private set; }

        /// <summary>
        /// Get a custom representation of the window class base on https://docs.microsoft.com/en-us/windows/win32/winmsg/about-window-classes documentation
        /// </summary>
        public string WindowType { get; private set; }

        /// <summary>
        /// Get the name of the class that represents the windows
        /// </summary>
        public string WindowClassName { get; private set; }

        private const UInt32 WM_CLOSE = 0x0010;
        public static int Amount = 0;

        // Constants for window styling
        private const int GWL_STYLE = -16;
        private const int GWL_EXSTYLE = -20;
        private const uint WS_CAPTION = 0x00C00000;
        private const uint WS_THICKFRAME = 0x00040000;
        private const uint WS_MINIMIZEBOX = 0x00020000;
        private const uint WS_MAXIMIZEBOX = 0x00010000;
        private const uint WS_SYSMENU = 0x00080000;
        private const uint WS_EX_TOPMOST = 0x00000008;
        private const uint WS_EX_LAYERED = 0x00080000;
        private const uint LWA_ALPHA = 0x00000002;

        // For window transparency
        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll")]
        private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        /// <summary>Creates a window object with a handle and a window title</summary>
        /// <param name="handle"></param>
        /// <param name="title"></param>
        public WindowObj(IntPtr handle, string title)
        {
            Handle = handle;
            Title = title;

            StringBuilder stringBuilder = new StringBuilder(256);
            GetClassName(handle, stringBuilder, stringBuilder.Capacity);
            WindowType = GetWindowClass(stringBuilder.ToString());
            WindowClassName = stringBuilder.ToString();

            Children = new List<WindowObj>();
            ArrayList handles = new ArrayList();
            EnumedWindow childProc = GetWindowHandle;

            EnumChildWindows(handle, childProc, handles);
            foreach (IntPtr item in handles)
            {
                int capacityChild = GetWindowTextLength(handle) * 2;

                StringBuilder stringBuilderChild = new StringBuilder(capacityChild);
                GetWindowText(handle, stringBuilder, stringBuilderChild.Capacity);

                StringBuilder stringBuilderChild2 = new StringBuilder(256);
                GetClassName(handle, stringBuilderChild2, stringBuilderChild2.Capacity);

                WindowObj win = new WindowObj(item, stringBuilder.ToString());
                win.WindowClassName = stringBuilderChild2.ToString();
                win.WindowType = GetWindowClass(stringBuilderChild2.ToString());
                Children.Add(win);
            }
        }

        [DllImport("user32.dll")]
        public static extern void mouse_event(Int32 dwFlags, Int32 dx, Int32 dy, Int32 dwData, UIntPtr dwExtraInfo);

        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MONITORPOWER = 0xF170;
        private const int MonitorTurnOn = -1;
        private const int MonitorShutoff = 2;


        public static void WakeMonitor()
        {
            //Turn them on
            PostMessage((IntPtr)0xffff, WM_SYSCOMMAND, SC_MONITORPOWER, MonitorTurnOn);
            System.Threading.Thread.Sleep(150);
            mouse_event(MOUSEEVENTF_MOVE, 0, 1, 0, UIntPtr.Zero);
            System.Threading.Thread.Sleep(40);
            mouse_event(MOUSEEVENTF_MOVE, 0, -1, 0, UIntPtr.Zero);
        }
        public static void SleepMonitor()
        {

            //Turn them off
            PostMessage((IntPtr)0xffff, WM_SYSCOMMAND, SC_MONITORPOWER, MonitorShutoff);
        }
        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();


        public static bool GetWindowHandle(IntPtr windowHandle, ArrayList windowHandles)
        {
            windowHandles.Add(windowHandle);
            return true;
        }

        ///<summary>A class to have better manipulation of windows sizes</summary>
        public struct RectStruct
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }

        }

        /// <summary>Open a new Process with the given path and return a window object if the process have a user interface, else return null</summary>
        /// <param name="filePath">Path of the file to look for</param>
        /// <param name="timeToWait">Time to wait until the proccess execute, *Only apply for process with a window interface*</param>
        public static WindowObj Open(string filePath, int timeToWait = -1)
        {
            if (!File.Exists(filePath))
                throw new Exception(string.Format("The filePath {0} is not valid", filePath));

            Process newProcess = Process.Start(filePath);

            if (timeToWait == -1)
                newProcess.WaitForInputIdle();
            else
                newProcess.WaitForInputIdle(timeToWait * 1000);

            if (newProcess != null && newProcess.MainWindowHandle != IntPtr.Zero)
                return new WindowObj(newProcess.MainWindowHandle, newProcess.MainWindowTitle);

            return null;
        }

        /// <summary>Look for the existence of a process with the given name an return the first occurrence of the process as a Window object</summary>
        /// <param name="name">Name of the process</param>
        /// <param name="attempts">Amount of tries that it will look for the window</param>
        /// <param name="waitInterval">Amount ot time it will stop the thread while waiting for the windows in each attemp</param>
        public static WindowObj GetWindow(string name, int attempts = 1, int waitInterval = 1000)
        {
            IEnumerable<Process> currentProcesses = Process.GetProcesses();
            int counter = 0;
            while (counter < attempts)
            {
                foreach (Process p in currentProcesses)
                    if (p.MainWindowHandle != IntPtr.Zero && p.MainWindowTitle == name)
                        return new WindowObj(p.MainWindowHandle, p.MainWindowTitle);

                System.Threading.Thread.Sleep(waitInterval);
                currentProcesses = Process.GetProcesses();
                counter++;
            }
            return null;
        }

        public static WindowObj GetFocusedWindow()
        {
            var sb = new StringBuilder();
            var handle = GetForegroundWindow();

            return new WindowObj(handle, GetWindowTitle(handle));
        }

        /// <summary>Look for the existence of processes with the given name an return all occurrences of the process as Windows objects, *case sensitive*</summary>
        /// <param name="name">Name of the process</param>
        /// <param name="attempts">Amount of tries that it will look for the window</param>
        /// <param name="waitInterval">Amount ot time it will stop the thread while waiting for the windows in each attemp</param>
        public static IEnumerable<WindowObj> GetWindows(string name, int attempts = 1, int waitInterval = 1000)
        {
            IEnumerable<Process> currentProcesses = Process.GetProcesses();
            ICollection<WindowObj> windows = new List<WindowObj>();
            int counter = 0;
            while (counter < attempts)
            {
                foreach (Process p in currentProcesses)
                    if (p.MainWindowHandle != IntPtr.Zero && p.ProcessName == name)
                        windows.Add(new WindowObj(p.MainWindowHandle, p.MainWindowTitle));

                if (windows.Count > 0)
                    break;

                System.Threading.Thread.Sleep(waitInterval);
                currentProcesses = Process.GetProcesses();
                counter++;
            }
            return windows;
        }

        public static IEnumerable<WindowObj> GetMainWindow(Process p, int attempts = 1, int waitInterval = 1000)
        {
            IEnumerable<Process> currentProcesses = Process.GetProcesses();
            ICollection<WindowObj> windows = new List<WindowObj>();
            int counter = 0;
            while (counter < attempts)
            {
                if (p.MainWindowHandle != IntPtr.Zero)
                {
                    try
                    {
                        windows.Add(new WindowObj(p.MainWindowHandle, p.MainWindowTitle));
                    }
                    catch
                    {
                        break;
                    }
                }

                if (windows.Count > 0)
                    break;

                System.Threading.Thread.Sleep(waitInterval);
                currentProcesses = Process.GetProcesses();
                counter++;
            }

            return windows;
        }

        public static IEnumerable<WindowObj> GetMainWindow(IntPtr windowHandle, int attempts = 1, int waitInterval = 1000)
        {
            IEnumerable<Process> currentProcesses = Process.GetProcesses();
            ICollection<WindowObj> windows = new List<WindowObj>();
            if (windowHandle != IntPtr.Zero)
            {
                try
                {
                    windows.Add(new WindowObj(windowHandle, ""));
                }
                catch { }
            }

            return windows;
        }
        /// <summary>Look for the existense of a process in all processes an return the first ocurrence of a process that contains the given name as a Window object</summary>
        /// <param name="name">Name of the process</param>
        /// <param name="attempts">Amount of tries that it will look for the window</param>
        /// <param name="waitInterval">Amount ot time it will stop the thread while waiting for the windows in each attemp</param>
        public static WindowObj GetWindowWithPartialName(string name, int attempts = 1, int waitInterval = 1000)
        {
            IEnumerable<Process> currentProcesses = Process.GetProcesses();
            int counter = 0;
            while (counter < attempts)
            {
                foreach (Process p in currentProcesses)
                    if (p.MainWindowHandle != IntPtr.Zero && p.MainWindowTitle.ToLower().Contains(name.ToLower()))
                        return new WindowObj(p.MainWindowHandle, p.MainWindowTitle);

                System.Threading.Thread.Sleep(waitInterval);
                currentProcesses = Process.GetProcesses();
                counter++;
            }
            return null;
        }

        /// <summary>Look for the existense of a process in all processes an return the processes that contains the given name as Windows objects</summary>
        /// <param name="name">Name of the process</param>
        /// <param name="attempts">Amount of tries that it will look for at least one window</param>
        /// <param name="waitInterval">Amount ot time it will stop the thread while waiting for the windows in each attemp</param>
        public static IEnumerable<WindowObj> GetWindowsWithPartialName(string name, int attempts = 1, int waitInterval = 1000)
        {
            IEnumerable<Process> currentProcesses = Process.GetProcesses();
            ICollection<WindowObj> windows = new List<WindowObj>();
            int counter = 0;
            while (counter < attempts)
            {
                foreach (Process p in currentProcesses)
                    if (p.MainWindowHandle != IntPtr.Zero && p.MainWindowTitle.ToLower().Contains(name.ToLower()))
                        windows.Add(new WindowObj(p.MainWindowHandle, p.MainWindowTitle));

                if (windows.Count > 0)
                    break;

                System.Threading.Thread.Sleep(waitInterval);
                currentProcesses = Process.GetProcesses();
                counter++;
            }
            return windows;
        }

        /// <summary>
        /// Get the active windows if possible.
        /// </summary>
        /// <returns></returns>
        public static WindowObj GetActive()
        {
            IntPtr handle = GetActiveWindow();
            if (handle != IntPtr.Zero)
            {
                foreach (Process p in Process.GetProcesses())
                    if (p.MainWindowHandle == handle)
                        return new WindowObj(p.MainWindowHandle, p.MainWindowTitle);
            }
            return null;
        }

        /// <summary>Focus the current window</summary>
        public void Focus()
        {
            AllowSetForegroundWindow(-1);
            SetForegroundWindow(this.Handle);
            SetFocus(Handle);
        }

        public void SetForeground()
        {
            // Temporarily set as topmost if not already

            // Show the window (SW_SHOW = 5)
            if (IsMinimized())
            {
                ShowWindowAsync(Handle, (int)ShowWindowCommands.Restore);
                System.Threading.Thread.Sleep(50);
            }

            // Ensure window is visible but don't activate it
            if (!IsVisible())
            {
                ShowWindowAsync(Handle, (int)ShowWindowCommands.ShowNoActivate);
            }

            SetForegroundWindow(Handle);
            BringWindowToTop(Handle);

            // Ensure the window is the active window and has keyboard focus
            SetActiveWindow(Handle);
            SetFocus(Handle);
        }

        /// <summary>Maximize the current window</summary>
        public bool Maximize()
        {
            return ShowWindowAsync(this.Handle, (int)ShowWindowCommands.Maximize);
        }

        /// <summary>Minimize the current window</summary>
        public bool Minimize()
        {
            return ShowWindowAsync(this.Handle, (int)ShowWindowCommands.Minimize);
        }

        /// <summary>Return the current windows at its first state when the windows was created</summary>
        public bool Restore()
        {
            return ShowWindowAsync(this.Handle, (int)ShowWindowCommands.Restore);
        }

        /// <summary>Return the current windows at its default state</summary>
        public bool DefaultState()
        {
            return ShowWindowAsync(this.Handle, (int)ShowWindowCommands.ShowDefault);
        }

        public static string GetWindowTitle(IntPtr hWnd)
        {
            var length = GetWindowTextLength(hWnd) + 1;
            var title = new StringBuilder(length);
            GetWindowText(hWnd, title, length);
            return title.ToString();
        }

        /// <summary>Hide the current window
        /// *If the application close with a hide process, this will not be close unless close method
        /// calls or manually kill the process*</summary>
        public bool Hide()
        {
            return ShowWindowAsync(this.Handle, (int)ShowWindowCommands.Hide);
        }

        /// <summary>Shows the current windows if it was hide</summary>
        public bool Show()
        {
            return ShowWindowAsync(this.Handle, (int)ShowWindowCommands.Show);
        }

        /// <summary>Close the current windows</summary>
        public bool Close()
        {
            return SendMessage(this.Handle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero) == IntPtr.Zero;
        }
        public bool SendMessage(UInt32 Msg, IntPtr wParam, IntPtr lParam)
        {
            return SendMessage(this.Handle, Msg, wParam, lParam) == IntPtr.Zero;
        }

        /// <summary>Resize the current window with the given params</summary>
        /// <param name="width">New width of the current windows</param>
        /// <param name="height">New height of the current windows</param>
        public bool Resize(int width, int height)
        {
            return MoveWindow(this.Handle, 0, 0, width, height, true);
        }

        /// <summary>Resize the current window with the given params</summary>
        /// <param name="pixels">this will use as new width and new height of the windows</param>
        public bool Resize(int pixels)
        {
            return MoveWindow(this.Handle, 0, 0, pixels, pixels, true);
        }

        /// <summary>Move the current window with the given params</summary>
        /// <param name="X">New X coordinate of the current windows</param>
        /// <param name="Y">New Y coordinate of the current windows</param>
        public bool Move(int X, int Y)
        {
            RectStruct rect = new RectStruct();
            GetWindowRect(this.Handle, ref rect);

            rect.Width = rect.Right - rect.Left + Amount;
            rect.Height = rect.Bottom - rect.Top + Amount;
            return MoveWindow(this.Handle, X, Y, rect.Width, rect.Height, true);
        }
        public bool Move(int X, int Y, int W, int H)
        {
            return MoveWindow(this.Handle, X, Y, W, H, true);
        }

        /// <summary>Return the position of the windows as X, Y coordinates</summary>
        public Point Position()
        {
            RectStruct rect = new RectStruct();
            GetWindowRect(this.Handle, ref rect);

            return new Point(rect.Left, rect.Top);
        }

        public int Left
        {

            get
            {
                return Position().X;
            }

            set
            {
                Move(value, Position().Y);
            }
        }
        public int Top
        {

            get
            {
                return Position().Y;
            }

            set
            {
                Move(Position().X, value);
            }
        }

        public int Width
        {

            get
            {
                return Size().Width;
            }

            set
            {
                Resize(value, Size().Height);
            }
        }
        public int Height
        {

            get
            {
                return Size().Height;
            }

            set
            {
                Resize(Size().Width, value);
            }
        }

        /// <summary>Return the Size of the windows as width, height coordinates</summary>
        public Size Size()
        {
            RectStruct rect = new RectStruct();
            GetWindowRect(this.Handle, ref rect);

            rect.Width = rect.Right - rect.Left + Amount;
            rect.Height = rect.Bottom - rect.Top + Amount;
            return new Size(rect.Width, rect.Height);
        }

        /// <summary>Return the position and size of the windows as X, Y, with, height coordinates</summary>
        /*
        public Rect Area()
        {
            RectStruct rect = new RectStruct();
            GetWindowRect(this.Handle, ref rect);

            rect.Width = rect.Right - rect.Left + Amount;
            rect.Height = rect.Bottom - rect.Top + Amount;
            return new System.Windows.(rect.Left, rect.Top, rect.Width, rect.Height);
        }
        */

        /// <summary>Check if the current windows is visible</summary>
        public bool IsVisible()
        {
            return IsWindowVisible(this.Handle);
        }
        [DllImport("user32.dll")]
        public static extern bool AllowSetForegroundWindow(int dwProcessId);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RectStruct rectangle);

        [DllImport("kernel32.dll")]
        public static extern int GetProcessId(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetFocus(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        [DllImport("user32.dll")]
        public static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public delegate bool EnumedWindow(IntPtr handleWindow, ArrayList handles);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr window, EnumedWindow callback, ArrayList lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        private enum ShowWindowCommands
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            Hide = 0,
            /// <summary>
            /// Activates and displays a window. If the window is minimized or
            /// maximized, the system restores it to its original size and position.
            /// An application should specify this flag when displaying the window
            /// for the first time.
            /// </summary>
            Normal = 1,
            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            ShowMinimized = 2,
            /// <summary>
            /// Maximizes the specified window.
            /// </summary>
            Maximize = 3, // is this the right value?
            /// <summary>
            /// Activates the window and displays it as a maximized window.
            /// </summary>
            ShowMaximized = 3,
            /// <summary>
            /// Displays a window in its most recent size and position. This value
            /// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except
            /// the window is not activated.
            /// </summary>
            ShowNoActivate = 4,
            /// <summary>
            /// Activates the window and displays it in its current size and position.
            /// </summary>
            Show = 5,
            /// <summary>
            /// Minimizes the specified window and activates the next top-level
            /// window in the Z order.
            /// </summary>
            Minimize = 6,
            /// <summary>
            /// Displays the window as a minimized window. This value is similar to
            /// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the
            /// window is not activated.
            /// </summary>
            ShowMinNoActive = 7,
            /// <summary>
            /// Displays the window in its current size and position. This value is
            /// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the
            /// window is not activated.
            /// </summary>
            ShowNA = 8,
            /// <summary>
            /// Activates and displays the window. If the window is minimized or
            /// maximized, the system restores it to its original size and position.
            /// An application should specify this flag when restoring a minimized window.
            /// </summary>
            Restore = 9,
            /// <summary>
            /// Sets the show state based on the SW_* value specified in the
            /// STARTUPINFO structure passed to the CreateProcess function by the
            /// program that started the application.
            /// </summary>
            ShowDefault = 10,
            /// <summary>
            ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread
            /// that owns the window is not responding. This flag should only be
            /// used when minimizing windows from a different thread.
            /// </summary>
            ForceMinimize = 11
        }
        private string GetWindowClass(string windowClass)
        {
            List<string> values = new List<string>(){
                "ComboLBox","DDEMLEvent","Message","#32768",
                "#32769","#32770","#32771","#32772","Button","Edit","ListBox","MDIClient",
                "ScrollBar","Static",""
            };

            if (windowClass == "#32768")
                return "Menu";
            else if (windowClass == "#32769")
                return "DektopWindow";
            else if (windowClass == "#32770")
                return "DialogBox";
            else if (windowClass == "#32771")
                return "TaskSwitchWindowClass";
            else if (windowClass == "#32772")
                return "IconTitlesClass";
            return values.SingleOrDefault(s => s == windowClass);
        }

        /// <summary>
        /// Sets or removes the "Always On Top" state of the window
        /// </summary>
        /// <param name="alwaysOnTop">True to set the window always on top, false otherwise</param>
        /// <returns>True if successful</returns>
        public bool SetAlwaysOnTop(bool alwaysOnTop)
        {
            uint exStyle = GetWindowLong(Handle, GWL_EXSTYLE);

            if (alwaysOnTop)
                exStyle |= WS_EX_TOPMOST;
            else
                exStyle &= ~WS_EX_TOPMOST;

            return SetWindowLong(Handle, GWL_EXSTYLE, exStyle) != 0;
        }

        /// <summary>
        /// Sets the opacity/transparency level of the window
        /// </summary>
        /// <param name="opacity">Opacity level from 0 (transparent) to 255 (opaque)</param>
        /// <returns>True if successful</returns>
        public bool SetOpacity(byte opacity)
        {
            uint exStyle = GetWindowLong(Handle, GWL_EXSTYLE);
            exStyle |= WS_EX_LAYERED;
            SetWindowLong(Handle, GWL_EXSTYLE, exStyle);
            return SetLayeredWindowAttributes(Handle, 0, opacity, LWA_ALPHA);
        }

        // Struct to save window position and state
        [Serializable]
        public class WindowState
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public bool IsMaximized { get; set; }
            public bool IsMinimized { get; set; }
            public bool IsAlwaysOnTop { get; set; }
            public byte Opacity { get; set; }
            public string Title { get; set; }
        }

        /// <summary>
        /// Captures the current state of the window
        /// </summary>
        /// <returns>A WindowState object containing the window's current state</returns>
        public WindowState CaptureState()
        {
            RectStruct rect = new RectStruct();
            GetWindowRect(Handle, ref rect);

            return new WindowState
            {
                X = rect.Left,
                Y = rect.Top,
                Width = rect.Right - rect.Left,
                Height = rect.Bottom - rect.Top,
                IsMaximized = IsMaximized(),
                IsMinimized = IsMinimized(),
                IsAlwaysOnTop = IsAlwaysOnTop(),
                Opacity = GetOpacity(),
                Title = Title
            };
        }

        /// <summary>
        /// Restores a previously captured window state
        /// </summary>
        /// <param name="state">The WindowState to restore</param>
        public void RestoreState(WindowState state)
        {
            if (state.IsMaximized)
                Maximize();
            else if (state.IsMinimized)
                Minimize();
            else
                Move(state.X, state.Y, state.Width, state.Height);

            SetAlwaysOnTop(state.IsAlwaysOnTop);
            SetOpacity(state.Opacity);
        }

        /// <summary>
        /// Checks if the window is currently maximized
        /// </summary>
        public bool IsMaximized()
        {
            return IsZoomed(Handle);
        }

        /// <summary>
        /// Checks if the window is currently minimized
        /// </summary>
        public bool IsMinimized()
        {
            return IsIconic(Handle);
        }

        /// <summary>
        /// Checks if the window is set to always be on top
        /// </summary>
        public bool IsAlwaysOnTop()
        {
            uint exStyle = GetWindowLong(Handle, GWL_EXSTYLE);
            return (exStyle & WS_EX_TOPMOST) != 0;
        }

        /// <summary>
        /// Gets the current opacity of the window
        /// </summary>
        public byte GetOpacity()
        {
            uint exStyle = GetWindowLong(Handle, GWL_EXSTYLE);
            if ((exStyle & WS_EX_LAYERED) == 0)
                return 255;

            byte opacity = 255;
            uint flags = 0;
            uint key = 0;
            GetLayeredWindowAttributes(Handle, ref key, ref opacity, ref flags);
            return opacity;
        }

        /// <summary>
        /// Fades the window in or out with animation
        /// </summary>
        /// <param name="fadeIn">True to fade in, false to fade out</param>
        /// <param name="duration">Duration of the animation in milliseconds</param>
        public void FadeWindow(bool fadeIn, int duration = 200)
        {
            byte startOpacity = fadeIn ? (byte)0 : (byte)255;
            byte endOpacity = fadeIn ? (byte)255 : (byte)0;
            int steps = 10;
            int delay = duration / steps;

            SetOpacity(startOpacity);
            if (fadeIn) Show();

            for (int i = 1; i <= steps; i++)
            {
                byte currentOpacity = (byte)(startOpacity + ((endOpacity - startOpacity) * i / steps));
                SetOpacity(currentOpacity);
                System.Threading.Thread.Sleep(delay);
            }

            if (!fadeIn) Hide();
        }

        /// <summary>
        /// Snaps the window to the nearest screen edge
        /// </summary>
        public void SnapToEdge()
        {
            var position = Position();
            var size = Size();
            var screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point(position.X, position.Y));

            int snapDistance = 20; // pixels
            var workArea = screen.WorkingArea;

            int x = position.X;
            int y = position.Y;

            // Snap to left edge
            if (Math.Abs(position.X - workArea.Left) < snapDistance)
                x = workArea.Left;

            // Snap to right edge
            if (Math.Abs((position.X + size.Width) - workArea.Right) < snapDistance)
                x = workArea.Right - size.Width;

            // Snap to top edge
            if (Math.Abs(position.Y - workArea.Top) < snapDistance)
                y = workArea.Top;

            // Snap to bottom edge
            if (Math.Abs((position.Y + size.Height) - workArea.Bottom) < snapDistance)
                y = workArea.Bottom - size.Height;

            Move(x, y);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsZoomed(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetLayeredWindowAttributes(IntPtr hwnd, ref uint crKey, ref byte bAlpha, ref uint dwFlags);

        public void RemoveBorder()
        {
            uint style = GetWindowLong(this.Handle, GWL_STYLE);
            style &= ~(WS_CAPTION | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX | WS_SYSMENU);
            SetWindowLong(this.Handle, GWL_STYLE, style);
        }

        /// <summary>
        /// Positions the window on the left half of the screen
        /// </summary>
        public bool PositionLeft()
        {
            try
            {
                // First restore the window if it's maximized or minimized
                if (IsMaximized() || IsMinimized())
                {
                    Restore();
                    System.Threading.Thread.Sleep(50); // Give OS time to complete operation
                }

                var screen = System.Windows.Forms.Screen.FromHandle(Handle);
                int width = screen.WorkingArea.Width / 2;
                int height = screen.WorkingArea.Height;

                // Special handling for known problematic window classes or Electron apps
                bool isElectronApp = IsElectronApp();
                if (isElectronApp)
                {
                    // For Electron apps like VS Code, do an extra step to ensure it works
                    // First set a different size to force a refresh
                    MoveWindow(Handle, screen.WorkingArea.X, screen.WorkingArea.Y, width - 1, height, true);
                    System.Threading.Thread.Sleep(10);
                }

                return MoveWindow(Handle, screen.WorkingArea.X, screen.WorkingArea.Y, width, height, true);
            }
            catch
            {
                // Fallback to basic implementation if anything goes wrong
                try
                {
                    var screen = System.Windows.Forms.Screen.FromHandle(Handle);
                    int width = screen.WorkingArea.Width / 2;
                    int height = screen.WorkingArea.Height;
                    return MoveWindow(Handle, screen.WorkingArea.X, screen.WorkingArea.Y, width, height, true);
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Positions the window on the right half of the screen
        /// </summary>
        public bool PositionRight()
        {
            try
            {
                // First restore the window if it's maximized or minimized
                if (IsMaximized() || IsMinimized())
                {
                    Restore();
                    System.Threading.Thread.Sleep(50); // Give OS time to complete operation
                }

                var screen = System.Windows.Forms.Screen.FromHandle(Handle);
                int width = screen.WorkingArea.Width / 2;
                int height = screen.WorkingArea.Height;
                int x = screen.WorkingArea.X + screen.WorkingArea.Width - width;

                // Special handling for known problematic window classes or Electron apps
                bool isElectronApp = IsElectronApp();
                if (isElectronApp)
                {
                    // For Electron apps like VS Code, do an extra step to ensure it works
                    // First set a different size to force a refresh
                    MoveWindow(Handle, x, screen.WorkingArea.Y, width - 1, height, true);
                    System.Threading.Thread.Sleep(10);
                }

                return MoveWindow(Handle, x, screen.WorkingArea.Y, width, height, true);
            }
            catch
            {
                // Fallback to basic implementation if anything goes wrong
                try
                {
                    var screen = System.Windows.Forms.Screen.FromHandle(Handle);
                    int width = screen.WorkingArea.Width / 2;
                    int height = screen.WorkingArea.Height;
                    int x = screen.WorkingArea.X + screen.WorkingArea.Width - width;
                    return MoveWindow(Handle, x, screen.WorkingArea.Y, width, height, true);
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Positions the window on the top half of the screen
        /// </summary>
        public bool PositionTop()
        {
            var screen = System.Windows.Forms.Screen.FromHandle(Handle);
            int width = screen.WorkingArea.Width;
            int height = screen.WorkingArea.Height / 2;
            return MoveWindow(Handle, screen.WorkingArea.X, screen.WorkingArea.Y, width, height, true);
        }

        /// <summary>
        /// Positions the window on the bottom half of the screen
        /// </summary>
        public bool PositionBottom()
        {
            var screen = System.Windows.Forms.Screen.FromHandle(Handle);
            int width = screen.WorkingArea.Width;
            int height = screen.WorkingArea.Height / 2;
            int y = screen.WorkingArea.Y + screen.WorkingArea.Height - height;
            return MoveWindow(Handle, screen.WorkingArea.X, y, width, height, true);
        }

        /// <summary>
        /// Positions the window in the top-left corner of the screen (quarter screen)
        /// </summary>
        public bool PositionTopLeft()
        {
            var screen = System.Windows.Forms.Screen.FromHandle(Handle);
            int width = screen.WorkingArea.Width / 2;
            int height = screen.WorkingArea.Height / 2;
            return MoveWindow(Handle, screen.WorkingArea.X, screen.WorkingArea.Y, width, height, true);
        }

        /// <summary>
        /// Positions the window in the top-right corner of the screen (quarter screen)
        /// </summary>
        public bool PositionTopRight()
        {
            var screen = System.Windows.Forms.Screen.FromHandle(Handle);
            int width = screen.WorkingArea.Width / 2;
            int height = screen.WorkingArea.Height / 2;
            int x = screen.WorkingArea.X + screen.WorkingArea.Width - width;
            return MoveWindow(Handle, x, screen.WorkingArea.Y, width, height, true);
        }

        /// <summary>
        /// Positions the window in the bottom-left corner of the screen (quarter screen)
        /// </summary>
        public bool PositionBottomLeft()
        {
            var screen = System.Windows.Forms.Screen.FromHandle(Handle);
            int width = screen.WorkingArea.Width / 2;
            int height = screen.WorkingArea.Height / 2;
            int y = screen.WorkingArea.Y + screen.WorkingArea.Height - height;
            return MoveWindow(Handle, screen.WorkingArea.X, y, width, height, true);
        }

        /// <summary>
        /// Positions the window in the bottom-right corner of the screen (quarter screen)
        /// </summary>
        public bool PositionBottomRight()
        {
            var screen = System.Windows.Forms.Screen.FromHandle(Handle);
            int width = screen.WorkingArea.Width / 2;
            int height = screen.WorkingArea.Height / 2;
            int x = screen.WorkingArea.X + screen.WorkingArea.Width - width;
            int y = screen.WorkingArea.Y + screen.WorkingArea.Height - height;
            return MoveWindow(Handle, x, y, width, height, true);
        }

        /// <summary>
        /// Positions the window in the center of the screen
        /// </summary>
        public bool PositionCentered()
        {
            var screen = System.Windows.Forms.Screen.FromHandle(Handle);
            int width = (int)(screen.WorkingArea.Width * 0.8);
            int height = (int)(screen.WorkingArea.Height * 0.8);
            int x = screen.WorkingArea.X + (screen.WorkingArea.Width - width) / 2;
            int y = screen.WorkingArea.Y + (screen.WorkingArea.Height - height) / 2;
            return MoveWindow(Handle, x, y, width, height, true);
        }

        /// <summary>
        /// Positions the window to fill the entire screen (without maximizing)
        /// </summary>
        public bool PositionFullscreen()
        {
            var screen = System.Windows.Forms.Screen.FromHandle(Handle);
            return MoveWindow(Handle, screen.WorkingArea.X, screen.WorkingArea.Y,
                             screen.WorkingArea.Width, screen.WorkingArea.Height, true);
        }

        /// <summary>
        /// Moves the window to a specified monitor
        /// </summary>
        /// <param name="monitorIndex">Monitor index: 0=primary, 1+=specific monitor (1-based index), -2=secondary monitor</param>
        /// <param name="preserveState">If true, preserves maximized/minimized state after moving</param>
        /// <returns>True if successful</returns>
        public bool MoveToMonitor(int monitorIndex, bool preserveState = true)
        {
            // Save current window state
            bool wasMaximized = IsMaximized();
            bool wasMinimized = IsMinimized();

            // Need to restore window before moving if it's maximized/minimized
            if (wasMaximized || wasMinimized)
            {
                Restore();
                System.Threading.Thread.Sleep(100); // Give OS time to complete the operation
            }

            // Get current size
            var size = Size();

            // Get all available screens
            var allScreens = WpfScreenHelper.Screen.AllScreens.ToList();
            WpfScreenHelper.Screen targetScreen = null;

            // Determine target monitor
            if (monitorIndex == 0)
            {
                // Primary monitor
                targetScreen = WpfScreenHelper.Screen.PrimaryScreen;
            }
            else if (monitorIndex == -2)
            {
                // Try to get from global variable in PowerShell
                // Default to secondary monitor if available, or current monitor
                try
                {
                    // We can't access PowerShell variables directly, so we'll rely on the caller
                    // to translate the -2 value appropriately
                    if (allScreens.Count > 1)
                    {
                        // Use second monitor as default secondary
                        targetScreen = allScreens[1];
                    }
                    else
                    {
                        // Fall back to primary if only one monitor
                        targetScreen = WpfScreenHelper.Screen.PrimaryScreen;
                    }
                }
                catch
                {
                    // Default to primary if anything goes wrong
                    targetScreen = WpfScreenHelper.Screen.PrimaryScreen;
                }
            }
            else if (monitorIndex > 0 && monitorIndex <= allScreens.Count)
            {
                // Specific monitor (1-based index)
                targetScreen = allScreens[monitorIndex - 1];
            }
            else
            {
                // Invalid monitor index, use current
                targetScreen = WpfScreenHelper.Screen.FromHandle(Handle);
            }

            // Center window on target monitor's working area
            int newX = (int)targetScreen.WorkingArea.X + (int)((targetScreen.WorkingArea.Width - size.Width) / 2.0);
            int newY = (int)targetScreen.WorkingArea.Y + (int)((targetScreen.WorkingArea.Height - size.Height) / 2.0);

            // Move window to new position
            bool result = Move(newX, newY);

            // Restore previous state if requested
            if (preserveState)
            {
                if (wasMaximized)
                {
                    Maximize();
                }
                else if (wasMinimized)
                {
                    Minimize();
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the index of the monitor that the window is currently on
        /// </summary>
        /// <returns>
        /// Monitor index where the window is located:
        /// 0 = primary monitor
        /// 1+ = specific monitor (1-based index)
        /// -1 = unable to determine
        /// </returns>
        public int GetCurrentMonitor()
        {
            try
            {
                // Get window position (center point of the window)
                var windowRect = new RectStruct();
                GetWindowRect(Handle, ref windowRect);
                int centerX = windowRect.Left + ((windowRect.Right - windowRect.Left) / 2);
                int centerY = windowRect.Top + ((windowRect.Bottom - windowRect.Top) / 2);

                // Get all screens
                var allScreens = WpfScreenHelper.Screen.AllScreens.ToList();

                // Find which screen contains this point
                for (int i = 0; i < allScreens.Count; i++)
                {
                    var screen = allScreens[i];
                    if (centerX >= screen.Bounds.Left && centerX <= screen.Bounds.Right &&
                        centerY >= screen.Bounds.Top && centerY <= screen.Bounds.Bottom)
                    {
                        return i;
                    }
                }

                // If we reach here, try a different approach using FromHandle
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        // Add helper method to detect Electron apps like VS Code
        private bool IsElectronApp()
        {
            // Check window class name first
            if (WindowClassName.Contains("Chrome_WidgetWin") ||
                WindowClassName.Contains("Electron") ||
                Title.Contains("Visual Studio Code"))
            {
                return true;
            }

            // Try to get process name as additional check
            try
            {
                uint processId = 0;
                GetWindowThreadProcessId(Handle, out processId);
                if (processId != 0)
                {
                    var process = System.Diagnostics.Process.GetProcessById((int)processId);
                    string procName = process?.ProcessName?.ToLowerInvariant() ?? "";
                    return procName.Contains("code") || procName.Contains("electron");
                }
            }
            catch
            {
                // Ignore errors in process identification
            }

            return false;
        }
        [DllImport("user32.dll")]
        public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        /// <summary>
        /// Gets the parent process of the specified process
        /// </summary>
        private static Process GetParentProcess(Process process)
        {
            try
            {
                var parentId = GetParentProcessId(process.Id);
                return parentId > 0 ? Process.GetProcessById(parentId) : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the parent process ID using WMI
        /// </summary>
        private static int GetParentProcessId(int processId)
        {
            try
            {
                using (var searcher = new System.Management.ManagementObjectSearcher(
                    $"SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {processId}"))
                {
                    using (var results = searcher.Get())
                    {
                        var result = results.Cast<System.Management.ManagementObject>().FirstOrDefault();
                        return result != null ? Convert.ToInt32(result["ParentProcessId"]) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Determines if a process name suggests it's likely a terminal or host application
        /// </summary>
        private static bool IsLikelyHostProcess(string processName)
        {
            if (string.IsNullOrEmpty(processName))
                return false;

            var name = processName.ToLowerInvariant();

            // Common terminal/host process names
            return name.Contains("terminal") ||
                   name.Contains("console") ||
                   name.Contains("cmd") ||
                   name.Contains("conhost") ||
                   name.Contains("wsl") ||
                   name.Contains("bash") ||
                   name.Contains("ssh") ||
                   name.Contains("cursor") ||
                   name.Contains("putty") ||
                   name.Contains("code") ||          // VS Code
                   name.Contains("code - insiders") ||          // VS Code
                   name.Contains("idea") ||          // IntelliJ
                   name.Contains("eclipse") ||       // Eclipse
                   name.Contains("cmder") ||         // Cmder
                   name.Contains("conemu") ||        // ConEmu
                   name.Contains("hyper") ||         // Hyper terminal
                   name.Contains("alacritty") ||     // Alacritty
                   name.Contains("kitty") ||         // Kitty terminal
                   name.Contains("iterm") ||         // iTerm (if running on Windows via compatibility)
                   name.Contains("mintty");          // MinTTY (Git Bash, etc.)
        }
    }
}

