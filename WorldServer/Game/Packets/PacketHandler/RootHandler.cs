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
    public class RootHandler : Globals
    {
        public static void HandleMoveRoot(ref WorldClass session)
        {
            PacketWriter moveRoot = new PacketWriter(ServerMessage.MoveRoot);
            BitPack BitPack = new BitPack(moveRoot, session.Character.Guid);

            BitPack.WriteGuidMask(2, 5, 0, 7, 3, 6, 4, 1);
            BitPack.Flush();

            moveRoot.WriteUInt32(0);

            BitPack.WriteGuidBytes(5, 3, 6, 4, 0, 1, 7, 2);

            session.Send(ref moveRoot);
        }

        public static void HandleMoveUnroot(ref WorldClass session)
        {
            PacketWriter moveUnroot = new PacketWriter(ServerMessage.MoveUnroot);
            BitPack BitPack = new BitPack(moveUnroot, session.Character.Guid);

            BitPack.WriteGuidMask(1, 0, 7, 2, 3, 4, 5, 6);

            BitPack.WriteGuidBytes(3, 0, 1, 7, 4);
            BitPack.Flush();

            moveUnroot.WriteUInt32(0);

            BitPack.WriteGuidBytes(5, 2, 6);
            BitPack.Flush();

            session.Send(ref moveUnroot);
        }
    }
}
