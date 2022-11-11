﻿using NativeApi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace PFSnip
{
    internal static class CaptureEngine
    {
        #region Private Fields

        private static int zOrder = 0;

        #endregion

        #region Public Methods

        public static (Rectangle, int)[] CollectWindows(out Bitmap bitmap, out Rectangle totalScreenSize)
        {
            var result = new List<(Rectangle, int)>();

            var windows = new List<(IntPtr, int)>();
            GCHandle allocated = GCHandle.Alloc(windows);
            zOrder = -1;

            try
            {
                var wndEnumProc = new WNDENUMPROC(WndEnumProc);
                User32.EnumWindows(wndEnumProc, GCHandle.ToIntPtr(allocated));
            }
            finally
            {
                if (allocated.IsAllocated)
                {
                    allocated.Free();
                }
            }

            foreach ((IntPtr, int) window in windows)
            {
                //if (User32.GetWindowRect(handle, out RECT rect))
                if (DwmApi.DwmGetWindowAttribute(window.Item1, DWMWINDOWATTRIBUTE.DWMWA_EXTENDED_FRAME_BOUNDS, out RECT rect, Marshal.SizeOf(typeof(RECT))) == 0)
                {
                    int top = rect.Top;
                    int left = rect.Left;
                    result.Add((new Rectangle(left, top, Math.Abs(rect.Right - left), Math.Abs(rect.Bottom - top)), window.Item2));
                }
            }

            totalScreenSize = GetTotalScreenSize();
            bitmap = CaptureScreen(totalScreenSize);
            return result.ToArray();
        }

        #endregion

        #region Private Methods

        private static Bitmap CaptureScreen(Rectangle rectangle)
        {
            Bitmap result = new Bitmap(rectangle.Width, rectangle.Height);

            using (Graphics graphics = Graphics.FromImage(result))
            {
                IntPtr hdc = graphics.GetHdc();

                try
                {
                    IntPtr dc = User32.GetDC(IntPtr.Zero);

                    try
                    {
                        int left = rectangle.Left;
                        int top = rectangle.Top;
                        Gdi32.BitBlt(hdc, 0, 0, result.Width, result.Height, dc, left, top, WinGdi.SRCCOPY);
                    }
                    finally
                    {
                        User32.ReleaseDC(IntPtr.Zero, dc);
                    }
                }
                finally
                {
                    graphics.ReleaseHdc(hdc);
                }
            }

            return result;
        }

        private static Rectangle GetTotalScreenSize()
        {
            var result = new Rectangle();

            foreach (Screen screen in Screen.AllScreens)
            {
                Rectangle bounds = screen.Bounds;
                Rectangle prevResult = result;

                if (bounds.X < result.X)
                {
                    result.X = bounds.X;
                    result.Width += (prevResult.Right - result.Right);
                }

                if (bounds.Y < result.Y)
                {
                    result.Y = bounds.Y;
                    result.Height += (prevResult.Bottom - result.Bottom);
                }

                if (bounds.Right > result.Right)
                {
                    result.Width += (bounds.Right - result.Right);
                }

                if (bounds.Bottom > result.Bottom)
                {
                    result.Height += (bounds.Bottom - result.Bottom);
                }
            }

            return result;
        }

        private static bool WndEnumProc(IntPtr hWnd, IntPtr lParam)
        {
            ++zOrder;
            GCHandle gch = GCHandle.FromIntPtr(lParam);
            List<(IntPtr, int)> windows;

            try
            {
                windows = (List<(IntPtr, int)>)gch.Target;
            }
            catch
            {
                throw;
            }

            if (!User32.IsWindowVisible(hWnd))
            {
                return true;
            }

            var builder = new StringBuilder(256);

            if (User32.GetWindowText(hWnd, builder, builder.Capacity) < 1)
            {
                return true;
            }

            windows.Add((hWnd, zOrder));
            return true;
        }

        #endregion
    }
}
