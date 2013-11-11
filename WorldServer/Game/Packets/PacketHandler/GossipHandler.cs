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
using Framework.ObjectDefines;
using WorldServer.Game.WorldEntities;
using WorldServer.Network;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class GossipHandler : Globals
    {
        [Opcode(ClientMessage.CliTalkToGossip, "17128")]
        public static void HandleTalkToGossip(ref PacketReader packet, WorldClass session)
        {
            BitUnpack BitUnpack = new BitUnpack(packet);

            byte[] guidMask = { 5, 7, 0, 1, 3, 2, 4, 6 };
            byte[] guidBytes = { 6, 3, 2, 0, 5, 1, 7, 4 };

            var guid = BitUnpack.GetPackedValue(guidMask, guidBytes);
            var gossipData = GossipMgr.GetGossip<Creature>(SmartGuid.GetGuid(guid));

            if (gossipData != null)
            {
                PacketWriter gossipMessage = new PacketWriter(ServerMessage.GossipMessage);
                BitPack BitPack = new BitPack(gossipMessage, guid);

                BitPack.Write(0, 20);              // gossipData.OptionsCount
                BitPack.WriteGuidMask(5, 1, 7, 2);
                BitPack.Write(0, 19);              // gossipData.QuestsCount
                BitPack.WriteGuidMask(6, 4, 0, 3);

                BitPack.Flush();

                BitPack.WriteGuidBytes(2, 1);

                gossipMessage.WriteInt32(gossipData.Id);

                BitPack.WriteGuidBytes(7, 4, 6);

                gossipMessage.WriteInt32(gossipData.FriendshipFactionID);

                BitPack.WriteGuidBytes(0, 5);

                gossipMessage.WriteInt32(gossipData.TextID);

                BitPack.WriteGuidBytes(3);

                session.Send(ref gossipMessage);
            }
        }
    }
}
