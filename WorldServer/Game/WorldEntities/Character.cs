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
using Framework.Configuration;
using Framework.Constants;
using Framework.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using WorldServer.Game.ObjectDefines;
using Talent = WorldServer.Game.ObjectDefines.Talent;

using WorldServer.Game.Packets.PacketHandler;

namespace WorldServer.Game.WorldEntities
{
    public class Character : WorldObject
    {
        public UInt32 AccountId;
        public String Name;
        public Byte Race;
        public Byte Class;
        public Byte Gender;
        public Byte Skin;
        public Byte Face;
        public Byte HairStyle;
        public Byte HairColor;
        public Byte FacialHair;
        public Byte Level;
        public UInt32 Zone;
        public UInt64 GuildGuid;
        public UInt32 PetDisplayInfo;
        public UInt32 PetLevel;
        public UInt32 PetFamily;
        public UInt32 CharacterFlags;
        public UInt32 CustomizeFlags;
        public Boolean LoginCinematic;
        public Byte SpecGroupCount;
        public Byte ActiveSpecGroup;
        public UInt32 PrimarySpec;
        public UInt32 SecondarySpec;
        public Boolean UnitIsAfk;
        public String UnitIsAfkMessage;
        public Boolean UnitIsDnd;
        public String UnitIsDndMessage;

        public Dictionary<ulong, WorldObject> InRangeObjects = new Dictionary<ulong, WorldObject>();

        public List<ActionButton> ActionButtons = new List<ActionButton>();
        public List<Skill> Skills = new List<Skill>();
        public List<PlayerSpell> SpellList = new List<PlayerSpell>();
        public List<Talent> TalentList = new List<Talent>();

        public Character(UInt64 guid, int updateLength = (int)PlayerFields.End) : base(updateLength)
        {
            SQLResult result = DB.Characters.Select("SELECT * FROM characters WHERE guid = ?", guid);

            Guid            = result.Read<UInt64>(0, "Guid");
            AccountId       = result.Read<UInt32>(0, "AccountId");
            Name            = result.Read<String>(0, "Name");
            Race            = result.Read<Byte>(0, "Race");
            Class           = result.Read<Byte>(0, "Class");
            Gender          = result.Read<Byte>(0, "Gender");
            Skin            = result.Read<Byte>(0, "Skin");
            Face            = result.Read<Byte>(0, "Face");
            HairStyle       = result.Read<Byte>(0, "HairStyle");
            HairColor       = result.Read<Byte>(0, "HairColor");
            FacialHair      = result.Read<Byte>(0, "FacialHair");
            Level           = result.Read<Byte>(0, "Level");
            Zone            = result.Read<UInt32>(0, "Zone");
            Map             = result.Read<UInt32>(0, "Map");
            Position.X      = result.Read<Single>(0, "X");
            Position.Y      = result.Read<Single>(0, "Y");
            Position.Z      = result.Read<Single>(0, "Z");
            Position.O      = result.Read<Single>(0, "O");
            GuildGuid       = result.Read<UInt64>(0, "GuildGuid");
            PetDisplayInfo  = result.Read<UInt32>(0, "PetDisplayId");
            PetLevel        = result.Read<UInt32>(0, "PetLevel");
            PetFamily       = result.Read<UInt32>(0, "PetFamily");
            CharacterFlags  = result.Read<UInt32>(0, "CharacterFlags");
            CustomizeFlags  = result.Read<UInt32>(0, "CustomizeFlags");
            LoginCinematic  = result.Read<Boolean>(0, "LoginCinematic");
            SpecGroupCount  = result.Read<Byte>(0, "SpecGroupCount");
            ActiveSpecGroup = result.Read<Byte>(0, "ActiveSpecGroup");
            PrimarySpec     = result.Read<UInt32>(0, "PrimarySpecId");
            SecondarySpec   = result.Read<UInt32>(0, "SecondarySpecId");

            UnitIsAfk        = false;
            UnitIsAfkMessage = "";
            UnitIsDnd        = false;
            UnitIsDndMessage = "";

            Globals.SpecializationMgr.LoadTalents(this);
            Globals.SpellMgr.LoadSpells(this);
            Globals.SkillMgr.LoadSkills(this);
            Globals.ActionMgr.LoadActionButtons(this);
        }

        public override void SetUpdateFields()
        {
            // ObjectFields
            SetUpdateField<UInt64>((int)ObjectFields.Guid, Guid);
            SetUpdateField<UInt64>((int)ObjectFields.Data, 0);
            SetUpdateField<Int32>((int)ObjectFields.Type, 0x19);
            SetUpdateField<Int32>((int)ObjectFields.DynamicFlags, 0);
            SetUpdateField<Single>((int)ObjectFields.Scale, 1.0f);

            SetUpdateField<Int32>((int)UnitFields.Health, 123);
            SetUpdateField<Int32>((int)UnitFields.MaxHealth, 123);

            SetUpdateField<Int32>((int)UnitFields.Level, Level);
            SetUpdateField<UInt32>((int)UnitFields.FactionTemplate, CliDB.ChrRaces.Single(r => r.Id == Race).Faction);

            SetUpdateField<Byte>((int)UnitFields.DisplayPower, Race, 0);
            SetUpdateField<Byte>((int)UnitFields.DisplayPower, Class, 1);
            SetUpdateField<Byte>((int)UnitFields.DisplayPower, Gender, 2);
            SetUpdateField<Byte>((int)UnitFields.DisplayPower, 0, 3);

            var race = CliDB.ChrRaces.Single(r => r.Id == Race);
            var displayId = Gender == 0 ? race.MaleDisplayId : race.FemaleDisplayId;

            SetUpdateField<UInt32>((int)UnitFields.DisplayID, displayId);
            SetUpdateField<UInt32>((int)UnitFields.NativeDisplayID, displayId);

            SetUpdateField<UInt32>((int)UnitFields.Flags, 0x8);

            SetUpdateField<Single>((int)UnitFields.BoundingRadius, 0.389F);
            SetUpdateField<Single>((int)UnitFields.CombatReach, 1.5F);
            SetUpdateField<Single>((int)UnitFields.ModCastingSpeed, 1);
            SetUpdateField<Single>((int)UnitFields.MaxHealthModifier, 1);
            
            // PlayerFields
            SetUpdateField<Byte>((int)PlayerFields.HairColorID, Skin, 0);
            SetUpdateField<Byte>((int)PlayerFields.HairColorID, Face, 1);
            SetUpdateField<Byte>((int)PlayerFields.HairColorID, HairStyle, 2);
            SetUpdateField<Byte>((int)PlayerFields.HairColorID, HairColor, 3);
            SetUpdateField<Byte>((int)PlayerFields.RestState, FacialHair, 0);
            SetUpdateField<Byte>((int)PlayerFields.RestState, 0, 1);
            SetUpdateField<Byte>((int)PlayerFields.RestState, 0, 2);
            SetUpdateField<Byte>((int)PlayerFields.RestState, 2, 3);
            SetUpdateField<Byte>((int)PlayerFields.ArenaFaction, Gender, 0);
            SetUpdateField<Byte>((int)PlayerFields.ArenaFaction, 0, 1);
            SetUpdateField<Byte>((int)PlayerFields.ArenaFaction, 0, 2);
            SetUpdateField<Byte>((int)PlayerFields.ArenaFaction, 0, 3);
            SetUpdateField<Int32>((int)PlayerFields.WatchedFactionIndex, -1);
            SetUpdateField<Int32>((int)PlayerFields.XP, 0);
            SetUpdateField<Int32>((int)PlayerFields.NextLevelXP, 400);

            SetUpdateField<Int32>((int)PlayerFields.CurrentSpecID, (int)GetActiveSpecId());

            SetUpdateField<Int32>((int)PlayerFields.SpellCritPercentage + 0, SpecializationMgr.GetUnspentTalentRowCount(this), 0);
            SetUpdateField<Int32>((int)PlayerFields.SpellCritPercentage + 1, SpecializationMgr.GetMaxTalentRowCount(this), 0);

            for (int i = 0; i < 448; i++)
                if (i < Skills.Count)
                    SetUpdateField<UInt32>((int)PlayerFields.Skill + i, Skills[i].Id);

            SetUpdateField<UInt32>((int)PlayerFields.VirtualPlayerRealm, WorldConfig.RealmId);
        }

        public static string NormalizeName(string name)
        {
            return name[0].ToString().ToUpper() + name.Remove(0, 1).ToLower();
        }

        public uint GetActiveSpecId()
        {
            if ((ActiveSpecGroup == 0 && PrimarySpec == 0) || (ActiveSpecGroup == 1 && SecondarySpec == 0))
                return 0;

            return (ActiveSpecGroup == 0 && PrimarySpec != 0) ? PrimarySpec : SecondarySpec;
        }

        public void setAfkState(string afkText = "")
        {
            bool afkState = (afkText.Length > 0);

            if (this.UnitIsAfk != afkState)
            {
                this.UnitIsAfk = afkState;

                if (this.UnitIsAfk && this.UnitIsDnd)
                {
                    this.UnitIsDnd = false;
                    this.UnitIsDndMessage = "";
                }

                this.UnitIsAfkMessage = afkText;

                SetUpdateField<Int32>((int)PlayerFields.PlayerFlags, (int)((this.UnitIsAfk) ? PlayerFlag.Afk : PlayerFlag.None));

                var session = WorldMgr.GetSession(this.Guid);
                ObjectHandler.HandleUpdateObjectValues(ref session, true);
            }
        }

        public void setDndState(string dndText = "")
        {
            bool dndState = (dndText.Length > 0);

            if (this.UnitIsDnd != dndState)
            {
                this.UnitIsDnd = dndState;

                if (this.UnitIsAfk && this.UnitIsDnd)
                {
                    this.UnitIsAfk = false;
                    this.UnitIsAfkMessage = "";
                }

                this.UnitIsDndMessage = dndText;

                SetUpdateField<Int32>((int)PlayerFields.PlayerFlags, (int)((this.UnitIsDnd) ? PlayerFlag.Dnd : PlayerFlag.None));

                var session = WorldMgr.GetSession(this.Guid);
                
                ObjectHandler.HandleUpdateObjectValues(ref session, true);
            }
        }

    }
}
