﻿/*
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
using System.Collections;
using System.Net;
using System.Net.Sockets;
using Framework.Constants.NetMessage;
using Framework.Cryptography;
using Framework.Database;
using Framework.Logging;
using Framework.Logging.PacketLogging;
using Framework.Network.Packets;
using Framework.ObjectDefines;
using WorldServer.Game;
using WorldServer.Game.Packets;
using WorldServer.Game.WorldEntities;
using System.Collections.Generic;

namespace WorldServer.Network
{
    public sealed class WorldClass : IDisposable
    {
        public Account Account { get; set; }
        public List<Addon> Addons = new List<Addon>();
        public Character Character { get; set; }
        public static WorldNetwork world;
        public Socket clientSocket;
        public Queue PacketQueue;
        public PacketCrypt Crypt;
        byte[] DataBuffer;

        public WorldClass()
        {
            DataBuffer = new byte[8192];
            PacketQueue = new Queue();
            Crypt = new PacketCrypt();
        }

        public void OnData()
        {
            PacketReader packet = null;
            if (PacketQueue.Count > 0)
                packet = (PacketReader)PacketQueue.Dequeue();
            else
                packet = new PacketReader(DataBuffer);

            string clientInfo = ((IPEndPoint)clientSocket.RemoteEndPoint).Address + ":" + ((IPEndPoint)clientSocket.RemoteEndPoint).Port;
            PacketLog.WritePacket(clientInfo, null, packet);

            PacketManager.InvokeHandler(ref packet, this);
        }

        public void OnConnect()
        {
            PacketWriter TransferInitiate = new PacketWriter(ServerMessage.TransferInitiate);
            TransferInitiate.WriteCString("RLD OF WARCRAFT CONNECTION - SERVER TO CLIENT");

            Send(ref TransferInitiate);

            clientSocket.BeginReceive(DataBuffer, 0, DataBuffer.Length, SocketFlags.None, Receive, null);
        }

        public void Receive(IAsyncResult result)
        {
            try
            {
                var recievedBytes = clientSocket.EndReceive(result);
                if (recievedBytes != 0)
                {
                    if (Crypt.IsInitialized)
                    {
                        while (recievedBytes > 0)
                        {
                            Decode(ref DataBuffer);

                            var length = BitConverter.ToUInt16(DataBuffer, 0) + 4;

                            var packetData = new byte[length];
                            Buffer.BlockCopy(DataBuffer, 0, packetData, 0, length);

                            PacketReader packet = new PacketReader(packetData);
                            PacketQueue.Enqueue(packet);

                            recievedBytes -= length;
                            Buffer.BlockCopy(DataBuffer, length, DataBuffer, 0, recievedBytes);

                            OnData();
                        }
                    }
                    else
                        OnData();

                    clientSocket.BeginReceive(DataBuffer, 0, DataBuffer.Length, SocketFlags.None, Receive, null);
                }
            }
            catch (Exception ex)
            {
                Log.Message(LogType.Error, "{0}", ex.Message);

                if (Character != null)
                    Globals.WorldMgr.DeleteSession(Character.Guid);

                if (Account != null)
                    DB.Realms.Execute("UPDATE accounts SET online = 0 WHERE id = ?", Account.Id);
            }
        }

        void Decode(ref byte[] data)
        {
            Crypt.Decrypt(data);

            var header = BitConverter.ToUInt32(data, 0);
            ushort size = (ushort)(header >> 13);
            ushort opcode = (ushort)(header & 0x1FFF);

            data[0] = (byte)(0xFF & size);
            data[1] = (byte)(0xFF & (size >> 8));
            data[2] = (byte)(0xFF & opcode);
            data[3] = (byte)(0xFF & (opcode >> 8));
        }

        public void Send(ref PacketWriter packet)
        {
            if (packet.Opcode == 0)
                return;

            var buffer = packet.ReadDataToSend();

            try
            {
                if (Crypt.IsInitialized)
                {
                    uint totalLength = (uint)packet.Size - 2;
                    totalLength <<= 13;
                    totalLength |= ((uint)packet.Opcode & 0x1FFF);

                    var header = BitConverter.GetBytes(totalLength);

                    Crypt.Encrypt(header);

                    buffer[0] = header[0];
                    buffer[1] = header[1];
                    buffer[2] = header[2];
                    buffer[3] = header[3];
                }

                clientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);

                string clientInfo = ((IPEndPoint)clientSocket.RemoteEndPoint).Address + ":" + ((IPEndPoint)clientSocket.RemoteEndPoint).Port;
                PacketLog.WritePacket(clientInfo, packet);

                packet.Flush();
            }
            catch (Exception ex)
            {
                Log.Message(LogType.Error, "{0}", ex.Message);
                Log.Message();

                clientSocket.Close();
            }
        }

        public void Dispose()
        {
            Crypt.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
