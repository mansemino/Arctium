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

using Framework.Configuration;
using Framework.Constants.Authentication;
using Framework.Constants.NetMessage;
using Framework.Database;
using Framework.Network.Packets;
using Framework.ObjectDefines;
using System;
using WorldServer.Network;
using Framework.Logging;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class AuthenticationHandler : Globals
    {
        [Opcode(ClientMessage.TransferInitiate, "16992")]
        public static void HandleAuthChallenge(ref PacketReader packet, ref WorldClass session)
        {
            PacketWriter authChallenge = new PacketWriter(ServerMessage.AuthChallenge, true);

            for (int i = 0; i < 8; i++)
                authChallenge.WriteUInt32(0);

            authChallenge.WriteUInt32((uint)new Random(DateTime.Now.Second).Next(1, 0xFFFFFFF));
            authChallenge.WriteUInt8(1);

            session.Send(ref authChallenge);
        }

        [Opcode(ClientMessage.AuthSession, "16992")]
        public static void HandleAuthResponse(ref PacketReader packet, ref WorldClass session)
        {
            BitUnpack BitUnpack = new BitUnpack(packet);

            ushort      skipBytes;                      // Skip first 2 bytes
            uint        playerData1;                    // this[16]
            ushort      clientBuild;                    // this[20]
	        uint        playerData4, playerData5;       // this[24], this[28]
	        byte []     authChallenge = new byte[20];	// this[32] ~ this[51]
	        uint        playerData7, playerData8;       // this[52], this[56]
	        byte []     playerData9 = new byte[2];      // this[60], this[61]
	        ulong       playerData11;                   // this[64], this[68]

            skipBytes         = packet.ReadUInt16();
            playerData1       = packet.ReadUInt32();
            authChallenge[8]  = packet.ReadByte();
            authChallenge[13] = packet.ReadByte();
            authChallenge[3]  = packet.ReadByte();
            playerData5       = packet.ReadUInt32();
            authChallenge[6]  = packet.ReadByte();
            clientBuild       = packet.ReadUInt16();
            authChallenge[2]  = packet.ReadByte();
            authChallenge[0]  = packet.ReadByte();
            authChallenge[7]  = packet.ReadByte();
            authChallenge[11] = packet.ReadByte();
            playerData8       = packet.ReadUInt32();
            authChallenge[5]  = packet.ReadByte();
            authChallenge[15] = packet.ReadByte();
            authChallenge[14] = packet.ReadByte();
            authChallenge[12] = packet.ReadByte();
            playerData11      = packet.ReadUInt64();       
            playerData9[1]    = packet.ReadByte();
            playerData7       = packet.ReadUInt32();
            playerData4       = packet.ReadUInt32();
            authChallenge[1]  = packet.ReadByte();
            authChallenge[9]  = packet.ReadByte();
            authChallenge[4]  = packet.ReadByte();
            authChallenge[17] = packet.ReadByte();
            authChallenge[16] = packet.ReadByte();
            authChallenge[19] = packet.ReadByte();
            authChallenge[18] = packet.ReadByte();
            authChallenge[10] = packet.ReadByte();
            playerData9[0]    = packet.ReadByte();

            // packet.Skip(54);

            int addonPackedSize     = packet.Read<int>();
            int addonUnpackedSize   = packet.Read<int>();

            byte[] packedAddon = packet.ReadBytes(addonPackedSize - 4);
            AddonMgr.ReadAddonData(packedAddon, addonUnpackedSize, ref session);

            // packet.Skip(addonSize);

            bool aBit = BitUnpack.GetBit(); // this[72]

            uint nameLength = BitUnpack.GetBits<uint>(11);
            string accountName = packet.ReadString(nameLength);

            SQLResult result = DB.Realms.Select("SELECT * FROM accounts WHERE name = ?", accountName);
            if (result.Count == 0)
                session.clientSocket.Close();
            else
                session.Account = new Account()
                {
                    Id         = result.Read<int>(0, "id"),
                    Name       = result.Read<String>(0, "name"),
                    Password   = result.Read<String>(0, "password"),
                    SessionKey = result.Read<String>(0, "sessionkey"),
                    Expansion  = result.Read<byte>(0, "expansion"),
                    GMLevel    = result.Read<byte>(0, "gmlevel"),
                    IP         = result.Read<String>(0, "ip"),
                    Language   = result.Read<String>(0, "language")
                };

            string K = session.Account.SessionKey;
            byte[] kBytes = new byte[K.Length / 2];

            for (int i = 0; i < K.Length; i += 2)
                kBytes[i / 2] = Convert.ToByte(K.Substring(i, 2), 16);

            session.Crypt.Initialize(kBytes);

            uint realmId = WorldConfig.RealmId;
            SQLResult realmClassResult = DB.Realms.Select("SELECT class, expansion FROM realm_classes WHERE realmId = ?", realmId);
            SQLResult realmRaceResult = DB.Realms.Select("SELECT race, expansion FROM realm_races WHERE realmId = ?", realmId);

            bool HasAccountData = true;
            bool IsInQueue = false;

            PacketWriter authResponse = new PacketWriter(ServerMessage.AuthResponse);
            BitPack BitPack = new BitPack(authResponse);

            BitPack.Write(IsInQueue);

            if (IsInQueue)
                BitPack.Write(1);                                  // Unknown

            BitPack.Write(HasAccountData);

            if (HasAccountData)
            {
                BitPack.Write(0);                                  // Unknown, 5.0.4
                BitPack.Write(0);                                  // Unknown, 5.3.0
                BitPack.Write(realmRaceResult.Count, 23);          // Activation count for races
                BitPack.Write(0);                                  // Unknown, 5.1.0
                BitPack.Write(0, 21);                              // Activate character template windows/button
                
                //if (HasCharacterTemplate)
                //Write bits for char templates...

                BitPack.Write(realmClassResult.Count, 23);         // Activation count for classes
                BitPack.Write(0, 22);                              // Unknown, 5.3.0
                BitPack.Write(0);                                  // Unknown2, 5.3.0
            }

            BitPack.Flush();

            if (HasAccountData)
            {
                authResponse.WriteUInt8(0);

                for (int c = 0; c < realmClassResult.Count; c++)
                {
                    authResponse.WriteUInt8(realmClassResult.Read<byte>(c, "expansion"));
                    authResponse.WriteUInt8(realmClassResult.Read<byte>(c, "class"));
                }

                //if (Unknown2)
                //    authResponse.WriteUInt16(0);
    
                //if (HasCharacterTemplate)
                //Write data for char templates...

                //if (Unknown)
                //    authResponse.WriteUInt16(0);

                for (int r = 0; r < realmRaceResult.Count; r++)
                {
                    authResponse.WriteUInt8(realmRaceResult.Read<byte>(r, "expansion"));
                    authResponse.WriteUInt8(realmRaceResult.Read<byte>(r, "race"));
                }

                authResponse.WriteUInt32(0);
                authResponse.WriteUInt32(0);
                authResponse.WriteUInt32(0);

                authResponse.WriteUInt8(session.Account.Expansion);

                // Unknown Counter
                // Write UInt32...

                authResponse.WriteUInt8(session.Account.Expansion);
            }

            authResponse.WriteUInt8((byte)AuthCodes.AUTH_OK);

            if (IsInQueue)
                authResponse.WriteUInt32(0); 

            session.Send(ref authResponse);

            MiscHandler.HandleCacheVersion(ref session);
            TutorialHandler.HandleTutorialFlags(ref session);
        }
    }
}
