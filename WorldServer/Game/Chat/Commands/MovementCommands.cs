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
using Framework.Database;
using Framework.ObjectDefines;
using WorldServer.Game.Packets.PacketHandler;
using WorldServer.Network;

using System;
using System.Linq;
using System.Collections.Generic;
using Framework.ClientDB;
using Framework.ClientDB.Structures.Dbc;
using Framework.Constants;

namespace WorldServer.Game.Chat.Commands
{
    public class MovementCommands : Globals
    {
        [ChatCommand("fly", "Usage: !fly #state (Turns the fly mode 'on' or 'off')")]
        public static void Fly(string[] args, WorldClass session)
        {
            var state = CommandParser.Read<string>(args, 1);
            var message = state == "on" ? "Fly mode enabled." : "Fly mode disabled.";

            ChatMessageValues chatMessage = new ChatMessageValues(0, message);

            if (state == "on")
            {
                MoveHandler.HandleMoveSetCanFly(session);
                ChatHandler.SendMessage(session, chatMessage);
            }
            else if (state == "off")
            {
                MoveHandler.HandleMoveUnsetCanFly(session);
                ChatHandler.SendMessage(session, chatMessage);
            }
        }

        [ChatCommand("walkspeed", "Usage: !walkspeed #speed (Set the current walk speed)")]
        public static void WalkSpeed(string[] args, WorldClass session)
        {
            ChatMessageValues chatMessage = new ChatMessageValues(0, "");

            if (args.Length == 1)
                MoveHandler.HandleMoveSetWalkSpeed(session);
            else
            {
                var speed = CommandParser.Read<float>(args, 1);

                if (speed <= 50 && speed > 0)
                {
                    chatMessage.Message = "Walk speed set to " + speed + "!";

                    MoveHandler.HandleMoveSetWalkSpeed(session, speed);
                    ChatHandler.SendMessage(session, chatMessage);
                }
                else
                {
                    chatMessage.Message = "Please enter a value between 0.0 and 50.0!";

                    ChatHandler.SendMessage(session, chatMessage);
                }

                return;
            }

            chatMessage.Message = "Walk speed set to default.";
            ChatHandler.SendMessage(session, chatMessage);
        }

        [ChatCommand("runspeed", "Usage: !runspeed #speed (Set the current run speed)")]
        public static void RunSpeed(string[] args, WorldClass session)
        {
            ChatMessageValues chatMessage = new ChatMessageValues(0, "");

            if (args.Length == 1)
                MoveHandler.HandleMoveSetRunSpeed(session);
            else
            {
                var speed = CommandParser.Read<float>(args, 1);
                if (speed <= 50 && speed > 0)
                {
                    chatMessage.Message = "Run speed set to " + speed + "!";

                    MoveHandler.HandleMoveSetRunSpeed(session, speed);
                    ChatHandler.SendMessage(session, chatMessage);
                }
                else
                {
                    chatMessage.Message = "Please enter a value between 0.0 and 50.0!";

                    ChatHandler.SendMessage(session, chatMessage);
                }

                return;
            }

            chatMessage.Message = "Run speed set to default.";

            ChatHandler.SendMessage(session, chatMessage);
        }

        [ChatCommand("swimspeed", "Usage: !swimspeed #speed (Set the current swim speed)")]
        public static void SwimSpeed(string[] args, WorldClass session)
        {
            ChatMessageValues chatMessage = new ChatMessageValues(0, "");

            if (args.Length == 1)
                MoveHandler.HandleMoveSetSwimSpeed(session);
            else
            {
                var speed = CommandParser.Read<float>(args, 1);
                if (speed <= 50 && speed > 0)
                {
                    chatMessage.Message = "Swim speed set to " + speed + "!";

                    MoveHandler.HandleMoveSetSwimSpeed(session, speed);
                    ChatHandler.SendMessage(session, chatMessage);
                }
                else
                {
                    chatMessage.Message = "Please enter a value between 0.0 and 50.0!";

                    ChatHandler.SendMessage(session, chatMessage);
                }

                return;
            }

            chatMessage.Message = "Swim speed set to default.";

            ChatHandler.SendMessage(session, chatMessage);
        }

        [ChatCommand("flightspeed", "Usage: !flightspeed #speed (Set the current flight speed)")]
        public static void FlightSpeed(string[] args, WorldClass session)
        {
            ChatMessageValues chatMessage = new ChatMessageValues(0, "");

            if (args.Length == 1)
                MoveHandler.HandleMoveSetFlightSpeed(session);
            else
            {
                var speed = CommandParser.Read<float>(args, 1);

                if (speed <= 50 && speed > 0)
                {
                    chatMessage.Message = "Flight speed set to " + speed + "!";

                    MoveHandler.HandleMoveSetFlightSpeed(session, speed);
                    ChatHandler.SendMessage(session, chatMessage);
                }
                else
                {
                    chatMessage.Message = "Please enter a value between 0.0 and 50.0!";

                    ChatHandler.SendMessage(session, chatMessage);
                }

                return;
            }

            chatMessage.Message = "Flight speed set to default.";

            ChatHandler.SendMessage(session, chatMessage);
        }

        [ChatCommand("rotationspeed", "Usage: !rotationspeed #speed (Set the current rotation speed)")]
        public static void RotationSpeed(string[] args, WorldClass session)
        {
            ChatMessageValues chatMessage = new ChatMessageValues(0, "");

            if (args.Length == 1)
                MoveHandler.HandleMoveSetRotationSpeed(session);
            else
            {
                var speed = CommandParser.Read<float>(args, 1);

                if (speed <= 50 && speed > 0)
                {
                    chatMessage.Message = "Rotation speed set to " + speed + "!";

                    MoveHandler.HandleMoveSetRotationSpeed(session, speed);
                    ChatHandler.SendMessage(session, chatMessage);
                }
                else
                {
                    chatMessage.Message = "Please enter a value between 0.0 and 50.0!";

                    ChatHandler.SendMessage(session, chatMessage);
                }

                return;
            }

            chatMessage.Message = "Rotation speed set to default.";

            ChatHandler.SendMessage(session, chatMessage);
        }

        [ChatCommand("tele", "Usage: !tele [#x #y #z #o #map] or [#location] (Force teleport to a new location by coordinates or location)")]
        public static void Teleport(string[] args, WorldClass session)
        {
            var pChar = session.Character;
            Vector4 vector;
            uint mapId;

            if (args.Length > 2)
            {
                vector = new Vector4()
                {
                    X = CommandParser.Read<float>(args, 1),
                    Y = CommandParser.Read<float>(args, 2),
                    Z = CommandParser.Read<float>(args, 3),
                    O = CommandParser.Read<float>(args, 4)
                };

                mapId = CommandParser.Read<uint>(args, 5);
            }
            else
            {
                string location = CommandParser.Read<string>(args, 1);
                SQLResult result = DB.World.Select("SELECT * FROM teleport_locations WHERE location = ?", location);

                if (result.Count == 0)
                {
                    ChatMessageValues chatMessage = new ChatMessageValues(0, "Teleport location '" + location + "' does not exist.");

                    ChatHandler.SendMessage(session, chatMessage);
                    return;
                }

                vector = new Vector4()
                {
                    X = result.Read<float>(0, "X"),
                    Y = result.Read<float>(0, "Y"),
                    Z = result.Read<float>(0, "Z"),
                    O = result.Read<float>(0, "O")
                };

                mapId = result.Read<uint>(0, "Map");
            }

            if (pChar.Map == mapId)
            {
                MoveHandler.HandleMoveTeleport(session, vector);
                ObjectMgr.SetPosition(ref pChar, vector);
            }
            else
            {
                MoveHandler.HandleTransferPending(session, mapId);
                MoveHandler.HandleNewWorld(session, vector, mapId);

                ObjectMgr.SetPosition(ref pChar, vector);
                ObjectMgr.SetMap(ref pChar, mapId);

                ObjectHandler.HandleUpdateObjectCreate(session);
            }
        }

        [ChatCommand("start", "Usage: !start (Teleports yourself to your start position)")]
        public static void Start(string[] args, WorldClass session)
        {
            var pChar = session.Character;

            SQLResult result = DB.Characters.Select("SELECT map, posX, posY, posZ, posO FROM character_creation_data WHERE race = ? AND class = ?", pChar.Race, pChar.Class);

            Vector4 vector = new Vector4()
            {
                X = result.Read<float>(0, "PosX"),
                Y = result.Read<float>(0, "PosY"),
                Z = result.Read<float>(0, "PosZ"),
                O = result.Read<float>(0, "PosO")
            };

            uint mapId = result.Read<uint>(0, "Map");

            if (pChar.Map == mapId)
            {
                MoveHandler.HandleMoveTeleport(session, vector);
                ObjectMgr.SetPosition(ref pChar, vector);
            }
            else
            {
                MoveHandler.HandleTransferPending(session, mapId);
                MoveHandler.HandleNewWorld(session, vector, mapId);

                ObjectMgr.SetPosition(ref pChar, vector);
                ObjectMgr.SetMap(ref pChar, mapId);

                ObjectHandler.HandleUpdateObjectCreate(session);
            }
        }

        [ChatCommand("gps", "Usage: !gps (Show your current location)")]
        public static void GPS(string[] args, WorldClass session)
        {
            var pChar = session.Character;

            var message = string.Format("Your position is X: {0}, Y: {1}, Z: {2}, W(O): {3}, Map: {4}, Zone: {5}", pChar.Position.X, pChar.Position.Y, pChar.Position.Z, pChar.Position.O, pChar.Map, pChar.Zone);
            ChatMessageValues chatMessage = new ChatMessageValues(0, message);

            ChatHandler.SendMessage(session, chatMessage);
        }

        [ChatCommand("addtele", "Usage: !addtele #name (Adds a new teleport location to the world database with the given name)")]
        public static void AddTele(string[] args, WorldClass session)
        {
            var pChar = session.Character;

            string location = CommandParser.Read<string>(args, 1);
            SQLResult result = DB.World.Select("SELECT * FROM teleport_locations WHERE location = ?", location);

            ChatMessageValues chatMessage = new ChatMessageValues(0, "");

            if (result.Count == 0)
            {
                if (DB.World.Execute("INSERT INTO teleport_locations (location, x, y, z, o, map) " +
                    "VALUES (?, ?, ?, ?, ?, ?)", location, pChar.Position.X, pChar.Position.Y, pChar.Position.Z, pChar.Position.O, pChar.Map))
                {
                    chatMessage.Message = string.Format("Teleport location '{0}' successfully added.", location);

                    ChatHandler.SendMessage(session, chatMessage);
                }
            }
            else
            {
                chatMessage.Message = string.Format("Teleport location '{0}' already exist.", location);

                ChatHandler.SendMessage(session, chatMessage);
            }
        }

        [ChatCommand("deltele", "Usage: !deltele #name (Delete the given teleport location from the world database)")]
        public static void DelTele(string[] args, WorldClass session)
        {
            var pChar = session.Character;

            string location = CommandParser.Read<string>(args, 1);

            ChatMessageValues chatMessage = new ChatMessageValues(0, string.Format("Teleport location '{0}' successfully deleted.", location));

            if (DB.World.Execute("DELETE FROM teleport_locations WHERE location = ?", location))
                ChatHandler.SendMessage(session, chatMessage);
        }

        [ChatCommand("telelist", "Usage: !telelist [#string] (List all the teleport locations [containing #string])")]
        public static void TeleList(string[] args, WorldClass session)
        {
            string infoMsg = "No locations found!";
            ChatMessageValues chatMessage;
            SQLResult result;

            // If there is an argument, use it as 'LIKE' parameter into the SQL command
            if (args.Length > 1)
            {
                string query = string.Format("SELECT location FROM teleport_locations WHERE location LIKE '%{0}%' ORDER BY location ASC", CommandParser.Read<string>(args, 1));
                infoMsg = "There aren't locations matching your criteria!";
                result = DB.World.Select(query);
            }
            else
            {
                result = DB.World.Select("SELECT location FROM teleport_locations ORDER BY location ASC");
            }

            if (result.Count == 0)
            {
                chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, infoMsg);
                ChatHandler.SendMessage(session, chatMessage);
            }
            else
            {
                chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, string.Format("Found {0} result(s):", result.Count));
                ChatHandler.SendMessage(session, chatMessage);

                for (int i = 0; i < result.Count; i++)
                {
                    chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, result.Read<string>(i, "location"));
                    ChatHandler.SendMessage(session, chatMessage);
                }
            }
        }

        [ChatCommand("goPOI", "Usage: !goPOI #ID [#z] (teleport to a POI from DBC file [at height #z])")]
        public static void goPOI(string[] args, WorldClass session)
        {
            var pChar = session.Character;
            ChatMessageValues chatMessage;

            int nPOIs = CliDB.AreaPOI.Count;
            if ((args.Length < 2) || (args.Length > 3))
            {
                chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, "wrong number of parameters!");
                ChatHandler.SendMessage(session, chatMessage);
                return;
            }

            uint POI = CommandParser.Read<uint>(args, 1);

            // Z value not stored into AreaPOI.dbc, so I take a fixed value or the one given at command line
            float z = (args.Length == 3) ? CommandParser.Read<float>(args, 2) : 300;

            var area = CliDB.AreaPOI.SingleOrDefault(areapoi => areapoi.AreaID == POI);
            if (area == null)
            {
                chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, "POI not found!!!");
                ChatHandler.SendMessage(session, chatMessage);
                return;
            }

            Vector4 vector = new Vector4()
            {
                X = area.X,
                Y = area.Y,
                Z = z,
                O = 0
            };

            uint mapId = area.mapID;

            if (pChar.Map == mapId)
            {
                MoveHandler.HandleMoveTeleport(session, vector);
                ObjectMgr.SetPosition(ref pChar, vector);
            }
            else
            {
                MoveHandler.HandleTransferPending(session, mapId);
                MoveHandler.HandleNewWorld(session, vector, mapId);

                ObjectMgr.SetPosition(ref pChar, vector);
                ObjectMgr.SetMap(ref pChar, mapId);

                ObjectHandler.HandleUpdateObjectCreate(session);
            }

            chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, string.Format("Teleported to {0}!", area.Name));
            ChatHandler.SendMessage(session, chatMessage);
        }

        [ChatCommand("listPOI", "Usage: !listPOI [#string] (lists all POIs from DBC file [which contains #string])")]
        public static void listPOI(string[] args, WorldClass session)
        {
            var pChar = session.Character;
            ChatMessageValues chatMessage;

            if (args.Length == 1)
            {
                // List all POIs
                foreach (var poi in CliDB.AreaPOI)
                {
                    chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, string.Format("{0} -> {1}", poi.Name, poi.AreaID));
                    ChatHandler.SendMessage(session, chatMessage);
                }
            }
            else if (args.Length == 2)
            {
                // List POIs which contains the given string
                string givenstr = CommandParser.Read<string>(args, 1);
                var areas = CliDB.AreaPOI.Where(areapoi => areapoi.Name.IndexOf(givenstr) > -1).ToList();

                int count = areas.Count;
                if (count == 0)
                {
                    chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, string.Format("No POIs found containing '{0}'!!!", givenstr));
                    ChatHandler.SendMessage(session, chatMessage);
                }
                else
                {
                    chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, string.Format("Found {0} POIs containing '{1}':", count, givenstr));
                    ChatHandler.SendMessage(session, chatMessage);
                    foreach (var poi in areas)
                    {
                        chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, string.Format("{0} -> {1}", poi.Name, poi.AreaID));
                        ChatHandler.SendMessage(session, chatMessage);
                    }
                }
            }
            else
            {
                chatMessage = new ChatMessageValues(0, "Wrong number of parameters");
                ChatHandler.SendMessage(session, chatMessage);
            }
        }

        [ChatCommand("goLoc", "Usage: !goLoc #ID (teleport to a WorldSafeLoc from DBC file)")]
        public static void goLoc(string[] args, WorldClass session)
        {
            var pChar = session.Character;
            ChatMessageValues chatMessage;

            if (args.Length != 2)
            {
                chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, "You must provide an ID to retrieve its location!");
                ChatHandler.SendMessage(session, chatMessage);
                return;
            }

            uint WSL = CommandParser.Read<uint>(args, 1);

            var safeloc = CliDB.WorldSafeLocs.SingleOrDefault(areapoi => areapoi.ID == WSL);

            if (safeloc == null)
            {
                chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, "ID not found!!!");
                ChatHandler.SendMessage(session, chatMessage);
                return;
            }

            Vector4 vector = new Vector4()
            {
                X = safeloc.X,
                Y = safeloc.Y,
                Z = safeloc.Z,
                O = Math.Min(Math.Max(safeloc.O, -6.2831853F), 6.2831853F) // Avoid extrange values on DBCs
            };
            uint mapId = safeloc.mapID;

            if (pChar.Map == mapId)
            {
                MoveHandler.HandleMoveTeleport(session, vector);
                ObjectMgr.SetPosition(ref pChar, vector);
            }
            else
            {
                MoveHandler.HandleTransferPending(session, mapId);
                MoveHandler.HandleNewWorld(session, vector, mapId);

                ObjectMgr.SetPosition(ref pChar, vector);
                ObjectMgr.SetMap(ref pChar, mapId);

                ObjectHandler.HandleUpdateObjectCreate(session);
            }

            chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, string.Format("Teleported to {0}", safeloc.Name));
            ChatHandler.SendMessage(session, chatMessage);
        }

        [ChatCommand("listLocs", "Usage: !listSafeLocs [#string] (lists all WorldSafeLocs from DBC file [which contains #string])")]
        public static void listLocs(string[] args, WorldClass session)
        {
            var pChar = session.Character;
            ChatMessageValues chatMessage;

            int nWSLs = CliDB.WorldSafeLocs.Count;
            if (args.Length == 1)
            {
                // List all WorldSafeLocs
                foreach (var wsl in CliDB.WorldSafeLocs)
                {
                    chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, string.Format("{0} -> {1}", wsl.Name, wsl.ID));
                    ChatHandler.SendMessage(session, chatMessage);
                }
            }
            else if (args.Length == 2)
            {
                // List WSLs which contains the given string
                string givenstr = CommandParser.Read<string>(args, 1);
                var locs = CliDB.WorldSafeLocs.Where(safeloc => safeloc.Name.IndexOf(givenstr) > -1).ToList();

                // If no POIs found, we'll warn about it
                int count = locs.Count;
                if (count == 0)
                {
                    chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, string.Format("No WorldSafeLocs found containing '{0}'!!!", givenstr));
                    ChatHandler.SendMessage(session, chatMessage);
                }
                else
                {
                    chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, string.Format("Found {0} WorldSafeLocs containing '{1}':", count, givenstr));
                    ChatHandler.SendMessage(session, chatMessage);
                    foreach (var wfl in locs)
                    {
                        chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, string.Format("{0} -> {1}", wfl.Name, wfl.ID));
                        ChatHandler.SendMessage(session, chatMessage);
                    }
                }
            }
            else
            {
                chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, "Wrong number of parameters");
                ChatHandler.SendMessage(session, chatMessage);
            }
        }
    }
}
