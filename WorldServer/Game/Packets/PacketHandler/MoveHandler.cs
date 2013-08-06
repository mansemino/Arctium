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
using Framework.Constants.Movement;
using Framework.Constants.NetMessage;
using Framework.Network.Packets;
using Framework.ObjectDefines;
using WorldServer.Game.WorldEntities;
using WorldServer.Network;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class MoveHandler : Globals
    {
        [Opcode(ClientMessage.MoveStartForward,         "17128")]
        [Opcode(ClientMessage.MoveStartBackward,        "17128")]
        [Opcode(ClientMessage.MoveStop,                 "17128")]
        [Opcode(ClientMessage.MoveStartStrafeLeft,      "17128")]
        [Opcode(ClientMessage.MoveStartStrafeRight,     "17128")]
        [Opcode(ClientMessage.MoveStopStrafe,           "17128")]
        [Opcode(ClientMessage.MoveJump,                 "17128")]
        [Opcode(ClientMessage.MoveStartTurnLeft,        "17128")]
        [Opcode(ClientMessage.MoveStartTurnRight,       "17128")]
        [Opcode(ClientMessage.MoveStopTurn,             "17128")]
        [Opcode(ClientMessage.MoveStartPitchUp,         "17128")]
        [Opcode(ClientMessage.MoveStartPitchDown,       "17128")]
        [Opcode(ClientMessage.MoveStopPitch,            "17128")]
        [Opcode(ClientMessage.MoveSetRunMode,           "17128")]
        [Opcode(ClientMessage.MoveSetWalkMode,          "17128")]
        [Opcode(ClientMessage.MoveFallLand,             "17128")]
        [Opcode(ClientMessage.MoveStartSwim,            "17128")]
        [Opcode(ClientMessage.MoveStopSwim,             "17128")]
        [Opcode(ClientMessage.MoveToggleCollisionCheat, "17128")]
        [Opcode(ClientMessage.MoveSetFacing,            "17128")]
        [Opcode(ClientMessage.MoveSetPitch,             "17128")]
        [Opcode(ClientMessage.MoveHeartbeat,            "17128")]
        [Opcode(ClientMessage.MoveFallReset,            "17128")]
        [Opcode(ClientMessage.MoveSetFly,               "17128")]
        [Opcode(ClientMessage.MoveStartAscend,          "17128")]
        [Opcode(ClientMessage.MoveStopAscend,           "17128")]
        [Opcode(ClientMessage.MoveChangeTransport,      "17128")]
        [Opcode(ClientMessage.MoveStartDescend,         "17128")]
        [Opcode(ClientMessage.MoveDismissVehicle,       "17128")]
        public static void HandlePlayerMove(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            Vector4 vector = new Vector4()
            {
                Y = packet.Read<float>(),
                Z = packet.Read<float>(),
                X = packet.Read<float>()
            };

            guidMask[3] = BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            guidMask[2] = BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();
            var Unknown2 = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();
            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();
            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            var Unknown3 = !BitUnpack.GetBit();
            var Unknown4 = BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();
            guidMask[1] = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();
            guidMask[5] = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (guidMask[0]) guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[4]) guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);
            if (guidMask[1]) guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);
            if (guidMask[5]) guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);
            if (guidMask[6]) guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);
            if (guidMask[2]) guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);
            if (guidMask[3]) guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);
            if (guidMask[7]) guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (HasPitch)
                packet.Read<float>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (movementValues.IsFallingOrJumping)
            {
                movementValues.JumpVelocity = packet.Read<float>();

                if (movementValues.HasJumpData)
                {
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                }

                movementValues.FallTime = packet.Read<uint>();
            }

            if (Unknown3)
                packet.Read<uint>();

            if (HasSplineElevation)
                packet.Read<float>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);

            // Clear emote if needed
            session.Character.setEmoteState();
        }

        [Opcode(ClientMessage.MoveForceRootAck, "17128")]
        [Opcode(ClientMessage.MoveForceUnrootAck, "17128")]
        public static void HandlePlayerMoveRoot(ref PacketReader packet, WorldClass session)
        {
            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guidMask = new bool[8];
            var guidBytes = new byte[8];

            var rootValue = packet.Read<UInt32>();

            Vector4 vector = new Vector4()
            {
                Y = packet.Read<float>(),
                Z = packet.Read<float>(),
                X = packet.Read<float>()
            };

            guidMask[3] = BitUnpack.GetBit();

            var HasPitch = !BitUnpack.GetBit();

            guidMask[0] = BitUnpack.GetBit();

            var counter = BitUnpack.GetBits<uint>(22);

            guidMask[2] = BitUnpack.GetBit();

            var HasSplineElevation = !BitUnpack.GetBit();

            movementValues.HasRotation = !BitUnpack.GetBit();

            var Unknown = BitUnpack.GetBit();
            var Unknown2 = BitUnpack.GetBit();

            guidMask[7] = BitUnpack.GetBit();

            var HasTime = !BitUnpack.GetBit();

            movementValues.IsFallingOrJumping = BitUnpack.GetBit();
            movementValues.HasMovementFlags2 = !BitUnpack.GetBit();
            movementValues.HasMovementFlags = !BitUnpack.GetBit();

            var Unknown3 = !BitUnpack.GetBit();
            var Unknown4 = BitUnpack.GetBit();

            guidMask[6] = BitUnpack.GetBit();
            guidMask[1] = BitUnpack.GetBit();

            movementValues.IsTransport = BitUnpack.GetBit();

            guidMask[4] = BitUnpack.GetBit();
            guidMask[5] = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags)
                movementValues.MovementFlags = (MovementFlag)BitUnpack.GetBits<uint>(30);

            if (movementValues.IsFallingOrJumping)
                movementValues.HasJumpData = BitUnpack.GetBit();

            if (movementValues.HasMovementFlags2)
                movementValues.MovementFlags2 = (MovementFlag2)BitUnpack.GetBits<uint>(13);

            if (guidMask[0]) guidBytes[0] = (byte)(packet.Read<byte>() ^ 1);

            for (int i = 0; i < counter; i++)
                packet.Read<uint>();

            if (guidMask[4]) guidBytes[4] = (byte)(packet.Read<byte>() ^ 1);
            if (guidMask[1]) guidBytes[1] = (byte)(packet.Read<byte>() ^ 1);
            if (guidMask[5]) guidBytes[5] = (byte)(packet.Read<byte>() ^ 1);
            if (guidMask[6]) guidBytes[6] = (byte)(packet.Read<byte>() ^ 1);
            if (guidMask[2]) guidBytes[2] = (byte)(packet.Read<byte>() ^ 1);
            if (guidMask[3]) guidBytes[3] = (byte)(packet.Read<byte>() ^ 1);
            if (guidMask[7]) guidBytes[7] = (byte)(packet.Read<byte>() ^ 1);

            if (HasPitch)
                packet.Read<float>();

            if (movementValues.HasRotation)
                vector.O = packet.Read<float>();

            if (movementValues.IsFallingOrJumping)
            {
                movementValues.JumpVelocity = packet.Read<float>();

                if (movementValues.HasJumpData)
                {
                    movementValues.CurrentSpeed = packet.Read<float>();
                    movementValues.Sin = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                }

                movementValues.FallTime = packet.Read<uint>();
            }

            if (Unknown3)
                packet.Read<uint>();

            if (HasSplineElevation)
                packet.Read<float>();

            if (HasTime)
                movementValues.Time = packet.Read<uint>();

            var guid = BitConverter.ToUInt64(guidBytes, 0);
            HandleMoveUpdate(guid, movementValues, vector);

            // Clear emote if needed
            session.Character.setEmoteState();
        }


        public static void HandleMoveUpdate(ulong guid, ObjectMovementValues movementValues, Vector4 vector)
        {
            PacketWriter moveUpdate = new PacketWriter(ServerMessage.MoveUpdate);
            BitPack BitPack = new BitPack(moveUpdate, guid);

            BitPack.WriteGuidMask(0);

            BitPack.Write(!movementValues.HasRotation);
            BitPack.Write(!movementValues.HasMovementFlags2);

            BitPack.WriteGuidMask(5);

            if (movementValues.HasMovementFlags2)
                BitPack.Write((uint)movementValues.MovementFlags2, 13);

            BitPack.Write(!movementValues.HasMovementFlags);
            BitPack.Write(0);
            BitPack.Write<uint>(0, 22);
            BitPack.Write(1);
            BitPack.Write(1);
            BitPack.Write(0);
            BitPack.Write(0);

            BitPack.WriteGuidMask(7);

            BitPack.Write(movementValues.IsTransport);
            BitPack.Write(1);

            BitPack.WriteGuidMask(4, 1);

            BitPack.Write(movementValues.IsFallingOrJumping);

            if (movementValues.HasMovementFlags)
                BitPack.Write((uint)movementValues.MovementFlags, 30);

            BitPack.Write(movementValues.Time == 0);

            if (movementValues.IsFallingOrJumping)
                BitPack.Write(movementValues.HasJumpData);

            BitPack.WriteGuidMask(2, 3, 6);

            BitPack.Flush();

            BitPack.WriteGuidBytes(6);

            if (movementValues.Time != 0)
                moveUpdate.WriteUInt32(movementValues.Time);

            if (movementValues.IsFallingOrJumping)
            {
                moveUpdate.WriteFloat(movementValues.JumpVelocity);

                if (movementValues.HasJumpData)
                {
                    moveUpdate.WriteFloat(movementValues.Cos);
                    moveUpdate.WriteFloat(movementValues.CurrentSpeed);
                    moveUpdate.WriteFloat(movementValues.Sin);
                }

                moveUpdate.WriteUInt32(movementValues.FallTime);
            }

            moveUpdate.WriteFloat(vector.X);

            BitPack.WriteGuidBytes(1);

            moveUpdate.WriteFloat(vector.Y);

            BitPack.WriteGuidBytes(2, 7, 5);

            moveUpdate.WriteFloat(vector.Z);

            BitPack.WriteGuidBytes(0, 4, 3);

            if (movementValues.HasRotation)
                moveUpdate.WriteFloat(vector.O);

            var session = WorldMgr.GetSession(guid);
            if (session != null)
            {
                Character pChar = session.Character;

                ObjectMgr.SetPosition(ref pChar, vector, false);
                WorldMgr.SendToInRangeCharacter(pChar, moveUpdate);
            }
        }

        public static void HandleMoveSetWalkSpeed(WorldClass session, float speed = 2.5f)
        {
            PacketWriter setWalkSpeed = new PacketWriter(ServerMessage.MoveSetWalkSpeed);
            BitPack BitPack = new BitPack(setWalkSpeed, session.Character.Guid);

            BitPack.WriteGuidMask(1, 0, 2, 4, 6, 5, 3, 7);
            BitPack.Flush();

            BitPack.WriteGuidBytes(5);

            setWalkSpeed.WriteFloat(speed);

            BitPack.WriteGuidBytes(0, 3, 4);

            setWalkSpeed.WriteUInt32(0);

            BitPack.WriteGuidBytes(2, 1, 6, 7);

            session.Send(ref setWalkSpeed);
        }

        public static void HandleMoveSetRunSpeed(WorldClass session, float speed = 7f)
        {
            PacketWriter setRunSpeed = new PacketWriter(ServerMessage.MoveSetRunSpeed);
            BitPack BitPack = new BitPack(setRunSpeed, session.Character.Guid);

            BitPack.WriteGuidMask(6, 1, 2, 5, 0, 3, 7, 4);
            BitPack.Flush();

            BitPack.WriteGuidBytes(3, 0, 1, 5);

            setRunSpeed.WriteFloat(speed);

            BitPack.WriteGuidBytes(7, 4, 2);

            setRunSpeed.WriteUInt32(0);

            BitPack.WriteGuidBytes(6);

            session.Send(ref setRunSpeed);
        }

        public static void HandleMoveSetSwimSpeed(WorldClass session, float speed = 4.72222f)
        {
            PacketWriter setSwimSpeed = new PacketWriter(ServerMessage.MoveSetSwimSpeed);
            BitPack BitPack = new BitPack(setSwimSpeed, session.Character.Guid);

            BitPack.WriteGuidMask(7, 2, 3, 6, 1, 4, 0, 5);
            BitPack.Flush();

            BitPack.WriteGuidBytes(7, 0, 6, 2, 3);

            setSwimSpeed.WriteUInt32(0);

            BitPack.WriteGuidBytes(1, 5, 4);

            setSwimSpeed.WriteFloat(speed);

            session.Send(ref setSwimSpeed);
        }

        public static void HandleMoveSetFlightSpeed(WorldClass session, float speed = 7f)
        {
            PacketWriter setFlightSpeed = new PacketWriter(ServerMessage.MoveSetFlightSpeed);
            BitPack BitPack = new BitPack(setFlightSpeed, session.Character.Guid);

            BitPack.WriteGuidMask(5, 4, 2, 1, 7, 6, 3, 0);
            BitPack.Flush();

            setFlightSpeed.WriteUInt32(0);

            BitPack.WriteGuidBytes(5, 7);

            setFlightSpeed.WriteFloat(speed);

            BitPack.WriteGuidBytes(1, 2, 3, 6, 0, 4);

            session.Send(ref setFlightSpeed);
        }

        public static void HandleMoveSetRotationSpeed(WorldClass session, float speed = 7f)
        {
            PacketWriter packet = new PacketWriter(ServerMessage.MoveSetRotationSpeed);
            BitPack BitPack = new BitPack(packet, session.Character.Guid);

            BitPack.WriteGuidMask(5, 3, 7, 6, 2, 0, 4, 1);
            BitPack.Flush();

            packet.WriteFloat(speed);
            packet.WriteUInt32(0);

            BitPack.WriteGuidBytes(3, 7, 6, 0, 4, 5, 2, 1);

            session.Send(ref packet);
        }

        public static void HandleMoveSetCanFly(WorldClass session)
        {
            PacketWriter moveSetCanFly = new PacketWriter(ServerMessage.MoveSetCanFly);
            BitPack BitPack = new BitPack(moveSetCanFly, session.Character.Guid);


            BitPack.WriteGuidMask(5, 3, 0, 2, 4, 1, 6, 7);
            BitPack.Flush();

            BitPack.WriteGuidBytes(1, 3, 5, 0, 7, 2, 4, 6);
            
            moveSetCanFly.WriteUInt32(0);

            session.Send(ref moveSetCanFly);
        }

        public static void HandleMoveUnsetCanFly(WorldClass session)
        {
            PacketWriter unsetCanFly = new PacketWriter(ServerMessage.MoveUnsetCanFly);
            BitPack BitPack = new BitPack(unsetCanFly, session.Character.Guid);

            BitPack.WriteGuidMask(4, 1, 6, 7, 0, 5, 3, 2);
            BitPack.Flush();

            BitPack.WriteGuidBytes(0, 2, 7, 1, 4, 3, 5, 6);

            unsetCanFly.WriteUInt32(0);

            session.Send(ref unsetCanFly);
        }

        public static void HandleMoveTeleport(WorldClass session, Vector4 vector)
        {
            bool IsTransport = false;
            bool Unknown = false;

            PacketWriter moveTeleport = new PacketWriter(ServerMessage.MoveTeleport);
            BitPack BitPack = new BitPack(moveTeleport, session.Character.Guid);

            BitPack.WriteGuidMask(6, 1, 4, 2, 3, 5);
            BitPack.Write(Unknown);
            BitPack.WriteGuidMask(0);
            BitPack.Write(IsTransport);

            if (Unknown)
            {
                BitPack.Write(0);
                BitPack.Write(0);
            }

            if (IsTransport)
                BitPack.WriteTransportGuidMask(2, 1, 0, 4, 7, 6, 5, 3);

            BitPack.WriteGuidMask(7);

            BitPack.Flush();

            if (IsTransport)
                BitPack.WriteTransportGuidBytes(6, 3, 1, 4, 7, 5, 0, 2);

            moveTeleport.WriteFloat(vector.Y);

            BitPack.WriteGuidBytes(6);

            moveTeleport.WriteFloat(vector.O);

            BitPack.WriteGuidBytes(1, 7);

            moveTeleport.WriteFloat(vector.X);
            moveTeleport.WriteUInt32(0);
            moveTeleport.WriteFloat(vector.Z);

            if (Unknown)
                moveTeleport.WriteUInt8(0);

            BitPack.WriteGuidBytes(2, 0, 5, 4, 3);

            session.Send(ref moveTeleport);
        }

        public static void HandleTransferPending(WorldClass session, uint mapId)
        {
            bool Unknown = false;
            bool IsTransport = false;

            PacketWriter transferPending = new PacketWriter(ServerMessage.TransferPending);
            BitPack BitPack = new BitPack(transferPending);

            BitPack.Write(Unknown);
            BitPack.Write(IsTransport);

            if (Unknown)
                transferPending.WriteUInt32(0);

            if (IsTransport)
            {
                transferPending.WriteUInt32(0);
                transferPending.WriteUInt32(0);
            }

            transferPending.WriteUInt32(mapId);

            session.Send(ref transferPending);
        }

        public static void HandleNewWorld(WorldClass session, Vector4 vector, uint mapId)
        {
            PacketWriter newWorld = new PacketWriter(ServerMessage.NewWorld);

            newWorld.WriteFloat(vector.O);
            newWorld.WriteFloat(vector.Y);
            newWorld.WriteFloat(vector.Z);
            newWorld.WriteFloat(vector.X);
            newWorld.WriteUInt32(mapId);

            session.Send(ref newWorld);
        }

        public static void HandleMoveRoot(WorldClass session)
        {
            PacketWriter moveRoot = new PacketWriter(ServerMessage.MoveRoot);
            BitPack BitPack = new BitPack(moveRoot, session.Character.Guid);

            BitPack.WriteGuidMask(2, 5, 0, 7, 3, 6, 4, 1);
            BitPack.Flush();

            moveRoot.WriteUInt32(0);

            BitPack.WriteGuidBytes(5, 3, 6, 4, 0, 1, 7, 2);

            session.Send(ref moveRoot);
        }

        public static void HandleMoveUnroot(WorldClass session)
        {
            PacketWriter moveUnroot = new PacketWriter(ServerMessage.MoveUnroot);
            BitPack BitPack = new BitPack(moveUnroot, session.Character.Guid);

            BitPack.WriteGuidMask(1, 0, 7, 2, 3, 4, 5, 6);

            BitPack.WriteGuidBytes(3, 0, 1, 7, 4);
            BitPack.Flush();

            moveUnroot.WriteUInt32(0);

            BitPack.WriteGuidBytes(5, 2, 6);
            BitPack.Flush();

            session.Send(ref moveUnroot);
        }    
    }
}
