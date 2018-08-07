using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Flux.Game;
using Flux.Helpers;

namespace Flux.Cheats
{
    public class AimManager
    {
        private readonly AddressManager _address;
        private readonly Offsets _offsets;

        public readonly int CrosshairX = (2560 / 2);
        public readonly int CrosshairY = (1440 / 2);
        private bool _wasButtonPressed = false;

        private EntityFunctions.PlayerInfo _tempTarget;
        public EntityFunctions.PlayerInfo TempClosestTarget;
        private float _tempDist;

        public AimManager(AddressManager address, Offsets offsets)
        {
            _offsets = offsets;
            _address = address;
        }

        private float DistanceFromCrosshair(EntityFunctions.PlayerInfo entity)
        {
            var x = (entity.W2SHead.X - CrosshairX);
            var y = (entity.W2SHead.Y - CrosshairY);

            return (float)Math.Sqrt(x * x + y * y);
        }

        public void GetClosestTargetFromCrosshair(EntityFunctions.PlayerInfo entity)
        {
            var distanceFromCrosshair = DistanceFromCrosshair(entity);

            if (distanceFromCrosshair <= _tempDist)
            {
                _tempDist = distanceFromCrosshair;
                TempClosestTarget = entity;
            }
        }

        public void LockonPlayer(EntityFunctions.PlayerInfo entity)
        {
            if (entity.Name is "null")
            {
                return;
            }
            var updated = H.Players.Find(p => p.Name == entity.Name);
            entity = updated;
            if (H.AimbotEnabled && !_wasButtonPressed)
            {
                _wasButtonPressed = true;
                _tempTarget = entity;
            }

            if (H.AimbotEnabled && _wasButtonPressed && entity.Health == 0)
            {
                _wasButtonPressed = false;
            }
            H.CurrentTargetName = entity.Name;
        }

        private void AimPlayer(EntityFunctions.PlayerInfo entity)
        {
            if (entity.Name is "null")
            {
                return;

            }
            var updated = H.Players.Find(p => p.Name == entity.Name);
            entity = updated;
            var distXHair = DistanceFromCrosshair(entity);

            var boneX = entity.W2SHead.X - (2560 / 2);
            var boneY = entity.W2SHead.Y - (1440 / 2);

            if (H.AimbotEnabled && entity.Health != 0 && distXHair <= H.MaxFOV && _tempTarget.W2SHead.Y != 0)
            {
                Win32.mouse_event(0x0001, (int)(boneX * (H.AimSpeed * 0.1)), (int)(boneY * (H.AimSpeed * 0.1)), 0, 0);
            }
        }

        public void AimLoop()
        {
            while (true)
            {
                if (!H.AimbotEnabled)
                {
                    _wasButtonPressed = false;
                    _tempTarget.Name = "null";
                    _tempTarget.W2SHead = new Vector3(0,0,0);
                }

                if (_tempTarget.Name == "null")
                {
                    _tempDist = H.MaxFOV;
                }
                AimPlayer(_tempTarget);
                Thread.Sleep(5);
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }

}
