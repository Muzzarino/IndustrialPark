﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class EntryCOLL
    {
        public AssetID ModelAssetID { get; set; }
        public AssetID Collision_ModelAssetID { get; set; }
        public AssetID CameraCollision_ModelAssetID { get; set; }

        public EntryCOLL()
        {
            ModelAssetID = 0;
            Collision_ModelAssetID = 0;
            CameraCollision_ModelAssetID = 0;
        }

        public override string ToString()
        {
            if (Collision_ModelAssetID != 0)
                return $"[{Program.MainForm.GetAssetNameFromID(ModelAssetID)}] - [{Program.MainForm.GetAssetNameFromID(Collision_ModelAssetID)}]";
            return $"[{Program.MainForm.GetAssetNameFromID(ModelAssetID)}] - [{Program.MainForm.GetAssetNameFromID(CameraCollision_ModelAssetID)}]";
        }
        
        public override bool Equals(object obj)
        {
            if (obj != null && obj is EntryCOLL entryCOLL)
                return ModelAssetID == entryCOLL.ModelAssetID;
            return false;
        }

        public override int GetHashCode()
        {
            return ModelAssetID.GetHashCode();
        }
    }

    public class AssetCOLL : Asset
    {
        public AssetCOLL(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            foreach (EntryCOLL a in COLL_Entries)
                if (a.ModelAssetID == assetID || a.Collision_ModelAssetID == assetID || a.CameraCollision_ModelAssetID == assetID)
                    return true;
            
            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            foreach (EntryCOLL a in COLL_Entries)
            {
                if (a.ModelAssetID == 0)
                    result.Add("COLL entry with ModelAssetID set to 0");

                Verify(a.ModelAssetID, ref result);
                Verify(a.Collision_ModelAssetID, ref result);
                Verify(a.CameraCollision_ModelAssetID, ref result);
            }
        }

        [Category("Collision Table")]
        public EntryCOLL[] COLL_Entries
        {
            get
            {
                List<EntryCOLL> entries = new List<EntryCOLL>();
                int amount = ReadInt(0);

                for (int i = 0; i < amount; i++)
                {
                    entries.Add(new EntryCOLL()
                    {
                        ModelAssetID = ReadUInt(4 + i * 0xC),
                        Collision_ModelAssetID = ReadUInt(8 + i * 0xC),
                        CameraCollision_ModelAssetID = ReadUInt(12 + i * 0xC)
                    });
                }
                
                return entries.ToArray();
            }
            set
            {
                List<byte> newData = new List<byte>();
                newData.AddRange(BitConverter.GetBytes(Switch(value.Length)));

                foreach (EntryCOLL i in value)
                {
                    newData.AddRange(BitConverter.GetBytes(Switch(i.ModelAssetID)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.Collision_ModelAssetID)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.CameraCollision_ModelAssetID)));
                }
                
                Data = newData.ToArray();
            }
        }

        public void Merge(AssetCOLL assetCOLL)
        {
            List<EntryCOLL> entriesCOLL = COLL_Entries.ToList();
            List<uint> assetIDsAlreadyPresent = new List<uint>();

            foreach (EntryCOLL entryCOLL in entriesCOLL)
                assetIDsAlreadyPresent.Add(entryCOLL.ModelAssetID);

            foreach (EntryCOLL entryCOLL in assetCOLL.COLL_Entries)
                if (!assetIDsAlreadyPresent.Contains(entryCOLL.ModelAssetID))
                    entriesCOLL.Add(entryCOLL);

            COLL_Entries = entriesCOLL.ToArray();
        }
    }
}