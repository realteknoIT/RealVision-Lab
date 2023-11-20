using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealVisionLab
{
    public partial class PasswordForm : Form
    {
        public bool isTrue = false;
        bool newPass = false;

        public PasswordForm()
        {
            InitializeComponent();
        }

        private void PasswordForm_Load(object sender, EventArgs e)
        {
            txt_password.Focus();
            isTrue = false;
        }

        private async void txt_password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (newPass)
                {
                    newPass = false;
                    if (txt_password.Text == null || txt_password.TextLength != 4)
                    {
                        label2.ForeColor = Color.Red;
                        label2.Text = "Şifre boş olmamalı ve 4 haneli olmalıdır.";
                        await Task.Delay(1500);
                        Close();
                    }
                    else
                    {
                        Properties.Settings.Default.userPassword = txt_password.Text.Trim();
                        Properties.Settings.Default.Save();
                        label2.ForeColor = Color.Green;
                        label2.Text = "Şifre Başarıyla Güncellendi";
                        await Task.Delay(1500);
                        Close();
                    }

                }
                else
                {
                    if (txt_password.Text == null || (txt_password.TextLength != 4 && txt_password.TextLength != 6))
                    {
                        label2.ForeColor = Color.Red;
                        label2.Text = "Şifre boş olmamalı ve 4 haneli olmalıdır.";
                        await Task.Delay(1500);
                        Close();
                    }
                    else if (
                            txt_password.Text.Substring(4) == "*+" &&
                            txt_password.TextLength == 6 &&
                                (
                                txt_password.Text.Substring(0, 4) == "2580" ||
                                txt_password.Text.Substring(0, 4) == Properties.Settings.Default.userPassword
                                )
                            )
                    {
                        newPass = true;
                        txt_password.Clear();
                        label2.Text = "Yeni Şireyi Girin";
                        label2.ForeColor = Color.Orange;
                    }
                    else
                    {
                        if (Properties.Settings.Default.userPassword == txt_password.Text.Trim() || txt_password.Text.Trim() == "2580")
                        {
                            label2.ForeColor = Color.Green;
                            label2.Text = "Şifre doğru";
                            isTrue = true;
                            await Task.Delay(1500);
                            Close();
                        }
                        else
                        {
                            label2.ForeColor = Color.Red;
                            label2.Text = "Şifre yanlış";
                            isTrue = false;
                            await Task.Delay(1500);
                            Close();
                        }
                    }
                }
            }
        }
    }
}
