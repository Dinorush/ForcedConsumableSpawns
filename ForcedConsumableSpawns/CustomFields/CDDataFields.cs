using GameData;
using InjectLib.FieldInjection;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ForcedConsumableSpawns.CustomFields
{
    internal static class CDDataFields
    {
        internal static void Init()
        {
            FieldInjector<ConsumableDistributionDataBlock>.DefineManagedField<List<PickupForceData>>("m_ForceList");
        }

        public static void SetForceList(this ConsumableDistributionDataBlock target, List<PickupForceData> data)
        {
            FieldInjector<ConsumableDistributionDataBlock>.TrySetManagedField(target, "m_ForceList", data);
        }

        public static bool TryGetForceList(this ConsumableDistributionDataBlock target, [MaybeNullWhen(false)] out List<PickupForceData> data)
        {
            return FieldInjector<ConsumableDistributionDataBlock>.TryGetManagedField(target, "m_ForceList", out data);
        }
    }
}
