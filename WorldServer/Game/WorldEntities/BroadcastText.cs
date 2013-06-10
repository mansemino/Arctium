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
using Framework.Database;

namespace WorldServer.Game.WorldEntities
{
    public class BroadcastText
    {
        public int Id;
        public int Language;
        public string Text;
        public string AlternativeText;
        public List<int> Emotes = new List<int>(8);

        public BroadcastText() { }
        public BroadcastText(int id)
        {
            SQLResult result = DB.World.Select("SELECT * FROM broadcast_texts WHERE id = ?", id);

            if (result.Count != 0)
            {
                Id              = result.Read<int>(0, "Id");
                Language        = result.Read<int>(0, "Language");
                Text            = result.Read<string>(0, "Text");
                AlternativeText = result.Read<string>(0, "AlternativeText");

                for (int i = 0; i < Emotes.Capacity; i++)
                    Emotes.Add(result.Read<int>(0, "Emote" + i));
            }
        }
    }
}
