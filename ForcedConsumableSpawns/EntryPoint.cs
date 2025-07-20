using BepInEx;
using BepInEx.Unity.IL2CPP;
using ForcedConsumableSpawns.CustomFields;
using ForcedConsumableSpawns.JSON;
using HarmonyLib;
using InjectLib.JsonNETInjection;

namespace ForcedConsumableSpawns
{
    [BepInPlugin("Dinorush." + MODNAME, MODNAME, "1.1.0")]
    [BepInDependency("GTFO.InjectLib", BepInDependency.DependencyFlags.HardDependency)]
    internal sealed class EntryPoint : BasePlugin
    {
        public const string MODNAME = "ForcedConsumableSpawns";

        public override void Load()
        {
            new Harmony(MODNAME).PatchAll();
            BPDDataFields.Init();
            CDDataFields.Init();
            JsonInjector.AddHandler(new ConsumableForceDataHandler());
            JsonInjector.AddHandler(new BigPickupForceDataHandler());
            Log.LogMessage("Loaded " + MODNAME);
        }
    }
}