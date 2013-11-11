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

using Framework.Constants.NetMessage;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace Framework.Network.Packets
{
    public class PacketWriter : BinaryWriter
    {
        public ServerMessage Opcode { get; set; }
        public uint Size { get; set; }

        public PacketWriter() : base(new MemoryStream()) { }
        public PacketWriter(ServerMessage message, bool isWorldPacket = false) : base(new MemoryStream())
        {
            WritePacketHeader(message, isWorldPacket);
        }

        protected void WritePacketHeader(ServerMessage opcode, bool isWorldPacket = false)
        {
            Opcode = opcode;

            WriteUInt8(0);
            WriteUInt8(0);
            WriteUInt8((byte)(0xFF & (ushort)opcode));
            WriteUInt8((byte)(0xFF & ((ushort)opcode >> 8)));

            if (isWorldPacket)
            {
                WriteUInt8(0);
                WriteUInt8(0);
            }
        }

        public byte[] ReadDataToSend(bool isAuthPacket = false)
        {
            byte[] data = new byte[BaseStream.Length];
            Seek(0, SeekOrigin.Begin);

            for (int i = 0; i < BaseStream.Length; i++)
                data[i] = (byte)BaseStream.ReadByte();

            Size = (uint)data.Length - 2;
            if (!isAuthPacket)
            {
                data[0] = (byte)(0xFF & Size);
                data[1] = (byte)(0xFF & (Size >> 8));

                if (Size > 0x7FFF)
                {
                    Seek(0, SeekOrigin.Begin);

                    byte bigSize = (byte)(0x80 | (0xFF & (Size >> 16)));
                    WriteUInt8(bigSize);
                }
            }
           
            return data;
        }

        public void WriteInt8(sbyte data)
        {
            base.Write(data);
        }

        public void WriteInt16(short data)
        {
            base.Write(data);
        }

        public void WriteInt32(int data)
        {
            base.Write(data);
        }

        public void WriteInt64(long data)
        {
            base.Write(data);
        }

        public void WriteUInt8(byte data)
        {
            base.Write(data);
        }

        public void WriteUInt16(ushort data)
        {
            base.Write(data);
        }

        public void WriteUInt32(uint data)
        {
            base.Write(data);
        }

        public void WriteUInt64(ulong data)
        {
            base.Write(data);
        }

        public void WriteFloat(float data)
        {
            base.Write(data);
        }

        public void WriteDouble(double data)
        {
            base.Write(data);
        }

        public void WriteCString(string data)
        {
            byte[] sBytes = UTF8Encoding.UTF8.GetBytes(data);

            WriteBytes(sBytes);
            WriteUInt8(0);
        }

        public void WriteString(string data, bool nullIfEmpty = true)
        {
            byte[] sBytes = UTF8Encoding.UTF8.GetBytes(data);

            if (sBytes.Length == 0 && nullIfEmpty)
                sBytes = new byte[1];
            
            WriteBytes(sBytes);
        }

        public void WriteUnixTime()
        {
            DateTime baseDate = new DateTime(1970, 1, 1);
            DateTime currentDate = DateTime.Now;
            TimeSpan ts = currentDate - baseDate;

            WriteUInt32(Convert.ToUInt32(ts.TotalSeconds));
        }

        public void WritePackedTime()
        {
            DateTime currentDate = DateTime.Now;

            WriteUInt32(Convert.ToUInt32((currentDate.Year - 100) << 24 | currentDate.Month << 20 | (currentDate.Day - 1) << 14 | (int)currentDate.DayOfWeek << 11 | currentDate.Hour << 6 | currentDate.Minute));
        }

        public void WriteGuid(ulong guid)
        {
            byte[] packedGuid = new byte[9];
            byte length = 1;

            for (byte i = 0; guid != 0; i++)
            {
                if ((guid & 0xFF) != 0)
                {
                    packedGuid[0] |= (byte)(1 << i);
                    packedGuid[length] = (byte)(guid & 0xFF);
                    ++length;
                }

                guid >>= 8;
            }

            WriteBytes(packedGuid, length);
        }

        public void WriteBytes(byte[] data, int count = 0)
        {
            if (count == 0)
                base.Write(data);
            else
                base.Write(data, 0, count);
        }

        public void WriteBitArray(BitArray buffer, int len)
        {
            var bufferArray = new byte[Convert.ToByte((buffer.Length + 8) / 8) + 1];

            if (len > bufferArray.Length)
                bufferArray = bufferArray.Concat(new byte[len - bufferArray.Length]).ToArray();

            buffer.CopyTo(bufferArray, 0);

            WriteBytes(bufferArray.ToArray(), len);
        }

        public void WriteUInt32Pos(uint data, int pos)
        {
            Seek(pos, SeekOrigin.Begin);

            WriteUInt32(data);

            Seek((int)BaseStream.Length - 1, SeekOrigin.Begin);
        }
    }
}
