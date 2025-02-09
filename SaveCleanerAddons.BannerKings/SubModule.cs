using TaleWorlds.MountAndBlade;

// ReSharper disable ClassNeverInstantiated.Global

namespace SaveCleanerAddons.BannerKings;

public class SubModule : MBSubModuleBase
{
    protected override void OnSubModuleLoad()
    {
        SaveCleanerBannerKingsAddon.Register();
    }
}