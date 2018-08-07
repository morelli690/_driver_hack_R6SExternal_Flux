using System;
using System.Reflection;
using System.Threading;
using Flux.Game;
using Flux.Helpers;

namespace Flux.Cheats
{
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    internal class RecoilManager
    {
        private readonly AddressManager _address;
        private readonly Offsets _offsets;
        private int _failCounter;

        public RecoilManager(AddressManager address, Offsets offsets)
        {
            _offsets = offsets;
            _address = address;
        }

        public void RecoilLoop()
        {
            while (true)
            {
                var recoil = M.Read<float>(_address.WeaponComponent + _offsets.Recoil);
                if (recoil <= .200f)
                {
                    continue;
                }
                M.Write(0.0f, _address.WeaponComponent + _offsets.Spread);
                M.Write(0.012f, _address.WeaponComponent + _offsets.Recoil);
                recoil = M.Read<float>(_address.WeaponComponent + _offsets.Recoil);
                if (recoil > .05f && _failCounter >= 10)
                {
                    H.switchOffset = true;
                    _failCounter = 0;
                } else if (recoil > .05f && _failCounter < 10)
                {
                    _failCounter++;
                }
                else
                {
                    _failCounter = 0;
                    H.switchOffset = false;
                    continue;
                }
                Thread.Sleep(2000);
            }
        }
    }
}
