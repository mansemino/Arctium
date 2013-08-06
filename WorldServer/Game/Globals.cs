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

using Framework.ClientDB;
using WorldServer.Game.Managers;

namespace WorldServer.Game
{
    public class Globals
    {
        public static ActionManager ActionMgr;
        public static AddonManager AddonMgr;
        public static DataManager DataMgr;
        public static GossipManager GossipMgr;
        public static ObjectManager ObjectMgr;
        public static SkillManager SkillMgr;
        public static SpawnManager SpawnMgr;
        public static SpecializationManager SpecializationMgr;
        public static SpellManager SpellMgr;
        public static WorldManager WorldMgr;
        // public static LogoutManager LogOutMgr;

        public static void Initialize()
        {
            // Load dbc files.
            CliDB.Initialize();

            ActionMgr         = ActionManager.GetInstance();
            AddonMgr          = AddonManager.GetInstance();
            SkillMgr          = SkillManager.GetInstance();
            SpellMgr          = SpellManager.GetInstance();
            SpecializationMgr = SpecializationManager.GetInstance();
            DataMgr           = DataManager.GetInstance();
            SpawnMgr          = SpawnManager.GetInstance();
            GossipMgr         = GossipManager.GetInstance();
            ObjectMgr         = ObjectManager.GetInstance();
            WorldMgr          = WorldManager.GetInstance();
            // LogOutMgr         = LogoutManager.GetInstance();
        }
    }
}
