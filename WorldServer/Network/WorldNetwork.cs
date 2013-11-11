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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Framework.Logging;

namespace WorldServer.Network
{
    public class WorldNetwork
    {
        public volatile bool listenSocket = true;
        TcpListener listener;

        public bool Start(string host, int port)
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse(host), port);
                listener.Start();

                return true;
            }
            catch (Exception e)
            {
                Log.Message(LogType.Error, "{0}", e.Message);
                Log.Message();

                return false;
            }
        }

        public void AcceptConnectionThread()
        {
            new Thread(AcceptConnection).Start();
        }
 
        async void AcceptConnection()
        {
            while (listenSocket)
            {
                Thread.Sleep(1);

                if (listener.Pending())
                {
                    WorldClass worldClient = new WorldClass();
                    worldClient.clientSocket = await listener.AcceptSocketAsync();

                    worldClient.OnConnect();
                }
            }
        }
    }
}
