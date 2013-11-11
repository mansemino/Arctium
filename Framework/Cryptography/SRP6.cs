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
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Framework.Cryptography
{
    public class SRP6 : IDisposable
    {
        public byte[] B { get; private set; }
        public byte[] K { get; private set; }
        public byte[] M2 { get; private set; }

        public readonly BigInteger g;
        public readonly BigInteger k;

        public readonly byte[] N;
        public readonly byte[] Salt;

        protected readonly SHA1 Sha1;
        protected BigInteger A;
        protected BigInteger BN;
        protected BigInteger v;
        protected BigInteger b;
        protected BigInteger s;

        public SRP6()
        {
            Sha1 = new SHA1Managed();

            N = new byte[]
            {
                0x89, 0x4B, 0x64, 0x5E, 0x89, 0xE1, 0x53, 0x5B, 0xBD, 0xAD, 0x5B, 0x8B, 0x29, 0x06, 0x50, 0x53,
                0x08, 0x01, 0xB1, 0x8E, 0xBF, 0xBF, 0x5E, 0x8F, 0xAB, 0x3C, 0x82, 0x87, 0x2A, 0x3E, 0x9B, 0xB7,
            };

            Salt = new byte[]
            {
                0xAD, 0xD0, 0x3A, 0x31, 0xD2, 0x71, 0x14, 0x46, 0x75, 0xF2, 0x70, 0x7E, 0x50, 0x26, 0xB6, 0xD2,
                0xF1, 0x86, 0x59, 0x99, 0x76, 0x02, 0x50, 0xAA, 0xB9, 0x45, 0xE0, 0x9E, 0xDD, 0x2A, 0xA3, 0x45
            };

            BN = MakeBigInteger(N);
            g = MakeBigInteger(new byte[] { 7 });
            k = MakeBigInteger(new byte[] { 3 });
        }

        public void CalculateX(string userName, string password)
        {
            var p = Encoding.UTF8.GetBytes(userName + ":" + password);
            var x = MakeBigInteger(Sha1.ComputeHash(CombineData(Salt, Sha1.ComputeHash(p))));

            CalculateV(x);
        }

        void CalculateV(BigInteger x)
        {
            v = BigInteger.ModPow(g, x, BN);

            CalculateB(); 
        }

        void CalculateB()
        {
            var randBytes = new byte[20];

            var random = RNGCryptoServiceProvider.Create();
            random.GetBytes(randBytes);

            b = MakeBigInteger(randBytes);
            B = GetBytes(((k * v + BigInteger.ModPow(g, b, BN)) % BN).ToByteArray());
        }

        public void CalculateU(byte[] a)
        {
            A = MakeBigInteger(a);

            CalculateS(MakeBigInteger(Sha1.ComputeHash(CombineData(a, B))));
        }

        void CalculateS(BigInteger u)
        {
            s = BigInteger.ModPow(A * BigInteger.ModPow(v, u, BN), b, BN);

            CalculateK();
        }

        public void CalculateK()
        {
            var sBytes = GetBytes(s.ToByteArray());

            var part1 = new byte[sBytes.Length / 2];
            var part2 = new byte[sBytes.Length / 2];

            for (int i = 0; i < part1.Length; i++)
            {
                part1[i] = sBytes[i * 2];
                part2[i] = sBytes[i * 2 + 1];
            }

            part1 = Sha1.ComputeHash(part1);
            part2 = Sha1.ComputeHash(part2);

            K = new byte[part1.Length + part2.Length];

            for (int i = 0; i < part1.Length; i++)
            {
                K[i * 2] = part1[i];
                K[i * 2 + 1] = part2[i];
            }
        }

        public void CalculateM2(byte[] m1)
        {
            M2 = Sha1.ComputeHash(CombineData(CombineData(GetBytes(A.ToByteArray()), m1), K));
        }

        public byte[] GetBytes(byte[] data, int count = 32)
        {
            if (data.Length == count)
                return data;

            var bytes = new byte[count];

            Buffer.BlockCopy(data, 0, bytes, 0, 32);

            return bytes;
        }

        public BigInteger MakeBigInteger(byte[] data)
        {
            return new BigInteger(CombineData(data, new byte[] { 0 }));
        }

        public byte[] CombineData(byte[] data, byte[] data2)
        {
            return new byte[0].Concat(data).Concat(data2).ToArray();
        }

        public void Dispose()
        {
            K = null;
            M2 = null;
        }
    }
}
