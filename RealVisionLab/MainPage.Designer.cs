using System.Drawing;

namespace RealVisionLab
{
    partial class MainPage
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this.lbl_alarm = new System.Windows.Forms.Label();
			this.lbl_productName = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.lbl_copyright = new System.Windows.Forms.Label();
			this.lbl_webSite = new System.Windows.Forms.Label();
			this.lbl_info = new System.Windows.Forms.Label();
			this.p_scene = new System.Windows.Forms.Panel();
			this.pb_scene = new System.Windows.Forms.PictureBox();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.Reset_setting = new System.Windows.Forms.Label();
			this.Uart_to_IO = new System.IO.Ports.SerialPort(this.components);
			this.port_text = new System.Windows.Forms.Label();
			this.lbl_datetime = new System.Windows.Forms.Label();
			this.tx_debug = new System.Windows.Forms.TextBox();
			this.pb_companyLogo = new System.Windows.Forms.PictureBox();
			this.pb_realteknoLogo = new System.Windows.Forms.PictureBox();
			this.lbl_islemTime = new System.Windows.Forms.Label();
			this.txt_qr = new System.Windows.Forms.Label();
			this.qrGelen = new System.Windows.Forms.TextBox();
			this.txt_qr2 = new System.Windows.Forms.Label();
			this.Photo = new System.Windows.Forms.Button();
			this.sec = new System.Windows.Forms.Button();
			this.iptal = new System.Windows.Forms.Button();
			this.RotUzunluk = new System.Windows.Forms.Label();
			this.panel2.SuspendLayout();
			this.p_scene.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pb_scene)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pb_companyLogo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pb_realteknoLogo)).BeginInit();
			this.SuspendLayout();
			// 
			// lbl_alarm
			// 
			this.lbl_alarm.BackColor = System.Drawing.SystemColors.Control;
			this.lbl_alarm.Font = new System.Drawing.Font("Microsoft Sans Serif", 40F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.lbl_alarm.ForeColor = System.Drawing.Color.Red;
			this.lbl_alarm.Location = new System.Drawing.Point(0, 140);
			this.lbl_alarm.Name = "lbl_alarm";
			this.lbl_alarm.Size = new System.Drawing.Size(1280, 85);
			this.lbl_alarm.TabIndex = 16;
			this.lbl_alarm.Text = "PARÇA BULUNAMADI";
			this.lbl_alarm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbl_productName
			// 
			this.lbl_productName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(39)))), ((int)(((byte)(53)))));
			this.lbl_productName.Cursor = System.Windows.Forms.Cursors.Default;
			this.lbl_productName.Font = new System.Drawing.Font("Microsoft Sans Serif", 24.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.lbl_productName.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lbl_productName.Location = new System.Drawing.Point(0, 0);
			this.lbl_productName.Margin = new System.Windows.Forms.Padding(0);
			this.lbl_productName.Name = "lbl_productName";
			this.lbl_productName.Size = new System.Drawing.Size(1280, 140);
			this.lbl_productName.TabIndex = 13;
			this.lbl_productName.Text = "Tank Blocker Rear";
			this.lbl_productName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lbl_productName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Move_MouseDown);
			this.lbl_productName.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Move_MouseMove);
			this.lbl_productName.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Move_MouseUp);
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(39)))), ((int)(((byte)(53)))));
			this.panel2.Controls.Add(this.lbl_copyright);
			this.panel2.Controls.Add(this.lbl_webSite);
			this.panel2.Controls.Add(this.lbl_info);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 994);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(1280, 30);
			this.panel2.TabIndex = 18;
			// 
			// lbl_copyright
			// 
			this.lbl_copyright.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.lbl_copyright.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lbl_copyright.Location = new System.Drawing.Point(390, 1);
			this.lbl_copyright.Name = "lbl_copyright";
			this.lbl_copyright.Size = new System.Drawing.Size(500, 30);
			this.lbl_copyright.TabIndex = 2;
			this.lbl_copyright.Text = "© 2023 - RealTekno Tarafından Tüm Hakları Saklıdır.";
			this.lbl_copyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbl_webSite
			// 
			this.lbl_webSite.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.lbl_webSite.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lbl_webSite.Location = new System.Drawing.Point(1060, 0);
			this.lbl_webSite.Name = "lbl_webSite";
			this.lbl_webSite.Size = new System.Drawing.Size(209, 29);
			this.lbl_webSite.TabIndex = 1;
			this.lbl_webSite.Text = "www.realtekno.com";
			this.lbl_webSite.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lbl_webSite.Click += new System.EventHandler(this.Close_Click);
			// 
			// lbl_info
			// 
			this.lbl_info.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.lbl_info.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lbl_info.Location = new System.Drawing.Point(10, 1);
			this.lbl_info.Name = "lbl_info";
			this.lbl_info.Size = new System.Drawing.Size(209, 29);
			this.lbl_info.TabIndex = 0;
			this.lbl_info.Text = "info@realtekno.com";
			this.lbl_info.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// p_scene
			// 
			this.p_scene.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(150)))), ((int)(((byte)(0)))));
			this.p_scene.Controls.Add(this.pb_scene);
			this.p_scene.Location = new System.Drawing.Point(7, 242);
			this.p_scene.Name = "p_scene";
			this.p_scene.Size = new System.Drawing.Size(1260, 720);
			this.p_scene.TabIndex = 19;
			// 
			// pb_scene
			// 
			this.pb_scene.BackColor = System.Drawing.Color.Black;
			this.pb_scene.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.pb_scene.Image = global::RealVisionLab.Properties.Resources.carpi;
			this.pb_scene.Location = new System.Drawing.Point(10, 10);
			this.pb_scene.Name = "pb_scene";
			this.pb_scene.Size = new System.Drawing.Size(1240, 700);
			this.pb_scene.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pb_scene.TabIndex = 17;
			this.pb_scene.TabStop = false;
			this.pb_scene.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pb_scene_MouseClick);
			// 
			// timer
			// 
			this.timer.Tick += new System.EventHandler(this.Main_timer_Tick);
			// 
			// Reset_setting
			// 
			this.Reset_setting.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(39)))), ((int)(((byte)(93)))));
			this.Reset_setting.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(39)))), ((int)(((byte)(53)))));
			this.Reset_setting.Location = new System.Drawing.Point(12, 9);
			this.Reset_setting.Name = "Reset_setting";
			this.Reset_setting.Size = new System.Drawing.Size(5, 5);
			this.Reset_setting.TabIndex = 20;
			this.Reset_setting.Text = "AyarSıfırlama";
			this.Reset_setting.Click += new System.EventHandler(this.Reset_setting_Click);
			// 
			// port_text
			// 
			this.port_text.AutoSize = true;
			this.port_text.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.port_text.ForeColor = System.Drawing.Color.Red;
			this.port_text.Location = new System.Drawing.Point(4, 974);
			this.port_text.Name = "port_text";
			this.port_text.Size = new System.Drawing.Size(165, 17);
			this.port_text.TabIndex = 21;
			this.port_text.Text = "Çıkış Cihazı Bağlanamadı";
			this.port_text.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbl_datetime
			// 
			this.lbl_datetime.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.lbl_datetime.ForeColor = System.Drawing.Color.Black;
			this.lbl_datetime.Location = new System.Drawing.Point(984, 965);
			this.lbl_datetime.Name = "lbl_datetime";
			this.lbl_datetime.Size = new System.Drawing.Size(286, 26);
			this.lbl_datetime.TabIndex = 22;
			this.lbl_datetime.Text = "-";
			this.lbl_datetime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lbl_datetime.Click += new System.EventHandler(this.lbl_datetime_Click);
			// 
			// tx_debug
			// 
			this.tx_debug.BackColor = System.Drawing.SystemColors.Control;
			this.tx_debug.Location = new System.Drawing.Point(884, 974);
			this.tx_debug.Multiline = true;
			this.tx_debug.Name = "tx_debug";
			this.tx_debug.ReadOnly = true;
			this.tx_debug.Size = new System.Drawing.Size(396, 20);
			this.tx_debug.TabIndex = 24;
			this.tx_debug.Visible = false;
			this.tx_debug.TextChanged += new System.EventHandler(this.Tx_debug_TextChanged);
			// 
			// pb_companyLogo
			// 
			this.pb_companyLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(39)))), ((int)(((byte)(53)))));
			this.pb_companyLogo.Image = global::RealVisionLab.Properties.Resources.marturpolmar_logo;
			this.pb_companyLogo.Location = new System.Drawing.Point(997, 11);
			this.pb_companyLogo.Name = "pb_companyLogo";
			this.pb_companyLogo.Size = new System.Drawing.Size(280, 118);
			this.pb_companyLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pb_companyLogo.TabIndex = 15;
			this.pb_companyLogo.TabStop = false;
			this.pb_companyLogo.Click += new System.EventHandler(this.Pb_companyLogo_Click);
			// 
			// pb_realteknoLogo
			// 
			this.pb_realteknoLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(39)))), ((int)(((byte)(53)))));
			this.pb_realteknoLogo.Image = global::RealVisionLab.Properties.Resources.realtekno_logo;
			this.pb_realteknoLogo.Location = new System.Drawing.Point(7, 40);
			this.pb_realteknoLogo.Name = "pb_realteknoLogo";
			this.pb_realteknoLogo.Size = new System.Drawing.Size(280, 60);
			this.pb_realteknoLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pb_realteknoLogo.TabIndex = 14;
			this.pb_realteknoLogo.TabStop = false;
			this.pb_realteknoLogo.Click += new System.EventHandler(this.pb_RealTekno_Click);
			// 
			// lbl_islemTime
			// 
			this.lbl_islemTime.AutoSize = true;
			this.lbl_islemTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.lbl_islemTime.ForeColor = System.Drawing.Color.DarkGray;
			this.lbl_islemTime.Location = new System.Drawing.Point(345, 974);
			this.lbl_islemTime.Name = "lbl_islemTime";
			this.lbl_islemTime.Size = new System.Drawing.Size(0, 17);
			this.lbl_islemTime.TabIndex = 25;
			this.lbl_islemTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txt_qr
			// 
			this.txt_qr.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.txt_qr.Location = new System.Drawing.Point(8, 962);
			this.txt_qr.Name = "txt_qr";
			this.txt_qr.Size = new System.Drawing.Size(364, 13);
			this.txt_qr.TabIndex = 26;
			this.txt_qr.Text = "QR:";
			// 
			// qrGelen
			// 
			this.qrGelen.Location = new System.Drawing.Point(390, 110);
			this.qrGelen.Multiline = true;
			this.qrGelen.Name = "qrGelen";
			this.qrGelen.Size = new System.Drawing.Size(500, 30);
			this.qrGelen.TabIndex = 500;
			this.qrGelen.Visible = false;
			this.qrGelen.KeyDown += new System.Windows.Forms.KeyEventHandler(this.qrGelen_Enter);
			// 
			// txt_qr2
			// 
			this.txt_qr2.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.txt_qr2.Location = new System.Drawing.Point(798, 962);
			this.txt_qr2.Name = "txt_qr2";
			this.txt_qr2.Size = new System.Drawing.Size(470, 10);
			this.txt_qr2.TabIndex = 501;
			this.txt_qr2.Text = "QR:";
			// 
			// Photo
			// 
			this.Photo.Location = new System.Drawing.Point(1195, 143);
			this.Photo.Name = "Photo";
			this.Photo.Size = new System.Drawing.Size(75, 23);
			this.Photo.TabIndex = 502;
			this.Photo.Text = "Photo";
			this.Photo.UseVisualStyleBackColor = true;
			this.Photo.Click += new System.EventHandler(this.Photo_Click);
			// 
			// sec
			// 
			this.sec.Location = new System.Drawing.Point(1195, 172);
			this.sec.Name = "sec";
			this.sec.Size = new System.Drawing.Size(75, 23);
			this.sec.TabIndex = 503;
			this.sec.Text = "Seç";
			this.sec.UseVisualStyleBackColor = true;
			this.sec.Click += new System.EventHandler(this.sec_Click);
			// 
			// iptal
			// 
			this.iptal.Location = new System.Drawing.Point(1195, 201);
			this.iptal.Name = "iptal";
			this.iptal.Size = new System.Drawing.Size(75, 23);
			this.iptal.TabIndex = 504;
			this.iptal.Text = "İptal";
			this.iptal.UseVisualStyleBackColor = true;
			this.iptal.Click += new System.EventHandler(this.iptal_Click);
			// 
			// RotUzunluk
			// 
			this.RotUzunluk.AutoSize = true;
			this.RotUzunluk.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
			this.RotUzunluk.Location = new System.Drawing.Point(545, 970);
			this.RotUzunluk.Name = "RotUzunluk";
			this.RotUzunluk.Size = new System.Drawing.Size(187, 16);
			this.RotUzunluk.TabIndex = 505;
			this.RotUzunluk.Text = "SolKol : 100 - SağKol : 101";
			// 
			// MainPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1280, 1024);
			this.Controls.Add(this.RotUzunluk);
			this.Controls.Add(this.iptal);
			this.Controls.Add(this.sec);
			this.Controls.Add(this.Photo);
			this.Controls.Add(this.txt_qr2);
			this.Controls.Add(this.qrGelen);
			this.Controls.Add(this.txt_qr);
			this.Controls.Add(this.lbl_islemTime);
			this.Controls.Add(this.tx_debug);
			this.Controls.Add(this.lbl_datetime);
			this.Controls.Add(this.port_text);
			this.Controls.Add(this.Reset_setting);
			this.Controls.Add(this.p_scene);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.lbl_alarm);
			this.Controls.Add(this.pb_companyLogo);
			this.Controls.Add(this.pb_realteknoLogo);
			this.Controls.Add(this.lbl_productName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "MainPage";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainPage_FormClosing);
			this.Load += new System.EventHandler(this.MainPage_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.password_KeyDown);
			this.panel2.ResumeLayout(false);
			this.p_scene.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pb_scene)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pb_companyLogo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pb_realteknoLogo)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.PictureBox pb_companyLogo;
        private System.Windows.Forms.PictureBox pb_realteknoLogo;
        private System.Windows.Forms.Label lbl_productName;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbl_copyright;
        private System.Windows.Forms.Label lbl_webSite;
        private System.Windows.Forms.Label lbl_info;
        private System.Windows.Forms.Panel p_scene;
        public System.Windows.Forms.Label lbl_alarm;
        private System.Windows.Forms.Label Reset_setting;
        private System.Windows.Forms.Label port_text;
        private System.Windows.Forms.Label lbl_datetime;
        public System.Windows.Forms.TextBox tx_debug;
        public System.Windows.Forms.PictureBox pb_scene;
        public System.Windows.Forms.Timer timer;
        public System.IO.Ports.SerialPort Uart_to_IO;
        private System.Windows.Forms.Label lbl_islemTime;
        private System.Windows.Forms.Label txt_qr;
        private System.Windows.Forms.TextBox qrGelen;
        private System.Windows.Forms.Label txt_qr2;
		private System.Windows.Forms.Button Photo;
		private System.Windows.Forms.Button sec;
		private System.Windows.Forms.Button iptal;
		private System.Windows.Forms.Label RotUzunluk;
	}
}

