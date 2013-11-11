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
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Framework.Logging.PacketLogging
{
    public class PacketLog
    {
        static TextWriter logWriter;
        static Object syncObj = new Object();

        public static void WritePacket(string clientInfo, PacketWriter serverPacket = null, PacketReader clientPacket = null)
        {
            lock (syncObj)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();

                    if (serverPacket != null)
                    {
                        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "Client: {0}", clientInfo));
                        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "Time: {0}", DateTime.Now.ToString()));

                        if (Enum.IsDefined(typeof(ServerMessage), serverPacket.Opcode))
                        {
                            sb.AppendLine("Type: ServerMessage");
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "Name: {0}", Enum.GetName(typeof(ServerMessage), serverPacket.Opcode)));
                        }

                        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "Value: 0x{0:X} ({1})", serverPacket.Opcode, serverPacket.Opcode));
                        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "Length: {0}", serverPacket.Size - 2));

                        sb.AppendLine("|----------------------------------------------------------------|");
                        sb.AppendLine("| 00  01  02  03  04  05  06  07  08  09  0A  0B  0C  0D  0E  0F |");
                        sb.AppendLine("|----------------------------------------------------------------|");
                        sb.Append("|");

                        if (serverPacket.Size - 2 != 0)
                        {
                            var data = serverPacket.ReadDataToSend().ToList();
                            data.RemoveRange(0, 4);

                            byte count = 0;
                            data.ForEach(b =>
                            {
                                sb.Append(string.Format(CultureInfo.InvariantCulture, " {0:X2} ", b));

                                if (count == 15)
                                {
                                    sb.Append("|");
                                    sb.AppendLine();
                                    sb.Append("|");
                                    count = 0;
                                }
                                else
                                    count++;
                            });

                            sb.AppendLine("");
                            sb.AppendLine("|----------------------------------------------------------------|");
                        }

                        sb.AppendLine("");
                    }

                    if (clientPacket != null)
                    {
                        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "Client: {0}", clientInfo));
                        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "Time: {0}", DateTime.Now.ToString()));

                        sb.AppendLine("Type: ClientMessage");

                        if (Enum.IsDefined(typeof(ClientMessage), clientPacket.Opcode))
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "Name: {0}", clientPacket.Opcode));
                        else
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "Name: {0}", "Unknown"));

                        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "Value: 0x{0:X} ({1})", (ushort)clientPacket.Opcode, (ushort)clientPacket.Opcode));
                        sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "Length: {0}", clientPacket.Size));

                        sb.AppendLine("|----------------------------------------------------------------|");
                        sb.AppendLine("| 00  01  02  03  04  05  06  07  08  09  0A  0B  0C  0D  0E  0F |");
                        sb.AppendLine("|----------------------------------------------------------------|");
                        sb.Append("|");

                        if (clientPacket.Size - 2 != 0)
                        {
                            var data = clientPacket.Storage.ToList();

                            byte count = 0;
                            data.ForEach(b =>
                            {
                                sb.Append(string.Format(CultureInfo.InvariantCulture, " {0:X2} ", b));

                                if (count == 15)
                                {
                                    sb.Append("|");
                                    sb.AppendLine();
                                    sb.Append("|");
                                    count = 0;
                                }
                                else
                                    count++;
                            });

                            sb.AppendLine();
                            sb.Append("|----------------------------------------------------------------|");
                        }

                        sb.AppendLine("");
                    }

                    logWriter = TextWriter.Synchronized(File.AppendText("Packet.log"));
                    logWriter.WriteLine(sb.ToString());
                    logWriter.Flush();
                }
                finally
                {
                    logWriter.Close();
                }
            }
        }
    }
}
