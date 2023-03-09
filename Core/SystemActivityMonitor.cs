using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NovinDevHubStaffCore.Core
{
    class SystemActivityMonitor
    {

        private const int MinKeyboardIntervalSeconds = 3;
        private DateTime? lastKeyboardTime;
        private DateTime startKeyboardTime;

        private const int MinMouseIntervalSeconds = 1;
        private DateTime? lastMouseTime;
        private DateTime startMouseTime;


        public event OnProccessChangedHandler OnProccessChanged;
        public delegate void OnProccessChangedHandler(string windowTitle, IntPtr windowHandle, Process process);

        public event OnKeyboardMouseActivityHandler OnKeyboardMouseActivity;
        public delegate void OnKeyboardMouseActivityHandler(short type, DateTime startTime, DateTime endTime);


        IntPtr _processhook;
        IntPtr _keyboardhook;
        IntPtr _mousehook;
        public User2Api.WinEventDelegate dele = null;



        private User2Api.LowLevelMouseKeyboardProc _keyboardmouse_proc;

        public SystemActivityMonitor()
        {

        }

        public void Start()
        {
            dele = new User2Api.WinEventDelegate(WinEventProc);
            _processhook = User2Api.SetWinEventHook(User2Api.EVENT_SYSTEM_FOREGROUND, User2Api.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, User2Api.WINEVENT_OUTOFCONTEXT);

            _keyboardmouse_proc = KeyboardMouseHookCallback;
            _keyboardhook = SetKeyboardMouseHook(_keyboardmouse_proc, User2Api.WH_KEYBOARD_LL);
            _mousehook = SetKeyboardMouseHook(_keyboardmouse_proc, User2Api.WH_MOUSE_LL);
        }

        private IntPtr SetKeyboardMouseHook(User2Api.LowLevelMouseKeyboardProc proc, int type)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return User2Api.SetWindowsHookEx(type, proc,
                    User2Api.GetModuleHandle(curModule.ModuleName), 0);
            }
        }



        public void Stop()
        {
            User2Api.UnhookWindowsHookEx(_processhook);
            User2Api.UnhookWindowsHookEx(_keyboardhook);
            User2Api.UnhookWindowsHookEx(_mousehook);
        }

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            string windowTitle = "";

            const int nChars = 1024;
            IntPtr handle = IntPtr.Zero;
            StringBuilder buffer;
            handle = User2Api.GetForegroundWindow();


            buffer = new StringBuilder(nChars);
            if (User2Api.GetWindowText(handle, buffer, nChars) > 0)
            {
                windowTitle = buffer.ToString();
            }

            uint pid;
            User2Api.GetWindowThreadProcessId(handle, out pid);
            Process p = Process.GetProcessById((int)pid);

            if (OnProccessChanged != null && !string.IsNullOrEmpty(windowTitle))
            {
                OnProccessChanged(windowTitle, handle, p);
            }

        }

        private IntPtr KeyboardMouseHookCallback(
           int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (wParam == (IntPtr)User2Api.WM_KEYDOWN)
                {
                    //Debug.WriteLine("Keyboard detected");
                    HandleKeyboard(Marshal.ReadInt32(lParam));
                    return User2Api.CallNextHookEx(_keyboardhook, nCode, wParam, lParam);
                }
                else if (wParam == (IntPtr)User2Api.MouseMessages.WM_LBUTTONDOWN
                         || wParam == (IntPtr)User2Api.MouseMessages.WM_LBUTTONUP
                         || wParam == (IntPtr)User2Api.MouseMessages.WM_RBUTTONDOWN
                         || wParam == (IntPtr)User2Api.MouseMessages.WM_RBUTTONUP
                         || wParam == (IntPtr)User2Api.MouseMessages.WM_MOUSEWHEEL
                         || wParam == (IntPtr)User2Api.MouseMessages.WM_MOUSEMOVE)
                {
                    //Debug.WriteLine("Mouse detected");
                    HandleMouse();
                    return User2Api.CallNextHookEx(_mousehook, nCode, wParam, lParam);
                }

            }

            return User2Api.CallNextHookEx(_keyboardhook, nCode, wParam, lParam);

        }

        private void HandleKeyboard(int vkCode)
        {
            CheckMouseIdle();
            //Debug.WriteLine((Keys)vkCode);

            var now = DateTime.Now;
            if (lastKeyboardTime == null)
            {
                startKeyboardTime = now;
                lastKeyboardTime = now;
                Debug.WriteLine("Start Keyboard Activity");
            }
            else
            {
                //if two keyboard keydown is less than {MinKeyboardIntervalSeconds} consider it continuous keyboard usage
                //otherwise create new keyboard activity
                if ((now - lastKeyboardTime.Value).TotalSeconds > MinKeyboardIntervalSeconds)
                {
                    //Debug.WriteLine("End Keyboard Activity");
                    if (OnKeyboardMouseActivity != null)
                    {
                        OnKeyboardMouseActivity(1, startKeyboardTime, now);
                    }
                    lastKeyboardTime = null;
                }
                else
                {
                    
                    if (!ChecKeyboardIdle())
                        lastKeyboardTime = now;
                    //Debug.WriteLine("Continue Keyboard Activity");
                }

            }
        }

        private void HandleMouse()
        {
            //trigger to check keyboard is idle?
            ChecKeyboardIdle();

            var now = DateTime.Now;
            if (lastMouseTime == null)
            {
                startMouseTime = now;
                lastMouseTime = now;
                //Debug.WriteLine("Start Mouse Activity");
            }
            else
            {
                //if two Mouse keydown is less than {MinMouseIntervalSeconds} consider it continuous Mouse usage
                //otherwise create new Mouse activity
                
                if(!CheckMouseIdle())
                {
                    lastMouseTime = now;
                    //Debug.WriteLine("Continue Mouse Activity");
                }

            }
        }

        private bool ChecKeyboardIdle()
        {
            //call from mouse activity
            if (lastKeyboardTime == null)
                return true;

            var now = DateTime.Now;

            if ((now - lastKeyboardTime.Value).TotalSeconds > MinKeyboardIntervalSeconds)
            {
                //Debug.WriteLine("End Keyboard Activity");
                if (OnKeyboardMouseActivity != null)
                {
                    OnKeyboardMouseActivity(1, startKeyboardTime, now);
                }
                lastKeyboardTime = null;
                return true;
            }
            return false;
        }

        private bool CheckMouseIdle()
        {
            //check from keyboard activity
            if (lastMouseTime == null)
                return true;

            var now = DateTime.Now;

            if ((now - lastMouseTime.Value).TotalSeconds > MinMouseIntervalSeconds)
            {
                //Debug.WriteLine("End Mouse Activity");
                if (OnKeyboardMouseActivity != null)
                {
                    OnKeyboardMouseActivity(2, startMouseTime, now);
                }
                lastMouseTime = null;
                return true;
            }
            return false;
        }
    }
}
