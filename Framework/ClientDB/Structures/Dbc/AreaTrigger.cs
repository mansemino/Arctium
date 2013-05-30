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

using Framework.ClientDB.CustomTypes;

namespace Framework.ClientDB.Structures.Dbc
{
    public class AreaTrigger
    {
        public uint Id;
        public uint MapId;
        public float PositionX;
        public float PositionY;
        public float PositionZ;
        public uint Flags;
        public Unused Unknown1;
        public Unused Unknown2;
        public float Radius;
        public float BoxPositionX;
        public float BoxPositionY;
        public float BoxPositionZ;
        public float BoxOrientation;
        public Unused Unknown3;
        public Unused Unknown4;
        public Unused Unknown5;
    }
}
