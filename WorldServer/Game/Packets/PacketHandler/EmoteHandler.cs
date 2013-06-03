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
using System.Linq;
using Framework.Constants;
using Framework.Constants.NetMessage;
using Framework.Logging;
using Framework.Network.Packets;
using WorldServer.Game.ObjectDefines;
using WorldServer.Network;
using Framework.Database;
using Framework.ObjectDefines;

using Framework.ClientDB;
using Framework.ClientDB.Structures.Dbc;
using System.Text;
using WorldServer.Game.Chat;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class EmoteHandler : Globals
    {
        [Opcode(ClientMessage.CliChatMessageAfk, "16992")]
        public static void HandleChangePlayerAFKState(ref PacketReader packet, ref WorldClass session)
        {
            var pChar           = session.Character;
            string afkText      = "";

            byte stateLength    = packet.Read<byte>();

            if (stateLength > 0)
                afkText = packet.ReadString(stateLength);

            pChar.setAfkState(afkText);
        }

        [Opcode(ClientMessage.CliChatMessageDnd, "16992")]
        public static void HandleChangePlayerDNDState(ref PacketReader packet, ref WorldClass session)
        {
            var pChar           = session.Character;
            string dndText      = ""; 

            uint stateLength    = packet.Read<byte>();

            if (stateLength > 0)
                dndText = packet.ReadString(stateLength);

            pChar.setDndState(dndText);
        }

    }
}
