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
using Framework.Network.Packets;
using Framework.Singleton;
using WorldServer.Network;

using Framework.Logging;
using System;
using Framework.Database;
using Framework.Cryptography;

using System.IO;
using System.Text;

namespace WorldServer.Game.Managers
{
    public partial class AddonManager : SingletonBase<AddonManager>
    {
        AddonManager() { }

        public void WriteAddonData(ref WorldClass session)
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

        public void ReadAddonData(byte[] buffer, int size, ref WorldClass session)
        {
            // Clean possible addon data
            session.Addons.Clear();
       
            SQLResult result;
            string addonName;
            byte addonEnabled, usePUBData, ctr;
            byte[] PUBData;
            uint addonCRC, addonVersion, storedCRC, storedVersion;

            // Decompress received Addon Data            
            PacketReader addonData = new PacketReader(ZLib.ZLibDecompress(buffer, true, size), false);

            // Get Addon number
            int numAddons = addonData.ReadInt32();

            // Debug Info
            // Log.Message(LogType.Debug, "Received data for {0} addons...", numAddons);

            // For each addon, read data from decoded packet and store into session for later use if needed
            for (ctr = 0; ctr < numAddons; ctr++)
            {
                addonName       = addonData.ReadCString();
                addonEnabled    = addonData.ReadByte();
                addonCRC        = addonData.ReadUInt32();
                addonVersion    = addonData.ReadUInt32();

                // Get data from DB and check if all is right
                result = DB.Characters.Select("SELECT * FROM addons WHERE Name=?", addonName);

                // If addon doesn't exist into DB, disable it and store default data
                if (result.Count != 1)
                {
                    session.Addons.Add(new Framework.ObjectDefines.Addon()
                    {
                        Id              = 0,
                        AuthType        = 2,
                        Enabled         = 0,
                        CRC             = addonCRC,
                        HasPUBData      = 0,
                        PUBData         = null,
                        Version         = (byte)addonVersion,
                        HasUrlString    = 0,
                        UrlString       = "",
                    });
                    Log.Message(LogType.Debug, "{0}: Unknown Addon (CRC {1}).", addonName, addonCRC);
                }
                else
                {
                    // Check if addon disabled. If so, take real value from DB
                    if (addonEnabled == 0x01)
                        addonEnabled = result.Read<byte>(0, "enabled");

                    // Check if CRC is Ok
                    storedCRC = result.Read<uint>(0, "CRC");
                    if (addonCRC != storedCRC)
                    {
                        addonEnabled = 0;
                        Log.Message(LogType.Debug, "{0}: Addon with wrong CRC (stored: {1}; Got: {2}).", addonName, storedCRC, addonCRC);
                    }

                    // Check if Change in Version is Ok
                    storedVersion = result.Read<uint>(0, "Version");
                    if (addonVersion != storedVersion)
                    {
                        addonEnabled = 0;
                        Log.Message(LogType.Debug, "{0}: Addon with wrong Version (stored: {1}; Got: {2}).", addonName, storedVersion, addonVersion);
                    }

                    // Get PUB data if needed
                    usePUBData = result.Read<byte>(0, "Use_PUB");
                    if (usePUBData == 0x01)
                    {
                        PUBData = result.Read<byte[]>(0, "PUB_Data");
                    }
                    else
                    {
                        PUBData = null;
                    }

                    session.Addons.Add(new Framework.ObjectDefines.Addon()
                    {
                        Id              = result.Read<short>(0, "Index"),
                        AuthType        = result.Read<byte>(0, "Auth_Type"),
                        Enabled         = addonEnabled,
                        CRC             = addonCRC,
                        HasPUBData      = usePUBData,
                        PUBData         = PUBData,
                        Version         = (byte)result.Read<uint>(0, "Version"),
                        HasUrlString    = result.Read<byte>(0, "Has_Url_String"),
                        UrlString       = result.Read<string>(0, "Url_String"),
                    });
                }
            }
            int addonEnd = addonData.ReadInt32(); // Unknown
        }
    }
}
