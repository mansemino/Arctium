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

namespace Framework.Constants
{
    [Flags]
    public enum PlayerFlag
    {
        None                        = 0x0,
        GroupLeader                 = 0x1,
        Afk                         = 0x2,
        Dnd                         = 0x4,
        GameMaster                  = 0x8,
        Ghost                       = 0x10,
        Resting                     = 0x20,
        Unknown6                    = 0x40,
        Unknown7                    = 0x80,
        PVPEnabled                  = 0x100,
        PVPDesired                  = 0x200,
        ToogleHelm                  = 0x400,
        ToogleCloak                 = 0x800,
        PartialPlayTime             = 0x1000,
        NoPlayTime                  = 0x2000,
        OutOfBounds                 = 0x4000,
        Unknown15                   = 0x8000,
        Unknown16                   = 0x10000,
        TaxiBenchmarkMode           = 0x20000,
        PVPTimerRunning             = 0x40000,
        Commentator                 = 0x80000,
        Unknown20                   = 0x100000,
        Unknown21                   = 0x200000,
        CommentatorUberOrInArena    = 0x400000,
        AccountAchievementsHidden   = 0x800000,
        Unknown24                   = 0x1000000,
        XPUserDisabled              = 0x2000000,
        Unknown26                   = 0x4000000,
        AutoDeclineGuildInvites     = 0x8000000,
        GuildLevelEnabled           = 0x10000000,
        CanUseVoidStorage           = 0x20000000,
    }
}
