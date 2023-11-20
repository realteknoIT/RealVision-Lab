using AForge.Video.DirectShow;
using RealVisionLab.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;

namespace RealVisionLab
{
    public partial class Property : Form
    {
#if DEBUG
        bool debugMode = true;
#else
        bool debugMode = false;
#endif

        public static FilterInfoCollection devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        public static VideoCaptureDevice[] cam;
        public static GroupBox[] gb, plcGb;
        public static TabPage[] tab, plcTab;
        public static Button[] camSet;
        public static Button[] camSetget;
        public static Button[] camSetset;
        public static ComboBox[] cmbCam;
        public static ComboBox[] cmbRes;
        public static Button[] btnAdd, plcBtnAdd;
        public static Panel[] p_rect, p_fonk;
        public static Button[] btn_rect, btn_del, plcBtn_del;
        public static Label[] lbl_rectNo, lbl_fonkNo; public Label[,] lbl, lbl_fonk;
        public static NumericUpDown[,] nud, nud_fonk;
        public static NumericUpDown[] nudThrs;
        public static ComboBox[] cmb_islem;
        public static ComboBox[] cmb_fonk;
        public static TextBox[] txb_parca;
        public static CheckBox[] chk_hata;
        public static CheckBox[] chk_fonk_value;
        public static int tab_index = 0;
        public static object[] list = new object[]
        {
            "KırmızıSay",
            "MaksGenişlik",
            "SiyahSay",
            "BeyazSay",
            "SolKonumKontrol",
            "ÜstKonumKontrol",
            "AçıHesaplaYatay",
            "AçıHesaplaDikey",
            "AudiAçıHesapla",
            "CizgiIleLokasyonBul",
            "AudiUzunlukHesapla",
            "AudiAlttanUzat",
            "DikIkiSiyahArasiMesafe",
            "RYatayIkiSiyahArasiMesafe",
            "LYatayIkiSiyahArasiMesafe",
            "LDikeyAciOlcme",
            "RDikeyAciOlcme",
            "ikiOlcumArasıFark",
            "QrOku",
            "ButonBekle"
        };
        int index = 0;
        bool visib = false, adminMode = false;
        // rectangle setting = X,Y,Width,Height,MinOfset,MaxOfset,CamId,Taramaİşlem,Threshold,ResetorStop,ParçaAdı

        public Property(int tabindex)
        {
            int[] beklee = Settings.Default.Bekleme.Split('-').Select(int.Parse).ToArray();
            InitializeComponent();
            adminMode = Settings.Default.adminMode;
            if(debugMode) { adminMode = true; }
            Panel_ayarla();
            tab_index = tabindex;
            tabControl.SelectedIndex = tabindex;
            Printing();
            txb_productName.Text = Settings.Default.productName;
            nud_Wait1.Value = beklee[0];
            nud_Wait2.Value = beklee[1];
            nud_Wait3.Value = beklee[2];
            nud_Wait4.Value = beklee[3];
            nud_Wait5.Value = beklee[4];
            nud_Wait6.Value = beklee[5];
            nud_Wait7.Value = beklee[6];
            nud_Wait8.Value = beklee[7];
            nud_Wait9.Value = beklee[8];
            nud_Lamba.Value = Settings.Default.Lamba + 1;
            nud_Kase.Value = Settings.Default.Kase + 1;
            nud_Piston.Value = Settings.Default.Piston + 1;
            nud_bosBit.Value = Settings.Default.bosBit + 1;
            if (File.Exists("degerler.txt")) Dosya_goster("degerler.txt");
            //disable işlemi
            btn_addCam.Visible = adminMode;
            btn_delCam.Visible = adminMode;

        }
        public void Panel_ayarla()
        {
            int gbCount = Settings.Default.Cam.Split('/').Length;
            int rectCount = Settings.Default.Rect.Split('/').Length;
            int fonkCount = Settings.Default.PlcFonk.Split('/').Length;
            string[] rectString = Settings.Default.Rect.Split('/');
            string[] fonkString = Settings.Default.PlcFonk.Split('/');
            int rectangleAdet = 0;
            int fonkAdet = 0;

            tab = new TabPage[gbCount];
            plcTab = new TabPage[gbCount];
            gb = new GroupBox[gbCount];
            plcGb = new GroupBox[gbCount];
            camSet = new Button[gbCount];
            camSetget = new Button[gbCount];
            camSetset = new Button[gbCount];
            btnAdd = new Button[gbCount];
            plcBtnAdd = new Button[gbCount];
            cmbCam = new ComboBox[gbCount];
            cmbRes = new ComboBox[gbCount];


            p_rect = new Panel[rectCount];
            p_fonk = new Panel[fonkCount];
            btn_rect = new Button[rectCount];
            btn_del = new Button[rectCount];
            plcBtn_del = new Button[fonkCount];
            cmb_islem = new ComboBox[rectCount];
            txb_parca = new TextBox[rectCount];
            chk_hata = new CheckBox[rectCount];
            chk_fonk_value = new CheckBox[fonkCount];
            lbl_rectNo = new Label[rectCount];
            lbl_fonkNo = new Label[fonkCount];
            lbl = new Label[rectCount, 7];
            lbl_fonk = new Label[fonkCount, 4];
            nud = new NumericUpDown[rectCount, 7];
            nud_fonk = new NumericUpDown[fonkCount, 3];
            cmb_fonk = new ComboBox[fonkCount];

            tabControl.SelectedIndexChanged += Tab_changed;


            for (int a = 0; a < gbCount; a++)
            {
                tab[a] = new TabPage();
                tabControl.Controls.Add(tab[a]);
                tab[a].BackColor = Color.White;
                tab[a].Location = new Point(4, 22);
                tab[a].Padding = new Padding(3);
                tab[a].Size = new Size(556, 124);
                tab[a].TabIndex = 0;
                tab[a].Text = "Kamera " + (a + 1).ToString();

                plcTab[a] = new TabPage();
                plc_tabControl.Controls.Add(plcTab[a]);
                plcTab[a].BackColor = Color.White;
                plcTab[a].Location = new Point(4, 22);
                plcTab[a].Padding = new Padding(3);
                plcTab[a].TabIndex = 0;
                plcTab[a].Text = "Kamera " + (a + 1).ToString();

                gb[a] = new GroupBox();
                tab[a].Controls.Add(gb[a]);
                gb[a].BackColor = Color.WhiteSmoke;
                gb[a].Location = new Point(5, 42);
                gb[a].Size = new Size(544, 79);
                gb[a].Text = "Dikdörtgenler";

                plcGb[a] = new GroupBox();
                plcTab[a].Controls.Add(plcGb[a]);
                plcGb[a].BackColor = Color.WhiteSmoke;
                plcGb[a].Location = new Point(5, 5);
                plcGb[a].Size = new Size(510, 79);
                plcGb[a].Text = "Fonksiyonlar";

                camSet[a] = new Button();
                tab[a].Controls.Add(camSet[a]);
                camSet[a].BackColor = Color.Orange;
                camSet[a].FlatStyle = FlatStyle.Popup;
                camSet[a].Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point);
                camSet[a].Location = new Point(280, 7);
                camSet[a].Size = new Size(103, 33);
                camSet[a].TabIndex = 14;
                camSet[a].Text = "Kamera Ayarları";
                camSet[a].UseVisualStyleBackColor = false;
                camSet[a].Click += Cam_settings;

                camSetget[a] = new Button();
                tab[a].Controls.Add(camSetget[a]);
                camSetget[a].BackColor = Color.Aqua;
                camSetget[a].FlatStyle = FlatStyle.Popup;
                camSetget[a].Font = new Font("Microsoft Sans Serif", 7F, FontStyle.Regular, GraphicsUnit.Point);
                camSetget[a].Location = new Point(383, 7);
                camSetget[a].Size = new Size(60, 33);
                camSetget[a].TabIndex = 14;
                camSetget[a].Text = "Ayar Al";
                camSetget[a].UseVisualStyleBackColor = false;
                camSetget[a].Click += Cam_settingsGet;
                camSetget[a].Visible = adminMode;

                camSetset[a] = new Button();
                tab[a].Controls.Add(camSetset[a]);
                camSetset[a].BackColor = Color.Green;
                camSetset[a].FlatStyle = FlatStyle.Popup;
                camSetset[a].Font = new Font("Microsoft Sans Serif", 7F, FontStyle.Regular, GraphicsUnit.Point);
                camSetset[a].Location = new Point(443, 7);
                camSetset[a].Size = new Size(60, 33);
                camSetset[a].TabIndex = 14;
                camSetset[a].Text = "Ayar Ver";
                camSetset[a].UseVisualStyleBackColor = false;
                camSetset[a].Click += Cam_settingsSet;
                camSetset[a].Visible = adminMode;

                btnAdd[a] = new Button();
                gb[a].Controls.Add(btnAdd[a]);
                btnAdd[a].BackColor = Color.Green;
                btnAdd[a].FlatStyle = FlatStyle.Popup;
                btnAdd[a].Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
                btnAdd[a].ForeColor = Color.White;
                btnAdd[a].Location = new Point(456, -2);
                btnAdd[a].Size = new Size(39, 20);
                btnAdd[a].TabIndex = 0;
                btnAdd[a].Text = "Add";
                btnAdd[a].Click += new EventHandler(this.btn_add_Click);
                btnAdd[a].Visible = adminMode;

                plcBtnAdd[a] = new Button();
                plcGb[a].Controls.Add(plcBtnAdd[a]);
                plcBtnAdd[a].BackColor = Color.Green;
                plcBtnAdd[a].FlatStyle = FlatStyle.Popup;
                plcBtnAdd[a].Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
                plcBtnAdd[a].ForeColor = Color.White;
                plcBtnAdd[a].Location = new Point(456, -2);
                plcBtnAdd[a].Size = new Size(39, 20);
                plcBtnAdd[a].Text = "Add";
                plcBtnAdd[a].Click += new EventHandler(this.btn_fonk_add_Click);

                cmbCam[a] = new ComboBox();
                tab[a].Controls.Add(cmbCam[a]);
                cmbCam[a].FormattingEnabled = true;
                cmbCam[a].Location = new Point(6, 15);
                cmbCam[a].Size = new Size(120, 21);
                cmbCam[a].TabIndex = 12;
                cmbCam[a].Name = "Cambox-" + a.ToString();

                cmbRes[a] = new ComboBox();
                tab[a].Controls.Add(cmbRes[a]);
                cmbRes[a].FormattingEnabled = true;
                cmbRes[a].Location = new Point(132, 15);
                cmbRes[a].Size = new Size(142, 21);
                cmbRes[a].TabIndex = 13;
                cmbRes[a].Name = "Resbox-" + a.ToString();

                int rectSay = 0;
                int fonkSay = 0;
                for (int j = 0; j < rectCount; j++)
                {
                    p_rect[j] = new Panel();
                    if (a == int.Parse(rectString[j].Split(',')[6]))
                    {
                        gb[a].Controls.Add(p_rect[j]);
                    }

                    p_rect[j].BackColor = Color.FromArgb(255, 27, 39, 53);
                    p_rect[j].Location = new Point(6, 20 + rectSay * 60);
                    p_rect[j].Size = new Size(532, 55);

                    if (a == int.Parse(rectString[j].Split(',')[6]))
                    {
                        rectSay++;
                    }

                    btn_rect[j] = new Button();
                    p_rect[j].Controls.Add(btn_rect[j]);
                    btn_rect[j].Text = "AL";
                    btn_rect[j].Location = new Point(14, 25);
                    btn_rect[j].Size = new Size(55, 25);
                    btn_rect[j].BackColor = Color.White;
                    btn_rect[j].Name = "Al-" + j.ToString();
                    btn_rect[j].Click += Btn_rect_Click;
                    btn_rect[j].Visible = adminMode;


                    btn_del[j] = new Button();
                    p_rect[j].Controls.Add(btn_del[j]);
                    btn_del[j].Text = "X";
                    btn_del[j].Name = "X/" + j.ToString();
                    btn_del[j].Location = new Point(512, 0);
                    btn_del[j].Size = new Size(20, 20);
                    btn_del[j].BackColor = Color.Red;
                    btn_del[j].FlatStyle = FlatStyle.Popup;
                    btn_del[j].ForeColor = Color.White;
                    btn_del[j].Margin = new Padding(0);
                    btn_del[j].TextAlign = ContentAlignment.TopCenter;
                    btn_del[j].Click += new EventHandler(btn_del_Click);
                    btn_del[j].Visible = adminMode;

                    cmb_islem[j] = new ComboBox();
                    p_rect[j].Controls.Add(cmb_islem[j]);
                    cmb_islem[j].FormattingEnabled = true;
                    cmb_islem[j].Items.AddRange(list);
                    cmb_islem[j].Location = new Point(80 + 7 * 47, 26);
                    cmb_islem[j].Name = "cmb-" + j.ToString();
                    cmb_islem[j].Size = new Size(121, 21);
                    cmb_islem[j].SelectedIndex = int.Parse(rectString[j].Split(',')[7]);
                    cmb_islem[j].SelectedIndexChanged += Value_changed;
                    cmb_islem[j].BringToFront();
                    cmb_islem[j].Enabled = adminMode;


                    txb_parca[j] = new TextBox();
                    p_rect[j].Controls.Add(txb_parca[j]);
                    txb_parca[j].Location = new Point(80 + 7 * 47, 3);
                    txb_parca[j].Name = "txb-" + j.ToString();
                    txb_parca[j].Size = new Size(100, 21);
                    txb_parca[j].Text = rectString[j].Split(',')[10];
                    txb_parca[j].TextChanged += Value_changed;
                    txb_parca[j].BringToFront();

                    lbl_rectNo[j] = new Label();
                    lbl_rectNo[j].Text = rectSay.ToString();
                    lbl_rectNo[j].BackColor = Color.FromArgb(255, 27, 39, 53);
                    lbl_rectNo[j].Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold, GraphicsUnit.Point);
                    lbl_rectNo[j].ForeColor = Color.FromArgb(255, 255, 255, 255);
                    lbl_rectNo[j].Location = new Point(5, 3);
                    lbl_rectNo[j].Size = new Size(25, 18);


                    btn_del[j].BringToFront();

                    chk_hata[j] = new CheckBox();
                    chk_hata[j].AutoSize = false;
                    chk_hata[j].FlatStyle = FlatStyle.Flat;
                    chk_hata[j].Location = new Point(30, 3);
                    chk_hata[j].Name = "chk-" + j.ToString();
                    chk_hata[j].Size = new Size(55, 18);
                    chk_hata[j].Text = "RST";
                    chk_hata[j].ForeColor = MainPage.wht;
                    chk_hata[j].TextAlign = ContentAlignment.MiddleLeft;
                    chk_hata[j].UseVisualStyleBackColor = true;
                    chk_hata[j].Checked = bool.Parse(rectString[j].Split(',')[9]);
                    chk_hata[j].CheckedChanged += Value_changed;
                    p_rect[j].Controls.Add(chk_hata[j]);
                    chk_hata[j].BringToFront();

                }

                for (int p = 0; p < fonkCount; p++)
                {
                    p_fonk[p] = new Panel();
                    if (a == int.Parse(fonkString[p].Split(',')[3]))
                    {
                        plcGb[a].Controls.Add(p_fonk[p]);
                    }

                    p_fonk[p].BackColor = Color.FromArgb(255, 27, 39, 53);
                    p_fonk[p].Location = new Point(6, 20 + fonkSay * 60);
                    p_fonk[p].Size = new Size(497, 55);

                    if (a == int.Parse(fonkString[p].Split(',')[3]))
                    {
                        fonkSay++;
                    }

                    plcBtn_del[p] = new Button();
                    p_fonk[p].Controls.Add(plcBtn_del[p]);
                    plcBtn_del[p].Text = "X";
                    plcBtn_del[p].Name = "X/" + p.ToString();
                    plcBtn_del[p].Location = new Point(477, 0);
                    plcBtn_del[p].Size = new Size(20, 20);
                    plcBtn_del[p].BackColor = Color.Red;
                    plcBtn_del[p].FlatStyle = FlatStyle.Popup;
                    plcBtn_del[p].ForeColor = Color.White;
                    plcBtn_del[p].Margin = new Padding(0);
                    plcBtn_del[p].TextAlign = ContentAlignment.TopCenter;
                    plcBtn_del[p].Click += new EventHandler(btn_fonk_del_Click);
                }

                if (rectSay > rectangleAdet) rectangleAdet = rectSay;
                gb[a].Height = (rectangleAdet * 60) + 30;
                tabControl.Height = gb[a].Height + 105;
                tab[a].Height = gb[a].Height + 105;
                int dikTopYuk = gb[a].Height + 115;

                if (fonkSay > fonkAdet) fonkAdet = fonkSay;
                plcGb[a].Height = (fonkAdet * 60) + 30;
                plc_tabControl.Height = plcGb[a].Height + 105;
                plcTab[a].Height = plcGb[a].Height + 105;
                int fonkTopYuk = plcGb[a].Height + 115;

                if (dikTopYuk > fonkTopYuk) Height = dikTopYuk;
                else Height = fonkTopYuk;

                for (int e = 0; e < rectCount; e++)
                {
                    if (a == int.Parse(rectString[e].Split(',')[6]))
                    {
                        gb[a].Controls.Add(p_rect[e]);
                    }
                    p_rect[e].Controls.Add(lbl_rectNo[e]);

                    for (int i = 0; i < 7; i++)
                    {
                        lbl[e, i] = new Label();
                        p_rect[e].Controls.Add(lbl[e, i]);
                        lbl[e, i].Size = new Size(45, 15);
                        lbl[e, i].Top = 6;
                        lbl[e, i].ForeColor = Color.White;
                        lbl[e, i].Text = i.ToString();
                        lbl[e, i].Left = 80 + i * 47;
                        lbl[e, i].BringToFront();

                        nud[e, i] = new NumericUpDown();
                        p_rect[e].Controls.Add(nud[e, i]);
                        nud[e, i].Size = new Size(45, 20);
                        nud[e, i].Top = 26;
                        nud[e, i].Left = 80 + i * 47;
                        nud[e, i].Maximum = new decimal(new int[] { 500000, 0, 0, 0 });
                        if (i == 6) nud[e, i].Value = int.Parse(rectString[e].Split(',')[8]);
                        else nud[e, i].Value = int.Parse(rectString[e].Split(',')[i]);
                        nud[e, i].ValueChanged += Value_changed;
                        nud[e, i].Name = e.ToString() + "/" + i.ToString();

                        if (i < 4)
                        {
                            nud[e, i].Enabled = adminMode;
                        }
                    }
                    lbl[e, 0].Text = "X"; lbl[e, 1].Text = "Y"; lbl[e, 2].Text = "Width"; lbl[e, 3].Text = "Height"; lbl[e, 4].Text = "Min-Ofset"; lbl[e, 5].Text = "Max-Ofset"; lbl[e, 6].Text = "Thrs";

                }

                for (int e = 0; e < fonkCount; e++)
                {
                    if (a == int.Parse(fonkString[e].Split(',')[3]))
                    {
                        plcGb[a].Controls.Add(p_fonk[e]);
                    }
                    p_fonk[e].Controls.Add(lbl_fonkNo[e]);

                    for (int i = 0; i < 4; i++)
                    {
                        lbl_fonk[e, i] = new Label();
                        p_fonk[e].Controls.Add(lbl_fonk[e, i]);
                        lbl_fonk[e, i].Size = new Size(45, 15);
                        lbl_fonk[e, i].Top = 6;
                        lbl_fonk[e, i].ForeColor = Color.White;
                        lbl_fonk[e, i].Text = i.ToString();
                        lbl_fonk[e, i].Left = 80 + i * 47;
                        lbl_fonk[e, i].BringToFront();


                        if (i < 3)
                        {
                            nud_fonk[e, i] = new NumericUpDown();
                            p_fonk[e].Controls.Add(nud_fonk[e, i]);
                            nud_fonk[e, i].Size = new Size(45, 20);
                            nud_fonk[e, i].Top = 26;
                            nud_fonk[e, i].Left = 80 + i * 47;
                            nud_fonk[e, i].Value = int.Parse(fonkString[e].Split(',')[i]);
                            nud_fonk[e, i].ValueChanged += Function_Value_Changed;
                            nud_fonk[e, i].Name = e.ToString() + "*" + i.ToString();
                        }
                    }


                    cmb_fonk[e] = new ComboBox();
                    p_fonk[e].Controls.Add(cmb_fonk[e]);
                    cmb_fonk[e].FormattingEnabled = true;
                    for (int i = 0; i < rectCount; i++)
                    {
                        cmb_fonk[e].Items.Add(i + 1);
                    }
                    cmb_fonk[e].Name = "cmbfonk-" + e.ToString();
                    cmb_fonk[e].Size = new Size(75, 20);
                    cmb_fonk[e].Top = 26;
                    cmb_fonk[e].Left = 80 + 3 * 47;
                    cmb_fonk[e].SelectedIndexChanged += Function_Value_Changed;
                    cmb_fonk[e].SelectedIndex = cmb_fonk[e].Items.IndexOf(int.Parse(fonkString[e].Split(',')[4]));
                    cmb_fonk[e].BringToFront();




                    lbl_fonk[e, 0].Text = "DB No"; lbl_fonk[e, 1].Text = "Başlgç"; lbl_fonk[e, 2].Text = "Boyut"; lbl_fonk[e, 3].Text = "İşlem";

                }
            }
            Cam_cmboxAdjust();
        }
        private void Printing()
        {
            cmb_Printers.Items.Clear();
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                cmb_Printers.Items.Add(printer);
            }
            try
            {
                cmb_Printers.SelectedItem = Settings.Default.Printer;
            }
            catch
            {

                cmb_Printers.SelectedIndex = 0;
            }
        }
        private void Tab_changed(object sender, EventArgs e)
        {
            Program.mainPage.visibility_set(tabControl.SelectedIndex);
            MainPage.caming_id = tabControl.SelectedIndex;
        }
        private void Cam_settings(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            for (int i = 0; i < cam.Length; i++)
            {
                if (btn == camSet[i])
                    MainPage.cam[i].DisplayPropertyPage(IntPtr.Zero);
            }
        }
        private void Cam_settingsGet(object sender, EventArgs e)
        {
            string setting = "";
            for (int i = 0; i < cam.Length; i++)
            {
                setting += GetCameraSettings(MainPage.cam[i]) + "/";
            }
            setting = setting.TrimEnd('/');
            Settings.Default.CamSetting = setting;
            Settings.Default.Save();
        }
        private void Cam_settingsSet(object sender, EventArgs e)
        {
            for (int i = 0; i < cam.Length; i++)
            {
                SetCameraSettings(MainPage.cam[i], i);
            }
        }
        static string GetCameraSettings(VideoCaptureDevice videoSource)
        {
            string settings = "";
            int Iris, Focus, pan, tilt, roll, zoom, exposure;
            videoSource.GetCameraProperty(CameraControlProperty.Iris, out Iris, out CameraControlFlags flags);
            videoSource.GetCameraProperty(CameraControlProperty.Focus, out Focus, out CameraControlFlags flags1);
            videoSource.GetCameraProperty(CameraControlProperty.Pan, out pan, out CameraControlFlags flags2);
            videoSource.GetCameraProperty(CameraControlProperty.Tilt, out tilt, out CameraControlFlags flags3);
            videoSource.GetCameraProperty(CameraControlProperty.Roll, out roll, out CameraControlFlags flags4);
            videoSource.GetCameraProperty(CameraControlProperty.Zoom, out zoom, out CameraControlFlags flags5);
            videoSource.GetCameraProperty(CameraControlProperty.Exposure, out exposure, out CameraControlFlags flags6);
            settings += $"{Iris}*";
            settings += $"{Focus}*";
            settings += $"{pan}*";
            settings += $"{tilt}*";
            settings += $"{roll}*";
            settings += $"{zoom}*";
            settings += $"{exposure}*";
            settings = settings.TrimEnd('*');
            return settings;
        }
        static void SetCameraSettings(VideoCaptureDevice videoSource, int i)
        {
            string[] camsettingsstring = Settings.Default.CamSetting.Split('/');
            int iris = int.Parse(camsettingsstring[i].Split('*')[0]);
            int focus = int.Parse(camsettingsstring[i].Split('*')[1]);
            int pan = int.Parse(camsettingsstring[i].Split('*')[2]);
            int tilt = int.Parse(camsettingsstring[i].Split('*')[3]);
            int roll = int.Parse(camsettingsstring[i].Split('*')[4]);
            int zoom = int.Parse(camsettingsstring[i].Split('*')[5]);
            int exposure = int.Parse(camsettingsstring[i].Split('*')[6]);
            CameraControlFlags flags = CameraControlFlags.Manual;
            videoSource.SetCameraProperty(CameraControlProperty.Iris, iris, flags);
            videoSource.SetCameraProperty(CameraControlProperty.Focus, focus, flags);
            videoSource.SetCameraProperty(CameraControlProperty.Pan, pan, flags);
            videoSource.SetCameraProperty(CameraControlProperty.Tilt, tilt, flags);
            videoSource.SetCameraProperty(CameraControlProperty.Roll, roll, flags);
            videoSource.SetCameraProperty(CameraControlProperty.Zoom, zoom, flags);
            videoSource.SetCameraProperty(CameraControlProperty.Exposure, exposure, flags);
        }
        private void Cam_cmboxAdjust()
        {
            for (int i = 0; i < tab.Length; i++)
            {
                cmbCam[i].Items.Clear();
            }

            devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (devices.Count > 0)
            {
                cam = new VideoCaptureDevice[tab.Length];
                int[] Cam_no = new int[tab.Length];
                int[] Res_no = new int[tab.Length];
                string[] camString = Settings.Default.Cam.Split('/');
                for (int i = 0; i < tab.Length; i++)
                {
                    int no = 1;
                    foreach (FilterInfo item in devices)
                    {
                        cmbCam[i].Items.Add(no.ToString() + " - " + item.Name);
                        no++;
                    }
                    cmbCam[i].Items.Add("Kamera Yok");
                    if (cmbCam[i].SelectedIndex < 0)
                    {
                        cmbCam[i].SelectedIndex = 0;
                    }
                    else if (cmbCam[i].SelectedIndex >= devices.Count)
                    {
                        cmbCam[i].SelectedIndex = 0;
                    }
                    cam[i] = new VideoCaptureDevice(devices[cmbCam[i].SelectedIndex].MonikerString);

                    cmbRes[i].Items.Clear();


                    foreach (var capability in cam[i].VideoCapabilities)
                    {
                        cmbRes[i].Items.Add($"{capability.FrameSize.Width}x{capability.FrameSize.Height} - {capability.AverageFrameRate} FPS");
                    }
                    Res_no[i] = int.Parse(camString[i].Split(',')[1]);
                    if (Res_no[i] < 0)
                    {
                        Res_no[i] = 0;
                    }
                    if (Res_no[i] >= cam[i].VideoCapabilities.Length)
                    {
                        Res_no[i] = 0;
                    }

                    cmbRes[i].SelectedIndex = Res_no[i];

                }
                for (int i = 0; i < tab.Length; i++)
                {
                    Cam_no[i] = int.Parse(camString[i].Split(',')[0]);
                }

                for (int i = 0; i < tab.Length; i++)
                {
                    if (devices.Count > Cam_no[i])
                    {
                        cmbCam[i].SelectedIndex = Cam_no[i];
                    }
                    else Cam_no[i] = 0;
                }
            }
        }
        private void Btn_rect_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            index = int.Parse(clickedButton.Name.Split('-')[1]);

            Program.mainPage.pb_scene.MouseMove += new MouseEventHandler(Program.mainPage.pb_scene_MouseMove);
        }
        public void UpdateLabel(Point coordinate)
        {
            int corx = coordinate.X;
            int cory = coordinate.Y;
            int middlex = (int)nud[index, 2].Value / 2;
            int middley = (int)nud[index, 3].Value / 2;
            int sonucX = corx - middlex;
            int sonucY = cory - middley;
            if (sonucX < 0) { sonucX = 0; }
            if (sonucY < 0) { sonucY = 0; }
            nud[index, 0].Value = (decimal)sonucX;
            nud[index, 1].Value = (decimal)sonucY;
        }
        private void Value_changed(object sender, EventArgs e)
        {

            if (sender is ComboBox cb)
            {
                if (cb.Name.Split('-')[0] == "Cambox")
                {
                    int[] Res_no = new int[tab.Length];
                    int i = int.Parse(cb.Name.Split('-')[1]);
                    string[] camString = Settings.Default.Cam.Split('/');

                    // Kontrol sayısını `devices.Count` ile sınırla
                    cmbRes[i].Items.Clear();

                    if (cmbCam[i].SelectedIndex < 0)
                    {
                        cmbCam[i].SelectedIndex = 0;
                    }
                    else if (cmbCam[i].SelectedIndex >= devices.Count)
                    {
                        cmbCam[i].SelectedIndex = devices.Count - 1;
                    }
                    cam[i] = new VideoCaptureDevice(devices[cmbCam[i].SelectedIndex].MonikerString);
                    foreach (var capability in cam[i].VideoCapabilities)
                    {
                        cmbRes[i].Items.Add($"{capability.FrameSize.Width}x{capability.FrameSize.Height} - {capability.AverageFrameRate} FPS");
                    }

                    Res_no[i] = int.Parse(camString[i].Split(',')[1]);
                    if (Res_no[i] < 0)
                    {
                        Res_no[i] = 0;
                    }
                    if (Res_no[i] >= cam[i].VideoCapabilities.Length)
                    {
                        Res_no[i] = cam[i].VideoCapabilities.Length - 1;
                    }

                    cmbRes[i].SelectedIndex = Res_no[i];

                }
                else if (cb.Name.Split('-')[0] == "Resbox")
                {
                    string newCamString = "";
                    for (int i = 0; i < tab.Length; i++)
                    {
                        newCamString = newCamString + cmbCam[i].SelectedIndex + "," + cmbRes[i].SelectedIndex + "/";
                    }
                    if (newCamString.StartsWith("/"))
                    {
                        newCamString = newCamString.Substring(1);
                    }

                    if (newCamString.EndsWith("/"))
                    {
                        newCamString = newCamString.Substring(0, newCamString.Length - 1);
                    }
                    Settings.Default.Cam = newCamString;
                    Settings.Default.Save();
                    Program.mainPage.Cam_adjust();
                    Program.mainPage.Label_adjust(MainPage.org);
                }
                else if (cb.Name.Split('-')[0] == "cmb")
                {
                    int a = cmb_islem[int.Parse(cb.Name.Split('-')[1])].SelectedIndex;
                    string newValue = "";
                    string[] SavedRect = Settings.Default.Rect.Split('/');
                    // rectangle setting = X,Y,Width,Height,MinOfset,MaxOfset,CamId,Taramaİşlem,Threshold,ResetorStop,ParçaAdı
                    for (int i = 0; i < p_rect.Length; i++)
                    {
                        string[] rectNud = SavedRect[i].Split(',');
                        for (int j = 0; j < 11; j++)
                        {
                            if (j == 7 && i == int.Parse(cb.Name.Split('-')[1])) rectNud[j] = cb.SelectedIndex.ToString();
                            newValue = newValue + rectNud[j] + ",";
                        }
                        if (newValue.StartsWith(",")) newValue = newValue.Substring(1);
                        if (newValue.EndsWith(",")) newValue = newValue.Substring(0, newValue.Length - 1);
                        newValue += "/";
                    }


                    if (newValue.StartsWith("/"))
                    {
                        newValue = newValue.Substring(1);
                    }

                    if (newValue.EndsWith("/"))
                    {
                        newValue = newValue.Substring(0, newValue.Length - 1);
                    }

                    Settings.Default.Rect = newValue;
                    Settings.Default.Save();
                }

            }
            else if (sender is NumericUpDown nu)
            {
                if (nu != null)
                {
                    int a = int.Parse(nu.Name.Split('/')[0]);
                    int b = int.Parse(nu.Name.Split('/')[1]);
                    nud[a, b].Value = (decimal)nu.Value;
                }
                string newValue = "";
                // rectangle setting = X,Y,Width,Height,MinOfset,MaxOfset,CamId,Taramaİşlem,Threshold,ResetorStop,ParçaAdı
                for (int i = 0; i < p_rect.Length; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        newValue = newValue + nud[i, j].Value.ToString() + ",";
                    }
                    newValue = newValue + Settings.Default.Rect.Split('/')[i].Split(',')[6] + "," + cmb_islem[i].SelectedIndex + "," + nud[i, 6].Value.ToString() + "," + chk_hata[i].Checked + "," + txb_parca[i].Text + "/";
                }


                if (newValue.StartsWith("/"))
                {
                    newValue = newValue.Substring(1);
                }

                if (newValue.EndsWith("/"))
                {
                    newValue = newValue.Substring(0, newValue.Length - 1);
                }

                Settings.Default.Rect = newValue;
                Settings.Default.Save();
                Program.mainPage.Label_adjust(MainPage.org);
                Program.mainPage.visibility_set(tabControl.SelectedIndex);

            }
            else if (sender is CheckBox chk)
            {
                if (chk != null)
                {
                    int b = int.Parse(chk.Name.Split('-')[1]);
                    chk_hata[b].Checked = chk.Checked;
                }
                string newValue = "";
                // rectangle setting = X,Y,Width,Height,MinOfset,MaxOfset,CamId,Taramaİşlem,Threshold,ResetorStop,ParçaAdı
                for (int i = 0; i < p_rect.Length; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        newValue = newValue + nud[i, j].Value.ToString() + ",";
                    }
                    newValue = newValue + Settings.Default.Rect.Split('/')[i].Split(',')[6] + "," + cmb_islem[i].SelectedIndex + "," + nud[i, 6].Value.ToString() + "," + chk_hata[i].Checked + "," + txb_parca[i].Text + "/";
                }


                if (newValue.StartsWith("/"))
                {
                    newValue = newValue.Substring(1);
                }

                if (newValue.EndsWith("/"))
                {
                    newValue = newValue.Substring(0, newValue.Length - 1);
                }

                Settings.Default.Rect = newValue;
                Settings.Default.Save();
                Program.mainPage.Label_adjust(MainPage.org);
                Program.mainPage.visibility_set(tabControl.SelectedIndex);

            }
            else if (sender is TextBox txb)
            {
                if (txb != null)
                {
                    int b = int.Parse(txb.Name.Split('-')[1]);
                    txb_parca[b].Text = txb.Text;
                }
                string newValue = "";
                // rectangle setting = X,Y,Width,Height,MinOfset,MaxOfset,CamId,Taramaİşlem,Threshold,ResetorStop,ParçaAdı
                for (int i = 0; i < p_rect.Length; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        newValue = newValue + nud[i, j].Value.ToString() + ",";
                    }
                    newValue = newValue + Settings.Default.Rect.Split('/')[i].Split(',')[6] + "," + cmb_islem[i].SelectedIndex + "," + nud[i, 6].Value.ToString() + "," + chk_hata[i].Checked + "," + txb_parca[i].Text + "/";
                }


                if (newValue.StartsWith("/"))
                {
                    newValue = newValue.Substring(1);
                }

                if (newValue.EndsWith("/"))
                {
                    newValue = newValue.Substring(0, newValue.Length - 1);
                }

                Settings.Default.Rect = newValue;
                Settings.Default.Save();
                Program.mainPage.Label_adjust(MainPage.org);
                Program.mainPage.visibility_set(tabControl.SelectedIndex);

            }
        }

        private void Function_Value_Changed(object sender, EventArgs e)
        {
            if (sender is ComboBox cb)
            {
                if (cb.Name.Split('-')[0] == "cmbfonk")
                {
                    string newValue = "";
                    string[] SavedFonk = Settings.Default.PlcFonk.Split('/');

                    for (int i = 0; i < p_fonk.Length; i++)
                    {
                        string[] fonkNud = SavedFonk[i].Split(',');
                        for (int j = 0; j < 5; j++)
                        {
                            if (j == 4 && i == int.Parse(cb.Name.Split('-')[1])) fonkNud[j] = (cb.SelectedIndex + 1).ToString();
                            newValue = newValue + fonkNud[j] + ",";
                        }
                        if (newValue.StartsWith(",")) newValue = newValue.Substring(1);
                        if (newValue.EndsWith(",")) newValue = newValue.Substring(0, newValue.Length - 1);
                        newValue += "/";
                    }

                    if (newValue.StartsWith("/"))
                    {
                        newValue = newValue.Substring(1);
                    }

                    if (newValue.EndsWith("/"))
                    {
                        newValue = newValue.Substring(0, newValue.Length - 1);
                    }

                    Settings.Default.PlcFonk = newValue;
                    Settings.Default.Save();
                }

            }
            else if (sender is NumericUpDown nu)
            {
                if (nu != null)
                {
                    int a = int.Parse(nu.Name.Split('*')[0]);
                    int b = int.Parse(nu.Name.Split('*')[1]);
                    nud_fonk[a, b].Value = nu.Value;
                }
                string newValue = "";
                for (int i = 0; i < p_fonk.Length; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        newValue = newValue + nud_fonk[i, j].Value.ToString() + ",";
                    }
                    newValue = newValue + Settings.Default.PlcFonk.Split('/')[i].Split(',')[3] + "," + (cmb_fonk[i].SelectedIndex + 1).ToString();
                }


                if (newValue.StartsWith("/"))
                {
                    newValue = newValue.Substring(1);
                }

                if (newValue.EndsWith("/"))
                {
                    newValue = newValue.Substring(0, newValue.Length - 1);
                }

                Settings.Default.PlcFonk = newValue;
                Settings.Default.Save();
            }
        }

        private void btn_del_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            int index = int.Parse(clickedButton.Name.Split('/')[1]);
            string[] rectString = Settings.Default.Rect.Split('/');
            int lenght = 0;
            foreach (string rr in rectString)
            {
                if (int.Parse(rr.Split(',')[6]) == tabControl.SelectedIndex) lenght++;
            }
            if (lenght <= 1) { DialogResult results = MessageBox.Show("Son Tarama Alanını Silemezsiniz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Question); }
            else
            {
                string rectJust = Settings.Default.Rect;
                int rectCount = rectJust.Split('/').Length;
                tab_index = int.Parse(rectString[index].Split(',')[6]);
                string newRect = "";

                DialogResult result = MessageBox.Show("Silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    for (int i = 0; i < rectCount; i++)
                    {
                        if (i != index)
                        {
                            newRect += "/" + rectString[i];
                        }
                    }
                    if (newRect.StartsWith("/"))
                    {
                        newRect = newRect.Substring(1);
                    }

                    if (newRect.EndsWith("/"))
                    {
                        newRect = newRect.Substring(0, newRect.Length - 1);
                    }
                    Settings.Default.Rect = newRect;
                    Settings.Default.Save();

                    Point formLocation = this.Location;
                    this.Close();
                    Property newPropertyForm = new Property(tab_index) { Location = formLocation };
                    newPropertyForm.Show();
                }
            }
        }

        private void btn_fonk_del_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            int index = int.Parse(clickedButton.Name.Split('/')[1]);
            string[] rectString = Settings.Default.PlcFonk.Split('/');
            int lenght = 0;
            foreach (string rr in rectString)
            {
                if (int.Parse(rr.Split(',')[3]) == plc_tabControl.SelectedIndex) lenght++;
            }
            if (lenght <= 1) { DialogResult results = MessageBox.Show("Son Fonksiyonu Silemezsiniz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Question); }
            else
            {
                string rectJust = Settings.Default.PlcFonk;
                int rectCount = rectJust.Split('/').Length;
                tab_index = int.Parse(rectString[index].Split(',')[3]);
                string newRect = "";

                DialogResult result = MessageBox.Show("Silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    for (int i = 0; i < rectCount; i++)
                    {
                        if (i != index)
                        {
                            newRect += "/" + rectString[i];
                        }
                    }
                    if (newRect.StartsWith("/"))
                    {
                        newRect = newRect.Substring(1);
                    }

                    if (newRect.EndsWith("/"))
                    {
                        newRect = newRect.Substring(0, newRect.Length - 1);
                    }
                    Settings.Default.PlcFonk = newRect;
                    Settings.Default.Save();

                    Point formLocation = this.Location;
                    this.Close();
                    Property newPropertyForm = new Property(tab_index) { Location = formLocation };
                    newPropertyForm.Show();
                }
            }
        }
        private void btn_add_Click(object sender, EventArgs e)
        {
            // rectangle setting = X,Y,Width,Height,MinOfset,MaxOfset,CamId,Taramaİşlem,Threshold,ResetorStop,ParçaAdı
            Button clickedButton = (Button)sender;
            int index = Array.IndexOf(btnAdd, clickedButton);

            DialogResult result = MessageBox.Show("Eklemek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string rectJust = Settings.Default.Rect;
                rectJust = rectJust + "/0,0,1,1,0,100," + index.ToString() + ",0,128,true,Parça Adı";
                if (rectJust.StartsWith("/"))
                {
                    rectJust = rectJust.Substring(1);
                }

                if (rectJust.EndsWith("/"))
                {
                    rectJust = rectJust.Substring(0, rectJust.Length - 1);
                }
                Settings.Default.Rect = rectJust;
                Settings.Default.Save();
                Point formLocation = this.Location;
                this.Close();
                Property newPropertyForm = new Property(index) { Location = formLocation };
                newPropertyForm.Show();
            }
        }

        private void btn_fonk_add_Click(object sender, EventArgs e)
        {
            // rectangle setting = dbno,başlangıç,boyut,cam id

            Button clickedButton = (Button)sender;
            int index = Array.IndexOf(plcBtnAdd, clickedButton);

            string rectJust = Settings.Default.PlcFonk;
            rectJust = rectJust + "/0,0,0," + index.ToString() + ",0";
            if (rectJust.StartsWith("/"))
            {
                rectJust = rectJust.Substring(1);
            }

            if (rectJust.EndsWith("/"))
            {
                rectJust = rectJust.Substring(0, rectJust.Length - 1);
            }
            Settings.Default.PlcFonk = rectJust;
            Settings.Default.Save();
            Point formLocation = this.Location;
            this.Close();
            Property newPropertyForm = new Property(index) { Location = formLocation };
            newPropertyForm.Show();
        }
        private void btn_addCam_Click(object sender, EventArgs e)
        {
            // rectangle setting = X,Y,Width,Height,MinOfset,MaxOfset,CamId,Taramaİşlem,Threshold,ResetorStop,ParçaAdı
            int index = btnAdd.Length;

            DialogResult result = MessageBox.Show("Eklemek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string rectJust = Settings.Default.Rect;
                rectJust = rectJust + "/0,0,1,1,0,100," + index.ToString() + ",0,128,true,Parça Adı";
                if (rectJust.StartsWith("/"))
                {
                    rectJust = rectJust.Substring(1);
                }

                if (rectJust.EndsWith("/"))
                {
                    rectJust = rectJust.Substring(0, rectJust.Length - 1);
                }
                string camadd = Settings.Default.Cam;
                Settings.Default.Cam = camadd + "/0,0";
                Settings.Default.Rect = rectJust;
                Settings.Default.Save();
                Point formLocation = this.Location;
                this.Close();
                Property newPropertyForm = new Property(index) { Location = formLocation };
                newPropertyForm.Show();
            }
        }
        private void btn_delCam_Click(object sender, EventArgs e)
        {
            int camIndex = tabControl.SelectedIndex;
            if (gb.Length <= 1) { DialogResult results = MessageBox.Show("Son Kamerayı Silemezsiniz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Question); }
            else
            {
                // Kamera silindiğinde, dikdörtgenlerin indeksini güncelle
                DialogResult result = MessageBox.Show("Seçili kamerayı silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string[] rectArray = Settings.Default.Rect.Split('/');
                    for (int i = 0; i < rectArray.Length; i++)
                    {
                        string[] rectData = rectArray[i].Split(',');

                        // İlgili kameranın dikdörtgen verisini sil
                        if (int.Parse(rectData[6]) == camIndex)
                        {
                            rectArray[i] = null;
                        }
                    }
                    // Boş değerleri filtrele
                    rectArray = rectArray.Where(rect => !string.IsNullOrEmpty(rect)).ToArray();

                    // Kameraların string değerlerini güncelle
                    string[] camArray = Settings.Default.Cam.Split(',');
                    camArray[camIndex] = null; // İlgili kamerayı sil
                                               // Boş değerleri filtrele
                    camArray = camArray.Where(cam => !string.IsNullOrEmpty(cam)).ToArray();

                    // İndeksleri eksilt
                    for (int i = 0; i < rectArray.Length; i++)
                    {
                        string[] rectData = rectArray[i].Split(',');
                        int camId = int.Parse(rectData[6]);

                        if (camId > camIndex)
                        {
                            rectData[6] = (camId - 1).ToString();
                            rectArray[i] = string.Join(",", rectData);
                        }
                    }

                    // Yeni rect ve cam değerlerini birleştir
                    string updatedRect = string.Join("/", rectArray);
                    string updatedCam = string.Join(",", camArray);

                    // Ayarları güncelle
                    Settings.Default.Rect = updatedRect;
                    Settings.Default.Cam = updatedCam;
                    Settings.Default.Save();
                    Point formLocation = this.Location;
                    this.Close();
                    Property newPropertyForm = new Property(index) { Location = formLocation };
                    newPropertyForm.Show();
                }
            }
        }

        public void Reporting(string text)
        {
            txb_report.Items.Clear();
            txb_report.Groups.Clear();

            if (text.StartsWith("/"))
            {
                text = text.Substring(1);
            }

            if (text.EndsWith("/"))
            {
                text = text.Substring(0, text.Length - 1);
            }

            string[] satir = text.Trim().Split('/');
            Dictionary<string, ListViewGroup> groupDict = new Dictionary<string, ListViewGroup>();

            for (int i = 0; i < satir.Length; i++)
            {
                string[] row = satir[i].Split(',');

                string groupKey = row[0].Trim().Split('-')[1];

                ListViewGroup group;
                if (groupDict.ContainsKey(groupKey))
                {
                    group = groupDict[groupKey];
                }
                else
                {
                    group = new ListViewGroup("İşlem " + groupKey, HorizontalAlignment.Left);
                    txb_report.Groups.Add(group);
                    groupDict.Add(groupKey, group);
                }

                var lvi = new ListViewItem(row);
                lvi.Group = group;
                txb_report.Items.Add(lvi);
            }
            if (txb_report.Items.Count > 0)
            {
                txb_report.EnsureVisible(txb_report.Items.Count - 1);
            }

        }

        //Pencere Taşıma Start
        bool Moved; int Mouse_X, Mouse_Y;
        private void Move_MouseDown(object sender, MouseEventArgs e) { Mouse_X = e.X; Mouse_Y = e.Y; Moved = true; }
        private void Move_MouseMove(object sender, MouseEventArgs e) { if (Moved) SetDesktopLocation(MousePosition.X - Mouse_X, MousePosition.Y - Mouse_Y); }
        private void Move_MouseUp(object sender, MouseEventArgs e) { Moved = false; }
        private void Property_Load(object sender, EventArgs e)
        {
            for (int a = 0; a < gb.Length; a++)
            {
                cmbCam[a].SelectedIndexChanged += new EventHandler(Value_changed);
                cmbRes[a].SelectedIndexChanged += new EventHandler(Value_changed);
            }
            Program.mainPage.visibility_set(tabControl.SelectedIndex);
            MainPage.caming_id = tabControl.SelectedIndex;
            SizeChange.Height = Height - 80;
            TabControlSag.Height = Height - 60;//+83
            TabControlSonuc.Height = Height - 86;
            lst_txtData.Height = Height - 143;
            txb_report.Height = SizeChange.Height;
            Width = 582;
            lbl_banner.Width = 582;
            btn_close.Left = Width - 63;

            if (MainPage.reportold != null)
            {
                Reporting(MainPage.reportold);
            }

            chb_plc_enable.Checked = Settings.Default.PlcEnable;
            txt_plc_ip.Text = Settings.Default.PIp;
            nud_plc_rack.Value = Settings.Default.PRack;
            nud_plc_slot.Value = Settings.Default.PSlot;

            string[] ports = SerialPort.GetPortNames();
            int portscount = ports.Length;
            chb_uart_enable.Checked = Settings.Default.UartEnable;
            if (portscount < 0) txt_uart_com.Text = ports[portscount - 1]; else txt_uart_com.Text = "PortYok";
            txt_uart_baud.Text = Settings.Default.baud.ToString();


            nud_plc_lamba.Value = Settings.Default.plc_lamba;
            nud_plc_piston.Value = Settings.Default.plc_piston;
            nud_plc_kase.Value = Settings.Default.plc_kase;
            nud_plc_ok_bit.Value = Settings.Default.plc_bos;

            nud_Lamba.Value = Settings.Default.Lamba;
            nud_Piston.Value = Settings.Default.Piston;
            nud_Kase.Value = Settings.Default.Kase;
            nud_bosBit.Value = Settings.Default.bosBit;

        }
        private void SizeChange_Click(object sender, EventArgs e)
        {
            if (Width < 800)
            {
                this.Width = 1154;
                lbl_banner.Width = 1154;
                btn_close.Left = Width - 63;
            }
            else
            {
                this.Width = 582;
                lbl_banner.Width = 582;
                btn_close.Left = Width - 63;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            visib = !visib;
            for (int i = 0; i < MainPage.pbox.Length; i++)
            {
                MainPage.pbox[i].Visible = visib;
            }
        }
        private void cmb_Printers_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.Printer = (string)cmb_Printers.SelectedItem;
            Settings.Default.Save();
        }
        private void TabControlSag_SelectedIndexChanged(object sender, EventArgs e)
        {
            txb_report.Height = TabControlSonuc.Height - 40;
            txb_debug.Height = TabControlSag.Height - 40;
            lst_txtData.Height = Height - 143;

            if (TabControlSag.SelectedIndex == 0) //Tarama Sonuçları
            {
                if (txb_report.Items.Count > 0)
                {
                    txb_report.EnsureVisible(txb_report.Items.Count - 1);
                }
            }
            else if (TabControlSag.SelectedIndex == 1) // hatalar
            {
                try
                {
                    if (File.Exists("debug.txt"))
                    {
                        string[] debugLines = File.ReadAllLines("debug.txt");
                        txb_debug.Lines = debugLines;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
                txb_debug.SelectionStart = txb_debug.TextLength;
                txb_debug.ScrollToCaret();
            }
            else // genel ayarlar
            {

            }
        }
        private void txb_productName_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.productName = txb_productName.Text;
            Settings.Default.Save();
            Program.mainPage.Label_adjust(MainPage.org);
        }

        Bitmap orjbitmap;

        private void btn_thresApp_Click(object sender, EventArgs e)
        {
            if (orjbitmap != null)
            {
                Program.mainPage.Threshold_Apply(orjbitmap, (int)nd_threshold.Value);
            }
        }


        private void txt_QrFind_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.QrFind = txt_QrFind.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Settings.Default.PlcEnable = chb_plc_enable.Checked;
            Settings.Default.PIp = txt_plc_ip.Text;
            Settings.Default.PRack = (int)nud_plc_rack.Value;
            Settings.Default.PSlot = (int)nud_plc_slot.Value;

            Settings.Default.UartEnable = chb_uart_enable.Checked;
            Settings.Default.baud = int.Parse(txt_uart_baud.Text);

            Settings.Default.plc_lamba = (int)nud_plc_lamba.Value;
            Settings.Default.plc_piston = (int)nud_plc_piston.Value;
            Settings.Default.plc_kase = (int)nud_plc_kase.Value;
            Settings.Default.plc_bos = (int)nud_plc_ok_bit.Value;

            Settings.Default.Lamba = (int)nud_Lamba.Value;
            Settings.Default.Piston = (int)nud_Piston.Value;
            Settings.Default.Kase = (int)nud_Kase.Value;
            Settings.Default.bosBit = (int)nud_bosBit.Value;

            Settings.Default.Save();
        }

        bool push = false;
        private void btn_foto_Click(object sender, EventArgs e)
        {
            push = !push;
            if (push)
            {
                btn_foto.Text = "Durdur";
                Program.mainPage.timer.Stop();
                orjbitmap = new Bitmap(Program.mainPage.pb_scene.Image);
            }
            else
            {
                btn_foto.Text = "Başlat";
                Program.mainPage.timer.Start();
            }
        }

        private void btn_send_Click(object sender, EventArgs e)
        {
            try
            {
                if (txt_send.Text == "realtekno")
                {
                    //enable işlemi
                    Settings.Default.adminMode = true;
                    Settings.Default.Save();
                    int rectCount = Settings.Default.Rect.Split('/').Length;
                    int gbCount = Settings.Default.Cam.Split('/').Length;
                    for (int a = 0; a < gbCount; a++)
                    {
                        foreach (Control groupBox in gb[a].Controls)
                        {
                            foreach (Control control in groupBox.Controls)
                            {
                                control.Enabled = true;
                                control.Visible = true;
                            }
                        }
                        camSetget[a].Visible = true;
                        camSetset[a].Visible = true;
                        btnAdd[a].Visible = true;
                    }

                    for (int i = 0; i < rectCount; i++)
                    {
                        btn_rect[i].Visible = true;
                        btn_del[i].Visible = true;
                        cmb_islem[i].Enabled = true;
                    }

                    btn_addCam.Visible = true;
                    btn_delCam.Visible = true;
                }
                else
                {
                    if (Program.mainPage.Uart_to_IO.IsOpen)
                    {
                        bool sonuc = true;
                        string data = txt_send.Text;
                        while (sonuc)
                        {
                            if (data.Length % 8 != 0)
                            {
                                data = "0" + data;
                            }
                            else
                            {
                                sonuc = false;
                            }
                        }
                        var listt = new byte[data.Length / 8];
                        byte p_c = 0;
                        int y = 1;
                        for (int x = 0; x < (data.Length / 8); x++)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                int location = (x * 8) + i;
                                byte aa;
                                if (data.Substring(location, 1) == "0") { aa = 0; } else { aa = 1; }
                                p_c = (byte)((p_c << 1) | aa);
                            }
                            listt[x] = p_c;
                            y = x + 1;
                        }
                        Program.mainPage.Uart_to_IO.Write(listt, 0, y);
                    }
                    else
                    {
                        txt_send.Text = "UART açık değil!";
                    }

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

            txt_send.Text = "";
        }

        private void txt_send_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    if (txt_send.Text == "realtekno")
                    {
                        //enable işlemi
                        Settings.Default.adminMode = true;
                        Settings.Default.Save();
                        int rectCount = Settings.Default.Rect.Split('/').Length;
                        int gbCount = Settings.Default.Cam.Split('/').Length;
                        for (int a = 0; a < gbCount; a++)
                        {
                            foreach (Control groupBox in gb[a].Controls)
                            {
                                foreach (Control control in groupBox.Controls)
                                {
                                    control.Enabled = true;
                                    control.Visible = true;
                                }
                            }
                            camSetget[a].Visible = true;
                            camSetset[a].Visible = true;
                            btnAdd[a].Visible = true;
                        }

                        for (int i = 0; i < rectCount; i++)
                        {
                            btn_rect[i].Visible = true;
                            btn_del[i].Visible = true;
                            cmb_islem[i].Enabled = true;
                        }

                        btn_addCam.Visible = true;
                        btn_delCam.Visible = true;
                    }
                    else
                    {
                        if (Program.mainPage.Uart_to_IO.IsOpen)
                        {
                            bool sonuc = true;
                            string data = txt_send.Text;
                            while (sonuc)
                            {
                                if (data.Length % 8 != 0)
                                {
                                    data = "0" + data;
                                }
                                else
                                {
                                    sonuc = false;
                                }
                            }
                            var listt = new byte[data.Length / 8];
                            byte p_c = 0;
                            int y = 1;
                            for (int x = 0; x < (data.Length / 8); x++)
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    int location = (x * 8) + i;
                                    byte aa;
                                    if (data.Substring(location, 1) == "0") { aa = 0; } else { aa = 1; }
                                    p_c = (byte)((p_c << 1) | aa);
                                }
                                listt[x] = p_c;
                                y = x + 1;
                            }
                            Program.mainPage.Uart_to_IO.Write(listt, 0, y);
                        }
                        else
                        {
                            txt_send.Text = "UART açık değil!";
                        }

                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }

                txt_send.Text = "";
            }
        }

        private void btn_bekleme_Click(object sender, EventArgs e)
        {
            string beklemeci = nud_Wait1.Value + "-" + nud_Wait2.Value + "-" + nud_Wait3.Value + "-" + nud_Wait4.Value + "-" + nud_Wait5.Value + "-" + nud_Wait6.Value + "-" + nud_Wait7.Value + "-" + nud_Wait8.Value + "-" + nud_Wait9.Value;
            Settings.Default.Bekleme = beklemeci;
            Settings.Default.Save();
        }

        private void btn_bit_Click(object sender, EventArgs e)
        {
            if (nud_Lamba.Value < 1 || nud_Lamba.Value > 8) nud_Lamba.Value = 1;
            if (nud_Piston.Value < 1 || nud_Piston.Value > 8) nud_Lamba.Value = 2;
            if (nud_Kase.Value < 1 || nud_Kase.Value > 8) nud_Lamba.Value = 3;
            if (nud_bosBit.Value < 1 || nud_bosBit.Value > 8) nud_Lamba.Value = 4;
            Settings.Default.Lamba = (int)nud_Lamba.Value - 1;
            Settings.Default.Piston = (int)nud_Piston.Value - 1;
            Settings.Default.Kase = (int)nud_Kase.Value - 1;
            Settings.Default.bosBit = (int)nud_bosBit.Value - 1;
            Settings.Default.Save();
        }
        private void btn_txtDosyaAc_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Metin Dosyaları|*.txt";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Dosya_goster(openFileDialog.FileName);
            }
        }
        public void Dosya_goster(string yol)
        {
            string dosyaYolu = yol;
            lbl_path.Text = dosyaYolu;
            StreamReader str = new StreamReader(dosyaYolu);
            string text = str.ReadToEnd();
            str.Close();

            lst_txtData.Items.Clear();
            lst_txtData.Groups.Clear();

            if (text.StartsWith("/"))
            {
                text = text.Substring(1);
            }
            if (text.EndsWith("/"))
            {
                text = text.Substring(0, text.Length - 1);
            }

            string[] satir = text.Trim().Split('/');
            Dictionary<string, ListViewGroup> groupDict = new Dictionary<string, ListViewGroup>();

            for (int i = 0; i < satir.Length; i++)
            {
                string[] row = satir[i].Split(',');

                string groupKey = row[0].Trim().Split('-')[1];

                ListViewGroup group;
                if (groupDict.ContainsKey(groupKey))
                {
                    group = groupDict[groupKey];
                }
                else
                {
                    group = new ListViewGroup("İşlem " + groupKey, HorizontalAlignment.Left);
                    lst_txtData.Groups.Add(group);
                    groupDict.Add(groupKey, group);
                }

                var lvi = new ListViewItem(row);
                lvi.Group = group;
                lst_txtData.Items.Add(lvi);
            }
            if (lst_txtData.Items.Count > 0)
            {
                lst_txtData.EnsureVisible(lst_txtData.Items.Count - 1);
            }

        }
        private void chb_plc_enable_CheckedChanged(object sender, EventArgs e)
        {

            Settings.Default.PlcEnable = chb_plc_enable.Checked;
            Settings.Default.Save();
        }
        private void chb_uart_enable_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.UartEnable = chb_uart_enable.Checked;
            Settings.Default.Save();
        }

        private void Close_Click(object sender, EventArgs e) { Close(); }
        //Pencere Taşıma End

    }
}
