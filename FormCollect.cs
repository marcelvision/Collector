using Coinpare;
using Collector;
using Emgu.CV;
using Emgu.CV.Structure;
using MvCamCtrl.NET;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;

namespace Collector
{
    public partial class FormCollect : Form, IComListener
    {
        private PortDataReceived _portWorker;
        //PortDataReceived _portWorker = new PortDataReceived();

        #region Initial Deffinitions

        // Import the kernel32.dll function to allocate a console
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();


        //Optional: Import to attach to the parent process console if it exists
        //[DllImport("kernel32.dll")]
        //private static extern bool AttachConsole(int dwProcessId);
        //private const int ATTACH_PARENT_PROCESS = -1;

        FormName NameForm = new FormName();

        string VERSION = "Collector 0.1";
        string workingDirPath = "D:\\COINTER\\Images\\TrainUnsorted";
        string modelsDirPath = "D:\\COINTER\\Images\\ModelsN";
        public const string MODEL_IMAGES_DIRECTORY = "D:\\COINTER\\Images\\ModelsN\\";
        SimpleLogger log = new SimpleLogger(true);
        SimpleLogger logData = new SimpleLogger(false);
        CStatus s = new CStatus();
        int timerEntryIndex = 0;
        string lastISO;
        bool saveImgInRun = false;

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
        const int NUMofMODELS = 2500;
        Image<Gray, Byte>[] imageModel = new Image<Gray, Byte>[NUMofMODELS];
        Image<Gray, Byte>[] imageResModel = new Image<Gray, Byte>[NUMofMODELS];
        //private readonly ResultMatrix matrix = new ResultMatrix(NUMofMODELS, NUMofMODELS);
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
        bool takeImageFromFile = false;

        public struct Coin
        {
            public string name;
            public string score;
            public string angle;
            public string time;
        }
        Coin coin = new Coin();


        public enum tmType { IMAGE, TRIGG, RESULT, OUT };
        public struct tmCue
        {
            public int iDNumber;
            public string description;
            // 1 - IMAGE, 2 - TRIGG, 3 - RESULT, 4 - OUT
            public tmType tmType;
        }

        public static List<tmCue> tmCueList = new List<tmCue>();
        public static int tmCueCounterLast = 0;

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

        public FormCollect()
        {
            AllocConsole();
            InitializeComponent();
            this.Text = VERSION;
            _portWorker = new PortDataReceived(this);
            WriteDebug("Load seetings: ");
            Settings.Load();
            WriteDebug("Init com port: ");
            Console.WriteLine("Init com port: ");
            Debug.WriteLine("Init com port: 1");
            Trace.WriteLine("Init com port: 2");
            //string comPortInitialised = PortDataReceived.initIFCCOM();
            string comPortInitialised = _portWorker.initIFCCOM();

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
            int nOfCameras = CamGrab.getCameras();
            Thread.Sleep(500);
            WriteDebug("Cameras initialised: " + nOfCameras.ToString());

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

            // load filenames from models directory
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
            this.ActiveControl = buttonSetModels;
            //LoadModels(true);

            lastISO = Settings.SettingsItems.LISO;

            WriteDebug("simulation of image index " + imageIndex.ToString());
            ShowNextImage(imageIndex);
            ShowFileName();
            takeImageFromFile = true;            
            takeImageFromFile = false;
            System.Threading.Thread.Sleep(Settings.SettingsItems.SLPT);
            counterNG = 0;
            counterOK = 0;
            tct.Start();

        }

        #endregion Main Form

        #region Com Worker

        /*
		private void FormMain_Load(object sender, EventArgs e)
        {
            _portWorker.Start();
        }
		*/



        public void OnComData(int errCode, char data)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnComData(errCode, data)));
                return;
            }

            SendCommand(errCode, data);
            //txtOutput.AppendText($"{data}\r\n");
        }

        void IComListener.FromCOMSendResultsN(string results)
        {
            FromCOMSendResultsN(results);
        }

        private void FromCOMSendResultsN(string results)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => FromCOMSendResultsN(results)));
                return;
            }
        }

        public void TimerStartTC()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => TimerStartTC()));
                return;
            }

            tc.Start();
        }

        public void TimerStartTCi()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => TimerStartTCi()));
                return;
            }

            tci.Start();
        }

        public void TimerStopTC()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => TimerStopTC()));
                return;
            }
            tc.Stop();
        }

        /*
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _portWorker.Stop();
        }
		*/
        #endregion

        #region Timers        

        #region TIMER Image grabbing collection

        public void TimerImage_Elapsed(object? sender, ElapsedEventArgs e)
        {
            tci.Stop();

            
            if (FormCollect.tmCueList.Count != FormCollect.tmCueCounterLast)
            {
                WriteDebug("tmCueList count: " + FormCollect.tmCueList.Count.ToString() + " last: " + FormCollect.tmCueCounterLast.ToString());
                for (int i = 0; i < FormCollect.tmCueList.Count; i++)
                {
                    WriteDebug("tmCueList count: " + i.ToString() + " : " + FormCollect.tmCueList[i].iDNumber.ToString() + " : " + FormCollect.tmCueList[i].description);
                }

                FormCollect.tmCueCounterLast = FormCollect.tmCueList.Count;
            }
            

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

                if (imagesNotReadyCounter > 200)
                {
                    s.setErrMode();
                    CamGrab.destroyGrabbers();
                    Thread.Sleep(1000);
                    int nOfCameras = CamGrab.getCameras();
                    imagesNotReadyCounter = 0;
                    s.setIdleMode(); 
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
                if (_portWorker.operationMODE == 1)
                {
                    WriteDebug("save mode: operation mode changing from 1 to 2");
                    if (!s.isSaveMode()) s.setRunMode();
                    _portWorker.operationMODE = 2;
                }
                else WriteDebug("save mode: operation mode is not 1");
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
                    //imageProcessing();
                    
                    stop_tc = true;
                }
                catch (Exception improcErr)
                {
                    SendResultsN("NON000000000N12345678R999");
                    _portWorker.comTestSendRequested = false;
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
            else if (s.isErrMode())
            {
                if (_portWorker.comTestSendRequested == true)
                {
                    SendResultsN("NON00000000A0N12345678R999");
                    _portWorker.comTestSendRequested = false;
                }
            }
            if (!stop_tc) tc.Start();

            // wait for results
        }
        #endregion

        #region TIMER Heartbit

        public void TimerHeartbit_Elapsed(object? sender, ElapsedEventArgs e)
        {
            tct.Stop();
            
            CheckAndResetCams();
            /*
            if (_portWorker.IsCOMopen())
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
            }*/

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
                if (_portWorker.operationMODE == 2)
                    Invoke((System.Windows.Forms.MethodInvoker)(() => this.button3.BackColor = System.Drawing.Color.Yellow));
                else if (_portWorker.operationMODE == 1)
                    Invoke((System.Windows.Forms.MethodInvoker)(() => this.button3.BackColor = System.Drawing.Color.Green));
                else if (_portWorker.operationMODE == 0)
                    Invoke((System.Windows.Forms.MethodInvoker)(() => this.button3.BackColor = System.Drawing.Color.Gray));
                else if (_portWorker.operationMODE == 3)
                    Invoke((System.Windows.Forms.MethodInvoker)(() => this.button3.BackColor = System.Drawing.Color.Red));
                else if (CamGrab.m_bGrabbing)
                    Invoke((System.Windows.Forms.MethodInvoker)(() => this.button3.BackColor = System.Drawing.Color.LightBlue));
            }
            else
            {
                if (_portWorker.operationMODE == 2)
                    button3.BackColor = Color.Yellow;
                else if (_portWorker.operationMODE == 1)
                    button3.BackColor = Color.Green;
                else if (_portWorker.operationMODE == 0)
                    button3.BackColor = Color.Gray;
                else if (_portWorker.operationMODE == 3)
                    button3.BackColor = Color.Red;
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
            if (this.buttonSetModels.InvokeRequired)
            {
                Invoke((System.Windows.Forms.MethodInvoker)(() => this.buttonSetModels.Text = _portWorker.CounterCOMReceived.ToString() + "/" + _portWorker.CounterCOMPacket.ToString()));
                if (_portWorker.IsCOMopen())
                    Invoke((System.Windows.Forms.MethodInvoker)(() => this.buttonSetModels.BackColor = System.Drawing.Color.Green));
                else Invoke((System.Windows.Forms.MethodInvoker)(() => this.buttonSetModels.BackColor = System.Drawing.Color.Red));
            }
            else
            {
                this.buttonSetModels.Text = _portWorker.CounterCOMReceived.ToString() + "/" + _portWorker.CounterCOMPacket.ToString();
                if (_portWorker.IsCOMopen())
                    this.buttonSetModels.BackColor = System.Drawing.Color.Green;
                else this.buttonSetModels.BackColor = System.Drawing.Color.Red;
            }
            // refresh coin NameForm textbox
            if (this.textBoxCoinName.InvokeRequired)
            {
                Invoke((System.Windows.Forms.MethodInvoker)(() => this.textBoxCoinName.Text = coin.name));
                if (coin.name == "ND")
                    Invoke((System.Windows.Forms.MethodInvoker)(() => this.textBoxCoinName.ForeColor = Color.Red));
                else
                    Invoke((System.Windows.Forms.MethodInvoker)(() => this.textBoxCoinName.ForeColor = Color.Green));

            }
            else
            {
                this.textBoxCoinName.Text = coin.name;
                if (coin.name == "ND")
                    this.textBoxCoinName.ForeColor = Color.Red;
                else
                    this.textBoxCoinName.ForeColor = Color.Green;
            }

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
            */
            //refresh file NameForm textbox
            if (this.textBoxFileName.InvokeRequired)
            {
                if (s.isRunMode()) Invoke((System.Windows.Forms.MethodInvoker)(() => this.textBoxFileName.Text = NameForm.GetCoinName()));
                else Invoke((System.Windows.Forms.MethodInvoker)(() => this.textBoxFileName.Text = imageFileName));
                //Invoke((MethodInvoker)(() => this.textBoxCameraCounters.BackColor = System.Drawing.Color.Green));
            }
            else
            {
                if (s.isRunMode()) this.textBoxFileName.Text = NameForm.GetCoinName();
                else this.textBoxFileName.Text = imageFileName;
                //this.textBoxCameraCounters.BackColor = System.Drawing.Color.Green;
            }
            /*
            //refresh coin NameForm textbox
            if (this.textBoxCoinName.InvokeRequired)
                Invoke((System.Windows.Forms.MethodInvoker)(() => this.textBoxCoinName.Text = coin.NameForm));
            else
                this.textBoxCoinName.Text = coin.NameForm;
			*/
            if (this.textBox2.InvokeRequired)
                Invoke((System.Windows.Forms.MethodInvoker)(() => this.textBox2.Text = coin.score));
            else
                this.textBox2.Text = coin.score;

            if (this.textBox3.InvokeRequired)
                Invoke((System.Windows.Forms.MethodInvoker)(() => this.textBox3.Text = coin.time));
            else
                this.textBox3.Text = coin.time;

            tcr.Start();

        }
        #endregion

        #endregion Timers

        #region Load Models and find circles
        private void LoadModels(bool detailsCheck)
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
                                if (detailsCheck)
                                    modelDetail[i] = true;
                            }
                            else
                            {
                                modelDetail[i] = false;
                            }
                        }
                        // Collect information about presence of Negative models
                        string shouldNotHaveImagePath = modelsDirPath + "\\" + modelName[i] + "\\Negative\\";
                        if (Directory.Exists(shouldNotHaveImagePath))
                        {
                            string[] shutNotHaveImageNames = Directory.GetFiles(shouldNotHaveImagePath, "*.bmp", SearchOption.TopDirectoryOnly);
                            if (shutNotHaveImageNames.Length > 0)
                            {
                                if (detailsCheck)
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
                //SendResultsN("NON000000000N12345678R999");

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
            // build coin NameForm            
            StringBuilder pathname_builder = new StringBuilder();
            pathname_builder.Append(FormName.MODELSDIRECTORY);
            pathname_builder.Replace("Images\\ModelsN", "SavedImages");
            pathname_builder.Append(NameForm.GetCoinName());

            string[] path_nameParts = pathname_builder.ToString().Split(new string[] { "_" }, StringSplitOptions.None);

            string path_name = pathname_builder.ToString();

            if (NameForm.autoFillUp)
            {
                CultureInfo ci = CultureInfo.CurrentCulture;
                path_nameParts[3] = _portWorker.parSA[3].ToString("D4", ci);
                path_nameParts[4] = _portWorker.parSA[0].ToString("D4", ci);
                path_nameParts[5] = _portWorker.parSA[1].ToString("D4", ci);
                path_nameParts[6] = _portWorker.parSA[2].ToString("D4", ci);
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
                    _saveImages(filenameF, filenameB, filenameT);
                    
                    //path_nameParts[0].Substring(path_nameParts[0].Length - 3, path_nameParts[0].Length);
                    //string coin_name = string.Join("_", path_nameParts);
                    //NameForm.SetCoinName(coin_name);
                    
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

        public void _saveImages(string filenameF, string filenameB, string filenameT )
        {
            CamGrab.m_Bitmap2ShowF.Save(filenameF, ImageFormat.Bmp);
            CamGrab.m_Bitmap2ShowB.Save(filenameB, ImageFormat.Bmp);
            CamGrab.m_Bitmap2ShowT.Save(filenameT, ImageFormat.Bmp);
        }

        #endregion

        private void CheckAndResetCams()
        {
            int nOfCameras = 0;
            if (CamGrab.checkCameras() != 0)
            {
                s.setErrMode();
                CamGrab.destroyGrabbers();
                Thread.Sleep(1000);
                nOfCameras = CamGrab.getCameras();
                imagesNotReadyCounter = 0;
                if (nOfCameras == 3)
                {
                    s.setIdleMode();
                }
            }
            else
            {
                int nRet = -1;
                nRet = CamGrab.SetPacketSize(9000, CamGrab.m_FrontCamera);
                nRet = nRet + CamGrab.SetPacketSize(9000, CamGrab.m_BackCamera);
                nRet = nRet + CamGrab.SetPacketSize(9000, CamGrab.m_TopCamera);
                if (nRet != 0)
                {
                    //Console.WriteLine("Error setting packet size");
                    /*
                    s.setErrMode();
                    CamGrab.destroyGrabbers();
                    Thread.Sleep(1000);
                    nOfCameras = CamGrab.getCameras();
                    imagesNotReadyCounter = 0;
                    if (nOfCameras == 3)
                    {
                        s.setIdleMode();
                    }*/
                }
            }
        }

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

        // get actual image file NameForm, strip the path and load variable to be displaid in refresh
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

        public void SendResultsN(string strModelName)
        {
            tmCueList.Clear();
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

            if (_portWorker.triggCounter >= 65 && _portWorker.triggCounter <= 90)
                data[16] = Convert.ToByte(_portWorker.triggCounter);
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
                    // set model NameForm if the curency is not NON                 
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
                    data[16] = Convert.ToByte(_portWorker.triggCounter);
                    // image counter modulo 100
                    // data[4] = (byte)(ImageProc.ImageIndex % 100);
                    // PortDataReceived.send2IFCOM(data);
                    _portWorker.send2IFCOM(data);
                    Debug.WriteLine("VPC->IFC: " + data);
                    //log.Info("VPC->IFC port: " + System.Text.Encoding.UTF8.GetString(data, 0, data.Length));
                    _portWorker.DiscartCOMinBuffer();
                }
            }
            else
            {
                //PortDataReceived.send2IFCOM(data);
                _portWorker.send2IFCOM(data);
                Debug.WriteLine("VPC->IFC: " + data);
                _portWorker.DiscartCOMinBuffer();
                //log.Info("VPC->IFC port: " + System.Text.Encoding.UTF8.GetString(data, 0, data.Length));

            }
            // update task manager que 
            FormCollect.tmCue tmCueCom = new FormCollect.tmCue();
            tmCueCom.iDNumber = _portWorker.triggCounter;
            tmCueCom.description = "Sending to COM: " + data.ToString();
            tmCueCom.tmType = FormCollect.tmType.OUT;
            FormCollect.tmCueList.Add(tmCueCom);
        }

        public void SendCommand(int errCode, char comReply)
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

            _portWorker.send2IFCOM(data2serial);
            //PortDataReceived.send2IFCOM(data2serial);
            _portWorker.DiscartCOMinBuffer();
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
                // Pop up window to feel up fime NameForm                
                // FormName.CoinStruc coinStruc = new FormName.CoinStruc();
                //NameForm.ShowDialog();
                //textBoxFileName.Text = NameForm.GetCoinName();
                //if (NameForm.GetSaveMode())
                //{

                s.setSaveMode();
                button4.BackColor = Color.Green;
                textBoxFileName.Enabled = true;
                textBoxFileName.Visible = true;
                //}
            }
            else
            {
                //NameForm.ClearSaveMode();
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
                // Pop up window to feel up fime NameForm                
                //FormName.CoinStruc coinStruc = new FormName.CoinStruc();
                NameForm.ShowDialog();
                textBoxFileName.Text = NameForm.GetCoinName();
                if (NameForm.GetSaveMode())
                {
                    //s.setSaveMode();
                    button5.BackColor = Color.Green;
                }
            }
            else
            {
                //NameForm.ClearSaveMode();
                //s.setIdleMode();
                button5.BackColor = Color.Gray;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CheckAndResetCams();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!saveImgInRun)
            {
                button6.BackColor = Color.Green;
                saveImgInRun = true;
            }
            else
            {
                button6.BackColor = Color.White;
                saveImgInRun = false;

            }
        }
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Close camera driver
            CamGrab.destroyGrabbers();
        }
    }
}
