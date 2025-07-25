﻿using HarmonyLib;
using LevelGeneration;
using GameData;
using ForcedConsumableSpawns.CustomFields;
using System.Collections.Generic;
using UnityEngine;
using AssetShards;
using AIGraph;

namespace ForcedConsumableSpawns.Patches
{
    [HarmonyPatch]
    internal static class LevelGenerationPatches
    {
        // Used to flag a consumable spawn as our forced spawn
        private const eCommodityType CustomCommodityType = (eCommodityType) 4115;
        private static readonly ZonePlacementWeights DefaultWeights = new();

        // This is only separated from the Consumable job by BigPickupDistribution,
        // should be good enough - need a job that consistently gets inserted near the Consumable job
        // (since Consumable isn't guaranteed to run if SpawnsPerZone == 0).
        [HarmonyPatch(typeof(LG_Distribute_DetailedPlacedFunctionPerZone), nameof(LG_Distribute_DetailedPlacedFunctionPerZone.Build))]
        [HarmonyWrapSafe]
        [HarmonyPrefix]
        private static void Pre_Build(LG_Distribute_DetailedPlacedFunctionPerZone __instance)
        {
            if (__instance.m_func != ExpeditionFunction.PowerGenerator) return;

            DoForceConsumableSpawns(__instance.m_zone);
            DoForceBigPickupSpawns(__instance.m_zone);
        }

        // Largely copy/pasted logic from LG_Distribute_PickupItemsPerZone.Build() (for consumables)
        private static void DoForceBigPickupSpawns(LG_Zone zone)
        {
            var distID = zone.m_settings.m_zoneData.BigPickupDistributionInZone;
            if (distID == 0) return;

            var dist = BigPickupDistributionDataBlock.GetBlock(distID);
            if (dist == null)
            {
                DinoLogger.Error($"Attempted to get BigPickupDistribution {distID}, but it doesn't exist!");
                return;
            }

            if (!dist.TryGetForceList(out var forceList)) return;

            foreach (var forceSpawn in forceList)
            {
                for (int i = 0; i < forceSpawn.Count; i++)
                {
                    uint itemID = forceSpawn.ItemID;

                    LG_DistributePickUpItem? distItem = null;
                    if (LG_DistributionJobUtils.TryGetExistingZoneFunctionDistribution(zone, ExpeditionFunction.BigPickupItem, Builder.SessionSeedRandom.Value("Dist_Pickup_TryGetZoneFunctionDistribution_Pickupitems"), forceSpawn.PlacementWeights ?? DefaultWeights, out var foundDist, out var _))
                    {
                        var lG_DistributePickUpItem = foundDist.TryCast<LG_DistributePickUpItem>();
                        if (lG_DistributePickUpItem != null)
                        {
                            distItem = lG_DistributePickUpItem;
                        }
                        else
                        {
                            DinoLogger.Error("LG_Distribute_PickupItems could not cast distItem as LG_DistributePickupItem!");
                            continue;
                        }
                    }
                    else
                    {
                        AIG_CourseNode randomNodeFromZoneForFunction2 = LG_DistributionJobUtils.GetRandomNodeFromZoneForFunction(zone, ExpeditionFunction.BigPickupItem, Builder.BuildSeedRandom.Value("Dist_Pickup_GetRandomNodeFromZoneForFunction"));
                        distItem = new LG_DistributePickUpItem(ExpeditionFunction.BigPickupItem, ePickupItemType.BigGenericPickup, randomNodeFromZoneForFunction2, Builder.BuildSeedRandom.Range(0, int.MaxValue, "Dist_Pickup_GetRandomNodeFromZoneForFunction"), itemID);
                        randomNodeFromZoneForFunction2.m_zone.DistributionData.PickupItems.Enqueue(distItem);
                        DinoLogger.Error(string.Concat("LG_Distribute_PickupItems Had to create a new LG_DistributePickUpItem for function ", ExpeditionFunction.BigPickupItem, " in zone ", zone.NavInfo.GetFormattedText(LG_NavInfoFormat.Full_And_Number_No_Formatting), " dim:", zone.DimensionIndex, "! m_isWardenObjectiveGatherItem: ", false));
                    }
                    distItem.m_type = ePickupItemType.BigGenericPickup;
                    distItem.m_genericItemId = itemID;
                    distItem.m_consumableData = null;
                    distItem.m_bigPickupData = null;
                    distItem.m_isWardenObjective = false;
                    distItem.m_artifactCategory = 0;
                }
            }
        }

        // Largely copy/pasted logic from LG_Distribute_PickupItemsPerZone.Build() (for consumables)
        private static void DoForceConsumableSpawns(LG_Zone zone)
        {
            var distID = zone.m_settings.m_zoneData.ConsumableDistributionInZone;
            if (distID == 0) return;

            var dist = ConsumableDistributionDataBlock.GetBlock(distID);
            if (dist == null)
            {
                DinoLogger.Error($"Attempted to get ConsumableDistribution {distID}, but it doesn't exist!");
                return;
            }

            if (!dist.TryGetForceList(out var forceList)) return;

            foreach (var forceSpawn in forceList)
            {
                for (int i = 0; i < forceSpawn.Count; i++)
                {
                    ResourceContainerSpawnData spawnData = new()
                    {
                        m_type = eResourceContainerSpawnType.Consumable,
                        m_comType = CustomCommodityType,
                        m_genericItemId = forceSpawn.ItemID
                    };

                    if (LG_DistributionJobUtils.TryGetExistingZoneFunctionDistribution(zone, ExpeditionFunction.ResourceContainerWeak, Builder.SessionSeedRandom.Value("Dist_Pickup_TryGetZoneFunctionDistribution_reoourceContainers"), forceSpawn.PlacementWeights ?? DefaultWeights, out var foundDist, out var _))
                    {
                        var lG_DistributeResourceContainer = foundDist.TryCast<LG_DistributeResourceContainer>();
                        if (lG_DistributeResourceContainer != null)
                        {
                            lG_DistributeResourceContainer.m_packs.Add(spawnData);
                            lG_DistributeResourceContainer.m_locked = Builder.SessionSeedRandom.Value("Dist_Pickup_ChanceForConsumableLocker_Locked") > 0.85f;
                        }
                        else
                        {
                            DinoLogger.Error("LG_Distribute_PickupItems could not cast distItem as LG_DistributeResourceContainer!");
                        }
                    }
                    else
                    {
                        AIG_CourseNode randomNodeFromZoneForFunction = LG_DistributionJobUtils.GetRandomNodeFromZoneForFunction(zone, ExpeditionFunction.ResourceContainerWeak, Builder.BuildSeedRandom.Value("Dist_Pickup_GetRandomNodeFromZoneForFunction"));
                        LG_DistributeResourceContainer lG_DistributeResourceContainer2 = new(ExpeditionFunction.ResourceContainerWeak, spawnData, locked: false, randomNodeFromZoneForFunction, Builder.BuildSeedRandom.Range(0, int.MaxValue, "Dist_Pickup_GetRandomNodeFromZoneForFunction_Container"), Builder.BuildSeedRandom.Value("Dist_Pickup_GetRandomNodeFromZoneForFunction_ContainerLock"), Builder.BuildSeedRandom.Range(2, 4, "Dist_Pickup_GetRandomNodeFromZoneForFunction_StoragePotential"))
                        {
                            m_locked = Builder.SessionSeedRandom.Value("Dist_Pickup_ChanceForConsumableLocker_Locked") > 0.85f
                        };
                        zone.DistributionData.ResourceContainerItems.Enqueue(lG_DistributeResourceContainer2);
                        DinoLogger.Error("Had to create a new resourceContainer!");
                    }
                }
            }
        }

        [HarmonyPatch(typeof(LG_ResourceContainer_Storage), nameof(LG_ResourceContainer_Storage.SpawnConsumable))]
        [HarmonyWrapSafe]
        [HarmonyPrefix]
        private static bool Pre_SpawnConsumable(LG_ResourceContainer_Storage __instance, ResourceContainerSpawnData pack, Transform align, int randomSeed)
        {
            if (pack.m_comType != CustomCommodityType) return true;

            LG_PickupItem lG_PickupItem = GOUtil.SpawnChildAndGetComp<LG_PickupItem>(AssetShardManager.GetLoadedAsset<GameObject>(__instance.m_pickupAssetPath), align);
            lG_PickupItem.SetupAsConsumable(randomSeed, pack.m_genericItemId);
            __instance.SetSpawnNode(lG_PickupItem.gameObject, __instance.m_core.SpawnNode);
            __instance.DisableInteraction(lG_PickupItem.gameObject);
            return false;
        }
    }
}
