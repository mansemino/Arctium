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
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using Framework.Constants.Authentication;
using Framework.Cryptography;
using Framework.Database;
using Framework.Logging;
using Framework.Network.Packets;
using Framework.ObjectDefines;

namespace Framework.Network.Realm
{
    public class RealmClass
    {
        public static Account account { get; set; }
        public static Dictionary<string, ObjectDefines.Realm> Realms = new Dictionary<string, ObjectDefines.Realm>();
        public static RealmNetwork realm;
        public SRP6 SecureRemotePassword { get; set; }
        public Socket clientSocket;
        byte[] DataBuffer = new byte[0x400];

        public RealmClass()
        {
            account = new Account();
            SecureRemotePassword = new SRP6();
        }

        void HandleRealmData(byte[] data)
        {
            using (var reader = new PacketReader(data, false))
            {
                ClientLink cmd = (ClientLink)reader.Read<byte>();

                switch (cmd)
                {
                    case ClientLink.CMD_AUTH_LOGON_CHALLENGE:
                    case ClientLink.CMD_AUTH_RECONNECT_CHALLENGE:
                        HandleAuthLogonChallenge(reader);
                        break;
                    case ClientLink.CMD_AUTH_LOGON_PROOF:
                    case ClientLink.CMD_AUTH_RECONNECT_PROOF:
                        HandleAuthLogonProof(reader);
                        break;
                    case ClientLink.CMD_REALM_LIST:
                        HandleRealmList(reader);
                        break;
                    default:
                        Log.Message(LogType.Normal, "Received unknown ClientLink: {0}", cmd);
                        break;
                }
            }
        }

        public void HandleAuthLogonChallenge(PacketReader data)
        {
            Log.Message(LogType.Normal, "AuthLogonChallenge");

            data.Skip(10);
            ushort ClientBuild = data.Read<ushort>();
            data.Skip(8);
            account.Language = data.ReadStringFromBytes(4);
            data.Skip(4);

            account.IP = data.ReadIPAddress();
            account.Name = data.ReadAccountName();

            SQLResult result = DB.Realms.Select("SELECT id, name, password, expansion, gmlevel, securityFlags, online FROM accounts WHERE name = ?", account.Name);

            using (var logonChallenge = new PacketWriter())
            {
                logonChallenge.WriteUInt8((byte)ClientLink.CMD_AUTH_LOGON_CHALLENGE);
                logonChallenge.WriteUInt8(0);

                if (result.Count != 0)
                {
                    if (result.Read<bool>(0, "online"))
                    {
                        logonChallenge.WriteUInt8((byte)AuthResults.WOW_FAIL_ALREADY_ONLINE);
                        Send(logonChallenge);
                        return;
                    }

                    account.Id = result.Read<int>(0, "id");
                    account.Expansion = result.Read<byte>(0, "expansion");
                    account.SecurityFlags = result.Read<byte>(0, "securityFlags");

                    DB.Realms.Execute("UPDATE accounts SET ip = ?, language = ? WHERE id = ?", account.IP, account.Language, account.Id);

                    var username = result.Read<string>(0, "name").ToUpper();
                    var password = result.Read<string>(0, "password").ToUpper();

                    // WoW 5.3.0.17128
                    if (ClientBuild == 17128)
                    {
                        SecureRemotePassword.CalculateX(username, password);

                        var randBytes = new byte[0x10];

                        var random = RNGCryptoServiceProvider.Create();
                        random.GetBytes(randBytes);

                        logonChallenge.WriteUInt8((byte)AuthResults.WOW_SUCCESS);
                        logonChallenge.WriteBytes(SecureRemotePassword.B);
                        logonChallenge.WriteUInt8(1);
                        logonChallenge.WriteUInt8(SecureRemotePassword.g.ToByteArray()[0]);
                        logonChallenge.WriteUInt8(0x20);
                        logonChallenge.WriteBytes(SecureRemotePassword.N);
                        logonChallenge.WriteBytes(SecureRemotePassword.Salt);
                        logonChallenge.WriteBytes(randBytes);

                        // Security flags
                        logonChallenge.WriteUInt8(account.SecurityFlags);

                        // Enable authenticator
                        if ((account.SecurityFlags & 4) != 0)
                            logonChallenge.WriteUInt8(1);
                    }
                }
                else
                    logonChallenge.WriteUInt8((byte)AuthResults.WOW_FAIL_UNKNOWN_ACCOUNT);

                Send(logonChallenge);
            }
        }

        public void HandleAuthAuthenticator(PacketReader data)
        {
            Log.Message(LogType.Normal, "AuthAuthenticator");
        }

        public void HandleAuthLogonProof(PacketReader data)
        {
            Log.Message(LogType.Normal, "AuthLogonProof");

            using (var logonProof = new PacketWriter())
            {
                var a = data.ReadBytes(32);
                var m1 = data.ReadBytes(20);

                SecureRemotePassword.CalculateU(a);
                SecureRemotePassword.CalculateM2(m1);
                SecureRemotePassword.CalculateK();

                foreach (var b in SecureRemotePassword.K)
                    if (b < 0x10)
                        account.SessionKey += "0" + string.Format(CultureInfo.InvariantCulture, "{0:X}", b);
                    else
                        account.SessionKey += string.Format(CultureInfo.InvariantCulture, "{0:X}", b);

                logonProof.WriteUInt8((byte)ClientLink.CMD_AUTH_LOGON_PROOF);
                logonProof.WriteUInt8(0);
                logonProof.WriteBytes(SecureRemotePassword.M2);
                logonProof.WriteUInt32(0x800000);
                logonProof.WriteUInt32(0);
                logonProof.WriteUInt16(0);

                SecureRemotePassword.Dispose();

                DB.Realms.Execute("UPDATE accounts SET sessionkey = ? WHERE id = ?", account.SessionKey, account.Id);

                Send(logonProof);
            }
        }

        public void HandleRealmList(PacketReader data)
        {
            Log.Message(LogType.Normal, "RealmList");

            using (var realmData = new PacketWriter())
            {
                Realms.ToList().ForEach(r =>
                {
                    using (var result = DB.Characters.Select("SELECT COUNT(*) as Count FROM characters WHERE AccountId = ? AND RealmId = ?", account.Id, r.Value.Id))
                    {
                        var charCount = result.Read<byte>(0, "Count");

                        realmData.WriteUInt8(1);
                        realmData.WriteUInt8(0);
                        realmData.WriteUInt8(0);
                        realmData.WriteCString(r.Key);
                        realmData.WriteCString(r.Value.IP + ":" + r.Value.Port);
                        realmData.WriteFloat(0);
                        realmData.WriteUInt8(charCount);
                        realmData.WriteUInt8(1);
                        realmData.WriteUInt8(0x2C);
                    }
                });

                using (var realmList = new PacketWriter())
                {
                    realmList.WriteUInt8((byte)ClientLink.CMD_REALM_LIST);
                    realmList.WriteUInt16((ushort)(realmData.BaseStream.Length + 8));
                    realmList.WriteUInt32(0);
                    realmList.WriteUInt16((ushort)Realms.Count);
                    realmList.WriteBytes(realmData.ReadDataToSend());
                    realmList.WriteUInt8(0);
                    realmList.WriteUInt8(0x10);

                    Send(realmList);
                }
            }
        }

        public void Accept(IAsyncResult ar)
        {
            clientSocket = (ar.AsyncState as TcpListener).EndAcceptSocket(ar);
            realm.threadManager.Set();

            clientSocket.BeginReceive(DataBuffer, 0, DataBuffer.Length, SocketFlags.None, Receive, clientSocket);
        }

        public void Receive(IAsyncResult ar)
        {
            try
            {
                var handler = ar.AsyncState as Socket;
                var recievedBytes = clientSocket.EndReceive(ar);

                if (recievedBytes != 0)
                {
                    HandleRealmData(DataBuffer);

                    clientSocket.BeginReceive(DataBuffer, 0, DataBuffer.Length, SocketFlags.None, Receive, handler);
                }
                else
                    clientSocket.Close();
            }
            catch (Exception ex)
            {
                Log.Message(LogType.Error, "{0}", ex.Message);
            }
        }

        public void Send(PacketWriter packet)
        {
            if (packet == null)
                return;

            DataBuffer = packet.ReadDataToSend(true);

            try
            {
                clientSocket.Send(DataBuffer, DataBuffer.Length, SocketFlags.None);
                packet.Flush();
            }
            catch (SocketException ex)
            {
                Log.Message(LogType.Error, "{0}", ex.Message);
                Log.Message();

                clientSocket.Close();
            }
        }
    }
}
