using TaleWorlds.MountAndBlade;

// ReSharper disable ClassNeverInstantiated.Global

namespace SaveCleanerAddons.Diplomacy;

public class SubModule : MBSubModuleBase
{
    protected override void OnSubModuleLoad()
    {
        SaveCleanerDiplomacyAddon.Register();
    }
}