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
using System.Linq;
using Framework.Constants;
using Framework.Constants.NetMessage;
using Framework.Database;
using Framework.Logging;
using Framework.Network.Packets;
using Framework.ObjectDefines;
using WorldServer.Game.ObjectDefines;
using WorldServer.Network;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class MiscHandler : Globals
    {
        public static void HandleMessageOfTheDay(ref WorldClass session)
        {
            PacketWriter motd = new PacketWriter(ServerMessage.MOTD);
            BitPack BitPack = new BitPack(motd);

            List<string> motds = new List<string>();

            motds.Add("Arctium MoP test");
            motds.Add("Welcome to our MoP server test.");
            motds.Add("Your development team =)");

            BitPack.Write<uint>(3, 4);

            motds.ForEach(m => BitPack.Write(m.Length, 7));

            BitPack.Flush();

            motds.ForEach(m => motd.WriteString(m));

            session.Send(ref motd);
        }

        [Opcode(ClientMessage.Ping, "17538")]
        public static void HandlePong(ref PacketReader packet, WorldClass session)
        {
            uint latency = packet.Read<uint>();
            uint sequence = packet.Read<uint>();

            PacketWriter pong = new PacketWriter(ServerMessage.Pong);

            pong.WriteUInt32(sequence);

            session.Send(ref pong);
        }

        [Opcode(ClientMessage.LogDisconnect, "17538")]
        public static void HandleDisconnectReason(ref PacketReader packet, WorldClass session)
        {
            var pChar = session.Character;
            uint disconnectReason = packet.Read<uint>();

            if (pChar != null)
                WorldMgr.DeleteSession(pChar.Guid);

            DB.Realms.Execute("UPDATE accounts SET online = 0 WHERE id = ?", session.Account.Id);

            Log.Message(LogType.Debug, "Account with Id {0} disconnected. Reason: {1}", session.Account.Id, disconnectReason);
        }

        public static void HandleCacheVersion(ref WorldClass session)
        {
            PacketWriter cacheVersion = new PacketWriter(ServerMessage.CacheVersion);

            cacheVersion.WriteUInt32(0);

            session.Send(ref cacheVersion);
        }

        [Opcode(ClientMessage.LoadingScreenNotify, "17538")]
        public static void HandleLoadingScreenNotify(ref PacketReader packet, WorldClass session)
        {
            BitUnpack BitUnpack = new BitUnpack(packet);

            uint mapId = packet.Read<uint>();
            bool loadingScreenState = BitUnpack.GetBit();

            Log.Message(LogType.Debug, "Loading screen for map '{0}' is {1}.", mapId, loadingScreenState ? "enabled" : "disabled");
        }

        [Opcode(ClientMessage.ViolenceLevel, "17538")]
        public static void HandleViolenceLevel(ref PacketReader packet, WorldClass session)
        {
            byte violenceLevel = packet.Read<byte>();

            Log.Message(LogType.Debug, "Violence level from account '{0} (Id: {1})' is {2}.", session.Account.Name, session.Account.Id, (ViolenceLevel)violenceLevel);
        }

        [Opcode(ClientMessage.ActivePlayer, "17538")]
        public static void HandleActivePlayer(ref PacketReader packet, WorldClass session)
        {
            byte active = packet.Read<byte>();    // Always 0

            Log.Message(LogType.Debug, "Player {0} (Guid: {1}) is active.", session.Character.Name, session.Character.Guid);
        }

        [Opcode(ClientMessage.CliSetSelection, "17538")]
        public static void HandleSetSelection(ref PacketReader packet, WorldClass session)
        {
            byte[] guidMask = { 7, 2, 1, 3, 5, 4, 0, 6 };
            byte[] guidBytes = { 1, 2, 3, 0, 7, 5, 4, 6 };

            BitUnpack GuidUnpacker = new BitUnpack(packet);

            ulong fullGuid = GuidUnpacker.GetPackedValue(guidMask, guidBytes);
            ulong guid = SmartGuid.GetGuid(fullGuid);

            if (session.Character != null)
            {
                var sess = WorldMgr.GetSession(session.Character.Guid);

                if (sess != null)
                    sess.Character.TargetGuid = fullGuid;

                if (guid == 0)
                    Log.Message(LogType.Debug, "Character (Guid: {0}) removed current selection.", session.Character.Guid);
                else
                    Log.Message(LogType.Debug, "Character (Guid: {0}) selected a {1} (Guid: {2}, Id: {3}).", session.Character.Guid, SmartGuid.GetGuidType(fullGuid), guid, SmartGuid.GetId(fullGuid));
            }
        }

        [Opcode(ClientMessage.SetActionButton, "17538")]
        public static void HandleSetActionButton(ref PacketReader packet, WorldClass session)
        {
            var pChar = session.Character;

            byte[] actionMask = { 5, 0, 3, 1, 4, 7, 6, 2 };
            byte[] actionBytes = { 3, 4, 6, 7, 1, 2, 0, 5 };
            
            BitUnpack actionUnpacker = new BitUnpack(packet);

            var slotId = packet.Read<byte>();
            var actionId = actionUnpacker.GetPackedValue(actionMask, actionBytes);
            
            if (actionId == 0)
            {
                var action = pChar.ActionButtons.Where(button => button.SlotId == slotId && button.SpecGroup == pChar.ActiveSpecGroup).Select(button => button).First();

                ActionMgr.RemoveActionButton(pChar, action, true);
                Log.Message(LogType.Debug, "Character (Guid: {0}) removed action button {1} from slot {2}.", pChar.Guid, actionId, slotId);

                return;
            }

            var newAction = new ActionButton
            {
                Action    = actionId,
                SlotId    = slotId,
                SpecGroup = pChar.ActiveSpecGroup
            };

            ActionMgr.AddActionButton(pChar, newAction, true);
            Log.Message(LogType.Debug, "Character (Guid: {0}) added action button {1} to slot {2}.", pChar.Guid, actionId, slotId);
        }

        public static void HandleUpdateActionButtons(ref WorldClass session)
        {
            var pChar = session.Character;

            PacketWriter updateActionButtons = new PacketWriter(ServerMessage.UpdateActionButtons);
            BitPack BitPack = new BitPack(updateActionButtons);

            const int buttonCount = 132;
            var buttons = new byte[buttonCount][];

            byte[] buttonMask = { 0, 1, 3, 4, 6, 7, 2, 5 };
            byte[] buttonBytes = { 5, 2, 0, 1, 3, 6, 4, 7 };

            var actions = ActionMgr.GetActionButtons(pChar, pChar.ActiveSpecGroup);
            
            for (int i = 0; i < buttonCount; i++)
                if (actions.Any(action => action.SlotId == i))
                    buttons[i] = BitConverter.GetBytes((ulong)actions.Where(action => action.SlotId == i).Select(action => action.Action).First());
                else
                    buttons[i] = new byte[8];

            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < buttonCount; j++)
                {
                    if (i < 8)
                        BitPack.Write(buttons[j][buttonMask[i]]);
                    else if (i < 16)
                    {
                        BitPack.Flush();

                        if (buttons[j][buttonBytes[i - 8]] != 0)
                            updateActionButtons.WriteUInt8((byte)(buttons[j][buttonBytes[i - 8]] ^ 1));
                    }
                }
            }

            // 0 - Initial packet on Login (no verification) / 1 - Verify spells on switch (Spec change) / 2 - Clear Action Buttons (Spec change)
            updateActionButtons.WriteInt8(0);

            session.Send(ref updateActionButtons);
        }
    }
}
