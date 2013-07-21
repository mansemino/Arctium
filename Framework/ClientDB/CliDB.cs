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
using Framework.ClientDB.Reader;
using Framework.ClientDB.Structures.Dbc;
using Framework.Logging;

namespace Framework.ClientDB
{
    public class CliDB
    {
        public static int Count { get; set; }

        public static List<ChrClasses> ChrClasses;
        public static List<ChrRaces> ChrRaces;
        public static List<ChrSpecialization> ChrSpecialization;
        public static List<NameGen> NameGen;
        public static List<SpecializationSpells> SpecializationSpells;
        public static List<Spell> Spell;
        public static List<SpellLevels> SpellLevels;
        public static List<Talent> Talent;

        public static List<AreaTable> AreaTable;
        public static List<AreaPOI> AreaPOI;
        public static List<AreaTrigger> AreaTrigger;
        public static List<WorldSafeLocs> WorldSafeLocs;
        public static List<Emotes> Emotes;
        public static List<EmotesText> EmotesText;

        public static void Initialize()
        {
            Log.Message(LogType.Normal, "Loading CliDBs...");

            ChrClasses           = DBReader.Read<ChrClasses>("ChrClasses.dbc");
            ChrRaces             = DBReader.Read<ChrRaces>("ChrRaces.dbc");
            ChrSpecialization    = DBReader.Read<ChrSpecialization>("ChrSpecialization.dbc");
            NameGen              = DBReader.Read<NameGen>("NameGen.dbc");
            SpecializationSpells = DBReader.Read<SpecializationSpells>("SpecializationSpells.dbc");
            Spell                = DBReader.Read<Spell>("Spell.dbc");
            SpellLevels          = DBReader.Read<SpellLevels>("SpellLevels.dbc");
            Talent               = DBReader.Read<Talent>("Talent.dbc");

            AreaTable            = DBReader.Read<AreaTable>("AreaTable.dbc");
            AreaPOI              = DBReader.Read<AreaPOI>("AreaPOI.dbc");
            AreaTrigger          = DBReader.Read<AreaTrigger>("AreaTrigger.dbc");
            WorldSafeLocs        = DBReader.Read<WorldSafeLocs>("WorldSafeLocs.dbc");
            Emotes               = DBReader.Read<Emotes>("Emotes.dbc");
            EmotesText           = DBReader.Read<EmotesText>("EmotesText.dbc");

            Log.Message(LogType.Normal, "Loaded {0} CliDBs.", Count);
            Log.Message();
        }
    }
}
