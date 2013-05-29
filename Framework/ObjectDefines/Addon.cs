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

using Framework.Constants;
using System;

namespace Framework.ObjectDefines
{
    public class Addon
    {
        public byte AuthType { get; set; }          // 0, 1 or 2; Usually 2 (1 byte)
        public byte Enabled { get; set; }           // Obvious (1 byte)
        public byte HasPUBData { get; set; }        // Use/receive PUB data (1 byte)
        public byte HasUrlString { get; set; }      // Client sends an url string (1 byte)
        public byte Version { get; set; }           // Version to highlight button on new addons (1 byte)
        public uint CRC { get; set; }               // The CRC for the addon (4 bytes)
        public string UrlString { get; set; }       // The url string for the addon. 256 bytes
        public byte [] PUBData { get; set; }        // PUB data to send. 256 bytes
                                                    // Each addon block weights 524 bytes on client
        public short Id { get; set; }               // Id into DB for the addon readed
    }
}
