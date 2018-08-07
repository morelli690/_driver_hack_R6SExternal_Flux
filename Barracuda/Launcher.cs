using System;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;
using Flux.Helpers;

namespace Flux
{
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    internal static class MainCon
    {
        [STAThread]
        private static void Main()
        {
            //verify service is started
            var sc = new ServiceController("Serpentine");
            Console.WriteLine(sc.Status == ServiceControllerStatus.Running ? "Loaded : True" : "Loaded : False");
            sc.Close();


            //verify process is started and get pid
            Process proc = null;
            while (proc == null)
                try
                {
                    proc = Process.GetProcessesByName("RainbowSix")[0];
                    if (proc.Id == 0)
                        proc = null;
                }
                catch
                {
                    // ignored
                }

            //allow user to specify when to start
            Console.WriteLine(@"Press any key");
            Console.ReadKey();

            //initialization
            H.DriveRead = new MemoryManager(proc.Id);
            H.GameManager = (IntPtr)0x4ecc5e0;
            H.Renderer = (IntPtr)0x4db2130;
            H.Interface = (IntPtr)0x4ecaed0;
            H.WeaponManager = (IntPtr)0x36D7F98;
            H.DelayInMs = 550;
            H.MaxFOV = 80;
            H.AimSpeed = 3.5f;
            H.switchOffset = false;
            H.TriggerEnabled = false;

            //start threads
            Threading.Run();
        }
    }
}