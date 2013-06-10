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
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Database;
using Framework.Logging;
using Framework.ObjectDefines;
using Framework.Singleton;
using WorldServer.Game.Spawns;
using WorldServer.Game.WorldEntities;

namespace WorldServer.Game.Managers
{
    public sealed class SpawnManager : SingletonBase<SpawnManager>
    {
        public ConcurrentDictionary<ulong, CreatureSpawn> CreatureSpawns;
        public ConcurrentDictionary<ulong, GameObjectSpawn> GameObjectSpawns;

        SpawnManager()
        {
            CreatureSpawns = new ConcurrentDictionary<ulong, CreatureSpawn>();
            GameObjectSpawns = new ConcurrentDictionary<ulong, GameObjectSpawn>();

            Initialize();
        }

        public void Initialize()
        {
            LoadCreatureSpawns();
            LoadGameObjectSpawns();
        }

        public bool AddSpawn(CreatureSpawn spawn)
        {
            return CreatureSpawns.TryAdd(spawn.Guid, spawn);
        }

        public void RemoveSpawn(CreatureSpawn spawn)
        {
            CreatureSpawn removedSpawn;
            CreatureSpawns.TryRemove(spawn.Guid, out removedSpawn);

            DB.World.Execute("DELETE FROM creature_spawns WHERE Guid = ?", SmartGuid.GetGuid(spawn.Guid));
        }

        public CreatureSpawn FindSpawn(ulong guid)
        {
            CreatureSpawn spawn;
            CreatureSpawns.TryGetValue(guid, out spawn);

            return spawn;
        }

        public IEnumerable<CreatureSpawn> GetInRangeCreatures(WorldObject obj)
        {
            foreach (var c in CreatureSpawns)
                if (!obj.ToCharacter().InRangeObjects.ContainsKey(c.Key))
                    if (obj.CheckDistance(c.Value))
                        yield return c.Value;
        }

        public IEnumerable<GameObjectSpawn> GetInRangeGameObjects(WorldObject obj)
        {
            foreach (var g in GameObjectSpawns)
                if (!obj.ToCharacter().InRangeObjects.ContainsKey(g.Key))
                    if (obj.CheckDistance(g.Value))
                        yield return g.Value;
        }

        public IEnumerable<CreatureSpawn> GetOutOfRangeCreatures(WorldObject obj)
        {
            foreach (var c in CreatureSpawns)
                if (obj.ToCharacter().InRangeObjects.ContainsKey(c.Key))
                    if (!obj.CheckDistance(c.Value))
                        yield return c.Value;
        }

        public IEnumerable<GameObjectSpawn> GetOutOfRangeGameObjects(WorldObject obj)
        {
            foreach (var g in GameObjectSpawns)
                if (obj.ToCharacter().InRangeObjects.ContainsKey(g.Key))
                    if (!obj.CheckDistance(g.Value))
                        yield return g.Value;
        }

        public void LoadCreatureSpawns()
        {
            Log.Message(LogType.DB, "Loading creature spawns...");

            SQLResult result = DB.World.Select("SELECT * FROM creature_spawns");

            Parallel.For(0, result.Count, (i, loop) =>
            {
                var guid = result.Read<ulong>(i, "Guid");
                var id   = result.Read<int>(i, "Id");

                Creature data = Globals.DataMgr.FindCreature(id);
                if (data == null)
                {
                    Log.Message(LogType.Error, "Loading a creature spawn (Guid: {0}) with non-existing stats (Id: {1}) skipped.", guid, id);
                    return;
                }

                CreatureSpawn spawn = new CreatureSpawn
                {
                    Guid = guid,
                    Id   = id,    
                    Map  = result.Read<uint>(i, "Map"),

                    Position = new Vector4()
                    {
                        X = result.Read<float>(i, "X"),
                        Y = result.Read<float>(i, "Y"),
                        Z = result.Read<float>(i, "Z"),
                        O = result.Read<float>(i, "O")
                    }
                };

                spawn.CreateFullGuid();
                spawn.CreateData(data);

                AddSpawn(spawn);
            });
            
            Log.Message(LogType.DB, "Loaded {0} creature spawns.", CreatureSpawns.Count);
            Log.Message();
        }

        public bool AddSpawn(GameObjectSpawn spawn, ref GameObject data)
        {
            return GameObjectSpawns.TryAdd(spawn.Guid, spawn);
        }

        public void RemoveSpawn(GameObjectSpawn spawn)
        {
            GameObjectSpawn removedGameObject;
            GameObjectSpawns.TryRemove(spawn.Guid, out removedGameObject);

            DB.World.Execute("DELETE FROM creature_spawns WHERE Guid = ?", SmartGuid.GetGuid(spawn.Guid));
        }

        public void LoadGameObjectSpawns()
        {
            Log.Message(LogType.DB, "Loading gameobject spawns...");

            SQLResult result = DB.World.Select("SELECT * FROM gameobject_spawns");

            Parallel.For(0, result.Count, (i, loop) =>
            {
                var guid = result.Read<ulong>(i, "Guid");
                var id = result.Read<int>(i, "Id");

                GameObject data = Globals.DataMgr.FindGameObject(id);
                if (data == null)
                {
                    Log.Message(LogType.Error, "Loading a gameobject spawn (Guid: {0}) with non-existing stats (Id: {1}) skipped.", guid, id);
                    return;
                }

                GameObjectSpawn spawn = new GameObjectSpawn()
                {
                    Guid = guid,
                    Id   = id,
                    Map  = result.Read<uint>(i, "Map"),

                    Position = new Vector4()
                    {
                        X = result.Read<float>(i, "X"),
                        Y = result.Read<float>(i, "Y"),
                        Z = result.Read<float>(i, "Z"),
                        O = result.Read<float>(i, "O")
                    },

                    FactionTemplate = result.Read<uint>(i, "FactionTemplate"),
                    AnimProgress    = result.Read<byte>(i, "AnimProgress"),
                    State           = result.Read<byte>(i, "State"),
                };

                spawn.CreateFullGuid();
                spawn.CreateData(data);

                AddSpawn(spawn, ref data);
            });

            Log.Message(LogType.DB, "Loaded {0} gameobject spawns.", GameObjectSpawns.Count);
            Log.Message();
        }
    }
}
