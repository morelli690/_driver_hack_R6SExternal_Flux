using System;
using System.Threading;
using Flux.Game;
using Flux.Helpers;

namespace Flux.Cheats
{
    internal class TriggerManager
    {
        private readonly AddressManager _address;
        private readonly Offsets _offsets;
        const int EventfLeftdown = 0x0002;
        const int EventfLeftup = 0x0004;

        public TriggerManager(AddressManager address, Offsets offsets)
        {
            _offsets = offsets;
            _address = address;
        }

        private static void ShootMouse()
        {
            Win32.mouse_event(EventfLeftdown, 0, 0, default(uint), default(int));
            var rnd = new Random();
            Thread.Sleep(rnd.Next(H.DelayInMs, H.DelayInMs + 15));
            Win32.mouse_event(EventfLeftup, 0, 0, default(uint), default(int));
        }

        public void TriggerLoop()
        {
            while (true)
            {
                var pInterface = M.Read<IntPtr>(_address.PInterface + _offsets.InterfaceBase);
                var pIncrossBase = M.Read<IntPtr>(pInterface + _offsets.IncrossBase);

                if (H.TriggerEnabled && M.Read<int>(pIncrossBase + _offsets.InCross) == 1)
                {
                    H.AimbotEnabled = true;
                    ShootMouse();
                }
                Thread.Sleep(5);
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
