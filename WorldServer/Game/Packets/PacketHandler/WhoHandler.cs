using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Configuration;
using Framework.Constants;
using Framework.Constants.NetMessage;
using Framework.Logging;
using Framework.Network.Packets;
using WorldServer.Game.ObjectDefines;
using WorldServer.Network;
using Framework.Database;
using Framework.ObjectDefines;
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
            List<uint> intList      = new List<uint>();
            string name             = "";
            string guildName        = "";

            BitUnpack bitunpack     = new BitUnpack(packet);

            var skip        = packet.ReadUInt32();

            byte byteFlag   = packet.ReadByte();

            var levelBackup = packet.ReadUInt32();
            skip            = packet.ReadUInt32();
            var intFlag     = packet.ReadUInt32();
            var level       = packet.ReadUInt32();

            var nameLength  = bitunpack.GetBits<uint>(6);
            var textCounter = bitunpack.GetBits<byte>(3);
            var guildLength = bitunpack.GetBits<uint>(7);

            if (textCounter > 0)
            {
                for (var ctr = 0; ctr < textCounter; ctr++)
                    lengthList.Add(bitunpack.GetBits<uint>(7));
            }

            var intCounter = bitunpack.GetBits<byte>(4);
            var boolFalse = bitunpack.GetBit();

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

            if (intCounter > 0)
            {
                for (var ctr = 0; ctr < intCounter; ctr++)
                    intList.Add(packet.ReadUInt32());
            }

            List<Character> charactersList = new List<Character>();

            bool useLevel           = (level == levelBackup);
            bool useName            = (name.Length > 0);
            bool useGuild           = (guildName.Length > 0);
            bool maxResultsReached  = false;
            string allStringsToSearchIn;
            bool allStringsFound;

            // If not criteria, add char
            if (!useLevel && !useName && !useGuild && (stringList.Count == 0))
                foreach (KeyValuePair<ulong, WorldClass> _session in Globals.WorldMgr.Sessions)
                {
                    charactersList.Add(_session.Value.Character);
                    if (charactersList.Count >= 50)
                        break;
                }
            else 
                foreach (KeyValuePair<ulong, WorldClass> _session in Globals.WorldMgr.Sessions)
                {
                    if (maxResultsReached)
                        break;

                    var character = _session.Value.Character;

                    // Check if the "fixed" values match or the aren't needed
                    if (
                        ((useLevel && character.Level == level) || !useLevel ) &&
                        ((useName && character.Name.ToLower().Contains(name)) || !useName) && 
                        ((useGuild && character.getGuildName().ToLower().Contains(guildName)) || !useGuild)
                        )
                    {
                        // We're going to look for all possible texts; For that, let's use this trick
                        var chrRace     = CliDB.ChrRaces.SingleOrDefault(n => n.Id == character.Race);
                        var chrClass    = CliDB.ChrClasses.SingleOrDefault(n => n.Id == character.Class);
                        var chrArea     = CliDB.AreaTable.SingleOrDefault(n => n.Id == character.Zone);
                            
                        // We take all the strings for the character and we'll check if all strings are on it
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