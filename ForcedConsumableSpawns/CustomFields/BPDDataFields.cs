using GameData;
using InjectLib.FieldInjection;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ForcedConsumableSpawns.CustomFields
{
    internal static class BPDDataFields
    {
        internal static void Init()
        {
            FieldInjector<BigPickupDistributionDataBlock>.DefineManagedField<List<PickupForceData>>("m_ForceList");
        }

        public static void SetForceList(this BigPickupDistributionDataBlock target, List<PickupForceData> data)
        {
            FieldInjector<BigPickupDistributionDataBlock>.TrySetManagedField(target, "m_ForceList", data);
        }

        public static bool TryGetForceList(this BigPickupDistributionDataBlock target, [MaybeNullWhen(false)] out List<PickupForceData> data)
        {
            return FieldInjector<BigPickupDistributionDataBlock>.TryGetManagedField(target, "m_ForceList", out data);
        }
    }
}
