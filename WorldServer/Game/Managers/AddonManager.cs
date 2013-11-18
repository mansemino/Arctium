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

// using Framework.Constants.NetMessage;
// using Framework.Network.Packets;
// using WorldServer.Network;
// using System;
using Framework.Database;
using Framework.Logging;
using Framework.ObjectDefines;
using Framework.Singleton;
using System.Collections.Concurrent;

namespace WorldServer.Game.Managers
{
    public partial class AddonManager : SingletonBase<AddonManager>
    {
        ConcurrentDictionary<string, Addon> Addons;

        AddonManager()
        {
            Addons = new ConcurrentDictionary<string, Addon>();
            LoadAddons();
        }

        public void LoadAddons()
        {
            Log.Message(LogType.DB, "Loading addon data...");

            SQLResult result = DB.Characters.Select("SELECT * FROM addons");

            for (int i = 0; i < result.Count; i++)
            {
                string Name = result.Read<string>(i, "Name");

                var addon = new Addon
                {
                    Version         = result.Read<byte>(i, "Version"),
                    CRC             = result.Read<uint>(i, "CRC"),
                    AuthType        = result.Read<byte>(i, "Auth_Type"),
                    Enabled         = result.Read<byte>(i, "Enabled"),
                    HasPUBData      = result.Read<byte>(i, "Use_PUB"),
                    PUBData         = null,
                    HasUrlString    = result.Read<byte>(i, "Has_Url_String"),
                    UrlString       = result.Read<string>(i, "Url_String"),
                    UrlStringCRC    = result.Read<uint>(i, "Url_String_CRC")
                };

                if (addon.HasPUBData == 0x01)
                    addon.PUBData = result.Read<byte[]>(i, "PUB_Data");

                Addons.TryAdd(Name, addon);
            }

            Log.Message(LogType.DB, "Loaded {0} addons.", Addons.Count);
            Log.Message();
        }

        public Addon GetAddon(string Name)
        {
            Addon addon = null;

            Addons.TryGetValue(Name, out addon);

            return addon;
        }

        public Addon GetAddon(string Name, byte Enabled, uint CRC, uint UrlStringCRC)
        {
            Addon addon = null;

            Addons.TryGetValue(Name, out addon);

            if (addon != null)
            {
                if ((addon.Enabled == Enabled) && (addon.CRC == CRC) && (addon.UrlStringCRC == UrlStringCRC))
                    return addon;
                else
                    return null;
            }

            return addon;
        }
    }
}
