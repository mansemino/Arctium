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
using Framework.Database;

namespace WorldServer.Game.WorldEntities
{
    public class Gossip
    {
        public int Id;
        public int FriendshipFactionID;
        public int TextID;
        public int OptionsCount;
        public int QuestsCount;

        public BroadcastText BroadCastText;

        public Gossip() { }
        public Gossip(int id)
        {
            SQLResult result = DB.World.Select("SELECT * FROM gossip_data WHERE Id = ?", id);

            if (result.Count != 0)
            {
                Id                  = id;
                FriendshipFactionID = result.Read<int>(0, "FriendshipFactionID");
                TextID              = result.Read<int>(0, "TextID");
                OptionsCount        = result.Read<int>(0, "OptionsCount");
                QuestsCount         = result.Read<int>(0, "QuestsCount"); 
            }
        }
    }
}
