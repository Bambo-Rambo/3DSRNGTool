﻿using System.Linq;
using Pk3DSRNGTool.RNG;
using Pk3DSRNGTool.Core;

namespace Pk3DSRNGTool
{
    public class Egg6 : EggRNG
    {
        private static MersenneTwister mt = new MersenneTwister(19650218u);
        private static uint getmtrand => RNGPool.getrand;
        private static uint getrand => mt.Nextuint();
        private static uint rand(uint n) => (uint)(getrand * (ulong)n >> 32);
        private static bool rand2 => getrand < 0x80000000;
        public bool IsMainRNGEgg;

        public override RNGResult Generate() => Generate(IsMainRNGEgg ? getmtrand : (uint?)null, new uint[] { getmtrand, getmtrand });

        public RNGResult Generate(uint? MainRNGPID, uint[] key)
        {
            ResultE6 egg = new ResultE6(key);

            mt.Reseed(key);

            // Gender
            if (NidoType)
                egg.Gender = (byte)(rand2 ? 1 : 2);
            else
                egg.Gender = (byte)(RandomGender ? (rand(252) >= Gender ? 1 : 2) : Gender);

            // Nature
            egg.Nature = (byte)rand(25);

            // Everstone
            // Chooses which parent if necessary;
            if (Both_Everstone)
                egg.BE_InheritParents = rand2;
            else if (EverStone)
                egg.BE_InheritParents = MaleItem == 1;

            // Ability
            egg.Ability = (byte)getRandomAbility(InheritAbility, rand(100));

            // PowerItem
            // Chooses which parent if necessary
            if (Both_Power)
            {
                if (rand2)
                    egg.InheritMaleIV[M_Power] = true;
                else
                    egg.InheritMaleIV[F_Power] = false;
            }
            else if (Power)
            {
                if (MaleItem > 2)
                    egg.InheritMaleIV[M_Power] = true;
                else
                    egg.InheritMaleIV[F_Power] = false;
            }

            // Inherit IV
            uint tmp;
            for (int i = Power ? 1 : 0; i < InheritIVs_Cnt; i++)
            {
                do { tmp = rand(6); }
                while (egg.InheritMaleIV[tmp] != null);
                egg.InheritMaleIV[tmp] = rand2;
            }

            // IVs
            egg.IVs = new int[6];
            for (int j = 0; j < 6; j++)
            {
                egg.IVs[j] = (int)(getrand >> 27);
                if (egg.InheritMaleIV[j] == null) continue;
                egg.IVs[j] = (egg.InheritMaleIV[j] == true) ? MaleIVs[j] : FemaleIVs[j];
            }

            // Encryption Constant
            egg.EC = getrand;

            // PID
            if (MainRNGPID != null)
            {
                egg.PID = (uint)MainRNGPID;
                egg.Shiny = egg.PSV == TSV;
            }
            else
                for (int i = PID_Rerollcount; i > 0; i--)
                {
                    egg.PID = getrand;
                    if (egg.PSV == TSV) 
                    { 
                        egg.Shiny = true;
                        egg.SquareShiny = egg.PRV == TRV;
                        return egg; 
                    }
                }

            // Other TSVs
            tmp = egg.PSV;
            if (ConsiderOtherTSV && OtherTSVs.Contains((int)tmp))
                egg.Shiny = true;

            return egg;
        }
    }
}
