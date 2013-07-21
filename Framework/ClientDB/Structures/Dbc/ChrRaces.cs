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

using Framework.ClientDB.CustomTypes;

namespace Framework.ClientDB.Structures.Dbc
{
    public class ChrRaces
    {
        public uint Id;
        public uint Flags;
        public uint Faction;
        public uint ExplorationSound;
        public uint MaleDisplayId;
        public uint FemaleDisplayId;
        public uint ClientPrefix;
        public uint BaseLanguage; 
        public uint CreatureType;
        public uint ResSicknessSpellID;
        public uint SplashSoundID;
        public string ClientFileName; 
        public uint CinematicSequence;
        public uint BaseFaction;
        public string Name;
        public string NameFemale;
        public Unused NameMale;
        public Unused MaleFacialHairs;
        public Unused FemaleFacialHairs;
        public Unused Hairs;
        public uint OtherFactionRace;
        public uint Unknown3;
        public uint Unknown4;
        public uint Unknown5;
        public uint RaceCount;
        public uint Unknown6;
        public uint Unknown7;
        public uint Unknown8;
        public float[] Unknown9 = new float[6];
        public uint Unknown10;
        public uint Unknown11;
    }
}
