using GameData;

namespace ForcedConsumableSpawns.CustomFields
{
    public sealed class PickupForceData
    {
        public uint ItemID { get; set; } = 0;
        public uint Count { get; set; } = 1;
        public ZonePlacementWeights? PlacementWeights { get; set; }
    }
}
