using System;
using System.Reflection;
using System.Threading;

namespace Flux.Helpers
{
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    internal class KbManager
    {
        public void HotkeyLoop()
        {
            const int vkXbutton1 = 0x05;
            const int vkXbutton2 = 0x06;
            const int vkCtrl = 0x11;
            const int vkR = 0x52;
            const int vkQ = 0x51;
            const int vkNumpad7 = 0x67;
            const int vkNumpad9 = 0x69;
            const int vkNumpad4 = 0x64;
            const int vkNumpad6 = 0x66;

            const int sleepDelay = 450;
            var rnd = new Random();
            while (true)
            {
                //- Delay
                if ((Win32.GetAsyncKeyState(vkQ) & 0x8000) > 0 && (Win32.GetAsyncKeyState(vkCtrl) & 0x8000) > 0)
                {
                    H.DelayInMs = rnd.Next(10,20) ;
                    Console.WriteLine($@"Single Shot: {H.DelayInMs}");
                    Thread.Sleep(sleepDelay);
                }

                //+ Delay
                if ((Win32.GetAsyncKeyState(vkR) & 0x8000) > 0 && (Win32.GetAsyncKeyState(vkCtrl) & 0x8000) > 0)
                {
                    H.DelayInMs = rnd.Next(520, 650);
                    Console.WriteLine($@"Automatic {H.DelayInMs}");
                    Thread.Sleep(sleepDelay);
                }

                //Trigger
                if ((Win32.GetAsyncKeyState(vkXbutton1) & 0x8000) > 0)
                {
                    H.TriggerEnabled = true;
                    Thread.Sleep(3);
                }
                else
                {
                    H.TriggerEnabled = false;
                    H.AimbotEnabled = false;
                }

                //aim speed
                //+ Delay
                if ((Win32.GetAsyncKeyState(vkNumpad7) & 0x8000) > 0)
                {
                    H.AimSpeed -= .1f;
                    Console.WriteLine($@"Current Aim : {H.AimSpeed}");
                    Thread.Sleep(sleepDelay);
                }

                //- Delay
                if ((Win32.GetAsyncKeyState(vkNumpad9) & 0x8000) > 0)
                {
                    H.AimSpeed += .1f;
                    Console.WriteLine($@"Current Aim : {H.AimSpeed}");
                    Thread.Sleep(sleepDelay);
                }

                //aim fov
                //- FOV
                if ((Win32.GetAsyncKeyState(vkNumpad4) & 0x8000) > 0)
                {
                    H.MaxFOV -= 5;
                    Console.WriteLine($@"Current FOV : {H.MaxFOV}");
                    Thread.Sleep(sleepDelay);
                }

                //+ FOV
                if ((Win32.GetAsyncKeyState(vkNumpad6) & 0x8000) > 0)
                {
                    H.MaxFOV += 5;
                    Console.WriteLine($@"Current FOV : {H.MaxFOV}");
                    Thread.Sleep(sleepDelay);
                }
                Thread.Sleep(5);
            }
        }
    }
}