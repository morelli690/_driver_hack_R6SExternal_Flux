using System;
using System.Collections.Generic;
using Flux.Cheats;
using Flux.Game;

namespace Flux.Helpers
{
    public static class H
    {
        public static MemoryManager DriveRead { get; set; }

        //Global Offsets
        public static IntPtr GameManager { get; set; }
        public static IntPtr Renderer { get; set; }
        public static IntPtr Interface { get; set; }
        public static IntPtr WeaponManager { get; set; }

        //Global entity info
        public static List<EntityFunctions.PlayerInfo> Players { get; set; }
        public static EntityFunctions.PlayerInfo LocalPlayer { get; set; }
        public static Vector3 ViewTranslation { get; set; }

        //Gl
        public static bool TriggerEnabled { get; set; }
        public static int DelayInMs { get; set; }
        public static bool AimbotEnabled { get; set; }

        public static AimManager Aimbot { get; set; }
        public static String CurrentTargetName { get; set; }
        public static int MaxFOV { get; set; }
        public static float AimSpeed { get; set; }
        public static bool InCross { get; set; }

        public static bool switchOffset { get; set; }
    }
}