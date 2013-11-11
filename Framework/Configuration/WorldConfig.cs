﻿/*
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

using Framework.Logging;

namespace Framework.Configuration
{
    public static class WorldConfig
    {
        static Config config                    = new Config("./Configs/WorldServer.conf");

        public static string CharDBHost         = config.Read("CharDB.Host", "");
        public static int CharDBPort            = config.Read("CharDB.Port", 3306);
        public static string CharDBUser         = config.Read("CharDB.User", "");
        public static string CharDBPassword     = config.Read("CharDB.Password", "");
        public static string CharDBDataBase     = config.Read("CharDB.Database", "");

        public static string WorldDBHost        = config.Read("WorldDB.Host", "");
        public static int WorldDBPort           = config.Read("WorldDB.Port", 3306);
        public static string WorldDBUser        = config.Read("WorldDB.User", "");
        public static string WorldDBPassword    = config.Read("WorldDB.Password", "");
        public static string WorldDBDataBase    = config.Read("WorldDB.Database", "");

        public static uint RealmId              = config.Read<uint>("RealmId", 1);
        
        public static string BindIP             = config.Read("Bind.IP", "0.0.0.0");
        public static uint BindPort             = config.Read<uint>("Bind.Port", 8100);

        public static string DataPath           = config.Read("DataPath", "./Data/");

        public static string GMCommandStart     = config.Read("GM.Command.Start", "!");

        public static bool MySqlPooling         = config.Read("MySql.Pooling", false);
        public static int MySqlMinPoolSize      = config.Read("MySql.MinPoolSize", 5);
        public static int MySqlMaxPoolSize      = config.Read("MySql.MaxPoolSize", 150);

        public static LogType LogLevel          = (LogType)config.Read<uint>("LogLevel", 0, true);
    }
}
