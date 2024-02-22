using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.SQL;

namespace WowPacketParser.Store.Objects
{
    [DBTableName("gameobject_questender")]
    public sealed record GameObjectQuestEnder : IDataModel
    {
        [DBFieldName("id", true)]
        public uint? GameObjectID;

        [DBFieldName("quest", true)]
        public uint? QuestID;

        [DBFieldName("VerifiedBuild", TargetedDatabaseFlag.SinceShadowlands)]
        public int? VerifiedBuild = ClientVersion.BuildInt;
    }
}
