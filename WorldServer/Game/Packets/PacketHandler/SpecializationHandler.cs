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

using System.Collections.Generic;
using System.Linq;
using Framework.ClientDB;
using Framework.Constants;
using Framework.Constants.NetMessage;
using Framework.Logging;
using Framework.Network.Packets;
using WorldServer.Network;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class SpecializationHandler : Globals
    {
        [Opcode(ClientMessage.CliSetSpecialization, "17538")]
        public static void HandleCliSetSpecialization(ref PacketReader packet, WorldClass session)
        {
            var pChar = session.Character;

            uint specGroupId = packet.Read<uint>();

            uint specId = SpecializationMgr.GetSpecIdByGroup(pChar, (byte)specGroupId);

            // Check if new spec is primary or secondary
            if (pChar.SpecGroupCount == 1 && pChar.PrimarySpec == 0)
            {
                pChar.ActiveSpecGroup = 0;
                pChar.PrimarySpec = (ushort)specId;
            }
            else
            {
                pChar.ActiveSpecGroup = 1;
                pChar.SecondarySpec = (ushort)specId;
                pChar.SpecGroupCount = (byte)(pChar.SpecGroupCount + 1);
            }

            SpecializationMgr.SaveSpecInfo(pChar);

            SendSpecializationSpells(ref session);
            HandleUpdateTalentData(ref session);

            pChar.SetUpdateField<int>((int)PlayerFields.CurrentSpecID, (int)pChar.GetActiveSpecId());
            ObjectHandler.HandleUpdateObjectValues(ref session);

            Log.Message(LogType.Debug, "Character (Guid: {0}) choosed specialization {1} for spec group {2}.", pChar.Guid, pChar.GetActiveSpecId(), pChar.ActiveSpecGroup);
        }

        [Opcode(ClientMessage.CliLearnTalents, "17538")]
        public static void HandleLearnTalents(ref PacketReader packet, WorldClass session)
        {
            var pChar = session.Character;
            var talentSpells = new List<uint>();

            var BitUnpack = new BitUnpack(packet);
            var talentCount = BitUnpack.GetBits<uint>(23);

            for (int i = 0; i < talentCount; i++)
            {
                var talentId = packet.Read<ushort>();

                SpecializationMgr.AddTalent(pChar, pChar.ActiveSpecGroup, talentId, true);

                talentSpells.Add(CliDB.Talent.Single(talent => talent.Id == talentId).SpellId);
            }

            HandleUpdateTalentData(ref session);

            pChar.SetUpdateField<int>((int)PlayerFields.CharacterPoints, SpecializationMgr.GetUnspentTalentRowCount(pChar), 0);
            ObjectHandler.HandleUpdateObjectValues(ref session);

            foreach (var talentSpell in talentSpells)
                SpellHandler.HandleLearnedSpells(ref session, new List<uint>(1) { talentSpell });

            Log.Message(LogType.Debug, "Character (Guid: {0}) learned {1} talents.", pChar.Guid, talentCount);
        }

        public static void HandleUpdateTalentData(ref WorldClass session)
        {
            var pChar = session.Character;

            const byte glyphCount = 6;

            PacketWriter updateTalentData = new PacketWriter(ServerMessage.UpdateTalentData);
            BitPack BitPack = new BitPack(updateTalentData);

            BitPack.Write(pChar.SpecGroupCount, 19);

            for (int i = 0; i < pChar.SpecGroupCount; i++)
            {
                var talents = SpecializationMgr.GetTalentsBySpecGroup(pChar, (byte)i);

                BitPack.Write(talents.Count, 23);
            }

            BitPack.Flush();

            for (int i = 0; i < pChar.SpecGroupCount; i++)
            {
                var talents = SpecializationMgr.GetTalentsBySpecGroup(pChar, (byte)i);
                var specId = (i == 0) ? pChar.PrimarySpec : pChar.SecondarySpec;

                for (int j = 0; j < glyphCount; j++)
                    updateTalentData.WriteInt16(0);                 // Glyph Id

                for (int j = 0; j < talents.Count; j++)
                    updateTalentData.WriteUInt16(talents[j].Id);    // Talent Id

                updateTalentData.WriteUInt32(specId);               // Spec Id
            }

            updateTalentData.WriteUInt8(pChar.ActiveSpecGroup);     // Active Spec (0 or 1)

            session.Send(ref updateTalentData);
        }

        public static void SendSpecializationSpells(ref WorldClass session)
        {
            var specSpells = SpecializationMgr.GetSpecializationSpells(session.Character);
            var newSpells = specSpells.Select(specializationSpell => specializationSpell.Spell).ToList();

            if (newSpells.Count > 0)
                SpellHandler.HandleLearnedSpells(ref session, newSpells);
        }
    }
}
