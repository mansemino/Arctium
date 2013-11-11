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
using Framework.Configuration;
using Framework.Constants.Authentication;
using Framework.Constants.NetMessage;
using Framework.Database;
using Framework.Network.Packets;
using Framework.ObjectDefines;
using WorldServer.Network;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class AuthenticationHandler : Globals
    {
        [Opcode(ClientMessage.TransferInitiate, "17538")]
        public static void HandleAuthChallenge(ref PacketReader packet, WorldClass session)
        {
            PacketWriter authChallenge = new PacketWriter(ServerMessage.AuthChallenge, true);

            authChallenge.WriteUInt8(1);

            for (int i = 0; i < 8; i++)
                authChallenge.WriteUInt32(0);

            authChallenge.WriteUInt32((uint)new Random(DateTime.Now.Second).Next(1, 0xFFFFFFF));

            session.Send(ref authChallenge);
        }

        [Opcode(ClientMessage.AuthSession, "17538")]
        public static void HandleAuthResponse(ref PacketReader packet, WorldClass session)
        {
            BitUnpack BitUnpack = new BitUnpack(packet);

            packet.Skip(54);

            int addonSize = packet.Read<int>();
            packet.Skip(addonSize);

            uint nameLength = BitUnpack.GetBits<uint>(11);
            string accountName = packet.ReadString(nameLength);

            SQLResult result = DB.Realms.Select("SELECT * FROM accounts WHERE name = ?", accountName);
            if (result.Count == 0)
                session.clientSocket.Close();
            else
                session.Account = new Account()
                {
                    Id         = result.Read<int>(0, "id"),
                    Name       = result.Read<string>(0, "name"),
                    Password   = result.Read<string>(0, "password"),
                    SessionKey = result.Read<string>(0, "sessionkey"),
                    Expansion  = result.Read<byte>(0, "expansion"),
                    GMLevel    = result.Read<byte>(0, "gmlevel"),
                    IP         = result.Read<string>(0, "ip"),
                    Language   = result.Read<string>(0, "language")
                };

            string K = session.Account.SessionKey;
            byte[] kBytes = new byte[K.Length / 2];

            for (int i = 0; i < K.Length; i += 2)
                kBytes[i / 2] = Convert.ToByte(K.Substring(i, 2), 16);

            session.Crypt.Initialize(kBytes);

            uint realmId = WorldConfig.RealmId;
            SQLResult realmClassResult = DB.Realms.Select("SELECT class, expansion FROM realm_classes WHERE realmId = ?", realmId);
            SQLResult realmRaceResult = DB.Realms.Select("SELECT race, expansion FROM realm_races WHERE realmId = ?", realmId);

            var HasAccountData = true;
            var IsInQueue = false;

            PacketWriter authResponse = new PacketWriter(ServerMessage.AuthResponse);
            BitPack BitPack = new BitPack(authResponse);

            authResponse.WriteUInt8((byte)AuthCodes.AUTH_OK);

            BitPack.Write(IsInQueue);

            if (IsInQueue)
                BitPack.Write(1);                                  // Unknown

            BitPack.Write(HasAccountData);

            if (HasAccountData)
            {
                BitPack.Write(0);
                BitPack.Write(0, 21);
                BitPack.Write(0, 21);
                BitPack.Write(realmRaceResult.Count, 23);
                BitPack.Write(0);
                BitPack.Write(0);
                BitPack.Write(0);
                BitPack.Write(realmClassResult.Count, 23);
            }

            BitPack.Flush();

            if (HasAccountData)
            {
                authResponse.WriteUInt32(0);
                authResponse.WriteUInt32(0);
                authResponse.WriteUInt8(session.Account.Expansion);

                for (int r = 0; r < realmRaceResult.Count; r++)
                {
                    authResponse.WriteUInt8(realmRaceResult.Read<byte>(r, "expansion"));
                    authResponse.WriteUInt8(realmRaceResult.Read<byte>(r, "race"));

                }


                authResponse.WriteUInt8(session.Account.Expansion);
                authResponse.WriteUInt32(0);

                for (int c = 0; c < realmClassResult.Count; c++)
                {
                    authResponse.WriteUInt8(realmClassResult.Read<byte>(c, "class"));
                    authResponse.WriteUInt8(realmClassResult.Read<byte>(c, "expansion"));
                }

                authResponse.WriteUInt32(0);
                authResponse.WriteUInt32(0);
            }

            if (IsInQueue)
                authResponse.WriteUInt32(0);

            session.Send(ref authResponse);

            MiscHandler.HandleCacheVersion(ref session);
            TutorialHandler.HandleTutorialFlags(ref session);
        }
    }
}
