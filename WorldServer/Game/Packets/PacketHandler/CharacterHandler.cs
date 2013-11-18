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
using System.Text;
using Framework.ClientDB;
using Framework.Configuration;
using Framework.Constants;
using Framework.Constants.NetMessage;
using Framework.Database;
using Framework.Logging;
using Framework.Network.Packets;
using WorldServer.Game.Packets.PacketHandler;
using WorldServer.Game.WorldEntities;
using WorldServer.Network;

namespace WorldServer.Game.PacketHandler
{
    public class CharacterHandler : Globals
    {
        [Opcode(ClientMessage.EnumCharacters, "17538")]
        public static void HandleEnumCharactersResult(ref PacketReader packet, WorldClass session)
        {
            // Set existing character from last world session to null
            session.Character = null;

            DB.Realms.Execute("UPDATE accounts SET online = 1 WHERE id = ?", session.Account.Id);

            SQLResult result = DB.Characters.Select("SELECT * FROM characters WHERE accountid = ?", session.Account.Id);

            PacketWriter enumCharacters = new PacketWriter(ServerMessage.EnumCharactersResult);
            BitPack BitPack = new BitPack(enumCharacters);

            BitPack.Write(result.Count, 16);

            for (int c = 0; c < result.Count; c++)
            {
                var name = result.Read<string>(c, "Name");
                var loginCinematic = result.Read<bool>(c, "LoginCinematic");

                BitPack.Guid = result.Read<ulong>(c, "Guid");
                BitPack.GuildGuid = result.Read<ulong>(c, "GuildGuid");

                BitPack.WriteGuildGuidMask(3);
                BitPack.Write(loginCinematic);
                BitPack.WriteGuidMask(6);
                BitPack.WriteGuildGuidMask(1);
                BitPack.WriteGuidMask(1, 5);
                BitPack.WriteGuildGuidMask(6);
                BitPack.WriteGuidMask(7, 0);
                BitPack.WriteGuildGuidMask(5);
                BitPack.WriteGuidMask(2);
                BitPack.Write((uint)UTF8Encoding.UTF8.GetBytes(name).Length, 6);
                BitPack.WriteGuidMask(4);
                BitPack.WriteGuildGuidMask(4, 2);
                BitPack.WriteGuidMask(3);
                BitPack.WriteGuildGuidMask(0, 7);
            }

            BitPack.Write(1);
            BitPack.Write(0, 21);

            BitPack.Flush();

            for (int c = 0; c < result.Count; c++)
            {
                string name = result.Read<string>(c, "Name");
                BitPack.Guid = result.Read<ulong>(c, "Guid");
                BitPack.GuildGuid = result.Read<ulong>(c, "GuildGuid");

                enumCharacters.WriteUInt8(result.Read<byte>(c, "Skin"));

                BitPack.WriteGuidBytes(2, 7);

                enumCharacters.WriteUInt32(result.Read<uint>(c, "PetDisplayId"));
                enumCharacters.WriteString(name);

                //TODO: implement items
                for (int j = 0; j < 23; j++)
                {
                    enumCharacters.WriteUInt32(0);
                    enumCharacters.WriteUInt32(0);
                    enumCharacters.WriteUInt8(0);
                }

                BitPack.WriteGuidBytes(4, 6);

                enumCharacters.WriteUInt8(result.Read<byte>(c, "Level"));
                enumCharacters.WriteFloat(result.Read<float>(c, "Y"));
                enumCharacters.WriteFloat(result.Read<float>(c, "X"));
                enumCharacters.WriteUInt8(result.Read<byte>(c, "Face"));

                BitPack.WriteGuildGuidBytes(0);

                enumCharacters.WriteUInt8(0);
                enumCharacters.WriteUInt32(result.Read<uint>(c, "Zone"));

                BitPack.WriteGuildGuidBytes(7);

                enumCharacters.WriteUInt32(result.Read<uint>(c, "CharacterFlags"));
                enumCharacters.WriteUInt32(result.Read<uint>(c, "Map"));
                enumCharacters.WriteUInt8(result.Read<byte>(c, "Race"));
                enumCharacters.WriteFloat(result.Read<float>(c, "Z"));

                BitPack.WriteGuildGuidBytes(1);

                enumCharacters.WriteUInt8(result.Read<byte>(c, "Gender"));

                BitPack.WriteGuidBytes(3);

                enumCharacters.WriteUInt8(result.Read<byte>(c, "HairColor"));

                BitPack.WriteGuildGuidBytes(5);

                enumCharacters.WriteUInt8(result.Read<byte>(c, "Class"));

                BitPack.WriteGuildGuidBytes(3);
                BitPack.WriteGuidBytes(1);

                enumCharacters.WriteUInt32(result.Read<uint>(c, "CustomizeFlags"));
                enumCharacters.WriteUInt8(result.Read<byte>(c, "FacialHair"));

                BitPack.WriteGuildGuidBytes(6);
                BitPack.WriteGuidBytes(0);

                enumCharacters.WriteUInt8(result.Read<byte>(c, "HairStyle"));

                BitPack.WriteGuidBytes(5);

                enumCharacters.WriteUInt32(result.Read<uint>(c, "PetFamily"));

                BitPack.WriteGuildGuidBytes(2);

                enumCharacters.WriteUInt32(result.Read<uint>(c, "PetLevel"));

                BitPack.WriteGuildGuidBytes(4);
            }

            session.Send(ref enumCharacters);
        }

        [Opcode(ClientMessage.CreateCharacter, "17538")]
        public static void HandleCreateCharacter(ref PacketReader packet, WorldClass session)
        {
            BitUnpack BitUnpack = new BitUnpack(packet);

            var hairStyle = packet.ReadByte();
            var gender = packet.ReadByte();
            var race = packet.ReadByte();
            var hairColor = packet.ReadByte();
            var pClass = packet.ReadByte();
            var facialHair = packet.ReadByte();
            packet.ReadByte();                      // Always 0
            var skin = packet.ReadByte();
            var face       = packet.ReadByte();

            var nameLength = BitUnpack.GetBits<uint>(7);
            var name = Character.NormalizeName(packet.ReadString(nameLength));

            var result = DB.Characters.Select("SELECT * from characters WHERE name = ?", name);
            var createChar = new PacketWriter(ServerMessage.CreateChar);

            if (result.Count != 0)
            {
                // Name already in use
                createChar.WriteUInt8(0x32);
                session.Send(ref createChar);
                return;
            }

            result = DB.Characters.Select("SELECT map, zone, posX, posY, posZ, posO FROM character_creation_data WHERE race = ? AND class = ?", race, pClass);
            if (result.Count == 0)
            {
                createChar.WriteUInt8(0x31);
                session.Send(ref createChar);
                return;
            }

            var map  = result.Read<uint>(0, "map");
            var zone = result.Read<uint>(0, "zone");
            var posX = result.Read<float>(0, "posX");
            var posY = result.Read<float>(0, "posY");
            var posZ = result.Read<float>(0, "posZ");
            var posO = result.Read<float>(0, "posO");

            // Allow declined names for now
            var characterFlags = CharacterFlag.Decline;

            DB.Characters.Execute("INSERT INTO characters (name, accountid, realmId, race, class, gender, skin, zone, map, x, y, z, o, face, hairstyle, haircolor, facialhair, characterFlags) VALUES (" +
                                  "?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                                  name, session.Account.Id, WorldConfig.RealmId, race, pClass, gender, skin, zone, map, posX, posY, posZ, posO, face, hairStyle, hairColor, facialHair, characterFlags);

            // Success
            createChar.WriteUInt8(0x2F);
            session.Send(ref createChar);
        }

        [Opcode(ClientMessage.CharDelete, "17538")]
        public static void HandleCharDelete(ref PacketReader packet, WorldClass session)
        {
            byte[] guidMask = { 5, 6, 1, 0, 3, 4, 2, 7 };
            byte[] guidBytes = { 2, 0, 4, 1, 5, 3, 7, 6 };

            var GuidUnpacker = new BitUnpack(packet);
            var guid = GuidUnpacker.GetPackedValue(guidMask, guidBytes);

            PacketWriter deleteChar = new PacketWriter(ServerMessage.DeleteChar);

            deleteChar.WriteUInt8(0x47);

            session.Send(ref deleteChar);

            DB.Characters.Execute("DELETE FROM characters WHERE guid = ?", guid);
            DB.Characters.Execute("DELETE FROM character_spells WHERE guid = ?", guid);
            DB.Characters.Execute("DELETE FROM character_skills WHERE guid = ?", guid);
        }

        [Opcode(ClientMessage.GenerateRandomCharacterName, "17538")]
        public static void HandleGenerateRandomCharacterName(ref PacketReader packet, WorldClass session)
        {
            var race = packet.ReadByte();
            var gender = packet.ReadByte();

            List<string> names = CliDB.NameGen.Where(n => n.Race == race && n.Gender == gender).Select(n => n.Name).ToList();
            Random rand = new Random(Environment.TickCount);

            string NewName;
            SQLResult result;
            do
            {
                NewName = names[rand.Next(names.Count)];
                result = DB.Characters.Select("SELECT * FROM characters WHERE name = ?", NewName);
            }
            while (result.Count != 0);

            PacketWriter generateRandomCharacterNameResult = new PacketWriter(ServerMessage.GenerateRandomCharacterNameResult);
            BitPack BitPack = new BitPack(generateRandomCharacterNameResult);

            BitPack.Write(1);
            BitPack.Write(NewName.Length, 6);

            BitPack.Flush();

            generateRandomCharacterNameResult.WriteString(NewName);
            session.Send(ref generateRandomCharacterNameResult);
        }

        [Opcode(ClientMessage.PlayerLogin, "17538")]
        public static void HandlePlayerLogin(ref PacketReader packet, WorldClass session)
        {
            byte[] guidMask = { 1, 0, 7, 2, 5, 6, 4, 3 };
            byte[] guidBytes = { 7, 6, 0, 1, 4, 3, 2, 5 };

            BitUnpack GuidUnpacker = new BitUnpack(packet);

            var unknown = packet.Read<float>();
            var guid = GuidUnpacker.GetPackedValue(guidMask, guidBytes);

            Log.Message(LogType.Debug, "Character with Guid: {0}, AccountId: {1} tried to enter the world.", guid, session.Account.Id);

            session.Character = new Character(guid);

            if (!WorldMgr.AddSession(guid, ref session))
            {
                Log.Message(LogType.Error, "A Character with Guid: {0} is already logged in", guid);
                return;
            }

            WorldMgr.WriteAccountDataTimes(AccountDataMasks.CharacterCacheMask, ref session);

            MiscHandler.HandleMessageOfTheDay(ref session);
            TimeHandler.HandleLoginSetTimeSpeed(ref session);
            SpecializationHandler.HandleUpdateTalentData(ref session);
            SpellHandler.HandleSendKnownSpells(ref session);
            MiscHandler.HandleUpdateActionButtons(ref session);

            if (session.Character.LoginCinematic)
                CinematicHandler.HandleStartCinematic(ref session);

            ObjectHandler.HandleUpdateObjectCreate(ref session);
        }
    }
}
