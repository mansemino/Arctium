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
using System.Collections.Generic;
using Framework.Configuration;
using Framework.Constants;
using Framework.Constants.NetMessage;
using Framework.Database;
using Framework.Logging;
using Framework.Network.Packets;
using Framework.ObjectDefines;
using WorldServer.Game.WorldEntities;
using WorldServer.Network;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class CacheHandler : Globals
    {
        [Opcode(ClientMessage.CliQueryCreature, "17538")]
        public static void HandleQueryCreature(ref PacketReader packet, WorldClass session)
        {
            var id = packet.Read<int>();

            PacketWriter queryCreatureResponse = new PacketWriter(ServerMessage.QueryCreatureResponse);
            BitPack BitPack = new BitPack(queryCreatureResponse);

            queryCreatureResponse.WriteInt32(id);

            var creature = DataMgr.FindCreature(id);
            var hasData = (creature != null);

            BitPack.Write(hasData);

            if (hasData)
            {
                var stats = creature.Stats;

                BitPack.Write(0, 11);
                BitPack.Write(stats.QuestItemId.Count, 22);
                BitPack.Write(stats.IconName.Length + 1, 6);
                BitPack.Write(stats.RacialLeader);

                for (int i = 0; i < 8; i++)
                {
                    if (i == 0)
                        BitPack.Write(stats.Name.Length + 1, 11);
                    else
                        BitPack.Write(0, 11);
                }

                BitPack.Write(stats.SubName.Length != 0 ? stats.SubName.Length + 1 : 0, 11);

                BitPack.Flush();

                queryCreatureResponse.WriteInt32(stats.Family);
                queryCreatureResponse.WriteInt32(stats.ExpansionRequired);

                queryCreatureResponse.WriteInt32(stats.Type);

                if (stats.SubName != "")
                    queryCreatureResponse.WriteCString(stats.SubName);

                queryCreatureResponse.WriteInt32(stats.DisplayInfoId[0]);
                queryCreatureResponse.WriteInt32(stats.DisplayInfoId[3]);

                foreach (var v in stats.QuestItemId)
                    queryCreatureResponse.WriteInt32(v);

                queryCreatureResponse.WriteCString(stats.Name);

                if (stats.IconName != "")
                    queryCreatureResponse.WriteCString(stats.IconName);

                foreach (var v in stats.Flag)
                    queryCreatureResponse.WriteInt32(v);

                queryCreatureResponse.WriteFloat(stats.HealthModifier);
                queryCreatureResponse.WriteInt32(stats.Rank);
                queryCreatureResponse.WriteInt32(stats.QuestKillNpcId[0]);
                queryCreatureResponse.WriteInt32(stats.QuestKillNpcId[1]);
                queryCreatureResponse.WriteFloat(stats.PowerModifier);
                queryCreatureResponse.WriteInt32(stats.MovementInfoId);
                queryCreatureResponse.WriteInt32(stats.DisplayInfoId[1]);
                queryCreatureResponse.WriteInt32(stats.DisplayInfoId[2]);
            }
            else
                Log.Message(LogType.Debug, "Creature (Id: {0}) not found.", id);

            session.Send(ref queryCreatureResponse);
        }

        [Opcode(ClientMessage.CliQueryGameObject, "17538")]
        public static void HandleQueryGameObject(ref PacketReader packet, WorldClass session)
        {
            byte[] guidMask = { 6, 4, 0, 5, 1, 7, 3, 2 };
            byte[] guidBytes = { 5, 0, 6, 7, 3, 4, 2, 1 };

            BitUnpack BitUnpack = new BitUnpack(packet);

            var id = packet.Read<int>();
            var guid = BitUnpack.GetPackedValue(guidMask, guidBytes);

            PacketWriter queryGameObjectResponse = new PacketWriter(ServerMessage.QueryGameObjectResponse);
            BitPack BitPack = new BitPack(queryGameObjectResponse);

            var gObject = DataMgr.FindGameObject(id);
            var hasData = (gObject != null);

            queryGameObjectResponse.WriteInt32(id);
            queryGameObjectResponse.WriteInt32(0);

            if (hasData)
            {
                var stats = gObject.Stats;

                queryGameObjectResponse.WriteInt32((int)stats.Type);
                queryGameObjectResponse.WriteInt32(stats.DisplayInfoId);

                queryGameObjectResponse.WriteCString(stats.Name);

                for (int i = 0; i < 3; i++)
                    queryGameObjectResponse.WriteCString("");

                queryGameObjectResponse.WriteCString(stats.IconName);
                queryGameObjectResponse.WriteCString(stats.CastBarCaption);
                queryGameObjectResponse.WriteCString("");

                foreach (var v in stats.Data)
                    queryGameObjectResponse.WriteInt32(v);

                queryGameObjectResponse.WriteFloat(stats.Size);
                queryGameObjectResponse.WriteUInt8((byte)stats.QuestItemId.Count);

                foreach (var v in stats.QuestItemId)
                    queryGameObjectResponse.WriteInt32(v);

                queryGameObjectResponse.WriteInt32(stats.ExpansionRequired);

                var size = (uint)queryGameObjectResponse.BaseStream.Length - 13;
                queryGameObjectResponse.WriteUInt32Pos(size, 9);
            }
            else
                Log.Message(LogType.Debug, "Gameobject (Id: {0}) not found.", id);

            BitPack.Write(hasData);
            BitPack.Flush();

            session.Send(ref queryGameObjectResponse);
        }

        [Opcode(ClientMessage.CliQueryNPCText, "17538")]
        public static void HandleCliQueryNPCText(ref PacketReader packet, WorldClass session)
        {
            BitUnpack BitUnpack = new BitUnpack(packet);

            byte[] guidMask = { 7, 0, 6, 4, 2, 3, 1, 5 };
            byte[] guidBytes = { 6, 4, 1, 3, 2, 5, 7, 0 };

            var gossipTextId = packet.Read<int>();
            var guid = BitUnpack.GetPackedValue(guidMask, guidBytes);

            var gossipData = GossipMgr.GetGossip<Creature>(SmartGuid.GetGuid(guid));

            if (gossipData != null)
            {
                PacketWriter queryNPCTextResponse = new PacketWriter(ServerMessage.QueryNPCTextResponse);
                BitPack BitPack = new BitPack(queryNPCTextResponse);

                queryNPCTextResponse.WriteInt32(gossipTextId);
                queryNPCTextResponse.WriteInt32(0);
                queryNPCTextResponse.WriteFloat(1);

                for (int i = 0; i < 7; i++)
                    queryNPCTextResponse.WriteUInt32(0);

                queryNPCTextResponse.WriteInt32(gossipData.BroadCastText.Id);

                for (int i = 0; i < 7; i++)
                    queryNPCTextResponse.WriteUInt32(0);

                BitPack.Write(1);
                BitPack.Flush();

                var size = (uint)queryNPCTextResponse.BaseStream.Length - 13;
                queryNPCTextResponse.WriteUInt32Pos(size, 8);

                session.Send(ref queryNPCTextResponse);
            }
        }

        [Opcode(ClientMessage.QueryPlayerName, "17538")]
        public static void HandleQueryPlayerName(ref PacketReader packet, WorldClass session)
        {
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            guidMask[1] = BitUnpack.GetBit();
            guidMask[3] = BitUnpack.GetBit();
            guidMask[6] = BitUnpack.GetBit();
            guidMask[7] = BitUnpack.GetBit();
            guidMask[2] = BitUnpack.GetBit();
            guidMask[5] = BitUnpack.GetBit();

            var hasUnknown = BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            var hasUnknown2 = BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();

            if (guidMask[4]) guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);
            if (guidMask[6]) guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);
            if (guidMask[7]) guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);
            if (guidMask[1]) guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);
            if (guidMask[2]) guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);
            if (guidMask[5]) guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);
            if (guidMask[0]) guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);
            if (guidMask[3]) guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);

            if (hasUnknown)
                packet.Read<uint>();

            if (hasUnknown2)
                packet.Read<uint>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);

            var pSession = WorldMgr.GetSession(guid);
            if (pSession != null)
            {
                var pChar = pSession.Character;

                if (pChar != null)
                {
                    PacketWriter queryPlayerNameResponse = new PacketWriter(ServerMessage.QueryPlayerNameResponse);
                    BitPack BitPack = new BitPack(queryPlayerNameResponse, guid);

                    BitPack.WriteGuidMask(3, 2, 6, 0, 4, 1, 5, 7);

                    BitPack.Flush();

                    BitPack.WriteGuidBytes(7, 1, 2, 6, 3, 5);

                    queryPlayerNameResponse.WriteUInt8(0);
                    queryPlayerNameResponse.WriteUInt8(pChar.Gender);
                    queryPlayerNameResponse.WriteUInt8(pChar.Class);
                    queryPlayerNameResponse.WriteUInt8(pChar.Level);
                    queryPlayerNameResponse.WriteUInt32(1);
                    queryPlayerNameResponse.WriteUInt8(pChar.Race);
                    queryPlayerNameResponse.WriteUInt32(0);

                    BitPack.WriteGuidBytes(4, 0);

                    BitPack.WriteGuidMask(5);
                    BitPack.Write(0);
                    BitPack.Write(0);
                    BitPack.Write(0);
                    BitPack.WriteGuidMask(3, 7, 0, 6);
                    BitPack.Write(0);
                    BitPack.Write(0);
                    BitPack.WriteGuidMask(1);
                    BitPack.Write(0);

                    for (int i = 0; i < 5; i++)
                        BitPack.Write(0, 7);

                    BitPack.WriteGuidMask(2);
                    BitPack.Write(0);
                    BitPack.WriteGuidMask(4);
                    BitPack.Write(0);
                    BitPack.Write(pChar.Name.Length, 6);
                    BitPack.Write(0);

                    BitPack.Flush();

                    BitPack.WriteGuidBytes(0, 7, 5, 2);

                    queryPlayerNameResponse.WriteString(pChar.Name);

                    BitPack.WriteGuidBytes(4, 1, 3, 6);

                    session.Send(ref queryPlayerNameResponse);
                }
            }
        }

        [Opcode(ClientMessage.QueryRealmName, "17538")]
        public static void HandleQueryRealmName(ref PacketReader packet, WorldClass session)
        {
            Character pChar = session.Character;

            uint realmId = packet.Read<uint>();

            SQLResult result = DB.Realms.Select("SELECT name FROM realms WHERE id = ?", WorldConfig.RealmId);
            string realmName = result.Read<string>(0, "Name");

            PacketWriter realmQueryResponse = new PacketWriter(ServerMessage.RealmQueryResponse);
            BitPack BitPack = new BitPack(realmQueryResponse);

            realmQueryResponse.WriteUInt8(0);
            realmQueryResponse.WriteUInt32(realmId);       // <= 0 => End of packet

            BitPack.Write(realmName.Length, 8);
            BitPack.Write(1);
            BitPack.Write(realmName.Length, 8);

            BitPack.Flush();

            realmQueryResponse.WriteString(realmName);
            realmQueryResponse.WriteString(realmName);

            session.Send(ref realmQueryResponse);
        }

        [Opcode(ClientMessage.DBQueryBulk, "17538")]
        public static void HandleDBQueryBulk(ref PacketReader packet, WorldClass session)
        {
            List<int> IdList = new List<int>();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var type = (DBTypes)packet.Read<uint>();
            var count = BitUnpack.GetBits<uint>(21);

            bool[][] Mask = new bool[count][];
            byte[][] Bytes = new byte[count][];

            for (int i = 0; i < count; i++)
            {
                Mask[i] = new bool[8];
                Bytes[i] = new byte[8];
            }

            for (int i = 0; i < count; i++)
            {
                Mask[i][1] = BitUnpack.GetBit();
                Mask[i][7] = BitUnpack.GetBit();
                Mask[i][2] = BitUnpack.GetBit();
                Mask[i][5] = BitUnpack.GetBit();
                Mask[i][0] = BitUnpack.GetBit();
                Mask[i][6] = BitUnpack.GetBit();
                Mask[i][3] = BitUnpack.GetBit();
                Mask[i][4] = BitUnpack.GetBit();
            }

            for (int i = 0; i < count; i++)
            {
                if (Mask[i][4])
                    Bytes[i][4] = (byte)(packet.Read<byte>() ^ 1);

                if (Mask[i][7])
                    Bytes[i][7] = (byte)(packet.ReadByte() ^ 1);

                if (Mask[i][6])
                    Bytes[i][6] = (byte)(packet.ReadByte() ^ 1);

                if (Mask[i][0])
                    Bytes[i][0] = (byte)(packet.ReadByte() ^ 1);

                if (Mask[i][2])
                    Bytes[i][2] = (byte)(packet.ReadByte() ^ 1);

                if (Mask[i][3])
                    Bytes[i][3] = (byte)(packet.ReadByte() ^ 1);

                IdList.Add(packet.Read<int>());

                if (Mask[i][5])
                    Bytes[i][5] = (byte)(packet.ReadByte() ^ 1);

                if (Mask[i][1])
                    Bytes[i][1] = (byte)(packet.ReadByte() ^ 1);
            }

            switch (type)
            {
                case DBTypes.BroadcastText:
                {
                    foreach (var id in IdList)
                        HandleBroadcastText(ref session, id);

                    break;
                }
                default:
                    break;
            }
        }

        public static void HandleBroadcastText(ref WorldClass session, int id)
        {
            var broadCastText = GossipMgr.GetBroadCastText<Creature>(id);

            PacketWriter dbReply = new PacketWriter(ServerMessage.DBReply);
            BitPack BitPack = new BitPack(dbReply);

            var textLength = broadCastText.Text.Length;
            var alternativeTextLength = broadCastText.AlternativeText.Length;
            var size = 48;

            if (textLength == 0 || alternativeTextLength == 0)
                size += 1;

            size += textLength + alternativeTextLength;

            dbReply.WriteUInt32(0);    // UnixTime, last change server side
            dbReply.WriteUInt32((uint)DBTypes.BroadcastText);
            dbReply.WriteUInt32((uint)size);

            dbReply.WriteInt32(broadCastText.Id);
            dbReply.WriteInt32(broadCastText.Language);

            dbReply.WriteUInt16((ushort)broadCastText.Text.Length);
            dbReply.WriteString(broadCastText.Text);

            dbReply.WriteUInt16((ushort)broadCastText.AlternativeText.Length);
            dbReply.WriteString(broadCastText.AlternativeText);

            broadCastText.Emotes.ForEach(emote => dbReply.WriteInt32(emote));

            dbReply.WriteUInt32(1);

            dbReply.WriteInt32(broadCastText.Id);

            session.Send(ref dbReply);
        }
    }
}
