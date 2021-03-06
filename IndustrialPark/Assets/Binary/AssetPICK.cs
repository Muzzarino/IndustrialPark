﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class EntryPICK
    {
        public uint PickupHash { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte PickupType { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte PickupIndex { get; set; }
        [TypeConverter(typeof(HexShortTypeConverter))]
        public ushort PickupFlags { get; set; }
        public uint Quantity { get; set; }
        public AssetID ModelAssetID { get; set; }
        public AssetID AnimAssetID { get; set; }

        public EntryPICK()
        {
            ModelAssetID = 0;
        }

        public override string ToString()
        {
            return $"[{Program.MainForm.GetAssetNameFromID(PickupHash)}] - [{Program.MainForm.GetAssetNameFromID(ModelAssetID)}]";
        }
    }

    public class AssetPICK : Asset
    {
        public static Dictionary<uint, uint> pickEntries = new Dictionary<uint, uint>();

        public AssetPICK(Section_AHDR AHDR) : base(AHDR)
        {
            SetupDictionary();
        }

        private void SetupDictionary()
        {
            pickEntries.Clear();

            foreach (EntryPICK entry in PICK_Entries)
                pickEntries[entry.PickupHash] = entry.ModelAssetID;
        }

        public void ClearDictionary()
        {
            foreach (EntryPICK entry in PICK_Entries)
                pickEntries.Remove(entry.PickupHash);
        }

        public override bool HasReference(uint assetID)
        {
            foreach (EntryPICK a in PICK_Entries)
                if (a.ModelAssetID == assetID)
                    return true;
            
            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            foreach (EntryPICK a in PICK_Entries)
            {
                if (a.ModelAssetID == 0)
                    result.Add("PICK entry with ModelAssetID set to 0");
                Verify(a.ModelAssetID, ref result);
            }
        }

        [Category("Pickup Table")]
        public EntryPICK[] PICK_Entries
        {
            get
            {
                List<EntryPICK> entries = new List<EntryPICK>();
                int amount = ReadInt(0x4);

                for (int i = 0; i < amount; i++)
                {
                    entries.Add(new EntryPICK()
                    {
                        PickupHash = ReadUInt(8 + i * 0x14),
                        PickupType = ReadByte(12 + i * 0x14),
                        PickupIndex = ReadByte(13 + i * 0x14),
                        PickupFlags = ReadUShort(14 + i * 0x14),
                        Quantity = ReadUInt(16 + i * 0x14),
                        ModelAssetID = ReadUInt(20 + i * 0x14),
                        AnimAssetID = ReadUInt(24 + i * 0x14)
                    });
                }
                
                return entries.ToArray();
            }
            set
            {
                List<EntryPICK> newValues = value.ToList();

                List<byte> newData = Data.Take(4).ToList();
                newData.AddRange(BitConverter.GetBytes(Switch(newValues.Count)));

                foreach (EntryPICK i in newValues)
                {
                    newData.AddRange(BitConverter.GetBytes(Switch(i.PickupHash)));
                    newData.Add(i.PickupType);
                    newData.Add(i.PickupIndex);
                    newData.AddRange(BitConverter.GetBytes(Switch(i.PickupFlags)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.Quantity)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.ModelAssetID)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.AnimAssetID)));
                }
                
                Data = newData.ToArray();
            }
        }
    }
}