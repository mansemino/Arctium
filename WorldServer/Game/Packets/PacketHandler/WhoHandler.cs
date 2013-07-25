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
using Framework.Configuration;
using Framework.Constants.NetMessage;
using Framework.Logging;
using Framework.Network.Packets;
using WorldServer.Network;
using WorldServer.Game.WorldEntities;
using Framework.ClientDB;
using Framework.ClientDB.Structures.Dbc;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class WhoHandler : Globals
    {
        [Opcode(ClientMessage.CliWhoRequest, "17128")]
        public static void HandleWhoListRequest(ref PacketReader packet, ref WorldClass session)
        {
            List<string> stringList = new List<string>();
            List<uint> lengthList   = new List<uint>();
            List<uint> zoneList     = new List<uint>();
            string name             = "";
            string guildName        = "";

            BitUnpack bitunpack     = new BitUnpack(packet);

            var classMask   = packet.Read<uint>();

            byte byteFlag   = packet.Read<byte>();

            var maxLevel    = packet.Read<uint>();
            var raceMask    = packet.Read<uint>();
            var intFlag     = packet.Read<uint>();
            var minLevel    = packet.Read<uint>();

            var nameLength  = bitunpack.GetBits<uint>(6);
            var textCounter = bitunpack.GetBits<byte>(3);
            var guildLength = bitunpack.GetBits<uint>(7);

            if (textCounter > 0)
            {
                for (var ctr = 0; ctr < textCounter; ctr++)
                    lengthList.Add(bitunpack.GetBits<uint>(7));
            }

            var zoneCounter = bitunpack.GetBits<byte>(4);
            var boolFalse   = bitunpack.GetBit();

            if (textCounter > 0)
            {
                foreach (uint length in lengthList)
                    if (length > 0)
                        stringList.Add(packet.ReadString(length).ToLower());
            }

            if (nameLength > 0)
                name = packet.ReadString(nameLength).ToLower();
            if (guildLength > 0)
                guildName = packet.ReadString(guildLength).ToLower();

            if (zoneCounter > 0)
            {
                for (var ctr = 0; ctr < zoneCounter; ctr++)
                    zoneList.Add(packet.Read<uint>());
            }

            List<Character> charactersList  = new List<Character>();
            bool maxResultsReached          = false;

            // Let's see if special filters or general search
            bool useName            = (name.Length > 0);
            bool useGuild           = (guildName.Length > 0);
            bool useRace            = (raceMask != 0xFFFFFFFF);
            bool useClass           = (classMask != 0xFFFFFFFF);
            bool useZone            = (zoneCounter > 0);

            bool specialSearch      = (useName || useGuild || useRace || useClass || useZone);
            bool generalSearch      = (!specialSearch && (textCounter > 0));
            bool allSearch          = (!specialSearch && !generalSearch);

            var me                  = session.Character;

            // If allSearch (take only characters of same faction)
            if (allSearch)
            {
                foreach (KeyValuePair<ulong, WorldClass> _session in Globals.WorldMgr.Sessions)
                {
                    var character = _session.Value.Character;

                    if ((me.UnitFaction == character.UnitFaction) && (character.Level >= minLevel) && (character.Level <= maxLevel))
                        charactersList.Add(character);

                    if (charactersList.Count >= 50)
                        break;
                }
            }
            // If strings to search are given, use them in all search fields (take only characters of same faction)
            else if (generalSearch)
            {
                string allStringsToSearchIn;
                bool allStringsFound;

                foreach (KeyValuePair<ulong, WorldClass> _session in Globals.WorldMgr.Sessions)
                {
                    if (maxResultsReached)
                        break;

                    var character = _session.Value.Character;

                    // Check if the "fixed" values match or the aren't needed
                    if (
                        (me.UnitFaction == character.UnitFaction) &&
                        ((character.Level >= minLevel) && (character.Level <= maxLevel))
                        )
                    {
                        // We're going to look for all possible texts; For that, let's use this trick
                        var chrRace     = CliDB.ChrRaces.SingleOrDefault(n => n.Id == character.Race);
                        var chrClass    = CliDB.ChrClasses.SingleOrDefault(n => n.Id == character.Class);
                        var chrArea     = CliDB.AreaTable.SingleOrDefault(n => n.Id == character.Zone);
                            
                        // We take all the used strings for the character and we'll check if all texts of the array are on it
                        allStringsToSearchIn = character.Name.ToLower() + // 
                            character.getGuildName().ToLower() + //
                            chrRace.Name.ToString().ToLower() + chrRace.NameFemale.ToString().ToLower() + //
                            chrClass.Name.ToString().ToLower() + chrClass.NameFemale.ToString().ToLower() + //
                            chrArea.Name.ToString().ToLower();

                        allStringsFound = true;
                        foreach (string text in stringList)
                            if (!allStringsToSearchIn.Contains(text))
                            {
                                allStringsFound = false;
                                break;
                            }

                        if (allStringsFound)
                        {
                            charactersList.Add(character);
                            maxResultsReached = (charactersList.Count >= 50);
                        }
                    }
                }
            }
            else if (specialSearch)
            {
                foreach (KeyValuePair<ulong, WorldClass> _session in Globals.WorldMgr.Sessions)
                {
                    if (maxResultsReached)
                        break;

                    var character = _session.Value.Character;

                    if (
                        (me.UnitFaction == character.UnitFaction) &&
                        (character.Level >= minLevel) && (character.Level <= maxLevel) &&
                        ((!useRace) || (useRace && ((((uint)1 << character.Race) & raceMask) != 0))) &&
                        ((!useClass) || (useClass && ((((uint)1 << character.Class) & classMask) != 0))) &&
                        ((!useName) || (useName && character.Name.ToLower().Contains(name))) &&
                        ((!useGuild) || (useGuild && character.getGuildName().ToLower().Contains(guildName)))
                        )
                    {
                        // Let's check for the zones. If not given, great...
                        if (!useZone)
                        {
                            charactersList.Add(character);
                            maxResultsReached = (charactersList.Count >= 50);
                        }
                        else
                        {
                            // If zones given, then check if the character is on any of them
                            bool found = false;
                            foreach (uint zone in zoneList)
                            {
                                if (zone == character.Zone)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            // is it? then add it to the list...
                            if (found)
                            {
                                charactersList.Add(character);
                                maxResultsReached = (charactersList.Count >= 50);
                            }
                        }
                    }
                }
            }
            else
            {
                Log.Message(LogType.Debug, "WhoList: Wrong condition reached.");
                return;
            }
            
            PacketWriter whoPacket = new PacketWriter(ServerMessage.Who);
            BitPack bitpack = new BitPack(whoPacket);

            var count = charactersList.Count;

            bitpack.Write(count, 6);

            if (count > 0)
            {
                foreach (Character character in charactersList)
                {
                    bitpack.Write(character.getGuildName().Length, 7);
                    bitpack.Write(character.Name.Length, 6);
                    bitpack.Write(0); // false? !online? dead?
                }

                bitpack.Flush();

                foreach (Character character in charactersList)
                {
                    whoPacket.WriteUInt32(character.Zone);
                    whoPacket.WriteString(character.getGuildName(), false);
                    whoPacket.WriteUInt32(character.Class);
                    whoPacket.WriteUInt32(WorldConfig.RealmId); // atm, only one realm is supported
                    whoPacket.WriteUInt32(character.Race);
                    whoPacket.WriteUInt32(character.Level);
                    whoPacket.WriteUInt8(character.Gender);
                    whoPacket.WriteString(character.Name, false);
                }
            }
            else
            {
                bitpack.Flush();
            }

            session.Send(ref whoPacket);
        }
    }
}