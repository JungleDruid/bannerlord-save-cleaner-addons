using System;
using System.Reflection;
using HarmonyLib;
using SaveCleaner;
using TaleWorlds.CampaignSystem;
using TaleWorlds.LinQuick;

namespace SaveCleanerAddons.Diplomacy;

internal static class SaveCleanerDiplomacyAddon
{
    private static readonly Type HeroRelatedRecordType = AccessTools.TypeByName("Diplomacy.WarExhaustion.EventRecords.HeroRelatedRecord");
    private static readonly FieldInfo HeroRelatedRecordHeroField = AccessTools.Field(HeroRelatedRecordType, "_hero");

    internal static void Register()
    {
        var addon = new SaveCleanerAddon("SaveCleanerAddons.Diplomacy", "Diplomacy");
        addon.OnPreClean += PreClean;
        addon.Register<SubModule>();
    }

    private static bool PreClean(SaveCleanerAddon addon)
    {
        addon.Essential += IsEssential;
        addon.Removable += IsRemovable;
        return true;
    }

    private static bool IsRemovable(SaveCleanerAddon addon, object obj)
    {
        if (HeroRelatedRecordType.IsAssignableFrom(obj.GetType()))
        {
            object o = HeroRelatedRecordHeroField.GetValue(obj);
            if (o == null)
                return true;
        }

        return false;
    }

    private static bool IsEssential(SaveCleanerAddon addon, object obj)
    {
        if (obj is Hero hero)
        {
            bool hasHeroRecord = addon.GetParents(hero).AnyQ(p => HeroRelatedRecordType.IsAssignableFrom(p.GetType()));
            if (hasHeroRecord)
                return true;
        }

        return false;
    }
}