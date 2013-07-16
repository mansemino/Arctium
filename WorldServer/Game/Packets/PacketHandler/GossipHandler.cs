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
        public static void HandleTalkToGossip(ref PacketReader packet, ref WorldClass session)
        {
            BitUnpack BitUnpack = new BitUnpack(packet);

            byte[] guidMask = { 3, 1, 7, 4, 6, 0, 2, 5 };
            byte[] guidBytes = { 0, 1, 7, 6, 5, 2, 4, 3 };

            var guid = BitUnpack.GetPackedValue(guidMask, guidBytes);
            var gossipData = GossipMgr.GetGossip<Creature>(SmartGuid.GetGuid(guid));

            if (gossipData != null)
            {
                PacketWriter gossipMessage = new PacketWriter(ServerMessage.GossipMessage);
                BitPack BitPack = new BitPack(gossipMessage, guid);

                BitPack.WriteGuidMask(0, 1);
                BitPack.Write(0, 19);           // gossipData.QuestsCount
                BitPack.WriteGuidMask(2);
                BitPack.Write(0, 20);           // gossipData.OptionsCount

                // QuestsCount not supported.
                // for (int i = 0; i < gossipData.QuestsCount; i++)

                BitPack.WriteGuidMask(3);

                // OptionsCount not supported.
                // for (int i = 0; i < gossipData.OptionsCount; i++)

                BitPack.WriteGuidMask(5, 4, 6, 7);

                BitPack.Flush();

                BitPack.WriteGuidBytes(6);

                // OptionsCount not supported.
                // for (int i = 0; i < gossipData.OptionsCount; i++)

                BitPack.WriteGuidBytes(0);

                // QuestsCount not supported.
                // for (int i = 0; i < gossipData.QuestsCount; i++)

                gossipMessage.WriteInt32(gossipData.Id);

                BitPack.WriteGuidBytes(4, 3);

                gossipMessage.WriteInt32(gossipData.FriendshipFactionID);
                gossipMessage.WriteInt32(gossipData.TextID);

                BitPack.WriteGuidBytes(7, 1, 5, 1);

                session.Send(ref gossipMessage);
            }
        }
    }
}
