﻿using System.Text;
using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Parsing;

namespace WowPacketParserModule.V4_4_0_54481.Parsers
{
    public static class SessionHandler
    {
        [Parser(Opcode.SMSG_AUTH_CHALLENGE)]
        public static void HandleServerAuthChallenge(Packet packet)
        {
            for (uint i = 0; i < 8; ++i)
                packet.ReadUInt32("DosChallenge", i);
            packet.ReadBytes("Challenge", 16);
            packet.ReadByte("DosZeroBits");
        }

        [Parser(Opcode.CMSG_AUTH_SESSION)]
        public static void HandleAuthSession(Packet packet)
        {
            packet.ReadUInt64("DosResponse");
            packet.ReadUInt32("RegionID");
            packet.ReadUInt32("BattlegroupID");
            packet.ReadUInt32("RealmID");
            packet.ReadBytes("LocalChallenge", 16);
            packet.ReadBytes("Digest", 24);
            packet.ReadBit("UseIPv6");

            var realmJoinTicketSize = packet.ReadInt32();
            packet.ReadBytes("RealmJoinTicket", realmJoinTicketSize);
        }

        [Parser(Opcode.SMSG_ENTER_ENCRYPTED_MODE)]
        public static void HandleEnterEncryptedMode(Packet packet)
        {
            packet.ReadBytes("Signature (ED25519)", 64);
            packet.ReadBit("Enabled");
        }

        [Parser(Opcode.SMSG_AUTH_RESPONSE)]
        public static void HandleAuthResponse(Packet packet)
        {
            packet.ReadUInt32E<BattlenetRpcErrorCode>("Result");

            var ok = packet.ReadBit("Success");
            var queued = packet.ReadBit("Queued");
            if (ok)
            {
                packet.ReadUInt32("VirtualRealmAddress");
                var realms = packet.ReadUInt32();
                packet.ReadUInt32("TimeRested");
                packet.ReadByte("ActiveExpansionLevel");
                packet.ReadByte("AccountExpansionLevel");
                packet.ReadUInt32("TimeSecondsUntilPCKick");
                var classes = packet.ReadUInt32("AvailableClasses");
                var templates = packet.ReadUInt32("Templates");
                packet.ReadUInt32("AccountCurrency");

                if (ClientVersion.AddedInVersion(ClientVersionBuild.V9_0_5_37503) &&
                    ClientVersion.Expansion != ClientType.Classic)
                    packet.ReadTime64("Time");
                else
                    packet.ReadTime("Time");

                if (ClientVersion.AddedInVersion(ClientVersionBuild.V8_3_0_33062))
                {
                    for (var i = 0; i < classes; ++i)
                    {
                        packet.ReadByteE<Race>("RaceID", "AvailableClasses", i);
                        var classesForRace = packet.ReadUInt32();
                        for (var j = 0u; j < classesForRace; ++j)
                        {
                            packet.ReadByteE<Class>("ClassID", "AvailableClasses", i, "Classes", j);
                            packet.ReadByteE<ClientType>("ActiveExpansionLevel", "AvailableClasses", i, "Classes", j);
                            packet.ReadByteE<ClientType>("AccountExpansionLevel", "AvailableClasses", i, "Classes", j);
                            if (ClientVersion.AddedInVersion(ClientVersionBuild.V10_0_2_46479))
                                packet.ReadByte("MinActiveExpansionLevel", "AvailableClasses", i, "Classes", j);
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < classes; ++i)
                    {
                        packet.ReadByteE<Class>("Class", "AvailableClasses", i);
                        packet.ReadByteE<ClientType>("RequiredExpansion", "AvailableClasses", i);
                    }
                }

                packet.ResetBitReader();
                packet.ReadBit("IsExpansionTrial");
                packet.ReadBit("ForceCharacterTemplate");
                var horde = packet.ReadBit(); // NumPlayersHorde
                var alliance = packet.ReadBit(); // NumPlayersAlliance
                var trialExpiration = packet.ReadBit(); // ExpansionTrialExpiration
                var hasNewBuildKeys = false;
                if (ClientVersion.AddedInVersion(ClientVersionBuild.V10_1_5_50232))
                    hasNewBuildKeys = packet.ReadBit();

                packet.ResetBitReader();
                packet.ReadUInt32("BillingPlan");
                packet.ReadUInt32("TimeRemain");
                packet.ReadUInt32("Unk_V7_3_5");

                packet.ReadBit("InGameRoom");
                packet.ReadBit("InGameRoom");
                packet.ReadBit("InGameRoom");

                if (horde)
                    packet.ReadUInt16("NumPlayersHorde");

                if (alliance)
                    packet.ReadUInt16("NumPlayersAlliance");

                if (trialExpiration)
                {
                    if (ClientVersion.AddedInVersion(ClientVersionBuild.V10_0_2_46479))
                        packet.ReadInt64("ExpansionTrialExpiration");
                    else
                        packet.ReadInt32("ExpansionTrialExpiration");
                }

                if (hasNewBuildKeys)
                {
                    var newBuildKey = new byte[16];
                    var someKey = new byte[16];
                    for (var i = 0; i < 16; i++)
                    {
                        newBuildKey[i] = packet.ReadByte();
                        someKey[i] = packet.ReadByte();
                    }
                    packet.AddValue("NewBuildKey", Encoding.UTF8.GetString(newBuildKey));
                    packet.AddValue("SomeKey", Encoding.UTF8.GetString(someKey));
                }

                for (var i = 0; i < realms; ++i)
                {
                    packet.ReadUInt32("RealmAddress", "VirtualRealms", i);
                    packet.ResetBitReader();
                    packet.ReadBit("IsLocal", "VirtualRealms", i);
                    packet.ReadBit("IsInternalRealm", "VirtualRealms", i);

                    var bitsCount = 8;

                    if (ClientVersion.AddedInVersion(ClientVersionBuild.V8_1_0_28724) && ClientVersion.RemovedInVersion(ClientVersionBuild.V8_1_5_29683))
                        bitsCount = 9;

                    var nameLen1 = packet.ReadBits(bitsCount);
                    var nameLen2 = packet.ReadBits(bitsCount);
                    packet.ReadWoWString("RealmNameActual", nameLen1, "VirtualRealms", i);
                    packet.ReadWoWString("RealmNameNormalized", nameLen2, "VirtualRealms", i);
                }

                for (var i = 0; i < templates; ++i)
                {
                    packet.ReadUInt32("TemplateSetId", i);
                    var templateClasses = packet.ReadUInt32();
                    for (var j = 0; j < templateClasses; ++j)
                    {
                        packet.ReadByteE<Class>("Class", i, j);
                        packet.ReadByte("FactionGroup", i, j);
                    }

                    packet.ResetBitReader();
                    var nameLen = packet.ReadBits(7);
                    var descLen = packet.ReadBits(10);
                    packet.ReadWoWString("Name", nameLen, i);
                    packet.ReadWoWString("Description", descLen, i);
                }
            }

            if (queued)
            {
                packet.ReadUInt32("WaitCount");
                packet.ReadUInt32("WaitTime");
                if (ClientVersion.AddedInVersion(ClientVersionBuild.V10_2_5_52902))
                    packet.ReadUInt32("AllowedFactionGroupForCharacterCreate");
                packet.ResetBitReader();
                packet.ReadBit("HasFCM");
                if (ClientVersion.AddedInVersion(ClientVersionBuild.V10_2_5_52902))
                    packet.ReadBit("CanCreateOnlyIfExisting");
            }
        }

        [Parser(Opcode.SMSG_SET_TIME_ZONE_INFORMATION)]
        public static void HandleSetTimeZoneInformation(Packet packet)
        {
            var len1 = packet.ReadBits(7);
            var len2 = packet.ReadBits(7);
            var len3 = packet.ReadBits(7);

            packet.ReadWoWString("ServerTimeTZ", len1);
            packet.ReadWoWString("GameTimeTZ", len2);
            packet.ReadWoWString("ServerRegionalTZ", len3);
        }

        [Parser(Opcode.SMSG_UPDATE_BNET_SESSION_KEY)]
        public static void HandleUpdateBnetSessionKey(Packet packet)
        {
            var sessionKeyLength = (int)packet.ReadBits(7);

            packet.ReadBytes("Digest", 32);
            packet.ReadBytes("SessionKey", sessionKeyLength);
        }

        [Parser(Opcode.CMSG_ENTER_ENCRYPTED_MODE_ACK)]
        public static void HandleSessionZero(Packet packet)
        {
        }
    }
}
