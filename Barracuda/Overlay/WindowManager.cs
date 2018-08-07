using System;
using System.Reflection;
using System.Threading;
using Flux.Helpers;

namespace Flux.Overlay
{
    internal class WindowManager
    {
        [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
        private readonly IntPtr _handle;
        public WindowManager(IntPtr window)
        {
            _handle = window;
        }
        public void WindowLoop()
        {
            Console.WriteLine(@"Enjoy!");
            while (true)
            {
                var marg = new Win32.Margins
                {
                    Left = 0,
                    Top = 0,
                    Right = 2560,
                    Bottom = 1440
                };
                Win32.DwmExtendFrameIntoClientArea(_handle, ref marg);
                try
                {
                    if (_handle != IntPtr.Zero)
                    {
                        Win32.SetWindowPos(_handle, (IntPtr)Win32.SpecialWindowHandles.HWND_TOPMOST, 0, 0, 0, 0,
                            Win32.SetWindowPosFlags.SWP_NOMOVE | Win32.SetWindowPosFlags.SWP_NOSIZE);
                    }
                    else
                    {
                        break;
                    }
                }
                catch
                {
                    Thread.Sleep(15);
                    continue;
                }
                Thread.Sleep(1000 / 144);
            }
        }
    }
}
