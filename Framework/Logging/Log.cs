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

using Framework.Configuration;
using System;
using DefaultConsole = System.Console;
using Framework.ObjectDefines;
using System.Text;

namespace Framework.Logging
{
    public class Log
    {
        public static string ServerType { get; set; }

        static public void Message()
        {
            SetLogger(LogType.Default, "");
        }

        static public void Message(LogType type, string text, params object[] args)
        {
            SetLogger(type, text, args);
        }

        static void SetLogger(LogType type, string text, params object[] args)
        {
            DefaultConsole.OutputEncoding = UTF8Encoding.UTF8;

            switch (type)
            {
                case LogType.Normal:
                    DefaultConsole.ForegroundColor = ConsoleColor.Green;
                    text = text.Insert(0, "System: ");
                    break;
                case LogType.Error:
                    DefaultConsole.ForegroundColor = ConsoleColor.Red;
                    text = text.Insert(0, "Error: ");
                    break;
                case LogType.Dump:
                    DefaultConsole.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogType.Init:
                    DefaultConsole.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogType.DB:
                    DefaultConsole.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case LogType.Cmd:
                    DefaultConsole.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogType.Debug:
                    DefaultConsole.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                default:
                    DefaultConsole.ForegroundColor = ConsoleColor.White;
                    break;
            }

            if (((Log.ServerType == "World" ? WorldConfig.LogLevel : RealmConfig.LogLevel) & type) == type)
            {
                if (type.Equals(LogType.Init) | type.Equals(LogType.Default))
                    DefaultConsole.WriteLine(text, args);
                else if (type.Equals(LogType.Dump) || type.Equals(LogType.Cmd))
                    DefaultConsole.WriteLine(text, args);
                else
                    DefaultConsole.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] " + text, args);
            }
        }
    }
}
