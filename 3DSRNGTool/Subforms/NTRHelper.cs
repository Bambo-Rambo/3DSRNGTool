﻿using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pk3DSRNGTool
{
    public partial class NTRHelper : Form
    {
        public static NtrClient ntrclient;

        public NTRHelper()
        {
            InitializeComponent();
            IP.Text = Properties.Settings.Default.IP;
            ntrclient = new NtrClient();
            ntrclient.Connect += OnConnected;
            ntrclient.Message += OnMsgArrival;
            ntrclient.setServer(IP.Text, 8000);
            ORAS_Button.Checked = Program.mainform.IsORAS;
        }
        private void NTRHelper_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            this.Parent = null;
            e.Cancel = true;
        }
        public void SetGame(byte Version)
        {
            if (Version < 2)
                XY_Button.Checked = true;
            else if (Version < 4)
                ORAS_Button.Checked = true;

        }

        public void Connect(bool OneClick)
        {
            B_Connect_Click(OneClick ? B_OneClick : null, null);
        }

        private void B_Connect_Click(object sender, EventArgs e)
        {
            ntrclient.OneClick = sender == B_OneClick;
            L_NTRLog.Text = "Connecting...";
            ntrclient.setServer(IP.Text, 8000);
            try
            {
                ntrclient.connectToServer();
            }
            catch
            {
                OnDisconnected(false);
                FormUtil.Error("Unable to connect the console");
            }
        }

        public void B_Disconnect_Click(object sender, EventArgs e)
        {
            ntrclient.disconnect();
            OnDisconnected();
        }

        private void OnDisconnected(bool Success = true)
        {
            NTR_Timer.Enabled = false;
            B_Connect.Enabled = true;
            L_NTRLog.Text = Success ? "Disconnected" : "No Connection";
            B_Disconnect.Enabled = false;
            Program.mainform.OnConnected_Changed(false);
        }

        private void OnConnected(object sender, EventArgs e)
        {
            if (ntrclient.port == 8000)
            {
                NTR_Timer.Enabled = true;
                L_NTRLog.Text = "Console Connected";
                B_Connect.Enabled = false;
                B_Disconnect.Enabled = true;
                Program.mainform.OnConnected_Changed(true);
                Properties.Settings.Default.IP = IP.Text;
                ntrclient.listprocess();
            }
        }

        private void OnMsgArrival(object sender, NtrClient.InfoEventArgs e)
        {
            Invoke(new Action(() =>
            {
                switch (e.info)
                {
                    case "Disconnect":
                        B_Disconnect_Click(null, null);
                        return;
                    case null:
                        L_NTRLog.Text = (string)e.data;
                        return;
                }
                Program.mainform.parseNTRInfo(e.info, e.data);
            }));
        }

        private void NTRTick(object sender, EventArgs e)
        {
            try { ntrclient.sendHeartbeatPacket(); } catch { }
        }

        private void B_Help_Click(object sender, EventArgs e) =>
            System.Diagnostics.Process.Start(StringItem.GITHUB + "wiki/NTR-Helper-Usage");

        #region Bots
        private void Start()
        {
            ntrclient.CheckSocket();
            B_MashA.Enabled = B_A.Enabled = B_Begin.Enabled = false;
            B_Stop.Enabled = true;
        }

        private void XY_Button_CheckedChanged(object sender, EventArgs e)
        {
            SeedDelay1.Value = XY_Button.Checked ? 1800 : 2500;
            SeedDelay2.Value = XY_Button.Checked ? 1100 : 1600;
            SeedDelay3.Value = XY_Button.Checked ? 800 : 1200;
            SeedDelay4.Value = XY_Button.Checked ? 1500 : 2000;
            SeedDelay5.Value = XY_Button.Checked ? 300 : 600;
        }
        private void B_Start_Click(object sender, EventArgs e)
        {
            try
            {
                if (BotList.SelectedIndex == 1)
                {
                    int Ver = Program.mainform.Ver;
                    if (Ver < 2 || Ver == 4) // XY or transporter
                        return;
                    Start();
                    if (Ver > 4)
                        G7IDBot();
                    if (Ver < 4)
                        G6IDBot();
                }
                else
                {
                    Start();
                    G6SeedBot();
                }
                
            }
            catch { }
        }

        private void B_Stop_Click(object sender, EventArgs e)
        {
            B_MashA.Enabled = B_A.Enabled = B_Begin.Enabled = true;
            B_Stop.Enabled = false;
            ntrclient.disconnect();
        }

        private bool Botting => B_Stop.Enabled;
        private int delaylevel => 5 - (int)Speed.Value;
        private int Delay1 => 500 + 100 * delaylevel;
        private int Delay2 => 3800 + 200 * delaylevel;
        private int Delay3 => 2100 + 100 * delaylevel;
        private int Delay4 => 5000 + 200 * delaylevel;
        private int Delay5 => 1500 + 100 * delaylevel;

        private async void G7IDBot()
        {
            int CurrFrame = (int)StartFrame.Value;
            while (Botting && CurrFrame < (int)StopFrame.Value)
            {
                // Keyboard Input
                ntrclient.TouchCenter(); L_NTRLog.Text = "Character input";
                await Task.Delay(Delay1);
                // Confirm
                ntrclient.Confirm(JPN.Checked); L_NTRLog.Text = "Enter pressed";
                await Task.Delay(Delay2);
                // Discard
                ntrclient.PressB(); L_NTRLog.Text = "B pressed";
                await Task.Delay(Delay3);
                StartFrame.Value = ++CurrFrame;
            }
            B_Stop_Click(null, null);
        }

        private async void G6IDBot()
        {
            int CurrFrame = (int)StartFrame.Value;
            while (Botting && CurrFrame < (int)StopFrame.Value - 1)
            {
                // Choose gender
                ntrclient.PressA(); L_NTRLog.Text = "A pressed - 1";
                await Task.Delay(Delay5);
                // Confirm gender
                ntrclient.PressA(); L_NTRLog.Text = "A pressed - 2";
                await Task.Delay(Delay5);
                StartFrame.Value = ++CurrFrame;
                // Keyboard Input
                ntrclient.TouchCenter(); L_NTRLog.Text = "Character input";
                await Task.Delay(Delay1);
                // Dialogue-1 
                ntrclient.Confirm(JPN.Checked); L_NTRLog.Text = "Enter pressed";
                await Task.Delay(Delay4);
                // Discard
                ntrclient.PressB(); L_NTRLog.Text = "B pressed";
                await Task.Delay(Delay5);
                // Dialogue-2
                ntrclient.PressA(); L_NTRLog.Text = "A pressed";
                await Task.Delay(Delay1);
            }
            B_Stop_Click(null, null);
        }

        private async void G6SeedBot()
        {
            L_Count.Text = "0";
            uint Count = 0;
            B_Disconnect.PerformClick();
            await Task.Delay(500);
            do
            {
                Count++;
                await Task.Delay(100);

                B_B.PerformClick();
                await Task.Delay((int)SeedDelay1.Value);

                B_Start.PerformClick();
                await Task.Delay((int)SeedDelay2.Value);

                B_OneClick.PerformClick();
                await Task.Delay((int)SeedDelay3.Value);

                B_OneClick.PerformClick();
                await Task.Delay((int)SeedDelay4.Value);

                L_Count.Text = Count.ToString();

                B_Disconnect.PerformClick();
                await Task.Delay((int)SeedDelay5.Value);
            }
            while (Botting && Program.mainform.DGV.Rows.Count == 0);
            B_Stop_Click(null, null);
            Program.mainform.Focus();
        }

        private void B_A_Click(object sender, EventArgs e)
        {
            try 
            { 
                ntrclient.CheckSocket(); 
                ntrclient.PressA(); 
            } 
            catch { }
        }
        private void B_B_Click(object sender, EventArgs e)
        {
            try
            {
                ntrclient.CheckSocket();
                ntrclient.PressB();
            }
            catch { }
        }
        private void B_Start_Click_1(object sender, EventArgs e)
        {
            try
            {
                ntrclient.CheckSocket();
                ntrclient.PressStart();
            }
            catch { }
        }
        private void B_Select_Click(object sender, EventArgs e)
        {
            try
            {
                ntrclient.CheckSocket();
                ntrclient.PressSelect();
            }
            catch { }
        }
        private void B_MashA_Click(object sender, EventArgs e)
        {
            try { Start(); MashA(); } catch { }
        }
        private async void MashA()
        {
            while (Botting)
            {
                ntrclient.PressA(); L_NTRLog.Text = "A mashing";
                await Task.Delay(Delay1);
            }
        }
        #endregion

    }
}
