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

using Ionic.Zlib;
using System;
using System.IO;

namespace Framework.Cryptography
{
    public class ZLib
    {
        public static byte[] ZLibDecompress(byte[] compressed, bool mode, int outputSize)
        {
            byte[] output = new Byte[outputSize];

            bool expectRfc1950Header = mode;

            using (MemoryStream ms = new MemoryStream())
            {
                ZlibCodec compressor = new ZlibCodec();
                compressor.InitializeInflate(expectRfc1950Header);

                compressor.InputBuffer = compressed;
                compressor.AvailableBytesIn = compressed.Length;
                compressor.NextIn = 0;
                compressor.OutputBuffer = output;

                foreach (var f in new FlushType[] { FlushType.None, FlushType.Finish })
                {
                    int bytesToWrite = 0;
                    do
                    {
                        compressor.AvailableBytesOut = outputSize;
                        compressor.NextOut = 0;
                        compressor.Inflate(f);

                        bytesToWrite = outputSize - compressor.AvailableBytesOut;
                        if (bytesToWrite > 0)
                            ms.Write(output, 0, bytesToWrite);
                    }
                    while ((f == FlushType.None && (compressor.AvailableBytesIn != 0 || compressor.AvailableBytesOut == 0)) ||
                           (f == FlushType.Finish && bytesToWrite != 0));
                }

                compressor.EndInflate();

                return (ms.ToArray());
            }
        }
    }
}
