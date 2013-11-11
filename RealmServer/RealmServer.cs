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
using System.Threading;
using Framework.Configuration;
using Framework.Database;
using Framework.Logging;
using Framework.Network.Realm;
using Framework.ObjectDefines;

namespace RealmServer
{
    class RealmServer
    {
        static void Main()
        {
            Log.ServerType = "Realm";

            Log.Message(LogType.Init, "___________________________________________");
            Log.Message(LogType.Init, "    __                                     ");
            Log.Message(LogType.Init, "    / |                     ,              ");
            Log.Message(LogType.Init, "---/__|---)__----__--_/_--------------_--_-");
            Log.Message(LogType.Init, "  /   |  /   ) /   ' /    /   /   /  / /  )");
            Log.Message(LogType.Init, "_/____|_/_____(___ _(_ __/___(___(__/_/__/_");
            Log.Message(LogType.Init, "___________________________________________");
            Log.Message();

            Log.Message(LogType.Normal, "Starting Arctium RealmServer...");

            DB.Realms.Init(RealmConfig.RealmDBHost, RealmConfig.RealmDBUser, RealmConfig.RealmDBPassword, 
                           RealmConfig.RealmDBDataBase, RealmConfig.RealmDBPort, RealmConfig.MySqlPooling,
                           RealmConfig.MySqlMinPoolSize, RealmConfig.MySqlMaxPoolSize);

            DB.Characters.Init(WorldConfig.CharDBHost, WorldConfig.CharDBUser, WorldConfig.CharDBPassword,
                               WorldConfig.CharDBDataBase, WorldConfig.CharDBPort, RealmConfig.MySqlPooling,
                               RealmConfig.MySqlMinPoolSize, RealmConfig.MySqlMaxPoolSize);

            RealmClass.realm = new RealmNetwork();

            if (RealmClass.realm.Start(RealmConfig.BindIP, (int)RealmConfig.BindPort))
            {
                RealmClass.realm.AcceptConnectionThread();

                Log.Message(LogType.Normal, "RealmServer listening on {0} port {1}.", RealmConfig.BindIP, RealmConfig.BindPort);
                Log.Message(LogType.Normal, "RealmServer successfully started!");

                // Add realms from database.
                new Thread(() =>
                {
                    var start = true;

                    while (true)
                    {
                        Log.Message(LogType.Debug, "Updating Realm List...");

                        using (var result = DB.Realms.Select("SELECT * FROM realms"))
                        {
                            for (int i = 0; i < result.Count; i++)
                            {
                                var Name = result.Read<string>(i, "Name");

                                RealmClass.Realms[Name] = new Realm
                                {
                                    Id   = result.Read<uint>(i, "Id"),
                                    Name = Name,
                                    IP   = result.Read<string>(i, "IP"),
                                    Port = result.Read<uint>(i, "Port"),
                                };

                                if (start)
                                {
                                    Log.Message(LogType.Normal, "Added Realm \"{0}\"", Name);
                                    start = false;
                                }
                            }
                        }

                        Thread.Sleep((int)RealmConfig.RealmListUpdateTime);
                    }
                }).Start();
            }
            else
                Log.Message(LogType.Error, "RealmServer couldn't be started: ");

            Log.Message(LogType.Normal, "Total Memory: {0} Kilobytes", GC.GetTotalMemory(false) / 1024);
        }
    }
}
