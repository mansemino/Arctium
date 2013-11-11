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

using Framework.Constants.NetMessage;
using System;

namespace Framework.Network.Packets
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class OpcodeAttribute : Attribute
    {
        public ClientMessage Opcode { get; set; }
        public string WoWBuild { get; set; }

        public OpcodeAttribute(ClientMessage opcode, string wowBuild)
        {
            Opcode = opcode;
            WoWBuild = wowBuild;
        }
    }
}
