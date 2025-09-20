
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace Cointero
{
    [Serializable]
    public class SettingsItems
    {
        public int DBUG { get; set; } // Run in Debug mode - Slow and saving images
        public int MODE { get; set; } // Reserved
        public string CNED { get; set; } // Camera1 identification string
        public string CNS1 { get; set; } // Camera2 identification string
        public string CNS2 { get; set; } // Camera3 identification string
        public string EXPT { get; set; } // Exposition time in us (12000) - not used yet
        public int TRNO { get; set; } // Threshold normal
        public int TRMI { get; set; } // Threshold minimal
        public int RESZ { get; set; } // Zoom factor - resize (shring) image by factor to speed up find rotation
        
        public int RADT { get; set; } // Radius tolerance
        public int MG1T { get; set; } // Magnetic sensor 1 tolerance
        public int MG2T { get; set; } // Magnetic sensor 2 tolerance
        public int MG3T { get; set; } // Magnetic sensor 3 tolerance

        public float RADA { get; set; } // Radius calibration a
        public float MG1A { get; set; } // Magnetic sensor 1 calibration a
        public float MG2A { get; set; } // Magnetic sensor 2 calibration a
        public float MG3A { get; set; } // Magnetic sensor 3 calibration a

        public float RADB { get; set; } // Radius calibration b
        public float MG1B { get; set; } // Magnetic sensor 1 calibration b
        public float MG2B { get; set; } // Magnetic sensor 2 calibration b
        public float MG3B { get; set; } // Magnetic sensor 3 calibration b

        public float RADC { get; set; } // Radius calibration c
        public float MG1C { get; set; } // Magnetic sensor 1 calibration c
        public float MG2C { get; set; } // Magnetic sensor 2 calibration c
        public float MG3C { get; set; } // Magnetic sensor 3 calibration c

        public int BLBS { get; set; } // Blob size in pixels 
        public int BLBL { get; set; } // Blob lenght in larger axis size
        public int BLBW { get; set; } // Blob widht size in axis perpendicularon larger axis

        public int EXPE { get; set; } // Exposure time edge
        public int EXP1 { get; set; } // Exposure time side 1
        public int EXP2 { get; set; } // Exposure time side 2  

        public int DTLO { get; set; } // Diameter Threshold Low
        public int DTHI { get; set; } // Diameter Threshold High 
        public int NOMO { get; set; } // Number of models 
        public float ASTE { get; set; } // Angle step in model rotation

        public int SLPT { get; set; } // Sleep time after simulator decoding coin from image 
        public int NMOP { get; set; } // Number of MOtor Pulses per one camera shot move

    }

    static public class Settings
    {
        public static string UserPath = "D:/COINTER/";
        public static string UserFilename = UserPath + "AuxFiles/CointeraSettings.xml";
        public static string UserFilenameCommon;        
        public static SettingsItems SettingsItems;

        public struct sROI
        {
            public int UP;
            public int DOWN;
            public int LEFT;
            public int RIGHT;

            public int X1;
            public int Y1;
            public int X2;
            public int Y2;

            public int STEP;
            public bool SIZE;
            public bool LOAD;
        }
        public static sROI setROI;


        public static void Load()
        {
            string[] tempString;
            string tempStr;
            tempStr = Path.GetFileName(UserFilename);
            tempString = tempStr.Split('.');
            UserFilenameCommon = tempString[0];

            XmlSerializer serializer = new XmlSerializer(typeof(SettingsItems));
            FileStream fs = new FileStream(UserFilename, FileMode.Open);
            try
            {
                SettingsItems = (SettingsItems)serializer.Deserialize(fs);
            }
            catch (Exception)
            {

            }
            fs.Close();            
        }

        public static void Save()
        {
            XmlSerializer x = new XmlSerializer(typeof(SettingsItems));
            TextWriter writer = new StreamWriter(UserFilename);
            x.Serialize(writer, SettingsItems);
            writer.Close();
        }
    }
}
