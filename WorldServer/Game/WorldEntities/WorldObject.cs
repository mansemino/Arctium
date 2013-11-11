/*
 * Copyright (C) 2012-2013 Arctium <http://arctium.org>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections;
using Framework.Network.Packets;
using Framework.ObjectDefines;
using WorldServer.Game.Spawns;

namespace WorldServer.Game.WorldEntities
{
    public class WorldObject : Globals
    {
        // General object data
        public ulong Guid;
        public Vector4 Position;
        public uint Map;

        // Some data
        public ulong TargetGuid;

        public int MaskSize;
        public BitArray Mask;
        public Hashtable UpdateData = new Hashtable();

        public WorldObject() { }
        
        public WorldObject(int dataLength)
        {
            MaskSize = (dataLength + 32) / 32;
            Mask = new BitArray(dataLength, false);
        }

        public bool CheckDistance(WorldObject obj, float dist = 10000)
        {
            if (Map == obj.Map)
            {
                float disX = (float)Math.Pow(Position.X - obj.Position.X, 2);
                float disY = (float)Math.Pow(Position.Y - obj.Position.Y, 2);
                float disZ = (float)Math.Pow(Position.Z - obj.Position.Z, 2);

                float distance = disX + disY + disZ;

                return distance <= dist;
            }

            return false;
        }

        public virtual void SetUpdateFields() { }

        public void SetUpdateField<T>(int index, T value, byte offset = 0)
        {
            var typeName = value.GetType().Name;

            switch (typeName)
            {
                case "SByte":
                case "Int16":
                {
                    Mask.Set(index, true);

                    if (UpdateData.ContainsKey(index))
                        UpdateData[index] = (int)((int)UpdateData[index] | (int)((int)Convert.ChangeType(value, typeof(int)) << (offset * (typeName == "SByte" ? 8 : 16))));
                    else
                        UpdateData[index] = (int)((int)Convert.ChangeType(value, typeof(int)) << (offset * (typeName == "SByte" ? 8 : 16)));

                    break;
                }
                case "Byte":
                case "UInt16":
                {
                    Mask.Set(index, true);

                    if (UpdateData.ContainsKey(index))
                        UpdateData[index] = (uint)((uint)UpdateData[index] | (uint)((uint)Convert.ChangeType(value, typeof(uint)) << (offset * (typeName == "Byte" ? 8 : 16))));
                    else
                        UpdateData[index] = (uint)((uint)Convert.ChangeType(value, typeof(uint)) << (offset * (typeName == "Byte" ? 8 : 16)));

                    break;
                }
                case "Int64":
                {
                    Mask.Set(index, true);
                    Mask.Set(index + 1, true);

                    long tmpValue = (long)Convert.ChangeType(value, typeof(long));

                    UpdateData[index] = (uint)(tmpValue & int.MaxValue);
                    UpdateData[index + 1] = (uint)((tmpValue >> 32) & int.MaxValue);

                    break;
                }
                case "UInt64":
                {
                    Mask.Set(index, true);
                    Mask.Set(index + 1, true);

                    ulong tmpValue = (ulong)Convert.ChangeType(value, typeof(ulong));

                    UpdateData[index] = (uint)(tmpValue & uint.MaxValue);
                    UpdateData[index + 1] = (uint)((tmpValue >> 32) & uint.MaxValue);
                    
                    break;
                }
                default:
                {
                    Mask.Set(index, true);
                    UpdateData[index] = value;

                    break;
                }
            }
        }

        public void WriteUpdateFields(ref PacketWriter packet)
        {
            packet.WriteUInt8((byte)MaskSize);
            packet.WriteBitArray(Mask, MaskSize * 4);

            for (int i = 0; i < Mask.Count; i++)
            {
                if (Mask.Get(i))
                {
                    try
                    {
                        switch (UpdateData[i].GetType().Name)
                        {
                            case "UInt32":
                                packet.WriteUInt32((uint)UpdateData[i]);
                                break;
                            case "Single":
                                packet.WriteFloat((float)UpdateData[i]);
                                break;
                            default:
                                packet.WriteInt32((int)UpdateData[i]);
                                break;
                        }
                    }
                    catch
                    {
                    }
                }
            }

            Mask.SetAll(false);
        }

        public void WriteDynamicUpdateFields(ref PacketWriter packet)
        {
            packet.WriteUInt8(0);
        }

        public void RemoveFromWorld()
        {

        }

        public Character ToCharacter()
        {
            return this as Character;
        }

        public CreatureSpawn ToCreature()
        {
            return this as CreatureSpawn;
        }

        public GameObjectSpawn ToGameObject()
        {
            return this as GameObjectSpawn;
        }
    }
}
