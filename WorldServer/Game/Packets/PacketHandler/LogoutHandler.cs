﻿/*
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

using Framework.Constants.NetMessage;
using Framework.Network.Packets;
using WorldServer.Network;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class LogoutHandler : Globals
    {
        [Opcode(ClientMessage.CliLogoutRequest, "17538")]
        public static void HandleLogoutRequest(ref PacketReader packet, WorldClass session)
        {
            var pChar = session.Character;

            ObjectMgr.SavePositionToDB(pChar);

            PacketWriter logoutComplete = new PacketWriter(ServerMessage.LogoutComplete);
            session.Send(ref logoutComplete);

            // Destroy object after logout
            WorldMgr.SendToInRangeCharacter(pChar, ObjectHandler.HandleDestroyObject(ref session, pChar.Guid));
            WorldMgr.DeleteSession(pChar.Guid);
        }
    }
}
