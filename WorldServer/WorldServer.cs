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
using Framework.Configuration;
using Framework.Console;
using Framework.Console.Commands;
using Framework.Database;
using Framework.Logging;
using WorldServer.Game;
using WorldServer.Game.Chat;
using WorldServer.Game.Packets;
using WorldServer.Network;

namespace WorldServer
{
    class WorldServer
    {
        [MTAThreadAttribute]
        static void Main()
        {
            Log.ServerType = "World";

            Log.Message(LogType.Init, "___________________________________________");
            Log.Message(LogType.Init, "    __                                     ");
            Log.Message(LogType.Init, "    / |                     ,              ");
            Log.Message(LogType.Init, "---/__|---)__----__--_/_--------------_--_-");
            Log.Message(LogType.Init, "  /   |  /   ) /   ' /    /   /   /  / /  )");
            Log.Message(LogType.Init, "_/____|_/_____(___ _(_ __/___(___(__/_/__/_");
            Log.Message(LogType.Init, "___________________________________________");
            Log.Message();

            Log.Message(LogType.Normal, "Starting Arctium WorldServer...");

            DB.Characters.Init(WorldConfig.CharDBHost, WorldConfig.CharDBUser, WorldConfig.CharDBPassword,
                               WorldConfig.CharDBDataBase, WorldConfig.CharDBPort, WorldConfig.MySqlPooling,
                               WorldConfig.MySqlMinPoolSize, WorldConfig.MySqlMaxPoolSize);

            DB.Realms.Init(RealmConfig.RealmDBHost, RealmConfig.RealmDBUser, RealmConfig.RealmDBPassword,
                           RealmConfig.RealmDBDataBase, RealmConfig.RealmDBPort, WorldConfig.MySqlPooling,
                           WorldConfig.MySqlMinPoolSize, WorldConfig.MySqlMaxPoolSize);

            DB.World.Init(WorldConfig.WorldDBHost, WorldConfig.WorldDBUser, WorldConfig.WorldDBPassword,
                          WorldConfig.WorldDBDataBase, WorldConfig.WorldDBPort, WorldConfig.MySqlPooling,
                          WorldConfig.MySqlMinPoolSize, WorldConfig.MySqlMaxPoolSize);

            Log.Message();

            Globals.Initialize();

            // Set all accounts offline
            DB.Realms.Execute("UPDATE accounts SET online = 0");

            WorldClass.world = new WorldNetwork();

            if (WorldClass.world.Start(WorldConfig.BindIP, (int)WorldConfig.BindPort))
            {
                WorldClass.world.AcceptConnectionThread();
                Log.Message(LogType.Normal, "WorldServer listening on {0} port {1}.", WorldConfig.BindIP, WorldConfig.BindPort);
                Log.Message(LogType.Normal, "WorldServer successfully started!");

                PacketManager.DefineOpcodeHandler();
                ChatCommandParser.DefineChatCommands();
            }
            else
            {
                Log.Message(LogType.Error, "Server couldn't be started: ");
            }

            GC.Collect();
            Log.Message(LogType.Normal, "Total Memory: {0} Kilobytes", GC.GetTotalMemory(false) / 1024);

            CommandManager.InitCommands();
        }
    }
}
