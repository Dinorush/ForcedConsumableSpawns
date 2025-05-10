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
            FieldInjector<ConsumableDistributionDataBlock>.DefineManagedField<List<ConsumableForceData>>("m_CDForceList");
        }

        public static void SetCDForceList(this ConsumableDistributionDataBlock target, List<ConsumableForceData> data)
        {
            FieldInjector<ConsumableDistributionDataBlock>.TrySetManagedField(target, "m_CDForceList", data);
        }

        public static bool TryGetCDForceList(this ConsumableDistributionDataBlock target, [MaybeNullWhen(false)] out List<ConsumableForceData> data)
        {
            return FieldInjector<ConsumableDistributionDataBlock>.TryGetManagedField(target, "m_CDForceList", out data);
        }
    }
}
