﻿using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Pk3DSRNGTool.RNG;
using static PKHeX.Util;
using static Pk3DSRNGTool.StringItem;

namespace Pk3DSRNGTool
{
    public partial class Gen7MainRNGTool : Form
    {
        public Gen7MainRNGTool()
        {
            InitializeComponent();
        }
        private void Gen7MainRNGTool_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            this.Parent = null;
            e.Cancel = true;
        }

        private static int lindex => Program.mainform.lindex;
        private static uint Seed { set => Program.mainform.globalseed = value; }
        private int MinFrame => FuncUtil.getstartingframe(Program.mainform.Ver, RB_ID.Checked ? 4 : 0);
        private static bool IsUltra => Program.mainform.Ver > 6;

        #region Controls
        private void Clear_Click(object sender, EventArgs e)
        {
            Clock_List.Text = "";
        }
        private void Back_Click(object sender, EventArgs e)
        {
            string str = Clock_List.Text;
            if (Clock_List.Text != "")
            {
                if (str.LastIndexOf(',') != -1)
                    str = str.Remove(str.LastIndexOf(','));
                else
                    str = "";
            }
            Clock_List.Text = str;
        }
        private void Get_Clock_Number(object sender, EventArgs e)
        {
            string str = ((Button)sender).Name;
            string n = str.Remove(0, 6);
            int offset = 17 - (int)Offset.Value;
            if (Clock_List.Text != "") Clock_List.Text += ",";
            Clock_List.Text += !EndClockInput.Checked || RB_QR.Checked ? n : ((Convert.ToInt32(n) + offset) % 17).ToString();

            Search_Click(null, null);
        }
        private void RB_CheckedChanged(object sender, EventArgs e)
        {
            Offset.Enabled = StartClockInput.Enabled = EndClockInput.Enabled = RB_SaveScreen.Checked;
            (StartClockInput.Enabled ? EndClockInput : StartClockInput).Checked = true;
        }
        private void Search_Click(object sender, EventArgs e)
        {
            SeedResults.Visible = !RB_QR.Checked;
            ListResults.Visible = RB_QR.Checked;
            if (RB_QR.Checked)
            {
                QRSearch();
                return;
            }
            SearchSeed();
        }
        public void UpdatePara(decimal npc = -1, decimal target = -1, bool? raining = null)
        {
            if (npc >= 0)
                NPC.Value = npc;
            if (target >= 0)
                TargetFrame.Value = target;
            if (raining != null)
                Raining.Checked = raining ?? false;
        }
        private void Startup_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.OpenGen7Tool = Startup.Checked;
        }
        #endregion
        #region Search
        private void SearchSeed()
        {
            if (Program.mainform.Ver < 5)
                Program.mainform.Ver = 5;
            L_Gameversion.Text = Program.mainform.VersionStr;
            if (Clock_List.Text.Count(c => c == ',') < 7)
            {
                SeedResults.Text = "";
                return;
            }
            var text = "";
            try
            {
                SeedResults.Text = WAIT_STR[lindex];
                var results = SFMTSeedAPI.request(Clock_List.Text, RB_ID.Checked, IsUltra);
                if (!results?.Any() ?? true)
                    text = NORESULT_STR[lindex];
                else
                {
                    text = string.Join(" ", results.Select(r => r.seed));
                    if (results.Count() == 1)
                    {
                        if (RB_SaveScreen.Checked)
                            Program.mainform.Framemin = Time_min.Value = MinFrame + Clock_List.Text.Count(c => c == ',');
                        else
                            Program.mainform.IDCorrection = results.FirstOrDefault().add;
                        if (uint.TryParse(text, System.Globalization.NumberStyles.HexNumber, null, out uint s0))
                            Seed = s0;
                    }
                }
            }
            catch (Exception exc)
            {
                text = exc.Message;
            }
            finally
            {
                SeedResults.Text = text;
            }
        }

        private void QRSearch()
        {
            if (Clock_List.Text.Count(c => c == ',') < 3)
                return;
            int min = (int)Frame_min.Value;
            int max = (int)Frame_max.Value;
            if (Clock_List.Text == "")
                return;
            string[] str = Clock_List.Text.Split(',');
            try
            {
                int[] Clk_List = str.Select(s => int.Parse(s)).ToArray();
                int[] temp_List = new int[Clk_List.Length];

                SFMT sfmt = new SFMT(Program.mainform.globalseed);
                SFMT seed = (SFMT)sfmt.DeepCopy();

                ListResults.Items.Clear();

                for (int i = 0; i < min; i++)
                    sfmt.Next();

                int tmp = 0;
                for (int i = min; i <= max; i++, sfmt.Next())
                {
                    seed = (SFMT)sfmt.DeepCopy();

                    for (int j = 0; j < Clk_List.Length; j++)
                        temp_List[j] = (int)(seed.Nextulong() % 17);

                    if (temp_List.SequenceEqual(Clk_List))
                    {
                        ListResults.Items.Add(string.Format(QR_STR[lindex], i + Clk_List.Length - 1, i + Clk_List.Length + 1));
                        tmp = i + Clk_List.Length + 1;
                    }
                }
                if (ListResults.Items.Count == 1)
                    Program.mainform.Framemin = Time_min.Value = tmp;
            }
            catch
            {
                Error(INVALID_STR[lindex]);
            }
        }
        #endregion
        #region TimerCalculateFunction
        private void CalcTime_Output(int min, int max, bool fidget, bool raining)
        {
            int[] totaltime = FuncUtil.CalcFrame(Program.mainform.globalseed, min, max, (byte)(NPC.Value + 1), fidget, raining);
            double realtime = totaltime[0] / 30.0;
            string str = $" {totaltime[0] * 2}F ({realtime.ToString("F")}s) <{totaltime[1] * 2}F>. ";
            str = string.Format(TIMER_STR[lindex], str);
            MessageBox.Show(str, "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CalcTime(object sender, EventArgs e)
        {
            int min = (int)Time_min.Value;
            int max = (int)TargetFrame.Value;
            bool fidget = Fidget.Checked;
            bool raining = Raining.Checked;
            CalcTime_Output(min, max, fidget, raining);
        }
        #endregion
    }
}
