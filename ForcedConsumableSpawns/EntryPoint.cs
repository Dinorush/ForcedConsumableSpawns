using BepInEx;
using BepInEx.Unity.IL2CPP;
using ForcedConsumableSpawns.CustomFields;
using ForcedConsumableSpawns.JSON;
using HarmonyLib;
using InjectLib.JsonNETInjection;

namespace ForcedConsumableSpawns
{
    [BepInPlugin("Dinorush." + MODNAME, MODNAME, "1.0.0")]
    [BepInDependency("GTFO.InjectLib", BepInDependency.DependencyFlags.HardDependency)]
    internal sealed class EntryPoint : BasePlugin
    {
        public const string MODNAME = "ForcedConsumableSpawns";

        public override void Load()
        {
            Log.LogMessage("Loading " + MODNAME);
            new Harmony(MODNAME).PatchAll();
            CDDataFields.Init();
            JsonInjector.AddHandler(new ForceDataHandler());
            Log.LogMessage("Loaded " + MODNAME);
        }
    }
}