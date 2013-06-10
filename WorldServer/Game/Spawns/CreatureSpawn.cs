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
using Framework.Constants;
using Framework.Constants.NetMessage;
using Framework.Database;
using Framework.Logging;
using Framework.Network.Packets;
using Framework.ObjectDefines;
using WorldServer.Game.WorldEntities;

namespace WorldServer.Game.Spawns
{
    public class CreatureSpawn : WorldObject
    {
        public int Id;
        public Creature Creature;

        public CreatureSpawn(int updateLength = (int)UnitFields.End) : base(updateLength) { }

        public static ulong GetLastGuid()
        {
            SQLResult result = DB.World.Select("SELECT * FROM `creature_spawns` ORDER BY `guid` DESC LIMIT 1");
            if (result.Count != 0)
                return result.Read<ulong>(0, "guid");

            return 0;
        }

        public void CreateFullGuid()
        {
            Guid = new SmartGuid(Guid, Id, HighGuidType.Unit).Guid;
        }

        public void CreateData(Creature creature)
        {
            Creature = creature;
        }

        public bool AddToDB()
        {
            if (DB.World.Execute("INSERT INTO creature_spawns (Id, Map, X, Y, Z, O) VALUES (?, ?, ?, ?, ?, ?)", Id, Map, Position.X, Position.Y, Position.Z, Position.O))
            {
                Log.Message(LogType.DB, "Creature (Id: {0}) successfully spawned (Guid: {1})", Id, Guid);
                return true;
            }

            return false;
        }

        public void AddToWorld()
        {
            CreateFullGuid();
            CreateData(Creature);

            Globals.SpawnMgr.AddSpawn(this);

            WorldObject obj = this;
            UpdateFlag updateFlags = UpdateFlag.Alive | UpdateFlag.Rotation;

            foreach (var v in Globals.WorldMgr.Sessions)
            {
                Character pChar = v.Value.Character;

                if (pChar.CheckDistance(this))
                {
                    PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);

                    updateObject.WriteUInt16((ushort)Map);
                    updateObject.WriteUInt32(1);

                    WorldMgr.WriteCreateObject(ref updateObject, obj, updateFlags, ObjectType.Unit);

                    v.Value.Send(ref updateObject);
                }
            }
        }

        public override void SetUpdateFields()
        {
            // ObjectFields
            SetUpdateField<ulong>((int)ObjectFields.Guid, Guid);
            SetUpdateField<ulong>((int)ObjectFields.Data, 0);
            SetUpdateField<int>((int)ObjectFields.Entry, Id);
            SetUpdateField<int>((int)ObjectFields.Type, 0x9);
            SetUpdateField<float>((int)ObjectFields.Scale, Creature.Data.Scale);

            // UnitFields
            SetUpdateField<ulong>((int)UnitFields.Charm, 1);
            SetUpdateField<ulong>((int)UnitFields.Summon, 1);
            SetUpdateField<ulong>((int)UnitFields.Critter, 0);
            SetUpdateField<ulong>((int)UnitFields.CharmedBy, 0);
            SetUpdateField<ulong>((int)UnitFields.SummonedBy, 0);
            SetUpdateField<ulong>((int)UnitFields.CreatedBy, 0);
            SetUpdateField<ulong>((int)UnitFields.Target, 0);
            SetUpdateField<ulong>((int)UnitFields.ChannelObject, 0);

            SetUpdateField<int>((int)UnitFields.Health, Creature.Data.Health);

            for (int i = 0; i < 5; i++)
                SetUpdateField<int>((int)UnitFields.Power + i, 0);

            SetUpdateField<int>((int)UnitFields.MaxHealth, Creature.Data.Health);

            for (int i = 0; i < 5; i++)
                SetUpdateField<int>((int)UnitFields.MaxPower + i, 0);

            SetUpdateField<int>((int)UnitFields.PowerRegenFlatModifier, 0);
            SetUpdateField<int>((int)UnitFields.PowerRegenInterruptedFlatModifier, 0);
            SetUpdateField<int>((int)UnitFields.BaseHealth, 1);
            SetUpdateField<int>((int)UnitFields.BaseMana, 0);
            SetUpdateField<int>((int)UnitFields.Level, Creature.Data.Level);
            SetUpdateField<int>((int)UnitFields.FactionTemplate, Creature.Data.Faction);
            SetUpdateField<int>((int)UnitFields.Flags, Creature.Data.UnitFlags);
            SetUpdateField<int>((int)UnitFields.Flags2, Creature.Data.UnitFlags2);
            SetUpdateField<int>((int)UnitFields.NpcFlags, Creature.Data.NpcFlags);

            for (int i = 0; i < 5; i++)
            {
                SetUpdateField<int>((int)UnitFields.Stats + i, 0);
                SetUpdateField<int>((int)UnitFields.StatPosBuff + i, 0);
                SetUpdateField<int>((int)UnitFields.StatNegBuff + i, 0);
            }

            SetUpdateField<byte>((int)UnitFields.DisplayPower, 0, 0);
            SetUpdateField<byte>((int)UnitFields.DisplayPower, 0, 1);
            SetUpdateField<byte>((int)UnitFields.DisplayPower, 0, 2);
            SetUpdateField<byte>((int)UnitFields.DisplayPower, 0, 3);

            SetUpdateField<int>((int)UnitFields.DisplayID, Creature.Stats.DisplayInfoId[0]);
            SetUpdateField<int>((int)UnitFields.NativeDisplayID, Creature.Stats.DisplayInfoId[2]);
            SetUpdateField<int>((int)UnitFields.MountDisplayID, 0);

            SetUpdateField<float>((int)UnitFields.BoundingRadius, 0.389F);
            SetUpdateField<float>((int)UnitFields.CombatReach, 1.5F);
            SetUpdateField<float>((int)UnitFields.MinDamage, 0);
            SetUpdateField<float>((int)UnitFields.MaxDamage, 0);
            SetUpdateField<float>((int)UnitFields.ModCastingSpeed, 1);
            SetUpdateField<int>((int)UnitFields.AttackPower, 0);
            SetUpdateField<int>((int)UnitFields.RangedAttackPower, 0);

            for (int i = 0; i < 7; i++)
            {
                SetUpdateField<int>((int)UnitFields.Resistances + i, 0);
                SetUpdateField<int>((int)UnitFields.ResistanceBuffModsPositive + i, 0);
                SetUpdateField<int>((int)UnitFields.ResistanceBuffModsNegative + i, 0);
            }

            SetUpdateField<byte>((int)UnitFields.AnimTier, 0, 0);
            SetUpdateField<byte>((int)UnitFields.AnimTier, 0, 1);
            SetUpdateField<byte>((int)UnitFields.AnimTier, 0, 2);
            SetUpdateField<byte>((int)UnitFields.AnimTier, 0, 3);

            SetUpdateField<short>((int)UnitFields.RangedAttackRoundBaseTime, 0);
            SetUpdateField<short>((int)UnitFields.RangedAttackRoundBaseTime, 0, 1);
            SetUpdateField<float>((int)UnitFields.MinOffHandDamage, 0);
            SetUpdateField<float>((int)UnitFields.MaxOffHandDamage, 0);
            SetUpdateField<int>((int)UnitFields.AttackPowerModPos, 0);
            SetUpdateField<int>((int)UnitFields.RangedAttackPowerModPos, 0);
            SetUpdateField<float>((int)UnitFields.MinRangedDamage, 0);
            SetUpdateField<float>((int)UnitFields.MaxRangedDamage, 0);
            SetUpdateField<float>((int)UnitFields.AttackPowerMultiplier, 0);
            SetUpdateField<float>((int)UnitFields.RangedAttackPowerMultiplier, 0);
            SetUpdateField<float>((int)UnitFields.MaxHealthModifier, 1);
        }
    }
}
