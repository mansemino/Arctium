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
using Framework.Constants.NetMessage;
using Framework.Network.Packets;
using WorldServer.Game.WorldEntities;
using WorldServer.Network;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class SpellHandler : Globals
    {
        public static void HandleSendKnownSpells(ref WorldClass session)
        {
            Character pChar = session.Character;

            var specializationSpells = SpecializationMgr.GetSpecializationSpells(pChar);
            var specializationSpellCount = ((specializationSpells != null) ? specializationSpells.Count : 0);

            var talentSpells = SpecializationMgr.GetTalentSpells(pChar, pChar.ActiveSpecGroup);
            var talentSpellCount = ((talentSpells != null) ? talentSpells.Count : 0);

            int count = pChar.SpellList.Count + specializationSpellCount + talentSpellCount;

            PacketWriter writer = new PacketWriter(ServerMessage.SendKnownSpells);
            BitPack BitPack = new BitPack(writer);

            BitPack.Write(count, 22);
            BitPack.Write(1);

            BitPack.Flush();

            pChar.SpellList.ForEach(spell =>
                writer.WriteUInt32(spell.SpellId));

            if (specializationSpells != null)
                specializationSpells.ForEach(spell => writer.WriteUInt32(spell.Spell));

            if (talentSpells != null)
                talentSpells.ForEach(spell => writer.WriteUInt32(spell));

            session.Send(ref writer);
        }

        public static void HandleLearnedSpells(ref WorldClass session, List<uint> newSpells)
        {
            PacketWriter writer = new PacketWriter(ServerMessage.LearnedSpells);
            BitPack BitPack = new BitPack(writer);

            BitPack.Write(0);
            BitPack.Write<int>(newSpells.Count, 22);

            BitPack.Flush();

            for (int i = 0; i < newSpells.Count; i++)
                writer.WriteUInt32(newSpells[i]);

            session.Send(ref writer);
        }

        public static void HandleUnlearnedSpells(ref WorldClass session, List<uint> oldSpells)
        {
            PacketWriter writer = new PacketWriter(ServerMessage.UnlearnedSpells);
            BitPack BitPack = new BitPack(writer);

            BitPack.Write<int>(oldSpells.Count, 22);
            BitPack.Flush();

            for (int i = 0; i < oldSpells.Count; i++)
                writer.WriteUInt32(oldSpells[i]);

            session.Send(ref writer);
        }
    }
}
