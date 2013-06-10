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
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Framework.Constants.GameObject;
using Framework.Database;
using Framework.Logging;
using Framework.Singleton;
using WorldServer.Game.ObjectDefines;
using WorldServer.Game.WorldEntities;

namespace WorldServer.Game.Managers
{
    public class DataManager : SingletonBase<DataManager>
    {
        ConcurrentDictionary<int, Creature> Creatures;
        ConcurrentDictionary<int, GameObject> GameObjects;

        DataManager()
        {
            Creatures = new ConcurrentDictionary<int, Creature>();
            GameObjects = new ConcurrentDictionary<int, GameObject>();

            Initialize();
        }

        public bool Add(Creature creature)
        {
            return Creatures.TryAdd(creature.Stats.Id, creature);
        }

        public Creature Remove(Creature creature)
        {
            Creature removedCreature;
            Creatures.TryRemove(creature.Stats.Id, out removedCreature);

            return removedCreature;
        }

        public ConcurrentDictionary<int, Creature> GetCreatures()
        {
            return Creatures;
        }

        public Creature FindCreature(int id)
        {
            Creature creature;
            Creatures.TryGetValue(id, out creature);

            return creature;
        }

        public void LoadCreatureData()
        {
            Log.Message(LogType.DB, "Loading creatures...");

            SQLResult result = DB.World.Select("SELECT cs.Id FROM creature_stats cs LEFT JOIN creature_data cd ON cs.Id = cd.Id WHERE cd.Id IS NULL");

            if (result.Count != 0)
            {
                var missingIds = result.ReadAllValuesFromField("Id");
                DB.World.ExecuteBigQuery("creature_data", "Id", 1, result.Count, missingIds);

                Log.Message(LogType.DB, "Added {0} default data definition for creatures.", missingIds.Length);
            }

            result = DB.World.Select("SELECT * FROM creature_stats cs RIGHT JOIN creature_data cd ON cs.Id = cd.Id WHERE cs.Id IS NOT NULL");

            Parallel.For(0, result.Count, r =>
            {
                CreatureStats Stats = new CreatureStats
                {
                    Id                = result.Read<int>(r, "Id"),
                    Name              = result.Read<string>(r, "Name"),
                    SubName           = result.Read<string>(r, "SubName"),
                    IconName          = result.Read<string>(r, "IconName"),
                    Type              = result.Read<int>(r, "Type"),
                    Family            = result.Read<int>(r, "Family"),
                    Rank              = result.Read<int>(r, "Rank"),
                    HealthModifier    = result.Read<float>(r, "HealthModifier"),
                    PowerModifier     = result.Read<float>(r, "PowerModifier"),
                    RacialLeader      = result.Read<byte>(r, "RacialLeader"),
                    MovementInfoId    = result.Read<int>(r, "MovementInfoId"),
                    ExpansionRequired = result.Read<int>(r, "ExpansionRequired")
                };

                for (int i = 0; i < Stats.Flag.Capacity; i++)
                    Stats.Flag.Add(result.Read<int>(r, "Flag", i));

                for (int i = 0; i < Stats.QuestKillNpcId.Capacity; i++)
                    Stats.QuestKillNpcId.Add(result.Read<int>(r, "QuestKillNpcId", i));

                for (int i = 0; i < Stats.DisplayInfoId.Capacity; i++)
                    Stats.DisplayInfoId.Add(result.Read<int>(r, "DisplayInfoId", i));

                for (int i = 0; i < Stats.QuestItemId.Capacity; i++)
                {
                    var questItem = result.Read<int>(r, "QuestItemId", i);

                    if (questItem != 0)
                        Stats.QuestItemId.Add(questItem);
                }

                Add(new Creature
                {
                    Data = new CreatureData
                    {
                        Health     = result.Read<int>(r, "Health"),
                        Level      = result.Read<byte>(r, "Level"),
                        Class      = result.Read<byte>(r, "Class"),
                        Faction    = result.Read<int>(r, "Faction"),
                        Scale      = result.Read<int>(r, "Scale"),
                        UnitFlags  = result.Read<int>(r, "UnitFlags"),
                        UnitFlags2 = result.Read<int>(r, "UnitFlags2"),
                        NpcFlags   = result.Read<int>(r, "NpcFlags")
                    },

                    Stats = Stats,
                });
            });

            Log.Message(LogType.DB, "Loaded {0} creatures.", Creatures.Count);
            Log.Message();
        }

        public bool Add(GameObject gameobject)
        {
            return GameObjects.TryAdd(gameobject.Stats.Id, gameobject);
        }

        public GameObject Remove(GameObject gameobject)
        {
            GameObject removedGameObject;
            GameObjects.TryRemove(gameobject.Stats.Id, out removedGameObject);

            return removedGameObject;
        }

        public ConcurrentDictionary<int, GameObject> GetGameObjects()
        {
            return GameObjects;
        }

        public GameObject FindGameObject(int id)
        {
            GameObject gameObject;
            GameObjects.TryGetValue(id, out gameObject);

            return gameObject;
        }

        public void LoadGameObject()
        {
            Log.Message(LogType.DB, "Loading gameobjects...");

            SQLResult result = DB.World.Select("SELECT * FROM gameobject_stats");

            Parallel.For(0, result.Count, r =>
            {
                GameObjectStats Stats = new GameObjectStats
                {
                    Id                = result.Read<int>(r, "Id"),
                    Type              = result.Read<GameObjectType>(r, "Type"),
                    DisplayInfoId     = result.Read<int>(r, "DisplayInfoId"),
                    Name              = result.Read<string>(r, "Name"),
                    IconName          = result.Read<string>(r, "IconName"),
                    CastBarCaption    = result.Read<string>(r, "CastBarCaption"),
                    Size              = result.Read<float>(r, "Size"),
                    ExpansionRequired = result.Read<int>(r, "ExpansionRequired")
                };

                for (int i = 0; i < Stats.Data.Capacity; i++)
                    Stats.Data.Add(result.Read<int>(r, "Data", i));

                for (int i = 0; i < Stats.QuestItemId.Capacity; i++)
                {
                    var questItem = result.Read<int>(r, "QuestItemId", i);

                    if (questItem != 0)
                        Stats.QuestItemId.Add(questItem);
                }

                Add(new GameObject
                {
                    Stats = Stats
                });
            });

            Log.Message(LogType.DB, "Loaded {0} gameobjects.", GameObjects.Count);
            Log.Message();
        }

        public void Initialize()
        {
            LoadCreatureData();
            LoadGameObject();
        }
    }
}
