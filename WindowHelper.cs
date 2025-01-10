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

            return ScreenScalingFactor; // 1.25 = 125%
        }
    }

    public class WindowObj
    {

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
        }

        public void SetForeground()
        {
            ShowWindowAsync(this.Handle, (int)ShowWindowCommands.Show);
            SetForegroundWindow(this.Handle);
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

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetFocus(IntPtr hWnd);

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

        [DllImport("user32.dll", SetLastError = true)]
        public static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, UInt32 dwNewLong);
        public void RemoveBorder()
        {
            const int GWL_STYLE = -16;
            const long WS_CAPTION = 0x00C00000L;
            const long WS_THICKFRAME = 0x00040000L;
            const long WS_MINIMIZEBOX = 0x00020000L;
            const long WS_MAXIMIZEBOX = 0x00010000L;
            const long WS_SYSMENU = 0x00080000L;

            long lStyle = GetWindowLong(this.Handle, GWL_STYLE);
            lStyle &= ~(WS_CAPTION | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX | WS_SYSMENU);
            SetWindowLong(this.Handle, GWL_STYLE, (uint)lStyle);
        }
    }
}

