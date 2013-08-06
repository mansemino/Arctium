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

using System.Threading;
using System.Threading.Tasks;
using Framework.Constants.NetMessage;
using Framework.Network.Packets;
using WorldServer.Network;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class LogoutHandler : Globals
    {
        static CancellationTokenSource cts;

        [Opcode(ClientMessage.CliLogoutRequest, "17128")]
        public static void HandleLogoutRequest(ref PacketReader packet, WorldClass session)
        {
            PacketWriter logoutResponse = new PacketWriter(ServerMessage.LogoutResponse);
            BitPack BitPack = new BitPack(logoutResponse);

            logoutResponse.WriteUInt8(0);
            BitPack.Write(0);
            BitPack.Flush();

            session.Send(ref logoutResponse);

            Task.Delay(20000).ContinueWith(_ => HandleLogoutComplete(session), (cts = new CancellationTokenSource()).Token);

            session.Character.setStandState(1);
            
            MoveHandler.HandleMoveRoot(session); 
        }

        public static void HandleLogoutComplete(WorldClass session)
        {
            var pChar = session.Character;

            ObjectMgr.SavePositionToDB(pChar);

            PacketWriter logoutComplete = new PacketWriter(ServerMessage.LogoutComplete);
            session.Send(ref logoutComplete);

            WorldMgr.SendToInRangeCharacter(pChar, Packets.PacketHandler.ObjectHandler.HandleDestroyObject(session, pChar.Guid));
            WorldMgr.DeleteSession(pChar.Guid);
        }  

        [Opcode(ClientMessage.CliLogoutCancel, "17128")]
        public static void HandleLogoutCancel(ref PacketReader packet, WorldClass session)
        {
            cts.Cancel();

            MoveHandler.HandleMoveUnroot(session);

            PacketWriter LogoutCancelAck = new PacketWriter(ServerMessage.LogoutCancelAck);
            session.Send(ref LogoutCancelAck);

            session.Character.setStandState(0);
        }

        [Opcode(ClientMessage.CliLogoutInstant, "17128")]
        public static void HandleLogoutInstant(ref PacketReader packet, WorldClass session)
        {
            var pChar = session.Character;

            HandleLogoutComplete(session);
        }
    }
}
