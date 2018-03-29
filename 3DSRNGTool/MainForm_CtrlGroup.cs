﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Pk3DSRNGTool
{
    public partial class MainForm : Form
    {
        private static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }
        public int[] IVup
        {
            get => new[] { (int)ivmax0.Value, (int)ivmax1.Value, (int)ivmax2.Value, (int)ivmax3.Value, (int)ivmax4.Value, (int)ivmax5.Value, };
            set
            {
                if (value.Length < 6) return;
                ivmax0.Value = value[0]; ivmax1.Value = value[1]; ivmax2.Value = value[2];
                ivmax3.Value = value[3]; ivmax4.Value = value[4]; ivmax5.Value = value[5];
            }
        }
        public int[] IVlow
        {
            get => new[] { (int)ivmin0.Value, (int)ivmin1.Value, (int)ivmin2.Value, (int)ivmin3.Value, (int)ivmin4.Value, (int)ivmin5.Value, };
            set
            {
                if (value.Length < 6) return;
                ivmin0.Value = value[0]; ivmin1.Value = value[1]; ivmin2.Value = value[2];
                ivmin3.Value = value[3]; ivmin4.Value = value[4]; ivmin5.Value = value[5];
            }
        }
        private int[] BS
        {
            get => new[] { (int)BS_0.Value, (int)BS_1.Value, (int)BS_2.Value, (int)BS_3.Value, (int)BS_4.Value, (int)BS_5.Value, };
            set
            {
                if (value.Length < 6) return;
                BS_0.Value = value[0]; BS_1.Value = value[1]; BS_2.Value = value[2];
                BS_3.Value = value[3]; BS_4.Value = value[4]; BS_5.Value = value[5];
            }
        }
        private int[] Stats
        {
            get => new[] { (int)Stat0.Value, (int)Stat1.Value, (int)Stat2.Value, (int)Stat3.Value, (int)Stat4.Value, (int)Stat5.Value, };
            set
            {
                if (value.Length < 6) return;
                Stat0.Value = value[0]; Stat1.Value = value[1]; Stat2.Value = value[2];
                Stat3.Value = value[3]; Stat4.Value = value[4]; Stat5.Value = value[5];
            }
        }
        public int[] IV_Male
        {
            get => new[] { (int)M_IV0.Value, (int)M_IV1.Value, (int)M_IV2.Value, (int)M_IV3.Value, (int)M_IV4.Value, (int)M_IV5.Value, };
            set
            {
                if (value.Length < 6) return;
                M_IV0.Value = value[0]; M_IV1.Value = value[1]; M_IV2.Value = value[2];
                M_IV3.Value = value[3]; M_IV4.Value = value[4]; M_IV5.Value = value[5];
            }
        }
        public int[] IV_Female
        {
            get => new[] { (int)F_IV0.Value, (int)F_IV1.Value, (int)F_IV2.Value, (int)F_IV3.Value, (int)F_IV4.Value, (int)F_IV5.Value, };
            set
            {
                if (value.Length < 6) return;
                F_IV0.Value = value[0]; F_IV1.Value = value[1]; F_IV2.Value = value[2];
                F_IV3.Value = value[3]; F_IV4.Value = value[4]; F_IV5.Value = value[5];
            }
        }
        private uint[] Status
        {
            get => new[] { St0.Value, St1.Value, St2.Value, St3.Value };
            set
            {
                if (value.Length < 4) return;
                St0.Value = value[0]; St1.Value = value[1];
                St2.Value = value[2]; St3.Value = value[3];
            }
        }
        private string[] STATS_STR => new[] { L_H.Text, L_A.Text, L_B.Text, L_C.Text, L_D.Text, L_S.Text, };
        private NumericUpDown[] EventIV => new[] { EventIV0, EventIV1, EventIV2, EventIV3, EventIV4, EventIV5, };
        private CheckBox[] EventIVLocked => new[] { Event_IV_Fix0, Event_IV_Fix1, Event_IV_Fix2, Event_IV_Fix3, Event_IV_Fix4, Event_IV_Fix5, };
        private List<Controls.ComboItem> Locationlist = new List<Controls.ComboItem>();
    }
}