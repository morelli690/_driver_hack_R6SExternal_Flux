using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using Flux.Helpers;

namespace Flux.Game
{
    public class EntityFunctions
    {
        private readonly AddressManager _address;
        private readonly Offsets _offset;

        public EntityFunctions(Offsets offset, AddressManager address)
        {
            _offset = offset;
            _address = address;
        }

        public void BuildEntityList()
        {
            while (true)
            {
                var localEntity = GetLocalEntity();
                var localEntityInfo = GetAllEntityInfo(localEntity);
                H.LocalPlayer = localEntityInfo;
                var entityList = new List<PlayerInfo>();
                for (var i = 0; i < 12; i++)
                {
                    var entity = GetEntity(i);
                    if (entity == IntPtr.Zero || entity == localEntity) continue;
                    var player = GetAllEntityInfo(entity);
                    if (player.TeamId == 3 || player.TeamId == localEntityInfo.TeamId) continue;
                    if (player.Health <= 0 || player.Health > 200) continue;
                    if (player.W2SHead.Length <= 0 || player.W2SHead.Z < 1.0f) continue;
                    entityList.Add(player);
                }

                H.Players = entityList;
                Thread.Sleep(5);
            }
        }

        private IntPtr GetEntity(int i)
        {
            var entityBase = M.Read<IntPtr>(_address.PEntityList + i * _offset.Entity);
            return M.Read<IntPtr>(entityBase + _offset.EntityRef);
        }

        private string GetEntityPlayerName(IntPtr entity)
        {
            var playerInfo = M.Read<IntPtr>(entity + _offset.PlayerInfo);
            var playerInfoD = M.Read<IntPtr>(playerInfo);

            var pNameEntry = M.Read<IntPtr>(playerInfoD + _offset.PlayerName);
            var nameEntry = M.ReadStr(pNameEntry, 20);
            return nameEntry;
        }

        private IntPtr GetLocalEntity()
        {
            for (var i = 0; i < 12; i++)
            {
                var entity = GetEntity(i);
                var entityName = GetEntityPlayerName(entity);
                if (entityName != "benny.v2") continue;
                H.LocalPlayer = GetAllEntityInfo(entity);
                return entity;
            }

            return IntPtr.Zero;
        }

        private uint GetEntityHealth(IntPtr entity)
        {
            var entityInfo = M.Read<IntPtr>(entity + _offset.EntityInfo);

            var mainComponent = M.Read<IntPtr>(entityInfo + _offset.MainComponent);

            var childComponent = M.Read<IntPtr>(mainComponent + _offset.ChildComponent);

            return M.Read<uint>(childComponent + _offset.Health);
        }

        private static Vector3 GetBone(IntPtr tmp, int bone)
        {
            return M.Read<Vector3>(tmp + bone);
        }

        private Vector3[] GetAllBones(IntPtr entity)
        {
            var tmp = M.Read<IntPtr>(entity + 0x18);
            tmp = M.Read<IntPtr>(tmp + 0x138);
            tmp = M.Read<IntPtr>(tmp + 0xB8);
            tmp = M.Read<IntPtr>(tmp + 0x88);

            Vector3[] bones =
            {
                GetBone(tmp, _offset.LHand), GetBone(tmp, _offset.RHand),
                GetBone(tmp, _offset.LElbow), GetBone(tmp, _offset.RElbow),
                GetBone(tmp, _offset.LKnee), GetBone(tmp, _offset.RKnee),
                GetBone(tmp, _offset.LFoot), GetBone(tmp, _offset.RFoot),
                GetBone(tmp, _offset.HNeck), GetBone(tmp, _offset.LNeck),
                GetBone(tmp, _offset.LAnkle), GetBone(tmp, _offset.RAnkle),
                GetBone(tmp, _offset.LHip), GetBone(tmp, _offset.RHip)
            };
            var i = 0;
            foreach (var bone in bones)
            {
                bones[i] = WorldToScreen(bone);
                i++;
            }

            return bones;
        }

        private Vector3 GetEntityHeadPosition(IntPtr entity)
        {
            return M.Read<Vector3>(entity + _offset.HeadPosition);
        }

        private Vector3 GetEntityFeetPosition(IntPtr entity)
        {
            return M.Read<Vector3>(entity + _offset.FeetPosition);
        }

        private byte GetEntityTeamId(IntPtr entity)
        {
            var playerInfo = M.Read<IntPtr>(entity + _offset.PlayerInfo);
            var playerInfoD = M.Read<IntPtr>(playerInfo + 0x0);
            return M.Read<byte>(playerInfoD + _offset.PlayerTeamId);
        }

        private PlayerInfo GetAllEntityInfo(IntPtr entity)
        {
            var p = new PlayerInfo
            {
                Health = GetEntityHealth(entity),
                Name = GetEntityPlayerName(entity),
                Position = GetEntityFeetPosition(entity),
                TeamId = GetEntityTeamId(entity)
            };

            if (p.Health < 1 || p.Health > 160)
            {
                p.TeamId = 3;
                return p;
            }

            WorldToScreen(p.Position);
            p.W2SHead = WorldToScreen(GetEntityHeadPosition(entity));
            p.W2SBones = GetAllBones(entity);
            return p;
        }

        private Vector3 GetViewTranslation()
        {
            return M.Read<Vector3>(_address.PCamera + _offset.ViewTranslastion);
        }

        private Vector3 GetViewRight()
        {
            return M.Read<Vector3>(_address.PCamera + _offset.ViewRight);
        }

        private Vector3 GetViewUp()
        {
            return M.Read<Vector3>(_address.PCamera + _offset.ViewUp);
        }

        private Vector3 GetViewForward()
        {
            return M.Read<Vector3>(_address.PCamera + _offset.ViewForward);
        }

        private float GetFovx()
        {
            return M.Read<float>(_address.PCamera + _offset.Fovx);
        }

        private float GetFovy()
        {
            return M.Read<float>(_address.PCamera + _offset.Fovy);
        }


        private Vector3 WorldToScreen(Vector3 position)
        {
            H.ViewTranslation = GetViewTranslation();
            var temp = position - GetViewTranslation();

            var x = DotProduct(temp, GetViewRight());
            var y = DotProduct(temp, GetViewUp());
            var z = DotProduct(temp, GetViewForward() * -1);
            return new Vector3(
                2560 / 2 * (1 + x / GetFovx() / z),
                1440 / 2 * (1 - y / GetFovy() / z),
                z);
        }

        private static float DotProduct(Vector3 vecA, Vector3 vecB)
        {
            return vecA.X * vecB.X + vecA.Y * vecB.Y + vecA.Z * vecB.Z;
        }

        public struct PlayerInfo
        {
            public uint Health;
            public Vector3 Position;
            public Vector3 W2SHead;
            public Vector3[] W2SBones;
            public byte TeamId;
            public string Name;
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        private bool Equals(Vector3 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector3 vector3 && Equals(vector3);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public readonly float X;
        public float Y;
        public readonly float Z;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

        #region Overrides

        public override string ToString()
        {
            return $"{X},{Y},{Z}";
        }

        public float Distance(Vector3 vector)
        {
            return (float) Math.Sqrt(
                (X - vector.X) * (X - vector.X) +
                (Y - vector.Y) * (Y - vector.Y) +
                (Z - vector.Z) * (Y - vector.Z));
        }

        #endregion

        #region Operators

        public static Vector3 operator +(Vector3 vecA, Vector3 vecB)
        {
            return new Vector3(vecA.X + vecB.X, vecA.Y + vecB.Y, vecA.Z + vecB.Z);
        }

        public static Vector3 operator -(Vector3 vecA, Vector3 vecB)
        {
            return new Vector3(vecA.X - vecB.X, vecA.Y - vecB.Y, vecA.Z - vecB.Z);
        }

        public static Vector3 operator *(Vector3 vecA, Vector3 vecB)
        {
            return new Vector3(vecA.X * vecB.X, vecA.Y * vecB.Y, vecA.Z * vecB.Z);
        }

        public static Vector3 operator *(Vector3 vecA, int n)
        {
            return new Vector3(vecA.X * n, vecA.Y * n, vecA.Z * n);
        }

        public static Vector3 operator /(Vector3 vecA, Vector3 vecB)
        {
            return new Vector3(vecA.X / vecB.X, vecA.Y / vecB.Y, vecA.Z / vecB.Z);
        }

        public static bool operator ==(Vector3 vecA, Vector3 vecB)
        {
            return vecA.X == vecB.X && vecA.Y == vecB.Y && vecA.Y == vecB.Y;
        }

        public static bool operator !=(Vector3 vecA, Vector3 vecB)
        {
            return vecA.X != vecB.X || vecA.Y != vecB.Y || vecA.Y != vecB.Y;
        }

        #endregion
    }
}