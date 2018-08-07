using System.Reflection;

namespace Flux.Game
{
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public class Offsets
    {
        public readonly int Camera = 0x40;
        public readonly int ChildComponent = 0x8;
        public readonly int Engine = 0x230;



        public readonly int Spread = 0x0070;
        public readonly int Recoil = 0x00A8;


        public readonly int EngineLink = 0x120;
        public readonly int Entity = 0x0008;

        public readonly int EntityInfo = 0x18;
        public readonly int EntityList = 0xB8;
        public readonly int EntityRef = 0x20;

        public readonly int FeetPosition = 0x1D0;
        public readonly int Fovx = 0xF0;
        public readonly int Fovy = 0x104;
        public readonly int HeadPosition = 0x180;
        public readonly int Health = 0x148;

        public readonly int HNeck = 0x2E0;
        public readonly int InCross = 0x1C8;
        public readonly int IncrossBase = 0x398;

        public readonly int InterfaceBase = 0x400;

        public readonly int LAnkle = 0xC0;

        public readonly int LElbow = 0x320;

        public readonly int LFoot = 0xE0;


        public readonly int LHand = 0x340;

        public readonly int LHip = 0x120;

        public readonly int LKnee = 0x140;
        public readonly int LNeck = 0x170;

        public readonly int LShoulder = 0x300;
        public readonly int MainComponent = 0xA8;

        public readonly int PlayerInfo = 0x2B8;
        public readonly int PlayerName = 0x178;
        public readonly int PlayerTeamId = 0x146;
        public readonly int RAnkle = 0x1A0;
        public readonly int RElbow = 0xD00;
        public readonly int RFoot = 0x1C0;
        public readonly int RHand = 0xD20;
        public readonly int RHip = 0x200;
        public readonly int RKnee = 0x220;
        public readonly int RShoulder = 0xCE0;
        public readonly int ViewForward = 0xD0;
        public readonly int ViewRight = 0xB0;

        public readonly int ViewTranslastion = 0xE0;
        public readonly int ViewUp = 0xC0;
    }
}