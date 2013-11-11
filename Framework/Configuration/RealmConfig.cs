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
    public static class RealmConfig
    {
        static Config config = new Config("./Configs/RealmServer.conf");

        public static string RealmDBHost = config.Read("RealmDB.Host", "");
        public static int RealmDBPort = config.Read("RealmDB.Port", 3306);
        public static string RealmDBUser = config.Read("RealmDB.User", "");
        public static string RealmDBPassword = config.Read("RealmDB.Password", "");
        public static string RealmDBDataBase = config.Read("RealmDB.Database", "");

        public static string BindIP = config.Read("Bind.IP", "0.0.0.0");
        public static uint BindPort = config.Read<uint>("Bind.Port", 3724);

        public static bool MySqlPooling = config.Read("MySql.Pooling", false);
        public static int MySqlMinPoolSize = config.Read("MySql.MinPoolSize", 1);
        public static int MySqlMaxPoolSize = config.Read("MySql.MaxPoolSize", 30);

        public static float RealmListUpdateTime = config.Read<float>("RealmList.UpdateTime", 5) * 60000;

        public static LogType LogLevel = (LogType)config.Read<uint>("LogLevel", 0, true);
    }
}
