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

using System;
using Framework.Constants.GameObject;
using Framework.Database;
using WorldServer.Game.ObjectDefines;

namespace WorldServer.Game.WorldEntities
{
    public class GameObject
    {
        public GameObjectStats Stats;

        public GameObject() { }
        public GameObject(int id)
        {
            SQLResult result = DB.World.Select("SELECT * FROM gameobject_stats WHERE id = ?", id);

            if (result.Count != 0)
            {
                Stats = new GameObjectStats();

                Stats.Id             = result.Read<int>(0, "Id");
                Stats.Type           = result.Read<GameObjectType>(0, "Type");
                Stats.Flags          = result.Read<int>(0, "Flags");

                Stats.DisplayInfoId  = result.Read<int>(0, "DisplayInfoId");
                Stats.Name           = result.Read<string>(0, "Name");
                Stats.IconName       = result.Read<string>(0, "IconName");
                Stats.CastBarCaption = result.Read<string>(0, "CastBarCaption");

                for (int i = 0; i < Stats.Data.Capacity; i++)
                    Stats.Data.Add(result.Read<int>(0, "Data", i));

                Stats.Size = result.Read<float>(0, "Size");

                for (int i = 0; i < Stats.QuestItemId.Capacity; i++)
                {
                    var questItem = result.Read<int>(0, "QuestItemId", i);

                    if (questItem != 0)
                        Stats.QuestItemId.Add(questItem);
                }

                Stats.ExpansionRequired = result.Read<int>(0, "ExpansionRequired");
            }
        }
    }
}
