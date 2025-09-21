using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Runtime.InteropServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace Cointero
{
    
    public class PortDataReceived
    {
        
        private static SerialPort IFCport = new SerialPort(Settings.SettingsItems.EXPT);
        //private static SerialPort IFCport = new SerialPort("COM5", 115200);
        private static string dataInBuffer = "";

        public static int CounterCOMPacket = 0;
		public static int CounterCOMReceived = 0;
		public static int[] parSA = new int[5]; //{ 123,345,678,900,0 };
        public static int[] parSAorig = new int[5];
        public static char triggCounter = 'A';
        public static bool largeRadius = false; // when rdius is larger then limit in settings, all larger radiuses has to be involved 

        private static Status s = new Status();
        public readonly string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff ";

        /* PortDataReceived.operationMODE 
        0 - STOP/Idle
        1 - Run from camera
        2 - Run from file
        3 - Run reject all coins */
        public static int operationMODE =0;

        public static string initIFCCOM()
        {
            //IFCport.BaudRate = 115200;
            string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff ";
            string ComPortInitialised = "Com port opened" ;
            IFCport.BaudRate = 115200;
            IFCport.Parity = Parity.None;
            IFCport.StopBits = StopBits.One;
            IFCport.DataBits = 8;
            IFCport.Handshake = Handshake.None;
            IFCport.ReadTimeout = 300;
            
            IFCport.DataReceived += new SerialDataReceivedEventHandler(IFCReceivedHandler);
            
            
            try
            {
                IFCport.Open();
                //ComPortInitialised = true;
                Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "v0.71 : Port opened..." + Settings.SettingsItems.EXPT);
                ComPortInitialised = "Com port " + Settings.SettingsItems.EXPT + " opened";
                IFCport.DiscardInBuffer();
            }
            catch (Exception exSerial)
            {
                Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "Opening " + Settings.SettingsItems.EXPT +" failed.");
                Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "PortDataReceived: exeption: " + exSerial.Message );
                ComPortInitialised = exSerial.Message;
            }
            return ComPortInitialised;
        }

        public static bool DiscartCOMinBuffer()
        {
            /*
             if (comPortNo == 1)
            { SAport.DiscardInBuffer(); }
            else if (comPortNo == 2)
            { IFCport.DiscardInBuffer(); }
            else { return false; }
            */
            IFCport.DiscardInBuffer();
            return true;
        }

        public static bool IsCOMopen()
        {
            return IFCport.IsOpen;              
        }

        public static void closeCOM()
        {    
            IFCport.Close(); 
        }

        // not in use  - require LUT
        public static byte CalculateCrc8(byte[] data)
        {
            byte crc = 0;
            for (int i = 0; i < data.Length; i++)
            {
                crc ^= data[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x80) != 0)
                    {
                        crc = (byte)((crc << 1) ^ 0x07);
                    }
                    else
                    {
                        crc <<= 1;
                    }
                }
            }
            return crc;
        }
       
        internal static void send2IFCOM(byte[] char2sendIFC)
        {
            string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff ";
            if (IFCport.IsOpen)
            {
                IFCport.Write(char2sendIFC, 0, char2sendIFC.Length);
                Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "sent to IFC: " + System.Text.Encoding.UTF8.GetString(char2sendIFC, 0, char2sendIFC.Length));
                //char2sendIFC[0] = 0x40;
                //char2sendIFC[char2sendIFC.Length-1] = 0x0;                
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "data NOT sent to COM2 - IFC...");
            }
        }
        
        private static void IFCReceivedHandler(object senderIF, SerialDataReceivedEventArgs eIF)
        {
            string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff ";
            SerialPort spIF = (SerialPort)senderIF;
            Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "IFC COM: received, lenght: " + spIF.BytesToRead.ToString() + " B:" );
			CounterCOMReceived++;
			try
            {
                if (dataInBuffer.Length == 0)
                    dataInBuffer = spIF.ReadExisting();
                
                else
                    dataInBuffer = dataInBuffer + spIF.ReadExisting();
            }
            catch (Exception spE)
            {
                Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "DLL DataReceivedHandler: reading message failed: " + spE.Message);
                //log.Info("Decoding err failed: " + "," + spE.Message);
            }
            Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "IFC COM: buffer imput: " + dataInBuffer );


            int indexOfEndCHar = dataInBuffer.IndexOf('\u0013');
            string indataIFC = "";


            // process imput string if data lenght > 8 and End character exist inside the buffer data
            if (indexOfEndCHar > 8)
            {
                int indexOfStartCHar = dataInBuffer.IndexOf('\u0011');
                // put into indataIFC first command string
                if (indexOfStartCHar >= 0)
                {
                    // if in buffer are more data
                    indataIFC = dataInBuffer.Substring(indexOfStartCHar, (indexOfEndCHar - indexOfStartCHar + 1));
                }
                else
                {
                    // if in buffer is just one command string
                    indataIFC = dataInBuffer.Substring(0, indexOfEndCHar + 1);
                }

                // remove first command from dataInBuffer
                if (dataInBuffer.Length >= indexOfEndCHar)
                    dataInBuffer = dataInBuffer.Substring(indexOfEndCHar + 1, dataInBuffer.Length - indexOfEndCHar - 1);

                // if packet size of indataIFC is right go to parse/decode it
                if (indataIFC.Length == 8)
                {
                    Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "DLL DataReceivedHandler: " + spIF.BytesToRead.ToString() + "B string received : " + indataIFC);
                    IFVPCPacketParse(2, indataIFC); ;
                }
                else if (indataIFC.Length == 10)
                {
                    Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "DLL DataReceivedHandler: " + spIF.BytesToRead.ToString() + "B string received : " + indataIFC);
                    IFVPCPacketParse(2, indataIFC); ;
                }
                else if (indataIFC.Length == 23)
                {
                    IFVPCPacketParse(2, indataIFC);
                }
                else
                {
                    Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "DLL DataReceivedHandler: not short nor long message: %s", indataIFC);
                }

                // test if remaining data in dataInBuffer contains whole command
                indexOfStartCHar = dataInBuffer.IndexOf('\u0011');
                indexOfEndCHar = dataInBuffer.IndexOf('\u0013');

                // if there is not Start char -> clear the buffer 
                if (indexOfStartCHar < 0) dataInBuffer = dataInBuffer.Remove(0);

                // if there is Start and End char 
                if (indexOfStartCHar >= 0 && indexOfEndCHar >= 0)
                {
                    // remove all chars before Start char if End char is before Star char
                    if (indexOfStartCHar > indexOfEndCHar)
                        dataInBuffer = dataInBuffer.Remove(0, indexOfStartCHar + 1);
                    else
                    {
                        indataIFC = dataInBuffer.Substring(indexOfStartCHar, (indexOfEndCHar - indexOfStartCHar + 1));
                        if (dataInBuffer.Length >= indexOfEndCHar)
                            dataInBuffer = dataInBuffer.Substring(indexOfEndCHar + 1, dataInBuffer.Length - indexOfEndCHar - 1);
                        // if packet size of indataIFC is right go to parse/decode it
                        if (indataIFC.Length == 8 || indataIFC.Length == 10 || indataIFC.Length == 23)
                        {
                            Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "DataReceivedHandler2: " + spIF.BytesToRead.ToString() + "B string received : " + indataIFC);
                            IFVPCPacketParse(2, indataIFC); ;
                        }
                        else
                        {
                            Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "DataReceivedHandler2: not short nor long message: %s", indataIFC);
                        }
                    }
                }

                /*
                const int PACKETLENGHT = 23;
                if (spIF.BytesToRead >= PACKETLENGHT)
                {
                    try
                    {
                        string indataIFC = spIF.ReadTo("\u0013"); // read till Xoff
                        Console.WriteLine("IFC COM: string received: " + indataIFC);
                        IFVPCPacketParse(2, indataIFC);
                    }
                    catch (Exception eSerialHandlerIF)
                    {
                        Console.WriteLine("IF COM: error to find end of packet / image proc error");
                        Console.WriteLine(eSerialHandlerIF.Message);
                    }
                }

                if (spIF.BytesToRead == 8)
                {
                    try
                    {
                        string indataIFC = spIF.ReadTo("\u0013"); // read till Xoff
                        Console.WriteLine("IFC COM: reply: " + indataIFC);
                        IFVPCPacketParse(2, indataIFC);
                    }
                    catch (Exception eSerialHandlerIF)
                    {
                        _indataIFCBuffer = spIF.ReadExisting();
                        Console.WriteLine("IFC COM: error to find end of packet / image proc error");
                        Console.WriteLine(eSerialHandlerIF.Message);
                    }
                }
                if (spIF.BytesToRead == PACKETLENGHT - _indataIFCBuffer.Length && spIF.BytesToRead >= PACKETLENGHT)
                {
                    try
                    {
                        string indataIFC = spIF.ReadTo("\u0013"); // read till Xoff
                        Console.WriteLine("IFC COM: string received: " + indataIFC);
                        IFVPCPacketParse(2, indataIFC);
                    }
                    catch (Exception eSerialHandlerIF)
                    {
                        Console.WriteLine("IF COM: error to find end of packet 2nd time ");
                        Console.WriteLine(eSerialHandlerIF.Message);
                    }
                }
                */

                /* MM tagging update 
                string Operator = "1234567890";
                FormMain.fireImageProc(Operator);
                */

            }
        }

        internal static void IFVPCPacketParse(int portNo, string _indataVPC)
        {             // what we do with received data?
            string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff ";
            /* only for com port debugging 
                _indataVPC = "\x11ICM\u00000123045607892020\x13";
            */

            //< XON > IC “x” ”Ck” ”ID_TAG” ”M1” ”M2” ”M3” ”D”< XOFF >
            // <XON> ICMxK1111222233334444<XOFF>
            var log = new SimpleLogger(true);
            char[] sstrSA = new char[6];

            // setDefaultCoinData();
            // read com header
            _indataVPC.CopyTo(0, sstrSA, 0, 6);

            // com port SA asci data 
            const int deviceDataLenght = 9;
            char[] comData = new char[deviceDataLenght];
            
            int eStatus = 0;
            // test if header contains start char and IC token 
            if (sstrSA[0] == 17 && sstrSA[1] == 'I' && sstrSA[2] == 'C')
            {
                string tmpmessage = "";
                switch (sstrSA[3])
                {
                    
                    case 'M':
                        

                        // ///////////////////// !!! /////////////////////////////////////////////////
                        // situation when output to Kiosk is sent based on simulator is dangerouse !!!
                        // this has to be solved somehow in future, we keep it sending for debuging
                        //if (! ImageProc.FROMFILE)
                        if (s.isIdleMode() || s.isSaveMode())
                        {
                            operationMODE = 1;                            
                            tmpmessage = "TrigReceived";
                            ICMPacketParse(_indataVPC);
                            //result = DelegateFireImageProc(Operator);
                            string operatorX = sstrSA[5].ToString();
                            char[] sstrErr = new char[8];
                            CamGrab.bmpReadyF = false;
                            CamGrab.bmpReadyB = false;
                            CamGrab.bmpReadyT = false;
                            eStatus = operationMODE;
                            Main.SendCommand(eStatus, 'M');
							//FormMain.StartFirstTc();
		                    CounterCOMPacket++ ;
		                    Main.StartTCI();
                        }
                        else
                        {
                            tmpmessage = "TrigIgnored";
                            eStatus = operationMODE;
                            Main.SendCommand(eStatus, 'X');
                        }
                        break;

                    case 'T':
                        operationMODE = 2;
                        eStatus = 16;
                        eStatus = operationMODE;
                        Main.SendCommand(eStatus, 'T');
                        tmpmessage = "Accepting";
                        break;
                    case 'R':
                        operationMODE = 3;
                        eStatus = 0;
                        eStatus = operationMODE;
                        Main.SendCommand(eStatus, 'R');
                        tmpmessage = "Rejecting";
                        break;
                    case 'P':
                        operationMODE = 0;
                        eStatus = 16;
                        eStatus = operationMODE;
                        Main.SendCommand(eStatus, 'P');
                        Main.StopTc();
                        tmpmessage = "Stopping";
                        break;
                    case 'O':
                        tmpmessage = "Received status OK";
                        eStatus = operationMODE;
                        Main.SendCommand(eStatus, 'P');
                        break;
                    default:
                        operationMODE = 0;
                        // need to be checked
                        Main.StopTc();
                        break;
                }
                Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "IFC->VPC port: " + tmpmessage);
                log.Info("IFC->VPC port: "+ _indataVPC + " : " + tmpmessage );
                
            }
            else
            {
                Main.SendResultsN("NON000000000N12345678R999");
                Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "IFC port: Start pattern not identified...");
                log.Error("IFC port: Start byte not identified...");
            }
        }

        private static void ICMPacketParse(string _indataSA)
        {             // what we do with received _indataSA?
            string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff ";
            var log = new SimpleLogger(true);

            Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "IF SA packed parsing...");
            char[] sstr = new char[4];
            StringBuilder builder = new StringBuilder();

            //calculate the 
            int deviceDataLenght = 16;

            // get trigger cointer from the packet
            
            _indataSA.CopyTo(5, sstr, 0, 1);
            triggCounter = sstr[0];
            _indataSA.CopyTo(1, sstr, 0, 4);
            //sstr[4] is the packet coiunter

            //public static int[] parSA = new int[5];
            bool convertFailure = true;
            string tempstr = "";
            
            
            for (int indexer = 0; indexer < deviceDataLenght; indexer = indexer + 4)
            {
                if ((deviceDataLenght - indexer) > 3)
                {
                    _indataSA.CopyTo(indexer + 6, sstr, 0, 4);
                }
                else
                {
                    _indataSA.CopyTo(indexer + 6, sstr, 0, deviceDataLenght - indexer); // ???check the lenght
                }
                StringBuilder tempbuild = new StringBuilder();
                tempbuild.Append(sstr);
                tempstr = tempbuild.ToString();
                if (indexer < 17)
                {
                    convertFailure = int.TryParse(tempstr, out parSA[(indexer) / 4]);
                }
            }

            // store original values 
            parSAorig[0] = parSA[0]; parSAorig[1] = parSA[1]; parSAorig[2] = parSA[2]; parSAorig[3] = parSA[3]; parSAorig[4] = 0;
            // CIS & MAG data CALIBRATION
            // MAG1 calibration
            parSA[0] = (int)(Settings.SettingsItems.MG1A * (parSA[0] * parSA[0]) + Settings.SettingsItems.MG1B * parSA[0] + Settings.SettingsItems.MG1C);
            if (parSA[0] > 5000 || parSA[0] < 10)
            {
                parSA[0] = 10;
            }
            // MAG2 calibration
            parSA[1] = (int)(Settings.SettingsItems.MG2A * (parSA[1] * parSA[1]) + Settings.SettingsItems.MG2B * parSA[1] + Settings.SettingsItems.MG2C);
            if (parSA[1] > 5000 || parSA[1] < 10)
            {
                parSA[1] = 10;
            }
            // MAG3 calibration
            parSA[2] = (int)(Settings.SettingsItems.MG3A * (parSA[2] * parSA[2]) + Settings.SettingsItems.MG3B * parSA[2] + Settings.SettingsItems.MG3C);
            if (parSA[2] > 5000 || parSA[2] < 10)
            {
                parSA[2] = 10;
            }
            // diameter CIS calibration
            /* polynomyal 2nd grade regression
            parSA[3] = (int)(Settings.SettingsItems.RADA * (parSA[3]*parSA[3]) + Settings.SettingsItems.RADB * parSA[3] + Settings.SettingsItems.RADC);
            if (parSA[3] > 5000 || parSA[3] < 400)
            {
                parSA[3] = 400;
            }*/
            // linear regression with limit
            if (parSA[3] > Settings.SettingsItems.RADC)
            {
                PortDataReceived.largeRadius = true;
            }
            parSA[3] = (int)(Settings.SettingsItems.RADA * parSA[3] + Settings.SettingsItems.RADB );

            Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "IFSA packet decoded: " + parSA[3].ToString() +" "+ parSA[0].ToString() + " " + parSA[1].ToString() + " " + parSA[2].ToString());
            log.Trace("ICF packet decoded: " + parSA[3].ToString() + " " + parSA[0].ToString() + " " + parSA[1].ToString() + " " + parSA[2].ToString());
        }
    }
}
