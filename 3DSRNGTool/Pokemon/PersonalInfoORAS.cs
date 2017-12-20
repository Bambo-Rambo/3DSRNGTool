﻿using System;

namespace PKHeX.Core
{
    public class PersonalInfoORAS : PersonalInfo
    {
        public const int SIZE = 0x50;
        protected PersonalInfoORAS() { }
        public PersonalInfoORAS(byte[] data)
        {
            if (data.Length != SIZE)
                return;
            Data = data;
        }
        public override byte[] Write()
        {
            return Data;
        }

        public override int HP { get => Data[0x00]; set => Data[0x00] = (byte)value; }
        public override int ATK { get => Data[0x01]; set => Data[0x01] = (byte)value; }
        public override int DEF { get => Data[0x02]; set => Data[0x02] = (byte)value; }
        public override int SPE { get => Data[0x03]; set => Data[0x03] = (byte)value; }
        public override int SPA { get => Data[0x04]; set => Data[0x04] = (byte)value; }
        public override int SPD { get => Data[0x05]; set => Data[0x05] = (byte)value; }
        public override int[] Types
        {
            get => new int[] { Data[0x06], Data[0x07] };
            set
            {
                if (value?.Length != 2) return;
                Data[0x06] = (byte)value[0];
                Data[0x07] = (byte)value[1];
            }
        }
        public override int CatchRate { get => Data[0x08]; set => Data[0x08] = (byte)value; }
        public override int Gender { get => Data[0x12]; set => Data[012] = (byte)value; }
        public override int[] Items
        {
            get => new int[] { BitConverter.ToInt16(Data, 0xC), BitConverter.ToInt16(Data, 0xE), BitConverter.ToInt16(Data, 0x10) };
            set
            {
                if (value?.Length != 3) return;
                BitConverter.GetBytes((short)value[0]).CopyTo(Data, 0xC);
                BitConverter.GetBytes((short)value[1]).CopyTo(Data, 0xE);
                BitConverter.GetBytes((short)value[2]).CopyTo(Data, 0x10);
            }
        }
        public override int[] EggGroups
        {
            get => new int[] { Data[0x16], Data[0x17] };
            set
            {
                if (value?.Length != 2) return;
                Data[0x16] = (byte)value[0];
                Data[0x17] = (byte)value[1];
            }
        }
        public override int[] Abilities
        {
            get => new int[] { Data[0x18], Data[0x19], Data[0x1A] };
            set
            {
                if (value?.Length != 3) return;
                Data[0x18] = (byte)value[0];
                Data[0x19] = (byte)value[1];
                Data[0x1A] = (byte)value[2];
            }
        }
        protected internal override int FormStatsIndex { get => BitConverter.ToUInt16(Data, 0x1C); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1C); }
        public override int FormeCount { get => Data[0x20]; set => Data[0x20] = (byte)value; }
    }
}
