namespace RealVisionLab
{
    partial class Message
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbl_banner = new System.Windows.Forms.Label();
            this.pb_rtLogo = new System.Windows.Forms.PictureBox();
            this.lbl_msg = new System.Windows.Forms.Label();
            this.txb_excep = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_rtLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_banner
            // 
            this.lbl_banner.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(39)))), ((int)(((byte)(53)))));
            this.lbl_banner.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbl_banner.ForeColor = System.Drawing.Color.White;
            this.lbl_banner.Location = new System.Drawing.Point(0, 0);
            this.lbl_banner.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_banner.Name = "lbl_banner";
            this.lbl_banner.Size = new System.Drawing.Size(597, 46);
            this.lbl_banner.TabIndex = 155;
            this.lbl_banner.Text = "Form_Banner";
            this.lbl_banner.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pb_rtLogo
            // 
            this.pb_rtLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(39)))), ((int)(((byte)(53)))));
            this.pb_rtLogo.Image = global::RealVisionLab.Properties.Resources.realtekno_logo;
            this.pb_rtLogo.Location = new System.Drawing.Point(4, 3);
            this.pb_rtLogo.Margin = new System.Windows.Forms.Padding(4);
            this.pb_rtLogo.Name = "pb_rtLogo";
            this.pb_rtLogo.Size = new System.Drawing.Size(133, 38);
            this.pb_rtLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb_rtLogo.TabIndex = 156;
            this.pb_rtLogo.TabStop = false;
            // 
            // lbl_msg
            // 
            this.lbl_msg.BackColor = System.Drawing.Color.White;
            this.lbl_msg.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbl_msg.Location = new System.Drawing.Point(0, 46);
            this.lbl_msg.Name = "lbl_msg";
            this.lbl_msg.Size = new System.Drawing.Size(597, 57);
            this.lbl_msg.TabIndex = 157;
            this.lbl_msg.Text = "Message";
            this.lbl_msg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txb_excep
            // 
            this.txb_excep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txb_excep.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txb_excep.Location = new System.Drawing.Point(0, 103);
            this.txb_excep.Multiline = true;
            this.txb_excep.Name = "txb_excep";
            this.txb_excep.Size = new System.Drawing.Size(597, 247);
            this.txb_excep.TabIndex = 158;
            // 
            // Message
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 350);
            this.Controls.Add(this.txb_excep);
            this.Controls.Add(this.lbl_msg);
            this.Controls.Add(this.pb_rtLogo);
            this.Controls.Add(this.lbl_banner);
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Message";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Message";
            ((System.ComponentModel.ISupportInitialize)(this.pb_rtLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_banner;
        private System.Windows.Forms.PictureBox pb_rtLogo;
        private System.Windows.Forms.Label lbl_msg;
        private System.Windows.Forms.TextBox txb_excep;
    }
}