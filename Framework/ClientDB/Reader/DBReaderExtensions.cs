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
using System.IO;
using System.Text;

namespace Framework.ClientDB.Reader
{
    public static class DBReaderExtensions
    {
        public static sbyte[] ReadSByte(this BinaryReader br, int count)
        {
            var arr = new sbyte[count];
            for (int i = 0; i < count; i++)
                arr[i] = br.ReadSByte();

            return arr;
        }

        public static byte[] ReadByte(this BinaryReader br, int count)
        {
            var arr = new byte[count];
            for (int i = 0; i < count; i++)
                arr[i] = br.ReadByte();

            return arr;
        }

        public static int[] ReadInt32(this BinaryReader br, int count)
        {
            var arr = new int[count];
            for (int i = 0; i < count; i++)
                arr[i] = br.ReadInt32();

            return arr;
        }

        public static uint[] ReadUInt32(this BinaryReader br, int count)
        {
            var arr = new uint[count];
            for (int i = 0; i < count; i++)
                arr[i] = br.ReadUInt32();

            return arr;
        }

        public static float[] ReadSingle(this BinaryReader br, int count)
        {
            var arr = new float[count];
            for (int i = 0; i < count; i++)
                arr[i] = br.ReadSingle();

            return arr;
        }

        public static long[] ReadInt64(this BinaryReader br, int count)
        {
            var arr = new long[count];
            for (int i = 0; i < count; i++)
                arr[i] = br.ReadInt64();

            return arr;
        }

        public static ulong[] ReadUInt64(this BinaryReader br, int count)
        {
            var arr = new ulong[count];
            for (int i = 0; i < count; i++)
                arr[i] = br.ReadUInt64();

            return arr;
        }

        public static string ReadCString(this BinaryReader br)
        {
            StringBuilder tmpString = new StringBuilder();
            char tmpChar = br.ReadChar();
            char tmpEndChar = Convert.ToChar(Encoding.UTF8.GetString(new byte[] { 0 }));

            while (tmpChar != tmpEndChar)
            {
                tmpString.Append(tmpChar);
                tmpChar = br.ReadChar();
            }

            return tmpString.ToString();
        }

        public static string ReadString(this BinaryReader br, int count)
        {
            byte[] stringArray = br.ReadBytes(count);
            return Encoding.ASCII.GetString(stringArray);
        }
    }
}
