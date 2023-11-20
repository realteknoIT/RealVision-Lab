using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace RealVisionLab
{
    internal static class Program
    {
        public static MainPage mainPage;

        /// <summary>
        /// Uygulamanın ana girdi noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mainPage = new MainPage();
            Application.ThreadException += Application_ThreadException;
            Application.Run(mainPage);
        }
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            string newValue = "Hata: " + ex.Message + " -- Hata detayları: " + ex.StackTrace + "\r\n";
            try
            {
                using (StreamWriter writer = File.AppendText("debug.txt"))
                {
                    writer.WriteLine($"\r\n--------\r\n {DateTime.Now} - Hata Kodu: {newValue} ------------\r\n");
                }
            }
            catch
            {
            }

            Application.Restart();
        }
    }
}
