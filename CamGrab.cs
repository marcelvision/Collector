using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading ;
using MvCamCtrl.NET;
using MvCamCtrl.NET.CameraParams;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.ComponentModel.Design;
using System.Runtime.Serialization;


namespace Cointero
{
    class CamGrab
    {
        #region init variables
        private static cbOutputExdelegate ImageCallbackF;
        private static cbOutputExdelegate ImageCallbackB;
        private static cbOutputExdelegate ImageCallbackT;
        string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff ";

        public static bool m_bGrabbing = false;
        public static int IcountF = 0;
        public static int IcountB = 0;
        public static int IcountT = 0;
        //private uint TrigerMode = (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF;
        private static uint TrigerMode = (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON;

        public static CCamera m_FrontCamera = new CCamera();
        public static CCamera m_BackCamera = new CCamera();
        public static CCamera m_TopCamera = new CCamera();

        private static int nRet = CErrorDefine.MV_OK;
        private static bool m_bIsDeviceOpen = false;       // ch:设备打开状态 | en:Is device open
        // ch:Bitmap | en:Bitmap
        private static Bitmap m_pcBitmapT = null;
        public static Bitmap m_Bitmap2ShowT = null;
        public static bool bmpReadyT = false;
        private static PixelFormat m_enBitmapPixelFormatT = PixelFormat.DontCare;

        private static Bitmap m_pcBitmapF = null;
        public static Bitmap m_Bitmap2ShowF = null;
        public static bool bmpReadyF = false;
        private static PixelFormat m_enBitmapPixelFormatF = PixelFormat.DontCare;

        private static Bitmap m_pcBitmapB = null;
        public static Bitmap m_Bitmap2ShowB = null;
        public static bool bmpReadyB = false;
        private static PixelFormat m_enBitmapPixelFormatB = PixelFormat.DontCare;

        public static SimpleLogger logDr = new SimpleLogger(true);
        #endregion

        // Camera call back functions
        static void ImageCallbackFuncF(IntPtr pDataF, ref MV_FRAME_OUT_INFO_EX pFrameInfoF, IntPtr pUserF)
        {
            string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff ";            
            IcountF++;
            string infoMessage = "ImageCallbackFuncF : frame " + pFrameInfoF.nFrameNum.ToString() + "/" + IcountF.ToString() + " Width[" + Convert.ToString(pFrameInfoF.nWidth) + "] , Height[" + Convert.ToString(pFrameInfoF.nHeight) + "] , FrameNu" + "m[" + Convert.ToString(pFrameInfoF.nFrameNum) + "]";
            Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + infoMessage);
            logDr.Info(infoMessage);

            // CALLBACK FRONT CAMERA
            // Create bmp from pointer
            //
            Bitmap bmp = new Bitmap(pFrameInfoF.nWidth, pFrameInfoF.nHeight, pFrameInfoF.nWidth * 1, PixelFormat.Format8bppIndexed, pDataF);
            m_Bitmap2ShowF = (Bitmap)bmp.Clone();

            ColorPalette palette = m_Bitmap2ShowF.Palette;
            for (int i = 0; i < palette.Entries.Length; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            m_Bitmap2ShowF.Palette = palette;
            bmpReadyF = true;

            infoMessage = "ImageCallbackFuncF : Front camera image acquired " + pFrameInfoF.nFrameLen.ToString() + " B and converted";
            Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + infoMessage);
            logDr.Info(infoMessage);
            
        }

        static void ImageCallbackFuncB(IntPtr pDataB, ref MV_FRAME_OUT_INFO_EX pFrameInfoB, IntPtr pUserB)
        {
            string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff "; 
            IcountB++;
            string infoMessage = "ImageCallbackFuncB : frame " + pFrameInfoB.nFrameNum.ToString() + "/" + IcountB.ToString() + " Width[" + Convert.ToString(pFrameInfoB.nWidth) + "] , Height[" + Convert.ToString(pFrameInfoB.nHeight) + "] , FrameNu" + "m[" + Convert.ToString(pFrameInfoB.nFrameNum) + "]";
            Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + infoMessage);
            logDr.Info(infoMessage);
            // CALLBACK FRONT BACK
            // Create bmp from pointer
            //

            Bitmap bmp = new Bitmap(pFrameInfoB.nWidth, pFrameInfoB.nHeight, pFrameInfoB.nWidth * 1, PixelFormat.Format8bppIndexed, pDataB);
            m_Bitmap2ShowB = (Bitmap)bmp.Clone();

            ColorPalette palette = m_Bitmap2ShowB.Palette;
            for (int i = 0; i < palette.Entries.Length; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            m_Bitmap2ShowB.Palette = palette;
            bmpReadyB = true;

            //NecessaryOperBeforeGrab();
            //stopGrabbingB();
            infoMessage = "ImageCallbackFuncB : Back camera image acquired " + pFrameInfoB.nFrameLen.ToString() + " B and converted";
            Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + infoMessage);
            logDr.Info(infoMessage);
        }

        static void ImageCallbackFuncT(IntPtr pDataT, ref MV_FRAME_OUT_INFO_EX pFrameInfoT, IntPtr pUserT)      
        {
            IcountT++;
            string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff ";
            string infoMessage = "ImageCallbackFuncT : frame " + pFrameInfoT.nFrameNum.ToString() + "/" + IcountT.ToString() + " Width[" + Convert.ToString(pFrameInfoT.nWidth) + "] , Height[" + Convert.ToString(pFrameInfoT.nHeight) + "] , FrameNu" + "m[" + Convert.ToString(pFrameInfoT.nFrameNum) + "]";
            Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + infoMessage);
            logDr.Info(infoMessage);
            // CALLBACK FRONT CAMERA
            // Create bmp from pointer
            //

            Bitmap bmp = new Bitmap(pFrameInfoT.nWidth, pFrameInfoT.nHeight, pFrameInfoT.nWidth * 3, PixelFormat.Format24bppRgb, pDataT);
            //Bitmap bmp = new Bitmap(pFrameInfoT.nWidth, pFrameInfoT.nHeight, pFrameInfoT.nWidth * 4, PixelFormat.Format32bppRgb, pDataT);
            //Bitmap bmp2 = new Bitmap(pFrameInfoT.nWidth, pFrameInfoT.nHeight, pFrameInfoT.nWidth * 1, PixelFormat.Format24bppRgb, pDataT);
            m_Bitmap2ShowT = (Bitmap)bmp.Clone();

            ConvertRgbToBgrFast(m_Bitmap2ShowT);

            bmpReadyT = true;
            infoMessage = "ImageCallbackFuncT : Top camera image acquired " + pFrameInfoT.nFrameLen.ToString() + " B and converted";
            Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + infoMessage);
            logDr.Info(infoMessage);

            /*
            ColorPalette palette = m_Bitmap2ShowT.Palette;
            for (int i = 0; i < palette.Entries.Length; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            m_Bitmap2ShowT.Palette = palette;
            */

            /*
            //NecessaryOperBeforeGrab();
            */
            //stopGrabbingT();
        }

        static void ConvertRgbToBgrFast(Bitmap bitmap)
        {
            // Lock the bitmap's bits
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * bitmap.Height;
            byte[] pixelBuffer = new byte[byteCount];

            // Copy pixel data to buffer
            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, pixelBuffer, 0, byteCount);

            // Process each pixel
            for (int i = 0; i < pixelBuffer.Length; i += bytesPerPixel)
            {
                // Swap the Red and Blue channels (assumes format is BGR or BGRA)
                byte temp = pixelBuffer[i];        // Blue
                pixelBuffer[i] = pixelBuffer[i + 2]; // Red
                pixelBuffer[i + 2] = temp;        // Swap
            }

            // Copy modified buffer back to bitmap
            System.Runtime.InteropServices.Marshal.Copy(pixelBuffer, 0, bitmapData.Scan0, byteCount);

            // Unlock the bits
            bitmap.UnlockBits(bitmapData);
        }

        // ch:像素类型是否为Mono格式 | en:If Pixel Type is Mono 
        private static Boolean IsMono(MvGvspPixelType enPixelType)
        {
            switch (enPixelType)
            {
                case MvGvspPixelType.PixelType_Gvsp_Mono1p:
                case MvGvspPixelType.PixelType_Gvsp_Mono2p:
                case MvGvspPixelType.PixelType_Gvsp_Mono4p:
                case MvGvspPixelType.PixelType_Gvsp_Mono8:
                case MvGvspPixelType.PixelType_Gvsp_Mono8_Signed:
                case MvGvspPixelType.PixelType_Gvsp_Mono10:
                case MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case MvGvspPixelType.PixelType_Gvsp_Mono12:
                case MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                case MvGvspPixelType.PixelType_Gvsp_Mono14:
                case MvGvspPixelType.PixelType_Gvsp_Mono16:
                    return true;
                default:
                    return false;
            }
        }
        
        public static int checkCameras()
        {
            CEnumValue Triggermode = new CEnumValue();
            CEnumValue Triggermodesum = new CEnumValue();
            int nRetFront = 0;
            int nRetBack = 0;
            int nRetTop = 0;

            nRetFront = m_FrontCamera.GetEnumValue("TriggerMode", ref Triggermode);
            Triggermodesum = Triggermode;
            nRetBack = m_BackCamera.GetEnumValue("TriggerMode", ref Triggermode);
            Triggermodesum = Triggermode;
            nRetTop = m_TopCamera.GetEnumValue("TriggerMode", ref Triggermode);
            Triggermodesum = Triggermode;
            
            if (CErrorDefine.MV_OK != (nRetFront+ nRetBack + nRetTop))
            {
                //Console.WriteLine("Get TriggerMode failed:{0:x8}", nRet);                
            }
            return (nRetFront + nRetBack + nRetTop);
        }
        
        public static void getCameras()
        {
            string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff ";
            do
            {
                List<CCameraInfo> ltDeviceList = new List<CCameraInfo>();

                // ch:枚举设备 | en:Enum device
                nRet = CSystem.EnumDevices(CSystem.MV_GIGE_DEVICE | CSystem.MV_USB_DEVICE, ref ltDeviceList);
                if (CErrorDefine.MV_OK != nRet)
                {
                    Console.WriteLine("Enum device failed:{0:x8}", nRet);
                    break;
                }
                Console.WriteLine("Enum device count : " + Convert.ToString(ltDeviceList.Count));
                if (0 == ltDeviceList.Count)
                {
                    break;
                }

                // ch:打印设备信息 en:Print device info
                string[] camName = new string[ltDeviceList.Count];
                for (int i = 0; i < ltDeviceList.Count; i++)
                {
                    if (CSystem.MV_GIGE_DEVICE == ltDeviceList[i].nTLayerType)
                    {
                        CGigECameraInfo cGigEDeviceInfo = (CGigECameraInfo)ltDeviceList[i];

                        uint nIp1 = ((cGigEDeviceInfo.nCurrentIp & 0xff000000) >> 24);
                        uint nIp2 = ((cGigEDeviceInfo.nCurrentIp & 0x00ff0000) >> 16);
                        uint nIp3 = ((cGigEDeviceInfo.nCurrentIp & 0x0000ff00) >> 8);
                        uint nIp4 = (cGigEDeviceInfo.nCurrentIp & 0x000000ff);

                        Console.WriteLine("[device " + i.ToString() + "]:");
                        Console.WriteLine("  DevIP:" + nIp1 + "." + nIp2 + "." + nIp3 + "." + nIp4);
                        Console.WriteLine("  ManufacturerName:" + cGigEDeviceInfo.chManufacturerName );
                        Console.WriteLine("  Sn:" + cGigEDeviceInfo.chSerialNumber );
                        /*
                         * System.ArgumentException: ''GB2312' is not a supported encoding name. For information on defining a custom encoding, see the documentation for the Encoding.RegisterProvider method. (Parameter 'name')'
                        */
                        var stringBuilder = new StringBuilder();
                        for (int j=0; j<6; j++)
                        {
                            if (0 != cGigEDeviceInfo.chUserDefinedName[j])
                                stringBuilder.Append((char)cGigEDeviceInfo.chUserDefinedName[j]);
                        }
                        if (stringBuilder.Length > 0)
                        {
                            camName[i] = stringBuilder.ToString();
                            Console.WriteLine("  UserDefineName:" + stringBuilder + "\n");
                        }
                        else
                        {
                            camName[i] = "nd";
                        }
                    }
                }

                int nGrabersInitialised = 0;
                Console.WriteLine("\n Selected cameras:");
                for  (int nDevIndex = ltDeviceList.Count -1; nDevIndex >= 0;  nDevIndex--)
                {
                    var stringBuilder = new StringBuilder();
                    CGigECameraInfo cGigEDeviceInfo = (CGigECameraInfo)ltDeviceList[nDevIndex];
                    //Console.WriteLine(ltDeviceList[1].ToString());   
                    CCameraInfo camDevice = ltDeviceList[nDevIndex];
                    //Console.WriteLine(camDevice.GetType());
                    stringBuilder.Append(camName[nDevIndex]+" ");
                    stringBuilder.Append(cGigEDeviceInfo.chSerialNumber + " ");
                    stringBuilder.Append(cGigEDeviceInfo.chModelName + " ");                    
                    stringBuilder.Append("Major version " + camDevice.nMajorVer);
                    stringBuilder.Append(" MAC address low " + camDevice.nMacAddrLow );

                    // filter out wifi network , or accept only particular network  3232248397 (192.168.50.0) in this case 
                    if (cGigEDeviceInfo.chUserDefinedName[0] == 'F' /*&& cGigEDeviceInfo.nNetExport == 3232248397*/)
                    //if (nDevIndex == 2)
                    {
                        Console.WriteLine(stringBuilder);
                        // ch:获取选择的设备信息 | en:Get selected device information
                        CCameraInfo camFDevice = ltDeviceList[nDevIndex];
                        // ch:创建设备 | en:Create device
                        nRet = m_FrontCamera.CreateHandle(ref camFDevice);
                        if (CErrorDefine.MV_OK != nRet)
                        {
                            Console.WriteLine("Create device failed:{0:x8}", nRet);
                            break;
                        }
                        // ch:打开设备 | en:Open device
                        Console.WriteLine("CamGrab:opening Device: Front camera ");
                        nRet = m_FrontCamera.OpenDevice();
                        if (CErrorDefine.MV_OK != nRet)
                        {

                            nRet = m_FrontCamera.CloseDevice();
                            if (CErrorDefine.MV_OK != nRet)
                            {
                                Console.WriteLine("Close device failed:{0:x8}", nRet);
                            }
                            // ch:销毁设备 | en:Destroy device
                            nRet = m_FrontCamera.DestroyHandle();
                            if (CErrorDefine.MV_OK != nRet)
                            {
                                Console.WriteLine("Destroy device failed:{0:x8}", nRet);
                            }
                            nRet = m_FrontCamera.OpenDevice();
                            if (CErrorDefine.MV_OK != nRet)
                            {
                                Console.WriteLine("Open device failed:{0:x8}", nRet);
                                break;
                            }
                        }                        

                        // ch:设置触发模式为off || en:set trigger mode as off
                        nRet = m_FrontCamera.SetEnumValue("TriggerMode", TrigerMode);
                        if (CErrorDefine.MV_OK != nRet)
                        {
                            Console.WriteLine("Set TriggerMode failed:{0:x8}", nRet);
                            break;
                        }

                        // en:set exposure time
                        /*
                        nRet = m_FrontCamera.SetFloatValue("ExposureTime", 100);
                        if (CErrorDefine.MV_OK != nRet)
                        {
                            Console.WriteLine("Set TriggerMode failed:{0:x8}", nRet);
                            break;
                        }
                        */

                        // ch:注册回调函数 | en:Register image callback
                        ImageCallbackF = new cbOutputExdelegate(ImageCallbackFuncF);
                        nRet = m_FrontCamera.RegisterImageCallBackEx(ImageCallbackF, IntPtr.Zero);
                        if (CErrorDefine.MV_OK != nRet)
                        {
                            Console.WriteLine("Register image callback failed!");
                            break;
                        }
                        else nGrabersInitialised++;
                    }
                    if (cGigEDeviceInfo.chUserDefinedName[0] == 'B' /*&& cGigEDeviceInfo.nNetExport == 3232248397*/)
                    //if (nDevIndex == 1)
                    {
                        Console.WriteLine(stringBuilder);
                        // Camera back 
                        // ch:获取选择的设备信息 | en:Get selected device information
                        CCameraInfo camBDevice = ltDeviceList[nDevIndex];
                        // ch:创建设备 | en:Create device
                        nRet = m_BackCamera.CreateHandle(ref camBDevice);
                        if (CErrorDefine.MV_OK != nRet)
                        {
                            Console.WriteLine("Create device failed:{0:x8}", nRet);
                            break;
                        }
                        // ch:打开设备 | en:Open device
                        Console.WriteLine("CamGrab:opening Device: Back camera ");
                        nRet = m_BackCamera.OpenDevice();
                        if (CErrorDefine.MV_OK != nRet)
                        {

                            nRet = m_BackCamera.CloseDevice();
                            if (CErrorDefine.MV_OK != nRet)
                            {
                                Console.WriteLine("Close device failed:{0:x8}", nRet);
                            }
                            m_bIsDeviceOpen = false;

                            // ch:销毁设备 | en:Destroy device
                            nRet = m_BackCamera.DestroyHandle();
                            if (CErrorDefine.MV_OK != nRet)
                            {
                                Console.WriteLine("Destroy device failed:{0:x8}", nRet);
                            }

                            nRet = m_BackCamera.OpenDevice();
                            if (CErrorDefine.MV_OK != nRet)
                            {
                                Console.WriteLine("Open device failed:{0:x8}", nRet);
                                break;
                            }
                        }

                        // ch:设置触发模式为off || en:set trigger mode as off
                        nRet = m_BackCamera.SetEnumValue("TriggerMode", TrigerMode);
                        if (CErrorDefine.MV_OK != nRet)
                        {
                            Console.WriteLine("Set TriggerMode failed:{0:x8}", nRet);
                            break;
                        }

                        // en:set exposure time
                        /* 
                        nRet = m_BackCamera.SetFloatValue("ExposureTime", 100);
                        if (CErrorDefine.MV_OK != nRet)
                        {
                            Console.WriteLine("Set TriggerMode failed:{0:x8}", nRet);
                            break;
                        }
                        */

                        // ch:注册回调函数 | en:Register image callback
                        ImageCallbackB = new cbOutputExdelegate(ImageCallbackFuncB);
                        nRet = m_BackCamera.RegisterImageCallBackEx(ImageCallbackB, IntPtr.Zero);
                        if (CErrorDefine.MV_OK != nRet)
                        {
                            Console.WriteLine("Register image callback failed!");
                            break;
                        }
                        else nGrabersInitialised++;

                    }
                    if (cGigEDeviceInfo.chUserDefinedName[0] == 'T' /* && cGigEDeviceInfo.nNetExport == 3232248397*/)
                    //if (nDevIndex == 0)
                    {
                        Console.WriteLine(stringBuilder);
                        // ch:获取选择的设备信息 | en:Get selected device information
                        CCameraInfo camTDevice = ltDeviceList[nDevIndex];
                        // ch:创建设备 | en:Create device
                        nRet = m_TopCamera.CreateHandle(ref camTDevice);
                        if (CErrorDefine.MV_OK != nRet)
                        {
                            Console.WriteLine("Create device failed:{0:x8}", nRet);
                            break;
                        }
                        // ch:打开设备 | en:Open device
                        Console.WriteLine("CamGrab:opening Device: Top camera ");
                        nRet = m_TopCamera.OpenDevice();
                        if (CErrorDefine.MV_OK != nRet)
                        {

                            nRet = m_TopCamera.CloseDevice();
                            if (CErrorDefine.MV_OK != nRet)
                            {
                                Console.WriteLine("Close device failed:{0:x8}", nRet);
                            }
                            m_bIsDeviceOpen = false;

                            // ch:销毁设备 | en:Destroy device
                            nRet = m_TopCamera.DestroyHandle();
                            if (CErrorDefine.MV_OK != nRet)
                            {
                                Console.WriteLine("Destroy device failed:{0:x8}", nRet);
                            }

                            nRet = m_TopCamera.OpenDevice();
                            if (CErrorDefine.MV_OK != nRet)
                            {
                                Console.WriteLine("Open device failed:{0:x8}", nRet);
                                break;
                            }
                        }
                        // ch:设置触发模式为off || en:set trigger mode as off
                        nRet = m_TopCamera.SetEnumValue("TriggerMode", TrigerMode);
                        if (CErrorDefine.MV_OK != nRet)
                        {
                            Console.WriteLine("Set TriggerMode failed:{0:x8}", nRet);
                            break;
                        }

                        // en:set exposure time
                        /*
                        nRet = m_TopCamera.SetFloatValue("ExposureTime", 5000);
                        if (CErrorDefine.MV_OK != nRet)
                        {
                            Console.WriteLine("Set TriggerMode failed:{0:x8}", nRet);
                            break;
                        }
                        */
                        // ch:注册回调函数 | en:Register image callback
                        ImageCallbackT = new cbOutputExdelegate(ImageCallbackFuncT);
                        nRet = m_TopCamera.RegisterImageCallBackForRGB(ImageCallbackT, IntPtr.Zero);
                        if (CErrorDefine.MV_OK != nRet)
                        {
                            Console.WriteLine("Register image callback failed!");
                            break;
                        }
                        else nGrabersInitialised++;

                        /*
                        // ch:取像素格式 | en:Get Pixel Format
                        CEnumValue pcPixelFormat = new CEnumValue();
                        nRet = m_FrontCamera.GetEnumValue("PixelFormat", ref pcPixelFormat);
                        if (CErrorDefine.MV_OK != nRet)
                        {
                            Console.WriteLine("Get Pixel Format Fail!", nRet);
                            break ;
                        }
                        
                        Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "Pixel Format " + pcPixelFormat.CurValue.ToString());
                        Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "Pixel Format " + pcPixelFormat.SupportedNum.ToString());
                        Console.WriteLine(DateTime.Now.ToString(datetimeFormat) + "Pixel Format " + pcPixelFormat.SupportValue.ToString() );
                        // ch:获取选择的设备信息 | en:Get selected device information
                        CCameraInfo camFDevice = ltDeviceList[nDevIndex];

                        // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
                        if (CSystem.MV_GIGE_DEVICE == camFDevice.nTLayerType)
                        {
                            int nPacketSize = m_FrontCamera.GIGE_GetOptimalPacketSize();
                            if (nPacketSize > 0)
                            {
                                nRet = m_FrontCamera.SetIntValue("GevSCPSPacketSize", (uint)nPacketSize);
                                if (CErrorDefine.MV_OK != nRet)
                                {
                                    Console.WriteLine("Warning: Set Packet Size failed {0:x8}", nRet);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Warning: Get Packet Size failed {0:x8}", nPacketSize);
                            }
                        }
                        if (CSystem.MV_GIGE_DEVICE == camFDevice.nTLayerType)
                        {
                            int nPacketSize = m_BackCamera.GIGE_GetOptimalPacketSize();
                            if (nPacketSize > 0)
                            {
                                nRet = m_BackCamera.SetIntValue("GevSCPSPacketSize", (uint)nPacketSize);
                                if (CErrorDefine.MV_OK != nRet)
                                {
                                    Console.WriteLine("Warning: Set Packet Size failed {0:x8}", nRet);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Warning: Get Packet Size failed {0:x8}", nPacketSize);
                            }
                        }
                        if (CSystem.MV_GIGE_DEVICE == camFDevice.nTLayerType)
                        {
                            int nPacketSize = m_TopCamera.GIGE_GetOptimalPacketSize();
                            if (nPacketSize > 0)
                            {
                                nRet = m_TopCamera.SetIntValue("GevSCPSPacketSize", (uint)nPacketSize);
                                if (CErrorDefine.MV_OK != nRet)
                                {
                                    Console.WriteLine("Warning: Set Packet Size failed {0:x8}", nRet);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Warning: Get Packet Size failed {0:x8}", nPacketSize);
                            }
                        }*/
                    }
                    /*
                    // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
                    if (CSystem.MV_GIGE_DEVICE == camFDevice.nTLayerType)
                    {
                        int nPacketSize = m_FrontCamera.GIGE_GetOptimalPacketSize();
                        if (nPacketSize > 0)
                        {
                            nRet = m_FrontCamera.SetIntValue("GevSCPSPacketSize", (uint)nPacketSize);
                            if (CErrorDefine.MV_OK != nRet)
                            {
                                Console.WriteLine("Warning: Set Packet Size failed {0:x8}", nRet);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Warning: Get Packet Size failed {0:x8}", nPacketSize);
                        }
                    }
                    if (CSystem.MV_GIGE_DEVICE == camFDevice.nTLayerType)
                    {
                        int nPacketSize = m_BackCamera.GIGE_GetOptimalPacketSize();
                        if (nPacketSize > 0)
                        {
                            nRet = m_BackCamera.SetIntValue("GevSCPSPacketSize", (uint)nPacketSize);
                            if (CErrorDefine.MV_OK != nRet)
                            {
                                Console.WriteLine("Warning: Set Packet Size failed {0:x8}", nRet);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Warning: Get Packet Size failed {0:x8}", nPacketSize);
                        }
                    }
                    if (CSystem.MV_GIGE_DEVICE == camFDevice.nTLayerType)
                    {
                        int nPacketSize = m_TopCamera.GIGE_GetOptimalPacketSize();
                        if (nPacketSize > 0)
                        {
                            nRet = m_TopCamera.SetIntValue("GevSCPSPacketSize", (uint)nPacketSize);
                            if (CErrorDefine.MV_OK != nRet)
                            {
                                Console.WriteLine("Warning: Set Packet Size failed {0:x8}", nRet);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Warning: Get Packet Size failed {0:x8}", nPacketSize);
                        }
                    }
                    */
                }
                Console.WriteLine("Init Framegrabbers has finished " + nGrabersInitialised.ToString() + " grabers active");
                if (nGrabersInitialised==3)
                {
                    m_bIsDeviceOpen = true;
                    startGrabbing();
                    m_bGrabbing = true;

                }
                else
                {
                    destroyGrabbers();
                }
                
            } while (false);
        }        
        
        public static void stopGrabbingT()
        {
            // ch:停止抓图 | en:Stop grabbing
            Console.WriteLine("Stop grabbing Top camera ");
            nRet = m_TopCamera.StopGrabbing();
            if (CErrorDefine.MV_OK != nRet)
            {
                Console.WriteLine("Stop grabbing T failed:{0:x8}", nRet);
            }

        }

        public static void stopGrabbingF()
        {
            // ch:停止抓图 | en:Stop grabbing
            Console.WriteLine("Stop grabbing Front camera ");            
            nRet = m_FrontCamera.StopGrabbing();
            if (CErrorDefine.MV_OK != nRet)
            {
                Console.WriteLine("Stop grabbing F failed:{0:x8}", nRet);
            }
        }

        public static void stopGrabbingB()
        {
            // ch:停止抓图 | en:Stop grabbing
            Console.WriteLine("Stop grabbing Back camera ");
            nRet = m_BackCamera.StopGrabbing();
            if (CErrorDefine.MV_OK != nRet)
            {
                Console.WriteLine("Stop grabbing B failed:{0:x8}", nRet);
            }
        }

        public static void startGrabbing()
        {
            // ch:开启抓图 || en: start grab image
            try
            {
                nRet = m_FrontCamera.StartGrabbing();
                if (CErrorDefine.MV_OK != nRet)
                {
                    Console.WriteLine("Start grabbing failed:{0:x8}", nRet);
                }
                else Console.WriteLine("Front camera grabbing");


                nRet = m_BackCamera.StartGrabbing();
                if (CErrorDefine.MV_OK != nRet)
                {
                    Console.WriteLine("Start grabbing failed:{0:x8}", nRet);
                }
                else Console.WriteLine("Back camera grabbing");

                nRet = m_TopCamera.StartGrabbing();
                if (CErrorDefine.MV_OK != nRet)
                {
                    Console.WriteLine("Start grabbing failed:{0:x8}", nRet);
                }
                else Console.WriteLine("Top camera grabbing");
            }
            catch (Exception e)
            {
                Console.WriteLine("catch at startGrabbing: " + e.Message);
            }
        }

        public static void destroyGrabbers()
        {

            logDr.Info("destroyGrabbers: started");
            // ch:关闭设备 | en:Close device
            Console.WriteLine("Stop grabbing Front camera ");
            nRet = m_FrontCamera.StopGrabbing();
            if (CErrorDefine.MV_OK != nRet)
            {
                Console.WriteLine("No camera / Stop grabbing F f:{0:x8}", nRet);
            }
            else
            {
                nRet = m_FrontCamera.CloseDevice();
                if (CErrorDefine.MV_OK != nRet)
                {
                    Console.WriteLine("Close device failed:{0:x8}", nRet);
                }
                else
                {
                    // ch:销毁设备 | en:Destroy device
                    nRet = m_FrontCamera.DestroyHandle();
                    if (CErrorDefine.MV_OK != nRet)
                    {
                        Console.WriteLine("Destroy device failed:{0:x8}", nRet);
                    }
                }
            }

            Console.WriteLine("Stop grabbing Back camera ");
            nRet = m_BackCamera.StopGrabbing();
            if (CErrorDefine.MV_OK != nRet)
            {
                Console.WriteLine("No camera / Stop grabbing B f:{0:x8}", nRet);
            }
            else
            {
                nRet = m_BackCamera.CloseDevice();
                if (CErrorDefine.MV_OK != nRet)
                {
                    Console.WriteLine("Close device failed:{0:x8}", nRet);
                }
                else
                {
                    // ch:销毁设备 | en:Destroy device
                    nRet = m_BackCamera.DestroyHandle();
                    if (CErrorDefine.MV_OK != nRet)
                    {
                        Console.WriteLine("Destroy device failed:{0:x8}", nRet);
                    }
                }
            }

            Console.WriteLine("Stop grabbing Top camera ");
            nRet = m_TopCamera.StopGrabbing();
            if (CErrorDefine.MV_OK != nRet)
            {
                Console.WriteLine("No camera / Stop grabbing T f:{0:x8}", nRet);
            }
            else
            {
                nRet = m_TopCamera.CloseDevice();
                if (CErrorDefine.MV_OK != nRet)
                {
                    Console.WriteLine("Close device failed:{0:x8}", nRet);
                }
                else
                {
                    // ch:销毁设备 | en:Destroy device
                    nRet = m_TopCamera.DestroyHandle();
                    if (CErrorDefine.MV_OK != nRet)
                    {
                        Console.WriteLine("Destroy device failed:{0:x8}", nRet);
                    }
                }
            }
            m_bIsDeviceOpen = false;            
            m_bGrabbing = false;
            logDr.Info("destroyGrabbers: finished");
        }        
    }
}
