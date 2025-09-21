using Coinpare;
using Cointero;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualBasic.Logging;
using MvCamCtrl.NET;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Xml.Linq;
using static Cointero.Main;

namespace Cointero
{
	public partial class Main : Form
	{
		#region Initial Deffinitions

		// Import the kernel32.dll function to allocate a console
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool AllocConsole();


		//Optional: Import to attach to the parent process console if it exists
		//[DllImport("kernel32.dll")]
		//private static extern bool AttachConsole(int dwProcessId);
		//private const int ATTACH_PARENT_PROCESS = -1;

		FormName name = new FormName();

		string VERSION = "Cointero 0.1";
		string workingDirPath = "D:\\COINTER\\Images\\TrainUnsorted";
		string modelsDirPath = "D:\\COINTER\\Images\\ModelsN";
		SimpleLogger log = new SimpleLogger(true);
		SimpleLogger logData = new SimpleLogger(false);
		Status s = new Status();
		int timerEntryIndex = 0;

		Image<Gray, Byte> imageSide1;
		Image<Gray, Byte> imageSide2;
		Image<Gray, Byte> imageQuarterSide1;
		Image<Gray, Byte> imageQuarterSide2;
		Image<Bgr, Byte> imageTop;
		Image<Rgb, Byte> imageOut;
		Image<Gray, Byte> imageTemp;

		private Coinpare.ImageProc iP4;

		// !!! this should be asign from Settings.SettingItem.NUMO
		// public int NUMofMODELS = Settings.SettingsItems.NOMO;
		const int NUMofMODELS = 2000;
		Image<Gray, Byte>[] imageModel = new Image<Gray, Byte>[NUMofMODELS];
		Image<Gray, Byte>[] imageResModel = new Image<Gray, Byte>[NUMofMODELS];
		// !!! this should be asign from Settings.SettingItem.NUMO

		float[] modelDiemeters = new float[NUMofMODELS];
		int[] modelMag1 = new int[NUMofMODELS];
		int[] modelMag2 = new int[NUMofMODELS];
		int[] modelMag3 = new int[NUMofMODELS];
		string[] modelName = new string[NUMofMODELS];
		string[] modelId = new string[NUMofMODELS];
		bool[] modelDetail = new bool[NUMofMODELS];
		bool[] modelNegative = new bool[NUMofMODELS];
		int[] modelZoom = new int[NUMofMODELS];
		int diameterThresholdLow = 1800;
		int diameterThresholdHigh = 2200;
		bool imageModelsArraySet = false;
		Bitmap bmpImgTmp = null;

		int counterNG = 0;
		int counterOK = 0;
		int HidenHeartBeat = 0;

		public struct Coin
		{
			public string name;
			public string score;
			public string angle;
			public string time;
		}
		Coin coin = new Coin();

		bool imagesReady = false;
		int imagesNotReadyCounter = 0;
		public string[] imageFilePathsEdges;
		public string[] imageFilePathsSide1;
		public string[] imageFilePathsSide2;
		string[] imageFilePathModels;
		string imageFileName;
		int imageIndex = 0;

		float[] Xs;
		float[] Ys;
		float[] Rs;

		#endregion

		#region timers initialisation //??? needed
		private static TimerControl tc = new TimerControl();
		private static TimerControlImage tci = new TimerControlImage();
		private static TimerControlRefresh tcr = new TimerControlRefresh();
		private static TimerControlSimulator tct = new TimerControlSimulator();

		// Public static method to get refTc
		internal static void StartTCI()
		{
			tci.Start();
		}

		internal static TimerControl GetTc()
		{
			return tc;
		}

		internal static void StartFirstTc()
		{
			tc.timer.Interval = 20;
			tc.Start();
			return;
		}

		internal static void StopTc()
		{
			tc.Stop();
			return;
		}

		#endregion

		#region Main Form

		public Main()
		{
			AllocConsole();
			InitializeComponent();
			this.Text = VERSION;
			WriteDebug("Load seetings: ");
			Settings.Load();
			WriteDebug("Init com port: ");
			Console.WriteLine("Init com port: ");
			Debug.WriteLine("Init com port: 1");
			Trace.WriteLine("Init com port: 2");
			string comPortInitialised = PortDataReceived.initIFCCOM();

			coin.name = "ND";
			coin.score = "0";
			coin.time = "0";

			WriteDebug("Init timer handlers: ");
			tc.timer.Interval = 20;
			tci.timer.Interval = 20;
			tcr.timer.Interval = 300;
			tct.timer.Interval = 8000;
			tc.timer.Elapsed += Timer_Elapsed;
			tci.timer.Elapsed += TimerImage_Elapsed;
			tcr.timer.Elapsed += TimerRefresh_Elapsed;
			tct.timer.Elapsed += TimerHeartbit_Elapsed;
			tc.Stop();
			tci.Stop();
			tct.Stop();
			tcr.Start();

			//s.setIdleMode();

			WriteDebug("Get Cameras");
			Thread.Sleep(500);
			CamGrab.getCameras();
			Thread.Sleep(500);

			WriteDebug("Check working directory: " + workingDirPath);
			if (Directory.Exists(workingDirPath))
			{
				string[] directories = Directory.GetDirectories(workingDirPath);
				if (directories.Length >= 3)
				{
					imageFilePathsEdges = Directory.GetFiles(directories[0], "*.bmp", SearchOption.TopDirectoryOnly);
					imageFilePathsSide1 = Directory.GetFiles(directories[1], "*.bmp", SearchOption.TopDirectoryOnly);
					imageFilePathsSide2 = Directory.GetFiles(directories[2], "*.bmp", SearchOption.TopDirectoryOnly);

					if (imageFilePathsEdges.Length == imageFilePathsSide1.Length && imageFilePathsEdges.Length == imageFilePathsSide2.Length)
					{
						// condition has to be met in future
					}
				}
				else
				{
					MessageBox.Show("Loading image filed. Number of subdirectories does not match 3:" + workingDirPath);
					return;
				}
			}
			else
			{
				MessageBox.Show("Loading image filed. Main directory does not exist:" + workingDirPath);
				return;
			}
			WriteDebug("Check working directory: " + modelsDirPath);
			if (Directory.Exists(modelsDirPath))
			{
				imageFilePathModels = Directory.GetFiles(modelsDirPath, "*.bmp", SearchOption.TopDirectoryOnly);
			}
			else
			{
				WriteDebug("Loading model images filed. Main directory does not exist:" + modelsDirPath);
				MessageBox.Show("Loading momdels filed. Number of subdirectories does not match 3:" + workingDirPath);
				return;
			}

			WriteDebug("Prepare Models: " + imageFilePathModels.Length.ToString());
			Xs = new float[imageFilePathModels.Length];
			Ys = new float[imageFilePathModels.Length];
			Rs = new float[imageFilePathModels.Length];

			WriteDebug("Initiate ImageProc: " + imageFilePathModels[0]);
			try
			{
				iP4 = new Coinpare.ImageProc(imageFilePathsSide2[0], imageFilePathsSide2[0], imageFilePathsEdges[0], imageFilePathModels[0]);
				//iP4 = new ImageProc();
			}
			catch (Exception e)
			{
				WriteDebug(e.Message);
			}
			WriteDebug("Loading Models");

			// mmm
			this.ActiveControl = button1;
			LoadModels();

			WriteDebug("simulation of image index " + imageIndex.ToString());
			ShowNextImage(imageIndex);
			ShowFileName();
			DecodeFromFile();
			System.Threading.Thread.Sleep(Settings.SettingsItems.SLPT);

		}

		#endregion Main Form

		#region Timers        

		#region TIMER Image grabbing collection

		public void TimerImage_Elapsed(object? sender, ElapsedEventArgs e)
		{
			tci.Stop();
			if (CamGrab.bmpReadyF && CamGrab.bmpReadyB && CamGrab.bmpReadyT)
			{
				imagesReady = true;
				imagesNotReadyCounter = 0;
				tc.Start();
			}
			else
			{
				imagesReady = false;
				imagesNotReadyCounter++;

				if (imagesNotReadyCounter > 50)
				{
					CamGrab.destroyGrabbers();
					Thread.Sleep(1000);
					CamGrab.getCameras();
					imagesNotReadyCounter = 0;
				}
				else
				{
					tci.Start();
				}
			}

		}
		#endregion

		#region TIMER Image processing

		public void Timer_Elapsed(object? sender, ElapsedEventArgs e)
		{
			tc.Stop();

			bool stop_tc = false;
			string imp_results = "NON000000000N12345678R999";

			if (s.isIdleMode())
			{
				//stop_tc = true;
				if (PortDataReceived.operationMODE == 1)
				{
					WriteDebug("operation mode change from 1 to 2");
					if (!s.isSaveMode()) s.setRunMode();
					PortDataReceived.operationMODE = 2;
				}
			}

			if (s.isSaveMode() && imagesReady)
			{
				WriteDebug("save mode: images being diplayed:");
				imagesDisplay();
				CamGrab.bmpReadyF = false;
				CamGrab.bmpReadyT = false;
				CamGrab.bmpReadyB = false;
				stop_tc = true;
			}
			else if (s.isRunMode() && imagesReady)
			{
				WriteDebug("run mode: collection of images ready to process");
				try
				{
					// mmm 
					imageProcessing();


					imp_results = coin.name.Replace("-", "");
					Main.SendResultsN(imp_results);
					stop_tc = true;
				}
				catch (Exception improcErr)
				{
					Main.SendResultsN("NON000000000N12345678R999");
					WriteDebug(" error: " + improcErr.Message);
					stop_tc = true;
				}
				CamGrab.bmpReadyF = false;
				CamGrab.bmpReadyT = false;
				CamGrab.bmpReadyB = false;
				s.setIdleMode();
			}
			else if (s.isSimulMode())
			{
				timerEntryIndex++;
				WriteDebug("time simul entry index: " + timerEntryIndex);
				if ((imageFilePathsEdges.Length - 1) > imageIndex) imageIndex++;
				else imageIndex = 0;
				WriteDebug("simulation of image index " + imageIndex.ToString());
				// mmm
				// ShowNextImage(imageIndex);
				// ShowFileName();
				// DecodeFromFile();
				System.Threading.Thread.Sleep(Settings.SettingsItems.SLPT);
				timerEntryIndex--;
				WriteDebug("time simul leave index: " + timerEntryIndex);

			}

			if (!stop_tc) tc.Start();

			// wait for results
		}
		#endregion

		#region TIMER Heartbit

		public void TimerHeartbit_Elapsed(object? sender, ElapsedEventArgs e)
		{
			tct.Stop();
			int _status = s.getStatus();
			int _cams = CamGrab.checkCameras();
			if (PortDataReceived.IsCOMopen())
			{
				Console.Write("H" + _status.ToString() + "-" + _cams.ToString());
				if (_cams != 0)
				{
					_cams = 33;
				}
				SendCommand(_status + _cams, 'H');
			}
			else
			{
				Console.Write("HnC" + _status.ToString());
			}
			tct.Start();
		}
		#endregion

		#region TIMER Refresh display
		public void TimerRefresh_Elapsed(object? sender, ElapsedEventArgs e)
		{
			tcr.Stop();
			
			// do form main data refresh

			if (this.button3.InvokeRequired)
			{
				if (PortDataReceived.operationMODE == 2)
					Invoke((System.Windows.Forms.MethodInvoker)(() => this.button3.BackColor = System.Drawing.Color.Yellow));
				else if (PortDataReceived.operationMODE == 1)
					Invoke((System.Windows.Forms.MethodInvoker)(() => this.button3.BackColor = System.Drawing.Color.Green));
				else if (CamGrab.m_bGrabbing)
					Invoke((System.Windows.Forms.MethodInvoker)(() => this.button3.BackColor = System.Drawing.Color.LightBlue));
			}
			else
			{
				if (PortDataReceived.operationMODE == 2)
					button3.BackColor = Color.Yellow;
				else if (PortDataReceived.operationMODE == 1)
					button3.BackColor = Color.Green;
				else if (CamGrab.m_bGrabbing)
					button3.BackColor = Color.LightBlue;

			}

			// refresh camera button colour
			if (this.button2.InvokeRequired)
				if (CamGrab.m_bGrabbing)
					Invoke((System.Windows.Forms.MethodInvoker)(() => this.button2.BackColor = System.Drawing.Color.Green));
				else Invoke((System.Windows.Forms.MethodInvoker)(() => this.button2.BackColor = System.Drawing.Color.Red));
			else
				if (CamGrab.m_bGrabbing)
				this.button2.BackColor = System.Drawing.Color.Green;
			else this.button2.BackColor = System.Drawing.Color.Red;

			// refresh camera button colour
			if (this.button1.InvokeRequired)
			{
				Invoke((System.Windows.Forms.MethodInvoker)(() => this.button1.Text = PortDataReceived.CounterCOMReceived.ToString() + "/" + PortDataReceived.CounterCOMPacket.ToString()));
				if (PortDataReceived.IsCOMopen())
					Invoke((System.Windows.Forms.MethodInvoker)(() => this.button1.BackColor = System.Drawing.Color.Green));
				else Invoke((System.Windows.Forms.MethodInvoker)(() => this.button1.BackColor = System.Drawing.Color.Red));
			}
			else
			{
				this.button1.Text = PortDataReceived.CounterCOMReceived.ToString() + "/" + PortDataReceived.CounterCOMPacket.ToString();
				if (PortDataReceived.IsCOMopen())
					this.button1.BackColor = System.Drawing.Color.Green;
				else this.button1.BackColor = System.Drawing.Color.Red;
			}
			// refresh coin name textbox
			if (this.textBoxCoinName.InvokeRequired)
				Invoke((System.Windows.Forms.MethodInvoker)(() => this.textBoxCoinName.Text = coin.name));
			else
				this.textBoxCoinName.Text = coin.name;

			string separator = " - ";
			HidenHeartBeat++;
			if (HidenHeartBeat > 50) HidenHeartBeat = 0;
			if (HidenHeartBeat > 46) separator = " = ";
			if (this.textBoxCameraCounters.InvokeRequired)
			{
				if (!s.isSimulMode() && !s.isSaveMode()) Invoke((System.Windows.Forms.MethodInvoker)(() => this.textBoxCameraCounters.Text = counterNG.ToString() + " | " +
					CamGrab.IcountF.ToString() + separator + CamGrab.IcountT.ToString() + separator + CamGrab.IcountB.ToString() + " | " + counterOK.ToString()));
				//Invoke((MethodInvoker)(() => this.textBoxCameraCounters.BackColor = System.Drawing.Color.Green));
				else Invoke((System.Windows.Forms.MethodInvoker)(() => this.textBoxCameraCounters.Text = counterNG.ToString() + " | " + imageIndex.ToString() + separator +
					imageIndex.ToString() + separator + imageIndex.ToString() + " | " + counterOK.ToString()));
			}
			else
			{
				if (!s.isSimulMode() && !s.isSaveMode()) this.textBoxCameraCounters.Text = counterNG.ToString() + " | " + CamGrab.IcountF.ToString() + " - " +
					CamGrab.IcountT.ToString() + " - " + CamGrab.IcountB.ToString() + " | " + counterOK.ToString();
				else this.textBoxCameraCounters.Text = counterNG.ToString() + " | " + imageIndex.ToString() + " - " + imageIndex.ToString() + " - " + imageIndex.ToString() + " | " + counterOK.ToString();
				//this.textBoxCameraCounters.BackColor = System.Drawing.Color.Green;
			}



			// mmm - unlock one by one
			/*

            if (s.isRunMode())
                buttonRun.BackColor = Color.LightBlue;
            else
                buttonRun.BackColor = Color.DarkGray;

            if (s.isSaveMode())
                buttonSave.BackColor = Color.LightBlue;
            else
                buttonSave.BackColor = Color.DarkGray;

            // refresh camera button colour
            if (this.btnCam.InvokeRequired)
                if (CamGrab.m_bGrabbing)
                    Invoke((System.Windows.Forms.MethodInvoker)(() => this.btnCam.BackColor = System.Drawing.Color.Green));
                else Invoke((System.Windows.Forms.MethodInvoker)(() => this.btnCam.BackColor = System.Drawing.Color.Red));
            else
                if (CamGrab.m_bGrabbing)
                this.btnCam.BackColor = System.Drawing.Color.Green;
            else this.btnCam.BackColor = System.Drawing.Color.Red;

            //refresh camera counters
            string separator = " - ";
            HidenHeartBeat++;
            if (HidenHeartBeat > 50) HidenHeartBeat = 0;
            if (HidenHeartBeat > 46) separator = " = ";
            if (this.textBoxCameraCounters.InvokeRequired)
            {
                if (!s.isSimulMode() && !s.isSaveMode()) Invoke((System.Windows.Forms.MethodInvoker)(() => this.textBoxCameraCounters.Text = counterNG.ToString() + " | " +
                    CamGrab.IcountF.ToString() + separator + CamGrab.IcountT.ToString() + separator + CamGrab.IcountB.ToString() + " | " + counterOK.ToString()));
                //Invoke((MethodInvoker)(() => this.textBoxCameraCounters.BackColor = System.Drawing.Color.Green));
                else Invoke((System.Windows.Forms.MethodInvoker)(() => this.textBoxCameraCounters.Text = counterNG.ToString() + " | " + imageIndex.ToString() + separator +
                    imageIndex.ToString() + separator + imageIndex.ToString() + " | " + counterOK.ToString()));
            }
            else
            {
                if (!s.isSimulMode() && !s.isSaveMode()) this.textBoxCameraCounters.Text = counterNG.ToString() + " | " + CamGrab.IcountF.ToString() + " - " +
                    CamGrab.IcountT.ToString() + " - " + CamGrab.IcountB.ToString() + " | " + counterOK.ToString();
                else this.textBoxCameraCounters.Text = counterNG.ToString() + " | " + imageIndex.ToString() + " - " + imageIndex.ToString() + " - " + imageIndex.ToString() + " | " + counterOK.ToString();
                //this.textBoxCameraCounters.BackColor = System.Drawing.Color.Green;
            }

            //refresh file name textbox
            if (this.textBoxFileName.InvokeRequired)
            {
                if (s.isRunMode()) Invoke((System.Windows.Forms.MethodInvoker)(() => this.textBoxFileName.Text = name.GetCoinName()));
                else Invoke((System.Windows.Forms.MethodInvoker)(() => this.textBoxFileName.Text = imageFileName));
                //Invoke((MethodInvoker)(() => this.textBoxCameraCounters.BackColor = System.Drawing.Color.Green));
            }
            else
            {
                if (s.isRunMode()) this.textBoxFileName.Text = name.GetCoinName();
                else this.textBoxFileName.Text = imageFileName;
                //this.textBoxCameraCounters.BackColor = System.Drawing.Color.Green;
            }

            //refresh coin name textbox
            if (this.textBoxCoinName.InvokeRequired)
                Invoke((System.Windows.Forms.MethodInvoker)(() => this.textBoxCoinName.Text = coin.name));
            else
                this.textBoxCoinName.Text = coin.name;

            if (this.textBox2.InvokeRequired)
                Invoke((System.Windows.Forms.MethodInvoker)(() => this.textBox2.Text = coin.score));
            else
                this.textBox2.Text = coin.score;

            if (this.textBox3.InvokeRequired)
                Invoke((System.Windows.Forms.MethodInvoker)(() => this.textBox3.Text = coin.time));
            else
                this.textBox3.Text = coin.time;

            */


			tcr.Start();

		}
		#endregion

		#endregion Timers

		#region Load Models and find circles
		private void LoadModels()
		{
			WriteDebug("Start models processing");
			int tmpDbug = Settings.SettingsItems.DBUG;
			bool tmpDebug = ImageProc.DEBUG;
			Settings.SettingsItems.DBUG = 0;
			ImageProc.DEBUG = false;

			int asigned_models_counter = 0;
			int radius_min = 1;
			int radius_max = 1;
			if (imageFilePathModels.Length > 0)
			{
				WriteDebug("models:" + imageFilePathModels.Length.ToString());
				for (int i = 0; i < imageFilePathModels.Length; i++)
				{
					if (File.Exists(imageFilePathModels[i]))
					{
						var splitString = imageFilePathModels[i].Split("_");

						imageModel[i] = new Image<Gray, Byte>(imageFilePathModels[i]);
						// 1.2 read diameter from filename 

						modelDiemeters[i] = (float)(Convert.ToDouble(splitString[3]));
						modelMag1[i] = (int)(Convert.ToInt32(splitString[4]));
						modelMag2[i] = (int)(Convert.ToInt32(splitString[5]));
						modelMag3[i] = (int)(Convert.ToInt32(splitString[6]));
						var modelNameLong = splitString[0].Split("\\");
						modelName[i] = modelNameLong[4] + "-" + splitString[1] + "-" + splitString[2];
						modelId[i] = splitString[2];
						asigned_models_counter++;
						WriteDebug(i.ToString() + " diameter: " + modelDiemeters[i].ToString() + " image : " + imageFilePathModels[i]);

						// Collect information about presence of Detail models
						string mustHaveImagePath = modelsDirPath + "\\" + modelName[i] + "\\Detail\\";
						if (Directory.Exists(mustHaveImagePath))
						{
							string[] mustHaveImageNames = Directory.GetFiles(mustHaveImagePath, "*.bmp", SearchOption.TopDirectoryOnly);
							if (mustHaveImageNames.Length > 0)
							{
								modelDetail[i] = true;
							}
							else
							{
								modelDetail[i] = false;
							}
						}
						// Collect information about presence of Negative models
						string shouldNotHaveImagePath = modelsDirPath + "\\" + modelName[i] + "\\Negative\\";
						if (Directory.Exists(mustHaveImagePath))
						{
							string[] shutNotHaveImageNames = Directory.GetFiles(shouldNotHaveImagePath, "*.bmp", SearchOption.TopDirectoryOnly);
							if (shutNotHaveImageNames.Length > 0)
							{
								modelNegative[i] = true;
							}
							else
							{
								modelNegative[i] = false;
							}
						}
					}
				}
			}
			if (asigned_models_counter == imageFilePathModels.Length)
			{
				imageModelsArraySet = true;
			}
			// 2. Find circle in models
			// Debug.WriteLine("DecodeFromFiles: start Find circle on model");
			// log.Debug("DecodeFromFiles: start Find circle on model");
			WriteDebug("start Find circle on model");
			for (int i = 0; i < imageFilePathModels.Length; i++)
			{
				/*
                if (modelDiemeters[i] > diameterThresholdHigh) ImageProc.RESIZEF = 4;
                else if (modelDiemeters[i] > diameterThresholdLow) ImageProc.RESIZEF = 3;
                else ImageProc.RESIZEF = 3;
                modelZoom[i] = ImageProc.RESIZEF;
                */

				iP4.mCpyImage2Model(imageModel[i]);
				radius_min = (int)(((modelDiemeters[i] - Settings.SettingsItems.RADT) * 148 / 1000) / 2);
				radius_max = (int)(((modelDiemeters[i] + 4 + Settings.SettingsItems.RADT) * 148 / 1000) / 2);
				// MM : read it !
				// Models can be cut to coin sie to reduce the speed by skipping Circle Hough
				int result = iP4.mCircleHough(0, radius_min, radius_max, out float X, out float Y, out float R);
				Xs[i] = X;
				Ys[i] = Y;
				Rs[i] = R;
				// MM !!! confimr this
				Rs[i] = modelDiemeters[i] * 148 / 1000 / 2;
				// MM !!!

				int XX = 45;
				int YY = 45;
				int RR = (int)R;

				iP4.mCropModelImage(imageModel[i], X, Y, R);
				imageResModel[i] = new Image<Gray, Byte>(iP4.CropModelImage);
				iP4.mRotModelImages(imageResModel[i], i);


				Console.Write(".");
				// 2.2 create Detail model out of model image
				// this seciton has to be conditional, after detail model creation should not run to overright created model
				if (false)
				{
					string mustHaveName = modelName[i].Replace("-", "_");
					string mustHaveImagePath = modelsDirPath + "\\" + modelName[i] + "\\Detail\\" + mustHaveName
						+ "_1656_0948_" + XX.ToString("D4") + "_" + YY.ToString("D4") + "_" + XX.ToString("D4") + "_" + YY.ToString("D4") + "_XXXX_0000_F000.bmp";
					iP4.mDetailModelImage(imageModel[i], X, Y, RR, mustHaveImagePath);
				}

			}
			Settings.SettingsItems.DBUG = tmpDbug;
			ImageProc.DEBUG = tmpDebug;
		}
		#endregion

		#region image displaying
		public void imagesDisplay()
		{
			WriteDebug("show images");
			try
			{
				// if all images are acquired from camera -> save them
				if (CamGrab.m_Bitmap2ShowB != null && CamGrab.m_Bitmap2ShowF != null && CamGrab.m_Bitmap2ShowT != null)
				{					
					WriteDebug("before image saving");
					if (s.isSaveMode())
					{
						saveImages();
					}

					pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
					pictureBox2.Image = CamGrab.m_Bitmap2ShowB;

					pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
					pictureBox1.Image = CamGrab.m_Bitmap2ShowF;

					pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
					pictureBox3.Image = CamGrab.m_Bitmap2ShowT;

				}
				Main.SendResultsN("NON000000000N12345678R999");

			}
			catch (Exception imgdispErr)
			{
				WriteDebug("Image Saving: " + imgdispErr.Message);
			}

		}
		#endregion

		#region images saving		
        public void saveImages()
        {
            // build coin name            
            StringBuilder pathname_builder = new StringBuilder();
            pathname_builder.Append(FormName.MODELSDIRECTORY);
            pathname_builder.Replace("Images\\ModelsN", "SavedImages");
            pathname_builder.Append(name.GetCoinName());

            string[] path_nameParts = pathname_builder.ToString().Split(new string[] { "_" }, StringSplitOptions.None);

            string path_name = pathname_builder.ToString();

            if (name.autoFillUp)
            {
                CultureInfo ci = CultureInfo.CurrentCulture;
                path_nameParts[3] = PortDataReceived.parSA[3].ToString("D4", ci);
                path_nameParts[4] = PortDataReceived.parSA[0].ToString("D4", ci);
                path_nameParts[5] = PortDataReceived.parSA[1].ToString("D4", ci);
                path_nameParts[6] = PortDataReceived.parSA[2].ToString("D4", ci);
                path_name = string.Join("_", path_nameParts);
            }

            Debug.WriteLine("Image name to saved to " + path_name);
            log.Debug("File name to save images:" + path_name);
            string filenameF = path_name + "_F" + CamGrab.IcountF.ToString() + ".bmp";
            string filenameB = path_name + "_B" + CamGrab.IcountB.ToString() + ".bmp";
            string filenameT = path_name + "_T" + CamGrab.IcountT.ToString() + ".bmp";

            // create temporary model 


            // check temporary model


            // save images
            if (CamGrab.IcountF.ToString() == CamGrab.IcountB.ToString() && CamGrab.IcountB.ToString() == CamGrab.IcountT.ToString())
            {
                try
                {
                    CamGrab.m_Bitmap2ShowF.Save(filenameF, ImageFormat.Bmp);
                    CamGrab.m_Bitmap2ShowB.Save(filenameB, ImageFormat.Bmp);
                    CamGrab.m_Bitmap2ShowT.Save(filenameT, ImageFormat.Bmp);

                    path_nameParts[0].Substring(path_nameParts[0].Length - 3, path_nameParts[0].Length);
                    string coin_name = string.Join("_", path_nameParts);
                    name.SetCoinName(coin_name);
                }
                catch (Exception saveErr)
                {
                    Debug.WriteLine("Image Saving: " + saveErr.Message);
                    log.Debug("Image Saving: " + saveErr.Message);
                }
            }
            else
            {
                Debug.WriteLine("Image Saving: Image counters are not matching");
                log.Debug("Image Saving: Image counters are not matching");
            }
        }
        
		#endregion

		#region image processing
		public void imageProcessing()
		{

			WriteDebug("start");
			if (CamGrab.m_Bitmap2ShowB != null && CamGrab.m_Bitmap2ShowF != null && CamGrab.m_Bitmap2ShowT != null)
			{
				try
				{
					pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
					pictureBox2.Image = CamGrab.m_Bitmap2ShowB;

					pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
					pictureBox1.Image = CamGrab.m_Bitmap2ShowF;

					pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
					pictureBox3.Image = CamGrab.m_Bitmap2ShowT;

					WriteDebug("call DecodeFromCamera");
					DecodeFromCamera();
					WriteDebug("return from DecodeFromCamera");
				}
				catch (Exception eShow)
				{
					WriteDebug(" m_Bitmap show. " + eShow.Message);
				}
				finally
				{
					WriteDebug(" returning from imageProcessing");
				}
			}
			else
			{
				WriteDebug("missing CamGrab image/s for DecodeFromCamera");
			}
			// run image processing side 1 in thread 1
			// run image processing side 2 in thread 2
		}
		#endregion

		#region Image Decoding

		private void Decode(float dTolerance, float Rs1, float Xs1, float Xs2, float Ys1, float Ys2, int liveMag1, int liveMag2, int liveMag3,
		out string modelNameOK, out int scoreOK, out int angleOK, out int toleranceOut_max, out int non0_score_elements)
		{
			modelNameOK = "ND";
			scoreOK = -1;
			angleOK = -1;
			non0_score_elements = 0;


			float[] score_array1 = new float[imageFilePathModels.Length];
			float[] angle_array1 = new float[imageFilePathModels.Length];
			Point[] location_array1 = new Point[imageFilePathModels.Length];
			float[] score_array2 = new float[imageFilePathModels.Length];
			float[] angle_array2 = new float[imageFilePathModels.Length];
			Point[] location_array2 = new Point[imageFilePathModels.Length];
			float[] score_detail = new float[imageFilePathModels.Length];
			float[] score_negative = new float[imageFilePathModels.Length];

			float mag1_tol = Settings.SettingsItems.MG1T * dTolerance;
			float mag2_tol = Settings.SettingsItems.MG2T * dTolerance;
			float mag3_tol = Settings.SettingsItems.MG3T * dTolerance;
			float radius_tolerance = (((Settings.SettingsItems.RADT * 148 / 1000) / 2) * dTolerance);
			int[] toleranceOut = new int[imageFilePathModels.Length];
			toleranceOut_max = 0;
			double angle_found = 400;
			List<int> listOfPreSelected = new List<int>();
			int index_max = -1;

			for (int i = 0; i < Rs.Length; i++)
			{
				toleranceOut[i] = 0;

				if (i == -1)
				{
					ImageProc.DEBUG = true;
				}

				if (PortDataReceived.largeRadius)
				{
					// if large radius is set, then involve all models with larger radius
					if (Rs[i] + radius_tolerance > Rs1)
					{
						toleranceOut[i] += 1000;
					}
					/*if (((Rs[i] - (radius_tolerance * 3)) < Rs1) && (Rs1 < (Rs[i] + (radius_tolerance*3))))
                        toleranceOut[i] += 1000;
                    */
					//WriteDebug("large-coin detected" + radius_tolerance*3);
				}
				else
				{
					// if large radius is not set, then involve only models with radius in tolerance
					if ((Rs[i] - radius_tolerance < Rs1) && (Rs1 < Rs[i] + radius_tolerance))
						toleranceOut[i] += 1000;
				}

				if ((liveMag1 < (modelMag1[i] + mag1_tol)) && (liveMag1 > (modelMag1[i] - mag1_tol)))
					toleranceOut[i] += 1;
				if ((liveMag2 < (modelMag2[i] + mag2_tol)) && (liveMag2 > (modelMag2[i] - mag2_tol)))
					toleranceOut[i] += 10;
				if ((liveMag3 < (modelMag3[i] + mag3_tol)) && (liveMag3 > (modelMag3[i] - mag3_tol)))
					toleranceOut[i] += 100;


				//WriteDebug("Radius in " + (int)Rs1*2*1000/148 + "min " + (int)(Rs[i] - radius_tolerance) * 2 * 1000 / 148 + "max " + (int)(Rs[i] + radius_tolerance) * 2 * 1000 / 148);
				// if diameter and magnetic params are in tolerance
				// go to best model image search

				if (toleranceOut[i] == 1111)
				{
					listOfPreSelected.Add(i);
					// check side 1 matching models
					// int res_cutRot = iP4.mCutRotateImage(imageResModel[i], 1, Rs[i], Xs[i], Ys[i], out float score1, out float angle1, out Point location1);

					int res_cutRot = iP4.mCompareImage(imageResModel[i], i, 1, Rs[i], Xs[i], Ys[i], out float score1, out float angle1, out Point location1);
					score_array1[i] = score1;
					angle_array1[i] = angle1;
					location_array1[i] = location1;

					// check side 2 matching models
					// res_cutRot = iP4.mCutRotateImage(imageResModel[i], 2, Rs[i], Xs[i], Ys[i], out float score2, out float angle2, out Point location2);
					res_cutRot = iP4.mCompareImage(imageResModel[i], i, 2, Rs[i], Xs[i], Ys[i], out float score2, out float angle2, out Point location2);
					score_array2[i] = score2;
					angle_array2[i] = angle2;
					location_array2[i] = location2;

					WriteDebug("ISO: " + modelName[i] + " " + (int)(Rs1) * 2 * 1000 / 148 + " " + (int)(Rs[i]) * 2 * 1000 / 148 + " itt" + i.ToString() + " at Angle: " + angle1.ToString() + " Score: " + score1.ToString());
					WriteDebug("ISO: " + modelName[i] + " " + (int)(Rs1) * 2 * 1000 / 148 + " " + (int)(Rs[i]) * 2 * 1000 / 148 + " itt" + i.ToString() + " at Angle2: " + angle2.ToString() + " Score2: " + score2.ToString());

					Point location_detail = location_array1[i];
					Point location_neg = location_array1[i];

					float score_pattern = 0;
					float score_pattern_sum = 0;
					float score_pattern_ave = 0;
					float score_neg = 0;
					float score_neg_sum = 0;
					float score_neg_ave = 0;
					int selectedside;
					if (score1 > score2)
					{
						selectedside = 1;
						angle_found = angle1;
						location_detail.X = (int)Xs1;
						location_detail.Y = (int)Ys1;
						location_neg.X = (int)Xs1;
						location_neg.Y = (int)Ys1;
					}
					else
					{
						selectedside = 2;
						angle_found = angle2;
						location_detail.X = (int)Xs2;
						location_detail.Y = (int)Ys2;
						location_neg.X = (int)Xs2;
						location_neg.Y = (int)Ys2;
					}

					// check side if detail model exist and match it
					if (modelDetail[i])
					{
						string mustHaveImagePath = modelsDirPath + "\\" + modelName[i] + "\\Detail\\";
						if (Directory.Exists(mustHaveImagePath))
						{
							string[] mustHaveImageNames = Directory.GetFiles(mustHaveImagePath);
							if (mustHaveImageNames.Length > 0)
							{
								int loopIndex = mustHaveImageNames.Length;
								for (int j = 0; j < loopIndex; j++)
								{
									//int result_p = iP4.mCircleHough(2, radius_min, radius_max, out float Xsp, out float Ysp, out float Rsp);
									string[] mustHaveImageSplit = mustHaveImageNames[j].Split("_");
									int mustHaveImageOffsetX = (int)(Convert.ToUInt32(mustHaveImageSplit[6]));
									int mustHaveImageOffsetY = (int)(Convert.ToUInt32(mustHaveImageSplit[7]));
									score_pattern = iP4.mFindSmallPattern(mustHaveImageNames[j], selectedside, location_detail, Rs1, angle_found, mustHaveImageOffsetX, mustHaveImageOffsetY);
									score_pattern_sum = score_pattern_sum + score_pattern;
									WriteDebug("ISO: " + modelName[i] + " Detail" + j.ToString() + ": " + score_pattern.ToString() + " at ["
										+ mustHaveImageOffsetX.ToString() + "," + mustHaveImageOffsetY.ToString() + "]");
								}
								score_pattern_ave = score_pattern_sum / loopIndex;
								score_detail[i] = score_pattern_ave;

							}
							else score_detail[i] = 0;
						}
						else score_detail[i] = 0;
					}
					else score_detail[i] = 0;

					// check side if detail model exist and match it
					if (modelNegative[i])
					{
						string shuldNotHaveImagePath = modelsDirPath + "\\" + modelName[i] + "\\Negative\\";
						if (Directory.Exists(shuldNotHaveImagePath))
						{
							string[] shuldNotHaveImageNames = Directory.GetFiles(shuldNotHaveImagePath);
							if (shuldNotHaveImageNames.Length > 0)
							{
								int loopIndex = shuldNotHaveImageNames.Length;
								for (int j = 0; j < loopIndex; j++)
								{
									//int result_p = iP4.mCircleHough(2, radius_min, radius_max, out float Xsp, out float Ysp, out float Rsp);
									string[] shuldNotHaveImageSplit = shuldNotHaveImageNames[j].Split("_");
									int imageOffsetX = (int)(Convert.ToUInt32(shuldNotHaveImageSplit[6]));
									int imageOffsetY = (int)(Convert.ToUInt32(shuldNotHaveImageSplit[7]));
									score_neg = iP4.mFindSmallPattern(shuldNotHaveImageNames[j], selectedside, location_neg, Rs1, angle_found, imageOffsetX, imageOffsetY);
									score_neg_sum = score_neg_sum + score_neg;
								}
								score_neg_ave = score_neg_sum / loopIndex;
								score_negative[i] = score_neg_ave;
							}
							else score_negative[i] = 1;
						}
						else score_negative[i] = 1;
					}
					else score_negative[i] = 1;

					// Compare results for both sides of the coin
					//
					// 1. improve or reduce score based on Detail and Negative                   
					if (selectedside == 1 && score_detail[i] > 45)
					{
						//score_array1[i] = (score_array1[i] + (2 * score_detail[i] * (1 - score_array1[i]) - (1 - score_array1[i])));
						//score_array1[i] = score_array1[i] + score_detail[i];
						score_array1[i] = ((score_array1[i] + score_detail[i]) / 2);
						//if (scoreOK > 0) scoreOK = (int)(score_max1 * 100);
					}
					else if (selectedside == 2 && score_detail[i] > 45)
					{
						//score_array2[i] = (score_array2[i] + (2 * score_detail[i] * (1 - score_array2[i]) - (1 - score_array2[i])));
						score_array2[i] = ((score_array2[i] + score_detail[i]) / 2);
						//score_max = (score_max2 + score_detail[i]) / 2;                        
					}

					// 2. find maximun of side 1
					float score_max1 = (float)score_array1.Max();
					int index_max1 = Array.IndexOf(score_array1, score_max1);
					Point location_max1 = location_array1[index_max1];
					location_max1.X = (int)(Xs1);
					location_max1.Y = (int)(Ys1);
					double angle_found1 = angle_array1[index_max1];

					// 3. find maximun of side 2
					float score_max2 = (float)score_array2.Max();
					int index_max2 = Array.IndexOf(score_array2, score_max2);
					Point location_max2 = location_array2[index_max2];
					location_max2.X = (int)(Xs2);
					location_max2.Y = (int)(Ys2);
					double angle_found2 = angle_array2[index_max2];

					// 4 assigne maximal values to results
					float score_max;

					Point location_max = new Point(0, 0);
					angle_found = 400;


					int thresholdMinAccept = Settings.SettingsItems.TRMI;
					if (dTolerance > 1) { thresholdMinAccept = Settings.SettingsItems.TRNO; }

					if ((score_max2 * 100 < thresholdMinAccept) && (score_max1 * 100 < thresholdMinAccept))
					{
						modelNameOK = "ND";
						scoreOK = 0;
					}
					else if (score_max2 > score_max1)
					{
						selectedside = 2;
						modelNameOK = modelName[index_max2];
						scoreOK = (int)(score_max2 * 100);
						score_max = score_max2;
						location_max = location_max2;
						angle_found = angle_found2;
						index_max = index_max2;
						toleranceOut_max = toleranceOut[index_max2];
						angleOK = (int)angle_found;
					}
					else
					{
						selectedside = 1;
						modelNameOK = modelName[index_max1];
						scoreOK = (int)(score_max1 * 100);
						score_max = score_max1;
						location_max = location_max1;
						angle_found = angle_found1;
						index_max = index_max1;
						toleranceOut_max = toleranceOut[index_max1];
						angleOK = (int)angle_found;
					}
				}

				else
				{
					score_array1[i] = 0;
					angle_array1[i] = 0;
					location_array1[i] = new Point(0, 0);

					score_array2[i] = 0;
					angle_array2[i] = 0;
					location_array2[i] = new Point(0, 0);

					score_detail[i] = 0;
				}

			}
			if (index_max >= 0)
				WriteDebug("index of max score: " + index_max.ToString() + " from: " + listOfPreSelected.ToString());
			non0_score_elements = score_array1.Count(C => C != 0);
			PortDataReceived.largeRadius = false;
		}

		private void DecodeFromFile()
		{
			string imageSide1Path = this.imageFilePathsSide1[imageIndex];
			string imageSide2Path = this.imageFilePathsSide2[imageIndex];
			string imageEdgesPath = this.imageFilePathsEdges[imageIndex];

			WriteDebug("start");
			DateTime start = DateTime.Now;
			// Initialise image proessing
			iP4 = new ImageProc(imageSide1Path, imageSide2Path, imageEdgesPath, imageFilePathModels[0]);

			// MODELS -------------------------
			// 1. Load models if not yet loaded
			// 2. Find circle in model and
			// 3. generate all rotations of the model
			if (!imageModelsArraySet)
			{
				LoadModels();
			}

			// THICKNESS check top edge coin thickness and colour
			try
			{
				iP4.mFitLine();
			}
			catch (Exception eFitline1)
			{
				WriteDebug(eFitline1.Message);
			}

			WriteDebug("start Find circle on model");

			int radius_min = 1;
			int radius_max = 1;


			// Get diameter parameter of side1 from file name 
			string coinIDfromFile = "";
			int side1Diemeters = 0;
			int side2Diemeters = 0;
			int liveMag1 = 0;
			int liveMag2 = 0;
			int liveMag3 = 0;
			if (imageFilePathsSide1.Length > 0 && imageFilePathsSide2.Length > 0 && imageFilePathsEdges.Length > 0)
			{
				if (File.Exists(imageFilePathsSide1[imageIndex]) && File.Exists(imageFilePathsSide2[imageIndex]) && File.Exists(imageFilePathsEdges[imageIndex]))
				{
					// 1.2 read diameter from filename 
					var splitString1 = imageFilePathsSide1[imageIndex].Split("_");
					try
					{
						if (splitString1.Length > 6)
						{
							coinIDfromFile = splitString1[2];
							side1Diemeters = (int)(Convert.ToDouble(splitString1[3]));
							liveMag1 = (int)(Convert.ToDouble(splitString1[4]));
							liveMag2 = (int)(Convert.ToDouble(splitString1[5]));
							liveMag3 = (int)(Convert.ToDouble(splitString1[6]));
						}
						var splitString2 = imageFilePathsSide2[imageIndex].Split("_");
						if (splitString1.Length > 6)
						{
							side2Diemeters = (int)(Convert.ToDouble(splitString2[3]));
						}
					}
					catch (Exception ex)
					{
						WriteDebug("decoding filename failed: " + ex.Message);
					}
				}
			}

			// Get actual coin Side1, find circle  
			radius_min = (int)(((side1Diemeters - Settings.SettingsItems.RADT) * 148 / 1000) / 2);

			int MULTIPLE_FOR_LARGE_COINS = 4;
			if (PortDataReceived.largeRadius)
				radius_max = (int)(((side1Diemeters + Settings.SettingsItems.RADT * MULTIPLE_FOR_LARGE_COINS) * 148 / 1000) / 2);
			else
				radius_max = (int)(((side1Diemeters + 4 + Settings.SettingsItems.RADT) * 148 / 1000) / 2);

			ImageProc._image_models_array_set = imageModelsArraySet;
			int result_s1 = iP4.mCircleHough(1, radius_min, radius_max, out float Xs1, out float Ys1, out float Rs1);
			int result_s2 = iP4.mCircleHough(2, radius_min, radius_max, out float Xs2, out float Ys2, out float Rs2);

			// 3. Prepare rotated arrays

			// !!! MMM
			// replacing camera diameter with sensor diameter (when from camera)
			// replacing camera diameter with file name diameter (when from files)
			float Rs1_disp = Rs1;
			Rs1 = ((side1Diemeters * 148 / 1000) / 2);
			Rs2 = ((side1Diemeters * 148 / 1000) / 2);
			float radius_camera = Rs1 * 2 * 1000 / 148;

			// prepare quarter images for coin's side 1 and 2 
			int result_qs1 = iP4.mQuarterImage(1, Rs1, Xs1, Ys1);
			int result_qs2 = iP4.mQuarterImage(2, Rs2, Xs2, Ys2);

			// Stop for debugging MM !!!
			if (imageIndex == 90)
			{
				int nop = 0;
			}
			// Stop for debugging MM !!!

			// DECODE
			// Find rotation angle1
			// Check actual image rorated by the angle1 
			// and generate the correlation (score)
			Decode(1, Rs1, Xs1, Xs2, Ys1, Ys2, liveMag1, liveMag2, liveMag3,
				out string modelNameOK, out int scoreOK, out int angleOK, out int toleranceOut_max, out int non0_score_elements);

			if (scoreOK < Settings.SettingsItems.TRMI)
			{
				//int tmpResize = ImageProc.RESIZEF;
				//ImageProc.RESIZEF = 2;
				//Decode(2, Rs1, Xs1, Xs2, Ys1, Ys2, liveMag1, liveMag2, liveMag3,
				//    out modelNameOK, out scoreOK, out angleOK, out toleranceOut_max, out non0_score_elements);
				//ImageProc.RESIZEF = tmpResize;
				//WriteDebug("Result --2-- 2nd attempt : " + modelNameOK + " Score: " + scoreOK.ToString() + " at Angle: " + angleOK.ToString());
				counterNG++;
				// mmm
				//name.SetCoinName("Coin not decoded.");  // !!! MM !!! no threshold aplied for 2nd attempt , counting as NG for now
			}
			else
			{
				counterOK++;
				WriteDebug("Result --1-- 1st attempt : " + modelNameOK + " Score: " + scoreOK.ToString() + " at Angle: " + angleOK.ToString());
			}

			DateTime end = DateTime.Now;
			WriteDebug("start display results" + Rs1.ToString() + ":" + Xs1.ToString() + ":" + Ys1.ToString() + "::" + Rs2.ToString() + ":" + Xs2.ToString() + ":" + Ys2.ToString());

			// display and print
			try
			{
				iP4.mShowCircles2(1, Rs1, Xs1, Ys1);
				iP4.mShowCircles2(2, Rs2, Xs2, Ys2);

				if (ImageProc.DEBUG) iP4.mCheckImages();
				//WriteDebug("start display results");

				pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
				pictureBox1.Image = iP4.GetResult1_Bitmap();

				pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
				pictureBox2.Image = iP4.GetResult2_Bitmap();
				//WriteDebug("results displayed");
			}
			catch (Exception exDC)
			{
				WriteDebug("err dislpay circles" + exDC.Message);
			}

			var iproc_time = ((end.Minute * 60 * 1000) + (end.Second * 1000) + end.Millisecond) - ((start.Minute * 60 * 1000) + (start.Second * 1000) + start.Millisecond);
			coin.name = modelNameOK;
			coin.angle = angleOK.ToString();
			coin.score = scoreOK.ToString();
			coin.time = iproc_time.ToString();

			int mag1FromModel = 0;
			int mag2FromModel = 0;
			int mag3FromModel = 0;

			if (coinIDfromFile != "")
			{
				for (int j = 0; (!string.IsNullOrEmpty(modelId[j]) && j < NUMofMODELS); j++)
				{
					if (modelName[j].Contains(coinIDfromFile))
					{
						mag1FromModel = modelMag1[j];
						mag2FromModel = modelMag2[j];
						mag3FromModel = modelMag3[j];
					}
				}
			}

			string[] image_name = imageSide1Path.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
			string[] _coinName = coin.name.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
			string image_iso = image_name[0].Substring(image_name[0].Length - 3);
			if (image_name.Length > 5 && _coinName.Length >= 3)
			{
				logData.Trace(imageIndex.ToString() + "," + imageIndex.ToString() + "," + imageIndex.ToString() + ","
					+ coin.time + "," + coin.score + "," + coin.angle + "," + non0_score_elements.ToString() + "," + toleranceOut_max.ToString() + ","
					+ _coinName[0] + "," + _coinName[1] + "," + _coinName[2] + "," + Rs1_disp.ToString() + ","
					+ liveMag1.ToString() + "," + liveMag2.ToString() + "," + liveMag3.ToString() + ","
					+ image_iso + "," + image_name[1] + "," + image_name[2] + "," + image_name[3] + ","
					+ image_name[4] + "," + image_name[5] + "," + image_name[6] + "," + imageSide1Path);
			}
			else
			{
				logData.Trace(imageIndex.ToString() + "," + imageIndex.ToString() + "," + imageIndex.ToString() + ","
					+ coin.time + "," + coin.score + "," + coin.angle + "," + non0_score_elements.ToString() + "," + toleranceOut_max.ToString() + ","
					+ "XXX" + "," + "0" + "," + "1N00000000R000" + "," + Rs1_disp.ToString() + ","
					+ mag1FromModel.ToString() + "," + mag2FromModel.ToString() + "," + mag3FromModel.ToString() + ","
					+ image_iso + "," + image_name[1] + "," + image_name[2] + "," + image_name[3] + ","
					+ image_name[4] + "," + image_name[5] + "," + image_name[6] + "," + imageSide1Path);
			}
			return;
		}

		private void DecodeFromCamera()
		{
			/* mmm to remove
			string imageSide1Path = this.imageFilePathsSide1[imageIndex];
			string imageSide2Path = this.imageFilePathsSide2[imageIndex];
			string imageEdgesPath = this.imageFilePathsEdges[imageIndex];
			*/

			// if image not present at camera return from this method
			WriteDebug("start");
			if (CamGrab.m_Bitmap2ShowB == null || CamGrab.m_Bitmap2ShowF == null || CamGrab.m_Bitmap2ShowT == null)
			{
				WriteDebug("err: one of images is null ");
				return;
			}

			DateTime start = DateTime.Now;
			// Initialise image proessing
			iP4 = new ImageProc(imageFilePathModels[0], CamGrab.m_Bitmap2ShowF, CamGrab.m_Bitmap2ShowB, CamGrab.m_Bitmap2ShowT);
			if (iP4.imagesNotRead == true)
			{
				WriteDebug("err: ImageProc init failed ");
				return;
			}

			// Load MODELS if they are not loaded yet
			// 1. Load models if not yet loaded
			// 2. Find circle in model and
			// 3. generate all rotations of the model
			if (!imageModelsArraySet)
			{
				LoadModels();
			}

			// check top edge coin thickness and colour
			/*
            try
            {
                iP4.mFitLine();
            }
            catch (Exception eFitline1)
            {
                WriteDebug(eFitline1.Message);
            }            
            */

			int radius_min = 1;
			int radius_max = 1;

			// Get diameter parameter of side1 from file name 
			int side1Diemeters = PortDataReceived.parSA[3];
			int side2Diemeters = PortDataReceived.parSA[3];
			int liveMag1 = PortDataReceived.parSA[0];
			int liveMag2 = PortDataReceived.parSA[1];
			int liveMag3 = PortDataReceived.parSA[2];
			int liveRadius = PortDataReceived.parSA[3];

			int origMag1 = PortDataReceived.parSAorig[0];
			int origMag2 = PortDataReceived.parSAorig[1];
			int origMag3 = PortDataReceived.parSAorig[2];
			int origRadius = PortDataReceived.parSAorig[3];

			WriteDebug("start Find circle on the actual image");
			
			// get actual coin Side1, find circle, make quarter size  
			radius_min = (int)(((side1Diemeters - Settings.SettingsItems.RADT) * 148 / 1000) / 2);

			int MULTIPLE_FOR_LARGE_COINS = 4;
			if (PortDataReceived.largeRadius)
				radius_max = (int)(((side1Diemeters + Settings.SettingsItems.RADT * MULTIPLE_FOR_LARGE_COINS) * 148 / 1000) / 2);
			else
				radius_max = (int)(((side1Diemeters + 4 + Settings.SettingsItems.RADT) * 148 / 1000) / 2);

			ImageProc._image_models_array_set = imageModelsArraySet;
			int result_s1 = iP4.mCircleHough(1, radius_min, radius_max, out float Xs1, out float Ys1, out float Rs1);
			int result_s2 = iP4.mCircleHough(2, radius_min, radius_max, out float Xs2, out float Ys2, out float Rs2);
			
			// replacing camera diameter with sensor diameter (when from camera)
			// replacing camera diameter with file name diameter (when from files)
			float Rs1_disp = Rs1;
			Rs1 = ((side1Diemeters * 148 / 1000) / 2);
			Rs2 = ((side1Diemeters * 148 / 1000) / 2);
			float radius_camera = Rs1 * 2 * 1000 / 148;

			// prepare quarter images for coin's side 1 and 2 
			int result_qs1 = iP4.mQuarterImage(1, Rs1, Xs1, Ys1);
			int result_qs2 = iP4.mQuarterImage(2, Rs2, Xs2, Ys2);

			// Stop for debugging MM !!!
			if (imageIndex == 90)
			{
				int nop = 0;
			}
			// Stop for debugging MM !!!

			// DECODE
			// Find rotation angle1
			// Check actual image rorated by the angle1 
			// and generate the correlation (score)
			Decode(1, Rs1, Xs1, Xs2, Ys1, Ys2, liveMag1, liveMag2, liveMag3,
				out string modelNameOK, out int scoreOK, out int angleOK, out int toleranceOut_max, out int non0_score_elements);
			if (scoreOK < Settings.SettingsItems.TRMI)
			{
				// if second atempt is required, then do it here
				//Decode(2, Rs1, Xs1, Xs2, Ys1, Ys2, liveMag1, liveMag2, liveMag3, 
				//    out modelNameOK, out scoreOK, out angleOK, out toleranceOut_max, out non0_score_elements);
				//WriteDebug("Result --- 222 --- 2nd attempt : " + modelNameOK + " Score: " + scoreOK.ToString() + " at Angle: " + angleOK.ToString());
				counterNG++;				
				name.SetCoinName("Coin not decoded.");				
				// mmm
				//saveImages();
			}
			else
			{
				WriteDebug("Result --- 111 --- 1st attempt : " + modelNameOK + " Score: " + scoreOK.ToString() + " at Angle: " + angleOK.ToString());
				name.SetCoinName(modelNameOK + " Score: " + scoreOK.ToString() + " at Angle: " + angleOK.ToString());
				counterOK++;
			}

			DateTime end = DateTime.Now;
			WriteDebug("start display results" + Rs1.ToString() + ":" + Xs1.ToString() + ":" + Ys1.ToString() + "::" + Rs2.ToString() + ":" + Xs2.ToString() + ":" + Ys2.ToString());

			try
			{
				iP4.mShowCircles2(1, Rs1, Xs1, Ys1);
				iP4.mShowCircles2(2, Rs2, Xs2, Ys2);
				//WriteDebug("circles created");

				pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
				pictureBox1.Image = iP4.GetResult1_Bitmap();

				//WriteDebug("result 1 dislpayed");
				pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
				pictureBox2.Image = iP4.GetResult2_Bitmap();
				//WriteDebug("result 2 dislpayed");
			}
			catch (Exception eDisp)
			{
				WriteDebug("start display results:" + eDisp.Message);
			}

			WriteDebug("results displayed");

			var iproc_time = ((end.Minute * 60 * 1000) + (end.Second * 1000) + end.Millisecond) - ((start.Minute * 60 * 1000) + (start.Second * 1000) + start.Millisecond);

			int mag1FromModel = 0;
			int mag2FromModel = 0;
			int mag3FromModel = 0;
			string[] _coinName = { "-" };

			try
			{
				coin.name = modelNameOK;
				coin.angle = angleOK.ToString();
				coin.score = scoreOK.ToString();
				coin.time = iproc_time.ToString();
				_coinName = coin.name.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);


				if (coin.name != "")
				{
					for (int j = 0; (!string.IsNullOrEmpty(modelId[j]) && j < NUMofMODELS); j++)
					{
						if (modelName[j].Contains(coin.name))
						{
							mag1FromModel = modelMag1[j];
							mag2FromModel = modelMag2[j];
							mag3FromModel = modelMag3[j];
						}
					}
				}
			}
			catch (Exception eM)
			{
				WriteDebug("magXFromModel index fail");
			}

			try
			{


				if (_coinName.Length >= 3)
				{
					logData.Trace(
						CamGrab.IcountF.ToString() + "," + CamGrab.IcountT.ToString() + "," + CamGrab.IcountB.ToString() + ","      // Camera frame counters 
						+ coin.time + "," + coin.score + "," + coin.angle + ","                                                     // Coin parameters from Coindetection
						+ non0_score_elements.ToString() + "," + toleranceOut_max.ToString() + ","                                  // how many models were considered / which of the dia-mag parameters passed 
						+ _coinName[0] + "," + _coinName[1] + "," + _coinName[2] + "," + Rs1_disp.ToString() + ","                  // detected coin name split + radius
						+ mag1FromModel.ToString() + "," + mag2FromModel.ToString() + "," + mag3FromModel.ToString() + ","          // detected coin name split magnetic parameters
						+ "Cam" + "," + "0" + "," + "0N00000000R000" + "," + Rs1.ToString() + ","                                   // Parameters detected from the file name in simul mode  + radius in pixels
						+ liveRadius.ToString() + "," + liveMag1.ToString() + "," + liveMag2.ToString() + "," + liveMag3.ToString() + "," // magnetic parameters from the sensor after calibration
						+ origRadius.ToString() + "," + origMag1.ToString() + "," + origMag2.ToString() + "," + origMag3.ToString()); // magnetic parameters from the sensor original
				}
				else
				{
					logData.Trace(
						CamGrab.IcountF.ToString() + "," + CamGrab.IcountT.ToString() + "," + CamGrab.IcountB.ToString() + ","      // Camera frame counters 
						+ coin.time + "," + coin.score + "," + coin.angle + ","                                                     // Coin parameters from Coindetection
						+ non0_score_elements.ToString() + "," + toleranceOut_max.ToString() + ","                                  // how many models were considered / which of the dia-mag parameters passed 
						+ "CaM" + "," + "0" + "," + "0N00000000R000" + "," + Rs1_disp.ToString() + ","                  // detected coin name split + radius
						+ mag1FromModel.ToString() + "," + mag2FromModel.ToString() + "," + mag3FromModel.ToString() + ","          // detected coin name split magnetic parameters
						+ "CaM" + "," + "0" + "," + "0N00000000R000" + "," + Rs1.ToString() + ","                                   // Parameters detected from the file name in simul mode  + radius in pixels
						+ liveRadius.ToString() + "," + liveMag1.ToString() + "," + liveMag2.ToString() + "," + liveMag3.ToString() + "," // magnetic parameters from the sensor after calibration
						+ origRadius.ToString() + "," + origMag1.ToString() + "," + origMag2.ToString() + "," + origMag3.ToString()); // magnetic parameters from the sensor original
				}
			}
			catch (Exception eXX)
			{
				WriteDebug("writing logs fail");
			}
			/*
            if (image_name.Length > 5 && _coinName.Length >= 3)
            {
                logData.Trace(imageIndex.ToString() + "," + imageIndex.ToString() + "," + imageIndex.ToString() + ","
                    + coin.time + "," + coin.score + "," + coin.angle + "," + non0_score_elements.ToString() + "," + toleranceOut_max.ToString() + ","
                    + _coinName[0] + "," + _coinName[1] + "," + _coinName[2] + "," + Rs1_disp.ToString() + ","
                    + liveMag1.ToString() + "," + liveMag2.ToString() + "," + liveMag3.ToString() + ","
                    + image_iso + "," + image_name[1] + "," + image_name[2] + "," + image_name[3] + ","
                    + image_name[4] + "," + image_name[5] + "," + image_name[6] + "," + imageSide1Path);
            }
            else
            {
                logData.Trace(imageIndex.ToString() + "," + imageIndex.ToString() + "," + imageIndex.ToString() + ","
                    + coin.time + "," + coin.score + "," + coin.angle + "," + non0_score_elements.ToString() + "," + toleranceOut_max.ToString() + ","
                    + "XXX" + "," + "0" + "," + "1N00000000R000" + "," + Rs1_disp.ToString() + ","
                    + mag1FromModel.ToString() + "," + mag2FromModel.ToString() + "," + mag3FromModel.ToString() + ","
                    + image_iso + "," + image_name[1] + "," + image_name[2] + "," + image_name[3] + ","
                    + image_name[4] + "," + image_name[5] + "," + image_name[6] + "," + imageSide1Path);
            }
            */
			WriteDebug("write Logs, leave DecodeFromCamera");

			return;
		}

		#endregion


		// loads images from files list using index of image
		private void ShowNextImage(int NextImage)
		{

			string imageEdgesPath = "";
			string imageSide1Path = "";
			string imageSide2Path = "";

			if (imageFilePathsEdges.Length >= NextImage && imageFilePathsSide1.Length >= NextImage && imageFilePathsSide2.Length >= NextImage)
			{
				imageEdgesPath = this.imageFilePathsEdges[NextImage];
				imageSide1Path = this.imageFilePathsSide1[NextImage];
				imageSide2Path = this.imageFilePathsSide2[NextImage];
			}
			else
			{
				WriteDebug("ShowNextImage: Next image does not exist");
				return;
			}
			iP4 = new ImageProc(imageSide1Path, imageSide2Path, imageEdgesPath);

			if (File.Exists(imageEdgesPath) && File.Exists(imageSide1Path) && File.Exists(imageSide2Path))
			{
				//iP3.mNextImage(imageSide1Path, imageSide2Path, imageEdgesPath);
				//iP3 = new ImageProc(imageSide1Path, imageSide2Path, imageEdgesPath);

				imageTop = new Image<Bgr, Byte>(imageEdgesPath);
				imageSide1 = new Image<Gray, Byte>(imageSide1Path);
				imageSide2 = new Image<Gray, Byte>(imageSide2Path);
				ShowFileName();

				if (imageTop == null || imageSide1 == null || imageSide2 == null)
				{
					MessageBox.Show("ShowNextImage: Loading image filed");
					return;
				}
				else
				{
					int res = iP4.mCpyImages2iP(imageSide1, imageSide2, imageTop);
					pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
					pictureBox3.Image = imageTop.ToBitmap();
					pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
					pictureBox1.Image = imageSide1.ToBitmap();
					pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
					pictureBox2.Image = imageSide2.ToBitmap();
				}
			}
			else
			{
				MessageBox.Show("ShowNextImage: Images paths not OK");
				return;
			}
		}

		// get actual image file name, strip the path and load variable to be displaid in refresh
		private void ShowFileName()
		{
			if (imageFilePathsSide1.Length >= imageIndex)
			{
				imageFileName = this.imageFilePathsSide1[imageIndex];
				int path_index = imageFileName.LastIndexOf('\\');
				if (path_index < imageFileName.Length) imageFileName = imageFileName.Substring(path_index + 1, imageFileName.Length - path_index - 1);
				else imageFileName = "--- err load file name lenght ---";

			}
			else imageFileName = "--- err load file name ---";
		}

		static public void SendResultsN(string strModelName)
		{
			//default result: <Xon> CIy x CUR DENOMINA Tag CoinID <Xoff>
			//                <XON> CIxTagCCCDDDDDDDDxxxxx <XOFF >
			// y = A - Accept, R = Reject(or P or O - not sure what these were for) *”
			// Tag(A – Z) CUR(CCC) DENOM(DDDDDDDDn),
			// CoinID Coin identification 
			// Composite Result data for coin ”s” “n” “xxxxxxxx” “f” “mmm” where:
			// ”s” = Coin Status. 1 = Current, 2 = Outdated(?), 3 = Obsolete
			// “n” = Coin ID Code Type.N = Numista, C = ChangeBox Internal
			// “xxxxxxxx” = Coin ID Code. (numista or Changebox – see ID Code Type)
			// “f” = Coin face used for detection.O = Obverse, R = Reverse
			// “mmm” = Spares for future use – 999 as default
			// Example Message: - <XON>CIAxEUR000010000N12345678R999<XOFF>   

			//tct.timer.Interval = 8000;
			byte[] data = new byte[32]
			{
				(byte)17,(byte)67,(byte)73,(byte)82, // <Xonn> CIR - Reject
                (byte)120,  //<CRC8>    
                (byte)78,(byte)79,(byte)78,(byte)48,(byte)48,(byte)48,(byte)48,(byte)48,(byte)48,(byte)48,(byte)48, // CURDENOMINA - NON00000000
                (byte)65,  //Tag A  
                (byte)48,(byte)67,(byte)49,(byte)50,(byte)51,(byte)52,(byte)53,(byte)54,(byte)55,(byte)56,(byte)82,(byte)57,(byte)57,(byte)57, // 0C12345678R999 
                (byte)19   //<Xoff>
            };

			if (PortDataReceived.triggCounter >= 65 && PortDataReceived.triggCounter <= 90)
				data[16] = Convert.ToByte(PortDataReceived.triggCounter);
			else
				data[16] = Convert.ToByte('A'); ;

			int indexOfNON = strModelName.IndexOf("NON");
			if (indexOfNON == 0)
			{
				/*
                ImageProc.ScoreToDisplay = 0;
                if (ImageProc.ScoreToDisplay > ImageProc.ScoreMax)
                {
                    ImageProc.ScoreMax = ImageProc.ScoreToDisplay;
                }
                if (ImageProc.ScoreToDisplay < ImageProc.ScoreMin)
                {
                    ImageProc.ScoreMin = ImageProc.ScoreToDisplay;
                }
                */
				//ImageProc.ScoreMax = 0;
				//ImageProc.ScoreMin = 100;
			}
			if (strModelName.Length == 25) // test if string is valid coin passed or failure to detect 
			{
				if (strModelName != "NON00000000A0N12345678R999")
				{
					string[] ModelNameSplited;
					// set model name if the curency is not NON                 
					if (indexOfNON == -1)
					{
						data[3] = (byte)65; // A - Accept
						for (int i = 5; i < 16; i++)
						{
							data[i] = (byte)strModelName[i - 5];
						}
						for (int i = 17; i < 31; i++)
						{
							data[i] = (byte)strModelName[i - 6];
						}
					}
					else if (indexOfNON == 0)
					{
						for (int i = 5; i < 30; i++)
						{
							data[i] = (byte)strModelName[i - 5];

						}
						data[3] = (byte)82; // R - Reject

					}
					data[16] = Convert.ToByte(PortDataReceived.triggCounter);
					// image counter modulo 100
					// data[4] = (byte)(ImageProc.ImageIndex % 100);
					PortDataReceived.send2IFCOM(data);
					Debug.WriteLine("VPC->IFC: " + data);
					//log.Info("VPC->IFC port: " + System.Text.Encoding.UTF8.GetString(data, 0, data.Length));
					PortDataReceived.DiscartCOMinBuffer();
				}
			}
			else
			{
				PortDataReceived.send2IFCOM(data);
				Debug.WriteLine("VPC->IFC: " + data);
				PortDataReceived.DiscartCOMinBuffer();
				//log.Info("VPC->IFC port: " + System.Text.Encoding.UTF8.GetString(data, 0, data.Length));

			}
		}

		static public void SendCommand(int errCode, char comReply)
		{
			//
			//tct.timer.Interval = 8000;
			var log = new SimpleLogger(true);
			if (errCode > 99 || errCode < 0) errCode = 99;

			char[] data = new char[8]
			{
				(char)17,'C','I','M', 'x', '0', '0', (char)19
			}; //<Xoff> // <Xonn> ICMx

			data[3] = comReply;
			data[6] = (char)(48 + (errCode % 10));
			data[5] = (char)(48 + (errCode / 10));

			byte[] data2serial = System.Text.Encoding.UTF8.GetBytes(data);
			PortDataReceived.send2IFCOM(data2serial);
			PortDataReceived.DiscartCOMinBuffer();
			log.Info("VPC->IFC confirmation with err code: " + errCode.ToString());
		}

		private void WriteDebug(string d_message)
		{
			string methodName = new StackTrace().GetFrame(1).GetMethod().Name;
			string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff ";
			string full_message = /*System.DateTime.Now.ToString(datetimeFormat)*/ methodName + ": " + d_message;
			Debug.WriteLine(full_message);
			Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + full_message);
			log.Debug(full_message);
		}

		private void Main_Load(object sender, EventArgs e)
		{

		}

		private void button4_Click(object sender, EventArgs e)
		{
			if (!s.isSaveMode())
			{
				// Pop up window to feel up fime name                
				// FormName.CoinStruc coinStruc = new FormName.CoinStruc();
				//name.ShowDialog();
				//textBoxFileName.Text = name.GetCoinName();
				//if (name.GetSaveMode())
				//{

				s.setSaveMode();
				button4.BackColor = Color.Green;
				textBoxFileName.Enabled = true;
				textBoxFileName.Visible = true;
				//}
			}
			else
			{
				//name.ClearSaveMode();
				s.setIdleMode();
				button4.BackColor = Color.Gray;
				textBoxFileName.Enabled = false;
				textBoxFileName.Visible = false;
			}
		}

		private void button5_Click(object sender, EventArgs e)
		{
			if (!s.isSaveMode())
			{
				// Pop up window to feel up fime name                
				//FormName.CoinStruc coinStruc = new FormName.CoinStruc();
				name.ShowDialog();
				textBoxFileName.Text = name.GetCoinName();
				if (name.GetSaveMode())
				{
					s.setSaveMode();
					button5.BackColor = Color.Green;
				}
			}
			else
			{
				//name.ClearSaveMode();
				s.setIdleMode();
				button5.BackColor = Color.Gray;
			}
		}
	}
}
