using System;
using System.Runtime.InteropServices;

namespace WindowsLib.DLLImports
{
    public static class User32
    {
        public const int SWP_NOACTIVATE = 0x10;
        public const int SWP_NOZORDER = 0x4;
        public const int SWP_SHOWWINDOW = 0x40;
        public const int GWL_STYLE = -16;
        public const int WS_CHILD = 0x40000000;
        public const int GWL_HWNDPARENT = -8;
        public const int HWND_TOP = 0;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        } //RECT

        [DllImport("User32.dll")]
        public static extern int DestroyIcon(IntPtr hIcon);

        [DllImport("User32.dll")]
        public static extern int GetClientRect(IntPtr hwnd, ref RECT lpRect);
        [DllImport("User32.dll")]
        public static extern int GetWindowLongA(IntPtr hwnd, IntPtr nIndex);
        [DllImport("User32.dll")]
        public static extern int SetWindowLongA(IntPtr hwnd, IntPtr nIndex, IntPtr dwNewInteger);
        [DllImport("User32.dll")]
        public static extern int SetWindowPos(IntPtr hwnd, IntPtr hWndInsertAfter, IntPtr x, IntPtr y, IntPtr cx, IntPtr cy, IntPtr wFlags);
        [DllImport("User32.dll")]
        public static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
    } //User32 Class
} //DLLImports Namespace
