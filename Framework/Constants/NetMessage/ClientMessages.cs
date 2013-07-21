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
        ChatMessageSay                             = 0x016A,
        ChatMessageYell                            = 0x1333,
        ChatMessageWhisper                         = 0x143A,
        CliChatMessageAfk		                   = 0x062B,
        CliChatMessageDnd		                   = 0x1271,
        #endregion

        #region UserRouterClient
        AuthSession                                = 0x09F1,
        Ping                                       = 0x08E3,
        LogDisconnect                              = 0x09A2,
        #endregion

        #region Legacy
        ActivePlayer                               = 0x0704,
        ZoneUpdate                                 = 0x2000, // Not updated
        CliWhoRequest                              = 0x0308,
        #endregion

        // Value > 0x1FFF are not known.
        #region JAM
        CliObjectUpdateFailed                      = 0x2001, // Not updated
        ViolenceLevel                              = 0x054B,
        RealmSplit                                 = 0x0B48,
        DBQueryBulk                                = 0x0149,
        GenerateRandomCharacterName                = 0x091D,
        EnumCharacters                             = 0x0B1D,
        PlayerLogin                                = 0x0A19,
        LoadingScreenNotify                        = 0x0341,
        SetActionButton                            = 0x0400,
        CreateCharacter                            = 0x0404,
        QueryPlayerName                            = 0x0018,
        QueryRealmName                             = 0x0209,
        ReadyForAccountDataTimes                   = 0x0755,
        UITimeRequest                              = 0x0405,
        CharDelete                                 = 0x010C,
        CliSetSpecialization                       = 0x005B,
        CliLearnTalents                            = 0x2002, // Not updated
        CliQueryCreature                           = 0x050F,
        CliQueryGameObject                         = 0x058F,
        CliQueryNPCText                            = 0x009E,
        CliTalkToGossip                            = 0x1414,
        CliLogoutRequest                           = 0x1150,
        CliLogoutCancel                            = 0x1309,
        CliLogoutInstant                           = 0x0A82,
        CliSetSelection                            = 0x1759,
        #endregion

        #region PlayerMove
        MoveStartForward                           = 0x0A4B,
        MoveStartBackward                          = 0x08D2,
        MoveStop                                   = 0x0813,
        MoveStartStrafeLeft                        = 0x0816,
        MoveStartStrafeRight                       = 0x0843,
        MoveStopStrafe                             = 0x0A4A,
        MoveJump                                   = 0x0CCA,
        MoveStartTurnLeft                          = 0x0A9E,
        MoveStartTurnRight                         = 0x0A07,
        MoveStopTurn                               = 0x080A,
        MoveStartPitchUp                           = 0x0C02,
        MoveStartPitchDown                         = 0x0A12,
        MoveStopPitch                              = 0x0AC7,
        MoveSetRunMode                             = 0x081F,
        MoveSetWalkMode                            = 0x0856,
        MoveFallLand                               = 0x0CCF,
        MoveStartSwim                              = 0x0C13,
        MoveStopSwim                               = 0x0C93,
        MoveToggleCollisionCheat                   = 0x0A5F,
        MoveSetFacing                              = 0x0C43,
        MoveSetPitch                               = 0x0886,
        MoveHeartbeat                              = 0x0E0B,
        MoveFallReset                              = 0x0AC3,
        MoveSetFly                                 = 0x0ADF,
        MoveStartAscend                            = 0x089F,
        MoveStopAscend                             = 0x0A47,
        MoveChangeTransport                        = 0x0C92,
        MoveStartDescend                           = 0x0893,
        MoveDismissVehicle                         = 0x0A56,
        #endregion

        #region Emote
        CliStandStateChange                        = 0x1016,
        CliTextEmote                               = 0x1444,
        #endregion

        TransferInitiate                           = 0x4F57,
    }
}
