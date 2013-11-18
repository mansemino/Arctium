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

using System.Text;
using Framework.Configuration;
using Framework.Console.Commands;
using Framework.Constants;
using Framework.ObjectDefines;
using WorldServer.Game.Managers;
using WorldServer.Game.Packets.PacketHandler;
using WorldServer.Game.Spawns;
using WorldServer.Network;

namespace WorldServer.Game.Chat.Commands
{
    public class MiscCommands : Globals
    {
        [ChatCommand("help")]
        public static void Help(string[] args, WorldClass session)
        {
            StringBuilder commandList = new StringBuilder();

            foreach (var command in ChatCommandParser.ChatCommands)
            {
                var helpAttribute = (ChatCommandAttribute[])command.Value.Method.GetCustomAttributes(typeof(ChatCommandAttribute), false);
                foreach (var desc in helpAttribute)
                {
                    if (!string.IsNullOrEmpty(desc.Description))
                        commandList.AppendLine(WorldConfig.GMCommandStart + command.Key + " [" + desc.Description + "]");
                    else
                        commandList.AppendLine(WorldConfig.GMCommandStart + command.Key);
                }
            }

            ChatMessageValues chatMessage = new ChatMessageValues(0, commandList.ToString());

            ChatHandler.SendMessage(ref session, chatMessage);
        }

        [ChatCommand("save")]
        public static void Save(string[] args, WorldClass session)
        {
            ObjectMgr.SavePositionToDB(session.Character);

            ChatMessageValues chatMessage = new ChatMessageValues(0, "Your character is successfully saved to the database!");

            ChatHandler.SendMessage(ref session, chatMessage);
        }

        [ChatCommand("morph", "Usage: !morph #displayId (Change the current displayId for your own character or your target, if exists)")]
        public static void Morph(string[] args, WorldClass session)
        {
            var displayId = CommandParser.Read<uint>(args, 1);
            var pChar = session.Character;

            if (pChar != null)
            {
                if (pChar.TargetGuid == 0)
                {
                    pChar.SetUpdateField<uint>((int)UnitFields.DisplayID, displayId);
                    pChar.SetUpdateField<uint>((int)UnitFields.NativeDisplayID, displayId);

                    ObjectHandler.HandleUpdateObjectValues(ref session, true);

                    var chatMessage = new ChatMessageValues(0, "Successfully morphed");
                    ChatHandler.SendMessage(ref session, chatMessage);
                }
                else
                {
                    HighGuidType type = SmartGuid.GetGuidType(pChar.TargetGuid);
                    bool success = false;

                    switch (type)
                    {
                        case HighGuidType.Player:
                            var pSession = WorldMgr.GetSession(pChar.TargetGuid);
                            if (pSession == null)
                                return;
                            pSession.Character.SetUpdateField<uint>((int)UnitFields.DisplayID, displayId);
                            pSession.Character.SetUpdateField<uint>((int)UnitFields.NativeDisplayID, displayId);

                            ObjectHandler.HandleUpdateObjectValues(ref pSession, true);

                            success = true;
                            break;

                        case HighGuidType.Unit:
                        case HighGuidType.Pet:
                        default:
                            break;
                    }

                    if (success)
                    {
                        var chatMessage = new ChatMessageValues(0, "Successfully morphed");
                        ChatHandler.SendMessage(ref session, chatMessage);
                    }
                }                
            }
        }
    }
}
