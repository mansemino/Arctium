using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Constants.Creature
{
    public enum NpcFlag
    {
        None              = 0x0,
        Gossip            = 0x1,
        QuestUnit         = 0x2,
        Unknown           = 0x4,
        Unknown2          = 0x8,
        Trainer           = 0x10,
        Unknown3          = 0x20,
        Unknown4          = 0x40,
        Merchant          = 0x80,
        Unknown5          = 0x100,
        Unknown6          = 0x200,
        Unknown7          = 0x400,
        Unknown8          = 0x800,
        Unknown9          = 0x1000,
        TaxiUnit          = 0x2000,
        SpiritHealer      = 0x4000,
        AreaSpiritHealer  = 0x8000,
        Binder            = 0x10000,
        Banker            = 0x20000,
        PetitionVendor    = 0x40000,
        TabardVendor      = 0x80000,
        BattleMaster      = 0x100000,
        Auctioneer        = 0x200000,
        StableMaster      = 0x400000,
        GuildBanker       = 0x800000,
        SpellClick        = 0x1000000,
        RideVehicle       = 0x2000000,
        MailObject        = 0x4000000,
        ForgeMaster       = 0x8000000,
        Transmogrifier    = 0x10000000,
        VoidStorageBanker = 0x20000000,
        PetBattlePvE      = 0x40000000,
    }
}
