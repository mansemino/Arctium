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

namespace Framework.Constants
{
    public enum ObjectFields
    {
        Guid                              = 0x0,
        Data                              = 0x2,
        Type                              = 0x4,
        Entry                             = 0x5,
        DynamicFlags                      = 0x6,
        Scale                             = 0x7,
        End                               = 0x8
    };

    public enum DynamicObjectArrays
    {
        // Empty
        End                               = 0x0
    }

    public enum ItemFields
    {
        Owner                             = ObjectFields.End + 0x0,
        ContainedIn                       = ObjectFields.End + 0x2,
        Creator                           = ObjectFields.End + 0x4,
        GiftCreator                       = ObjectFields.End + 0x6,
        StackCount                        = ObjectFields.End + 0x8,
        Expiration                        = ObjectFields.End + 0x9,
        SpellCharges                      = ObjectFields.End + 0xA,
        DynamicFlags                      = ObjectFields.End + 0xF,
        Enchantment                       = ObjectFields.End + 0x10,
        PropertySeed                      = ObjectFields.End + 0x37,
        RandomPropertiesID                = ObjectFields.End + 0x38,
        Durability                        = ObjectFields.End + 0x39,
        MaxDurability                     = ObjectFields.End + 0x3A,
        CreatePlayedTime                  = ObjectFields.End + 0x3B,
        ModifiersMask                     = ObjectFields.End + 0x3C,
        End                               = ObjectFields.End + 0x3D
    };

    public enum ItemDynamicArrays
    {
        // Empty
        End                               = DynamicObjectArrays.End
    }

    public enum ContainerFields
    {
        Slots                             = ItemFields.End + 0x0,
        NumSlots                          = ItemFields.End + 0x48,
        End                               = ItemFields.End + 0x49
    };

    public enum ContainerDynamicArrays
    {
        // Empty
        End                               = ItemDynamicArrays.End
    }

    public enum UnitFields
    {
        Charm                             = ObjectFields.End + 0x0,
        Summon                            = ObjectFields.End + 0x2,
        Critter                           = ObjectFields.End + 0x4,
        CharmedBy                         = ObjectFields.End + 0x6,
        SummonedBy                        = ObjectFields.End + 0x8,
        CreatedBy                         = ObjectFields.End + 0xA,
        DemonCreator                      = ObjectFields.End + 0xC,
        Target                            = ObjectFields.End + 0xE,
        BattlePetCompanionGUID            = ObjectFields.End + 0x10,
        ChannelObject                     = ObjectFields.End + 0x12,
        ChannelSpell                      = ObjectFields.End + 0x14,
        SummonedByHomeRealm               = ObjectFields.End + 0x15,
        Sex                               = ObjectFields.End + 0x16,
        DisplayPower                      = ObjectFields.End + 0x17,
        OverrideDisplayPowerID            = ObjectFields.End + 0x18,
        Health                            = ObjectFields.End + 0x19,
        Power                             = ObjectFields.End + 0x1A,
        MaxHealth                         = ObjectFields.End + 0x1F,
        MaxPower                          = ObjectFields.End + 0x20,
        PowerRegenFlatModifier            = ObjectFields.End + 0x25,
        PowerRegenInterruptedFlatModifier = ObjectFields.End + 0x2A,
        Level                             = ObjectFields.End + 0x2F,
        EffectiveLevel                    = ObjectFields.End + 0x30,
        FactionTemplate                   = ObjectFields.End + 0x31,
        VirtualItemID                     = ObjectFields.End + 0x32,
        Flags                             = ObjectFields.End + 0x35,
        Flags2                            = ObjectFields.End + 0x36,
        AuraState                         = ObjectFields.End + 0x37,
        AttackRoundBaseTime               = ObjectFields.End + 0x38,
        RangedAttackRoundBaseTime         = ObjectFields.End + 0x3A,
        BoundingRadius                    = ObjectFields.End + 0x3B,
        CombatReach                       = ObjectFields.End + 0x3C,
        DisplayID                         = ObjectFields.End + 0x3D,
        NativeDisplayID                   = ObjectFields.End + 0x3E,
        MountDisplayID                    = ObjectFields.End + 0x3F,
        MinDamage                         = ObjectFields.End + 0x40,
        MaxDamage                         = ObjectFields.End + 0x41,
        MinOffHandDamage                  = ObjectFields.End + 0x42,
        MaxOffHandDamage                  = ObjectFields.End + 0x43,
        AnimTier                          = ObjectFields.End + 0x44,
        PetNumber                         = ObjectFields.End + 0x45,
        PetNameTimestamp                  = ObjectFields.End + 0x46,
        PetExperience                     = ObjectFields.End + 0x47,
        PetNextLevelExperience            = ObjectFields.End + 0x48,
        ModCastingSpeed                   = ObjectFields.End + 0x49,
        ModSpellHaste                     = ObjectFields.End + 0x4A,
        ModHaste                          = ObjectFields.End + 0x4B,
        ModRangedHaste                    = ObjectFields.End + 0x4C,
        ModHasteRegen                     = ObjectFields.End + 0x4D,
        CreatedBySpell                    = ObjectFields.End + 0x4E,
        NpcFlags                          = ObjectFields.End + 0x4F,
        EmoteState                        = ObjectFields.End + 0x51,
        Stats                             = ObjectFields.End + 0x52,
        StatPosBuff                       = ObjectFields.End + 0x57,
        StatNegBuff                       = ObjectFields.End + 0x5C,
        Resistances                       = ObjectFields.End + 0x61,
        ResistanceBuffModsPositive        = ObjectFields.End + 0x68,
        ResistanceBuffModsNegative        = ObjectFields.End + 0x6F,
        BaseMana                          = ObjectFields.End + 0x76,
        BaseHealth                        = ObjectFields.End + 0x77,
        ShapeshiftForm                    = ObjectFields.End + 0x78,
        AttackPower                       = ObjectFields.End + 0x79,
        AttackPowerModPos                 = ObjectFields.End + 0x7A,
        AttackPowerModNeg                 = ObjectFields.End + 0x7B,
        AttackPowerMultiplier             = ObjectFields.End + 0x7C,
        RangedAttackPower                 = ObjectFields.End + 0x7D,
        RangedAttackPowerModPos           = ObjectFields.End + 0x7E,
        RangedAttackPowerModNeg           = ObjectFields.End + 0x7F,
        RangedAttackPowerMultiplier       = ObjectFields.End + 0x80,
        MinRangedDamage                   = ObjectFields.End + 0x81,
        MaxRangedDamage                   = ObjectFields.End + 0x82,
        PowerCostModifier                 = ObjectFields.End + 0x83,
        PowerCostMultiplier               = ObjectFields.End + 0x8A,
        MaxHealthModifier                 = ObjectFields.End + 0x91,
        HoverHeight                       = ObjectFields.End + 0x92,
        MinItemLevel                      = ObjectFields.End + 0x93,
        MaxItemLevel                      = ObjectFields.End + 0x94,
        WildBattlePetLevel                = ObjectFields.End + 0x95,
        BattlePetCompanionNameTimestamp   = ObjectFields.End + 0x96,
        InteractSpellID                   = ObjectFields.End + 0x97,
        End                               = ObjectFields.End + 0x98
    };

    public enum UnitDynamicArrays
    {
        PassiveSpells                     = DynamicObjectArrays.End + 0x0,
        WorldEffects                      = DynamicObjectArrays.End + 0x101,
        End                               = DynamicObjectArrays.End + 0x202
    }

    public enum PlayerFields
    {
        DuelArbiter                       = UnitFields.End + 0x0,
        PlayerFlags                       = UnitFields.End + 0x2,
        GuildRankID                       = UnitFields.End + 0x3,
        GuildDeleteDate                   = UnitFields.End + 0x4,
        GuildLevel                        = UnitFields.End + 0x5,
        HairColorID                       = UnitFields.End + 0x6,
        RestState                         = UnitFields.End + 0x7,
        ArenaFaction                      = UnitFields.End + 0x8,
        DuelTeam                          = UnitFields.End + 0x9,
        GuildTimeStamp                    = UnitFields.End + 0xA,
        QuestLog                          = UnitFields.End + 0xB,
        VisibleItems                      = UnitFields.End + 0x2F9,
        PlayerTitle                       = UnitFields.End + 0x31F,
        FakeInebriation                   = UnitFields.End + 0x320,
        VirtualPlayerRealm                = UnitFields.End + 0x321,
        CurrentSpecID                     = UnitFields.End + 0x322,
        TaxiMountAnimKitID                = UnitFields.End + 0x323,
        CurrentBattlePetBreedQuality      = UnitFields.End + 0x324,
        InvSlots                          = UnitFields.End + 0x325,
        FarsightObject                    = UnitFields.End + 0x3D1,
        KnownTitles                       = UnitFields.End + 0x3D3,
        Coinage                           = UnitFields.End + 0x3DD,
        XP                                = UnitFields.End + 0x3DF,
        NextLevelXP                       = UnitFields.End + 0x3E0,
        Skill                             = UnitFields.End + 0x3E1,
        CharacterPoints                   = UnitFields.End + 0x5A1,
        MaxTalentTiers                    = UnitFields.End + 0x5A2,
        TrackCreatureMask                 = UnitFields.End + 0x5A3,
        TrackResourceMask                 = UnitFields.End + 0x5A4,
        MainhandExpertise                 = UnitFields.End + 0x5A5,
        OffhandExpertise                  = UnitFields.End + 0x5A6,
        RangedExpertise                   = UnitFields.End + 0x5A7,
        CombatRatingExpertise             = UnitFields.End + 0x5A8,
        BlockPercentage                   = UnitFields.End + 0x5A9,
        DodgePercentage                   = UnitFields.End + 0x5AA,
        ParryPercentage                   = UnitFields.End + 0x5AB,
        CritPercentage                    = UnitFields.End + 0x5AC,
        RangedCritPercentage              = UnitFields.End + 0x5AD,
        OffhandCritPercentage             = UnitFields.End + 0x5AE,
        SpellCritPercentage               = UnitFields.End + 0x5AF,
        ShieldBlock                       = UnitFields.End + 0x5B6,
        ShieldBlockCritPercentage         = UnitFields.End + 0x5B7,
        Mastery                           = UnitFields.End + 0x5B8,
        PvpPowerDamage                    = UnitFields.End + 0x5B9,
        PvpPowerHealing                   = UnitFields.End + 0x5BA,
        ExploredZones                     = UnitFields.End + 0x5BB,
        RestStateBonusPool                = UnitFields.End + 0x683,
        ModDamageDonePos                  = UnitFields.End + 0x684,
        ModDamageDoneNeg                  = UnitFields.End + 0x68B,
        ModDamageDonePercent              = UnitFields.End + 0x692,
        ModHealingDonePos                 = UnitFields.End + 0x699,
        ModHealingPercent                 = UnitFields.End + 0x69A,
        ModHealingDonePercent             = UnitFields.End + 0x69B,
        ModPeriodicHealingDonePercent     = UnitFields.End + 0x69C,
        WeaponDmgMultipliers              = UnitFields.End + 0x69D,
        ModSpellPowerPercent              = UnitFields.End + 0x6A0,
        ModResiliencePercent              = UnitFields.End + 0x6A1,
        OverrideSpellPowerByAPPercent     = UnitFields.End + 0x6A2,
        OverrideAPBySpellPowerPercent     = UnitFields.End + 0x6A3,
        ModTargetResistance               = UnitFields.End + 0x6A4,
        ModTargetPhysicalResistance       = UnitFields.End + 0x6A5,
        LifetimeMaxRank                   = UnitFields.End + 0x6A6,
        SelfResSpell                      = UnitFields.End + 0x6A7,
        PvpMedals                         = UnitFields.End + 0x6A8,
        BuybackPrice                      = UnitFields.End + 0x6A9,
        BuybackTimestamp                  = UnitFields.End + 0x6B5,
        YesterdayHonorableKills           = UnitFields.End + 0x6C1,
        LifetimeHonorableKills            = UnitFields.End + 0x6C2,
        WatchedFactionIndex               = UnitFields.End + 0x6C3,
        CombatRatings                     = UnitFields.End + 0x6C4,
        PvpInfo                           = UnitFields.End + 0x6DF,
        MaxLevel                          = UnitFields.End + 0x6F7,
        RuneRegen                         = UnitFields.End + 0x6F8,
        NoReagentCostMask                 = UnitFields.End + 0x6FC,
        GlyphSlots                        = UnitFields.End + 0x700,
        Glyphs                            = UnitFields.End + 0x706,
        GlyphSlotsEnabled                 = UnitFields.End + 0x70C,
        PetSpellPower                     = UnitFields.End + 0x70D,
        Researching                       = UnitFields.End + 0x70E,
        ProfessionSkillLine               = UnitFields.End + 0x716,
        UiHitModifier                     = UnitFields.End + 0x718,
        UiSpellHitModifier                = UnitFields.End + 0x719,
        HomeRealmTimeOffset               = UnitFields.End + 0x71A,
        ModPetHaste                       = UnitFields.End + 0x71B,
        SummonedBattlePetGUID             = UnitFields.End + 0x71C,
        OverrideSpellsID                  = UnitFields.End + 0x71E,
        LfgBonusFactionID                 = UnitFields.End + 0x71F,
        LootSpecID                        = UnitFields.End + 0x720,
        OverrideZonePVPType               = UnitFields.End + 0x721,
        ItemLevelDelta                    = UnitFields.End + 0x722,
        End                               = UnitFields.End + 0x723
    };

    public enum PlayerDynamicArrays
    {
        ResearchSites                     = UnitDynamicArrays.End + 0x0,
        DailyQuestsCompleted              = UnitDynamicArrays.End + 0x2,
        End                               = UnitDynamicArrays.End + 0x4
    }

    public enum GameObjectFields
    {
        CreatedBy                         = ObjectFields.End + 0x0,
        DisplayID                         = ObjectFields.End + 0x2,
        Flags                             = ObjectFields.End + 0x3,
        ParentRotation                    = ObjectFields.End + 0x4,
        FactionTemplate                   = ObjectFields.End + 0x8,
        Level                             = ObjectFields.End + 0x9,
        PercentHealth                     = ObjectFields.End + 0xA,
        StateSpellVisualID                = ObjectFields.End + 0xB,
        End                               = ObjectFields.End + 0xC
    };

    public enum GameObjectDynamicArrays
    {
        // One field, unknown
        UnknownField                      = DynamicObjectArrays.End + 0x0,
        End                               = DynamicObjectArrays.End + 0x1
    }

    public enum DynamicObjectFields
    {
        Caster                            = ObjectFields.End + 0x0,
        TypeAndVisualID                   = ObjectFields.End + 0x2,
        SpellId                           = ObjectFields.End + 0x3,
        Radius                            = ObjectFields.End + 0x4,
        CastTime                          = ObjectFields.End + 0x5,
        End                               = ObjectFields.End + 0x6
    };

    public enum DynamicObjectDynamicArrays
    {
        // Empty
        End                               = DynamicObjectArrays.End
    }

    public enum CorpseFields
    {
        Owner                             = ObjectFields.End + 0x0,
        PartyGuid                         = ObjectFields.End + 0x2,
        DisplayId                         = ObjectFields.End + 0x4,
        Items                             = ObjectFields.End + 0x5,
        SkinId                            = ObjectFields.End + 0x18,
        FacialHairStyleId                 = ObjectFields.End + 0x19,
        Flags                             = ObjectFields.End + 0x1A,
        DynamicFlags                      = ObjectFields.End + 0x1B,
        End                               = ObjectFields.End + 0x1C
    };

    public enum CorpseDynamicArrays
    {
        // Empty
        End                               = DynamicObjectArrays.End
    }

    public enum AreaTriggerFields
    {
        Caster                            = ObjectFields.End + 0x0,
        Duration                          = ObjectFields.End + 0x2,
        SpellId                           = ObjectFields.End + 0x3,
        SpellVisualId                     = ObjectFields.End + 0x4,
        ExplicitScale                     = ObjectFields.End + 0x5,
        End                               = ObjectFields.End + 0x6
    };

    public enum AreaTriggerDynamicArrays
    {
        // Empty
        End                               = DynamicObjectArrays.End
    }

    public enum SceneObjectFields
    {
        ScriptPackageId                   = ObjectFields.End + 0x0,
        RndSeedVal                        = ObjectFields.End + 0x1,
        CreatedBy                         = ObjectFields.End + 0x2,
        SceneType                         = ObjectFields.End + 0x4,
        End                               = ObjectFields.End + 0x5
    };

    public enum SceneObjectDynamicArrays
    {
        // Empty
        End                               = DynamicObjectArrays.End
    }
}
