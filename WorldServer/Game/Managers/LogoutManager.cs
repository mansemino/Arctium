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
using System.Collections.Concurrent;
using System.Threading;
using WorldServer.Network;
using System.Threading.Tasks;
using System;

namespace WorldServer.Game.Managers
{
    public sealed class LogoutManager : SingletonBase<LogoutManager>
    {
        static readonly object taskObject = new object();
        static ConcurrentDictionary<ulong, DateTime> registeredRequests = new ConcurrentDictionary<ulong, DateTime>();

        LogoutManager()
        {
            StartCheckLogoutTimers();
        }

        public void StartCheckLogoutTimers()
        {
            var updateTask = new Thread(UpdateTask);
            updateTask.IsBackground = true;
            updateTask.Start();
        }

        void UpdateTask()
        {
            while (true)
            {
                lock (taskObject)
                {
                    Thread.Sleep(1000);

                    if (registeredRequests.Count > 0)
                    {
                        Parallel.ForEach(registeredRequests, r =>
                        {
                            if (r.Value <= DateTime.Now)
                            {
                                var sess = Globals.WorldMgr.GetSession(r.Key);
                                if (sess != null)
                                    LogOut(ref sess);
                            }
                        });
                    }
                }
            }
        }

        public void Add(ulong _guid)
        {
            registeredRequests.TryAdd(_guid, DateTime.Now.AddSeconds(20));
        }


        public void Remove(ulong _guid)
        {
            DateTime time;
            registeredRequests.TryRemove(_guid, out time);
        }

        public void LogOut(ref WorldClass session)
        {
            var pChar = session.Character;

            Remove(pChar.Guid);

            Globals.ObjectMgr.SavePositionToDB(pChar);

            PacketWriter logoutComplete = new PacketWriter(ServerMessage.LogoutComplete);    
            session.Send(ref logoutComplete);

            // Destroy object after logout
            Globals.WorldMgr.SendToInRangeCharacter(pChar, Packets.PacketHandler.ObjectHandler.HandleDestroyObject(ref session, pChar.Guid));
            Globals.WorldMgr.DeleteSession(pChar.Guid);
        }     
    
    }
}
