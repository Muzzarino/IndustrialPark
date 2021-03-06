﻿using System;
using System.Collections.Generic;
using SharpDX;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaTeleport_MovieGame : DynaBase
    {
        private string _note = "Version is always 2?";
        public override string Note => _note;

        public DynaTeleport_MovieGame() : base()
        {
            MRKR_ID = 0;
            DYNA_Teleport_ID = 0;
            UnknownAssetID = 0;
        }

        public DynaTeleport_MovieGame(IEnumerable<byte> enumerable) : base (enumerable)
        {
            MRKR_ID = Switch(BitConverter.ToUInt32(Data, 0x0));
            UnknownInt = Switch(BitConverter.ToInt32(Data, 0x4));
            PlayerRotation = Switch(BitConverter.ToInt32(Data, 0x8));
            DYNA_Teleport_ID = Switch(BitConverter.ToUInt32(Data, 0x0C));
            try
            {
                UnknownAssetID = Switch(BitConverter.ToUInt32(Data, 0x10));
            }
            catch
            {
                UnknownAssetID = 0;
                _note = "UnknownAssetID is unused in this instance.";
            }
        }

        public override bool HasReference(uint assetID)
        {
            return MRKR_ID == assetID || DYNA_Teleport_ID == assetID || UnknownAssetID == assetID;
        }

        public override void Verify(ref List<string> result)
        {
            if (MRKR_ID == 0)
                result.Add("DYNA Teleport with no MRKR reference");
            Asset.Verify(MRKR_ID, ref result);
            if (DYNA_Teleport_ID == 0)
                result.Add("DYNA Teleport with no DYNA Teleport reference");
            Asset.Verify(DYNA_Teleport_ID, ref result);
            Asset.Verify(UnknownAssetID, ref result);
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(MRKR_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt)));
            list.AddRange(BitConverter.GetBytes(Switch(PlayerRotation)));
            list.AddRange(BitConverter.GetBytes(Switch(DYNA_Teleport_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownAssetID)));
            return list.ToArray();
        }

        public AssetID MRKR_ID { get; set; }
        public int UnknownInt { get; set; }
        public int PlayerRotation { get; set; }
        public AssetID DYNA_Teleport_ID { get; set; }
        public AssetID UnknownAssetID { get; set; }
    }
}