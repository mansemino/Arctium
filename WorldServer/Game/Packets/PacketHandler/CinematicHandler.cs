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

using System.Linq;
using Framework.ClientDB;
using Framework.Constants.NetMessage;
using Framework.Database;
using Framework.Network.Packets;
using WorldServer.Game.WorldEntities;
using WorldServer.Network;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class CinematicHandler
    {
        public static void HandleStartCinematic(ref WorldClass session)
        {
            Character pChar = session.Character;

            PacketWriter startCinematic = new PacketWriter(ServerMessage.StartCinematic);

            startCinematic.WriteUInt32(CliDB.ChrRaces.Single(race => race.Id == pChar.Race).CinematicSequence);

            session.Send(ref startCinematic);

            if (pChar.LoginCinematic)
                DB.Characters.Execute("UPDATE characters SET loginCinematic = 0 WHERE guid = ?", pChar.Guid);
        }
    }
}
