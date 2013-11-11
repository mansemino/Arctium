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

using Framework.Configuration;
using Framework.Helper;
using Framework.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace Framework.ClientDB.Reader
{
    public class DBReader
    {
        public static List<T> Read<T>(string dbcFile) where T : new()
        {
            List<T> tempList = null;

            try
            {
                using (var dbReader = new BinaryReader(new MemoryStream(File.ReadAllBytes(WorldConfig.DataPath + "/dbc/" + dbcFile))))
                {
                    DBHeader header = new DBHeader
                    {
                        Signature       = dbReader.ReadString(4),
                        RecordCount     = dbReader.Read<uint>(),
                        FieldCount      = dbReader.Read<uint>(),
                        RecordSize      = dbReader.Read<uint>(),
                        StringBlockSize = dbReader.Read<uint>()
                    };

                    if (header.IsValidDb2File)
                    {
                        var hash     = dbReader.Read<uint>();
                        var wowBuild = dbReader.Read<uint>();
                        var unknown  = dbReader.Read<uint>();
                        var min      = dbReader.Read<int>();
                        var max      = dbReader.Read<int>();
                        var locale   = dbReader.Read<int>();
                        var unknown2 = dbReader.Read<int>();

                        if (max != 0)
                        {
                            var diff = (max - min) + 1;

                            dbReader.ReadBytes(diff * 4);
                            dbReader.ReadBytes(diff * 2);
                        }
                    }

                    if (header.IsValidDbcFile || header.IsValidDb2File)
                    {
                        tempList = new List<T>();
                        var fields = typeof(T).GetFields();
                        var lastStringOffset = 0;
                        var lastString = "";

                        for (int i = 0; i < header.RecordCount; i++)
                        {
                            T newObj = new T();

                            foreach (var f in fields)
                            {
                                switch (f.FieldType.Name)
                                {
                                    case "SByte":
                                        f.SetValue(newObj, dbReader.ReadSByte());
                                        break;
                                    case "Byte":
                                        f.SetValue(newObj, dbReader.ReadByte());
                                        break;
                                    case "Int32":
                                        f.SetValue(newObj, dbReader.ReadInt32());
                                        break;
                                    case "UInt32":
                                        f.SetValue(newObj, dbReader.ReadUInt32());
                                        break;
                                    case "Int64":
                                        f.SetValue(newObj, dbReader.ReadInt64());
                                        break;
                                    case "UInt64":
                                        f.SetValue(newObj, dbReader.ReadUInt64());
                                        break;
                                    case "Single":
                                        f.SetValue(newObj, dbReader.ReadSingle());
                                        break;
                                    case "Boolean":
                                        f.SetValue(newObj, dbReader.ReadBoolean());
                                        break;
                                    case "SByte[]":
                                        f.SetValue(newObj, dbReader.ReadSByte(((sbyte[])f.GetValue(newObj)).Length));
                                        break;
                                    case "Byte[]":
                                        f.SetValue(newObj, dbReader.ReadByte(((byte[])f.GetValue(newObj)).Length));
                                        break;
                                    case "Int32[]":
                                        f.SetValue(newObj, dbReader.ReadInt32(((int[])f.GetValue(newObj)).Length));
                                        break;
                                    case "UInt32[]":
                                        f.SetValue(newObj, dbReader.ReadUInt32(((uint[])f.GetValue(newObj)).Length));
                                        break;
                                    case "Single[]":
                                        f.SetValue(newObj, dbReader.ReadSingle(((float[])f.GetValue(newObj)).Length));
                                        break;
                                    case "Int64[]":
                                        f.SetValue(newObj, dbReader.ReadInt64(((long[])f.GetValue(newObj)).Length));
                                        break;
                                    case "UInt64[]":
                                        f.SetValue(newObj, dbReader.ReadUInt64(((ulong[])f.GetValue(newObj)).Length));
                                        break;
                                    case "String":
                                    {
                                        var stringOffset = dbReader.ReadUInt32();

                                        if (stringOffset != lastStringOffset)
                                        {
                                            var currentPos = dbReader.BaseStream.Position;
                                            var stringStart = (header.RecordCount * header.RecordSize) + 20 + stringOffset;
                                            dbReader.BaseStream.Seek(stringStart, 0);

                                            f.SetValue(newObj, lastString = dbReader.ReadCString());

                                            dbReader.BaseStream.Seek(currentPos, 0);
                                        }
                                        else
                                            f.SetValue(newObj, lastString);

                                        break;
                                    }
                                    default:
                                        dbReader.BaseStream.Position += 4;
                                        break;
                                }
                            }

                            tempList.Add(newObj);
                        }
                    }
                }

                ++CliDB.Count;
            }
            catch (Exception ex)
            {
                Log.Message(LogType.Error, "Error while loading {0}: {1}", dbcFile, ex.Message);
            }

            return tempList;
        }
    }
}
