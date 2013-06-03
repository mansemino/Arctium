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

using Framework.Constants;
using Framework.Constants.NetMessage;
using Framework.Network.Packets;
using Framework.ObjectDefines;
using WorldServer.Game.Chat;
using WorldServer.Network;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class ChatHandler : Globals
    {
        [Opcode(ClientMessage.ChatMessageSay, "16992")]
        public static void HandleChatMessageSay(ref PacketReader packet, ref WorldClass session)
        {
            BitUnpack BitUnpack = new BitUnpack(packet);

            var language = packet.Read<int>();

            var messageLength = BitUnpack.GetBits<byte>(8);
            var message = packet.ReadString(messageLength);

            ChatMessageValues chatMessage = new ChatMessageValues(MessageType.ChatMessageSay, message, true, true);
            chatMessage.Language = (byte)language;

            if (ChatCommandParser.CheckForCommand(message))
                ChatCommandParser.ExecuteChatHandler(message, session);
            else
                SendMessage(ref session, chatMessage);
        }

        [Opcode(ClientMessage.ChatMessageYell, "16992")]
        public static void HandleChatMessageYell(ref PacketReader packet, ref WorldClass session)
        {
            BitUnpack BitUnpack = new BitUnpack(packet);

            var language = packet.Read<int>();

            var messageLength = packet.ReadByte();
            var message = packet.ReadString(messageLength);

            ChatMessageValues chatMessage = new ChatMessageValues(MessageType.ChatMessageYell, message, true, true);
            chatMessage.Language = (byte)language;

            SendMessage(ref session, chatMessage);
        }

        [Opcode(ClientMessage.ChatMessageWhisper, "16992")]
        public static void HandleChatMessageWhisper(ref PacketReader packet, ref WorldClass session)
        {
            BitUnpack BitUnpack = new BitUnpack(packet);

            var language = packet.Read<int>();

            var nameLength = BitUnpack.GetBits<byte>(9);
            var messageLength = BitUnpack.GetBits<byte>(8);

            string message = packet.ReadString(messageLength);
            string receiverName = packet.ReadString(nameLength);

            WorldClass rSession = WorldMgr.GetSession(receiverName);

            if (rSession == null)
                return;

            ChatMessageValues chatMessage = new ChatMessageValues(MessageType.ChatMessageWhisperInform, message, false, true);
            SendMessage(ref session, chatMessage, rSession);

            chatMessage = new ChatMessageValues(MessageType.ChatMessageWhisper, message, false, true);
            SendMessage(ref rSession, chatMessage, session);

            if (rSession.Character.UnitIsAfk)
            {
                chatMessage = new ChatMessageValues(MessageType.ChatMessageAfk, rSession.Character.UnitIsAfkMessage, false, true);
                SendMessage(ref session, chatMessage, rSession);
            }
            else if (rSession.Character.UnitIsDnd)
            {
                chatMessage = new ChatMessageValues(MessageType.ChatMessageDnd, rSession.Character.UnitIsDndMessage, false, true);
                SendMessage(ref session, chatMessage, rSession);
            }
        }

        public static void SendMessage(ref WorldClass session, ChatMessageValues chatMessage, WorldClass pSession = null)
        {
            byte[] GuidMask = { 5, 4, 1, 0, 6, 2, 7, 3};
            byte[] GuidMask3 = { 1, 5, 7, 4, 2, 0, 6, 3 };
            byte[] GuidBytes = { 5, 0, 7, 4, 3, 2, 1, 6 };
            byte[] GuidBytes3 = { 7, 4, 2, 3, 1, 5, 6, 0 };

            var pChar = session.Character;
            var guid = pChar.Guid;

            if (pSession != null)
                guid = pSession.Character.Guid;

            PacketWriter chat = new PacketWriter(ServerMessage.Chat);
            BitPack BitPack = new BitPack(chat, guid);

            BitPack.Write(1);
            BitPack.Write(0);
            BitPack.Write(1);
            BitPack.Write(1);
            BitPack.Write(0, 8);
            BitPack.Write(0);
            BitPack.Write(1);
            BitPack.WriteGuidMask(GuidMask3);
            BitPack.Write(0);
            BitPack.Write(1);
            BitPack.Write(1);
            BitPack.Write(0, 8);
            BitPack.Write(0);
            BitPack.Write(1);
            BitPack.WriteStringLength(chatMessage.Message, 12);
            BitPack.Write(!chatMessage.HasRealmId);
            BitPack.Write(1);
            BitPack.Write(0);
            BitPack.WriteGuidMask(GuidMask);
            BitPack.Write(0);
            BitPack.Write(8, 9);
            BitPack.Write(!chatMessage.HasLanguage);

            BitPack.Flush();

            BitPack.WriteGuidBytes(GuidBytes3);
            BitPack.WriteGuidBytes(GuidBytes);

            chat.WriteUInt8((byte)chatMessage.ChatType);
            chat.WriteString(chatMessage.Message, false);

            if (chatMessage.HasRealmId)
                chat.WriteInt32(chatMessage.RealmId);

            if (chatMessage.HasLanguage)
                chat.WriteUInt8(chatMessage.Language);

            switch (chatMessage.ChatType)
            {
                case MessageType.ChatMessageSay:
                    WorldMgr.SendByDist(pChar, chat, 625);
                    break;
                case MessageType.ChatMessageYell:
                    WorldMgr.SendByDist(pChar, chat, 90000);
                    break;
                default:
                    session.Send(ref chat);
                    break;
            }
        }
    }
}

