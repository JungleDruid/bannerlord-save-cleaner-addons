using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BannerKings.Behaviours.Diplomacy.Groups;
using BannerKings.Behaviours.Relations;
using BannerKings.Managers.Court;
using BannerKings.Managers.Populations.Estates;
using Microsoft.Extensions.Logging;
using SaveCleaner;
using TaleWorlds.CampaignSystem;

namespace SaveCleanerAddons.BannerKings;

internal static class SaveCleanerBannerKingsAddon
{
    internal static void Register()
    {
        var addon = new SaveCleanerAddon("SaveCleanerAddons.BannerKings", "BannerKings");
        addon.CanRemoveChild += CanRemoveChild;
        addon.DoRemoveChild += DoRemoveChild;
        addon.AddSupportedNamespace(new Regex(@"^BannerKings\b"));
        addon.Register<SubModule>();
    }

    private static bool CanRemoveChild(SaveCleanerAddon addon, Node node)
    {
        return RemoveChild(addon, node, true);
    }

    private static bool DoRemoveChild(SaveCleanerAddon addon, Node node)
    {
        return RemoveChild(addon, node, false);
    }

    private static bool RemoveChild(SaveCleanerAddon addon, Node node, bool dryRun)
    {
        bool result = false;
        try
        {
            object obj = node.Value;
            object parent = node.Parent.Value;
            object top = node.Top.Value;
            if (obj is Hero hero)
            {
                switch (top)
                {
                    case HeroRelations relations when relations.Hero == hero:
                        // the HeroRelations object will be removed anyway
                        result = true;
                        break;
                    case HeroRelations:
                    {
                        if (parent is Dictionary<Hero, List<RelationsModifier>> dictionary)
                        {
                            if (!dryRun) dictionary.Remove(hero);
                            result = true;
                        }

                        break;
                    }
                    case BKRelationsBehavior:
                    {
                        if (parent is Dictionary<Hero, HeroRelations> dictionary)
                        {
                            if (!dryRun) dictionary.Remove(hero);
                            result = true;
                        }

                        break;
                    }
                    case CouncilMember councilMember:
                    {
                        if (dryRun)
                        {
                            result = councilMember.Member == hero;
                        }
                        else
                        {
                            if (councilMember.Member == hero) councilMember.SetMember(null);
                            result = true;
                        }

                        break;
                    }
                    case Estate estate:
                    {
                        if (dryRun)
                        {
                            result = estate.Owner == hero;
                        }
                        else
                        {
                            if (estate.Owner == hero) estate.SetOwner(null);
                            result = true;
                        }

                        break;
                    }
                    case DiplomacyGroup diplomacyGroup:
                    {
                        if (dryRun)
                        {
                            result = diplomacyGroup.JoinTime.ContainsKey(hero);
                        }
                        else
                        {
                            diplomacyGroup.JoinTime.Remove(hero);
                            result = true;
                        }

                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            addon.Log("Error removing child.", LogLevel.Error, ex);
            result = false;
        }

        if (!result)
        {
#if DEBUG
            addon.Log($"Unable to remove {GetLogName(node.Value)} from {GetLogName(node.Parent?.Value)}. Link: {node.GetLinkString()}", LogLevel.Warning);
#else
            addon.Log($"Unable to remove {GetLogName(node.Value)} from {GetLogName(node.Parent?.Value)}.", LogLevel.Debug);
#endif
        }

        return result;
    }

    private static string GetLogName(object obj) => $"[{obj?.GetType().Name}]{obj}";
}