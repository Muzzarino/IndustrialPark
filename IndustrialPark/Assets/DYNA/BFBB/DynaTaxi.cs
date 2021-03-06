﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaTaxi : DynaBase
    {
        public override string Note => "Version is always 1";

        public DynaTaxi() : base()
        {
            MRKR_ID = 0;
            CAM_ID = 0;
            PORT_ID = 0;
            DYNA_Talkbox_ID = 0;
            TEXT_ID = 0;
            SIMP_ID = 0;
        }

        public DynaTaxi(IEnumerable<byte> enumerable) : base (enumerable)
        {
            MRKR_ID = Switch(BitConverter.ToUInt32(Data, 0x0));
            CAM_ID = Switch(BitConverter.ToUInt32(Data, 0x4));
            PORT_ID = Switch(BitConverter.ToUInt32(Data, 0x8));
            DYNA_Talkbox_ID = Switch(BitConverter.ToUInt32(Data, 0xC));
            TEXT_ID = Switch(BitConverter.ToUInt32(Data, 0x10));
            SIMP_ID = Switch(BitConverter.ToUInt32(Data, 0x14));
            InvisibleTimer = Switch(BitConverter.ToSingle(Data, 0x18));
            TeleportTimer = Switch(BitConverter.ToSingle(Data, 0x1C));
        }

        public override bool HasReference(uint assetID)
        {
            if (MRKR_ID == assetID)
                return true;
            if (CAM_ID == assetID)
                return true;
            if (PORT_ID == assetID)
                return true;
            if (DYNA_Talkbox_ID == assetID)
                return true;
            if (TEXT_ID == assetID)
                return true;
            if (SIMP_ID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(MRKR_ID, ref result);
            Asset.Verify(CAM_ID, ref result);
            Asset.Verify(PORT_ID, ref result);
            Asset.Verify(DYNA_Talkbox_ID, ref result);
            Asset.Verify(TEXT_ID, ref result);
            Asset.Verify(SIMP_ID, ref result);
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(MRKR_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(CAM_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(PORT_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(DYNA_Talkbox_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(TEXT_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(SIMP_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(InvisibleTimer)));
            list.AddRange(BitConverter.GetBytes(Switch(TeleportTimer)));
            return list.ToArray();
        }

        public AssetID MRKR_ID { get; set; }
        public AssetID CAM_ID { get; set; }
        public AssetID PORT_ID { get; set; }
        public AssetID DYNA_Talkbox_ID { get; set; }
        public AssetID TEXT_ID { get; set; }
        public AssetID SIMP_ID { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float InvisibleTimer{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float TeleportTimer{ get; set; }
    }
}