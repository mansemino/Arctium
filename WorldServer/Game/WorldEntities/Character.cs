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
using System.Linq;
using Framework.ClientDB;
using Framework.Configuration;
using Framework.Constants;
using Framework.Database;
using WorldServer.Game.ObjectDefines;
using Talent = WorldServer.Game.ObjectDefines.Talent;

namespace WorldServer.Game.WorldEntities
{
    public class Character : WorldObject
    {
        public uint AccountId;
        public string Name;
        public byte Race;
        public byte Class;
        public byte Gender;
        public byte Skin;
        public byte Face;
        public byte HairStyle;
        public byte HairColor;
        public byte FacialHair;
        public byte Level;
        public uint Zone;
        public ulong GuildGuid;
        public uint PetDisplayInfo;
        public uint PetLevel;
        public uint PetFamily;
        public uint CharacterFlags;
        public uint CustomizeFlags;
        public bool LoginCinematic;
        public byte SpecGroupCount;
        public byte ActiveSpecGroup;
        public uint PrimarySpec;
        public uint SecondarySpec;

        public Dictionary<ulong, WorldObject> InRangeObjects = new Dictionary<ulong, WorldObject>();

        public List<ActionButton> ActionButtons = new List<ActionButton>();
        public List<Skill> Skills = new List<Skill>();
        public List<PlayerSpell> SpellList = new List<PlayerSpell>();
        public List<Talent> TalentList = new List<Talent>();

        public Character(ulong guid, int updateLength = (int)PlayerFields.End) : base(updateLength)
        {
            SQLResult result = DB.Characters.Select("SELECT * FROM characters WHERE guid = ?", guid);

            Guid            = result.Read<ulong>(0, "Guid");
            AccountId       = result.Read<uint>(0, "AccountId");
            Name            = result.Read<string>(0, "Name");
            Race            = result.Read<byte>(0, "Race");
            Class           = result.Read<byte>(0, "Class");
            Gender          = result.Read<byte>(0, "Gender");
            Skin            = result.Read<byte>(0, "Skin");
            Face            = result.Read<byte>(0, "Face");
            HairStyle       = result.Read<byte>(0, "HairStyle");
            HairColor       = result.Read<byte>(0, "HairColor");
            FacialHair      = result.Read<byte>(0, "FacialHair");
            Level           = result.Read<byte>(0, "Level");
            Zone            = result.Read<uint>(0, "Zone");
            Map             = result.Read<uint>(0, "Map");
            Position.X      = result.Read<float>(0, "X");
            Position.Y      = result.Read<float>(0, "Y");
            Position.Z      = result.Read<float>(0, "Z");
            Position.O      = result.Read<float>(0, "O");
            GuildGuid       = result.Read<ulong>(0, "GuildGuid");
            PetDisplayInfo  = result.Read<uint>(0, "PetDisplayId");
            PetLevel        = result.Read<uint>(0, "PetLevel");
            PetFamily       = result.Read<uint>(0, "PetFamily");
            CharacterFlags  = result.Read<uint>(0, "CharacterFlags");
            CustomizeFlags  = result.Read<uint>(0, "CustomizeFlags");
            LoginCinematic  = result.Read<bool>(0, "LoginCinematic");
            SpecGroupCount  = result.Read<byte>(0, "SpecGroupCount");
            ActiveSpecGroup = result.Read<byte>(0, "ActiveSpecGroup");
            PrimarySpec     = result.Read<uint>(0, "PrimarySpecId");
            SecondarySpec   = result.Read<uint>(0, "SecondarySpecId");

            Globals.SpecializationMgr.LoadTalents(this);
            Globals.SpellMgr.LoadSpells(this);
            Globals.SkillMgr.LoadSkills(this);
            Globals.ActionMgr.LoadActionButtons(this);
        }

        public override void SetUpdateFields()
        {
            SetUpdateField<ulong>((int)ObjectFields.Guid, Guid);
            SetUpdateField<int>((int)ObjectFields.Type, 0x19);
            SetUpdateField<float>((int)ObjectFields.Scale, 1.0f);

            SetUpdateField<int>((int)UnitFields.Health, 123);
            SetUpdateField<int>((int)UnitFields.MaxHealth, 123);

            SetUpdateField<int>((int)UnitFields.Level, Level);
            SetUpdateField<uint>((int)UnitFields.FactionTemplate, CliDB.ChrRaces.Single(r => r.Id == Race).Faction);

            SetUpdateField<byte>((int)UnitFields.Sex, Race, 0);
            SetUpdateField<byte>((int)UnitFields.Sex, Class, 1);
            SetUpdateField<byte>((int)UnitFields.Sex, 0, 2);
            SetUpdateField<byte>((int)UnitFields.Sex, Gender, 3);

            SetUpdateField<int>((int)UnitFields.DisplayPower, 1);

            var race = CliDB.ChrRaces.Single(r => r.Id == Race);
            var displayId = Gender == 0 ? race.MaleDisplayId : race.FemaleDisplayId;

            SetUpdateField<uint>((int)UnitFields.DisplayID, displayId);
            SetUpdateField<uint>((int)UnitFields.NativeDisplayID, displayId);

            SetUpdateField<uint>((int)UnitFields.Flags, 0x8);

            SetUpdateField<float>((int)UnitFields.BoundingRadius, 0.389F);
            SetUpdateField<float>((int)UnitFields.CombatReach, 1.5F);
            SetUpdateField<float>((int)UnitFields.ModCastingSpeed, 1);
            SetUpdateField<float>((int)UnitFields.MaxHealthModifier, 1);
            
            SetUpdateField<byte>((int)PlayerFields.HairColorID, Skin, 0);
            SetUpdateField<byte>((int)PlayerFields.HairColorID, Face, 1);
            SetUpdateField<byte>((int)PlayerFields.HairColorID, HairStyle, 2);
            SetUpdateField<byte>((int)PlayerFields.HairColorID, HairColor, 3);
            SetUpdateField<byte>((int)PlayerFields.RestState, FacialHair, 0);
            SetUpdateField<byte>((int)PlayerFields.RestState, 0, 1);
            SetUpdateField<byte>((int)PlayerFields.RestState, 0, 2);
            SetUpdateField<byte>((int)PlayerFields.RestState, 2, 3);
            SetUpdateField<byte>((int)PlayerFields.ArenaFaction, Gender, 0);
            SetUpdateField<byte>((int)PlayerFields.ArenaFaction, 0, 1);
            SetUpdateField<byte>((int)PlayerFields.ArenaFaction, 0, 2);
            SetUpdateField<byte>((int)PlayerFields.ArenaFaction, 0, 3);
            SetUpdateField<int>((int)PlayerFields.WatchedFactionIndex, -1);
            SetUpdateField<int>((int)PlayerFields.XP, 0);
            SetUpdateField<int>((int)PlayerFields.NextLevelXP, 400);

            SetUpdateField<int>((int)PlayerFields.CurrentSpecID, (int)GetActiveSpecId());

            SetUpdateField<int>((int)PlayerFields.CharacterPoints, SpecializationMgr.GetUnspentTalentRowCount(this));
            SetUpdateField<int>((int)PlayerFields.MaxTalentTiers, SpecializationMgr.GetMaxTalentRowCount(this));

            for (int i = 0; i < 448; i++)
                if (i < Skills.Count)
                    SetUpdateField<uint>((int)PlayerFields.Skill + i, Skills[i].Id);

            SetUpdateField<uint>((int)PlayerFields.VirtualPlayerRealm, WorldConfig.RealmId);
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
    }
}
