using System;
using System.Threading;
using Flux.Helpers;

namespace Flux.Game
{
    public class AddressManager
    {
        private readonly Offsets _offset;
        private IntPtr _pEngine;
        private IntPtr _pEngineLink;

        private IntPtr _pGameManager;
        private IntPtr _pGameRender;
        private IntPtr _pRender;

        public IntPtr PCamera;
        public IntPtr PEntityList;
        public IntPtr PInterface;
        public IntPtr WeaponComponent;

        public AddressManager(Offsets offset)
        {
            _offset = offset;
        }

        public void AddressLoop()
        {
            while (true)
            {
                _pGameManager = M.Read<IntPtr>(H.GameManager);
                PEntityList = M.Read<IntPtr>(_pGameManager + _offset.EntityList);
                _pRender = M.Read<IntPtr>(H.Renderer);
                _pGameRender = M.Read<IntPtr>(_pRender);
                _pEngineLink = M.Read<IntPtr>(_pGameRender + _offset.EngineLink);
                _pEngine = M.Read<IntPtr>(_pEngineLink + _offset.Engine);
                PCamera = M.Read<IntPtr>(_pEngine + _offset.Camera);
                PInterface = M.Read<IntPtr>(H.Interface);
                if (H.switchOffset)
                {
                    var weaponManager = M.Read<IntPtr>(H.WeaponManager);
                    var weaponunk0 = M.Read<IntPtr>(weaponManager + 0x0020);
                    var weaponunk1 = M.Read<IntPtr>(weaponunk0 + 0x00C8);
                    var weaponunk2 = M.Read<IntPtr>(weaponunk1 + 0x0228);
                    WeaponComponent = M.Read<IntPtr>(weaponunk2 + 0x0110);
                }
                else
                {
                    var weaponManager = M.Read<IntPtr>(H.WeaponManager);
                    var weaponunk0 = M.Read<IntPtr>(weaponManager + 0x0020);
                    var weaponunk1 = M.Read<IntPtr>(weaponunk0 + 0x00E0);
                    var weaponunk2 = M.Read<IntPtr>(weaponunk1 + 0x0120);
                    WeaponComponent = M.Read<IntPtr>(weaponunk2 + 0x0110);
                }


                Thread.Sleep(5);
            }
        }
    }
}