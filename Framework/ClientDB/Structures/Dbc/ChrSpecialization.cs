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
    public class ChrSpecialization
    {
        public uint Id;
        public Unused Icon;
        public uint ClassId;
        public uint MasterySpellId;
        public uint Unknown;
        public uint TabId;
        public uint Unknown2;
        public uint RoleTypeId;
        public uint Unknown3;
        public uint Unknown4;
        public uint Unknown5;
        public Unused Name;
        public Unused Description;
        public uint Unknown6;
    }
}
