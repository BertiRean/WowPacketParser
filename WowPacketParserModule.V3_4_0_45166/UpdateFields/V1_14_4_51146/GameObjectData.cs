using WowPacketParser.Misc;
using WowPacketParser.Store.Objects.UpdateFields;

// This file is automatically generated, DO NOT EDIT

namespace WowPacketParserModule.V3_4_0_45166.UpdateFields.V1_14_4_51146
{
    public class GameObjectData : IMutableGameObjectData
    {
        public System.Nullable<int> DisplayID { get; set; }
        public System.Nullable<uint> SpellVisualID { get; set; }
        public System.Nullable<uint> StateSpellVisualID { get; set; }
        public System.Nullable<uint> SpawnTrackingStateAnimID { get; set; }
        public System.Nullable<uint> SpawnTrackingStateAnimKitID { get; set; }
        public System.Nullable<uint>[] StateWorldEffectIDs { get; set; }
        public WowGuid CreatedBy { get; set; }
        public WowGuid GuildGUID { get; set; }
        public System.Nullable<uint> Flags { get; set; }
        public Quaternion? ParentRotation { get; set; }
        public System.Nullable<int> FactionTemplate { get; set; }
        public System.Nullable<int> Level { get; set; }
        public System.Nullable<sbyte> State { get; set; }
        public System.Nullable<sbyte> TypeID { get; set; }
        public System.Nullable<byte> PercentHealth { get; set; }
        public System.Nullable<uint> ArtKit { get; set; }
        public System.Nullable<uint> CustomParam { get; set; }
        public DynamicUpdateField<System.Nullable<int>> EnableDoodadSets { get; } = new DynamicUpdateField<System.Nullable<int>>();
        public DynamicUpdateField<System.Nullable<int>> WorldEffects { get; } = new DynamicUpdateField<System.Nullable<int>>();
    }
}

