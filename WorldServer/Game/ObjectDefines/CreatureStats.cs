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

namespace WorldServer.Game.ObjectDefines
{
    public class CreatureStats
    {
        public int Id;
        public string Name;
        public string SubName;
        public string IconName;
        public List<int> Flag = new List<int>(2);
        public int Type;
        public int Family;
        public int Rank;
        public List<int> QuestKillNpcId = new List<int>(2);
        public List<int> DisplayInfoId = new List<int>(4);
        public float HealthModifier;
        public float PowerModifier;
        public byte RacialLeader;
        public List<int> QuestItemId = new List<int>(6);
        public int MovementInfoId;
        public int ExpansionRequired;
    }
}
