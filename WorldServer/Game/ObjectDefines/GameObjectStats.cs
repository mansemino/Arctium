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
using System.Collections.Generic;
using Framework.Constants.GameObject;

namespace WorldServer.Game.ObjectDefines
{
    public class GameObjectStats
    {
        public int Id;
        public GameObjectType Type;
        public int Flags;
        public int DisplayInfoId;
        public string Name;
        public string IconName;
        public string CastBarCaption;
        public List<int> Data = new List<int>(32);
        public float Size;
        public List<int> QuestItemId = new List<int>(6);
        public int ExpansionRequired;
    }
}
