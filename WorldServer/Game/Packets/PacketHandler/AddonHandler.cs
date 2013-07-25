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
using Framework.Constants.NetMessage;
using Framework.Cryptography;
using Framework.Network.Packets;
using WorldServer.Network;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class AddonHandler : Globals
    {

        public static void ReadAddonData(byte[] buffer, int size, ref WorldClass session)
        {
            // Clean possible addon data
            session.Addons.Clear();

            string addonName;
            byte addonEnabled;
            uint addonCRC, addonVersion;

            // Decompress received Addon Data            
            PacketReader addonData = new PacketReader(ZLib.ZLibDecompress(buffer, true, size), false);

            // Get Addon number
            int ctr, numAddons = addonData.Read<int>();

            // For each addon, read data from decoded packet and store into session for later use if needed
            for (ctr = 0; ctr < numAddons; ctr++)
            {
                addonName       = addonData.ReadCString();
                addonEnabled    = addonData.Read<byte>();
                addonCRC        = addonData.Read<uint>();
                addonVersion    = addonData.Read<uint>();

                // Get the addon with same properties
                var Addon = AddonMgr.GetAddon(addonName, addonEnabled, addonCRC, (byte)addonVersion);

                // Add if found, add default one if not.
                if (Addon != null)
                    session.Addons.Add(Addon);
                else
                    // Note: This can be skipped for those with no addon data.
                    // TODO: Test that!
                    session.Addons.Add(new Framework.ObjectDefines.Addon()
                    {
                        AuthType        = 2,
                        Enabled         = addonEnabled,
                        CRC             = addonCRC,
                        HasPUBData      = 0,
                        PUBData         = null,
                        Version         = (byte)addonVersion,
                        HasUrlString    = 0,
                        UrlString       = addonName
                    });
            }

            int addonEnd = addonData.Read<int>(); // Unknown
        }
        public static void WriteAddonData(ref WorldClass session)
        {
            PacketWriter addonInfo = new PacketWriter(ServerMessage.AddonInfo);
            BitPack BitPack = new BitPack(addonInfo);

            int AddOnCount = session.Addons.Count;

            BitPack.Write<int>(0, 18);
            BitPack.Write<int>(AddOnCount, 23);

            session.Addons.ForEach(addon =>
            {
                BitPack.Write<bool>(addon.Enabled != 0);
                BitPack.Write<bool>(addon.HasPUBData != 0);
                BitPack.Write<bool>(addon.HasUrlString != 0);
                if (addon.HasUrlString != 0)
                    BitPack.Write<int>(addon.UrlString.Length, 8);
            });

            BitPack.Flush();

            // Send Addon stored data for session
            session.Addons.ForEach(addon =>
            {
                // atm not url string
                if (addon.HasUrlString != 0x00)
                    addonInfo.WriteString(addon.UrlString, false);

                if (addon.HasPUBData != 0x00)
                    addonInfo.WriteBytes(addon.PUBData, addon.PUBData.Length);

                if (addon.Enabled != 0x00)
                {
                    addonInfo.WriteUInt32(addon.Version);
                    addonInfo.WriteUInt8(addon.Enabled);
                }

                addonInfo.WriteUInt8(addon.AuthType);
            });

            session.Send(ref addonInfo);

        }
    }
}
