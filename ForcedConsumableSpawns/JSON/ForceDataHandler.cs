using ForcedConsumableSpawns.CustomFields;
using GameData;
using Il2CppJsonNet.Linq;
using InjectLib.JsonNETInjection.Handler;
using InjectLib.JsonNETInjection.Supports;
using System.Collections.Generic;

namespace ForcedConsumableSpawns.JSON
{
    internal class ForceDataHandler : Il2CppJsonReferenceTypeHandler<ConsumableDistributionDataBlock>
    {
        public override void OnRead(in Il2CppSystem.Object result, in JToken jToken)
        {
            JObject jObject = jToken.TryCast<JObject>()!;
            if (jObject.TryGetValue("ForceSpawnData", out JToken jArray))
            {
                var forceList = InjectLibJSON.Deserialize<List<ConsumableForceData>>(jArray.ToString());
                var data = result.Cast<ConsumableDistributionDataBlock>();
                data.SetCDForceList(forceList);
            }
        }
    }
}
