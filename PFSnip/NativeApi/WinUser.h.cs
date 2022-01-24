using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NativeApi
{
    public static partial class User32
    {
        #region Public Methods

        [DllImport(AssemblyName)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, IntPtr lParam);

        [DllImport(AssemblyName)]
        public static extern IntPtr GetDC(IntPtr hWnd);

//        [DllImport(AssemblyName, SetLastError = true)]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport(AssemblyName, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport(AssemblyName)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport(AssemblyName)]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        #endregion
    }

    public delegate bool WNDENUMPROC(IntPtr hWnd, IntPtr lParam);
}
