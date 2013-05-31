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

using Framework.Constants.NetMessage;
using Framework.Network.Packets;
using WorldServer.Network;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class LogoutHandler : Globals
    {
        [Opcode(ClientMessage.CliLogoutRequest, "16992")]
        public static void HandleLogoutRequest(ref PacketReader packet, ref WorldClass session)
        {
            LogOutMgr.Add(session.Character.Guid);

            PacketWriter logoutResponse = new PacketWriter(ServerMessage.LogoutResponse);
            BitPack BitPack = new BitPack(logoutResponse);
            logoutResponse.WriteUInt8(0);
            BitPack.Write(0);
            BitPack.Flush();
            session.Send(ref logoutResponse);

            PacketWriter StandStateUpdate = new PacketWriter(ServerMessage.StandStateUpdate);
            StandStateUpdate.WriteUInt8(1);
            session.Send(ref StandStateUpdate);

            RootHandler.HandleMoveRoot(ref session); 
        }

        [Opcode(ClientMessage.CliLogoutCancel, "16992")]
        public static void HandleLogoutCancel(ref PacketReader packet, ref WorldClass session)
        {
            LogOutMgr.Remove(session.Character.Guid);

            RootHandler.HandleMoveUnroot(ref session);

            PacketWriter LogoutCancelAck = new PacketWriter(ServerMessage.LogoutCancelAck);
            session.Send(ref LogoutCancelAck);
        }

        [Opcode(ClientMessage.CliLogoutInstant, "16992")]
        public static void HandleLogoutInstant(ref PacketReader packet, ref WorldClass session)
        {
            var pChar = session.Character;

            LogOutMgr.LogOut(ref session);
        }
    }
}
