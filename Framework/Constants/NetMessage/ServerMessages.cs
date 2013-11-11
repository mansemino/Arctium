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
    public enum ServerMessage : ushort
    {
        #region Legacy
        ObjectUpdate                      = 0x0C22,
        TutorialFlags                     = 0x0D1B,
        StartCinematic                    = 0x0198,
        #endregion

        #region JAMClientConnection
        AuthChallenge                     = 0x0C5D,
        Pong                              = 0x005D,
        #endregion

        #region JAMClientMove
        MoveUpdate                        = 0x0337,
        MoveSetCanFly                     = 0x0209,
        MoveUnsetCanFly                   = 0x031B,
        MoveSetWalkSpeed                  = 0x0716,
        MoveSetRunSpeed                   = 0x06A5,
        MoveSetSwimSpeed                  = 0x0A0D,
        MoveSetFlightSpeed                = 0x02B2,
        MoveTeleport                      = 0x0A2E,
        #endregion

        #region JAMClientQuest
        GossipMessage                     = 0x03FC,
        #endregion

        #region JAMClientSpell
        SendKnownSpells                   = 0x1164,
        #endregion

        // Updated
        #region JAMClientDispatch
        DeleteChar                        = 0x0017,
        NewWorld                          = 0x010F,
        QueryCreatureResponse             = 0x011C,
        UpdateActionButtons               = 0x0406,
        EnumCharactersResult              = 0x040E,
        UpdateTalentData                  = 0x0494,
        UnlearnedSpells                   = 0x049E,
        MOTD                              = 0x04AC,
        DestroyObject                     = 0x04B4,
        RealmQueryResponse                = 0x052D,
        UITime                            = 0x05AC,
        GenerateRandomCharacterNameResult = 0x081E,
        RealmSplit                        = 0x0884,
        QueryGameObjectResponse           = 0x0916,
        TransferPending                   = 0x0917,
        AuthResponse                      = 0x0D05,
        LoginSetTimeSpeed                 = 0x0D17,
        LogoutComplete                    = 0x0D95,
        CreateChar                        = 0x1007,
        QueryNPCTextResponse              = 0x101F,
        CacheVersion                      = 0x1037,
        AddonInfo                         = 0x1136,
        LearnedSpells                     = 0x118E,
        DBReply                           = 0x1406,
        QueryPlayerNameResponse           = 0x1407,
        AccountDataTimes                  = 0x1486,
        Chat                              = 0x14AC,
        #endregion

        TransferInitiate                  = 0x4F57,
    }
}
