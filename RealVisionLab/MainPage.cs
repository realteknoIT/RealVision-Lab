using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;
using DirectShowLib;
using RealVisionLab.Properties;
using Sharp7;
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using ZXing;





namespace RealVisionLab
{
	public partial class MainPage : Form
	{

#if DEBUG
		bool debugMode = true;
#else
        bool debugMode = false;
#endif

		//---------- Değişken Tanımlamaları Başlangıç ------------------------------------------------------
		private Property set;

		public static S7Client
			client = null;

		public static VideoCaptureDevice[]
			cam;

		public static FilterInfoCollection
			devices = new FilterInfoCollection(AForge.Video.DirectShow.FilterCategory.VideoInputDevice);

		public static Label[]
		   artcle;

		public static Label[,]
			lbl;

		public static PictureBox[]
			pbox;

		public static Color
			blk = Color.FromArgb(255, 0, 0, 0),
			wht = Color.FromArgb(255, 255, 255, 255),
			gry = Color.FromArgb(255, 128, 128, 128),
			red = Color.FromArgb(255, 255, 0, 0),
			org = Color.FromArgb(255, 255, 165, 0),
			grn = Color.FromArgb(255, 0, 255, 0),
			ylw = Color.FromArgb(255, 255, 255, 0);

		public static Bitmap[]
			bmp;
		public static Rectangle[]
			Rect;


		public static bool
			capture = false,
			buton = false,
			rst_buton = false,
			alarm = false,
			capturing = true,
			islem = false,
			rotuzunlukvisible = false,
			gThrs = false,
			qr = false,
			uartResetAlreadyOpened = false;
		public static bool[]
			cam_var,
			bitArray = new bool[8] { false, false, false, false, false, false, false, false };
		public static bool[,]
			hata;
		public static byte[]
			controlPlc = new byte[1];
		public static int
			tab_index = 0,
			alarmtime = 0,
			capture_count = 0,
			tarama_count = 0,
			caming_id = 0,
			result = 1,
			gthresint = 128,
			renk = 0,
			newFileIndex,
			uartCount = 0;
		public static int[]
			Cam_no,
			Cam_res,
			Thrs;
		public static int[,]
			Ofset;
		public static float[,]
			oran;
		public static string
			reportold,
			report,
			txtReport,
			debug,
			folderPath = "Values",
			filePath = "",
			qrCodeText;
		public static string[]
			rectString,
			thrsString;


		//---------- Değişken Tanımlamaları Bitiş --------------------------------------------------------------------

		public MainPage()
		{
			try
			{
				if(debugMode)
				{
					this.WindowState = FormWindowState.Normal;
				}
				else
				{
					this.WindowState = FormWindowState.Maximized;
				}
				CheckForIllegalCrossThreadCalls = false;
				InitializeComponent();
				ApplyCameraSettingsToAll();

				client = new S7Client();

				string camString = Settings.Default.Cam;
				Cam_no = new int[camString.Split('/').Length];
				Cam_res = new int[camString.Split('/').Length];
				cam_var = new bool[camString.Split('/').Length];
				oran = new float[camString.Split('/').Length, 2];
				cam = new VideoCaptureDevice[camString.Split('/').Length];

				visibility(false);
				Cam_adjust();

				Label_adjust(org);

				if(!Directory.Exists(folderPath))
				{
					Directory.CreateDirectory(folderPath);
				}
				timer.Start();
			}
			catch(Exception ex)
			{
				tx_debug.Text = ex.ToString() + "\r\n";
			}
		}
		private void MainPage_Load(object sender, EventArgs e)
		{
			lbl_productName.Text = Settings.Default.productName;
			filePath = folderPath + "/" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt";
			Settings.Default.adminMode = false;
			Settings.Default.Save();
			try
			{
				if(File.Exists(filePath))
				{
					string fileContent = File.ReadAllText(filePath);
					string[] contentString = fileContent.Split('/');
					int id = 1;
					if(fileContent.StartsWith("/"))
					{
						id++;
					}

					if(fileContent.EndsWith("/"))
					{
						id++;
					}

					tarama_count = int.Parse(contentString[contentString.Length - id].Split(',')[0].Split('-')[1]);
					tarama_count++;
				}
				else
				{
					File.Create(filePath).Close();
					tarama_count = 0;
				}
			}
			catch(Exception ex)
			{
				tx_debug.Text = "Load Error: " + ex.Message;
			}
			if(!debugMode)
			{
				Photo.Visible = false;
				sec.Visible = false;
				iptal.Visible = false;
				txt_qr.Visible = false;
				txt_qr2.Visible = false;
			}
		}
		private void ClearControls()
		{
			try
			{
				foreach(Control control in pb_scene.Controls)
				{
					control.Dispose(); // Nesneyi bellekten kaldırma
				}
				pb_scene.Controls.Clear(); // Kontrol listesini temizleme

				// Rect, pbox, bmp, lbl, Ofset dizilerini boşaltma
				Rect = null;
				pbox = null;
				bmp = null;
				lbl = null;
				Ofset = null;
			}
			catch(Exception ex)
			{
				tx_debug.Text = ex.ToString() + "\r\n";
			}
		}
		public void Label_adjust(Color color)
		{
			try
			{
				string[] rectString = Settings.Default.Rect.Split('/');
				lbl_productName.Text = Settings.Default.productName;
				if(Rect == null || rectString.Length != Rect.Length)
				{

					ClearControls();
					hata = new bool[cam.Length, rectString.Length];
					Rect = new Rectangle[rectString.Length];
					pbox = new PictureBox[Rect.Length];
					bmp = new Bitmap[Rect.Length];
					lbl = new Label[Rect.Length, 4];
					Ofset = new int[Rect.Length, 3];
					Thrs = new int[Rect.Length];



					for(int r = 0; r < rectString.Length; r++)
					{
						for(int c = 0; c < cam.Length; c++)
						{
							hata[c, r] = false;
						}
						lbl[r, 0] = new Label();
						lbl[r, 1] = new Label();
						lbl[r, 2] = new Label();
						lbl[r, 3] = new Label();
						pbox[r] = new PictureBox();
						//top
						lbl[r, 0].Height = 2;
						lbl[r, 0].BackColor = color;
						pb_scene.Controls.Add(lbl[r, 0]);
						//right
						lbl[r, 1].Width = 2;
						lbl[r, 1].BackColor = color;
						pb_scene.Controls.Add(lbl[r, 1]);
						//bottom
						lbl[r, 2].Height = 2;
						lbl[r, 2].BackColor = color;
						pb_scene.Controls.Add(lbl[r, 2]);
						//left
						lbl[r, 3].Width = 2;
						lbl[r, 3].BackColor = color;
						pb_scene.Controls.Add(lbl[r, 3]);

						pbox[r].SizeMode = PictureBoxSizeMode.StretchImage;
						pbox[r].BackColor = color;
						pbox[r].MouseClick += pb_scene_MouseClick;
						pb_scene.Controls.Add(pbox[r]);
					}
				}
				int rect_cam;
				for(int i = 0; i < rectString.Length; i++)
				{
					Thrs[i] = int.Parse(rectString[i].Split(',')[8]);
					Rect[i].X = int.Parse(rectString[i].Split(',')[0]);
					Rect[i].Y = int.Parse(rectString[i].Split(',')[1]);
					Rect[i].Width = int.Parse(rectString[i].Split(',')[2]);
					Rect[i].Height = int.Parse(rectString[i].Split(',')[3]);
					Ofset[i, 0] = int.Parse(rectString[i].Split(',')[4]);
					Ofset[i, 2] = int.Parse(rectString[i].Split(',')[5]);
					rect_cam = int.Parse(rectString[i].Split(',')[6]);
					//top
					lbl[i, 0].Left = (int)(Rect[i].X / oran[rect_cam, 1]);
					lbl[i, 0].Top = (int)(Rect[i].Y / oran[rect_cam, 0]);
					lbl[i, 0].Width = (int)(Rect[i].Width / oran[rect_cam, 1]);
					lbl[i, 0].BringToFront();
					//right
					lbl[i, 1].Left = lbl[i, 0].Left + lbl[i, 0].Width;
					lbl[i, 1].Top = lbl[i, 0].Top;
					lbl[i, 1].Height = (int)(Rect[i].Height / oran[rect_cam, 0]);
					lbl[i, 1].BringToFront();
					//bottom
					lbl[i, 2].Left = lbl[i, 0].Left;
					lbl[i, 2].Top = lbl[i, 0].Top + lbl[i, 1].Height;
					lbl[i, 2].Width = lbl[i, 0].Width + 2;
					lbl[i, 2].BringToFront();
					//left
					lbl[i, 3].Left = lbl[i, 0].Left;
					lbl[i, 3].Top = lbl[i, 0].Top;
					lbl[i, 3].Height = lbl[i, 1].Height;
					lbl[i, 3].BringToFront();
					//Mini PictureBox
					pbox[i].Left = lbl[i, 0].Left + 2;
					pbox[i].Top = lbl[i, 0].Top + 2;
					pbox[i].Width = lbl[i, 0].Width - 2;
					pbox[i].Height = lbl[i, 1].Height - 2;
					pbox[i].BringToFront();
				}

				if(artcle == null)
				{
					artcle = new Label[2];

					artcle[0] = new Label();
					artcle[1] = new Label();
					artcle[0].BackColor = org;
					artcle[1].BackColor = org;

					artcle[1].Width = 60;
					artcle[1].Height = 4;
					artcle[0].Width = 4;
					artcle[0].Height = 60;

					pb_scene.Controls.Add(artcle[0]);
					pb_scene.Controls.Add(artcle[1]);

				}

				artcle[0].Location = new Point((pb_scene.Width / 2) - 2, (pb_scene.Height / 2) - 30);
				artcle[1].Location = new Point((pb_scene.Width / 2) - 30, (pb_scene.Height / 2) - 2);

				artcle[0].BringToFront();
				artcle[1].BringToFront();


			}
			catch(Exception ex)
			{
				tx_debug.Text = ex.ToString() + "\r\n";
			}
		}
		public void Cam_adjust()
		{
			try
			{
				string camString = Settings.Default.Cam;

				for(int i = 0; i < camString.Split('/').Length; i++)
				{
					Cam_no[i] = int.Parse(camString.Split('/')[i].Split(',')[0]);
					Cam_res[i] = int.Parse(camString.Split('/')[i].Split(',')[1]);
					if(Cam_no[i] < 0) Cam_no[i] = 0;
					if(Cam_res[i] < 0) Cam_res[i] = 0;
					capture = false;

					if(cam[i] != null && cam[i].IsRunning)
					{
						cam[i].SignalToStop();
						cam[i].WaitForStop();
					}
				}
				for(int i = 0; i < cam_var.Length; i++)
				{
					if(devices.Count > Cam_no[i]) cam_var[i] = true;
					else cam_var[i] = false;
				}


				for(int i = 0; i < cam_var.Length; i++)
				{

					if(cam_var[i])
					{
						try
						{
							// Kamera 1 Başlat 
							cam[i] = new VideoCaptureDevice(devices[Cam_no[i]].MonikerString);
							if(cam[i].VideoCapabilities.Length - 1 < Cam_res[i]) { Cam_res[i] = cam[i].VideoCapabilities.Length - 1; }
							cam[i].VideoResolution = cam[i].VideoCapabilities[Cam_res[i]];
							cam[i].NewFrame += New_Frame;
							cam[i].Start();

							oran[i, 0] = (float)cam[i].VideoResolution.FrameSize.Height / (float)pb_scene.Height;
							pb_scene.Width = (int)(cam[0].VideoResolution.FrameSize.Width / oran[0, 0]);
							oran[i, 1] = (float)cam[i].VideoResolution.FrameSize.Width / (float)pb_scene.Width;
							p_scene.Width = pb_scene.Width + 20;
							p_scene.Left = (Width - p_scene.Width) / 2;
							p_scene.Top = ((Height - (p_scene.Height + lbl_productName.Height + panel2.Height)) / 2) + lbl_productName.Height;
						}
						catch(Exception ex)
						{
							tx_debug.Text = ex.ToString() + "\r\n";
						}
					}
					if(cam[i] != null && cam[i].IsRunning)
					{
						lbl_alarm.ForeColor = org;
						lbl_alarm.Text = "HAZIR";
						p_scene.BackColor = org;
					}
					if(!cam_var[i])
					{
						lbl_alarm.ForeColor = red;
						lbl_alarm.Text = "Kamera " + (i + 1).ToString() + " Bulunamadı";
						p_scene.BackColor = red;
					}
				}
			}
			catch(Exception ex)
			{
				tx_debug.Text = ex.ToString() + "\r\n";
			}
		}
		private async void Reset()
		{
			try
			{
				qrGelen.Visible = false;
				qrGelen.Text = "";

				string[] rectString = Settings.Default.Rect.Split('/');
				buton = false;
				capturing = true;
				islem = false;

				for(int c = 0; c < cam.Length; c++)
				{
					for(int i = 0; i < rectString.Length; i++)
					{
						hata[c, i] = false;
					}
				}

				visibility(false);

				if(Uart_to_IO.IsOpen)
				{
					Uart_to_IO.Write(new byte[] { 0x00 }, 0, 1);
				}
				else
				{
					Uart_connect();
				}

				if(client.Connected)
				{
					S7.SetBitAt(controlPlc, 0, Settings.Default.plc_lamba, false); //Lamba
					S7.SetBitAt(controlPlc, 0, Settings.Default.plc_bos, false); //Lamba
					S7.SetBitAt(controlPlc, 0, Settings.Default.plc_piston, false); //Piston
					S7.SetBitAt(controlPlc, 0, Settings.Default.plc_kase, false); //kaşe
					result = client.DBWrite(1, 0, 1, controlPlc);

					if(result != 0) port_text.Text = client.ErrorText(result);
				}
				else
				{
					Uart_connect();
				}

				timer.Start();
				await Task.Delay(int.Parse(Settings.Default.Bekleme.Split('-')[10]));
				lbl_alarm.Text = "HAZIR";
				lbl_alarm.ForeColor = org;
				p_scene.BackColor = org;
				caming_id = 0;
			}
			catch(Exception exception)
			{
				tx_debug.Text = exception.ToString();
			}
		}
		private void Uart_connect()
		{
			try
			{
				if(Settings.Default.UartEnable)
				{
					if(!Uart_to_IO.IsOpen)
					{
						string[] ports = SerialPort.GetPortNames();
						int portscount = ports.Length;
						if(portscount > 0)
						{
							port_text.Text = "Çıkış Cihazı Bağlanamadı";
							Uart_to_IO.DataReceived -= Uart_to_IO_DataReceived;
							port_text.ForeColor = red;
							lbl_alarm.Text = "Bağlanıyor...";
							lbl_alarm.ForeColor = org;
							Uart_to_IO.PortName = ports[portscount - 1];
							Uart_to_IO.BaudRate = Settings.Default.baud;
							try
							{
								Uart_to_IO.Open();
							}
							catch(Exception)
							{
								if(!uartResetAlreadyOpened)
								{
									uartResetAlreadyOpened = true;

									// Uygulamayı yönetici olarak yeniden başlatma işlemine geçin
									ProcessStartInfo psi = new ProcessStartInfo();
									psi.FileName = "C:\\UartReset\\UartReset.exe"; // UartReset.exe dosya yolunu doğru şekilde değiştirin
									psi.UseShellExecute = true;
									psi.Verb = "runas"; // Uygulamayı yönetici olarak başlatmak için "runas" kullanılır

									try
									{
										Process.Start(psi);
									}
									catch(Exception ex)
									{
										tx_debug.Text = ex.ToString() + "\r\n";
									}
								}
								else
								{
									string processName = "UartReset";
									Process[] processes2 = Process.GetProcessesByName(processName);
									if(processes2.Length < 1) uartResetAlreadyOpened = false;
								}

								// SerialPort kapanana kadar bekle

								// SerialPort tekrar açıldı, işlemlerinizi burada devam ettirin
							}
							finally
							{
								Uart_to_IO.Write(new byte[] { 0x00 }, 0, 1);
								port_text.Text = "Çıkış Cihazı Bağlandı";
								Uart_to_IO.DataReceived += Uart_to_IO_DataReceived;
								port_text.ForeColor = grn;
							}
							Thread.Sleep(1000); // İstenilen süre boyunca bekleyin (örneğin 100 ms)
						}
						else
						{
							lbl_alarm.Text = "Cihaz bulunamadı";
							lbl_alarm.ForeColor = red;
							ProcessStartInfo psi = new ProcessStartInfo();
							psi.FileName = "C:\\UartReset\\UartReset.exe"; // UartReset.exe dosya yolunu doğru şekilde değiştirin
							psi.UseShellExecute = true;
							psi.Verb = "runas"; // Uygulamayı yönetici olarak başlatmak için "runas" kullanılır
							string processName = "UartReset";
							Process[] processes2 = Process.GetProcessesByName(processName);
							if(processes2.Length < 1)
							{
								try
								{
									Process.Start(psi);
								}
								catch(Exception ex)
								{
									tx_debug.Text = ex.ToString() + "\r\n";
								}
							}
						}
					}
					else
					{
						Uart_to_IO.Write(new byte[] { 0x00 }, 0, 1);
						port_text.Text = "Çıkış Cihazı Bağlandı (UART)";
						port_text.ForeColor = grn;
					}
				}

			}
			catch(Exception ex)
			{
				tx_debug.Text = ex.ToString() + "\r\n";
			}
			if(Settings.Default.PlcEnable)
			{
				result = client.ConnectTo(Settings.Default.PIp, Settings.Default.PRack, Settings.Default.PSlot);
				port_text.Text = result.ToString();
				if(result == 0)
				{
					S7.SetBitAt(controlPlc, 0, Settings.Default.plc_lamba, false); //Lamba
					S7.SetBitAt(controlPlc, 0, Settings.Default.plc_bos, false); //Lamba
					S7.SetBitAt(controlPlc, 0, Settings.Default.plc_piston, false); //Piston
					S7.SetBitAt(controlPlc, 0, Settings.Default.plc_kase, false); //kaşe
					result = client.DBWrite(1, 0, 1, controlPlc);

					if(result != 0) port_text.Text = client.ErrorText(result);
					else
					{
						port_text.Text = "Çıkış Cihazı Bağlandı (ETH)";
						port_text.ForeColor = grn;
					}
				}
				else
				{
					port_text.Text = client.ErrorText(result);
				}
			}


		}
		public void visibility_set(int tab)
		{
			try
			{
				Label_adjust(org);
				string[] rectStringv = Settings.Default.Rect.Split('/');
				for(int i = 0; i < rectStringv.Length; i++)
				{
					if(int.Parse(rectStringv[i].Split(',')[6]) == tab)
					{
						for(int j = 0; j < 4; j++)
						{
							lbl[i, j].Visible = true;
						}
						if(pbox[i].Image != null)
							pbox[i].Visible = true;
					}
					else
					{
						for(int j = 0; j < 4; j++)
						{
							lbl[i, j].Visible = false;
						}
						pbox[i].Visible = false;
					}
				}
			}
			catch(Exception ex)
			{
				tx_debug.Text = ex.ToString() + "\r\n";
			}
		}
		public void visibility(bool vis)
		{
			try
			{
				Label_adjust(org);
				string[] rectStringv = Settings.Default.Rect.Split('/');
				for(int i = 0; i < rectStringv.Length; i++)
				{
					for(int j = 0; j < 4; j++)
					{
						lbl[i, j].Visible = vis;
					}
					pbox[i].Visible = debugMode;
				}
			}
			catch(Exception ex)
			{
				tx_debug.Text = ex.ToString() + "\r\n";
			}
		}
		Bitmap newFrame = null;
		private void New_Frame(object sender, NewFrameEventArgs e)
		{
			try
			{
				if(takePhoto == null)
				{
					if(capture && sender == cam[caming_id])
					{
						// Eski nesneleri serbest bırak
						if(pb_scene.Image != null)
						{
							pb_scene.Image.Dispose();
						}

						if(newFrame != null)
						{
							newFrame.Dispose();
							newFrame = null;
						}

						// Yeni Bitmap nesnesini oluştur
						using(Bitmap tempFrame = (Bitmap)e.Frame.Clone())
						{
							newFrame = new Bitmap(tempFrame);

							if(gThrs)
							{
								// Threshold uygulaması
								Threshold_Apply(newFrame, gthresint, renk);
							}
							else
							{
								pb_scene.Image = (Bitmap)newFrame.Clone(); // Kopyalama yaparak referans sorunlarını önleyin
							}
						}

						capture = false;
					}
				}
				else
				{
					if(gThrs)
					{
						// Threshold uygulaması
						Threshold_Apply(takePhoto, gthresint, renk);
					}
					else
					{
						pb_scene.Image = takePhoto; // Kopyalama yaparak referans sorunlarını önleyin
					}
				}
			}
			catch(Exception ex)
			{
				tx_debug.Text = ex.ToString() + "\r\n";
			}
		}

		private DateTime startTime;
		private void Main_timer_Tick(object sender, EventArgs e)
		{
			try
			{
				if(!rotuzunlukvisible)
				{
					startTime = DateTime.Now;
					RotUzunluk.Visible = false;
					RotUzunluk.Text = "";
				}
				else
				{
					RotUzunluk.Visible = true;
					if((DateTime.Now - startTime).TotalSeconds >= 10)
					{
						rotuzunlukvisible = false;
					}
				}
				txt_qr2.Text = "Reset:" + rst_buton + " - start:" + buton;
				if(rst_buton)
				{
					rst_buton = false;
					Reset();
				}

				if(qrGelen.Visible) { qrGelen.Visible = false; }
				if(buton)
				{
					buton = false;
					capturing = false;
					capture_count = 0;
					islem = true;
				}
				else
				{
					capture = true;
				}

				if(!capturing)
				{
					if(capture_count > 3)
					{
						Islemler();
						alarmtime = 0;
						capturing = true;
					}
					else
					{
						capture = true;
						capture_count++;
					}
				}

				if(lbl_alarm.Text != "HAZIR" && Uart_to_IO.IsOpen)
				{
					lbl_alarm.Text = "HAZIR";
					lbl_alarm.ForeColor = org;
					p_scene.BackColor = org;
				}

				lbl_datetime.Text = DateTime.Now.ToString();
				alarm = false;
				if(Settings.Default.UartEnable)
				{
					if(!Uart_to_IO.IsOpen)
					{
						uartCount++;
						port_text.Text = "Çıkış Cihazı Bağlanamadı (UART)";
						Uart_to_IO.DataReceived -= Uart_to_IO_DataReceived;
						port_text.ForeColor = red;
					}
					else
					{
						uartCount = 0;
					}

					if(uartCount > 15)
					{
						uartCount = 0;
						Uart_connect();
					}
				}
				if(Settings.Default.PlcEnable)
				{
					if(!client.Connected)
					{
						uartCount++;
						port_text.Text = "Çıkış Cihazı Bağlanamadı (ETH)";
						port_text.ForeColor = red;
					}
					else
					{
						uartCount = 0;
						byte[] controlPLC = new byte[1];
						result = client.DBRead(1, 0, 1, controlPLC); buton = S7.GetBitAt(controlPLC, 0, Settings.Default.plc_buton);

					}

					if(uartCount > 15)
					{
						uartCount = 0;
						Uart_connect();
					}
				}

			}
			catch(Exception ex)
			{
				tx_debug.Text = ex.ToString() + "\r\n";
			}
		}

		private async void Islemler()
		{
			DateTime startTime = DateTime.Now;
			bool yon = false, sagMi = false;
			int[] yon_no = new int[2];
			yon_no[0] = 0;
			yon_no[1] = 0;
			for(int t = 0; t < 8; t++) { bitArray[t] = false; }
			try
			{
				lbl_alarm.Text = "İşlem Başladı.";
				Bitmap[] bitmap = new Bitmap[cam.Length];
				bool isTrue = false;
				string[] bekleme = Settings.Default.Bekleme.Split('-');
				rectString = Settings.Default.Rect.Split('/');

				if(Uart_to_IO.IsOpen)
				{
					bitArray[Settings.Default.Lamba] = true;
					bitArray[Settings.Default.Piston] = true;
					bitArray[Settings.Default.Kase] = false;
					bitArray[Settings.Default.bosBit] = false;

					Uart_to_IO.Write(new byte[] { ConvertToByte(bitArray) }, 0, 1);

				}
				if(client.Connected)
				{
					S7.SetBitAt(controlPlc, 0, Settings.Default.plc_lamba, true); //Lamba
					S7.SetBitAt(controlPlc, 0, Settings.Default.plc_bos, false); //Lazer
					S7.SetBitAt(controlPlc, 0, Settings.Default.plc_piston, true); //Piston
					S7.SetBitAt(controlPlc, 0, Settings.Default.plc_kase, false); //kaşe
					result = client.DBWrite(1, 0, 1, controlPlc);

					if(result != 0)
						port_text.Text = client.ErrorText(result);
					//else
					//    return;

				} // Piston-Lamba => 1 ve 2 çıkış

				await Task.Delay(int.Parse(bekleme[0]));

				timer.Stop();
				lbl_alarm.Text = "TARANIYOR";

				for(int c = 0; c < cam.Length; c++)
				{
					caming_id = c;
					int rectbno = 1;

					do
					{
						capture = true;
						await Task.Delay(int.Parse(bekleme[1]));
						try
						{
							bitmap[c] = new Bitmap(pb_scene.Image);
						}
						catch(Exception) { }
					}
					while
					(bitmap[c] == null);
					await Task.Delay(int.Parse(bekleme[2]));

					lbl_islemTime.Text = "Tarama Başladı." + "( " + (DateTime.Now - startTime).TotalMilliseconds + " ms)";
					startTime = DateTime.Now;

					for(int i = 0; i < rectString.Length; i++)
					{
						startTime = DateTime.Now;
						if(caming_id == int.Parse(rectString[i].Split(',')[6]))
						{
							if(rectString[i].Split(',')[10].Contains("yön"))
							{
								yon_no[0] = c;
								yon_no[1] = i;
							}
							if(i > 0)
							{
								if(rectString[i - 1].Split(',')[10].Contains("yön"))
								{
									yon = true;
									sagMi = hata[yon_no[0], yon_no[1]];
									hata[yon_no[0], yon_no[1]] = false;

								}
							}
							int islem_id = int.Parse(rectString[i].Split(',')[7]);
							bmp[i]?.Dispose();
							pbox[i].Image = null;
							bmp[i] = new Bitmap(bitmap[c].Clone(Rect[i], PixelFormat.Format24bppRgb));
							await Task.Run(async () =>
							{
								if(yon)
								{
									if((sagMi && (rectString[i].Split(',')[10].Contains("R-") || rectString[i].Split(',')[10].Contains("islem")))
												|| (!sagMi && (rectString[i].Split(',')[10].Contains("L-") || rectString[i].Split(',')[10].Contains("islem"))))
									{
										if(islem_id == 0)
											Kırmızı_say(bmp[i], i, c);
										else if(islem_id == 1)
											Maks_Genislik(bmp[i], i, c);
										else if(islem_id == 2)
											Siyah_say(bmp[i], i, c);
										else if(islem_id == 3)
											Beyaz_say(bmp[i], i, c);
										else if(islem_id == 4)
											Sol_Konum_Kontrol(bmp[i], i, c);
										else if(islem_id == 5)
											Ust_Konum_Kontrol(bmp[i], i, c);
										else if(islem_id == 6)
											Aci_hesapla_yatay(bmp[i], i, c);
										else if(islem_id == 7)
											Aci_hesapla_dikey(bmp[i], i, c);
										else if(islem_id == 8)
											Audi_aci_hesapla(bmp[i], i, c);
										else if(islem_id == 9)
											Cizgi_Ile_Lokasyon_Bul(bmp[i], i, c);
										else if(islem_id == 10)
											Audi_uzunluk_hesapla(bmp[i], i, c);
										else if(islem_id == 11)
										{
											Audi_alttan_uzat(bmp[i], i, c);
											await Task.Delay(int.Parse(bekleme[3]));
											double dx = Ofset[i, 2];
											double dy = (Ofset[i, 1]) - (Ofset[i, 0]);
											double m = dy / dx;
											double b = Ofset[i, 0] - (m * (Rect[i].Y + Rect[i].Height + dx));
											double y1 = (m * (Rect[i].X - 150)) + b;
											double y2 = (m * (Rect[i].X + Rect[i].Width + 150)) + b;
											using(Graphics g = Graphics.FromImage(bitmap[c]))
											{
												Pen p = new Pen(blk, 200);
												g.DrawLine(p, 0, Rect[i].Y + (int)y1 + 100, bitmap[c].Width, Rect[i].Y + (int)y2 + 100);
											}
											pb_scene.Image = bitmap[c];
										}
										else if(islem_id == 12)
											Iki_Siyah_Arasi_Mesafe_Dik(bmp[i], i, c);
										else if(islem_id == 13)
											R_Iki_Siyah_Arasi_Mesafe_Yatay(bmp[i], i, c);
										else if(islem_id == 14)
											L_Iki_Siyah_Arasi_Mesafe_Yatay(bmp[i], i, c);
										else if(islem_id == 15)
											L_Dikey_Aci_Olcme(bmp[i], i, c);
										else if(islem_id == 16)
											R_Dikey_Aci_Olcme(bmp[i], i, c);
										else if(islem_id == 17)
											Iki_method_arasi_fark(bmp[i], i, c);
										else if(islem_id == 18)
											Qr_Okuyucu(bmp[i], i, c);
										else if(islem_id == 19)
											Btn_bekle(bmp[i], i, c);
										else if(islem_id == 20)
											Qr_Oku_harici(bmp[i], i, c);

									}
									else hata[c, i] = false;
								}
								else
								{
									if(islem_id == 0)
										Kırmızı_say(bmp[i], i, c);
									else if(islem_id == 1)
										Maks_Genislik(bmp[i], i, c);
									else if(islem_id == 2)
										Siyah_say(bmp[i], i, c);
									else if(islem_id == 3)
										Beyaz_say(bmp[i], i, c);
									else if(islem_id == 4)
										Sol_Konum_Kontrol(bmp[i], i, c);
									else if(islem_id == 5)
										Ust_Konum_Kontrol(bmp[i], i, c);
									else if(islem_id == 6)
										Aci_hesapla_yatay(bmp[i], i, c);
									else if(islem_id == 7)
										Aci_hesapla_dikey(bmp[i], i, c);
									else if(islem_id == 8)
										Audi_aci_hesapla(bmp[i], i, c);
									else if(islem_id == 9)
										Cizgi_Ile_Lokasyon_Bul(bmp[i], i, c);
									else if(islem_id == 10)
										Audi_uzunluk_hesapla(bmp[i], i, c);
									else if(islem_id == 11)
									{
										Audi_alttan_uzat(bmp[i], i, c);
										await Task.Delay(int.Parse(bekleme[3]));
										double dx = Ofset[i, 2];
										double dy = (Ofset[i, 1]) - (Ofset[i, 0]);
										double m = dy / dx;
										double b = Ofset[i, 0] - (m * (Rect[i].Y + Rect[i].Height + dx));
										double y1 = (m * (Rect[i].X - 150)) + b;
										double y2 = (m * (Rect[i].X + Rect[i].Width + 150)) + b;
										using(Graphics g = Graphics.FromImage(bitmap[c]))
										{
											Pen p = new Pen(blk, 200);
											g.DrawLine(p, 0, Rect[i].Y + (int)y1 + 100, bitmap[c].Width, Rect[i].Y + (int)y2 + 100);
										}
										pb_scene.Image = bitmap[c];
									}
									else if(islem_id == 12)
										Iki_Siyah_Arasi_Mesafe_Dik(bmp[i], i, c);
									else if(islem_id == 13)
										R_Iki_Siyah_Arasi_Mesafe_Yatay(bmp[i], i, c);
									else if(islem_id == 14)
										L_Iki_Siyah_Arasi_Mesafe_Yatay(bmp[i], i, c);
									else if(islem_id == 15)
										L_Dikey_Aci_Olcme(bmp[i], i, c);
									else if(islem_id == 16)
										R_Dikey_Aci_Olcme(bmp[i], i, c);
									else if(islem_id == 17)
										Iki_method_arasi_fark(bmp[i], i, c);
									else if(islem_id == 18)
										Qr_Okuyucu(bmp[i], i, c);
									else if(islem_id == 19)
										Btn_bekle(bmp[i], i, c);
									else if(islem_id == 20)
										Qr_Oku_harici(bmp[i], i, c);
								}
							});
							pbox[i].Image = bmp[i];
							pbox[i].BringToFront();
							pbox[i].Visible = debugMode;
							string time;
							if((DateTime.Now - startTime).TotalMilliseconds.ToString().Split(',').Length > 1)
								time = (DateTime.Now - startTime).TotalMilliseconds.ToString().Split(',')[0];
							else
								time = (DateTime.Now - startTime).TotalMilliseconds.ToString().Split(',')[0];
							lbl_islemTime.Text = Property.list[islem_id] + "( " + time + " ms)";

							txtReport += "Kamera " + (int.Parse(rectString[i].Split(',')[6]) + 1) + " -" + tarama_count + "," + rectbno + "-" + time + "ms" + "," + hata[c, i] + "," + Ofset[i, 0] + "," + Ofset[i, 1] + "," + Ofset[i, 2] + "/";
							rectbno++;

							startTime = DateTime.Now;
						}
						else { pbox[i].Visible = debugMode; }

					}

					bool reseting = false;
					int hataliKamera = 0;
					for(int r = 0; r < rectString.Length; r++)
					{
						if(hata[c, r])
						{
							hataliKamera = c;
							isTrue = true;
							break;
						}
					}

					if(isTrue)
					{
						lbl_alarm.Text = "";
						using(Graphics g = Graphics.FromImage(bitmap[hataliKamera]))
						{
							Pen pen = new Pen(red);
							for(int i = 0; i < rectString.Length; i++)
							{
								if(hata[hataliKamera, i])
								{
									g.DrawImage(Resources.carpi, (Rect[i].X + (Rect[i].Width - 75) / 2), (Rect[i].Y + (Rect[i].Height - 75) / 2), 75, 75);
									lbl_alarm.Text += rectString[i].Split(',')[10] + " - ";
									lbl_alarm.ForeColor = red;
									reseting = !bool.Parse(rectString[i].Split(',')[9]);

								}
							}
							lbl_alarm.Text += "Hatası";
							pb_scene.Image = bitmap[hataliKamera];
							if(reseting)
							{
								await Task.Delay(int.Parse(bekleme[8]));
								Reset();
							}
							else
							{
								islem = false;
								c = cam.Length;
							}

						}
					}
				}

				await Task.Delay(int.Parse(bekleme[4]));
				lbl_islemTime.Text = "Tarama Bitti." + "( " + (DateTime.Now - startTime).TotalMilliseconds + " ms)";
				startTime = DateTime.Now;

				report += txtReport;
				if(reportold != report)
				{
					long maxFileSizeInBytes = 1000000000;

					if(File.Exists(filePath))
					{
						FileInfo fileInfo = new FileInfo(filePath);
						if(fileInfo.Length > maxFileSizeInBytes)
						{
							filePath = $"{folderPath}/{DateTime.Now.ToString("yyyy_MM_dd")}_2.txt";

							if(!File.Exists(filePath))
							{
								File.Create(filePath).Close();
							}
						}
					}
					else
					{
						File.Create(filePath).Close();
					}

					if(reportold != report)
					{
						reportold = report;
						File.AppendAllText(filePath, txtReport);
						tarama_count++;
						set?.Reporting(reportold);
					}
					txtReport = string.Empty;
				}

				visibility(false);

				if(!isTrue)
				{
					lbl_alarm.Text = "ONAYLANDI";
					lbl_alarm.ForeColor = grn;
					await Task.Delay(int.Parse(bekleme[5]));

					if(Settings.Default.UartEnable && Uart_to_IO.IsOpen)
					{
						bitArray[Settings.Default.Lamba] = true;
						bitArray[Settings.Default.Piston] = true;
						bitArray[Settings.Default.Kase] = true;
						bitArray[Settings.Default.bosBit] = true;
						Uart_to_IO.Write(new byte[] { ConvertToByte(bitArray) }, 0, 1);
					} // Piston-Lamba-Kaşe => 1, 2, 3 ve 4 çıkış
					if(Settings.Default.PlcEnable && client != null)
					{
						if(client.Connected)
						{
							S7.SetBitAt(controlPlc, 0, Settings.Default.plc_lamba, true); //Lamba
							S7.SetBitAt(controlPlc, 0, Settings.Default.plc_bos, true); //Lamba
							S7.SetBitAt(controlPlc, 0, Settings.Default.plc_piston, true); //Piston
							S7.SetBitAt(controlPlc, 0, Settings.Default.plc_kase, true); //kaşe
							result = client.DBWrite(1, 0, 1, controlPlc);

							if(result != 0)
								port_text.Text = client.ErrorText(result);

						}
					}
					await Task.Delay(int.Parse(bekleme[6]));
					if(Settings.Default.UartEnable && Uart_to_IO.IsOpen)
					{
						bitArray[Settings.Default.Lamba] = true;
						bitArray[Settings.Default.Piston] = true;
						bitArray[Settings.Default.Kase] = false;
						bitArray[Settings.Default.bosBit] = false;
						Uart_to_IO.Write(new byte[] { ConvertToByte(bitArray) }, 0, 1);
					}
					if(Settings.Default.PlcEnable && client != null)
					{
						if(client.Connected)
						{
							S7.SetBitAt(controlPlc, 0, Settings.Default.plc_lamba, true); //Lamba
							S7.SetBitAt(controlPlc, 0, Settings.Default.plc_bos, true); //Lamba
							S7.SetBitAt(controlPlc, 0, Settings.Default.plc_piston, true); //Piston
							S7.SetBitAt(controlPlc, 0, Settings.Default.plc_kase, false); //kaşe
							result = client.DBWrite(1, 0, 1, controlPlc);

							if(result != 0)
								port_text.Text = client.ErrorText(result);


						}
					}



					await Task.Delay(int.Parse(bekleme[7]));
					Reset();
				}

			}
			catch(Exception exception)
			{
				tx_debug.Text = exception.ToString();
				txt_qr.Text = "!" + exception.ToString();
				timer.Start();
				Reset();
			}
		}

		// Tarama Fonksiyonları Başlangıç --------------------------------------------------------------------

		//0-
		private void Kırmızı_say(Bitmap image, int r_n, int c_n)
		{
			try
			{
				int width = image.Width;
				int height = image.Height;
				int redPixelCount = 0;

				Bitmap result = new Bitmap(width, height);

				BitmapData sourceData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
				BitmapData resultData = result.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

				int bytesPerPixel = Bitmap.GetPixelFormatSize(PixelFormat.Format24bppRgb) / 8;
				int byteCount = sourceData.Stride * height;

				byte[] sourcePixels = new byte[byteCount];
				byte[] resultPixels = new byte[byteCount];

				Marshal.Copy(sourceData.Scan0, sourcePixels, 0, byteCount);

				for(int y = 0; y < height; y++)
				{
					int sourceOffset = y * sourceData.Stride;
					int resultOffset = y * resultData.Stride;

					for(int x = 0; x < width; x++)
					{
						byte b = sourcePixels[sourceOffset + x * bytesPerPixel];
						byte g = sourcePixels[sourceOffset + x * bytesPerPixel + 1];
						byte r = sourcePixels[sourceOffset + x * bytesPerPixel + 2];

						byte color = (r > ((g + b) / 2) + 25) ? (byte)255 : (byte)0;

						resultPixels[resultOffset + x * bytesPerPixel] = 0;
						resultPixels[resultOffset + x * bytesPerPixel + 1] = 0;
						resultPixels[resultOffset + x * bytesPerPixel + 2] = color;

						if(color == 255)
							redPixelCount++;
					}
				}

				Marshal.Copy(resultPixels, 0, resultData.Scan0, byteCount);

				image.UnlockBits(sourceData);
				result.UnlockBits(resultData);

				Ofset[r_n, 1] = redPixelCount;

				if(Ofset[r_n, 0] <= Ofset[r_n, 1] && Ofset[r_n, 1] <= Ofset[r_n, 2]) hata[c_n, r_n] = false;
				else hata[c_n, r_n] = true;
				bmp[r_n] = result;
			}
			catch(Exception ex)
			{
				tx_debug.Text = ex.ToString() + "\r\n";
			}
		}
		//1-
		private void Maks_Genislik(Bitmap bitmap, int r_n, int c_n)
		{
			try
			{
				if(bitmap == null) { hata[c_n, r_n] = true; return; }
				int thrs = Thrs[r_n];
				int maxBlobWidth = 0;
				for(int y = 0; y < bitmap.Height; y++)
				{
					for(int x = 0; x < bitmap.Width; x++)
					{
						Color c = bitmap.GetPixel(x, y);
						if((c.R + c.G + c.B) / 3 > thrs)
						{
							int blobWidth = 0;

							for(int i = x; i < bitmap.Width; i++)
							{
								bitmap.SetPixel(x, y, grn);
								Color c2 = bitmap.GetPixel(i, y);
								if((c2.R + c2.G + c2.B) / 3 < thrs)
								{
									break;
								}
								blobWidth++;
							}

							if(blobWidth > maxBlobWidth)
							{
								maxBlobWidth = blobWidth;
							}
						}
					}
				}
				Ofset[r_n, 1] = maxBlobWidth;
				if(Ofset[r_n, 0] <= Ofset[r_n, 1] && Ofset[r_n, 1] <= Ofset[r_n, 2]) hata[c_n, r_n] = false;
				else hata[c_n, r_n] = true;
				bmp[r_n] = bitmap;
			}
			catch(Exception exception)
			{
				tx_debug.Text = exception.ToString();
			}
		}
		//2-
		private void Siyah_say(Bitmap bitmap, int r_n, int c_n)
		{
			try
			{
				if(bitmap == null)
				{
					hata[c_n, r_n] = true; Ofset[r_n, 1] = 123456;
					return;
				}
				int width = bitmap.Width;
				int height = bitmap.Height;
				int thrs = Thrs[r_n];
				int siyah = 0;
				for(int i = 0; i < width; i++)
				{
					for(int j = 0; j < height; j++)
					{
						Color c = bitmap.GetPixel(i, j);
						if((c.R + c.G + c.B) / 3 < thrs)
						{
							c = blk;
							siyah++;
						}
						else
						{
							c = org;
						}
						bitmap.SetPixel(i, j, c);
					}
				}
				Ofset[r_n, 1] = siyah;
				if(Ofset[r_n, 0] <= Ofset[r_n, 1] && Ofset[r_n, 1] <= Ofset[r_n, 2]) hata[c_n, r_n] = false;
				else hata[c_n, r_n] = true;
				bmp[r_n] = bitmap;
			}
			catch(Exception exception)
			{
				tx_debug.Text = exception.ToString();
			}
		}
		//3-
		private void Beyaz_say(Bitmap bitmap, int r_n, int c_n)
		{
			try
			{
				if(bitmap == null)
				{
					hata[c_n, r_n] = true; Ofset[r_n, 1] = 123456;
					return;
				}
				int width = bitmap.Width;
				int height = bitmap.Height;
				int thrs = Thrs[r_n];
				int beyaz = 0;
				for(int i = 0; i < width; i++)
				{
					for(int j = 0; j < height; j++)
					{
						Color c = bitmap.GetPixel(i, j);
						if((c.R + c.G + c.B) / 3 < thrs)
						{
							c = blk;
						}
						else
						{
							c = org;
							beyaz++;
						}
						bitmap.SetPixel(i, j, c);
					}
				}
				Ofset[r_n, 1] = beyaz;
				if(Ofset[r_n, 0] <= Ofset[r_n, 1] && Ofset[r_n, 1] <= Ofset[r_n, 2]) hata[c_n, r_n] = false;
				else hata[c_n, r_n] = true;
				bmp[r_n] = bitmap;
			}
			catch(Exception exception)
			{
				tx_debug.Text = exception.ToString();
			}
		}
		//4-
		private void Sol_Konum_Kontrol(Bitmap bitmap, int r_n, int c_n)
		{
			try
			{
				if(bitmap == null)
				{
					hata[c_n, r_n] = true;
					return;
				}
				int width = bitmap.Width;
				int j = bitmap.Height / 2;
				int thrs = Thrs[r_n];
				int sol = 0;
				bool black = false;
				for(int i = 0; i < width; i++)
				{
					Color c = bitmap.GetPixel(i, j);
					if((c.R + c.G + c.B) / 3 < thrs) c = blk;
					else c = wht;
					if(i == 0)
					{
						if(c == blk) black = true;
						else black = false;
					}
					if(black)
					{
						bitmap.SetPixel(i, j, red);
						if(c == blk) sol++;
					}
					else
					{
						bitmap.SetPixel(i, j, org);
						if(c == wht) sol++;
					}
				}

				Ofset[r_n, 1] = sol;
				if(Ofset[r_n, 0] <= Ofset[r_n, 1] && Ofset[r_n, 1] <= Ofset[r_n, 2]) hata[c_n, r_n] = false;
				else hata[c_n, r_n] = true;
				bmp[r_n] = bitmap;
			}
			catch(Exception exception)
			{
				tx_debug.Text = exception.ToString();
			}
		}
		//5-
		private void Ust_Konum_Kontrol(Bitmap bitmap, int r_n, int c_n)
		{
			try
			{
				if(bitmap == null)
				{
					hata[c_n, r_n] = true;
					return;
				}
				int i = bitmap.Width / 2;
				int height = bitmap.Height;
				int thrs = Thrs[r_n];
				int ust = 0;
				bool black = false;
				for(int j = 0; j < height; j++)
				{
					Color c = bitmap.GetPixel(i, j);
					if((c.R + c.G + c.B) / 3 < thrs) c = blk;
					else c = wht;
					if(j == 0)
					{
						if(c == blk) black = true;
						else black = false;
					}
					if(black)
					{
						bitmap.SetPixel(i, j, red);
						if(c == blk) ust++;
					}
					else
					{
						bitmap.SetPixel(i, j, org);
						if(c == wht) ust++;
					}
				}

				Ofset[r_n, 1] = ust;
				if(Ofset[r_n, 0] <= Ofset[r_n, 1] && Ofset[r_n, 1] <= Ofset[r_n, 2]) hata[c_n, r_n] = false;
				else hata[c_n, r_n] = true;
				bmp[r_n] = bitmap;
			}
			catch(Exception exception)
			{
				tx_debug.Text = exception.ToString();
			}
		}
		//6-
		private void Aci_hesapla_yatay(Bitmap bitmap, int r_n, int c_n)
		{
			try
			{
				if(bitmap == null)
				{
					hata[c_n, r_n] = true;
					return;
				}
				int width = bitmap.Width;
				int height = bitmap.Height;
				int thrs = Thrs[r_n];
				int solust = 0, solalt = 0, sagust = 1000, sagalt = 0;
				int i = 10;
				int i2 = width - 10;
				bool kenar = false;
				bool kenar2 = false;

				for(int j = 10; j < height - 10; j++)
				{
					Color c = bitmap.GetPixel(i, j);
					if((c.R + c.G + c.B) / 3 < thrs)
					{
						if(kenar) kenar = false;
						solust = 1000;
					}
					else
					{
						kenar = true;
						bitmap.SetPixel(i, j, red);
						solust++;
					}
					if(!kenar)
					{
						solust = Math.Min(solust, j);
						solalt = Math.Max(solalt, j);
					}
					Color c2 = bitmap.GetPixel(i2, j);
					if((c2.R + c2.G + c2.B) / 3 < thrs)
					{
						if(kenar2) kenar2 = false;
						sagust = 0;
					}
					else
					{
						kenar2 = true;
						bitmap.SetPixel(i2, j, grn);
						sagust++;
					}
					if(!kenar2)
					{
						solust = Math.Min(sagust, j);
						solalt = Math.Max(sagalt, j);
					}
				}
				double a = height - 20;
				double b = Math.Abs((((solalt - solust) / 2) + solust) - (((sagalt - sagust) / 2) + sagust));
				Ofset[r_n, 1] = (int)(Math.Asin(b / Math.Sqrt((a * a) + (b * b))) * 180 / Math.PI);
				if(Ofset[r_n, 0] <= Ofset[r_n, 1] && Ofset[r_n, 1] <= Ofset[r_n, 2]) hata[c_n, r_n] = false;
				else hata[c_n, r_n] = true;
				bmp[r_n] = bitmap;
			}
			catch(Exception exception)
			{
				tx_debug.Text = exception.ToString();
			}
		}
		//7-
		private void Aci_hesapla_dikey(Bitmap bitmap, int r_n, int c_n)
		{
			try
			{
				if(bitmap == null)
				{
					hata[c_n, r_n] = true;
					return;
				}
				int width = bitmap.Width;
				int height = bitmap.Height;
				int thrs = Thrs[r_n];
				int solust = 0, solalt = 0, sagust = 1000, sagalt = 0;
				int i = 10;
				int i2 = height - 10;
				bool kenar = false;
				bool kenar2 = false;
				for(int j = 10; j < width - 10; j++)
				{
					Color c = bitmap.GetPixel(j, i);
					if((c.R + c.G + c.B) / 3 < thrs)
					{
						if(kenar) kenar = false;
						solust = 1000;
					}
					else
					{
						kenar = true;
						bitmap.SetPixel(j, i, red);
						solust++;
					}
					if(!kenar)
					{
						solust = Math.Min(solust, j);
						solalt = Math.Max(solalt, j);
					}
					Color c2 = bitmap.GetPixel(j, i2);
					if((c2.R + c2.G + c2.B) / 3 < thrs)
					{
						if(kenar2) kenar2 = false;
						sagust = 0;
					}
					else
					{
						kenar2 = true;
						bitmap.SetPixel(j, i2, grn);
						sagust++;
					}
					if(!kenar2)
					{
						solust = Math.Min(sagust, j);
						solalt = Math.Max(sagalt, j);
					}
				}
				double a = height - 20;
				double b = Math.Abs((((solalt - solust) / 2) + solust) - (((sagalt - sagust) / 2) + sagust));
				Ofset[r_n, 1] = (int)(Math.Asin(b / Math.Sqrt((a * a) + (b * b))) * 180 / Math.PI);
				if(Ofset[r_n, 0] <= Ofset[r_n, 1] && Ofset[r_n, 1] <= Ofset[r_n, 2]) hata[c_n, r_n] = false;
				else hata[c_n, r_n] = true;
				bmp[r_n] = bitmap;
			}
			catch(Exception exception)
			{
				tx_debug.Text = exception.ToString();
			}
		}
		//8-
		private void Audi_aci_hesapla(Bitmap bitmap, int r_n, int c_n)
		{
			try
			{
				if(bitmap == null)
				{
					hata[c_n, r_n] = true;
					return;
				}
				int aci = 0;
				int yust = bitmap.Height / 3;
				int yalt = yust * 2;
				bool yUst = false;
				bool yAlt = false;
				int ustNokta = 0;
				int altNokta = 0;
				double kenar1 = 0, kenar2 = 0, hip;
				for(int q = 0; q < 2; q++)
				{
					int j;
					if(q == 0) { j = yust; }
					else { j = yalt; }
					for(int i = 0; i < bitmap.Width; i++)
					{
						Color cc = bitmap.GetPixel(i, j);
						if((cc.R + cc.G + cc.B) / 3 < Thrs[r_n]) cc = blk;
						else cc = wht;
						if(cc == wht)
						{
							if(j == yust && !yUst) { ustNokta = i; cc = grn; }
							if(j == yalt && !yAlt) { altNokta = i; cc = grn; }
						}
						else
						{
							if(j == yust) { yUst = true; cc = red; }
							if(j == yalt) { yAlt = true; cc = red; }
						}

						kenar1 = altNokta - ustNokta;
						kenar2 = yalt - yust;
						hip = Math.Sqrt((kenar1 * kenar1) + (kenar2 * kenar2));
						aci = (int)((Math.Asin((kenar1 / hip)) * 180 / Math.PI) * 100);

						bitmap.SetPixel(i, j, cc);
					}
				}
				Ofset[r_n, 1] = Math.Abs(aci);
				if(Ofset[r_n, 0] <= Ofset[r_n, 1] && Ofset[r_n, 1] <= Ofset[r_n, 2]) hata[c_n, r_n] = false;
				else hata[c_n, r_n] = true;
				bmp[r_n] = bitmap;
			}
			catch(Exception exception)
			{
				tx_debug.Text = exception.ToString();
			}
		}
		//9-
		private void Cizgi_Ile_Lokasyon_Bul(Bitmap bitmap, int r_n, int c_n)
		{
			try
			{

				PointF a2 = new PointF(10, 15); // taramadan gelen sol
				PointF b2 = new PointF(10, 15); // taramadan gelen sağ

				bool sol = false, sag = false;
				int x1 = bitmap.Width / 3;
				int x2 = (bitmap.Width / 3) * 2;
				a2.X = x1;
				b2.X = x2;

				for(int q = 0; q < 2; q++)
				{
					int i;
					if(q == 0) i = x1;
					else i = x2;
					for(int j = 0; j < bitmap.Height; j++)
					{
						Color cc = bitmap.GetPixel(i, j);
						if((cc.R + cc.G + cc.B) / 3 < Thrs[r_n]) cc = blk;
						else cc = wht;
						if(cc == wht)
						{
							if(i == x1 && !sol) { a2.Y = j; cc = grn; }
							if(i == x2 && !sag) { b2.Y = j; cc = grn; }
						}
						else
						{
							if(i == x1) { sol = true; cc = red; }
							if(i == x2) { sag = true; cc = red; }
						}
						bitmap.SetPixel(i, j, cc);
					}
				}


				PointF a1 = new PointF(x1, 110);
				PointF b1 = new PointF(x2, 110);

				for(int i = 0; i < Rect.Length; i++)
				{
					if(i != r_n)
					{
						PointF c1 = new PointF(Rect[i].X, Rect[i].Y);
						PointF c2 = TransformPoint(a1, b1, c1, a2, b2);

						if(c2.X < 0) c2.X = 0; if(c2.Y < 0) c2.Y = 0; if(c2.X > pb_scene.Image.Width) c2.X = pb_scene.Image.Width - Rect[i].Width; if(c2.Y > pb_scene.Image.Height) c2.Y = pb_scene.Image.Height - Rect[i].Height;

						Rect[i].X = (int)c2.X; Rect[i].Y = (int)c2.Y;
					}
				}





				bmp[r_n] = bitmap;

			}
			catch(Exception exception)
			{
				tx_debug.Text = exception.ToString();
			}
		}
		//10-
		private void Audi_uzunluk_hesapla(Bitmap bitmap, int r_n, int c_n)
		{
			try
			{
				if(bitmap == null)
				{
					hata[c_n, r_n] = true;
					return;
				}





				int w = bitmap.Width / 2;
				int height = bitmap.Height;
				int leftX = w;
				int rightX = 0;
				int okLeft = 0;
				int nokLeft = 0;
				int okRight = 0;

				int orneklemeSayisi = 15;

				Color[] n = new Color[30];
				int[] np = new int[n.Length];
				bool nb = false;

				int solKenarOrt = 0;
				int sagKenarOrt = bitmap.Height - 10;

				for(int y = 0; y < orneklemeSayisi; y++)
				{
					for(int a = 1; a < w - n.Length; a++)
					{
						int lx = w - a;
						int rx = w + a;
						int right = (bitmap.GetPixel(rx, y).R + bitmap.GetPixel(rx, y).G + bitmap.GetPixel(rx, y).B) / 3;
						int left = (bitmap.GetPixel(lx, y).R + bitmap.GetPixel(lx, y).G + bitmap.GetPixel(lx, y).B) / 3;

						if(left < 127)
						{
							for(int i = 0; i < n.Length; i++)
							{
								n[i] = bitmap.GetPixel((lx - i) - 1, y);
								np[i] = (n[i].R + n[i].G + n[i].B) / 3;
								if(np[i] >= 127) nb = true;
								else { nb = false; break; }
							}
							if(nb)
							{
								bitmap.SetPixel(lx, y, grn);
								leftX = Math.Min(leftX, lx);
							}
						}

						if(right < 127)
						{
							for(int i = 0; i < n.Length; i++)
							{
								n[i] = bitmap.GetPixel((rx + i) + 1, y);
								np[i] = (n[i].R + n[i].G + n[i].B) / 3;
								if(np[i] >= 127) nb = true;
								else { nb = false; break; }
							}
							if(nb)
							{
								bitmap.SetPixel(rx, y, grn);
								rightX = Math.Max(rightX, rx);
							}
						}

					}
				}
				//yukardan aşşa önce beyaz sonra siyahı olandan çentik bul

				for(int y = 0; y < bitmap.Height - 1; y++)
				{
					bitmap.SetPixel(leftX, y, red);
					bitmap.SetPixel(rightX, y, red);
				}
				bool centiks = false;
				int centiksay = 0;
				for(int y = 10; y < bitmap.Height; y++)
				{
					centiksay = 0;
					for(int x = 1; x < 15; x++)
					{
						int thr = (bitmap.GetPixel(leftX + x, y).R + bitmap.GetPixel(leftX + x, y).G + bitmap.GetPixel(leftX + x, y).B) / 3;
						bitmap.SetPixel(leftX + x, y, org);
						if(thr > 127) centiks = true;
						else { centiks = false; break; }
						centiksay++;
					}
					if(centiksay > 13 && centiks)
					{
						solKenarOrt = y;

					}

					for(int x = 1; x < 15; x++)
					{
						int thr = (bitmap.GetPixel(rightX + x, y).R + bitmap.GetPixel(rightX + x, y).G + bitmap.GetPixel(rightX + x, y).B) / 3;
						bitmap.SetPixel(rightX + x, y, org);
						if(thr < 127) centiks = true;
						else { centiks = false; break; }
						centiksay++;
					}
					if(centiksay > 13 && centiks)
					{
						sagKenarOrt = Math.Min(sagKenarOrt, y);
					}
				}

				for(int x = 0; x < bitmap.Width - 1; x++)
				{
					bitmap.SetPixel(x, solKenarOrt, ylw);
					bitmap.SetPixel(x, sagKenarOrt, ylw);
				}
				int hassasiyet = 78;








				int turuncuUst = 116000;

				if(rectString[r_n].Split(',')[10].Split('*').Length > 1) turuncuUst = int.Parse(rectString[r_n].Split(',')[10].Split('*')[1]) * 1000;

				rotuzunlukvisible = true;


				Ofset[r_n, 1] = turuncuUst - ((solKenarOrt - sagKenarOrt) * hassasiyet);
				double ofsettt = (double)Ofset[r_n, 1] / 1000;
				if(RotUzunluk.Text == "")
					RotUzunluk.Text = "SağKol : " + ofsettt.ToString("0.0");
				else
					RotUzunluk.Text = RotUzunluk.Text + " - SolKol : " + ofsettt.ToString("0.0");

				if(Ofset[r_n, 0] <= Ofset[r_n, 1] && Ofset[r_n, 1] <= Ofset[r_n, 2]) hata[c_n, r_n] = false;
				else hata[c_n, r_n] = true;
				bmp[r_n] = bitmap;
			}
			catch(Exception exception)
			{
				tx_debug.Text = exception.ToString();
			}
		}
		//11-
		private void Audi_alttan_uzat(Bitmap bitmap, int r_n, int c_n)
		{
			try
			{
				if(bitmap == null)
				{
					hata[c_n, r_n] = true;
					return;
				}
				int solalt = 0, sagalt = 0;
				int x1 = bitmap.Width / 3;
				int x2 = x1 * 2;
				bool solx1 = false, solx2 = false;
				for(int j = bitmap.Height - 1; j > 0; j--)
				{
					Color c1 = bitmap.GetPixel(x1, j);
					Color c2 = bitmap.GetPixel(x2, j);
					if((c1.R + c1.G + c1.B) / 3 < Thrs[r_n]) c1 = org;
					else c1 = grn;
					if((c2.R + c2.G + c2.B) / 3 < Thrs[r_n]) c2 = org;
					else c2 = grn;
					if(c1 == org && !solx1) { solalt = j; solx1 = true; c1 = red; }

					if(c2 == org && !solx2) { sagalt = j; solx2 = true; c2 = red; }

					bitmap.SetPixel(x1, j, c1);
					bitmap.SetPixel(x2, j, c2);
				}
				Ofset[r_n, 0] = solalt;
				Ofset[r_n, 1] = sagalt;
				Ofset[r_n, 2] = x1;
				bmp[r_n] = bitmap;
			}
			catch(Exception exception)
			{
				tx_debug.Text = exception.ToString();
			}
		}
		//12-
		private void Iki_Siyah_Arasi_Mesafe_Dik(Bitmap bitmap, int r_n, int c_n)
		{
			if(bitmap == null)
			{
				hata[c_n, r_n] = true;
				return;
			}

			int mesafe = 10000;
			int mesafe2 = 0;
			for(int x = 0; x < bitmap.Width; x++)
			{
				mesafe2 = 0;
				for(int y = bitmap.Height - 1; y > 0; y--)
				{
					Color c = bitmap.GetPixel(x, y);

					if((c.R + c.G + c.B) / 3 < Thrs[r_n]) c = grn;
					else c = org;

					if(c == org) { mesafe2++; c = red; }

					bitmap.SetPixel(x, y, c);
				}
				if(mesafe2 < mesafe) mesafe = mesafe2;
			}
			double pxFark = 75;
			double mmFark = 17;
			int okumaOfset = 32;
			double carpan = pxFark / mmFark;
			double realmm = 198;
			double realpxl = realmm * carpan;
			mesafe = mesafe - okumaOfset;
			mesafe = (int)(mesafe / carpan);
			mesafe = (int)realmm - mesafe;
			Ofset[r_n, 1] = mesafe;
			if(Ofset[r_n, 0] <= Ofset[r_n, 1] && Ofset[r_n, 1] <= Ofset[r_n, 2]) hata[c_n, r_n] = false;
			else hata[c_n, r_n] = true;
			bmp[r_n] = bitmap;
		}
		//13-
		private void L_Iki_Siyah_Arasi_Mesafe_Yatay(Bitmap bitmap, int r_n, int c_n)
		{
			if(bitmap == null)
			{
				hata[c_n, r_n] = true;
				return;
			}

			int mesafe = 0;
			bool kenar = false;
			int sagAlt = 0;
			for(int y = 0; y < bitmap.Height; y++)
			{
				Color a = bitmap.GetPixel(bitmap.Width - 1, y);

				if((a.R + a.G + a.B) / 3 < Thrs[r_n] && !kenar) { sagAlt = y; kenar = true; a = red; }
				else a = grn;
				bitmap.SetPixel(bitmap.Width - 1, y, a);
			}
			for(int x = bitmap.Width - 1; x > 0; x--)
			{
				Color c = bitmap.GetPixel(x, sagAlt + 5);

				if((c.R + c.G + c.B) / 3 < Thrs[r_n]) c = red;
				else { mesafe++; c = grn; }

				bitmap.SetPixel(x, sagAlt + 5, c);
			}

			//double pxFark = 43;
			//double mmFark = 17;
			//int okumaOfset = 19;
			//double carpan = pxFark / mmFark;
			//double realmm = 198;
			//double realpxl = realmm * carpan;
			//mesafe = mesafe - okumaOfset;
			//mesafe = (int)(mesafe / carpan);
			//mesafe = (int)realmm - mesafe;

			Ofset[r_n, 1] = mesafe;
			if(Ofset[r_n, 0] <= Ofset[r_n, 1] && Ofset[r_n, 1] <= Ofset[r_n, 2]) hata[c_n, r_n] = false;
			else hata[c_n, r_n] = true;
			bmp[r_n] = bitmap;
		}
		//14-
		private void R_Iki_Siyah_Arasi_Mesafe_Yatay(Bitmap bitmap, int r_n, int c_n)
		{
			if(bitmap == null)
			{
				hata[c_n, r_n] = true;
				return;
			}

			int mesafe = 0;
			bool kenar = false;
			int solAlt = 0;
			for(int y = 0; y < bitmap.Height; y++)
			{
				Color a = bitmap.GetPixel(0, y);

				if((a.R + a.G + a.B) / 3 < Thrs[r_n] && !kenar) { solAlt = y; kenar = true; a = red; }
				else a = grn;
				bitmap.SetPixel(bitmap.Width - 1, y, a);
			}
			for(int x = 0; x < bitmap.Width; x++)
			{
				Color c = bitmap.GetPixel(x, solAlt + 5);

				if((c.R + c.G + c.B) / 3 < Thrs[r_n]) c = red;
				else { mesafe++; c = grn; }

				bitmap.SetPixel(x, solAlt + 5, c);
			}

			//double pxFark = 43;
			//double mmFark = 17;
			//int okumaOfset = 19;
			//double carpan = pxFark / mmFark;
			//double realmm = 198;
			//double realpxl = realmm * carpan;
			//mesafe = mesafe - okumaOfset;
			//mesafe = (int)(mesafe / carpan);
			//mesafe = (int)realmm - mesafe;

			Ofset[r_n, 1] = mesafe;
			if(Ofset[r_n, 0] <= Ofset[r_n, 1] && Ofset[r_n, 1] <= Ofset[r_n, 2]) hata[c_n, r_n] = false;
			else hata[c_n, r_n] = true;
			bmp[r_n] = bitmap;
		}
		//15-
		private void L_Dikey_Aci_Olcme(Bitmap bitmap, int r_n, int c_n)
		{
			bool a = false;
			bool b = false;
			int ust = 0, alt = 0;
			int y1 = bitmap.Height / 4;
			int y2 = (bitmap.Height / 4) * 3;
			for(int x = bitmap.Width - 1; x > 0; x--)
			{
				Color c1 = bitmap.GetPixel(x, y1);
				Color c2 = bitmap.GetPixel(x, y2);

				if((c1.R + c1.G + c1.B) / 3 < Thrs[r_n] && !a) { ust = x; a = true; c1 = red; }
				else c1 = grn;

				if((c2.R + c2.G + c2.B) / 3 < Thrs[r_n] && !b) { alt = x; b = true; c2 = red; }
				else c2 = grn;

				bitmap.SetPixel(x, y1, c1);
				bitmap.SetPixel(x, y2, c2);
			}

			double kenar1 = alt - ust;
			double kenar2 = y1 * 2;
			double hip = Math.Sqrt((kenar2 * kenar2) + (kenar1 * kenar1));
			double angle = Math.Asin(kenar2 / hip) * 180 / Math.PI;

			Ofset[r_n, 1] = (int)angle;
			if(Ofset[r_n, 0] <= Ofset[r_n, 1] && Ofset[r_n, 1] <= Ofset[r_n, 2]) hata[c_n, r_n] = false;
			else hata[c_n, r_n] = true;
			bmp[r_n] = bitmap;
		}
		//16-
		private void R_Dikey_Aci_Olcme(Bitmap bitmap, int r_n, int c_n)
		{
			bool a = false;
			bool b = false;
			int ust = 0, alt = 0;
			int y1 = bitmap.Height / 4;
			int y2 = (bitmap.Height / 4) * 3;
			for(int x = 0; x < bitmap.Width; x++)
			{
				Color c1 = bitmap.GetPixel(x, y1);
				Color c2 = bitmap.GetPixel(x, y2);

				if((c1.R + c1.G + c1.B) / 3 < Thrs[r_n] && !a) { ust = x; a = true; c1 = red; }
				else c1 = grn;

				if((c2.R + c2.G + c2.B) / 3 < Thrs[r_n] && !b) { alt = x; b = true; c2 = red; }
				else c2 = grn;

				bitmap.SetPixel(x, y1, c1);
				bitmap.SetPixel(x, y2, c2);
			}

			double kenar1 = alt - ust;
			double kenar2 = y1 * 2;
			double hip = Math.Sqrt((kenar2 * kenar2) + (kenar1 * kenar1));
			double angle = Math.Asin(kenar2 / hip) * 180 / Math.PI;

			Ofset[r_n, 1] = (int)angle;
			if(Ofset[r_n, 0] <= Ofset[r_n, 1] && Ofset[r_n, 1] <= Ofset[r_n, 2]) hata[c_n, r_n] = false;
			else hata[c_n, r_n] = true;
			bmp[r_n] = bitmap;
		}
		//17-
		private void Iki_method_arasi_fark(Bitmap bitmap, int r_n, int c_n)
		{
			int g1 = bitmap.Width - 1;
			int g2 = bitmap.Height - 1;
			int m1 = Ofset[g1, 1];
			int m2 = Ofset[g2, 1];
			int fark = Math.Abs(m1 - m2);

			Ofset[r_n, 1] = (int)fark;
			if(Ofset[r_n, 0] <= Ofset[r_n, 1] && Ofset[r_n, 1] <= Ofset[r_n, 2]) hata[c_n, r_n] = false;
			else hata[c_n, r_n] = true;
			bmp[r_n] = bitmap;
		}
		//18-
		private void Qr_Okuyucu(Bitmap bitmap, int r_n, int c_n)
		{
			Bitmap bwBitmap = new Bitmap(bitmap.Width, bitmap.Height);

			for(int x = 0; x < bitmap.Width; x++)
			{
				for(int y = 0; y < bitmap.Height; y++)
				{
					Color sourceColor = bitmap.GetPixel(x, y);
					int threshold = Thrs[r_n];
					Color newColor = (sourceColor.R + sourceColor.G + sourceColor.B) / 3 >= threshold ? Color.White : Color.Black;
					bwBitmap.SetPixel(x, y, newColor);
				}
			}

			IBarcodeReader reader = new BarcodeReader();
			var result = reader.Decode(bwBitmap);
			string find_text = rectString[r_n].Split(',')[10];
			if(result != null)
			{
				hata[c_n, r_n] = result.Text.Contains(find_text.Split('-')[1]) ? false : true;
				txt_qr.Text = "QR: " + result.Text + " " + find_text.Split('-')[1];
			}
			else
			{
				txt_qr.Text = "QR: okunamadı " + find_text.Split('-')[1];
				hata[c_n, r_n] = true;
			}

			bmp[r_n] = bwBitmap;
		}
		//19-
		private void Btn_bekle(Bitmap bitmap, int r_n, int c_n)
		{
			buton = false;
			lbl_alarm.Text = "BEKLENİYOR";
			lbl_alarm.ForeColor = org;
			if(client.Connected)
			{
				byte[] controlPLC = new byte[1];
				S7.SetBitAt(controlPLC, 0, Settings.Default.plc_lamba, true); //Lamba
				S7.SetBitAt(controlPLC, 0, Settings.Default.plc_piston, true); //Piston
				S7.SetBitAt(controlPLC, 0, Settings.Default.plc_bos, true); //Lazer
				result = client.DBWrite(1, 0, 1, controlPLC);

				if(result != 0)
					port_text.Text = client.ErrorText(result);
			}
			bool basildi = false;
			for(int i = 0; i < Thrs[r_n] * 10; i++)
			{
				bitmap = null;


				capture = true;
				try
				{
					bitmap = new Bitmap(pb_scene.Image);
				}
				catch(Exception) { }

				if(buton) i = Thrs[r_n] * 10;
				Thread.Sleep(100);

				if(client.Connected)
				{
					byte[] controlPLC = new byte[1];
					result = client.DBRead(1, 0, 1, controlPLC); basildi = S7.GetBitAt(controlPLC, 0, 0);
				}
				if(basildi)
				{
					if(client.Connected)
					{
						byte[] controlPLC = new byte[1];
						S7.SetBitAt(controlPLC, 0, Settings.Default.plc_lamba, true); //Lamba
						S7.SetBitAt(controlPLC, 0, Settings.Default.plc_piston, true); //Piston
						S7.SetBitAt(controlPLC, 0, Settings.Default.plc_bos, false); //Lazer
						result = client.DBWrite(1, 0, 1, controlPLC);

						if(result != 0)
							port_text.Text = client.ErrorText(result);
					}
					buton = true;
				}
			}
		}
		//20-
		private void Qr_Oku_harici(Bitmap bitmap, int r_n, int c_n)
		{
			txt_qr.Text = "";
			lbl_alarm.Text = "BEKLENİYOR...";
			lbl_alarm.ForeColor = org;
			if(client.Connected)
			{
				byte[] controlPLC = new byte[1];
				S7.SetBitAt(controlPLC, 0, Settings.Default.plc_lamba, true); //Lamba
				S7.SetBitAt(controlPLC, 0, Settings.Default.plc_piston, true); //Piston
				S7.SetBitAt(controlPLC, 0, Settings.Default.plc_bos, true); //Lazer
				result = client.DBWrite(1, 0, 1, controlPLC);

				if(result != 0)
					port_text.Text = client.ErrorText(result);
			}

			int qrCounter = 0;
			int qrErCount = 0;
			string gelen = "";
			string find_text = rectString[r_n].Split(',')[10];

			while(qrCounter < (int.Parse(Settings.Default.Bekleme.Split('-')[9]) * 10))
			{
				capture = true;
				gelen = txt_qr.Text;
				if(gelen.Contains(find_text.Split('-')[1])) break;
				qrCounter++;
				if(gelen != "") { qrErCount++; }
				if(qrErCount > 20) { hata[c_n, r_n] = true; break; }
				Thread.Sleep(100);
				Application.DoEvents();

			}
			txt_qr.Text = "";
		}

		// Tarama Fonksiyonları Bitiş ------------------------------------------------------------------------------

		byte ConvertToByte(bool[] bits)
		{
			if(bits.Length != 8)
			{
				// bits dizisini 8 elemanlı yapacak şekilde tamamlayın
				bool[] paddedBits = new bool[8];
				for(int i = 0; i < 8; i++)
				{
					if(i < bits.Length)
					{
						paddedBits[i] = bits[i];
					}
					else
					{
						paddedBits[i] = false;
					}
				}
				bits = paddedBits;
			}

			byte result = 0;

			for(int i = 0; i < 8; i++)
			{
				if(bits[i])
				{
					result |= (byte)(1 << i);
				}
			}

			return result;
		}
		static PointF TransformPoint(PointF a1, PointF b1, PointF c1, PointF a2, PointF b2)
		{
			float angle = (float)Math.Atan2(b2.Y - a2.Y, b2.X - a2.X) - (float)Math.Atan2(b1.Y - a1.Y, b1.X - a1.X);
			float scale = (float)Math.Sqrt(Math.Pow(b2.X - a2.X, 2) + Math.Pow(b2.Y - a2.Y, 2)) / (float)Math.Sqrt(Math.Pow(b1.X - a1.X, 2) + Math.Pow(b1.Y - a1.Y, 2));

			float cosAngle = (float)Math.Cos(angle);
			float sinAngle = (float)Math.Sin(angle);

			float c2X = (c1.X - a1.X) * scale * cosAngle - (c1.Y - a1.Y) * scale * sinAngle + a2.X;
			float c2Y = (c1.X - a1.X) * scale * sinAngle + (c1.Y - a1.Y) * scale * cosAngle + a2.Y;

			return new PointF(c2X, c2Y);
		}
		public void Threshold_Apply(Bitmap orjbitmaps, int threshold, int renk)
		{

			// Orijinal bitmap'i yeni bir bitmap'e kopyala
			Bitmap bitmap = new Bitmap(orjbitmaps);



			// Bitmap'in boyutlarını al
			int width = bitmap.Width;
			int height = bitmap.Height;

			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

			int bytesPerPixel = Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;
			int byteCount = bitmapData.Stride * bitmap.Height;
			byte[] pixels = new byte[byteCount];
			IntPtr ptrFirstPixel = bitmapData.Scan0;
			Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);

			byte thresholdValue = (byte)threshold;

			for(int i = 0; i < pixels.Length; i += bytesPerPixel)
			{
				if(renk == 0)
				{
					byte gray = (byte)((pixels[i] + pixels[i + 1] + pixels[i + 2]) / 3);
					byte color = (gray < thresholdValue) ? (byte)0 : (byte)255;

					pixels[i] = color;
					pixels[i + 1] = color;
					pixels[i + 2] = color;
				}
				else if(renk == 1)
				{
					if(pixels[i + 1] > ((pixels[i] + pixels[i + 2]) / 2) + 25)
					{
						pixels[i] = 0;
						pixels[i + 1] = 255;
						pixels[i + 2] = 0;

					}
					else
					{
						byte gray = (byte)((pixels[i] + pixels[i + 1] + pixels[i + 2]) / 3);
						byte color = (gray < thresholdValue) ? (byte)0 : (byte)255;

						pixels[i] = color;
						pixels[i + 1] = color;
						pixels[i + 2] = color;

					}

				}
				if(renk == 2)
				{
					if(pixels[i + 2] > ((pixels[i + 1] + pixels[i]) / 2) + 25)
					{
						pixels[i] = 0;
						pixels[i + 1] = 0;
						pixels[i + 2] = 255;

					}
					else
					{
						byte gray = (byte)((pixels[i] + pixels[i + 1] + pixels[i + 2]) / 3);
						byte color = (gray < thresholdValue) ? (byte)0 : (byte)255;

						pixels[i] = color;
						pixels[i + 1] = color;
						pixels[i + 2] = color;

					}
				}
			}

			Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
			bitmap.UnlockBits(bitmapData);

			pb_scene.Image = bitmap;
		}

		// Pencere İle İlgili Olaylar Başlangıç ---------------------------------------------------------------------

		bool Moved; int Mouse_X, Mouse_Y;
		private void Move_MouseDown(object sender, MouseEventArgs e) { Mouse_X = e.X; Mouse_Y = e.Y; Moved = true; }
		private void Move_MouseMove(object sender, MouseEventArgs e) { if(Moved) SetDesktopLocation(MousePosition.X - Mouse_X, MousePosition.Y - Mouse_Y); }
		private void Move_MouseUp(object sender, MouseEventArgs e) { Moved = false; }
		private void Close_Click(object sender, EventArgs e)
		{
			Close();
		}

		//-------------------------------------------------------------------------------
		private void MainPage_FormClosing(object sender, FormClosingEventArgs e)
		{
			for(int i = 0; i < cam.Length; i++)
			{
				if(cam[i] != null && cam[i].IsRunning)
				{
					cam[i].SignalToStop();
					cam[i].WaitForStop();
				}
			}
			SaveAllCameraSettingsToXml();
		}
		private void pb_RealTekno_Click(object sender, EventArgs e) { buton = true; }
		private void Reset_setting_Click(object sender, EventArgs e)
		{
			Settings.Default.Reset();
		}
		private void lbl_datetime_Click(object sender, EventArgs e) { caming_id = (caming_id == 0) ? 1 : 0; }
		private void Pb_companyLogo_Click(object sender, EventArgs e)
		{
			try
			{
				visibility(true);
				set = new Property(tab_index);
				set.Owner = this;
				set.FormClosed += (s, args) =>
				{
					visibility(false);
					caming_id = 0;
					set?.Dispose();
				};
				set.Show();

			}
			catch(Exception exception)
			{
				tx_debug.Text = exception.ToString();
				timer.Start();
			}
		}
		private void Tx_debug_TextChanged(object sender, EventArgs e)
		{
			string newValue = tx_debug.Text;
			try
			{
				// debug.txt dosyasına ekleme yapma ve kaydetme işlemi
				using(StreamWriter writer = File.AppendText("debug.txt"))
				{
					writer.WriteLine($"{DateTime.Now} - Hata Kodu: {newValue}");
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine("Hata: " + ex.Message);
			}

		}
		private void Uart_to_IO_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			try
			{
				if(Settings.Default.UartEnable)
				{
					int receiving = Uart_to_IO.BytesToRead;
					string recieveText = Uart_to_IO.ReadExisting();
					int receiveDec = Convert.ToByte(recieveText[0]);




					bool[] boolDizisi = new bool[8];

					for(int i = 0; i < 8; i++)
					{
						boolDizisi[i] = (receiveDec & (1 << i)) != 0; // her biti bool olarak ayır
					}
					Uart_to_IO.DiscardInBuffer();
					Uart_to_IO.DiscardOutBuffer();
					if(boolDizisi[Settings.Default.buton] && !islem)
					{
						buton = true;
					}
					if(boolDizisi[Settings.Default.reset_buton])
					{
						rst_buton = true;
					}
					if(Application.OpenForms.OfType<Property>().Any())
					{
						string de = "";
						for(int i = 0; i < 8; i++)
						{
							if(boolDizisi[i]) de += "1";
							else de += "0";
						}
						if(set != null) set.txt_recData.Text = de;
					}
				}
			}
			catch(Exception ex)
			{
				tx_debug.Text = "DataReceived Error: " + ex.Message;
			}
		}
		private void password_KeyDown(object sender, KeyEventArgs e)
		{
			if(txt_qr.Text.Length > 75) txt_qr.Text = "QR:";
			if(e.KeyCode == Keys.Enter)
			{
				txt_qr.Text = "QR:";
				if(!islem)
				{
					PasswordForm pasw = new PasswordForm
					{
						Owner = this
					};
					pasw.FormClosed += (s, args) =>
					{
						if(pasw.isTrue)
						{
							Reset();
						}
					};
					pasw.Show();
				}

			}
			else
			{
				txt_qr.Text += ((char)e.KeyValue);
			}
		}
		bool qrExtern = false;
		private void qrGelen_Enter(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter)
			{
				qrExtern = true;
				PasswordForm pasw = new PasswordForm
				{
					Owner = this
				};
				pasw.FormClosed += (s, args) =>
				{
					if(pasw.isTrue)
					{
						qrExtern = false;
						Reset();
					}
				};
				pasw.Show();
			}
		}

		//--------------------------------------------------------------------------------
		public void pb_scene_MouseClick(object sender, MouseEventArgs e)
		{
			if(set != null)
			{
				pb_scene.MouseMove -= new MouseEventHandler(pb_scene_MouseMove);
			}
		}
		public void pb_scene_MouseMove(object sender, MouseEventArgs e)
		{
			if(set != null)
			{
				Point loc1 = new Point((int)(e.Location.X * oran[caming_id, 0]), (int)(e.Location.Y * oran[caming_id, 1]));
				set.UpdateLabel(loc1);
				Label_adjust(org);
			}
		}


		// Pencere İle İlgili Olaylar Bitiş --------------------------------------------------------------------------


















		Bitmap takePhoto;
		private void Photo_Click(object sender, EventArgs e)
		{
			if(pb_scene.Image != null)
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog
				{
					Filter = "PNG Dosyaları|*.png",
					Title = "Görseli Kaydet",
					FileName = "image.png"
				};

				if(saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					pb_scene.Image.Save(saveFileDialog.FileName, ImageFormat.Png);
					MessageBox.Show("Görsel başarıyla kaydedildi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
					takePhoto = new Bitmap(saveFileDialog.FileName);
				}
			}
			else
			{
				MessageBox.Show("PictureBox içinde bir görsel bulunmuyor!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void sec_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "PNG Dosyaları|*.png",
				Title = "Bir PNG Dosyası Seç"
			};

			if(openFileDialog.ShowDialog() == DialogResult.OK)
			{
				try
				{
					// Seçilen görseli Bitmap olarak yükleme
					takePhoto = new Bitmap(openFileDialog.FileName);
				}
				catch(Exception ex)
				{
					MessageBox.Show($"Görsel yüklenirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void iptal_Click(object sender, EventArgs e)
		{
			takePhoto = null;
		}





		private void ApplyCameraSettingsToAll()
		{
			DsDevice[] devices = DsDevice.GetDevicesOfCat(DirectShowLib.FilterCategory.VideoInputDevice);
			string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CameraSettings.xml");

			if(!File.Exists(filePath))
			{
				//MessageBox.Show("XML dosyası bulunamadı. Ayarlar yüklenemedi.", "Dosya Eksik", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			var xmlDocument = XDocument.Load(filePath);

			for(int i = 0; i < devices.Length; i++)
			{
				var cameraElement = xmlDocument.Root.Element($"Camera_{i}");
				if(cameraElement == null) continue;

				IBaseFilter videoInputFilter = null;

				try
				{
					IFilterGraph2 graphBuilder = (IFilterGraph2)new FilterGraph();
					graphBuilder.AddSourceFilterForMoniker(devices[i].Mon, null, devices[i].Name, out videoInputFilter);

					var cameraControl = videoInputFilter as IAMCameraControl;
					var videoProcAmp = videoInputFilter as IAMVideoProcAmp;

					// Kamera Kontrol Ayarları
					var cameraControlElement = cameraElement.Element("CameraControlProperties");
					if(cameraControl != null && cameraControlElement != null)
					{
						foreach(var propertyElement in cameraControlElement.Elements())
						{
							if(Enum.TryParse(propertyElement.Name.ToString(), out DirectShowLib.CameraControlProperty property))
							{
								int value = int.Parse(propertyElement.Attribute("Value").Value);
								var flag = (DirectShowLib.CameraControlFlags)Enum.Parse(typeof(DirectShowLib.CameraControlFlags), propertyElement.Attribute("Flags").Value);
								cameraControl.Set(property, value, flag);
							}
						}
					}

					// Video İşlem Ayarları
					var videoProcAmpElement = cameraElement.Element("VideoProcAmpProperties");
					if(videoProcAmp != null && videoProcAmpElement != null)
					{
						foreach(var propertyElement in videoProcAmpElement.Elements())
						{
							if(Enum.TryParse(propertyElement.Name.ToString(), out VideoProcAmpProperty property))
							{
								int value = int.Parse(propertyElement.Attribute("Value").Value);
								var flag = (VideoProcAmpFlags)Enum.Parse(typeof(VideoProcAmpFlags), propertyElement.Attribute("Flags").Value);
								videoProcAmp.Set(property, value, flag);
							}
						}
					}
				}
				catch(Exception ex)
				{
					//MessageBox.Show($"Kamera {i} ayarları yüklenirken hata oluştu: {ex.Message}");
				}
				finally
				{
					if(videoInputFilter != null)
					{
						Marshal.ReleaseComObject(videoInputFilter);
					}
				}
			}

			//MessageBox.Show("Tüm kameraların ayarları XML'den yüklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}



		private void SaveAllCameraSettingsToXml()
		{
			DsDevice[] devices = DsDevice.GetDevicesOfCat(DirectShowLib.FilterCategory.VideoInputDevice);
			var xmlDocument = new XDocument(new XElement("CameraSettings"));

			for(int i = 0; i < devices.Length; i++)
			{
				IBaseFilter videoInputFilter = null;

				try
				{
					IFilterGraph2 graphBuilder = (IFilterGraph2)new FilterGraph();
					graphBuilder.AddSourceFilterForMoniker(devices[i].Mon, null, devices[i].Name, out videoInputFilter);

					var cameraControl = videoInputFilter as IAMCameraControl;
					var videoProcAmp = videoInputFilter as IAMVideoProcAmp;

					var cameraElement = new XElement($"Camera_{i}", new XAttribute("Name", devices[i].Name));

					// Kamera Kontrol Ayarları
					if(cameraControl != null)
					{
						var cameraControlElement = new XElement("CameraControlProperties");
						foreach(DirectShowLib.CameraControlProperty property in Enum.GetValues(typeof(DirectShowLib.CameraControlProperty)))
						{
							int value;
							DirectShowLib.CameraControlFlags flag;
							if(cameraControl.Get(property, out value, out flag) == 0)
							{
								cameraControlElement.Add(new XElement(property.ToString(),
									new XAttribute("Value", value),
									new XAttribute("Flags", flag)));
							}
						}
						cameraElement.Add(cameraControlElement);
					}

					// Video İşlem Ayarları
					if(videoProcAmp != null)
					{
						var videoProcAmpElement = new XElement("VideoProcAmpProperties");
						foreach(VideoProcAmpProperty property in Enum.GetValues(typeof(VideoProcAmpProperty)))
						{
							int value;
							VideoProcAmpFlags flag;
							if(videoProcAmp.Get(property, out value, out flag) == 0)
							{
								videoProcAmpElement.Add(new XElement(property.ToString(),
									new XAttribute("Value", value),
									new XAttribute("Flags", flag)));
							}
						}
						cameraElement.Add(videoProcAmpElement);
					}

					xmlDocument.Root.Add(cameraElement);
				}
				catch(Exception ex)
				{
					//MessageBox.Show($"Kamera {i} ayarları kaydedilirken hata oluştu: {ex.Message}");
				}
				finally
				{
					if(videoInputFilter != null)
					{
						Marshal.ReleaseComObject(videoInputFilter);
					}
				}
			}

			string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CameraSettings.xml");
			xmlDocument.Save(filePath);
			//MessageBox.Show("Tüm kameraların ayarları XML'e kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}













	}

}
