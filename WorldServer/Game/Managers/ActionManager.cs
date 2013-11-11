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

using System.Collections.Generic;
using System.Linq;
using Framework.Database;
using Framework.Singleton;
using WorldServer.Game.ObjectDefines;
using WorldServer.Game.WorldEntities;

namespace WorldServer.Game.Managers
{
    public class ActionManager : SingletonBase<ActionManager>
    {
        ActionManager() { }

        public void LoadActionButtons(Character pChar)
        {
            SQLResult result = DB.Characters.Select("SELECT * FROM character_actions WHERE guid = ? ORDER BY slotId ASC", pChar.Guid);

            if (result.Count == 0)
            {
                result = DB.Characters.Select("SELECT action, slotId FROM character_creation_actions WHERE race = ? AND class = ? ORDER BY slotId ASC", pChar.Race, pChar.Class);

                for (int i = 0; i < result.Count; i++)
                {
                    var newAction = new ActionButton
                    {
                        Action    = result.Read<uint>(i, "action"),
                        SlotId    = result.Read<byte>(i, "slotId"),
                        SpecGroup = pChar.ActiveSpecGroup
                    };

                    AddActionButton(pChar, newAction, true);
                }
            }
            else
            {
                for (int i = 0; i < result.Count; i++)
                {
                    var newAction = new ActionButton
                    {
                        Action    = result.Read<uint>(i, "action"),
                        SlotId    = result.Read<byte>(i, "slotId"),
                        SpecGroup = result.Read<byte>(i, "specGroup")
                    };

                    AddActionButton(pChar, newAction);
                }
            }
        }

        public List<ActionButton> GetActionButtons(Character pChar, byte specGroup)
        {
            return pChar.ActionButtons.Where(action => action.SpecGroup == specGroup).ToList();
        }

        public void AddActionButton(Character pChar, ActionButton actionButton, bool addToDb = false)
        {
            if (pChar == null || actionButton == null)
                return;

            pChar.ActionButtons.Add(actionButton);

            if (addToDb)
                DB.Characters.Execute("INSERT INTO character_actions (guid, action, slotId, specGroup) VALUES (?, ?, ?, ?)", pChar.Guid, actionButton.Action, actionButton.SlotId, actionButton.SpecGroup);
        }

        public void RemoveActionButton(Character pChar, ActionButton actionButton, bool deleteFromDb = false)
        {
            if (pChar == null || actionButton == null)
                return;

            var deleted = pChar.ActionButtons.Remove(actionButton);

            if (deleted && deleteFromDb)
                DB.Characters.Execute("DELETE FROM character_actions WHERE guid = ? AND action = ? AND slotId = ? AND specGroup = ?", pChar.Guid, actionButton.Action, actionButton.SlotId, pChar.ActiveSpecGroup);
        }
    }
}
