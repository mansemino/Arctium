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

using Framework.Console.Commands;
using Framework.Constants;
using Framework.ObjectDefines;
using WorldServer.Game.Packets.PacketHandler;
using WorldServer.Game.Spawns;
using WorldServer.Game.WorldEntities;
using WorldServer.Network;

namespace WorldServer.Game.Chat.Commands
{
    public class CreatureCommands : Globals
    {
        [ChatCommand("addnpc")]
        public static void AddNpc(string[] args, WorldClass session)
        {
            var pChar = session.Character;

            int creatureId = CommandParser.Read<int>(args, 1);

            Creature creature = DataMgr.FindCreature(creatureId);
            if (creature != null)
            {
                ChatMessageValues chatMessage = new ChatMessageValues(0, "");

                CreatureSpawn spawn = new CreatureSpawn()
                {
                    Guid     = CreatureSpawn.GetLastGuid() + 1,
                    Id       = creatureId,
                    Creature = creature,
                    Position = pChar.Position,
                    Map      = pChar.Map
                };

                if (spawn.AddToDB())
                {
                    chatMessage.Message = "Spawn successfully added.";

                    spawn.AddToWorld();
                    ChatHandler.SendMessage(ref session, chatMessage);
                }
                else
                {
                    chatMessage.Message = "Spawn can't be added.";
                    ChatHandler.SendMessage(ref session, chatMessage);
                }
            }
        }

        [ChatCommand("delnpc")]
        public static void DeleteNpc(string[] args, WorldClass session)
        {
            ChatMessageValues chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, "");

            var pChar = session.Character;
            var spawn = SpawnMgr.FindSpawn(pChar.TargetGuid);

            if (spawn != null)
            {
                chatMessage.Message = "Selected Spawn successfully removed.";

                SpawnMgr.RemoveSpawn(spawn);
                WorldMgr.SendToInRangeCharacter(pChar, ObjectHandler.HandleDestroyObject(ref session, pChar.TargetGuid));
                ChatHandler.SendMessage(ref session, chatMessage);
            }
            else
            {
                chatMessage.Message = "Not a creature.";

                ChatHandler.SendMessage(ref session, chatMessage);
            }
        }
    }
}
