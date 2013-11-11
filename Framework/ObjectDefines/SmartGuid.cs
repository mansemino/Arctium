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

using Framework.Constants;
using System;

namespace Framework.ObjectDefines
{
    public class SmartGuid
    {
        public ulong Guid { get; set; }

        public SmartGuid(ulong low, int id, HighGuidType highType)
        {
            Guid = (ulong)(low | ((ulong)id << 32) | (ulong)highType << 52);
        }

        public static HighGuidType GetGuidType(ulong guid)
        {
            return (HighGuidType)(guid >> 52);
        }

        public static int GetId(ulong guid)
        {
            return (int)((guid >> 32) & 0xFFFFF);
        }

        public static ulong GetGuid(ulong guid)
        {
            return guid & 0xFFFFFFFF;
        }
    }
}
