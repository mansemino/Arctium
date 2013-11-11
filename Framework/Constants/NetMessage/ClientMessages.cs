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

namespace Framework.Constants.NetMessage
{
    public enum ClientMessage : ushort
    {
        #region ChatMessages
        ChatMessageSay               = 0x14FC,
        ChatMessageYell              = 0x045C,
        ChatMessageWhisper           = 0x14D8,
        #endregion

        #region UserRouterClient
        AuthSession                  = 0x14DA,
        Ping                         = 0x11E6,
        LogDisconnect                = 0x14FA,
        #endregion

        #region Legacy
        ActivePlayer                 = 0x1A4D,
        #endregion

        // Value > 0x1FFF are not known.
        #region JAM
        ObjectUpdateFailed          = 0x1A44,
        ViolenceLevel               = 0x13CD,
        RealmSplit                  = 0x0449,
        DBQueryBulk                 = 0x01E4,
        GenerateRandomCharacterName = 0x184C,
        EnumCharacters              = 0x0848,
        PlayerLogin                 = 0x01E1,
        LoadingScreenNotify         = 0x1148,
        SetActionButton             = 0x014C,
        CreateCharacter             = 0x08CD,
        QueryPlayerName             = 0x11E9,
        QueryRealmName              = 0x09E1,
        ReadyForAccountDataTimes    = 0x144C,
        UITimeRequest               = 0x04EC,
        CharDelete                  = 0x09C0,
        CliSetSpecialization        = 0x17DF,
        CliLearnTalents             = 0x1776,
        CliQueryCreature            = 0x1647,
        CliQueryGameObject          = 0x1677,
        CliQueryNPCText             = 0x17CF,
        CliTalkToGossip             = 0x025C,
        CliLogoutRequest            = 0x03EC,
        CliSetSelection             = 0x07CD,
        #endregion

        #region PlayerMove
        MoveStartForward            = 0x13C9,
        MoveStartBackward           = 0x12C0,
        MoveStop                    = 0x0649,
        MoveStartStrafeLeft         = 0x0EC8,
        MoveStartStrafeRight        = 0x0269,
        MoveStopStrafe              = 0x12C9,
        MoveJump                    = 0x07C9,
        MoveStartTurnLeft           = 0x0760,
        MoveStartTurnRight          = 0x17C9,
        MoveStopTurn                = 0x1749,
        MoveStartPitchUp            = 0x0FE1,
        MoveStartPitchDown          = 0x16E8,
        MoveStopPitch               = 0x1A48,
        MoveSetRunMode              = 0x0748,
        MoveSetWalkMode             = 0x0BE1,
        MoveFallLand                = 0x17E9,
        MoveStartSwim               = 0x0FC8,
        MoveStopSwim                = 0x0FC9,
        MoveToggleCollisionCheat    = 0x0BC8,
        MoveSetFacing               = 0x1368,
        MoveSetPitch                = 0x0261,
        MoveHeartbeat               = 0x0AC8,
        MoveFallReset               = 0x12C1,
        MoveSetFly                  = 0x1769,
        MoveStartAscend             = 0x16C1,
        MoveStopAscend              = 0x1268,
        MoveChangeTransport         = 0x1361,
        MoveStartDescend            = 0x0641,
        MoveDismissVehicle          = 0x1261,
        #endregion

        TransferInitiate            = 0x4F57,
    }
}
