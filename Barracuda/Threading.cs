using System;
using System.Reflection;
using System.Threading;
using Flux.Cheats;
using Flux.Game;
using Flux.Helpers;
using Flux.Overlay;

namespace Flux
{
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    internal static class Threading
    {
        public static void Run()
        {
            Console.WriteLine(@"Starting threads...");

            //init classes
            var offset = new Offsets();
            var address = new AddressManager(offset);
            var entityManager = new EntityFunctions(offset, address);
            var trigger = new TriggerManager(address, offset);
            var kb = new KbManager();
            var recoil = new RecoilManager(address, offset);
            var aim = new AimManager(address, offset);
            //start threads
            new Thread(address.AddressLoop).Start();
            new Thread(entityManager.BuildEntityList).Start();
            new Thread(trigger.TriggerLoop).Start();
            new Thread(kb.HotkeyLoop).Start();
            new Thread(recoil.RecoilLoop).Start();
            new Thread(aim.AimLoop).Start();

            H.Aimbot = aim;
            //start overlay thread
            var overlay = new flux();
            overlay.Show();
            var wm = new WindowManager(overlay.Handle);
            new Thread(wm.WindowLoop).Start();
        }
    }
 }
