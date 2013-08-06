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
        // This can be used also to send NPC animations
        public static PacketWriter PlayOneShotAnimKit(ushort AnimId, ulong guid)
        {
            PacketWriter result = new PacketWriter(ServerMessage.PlayOneShotAnimKit);
            BitPack Result = new BitPack(result, guid);

            byte[] bitmask = { 5, 6, 4, 0, 2, 3, 7, 1 };

            Result.WriteGuidMask(bitmask);

            Result.WriteGuidBytes(6, 0, 5, 3, 1, 4);

            result.WriteUInt16(AnimId);

            Result.WriteGuidBytes(7, 2);

            return result;
        }

        public static PacketWriter HandlePlayObjectSound(uint animKit, ulong sourceGuid, ulong targetGuid)
        {
            PacketWriter result = new PacketWriter(ServerMessage.PlayUnitEventSound);

            result.WriteUInt32(animKit);
            result.WriteUInt64(sourceGuid);
            result.WriteUInt64(targetGuid);

            return result;
        }

        public static PacketWriter TextEmotePacket(uint emote, int emoteSoundKit, string targetName, ulong sourceGuid)
        {
            byte[] bytes = { 7, 0, 5, 3, 4, 2, 1, 6 };

            PacketWriter result = new PacketWriter(ServerMessage.TextEmote);
            BitPack BitPack = new BitPack(result, sourceGuid);

            BitPack.WriteGuidMask(4, 0, 2, 6, 1, 5);

            BitPack.Write(targetName.Length, 7);

            BitPack.WriteGuidMask(7, 3);

            BitPack.Flush();

            result.WriteInt32(emoteSoundKit);

            result.WriteString(targetName, false);

            result.WriteUInt32(emote);

            BitPack.WriteGuidBytes(bytes);

            return result;
        }

        public static PacketWriter AnimEmotePacket(uint EmoteId, ulong guid)
        {
            PacketWriter result = new PacketWriter(ServerMessage.PlayEmote);

            result.WriteUInt32(EmoteId);
            result.WriteUInt64(guid);

            return result;
        }

        public static void HandlePlayEmote(uint emote, WorldClass session)
        {
            // Look for value into DBC. If not present, return
            var emotetext = CliDB.EmotesText.SingleOrDefault(textemote => textemote.ID == emote);
            // If it doesn't exists, advice and exit
            if (emotetext == null)
                return;

            // Take the value from DBC
            uint EmoteId = emotetext.EmoteId;

            // If it's zero, no animation so return
            if (EmoteId == 0)
                return;

            // There's an animation; Compose packet and send it to player and to nearest players
            PacketWriter AnimEmoteResponse = AnimEmotePacket(EmoteId, session.Character.Guid);
            session.Send(ref AnimEmoteResponse);

            WorldMgr.SendToInRangeCharacter(session.Character, AnimEmoteResponse);
        }

        public static void HandleTextEmote(uint emote, int emoteSoundKit, string targetName, ulong targetGuid, WorldClass session)
        {
            // Compose text emote response
            PacketWriter TextEmoteResponse = TextEmotePacket(emote, emoteSoundKit, targetName, targetGuid);
            session.Send(ref TextEmoteResponse);

            // Send it to nearest players.
            WorldMgr.SendToInRangeCharacter(session.Character, TextEmoteResponse);
        }

        [Opcode(ClientMessage.CliTextEmote, "17128")]
        public static void HandleEmote(ref PacketReader packet, WorldClass session)
        {
            BitUnpack GuidUnpacker  = new BitUnpack(packet);
            byte[] guidMask         = { 4, 7, 1, 2, 5, 3, 0, 6 };
            byte[] guidBytes        = { 6, 7, 4, 5, 2, 1, 3, 0 };

            uint emote              = packet.Read<uint>();
            int emoteSoundKit       = packet.Read<int>();

            var targetGuid          = GuidUnpacker.GetPackedValue(guidMask, guidBytes);

            string targetName       = "";
            string strEmote         = ""; // To log status

            // Look for value into DBC. If not present, return
            var emotetext = CliDB.EmotesText.SingleOrDefault(textemote => textemote.ID == emote);
            if (emotetext == null)
                return;

            strEmote = emotetext.Name;

            // If target, get the name according to type
            if (targetGuid != 0)
            {
                HighGuidType type = SmartGuid.GetGuidType(targetGuid);

                switch (type)
                {
                    // Get name from objects, etc
                    case HighGuidType.GameObject: // To do: Check if emote with objects is allowed 
                    case HighGuidType.Unit:
                        var target = DataMgr.FindCreature(SmartGuid.GetId(targetGuid));

                        if (target != null)
                            targetName = target.Stats.Name;
                        else
                        {
                            Log.Message(LogType.Debug, "Character (Guid: {0:X8}) tried to do the {1} emote with unknown target (Guid {2:X8}).", session.Character.Guid, strEmote, targetGuid);
                            return;
                        }
                        break;

                    // Get name from players
                    case HighGuidType.Player:
                        var pSession = WorldMgr.GetSession(targetGuid);
                        if (pSession != null)
                            targetName = pSession.Character.Name;
                        else
                        {
                            Log.Message(LogType.Debug, "Character (Guid: {0:X8}) tried to do the {1} emote with unknown player (Guid {2:X8}).", session.Character.Guid, strEmote, targetGuid);
                            return;
                        }
                        break;

                    // Get name from pets
                    case HighGuidType.Pet: // To do: Get pet name when pets were supported
                        Log.Message(LogType.Debug, "Character (Guid: {0:X8}) tried to do the {1} emote with a pet (unsupported ATM).", session.Character.Guid, strEmote);
                        break;

                    default:
                        Log.Message(LogType.Debug, "Character (Guid: {0:X8}) tried to do the {1} emote with an unknown target type.", session.Character.Guid, strEmote);
                        return;
                }
            }

            // Always send text emote
            HandleTextEmote(emote, emoteSoundKit, targetName, session.Character.Guid, session);

            // Check the animation: If exists and one shot
            var emoteanim = CliDB.EmotesText.SingleOrDefault(aemote => aemote.ID == emote);
            if ((emoteanim != null) && (emoteanim.EmoteId > 0))
            {
                var emotetype = CliDB.Emotes.SingleOrDefault(atype => atype.ID == emoteanim.EmoteId);
                if (emotetype != null)
                {
                    if ((emotetype.StandStateChange != session.Character.UnitStandState) || ((emotetype.StandStateChange != 0)))
                        session.Character.setStandState(emotetype.StandStateChange, true, false);
                    else
                    {
                        if (emotetype.FlagOneShot == 0)
                            HandlePlayEmote(emote, session);
                        else
                            session.Character.setEmoteState(emoteanim.EmoteId);
                    }
                }
            }
        }

        [Opcode(ClientMessage.CliChatMessageAfk, "17128")]
        public static void HandleChangePlayerAFKState(ref PacketReader packet, WorldClass session)
        {
            var pChar           = session.Character;
            string afkText      = "";

            byte stateLength    = packet.Read<byte>();

            if (stateLength > 0)
                afkText = packet.ReadString(stateLength);

            pChar.setAfkState(afkText);
        }

        [Opcode(ClientMessage.CliChatMessageDnd, "17128")]
        public static void HandleChangePlayerDNDState(ref PacketReader packet, WorldClass session)
        {
            var pChar           = session.Character;
            string dndText      = ""; 

            uint stateLength    = packet.Read<byte>();

            if (stateLength > 0)
                dndText = packet.ReadString(stateLength);

            pChar.setDndState(dndText);
        }

        [Opcode(ClientMessage.CliStandStateChange, "17128")]
        public static void HandleStandStateChange(ref PacketReader packet, WorldClass session)
        {
            int readedStatus = packet.Read<int>();

            session.Character.setStandState(readedStatus);
        }

        public static void HandleStandStateChangeAck(byte status, WorldClass session)
        {
            PacketWriter StandStateChangeAck = new PacketWriter(ServerMessage.StandStateUpdate);

            StandStateChangeAck.WriteUInt8(status);

            session.Send(ref StandStateChangeAck);
        }
    }
}
