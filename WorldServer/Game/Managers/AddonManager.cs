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

namespace WorldServer.Game.Managers
{
    public partial class AddonManager : SingletonBase<AddonManager>
    {
        AddonManager() { }

        public void WriteAddonData(ref WorldClass session)
        {
            PacketWriter addonInfo = new PacketWriter(ServerMessage.AddonInfo);

            // Default static value for now.
            // Full system will be implanted later.
            uint addonCount = 40;

            for (int i = 0; i < addonCount; i++)
            {
                addonInfo.WriteUInt8(2);
                addonInfo.WriteUInt8(1);
                addonInfo.WriteUInt8(0);
                addonInfo.WriteUInt32(0);
                addonInfo.WriteUInt8(0);
            }

            addonInfo.WriteUInt32(0);

            session.Send(ref addonInfo);
        }
    }
}
