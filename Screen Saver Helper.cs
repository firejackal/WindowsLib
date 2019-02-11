// Requires:
//   * User32.cs

using System;
using System.Diagnostics;
using System.Windows.Forms;
using WindowsLib.DLLImports;

namespace WindowsLib
{
    public static class ScreenSaverHelper
    {
        public enum StartupModes { Failure, Configure, Preview, Start }

        // Second revision of function, before it was returning whatever was at position 0 of the arguments,
        // this doesn't work anymore cause Visual Studio puts the application's full exe path there.
        // So now we are doing a search for what we want.
        public static StartupModes ParseStartupMode(string[] args, out int outTargetWindowHandle)
        {
            outTargetWindowHandle = 0;

            if (args.Length == 0)
                return StartupModes.Configure;
            else
            {
                // search for our argument
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].ToLower().Equals("/c"))
                    {
                        return StartupModes.Configure;
                    } else if (args[i].ToLower().Equals("/s"))
                    {
                        return StartupModes.Start;
                    } else if (args[i].ToLower().Equals("/p"))
                    {
                        if (args.Length > (i + 1))
                        {
                            outTargetWindowHandle = Int32.Parse(args[i + 1]);
                            return StartupModes.Preview;
                        } else
                        {
                            break;
                        }
                    }
                }
            }

            return StartupModes.Failure;
        } //ParseStartupMode Function

        public static bool HasPreviousInstance()
        {
            return (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).GetUpperBound(0) > 0);
        } //HasPreviousInstance Function

        public static void SetPreviewWindow(Form f, string targetHandle)
        {
            SetPreviewWindow(f, (IntPtr)Int32.Parse(targetHandle));
        } //SetPreviewWindow Function

        public static void SetPreviewWindow(Form f, IntPtr targetHandle)
        {
            //get dimensions of preview
            User32.RECT r = new User32.RECT();
            User32.GetClientRect(targetHandle, ref r);

            f.WindowState = FormWindowState.Normal;
            f.FormBorderStyle = FormBorderStyle.None;
            f.Width = r.right;
            f.Height = r.bottom;

            // get and set new window style
            int style = User32.GetWindowLongA(f.Handle, (IntPtr)User32.GWL_STYLE);
            style = style | User32.WS_CHILD;
            User32.SetWindowLongA(f.Handle, (IntPtr)User32.GWL_STYLE, (IntPtr)style);

            // set parent window (preview window)
            User32.SetParent(f.Handle, targetHandle);

            // save preview in forms window structure
            User32.SetWindowLongA(f.Handle, (IntPtr)User32.GWL_HWNDPARENT, targetHandle);
            User32.SetWindowPos(f.Handle, (IntPtr)0, (IntPtr)r.left, (IntPtr)0, (IntPtr)r.right, (IntPtr)r.bottom, (IntPtr)(User32.SWP_NOACTIVATE | User32.SWP_NOZORDER | User32.SWP_SHOWWINDOW));
        } //SetPreviewWindow Function
    } //ScreenSaverHelper Class
}
