using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cointero
{
    public partial class FormImageMan : Form
    {
        public const string SAVED_IMAGES_DIRECTORY = "D:\\COINTER\\SavedImages\\";
        public const string MODEL_IMAGES_DIRECTORY = "D:\\COINTER\\Images\\ModelsN\\";
        public string selectedCoinName = "";
        string[] cs_FileNames;
        public bool changeRunList = false;
        public List<string> run_FileNamesE = new List<string>();
        public List<string> run_FileNames1 = new List<string>();
        public List<string> run_FileNames2 = new List<string>();
        public FormImageMan()
        {
            InitializeComponent();

            // list of all images in SavedImages directory
            Update_SavedImages_List();
            Update_Models_List();

        }

        private void Update_Models_List(string selectedCoinName)
        {

            run_FileNamesE = new List<string>();
            run_FileNames1 = new List<string>();
            run_FileNames2 = new List<string>();

            string[] m_filesE = Directory.GetFiles(selectedCoinName + "\\Edges");
            string[] m_files1 = Directory.GetFiles(selectedCoinName + "\\Side1");
            string[] m_files2 = Directory.GetFiles(selectedCoinName + "\\Side2");

            foreach (string m_fileE in m_filesE) run_FileNamesE.Add(m_fileE);
            foreach (string m_file1 in m_files1) run_FileNames1.Add(m_file1);
            foreach (string m_file2 in m_files2) run_FileNames2.Add(m_file2);

            textBoxAllFiles.Text = run_FileNamesE.Count.ToString();
            textBoxNoModels.Text = "1";
        }

        private void Update_Models_List()
        {

            string m_uniqueName = "";
            StringBuilder builder = new StringBuilder();
            string[] m_directories = Directory.GetDirectories(MODEL_IMAGES_DIRECTORY);
            string[] m_models = Directory.GetFiles(MODEL_IMAGES_DIRECTORY);
            listBoxModels.Items.Clear();
            //if (m_directories.Count > int(0))
            //{
            run_FileNamesE = new List<string>();
            run_FileNames1 = new List<string>();
            run_FileNames2 = new List<string>();
            //}
            foreach (string directory in m_directories)
            {
                string m_Name = StripCoinName(directory);
                listBoxModels.Items.Add(m_Name);
                string[] m_filesE = Directory.GetFiles(directory + "\\Edges");
                string[] m_files1 = Directory.GetFiles(directory + "\\Side1");
                string[] m_files2 = Directory.GetFiles(directory + "\\Side2");

                // temoporary code correct diameter on previousely saved test samples
                // CHF EUR GBP converted on 19 8 2025
                /*
                foreach (string m_model in m_models)
                {
                    if (m_model.Contains(m_Name))
                    {
                        string model_name_split = m_model.Split('_')[3];
                        
                        for(int i = 0; i<m_filesE.Length; i++)
                        
                        {
                            string diam_in_name = m_filesE[i];
                            string e_files_split = diam_in_name.Split('_')[3];
                            
                            int modelDiam  = int.Parse(model_name_split);
                            int testDiam = int.Parse(e_files_split);

                            int orig_testDiam = (int)((testDiam - 1500) / 0.555);
                            modelDiam = (int)(1521 + (orig_testDiam * 0.5571));
                            model_name_split = modelDiam.ToString("D" + 4);

                            string new_diam_in_placeE = m_filesE[i].Replace(e_files_split, model_name_split);
                            File.Move(m_filesE[i], new_diam_in_placeE);

                            string new_diam_in_place1 = m_files1[i].Replace(e_files_split, model_name_split);
                            File.Move(m_files1[i], new_diam_in_place1);

                            string new_diam_in_place2 = m_files2[i].Replace(e_files_split, model_name_split);
                            File.Move(m_files2[i], new_diam_in_place2);

                        }
                    }
                }*/

                foreach (string m_fileE in m_filesE) run_FileNamesE.Add(m_fileE);
                foreach (string m_file1 in m_files1) run_FileNames1.Add(m_file1);
                foreach (string m_file2 in m_files2) run_FileNames2.Add(m_file2);
            }
            textBoxAllFiles.Text = run_FileNamesE.Count.ToString();
            /*
            cs_FileNames = Directory.GetFiles(SAVED_IMAGES_DIRECTORY, "*.bmp", SearchOption.TopDirectoryOnly);
            listBoxSImageFiles.Items.Clear();
            foreach (var cs_FileName in cs_FileNames)
            {
                string cs_Name = StripCoinName(cs_FileName);
                if (m_uniqueName == "")
                {
                    m_uniqueName = cs_Name;
                    listBoxSImageFiles.Items.Add(cs_Name);
                }
                else if (m_uniqueName == cs_Name)
                {


                }
                else
                {
                    m_uniqueName = cs_Name;
                    listBoxSImageFiles.Items.Add(cs_Name);
                }
                //builder = cs_Name;
                //builder.Replace(SAVED_IMAGES_DIRECTORY, "");
                //listBoxSImageFiles.Items.Add(builder.ToString());
            }
            */
            textBoxNoModels.Text = listBoxModels.Items.Count.ToString();

        }

        private void Update_Selected_Models_List(string search_text)
        {

            
            StringBuilder builder = new StringBuilder();
            string[] m_directories = Directory.GetDirectories(MODEL_IMAGES_DIRECTORY);
            listBoxModels.Items.Clear();
            //if (m_directories.Count > int(0))
            //{
            run_FileNamesE = new List<string>();
            run_FileNames1 = new List<string>();
            run_FileNames2 = new List<string>();
            //}


            foreach (string directory in m_directories)
            {
                string m_Name = StripCoinName(directory);
                if (m_Name.Contains(search_text))
                {
                    listBoxModels.Items.Add(m_Name);
                    string[] m_filesE = Directory.GetFiles(directory + "\\Edges");
                    string[] m_files1 = Directory.GetFiles(directory + "\\Side1");
                    string[] m_files2 = Directory.GetFiles(directory + "\\Side2");

                    foreach (string m_fileE in m_filesE) run_FileNamesE.Add(m_fileE);
                    foreach (string m_file1 in m_files1) run_FileNames1.Add(m_file1);
                    foreach (string m_file2 in m_files2) run_FileNames2.Add(m_file2);
                }
            }
            textBoxAllFiles.Text = run_FileNamesE.Count.ToString();            
            textBoxNoModels.Text = listBoxModels.Items.Count.ToString();
        }

        private void Update_SavedImages_List()
        {
            string cs_uniqueName = "";
            StringBuilder builder = new StringBuilder();
            cs_FileNames = Directory.GetFiles(SAVED_IMAGES_DIRECTORY, "*.bmp", SearchOption.TopDirectoryOnly);
            listBoxSImageFiles.Items.Clear();
            foreach (var cs_FileName in cs_FileNames)
            {
                string cs_Name = StripCoinName(cs_FileName);
                if (cs_uniqueName == "")
                {
                    cs_uniqueName = cs_Name;
                    listBoxSImageFiles.Items.Add(cs_Name);
                }
                else if (cs_uniqueName == cs_Name)
                {


                }
                else
                {
                    cs_uniqueName = cs_Name;
                    listBoxSImageFiles.Items.Add(cs_Name);
                }
                //builder = cs_Name;
                //builder.Replace(SAVED_IMAGES_DIRECTORY, "");
                //listBoxSImageFiles.Items.Add(builder.ToString());
            }
            textBoxNoUniqCoins.Text = listBoxSImageFiles.Items.Count.ToString();

        }

        public string StripCoinName(string pathAndName)
        {
            string cs_coinName = "";
            var splitString = pathAndName.Split("_");

            if (splitString.Length >= 6)
            {
                float cf_Diemeters = (float)(Convert.ToDouble(splitString[3]));
                float cf_Mag1 = (float)(Convert.ToDouble(splitString[4]));
                float cf_Mag2 = (float)(Convert.ToDouble(splitString[5]));
                float cf_Mag3 = (float)(Convert.ToDouble(splitString[6]));
                var cs_NameLong = splitString[0].Split("\\");
                var cs_lNameShort = splitString[1].Split("\\");
                cs_coinName = cs_NameLong[3] + "-" + splitString[1] + "-" + splitString[2];
            }
            else if (splitString.Length > 0)
            {
                splitString = pathAndName.Split("-");
                if (splitString.Length >= 3)
                {
                    var isoString = splitString[0].Substring(splitString[0].Length - 3);
                    var cs_NameLong = splitString[0].Split("\\");
                    var cs_lNameShort = splitString[1].Split("\\");
                    cs_coinName = isoString + "_" + splitString[1] + "_" + splitString[2];
                }
                else
                {
                    cs_coinName = "";
                }
            }
            else
            {
                cs_coinName = "";
            }
            return cs_coinName;
        }

        private bool SpreadSelectedCoinImages(string selectedCoinName)
        {
            int a, b = 0;
            var coinIndexes = new List<int>();

            foreach (var cs_FileName in cs_FileNames)
            {

                string cs_Name = StripCoinName(cs_FileName);

                var coina = cs_Name.ToString();
                //coinIndexes.Append(listBoxSImageFiles.Items.IndexOf(coin));
                //int[] coinIndexes = new int[];
                if (selectedCoinName == coina)
                {

                    coinIndexes.Add(b);
                    //b = listBoxSImageFiles.Items.IndexOf(coina);
                    //coinIndexes.Add(b);
                    //coinIndexes.Append(listBoxSImageFiles.Items.IndexOf(coin));
                }
                b++;
            }
            // Creatte folders
            if (coinIndexes.Count % 3 == 0)
            {
                if (selectedCoinName.Length > 0)
                {
                    // if coin Name directory does not exist , create it
                    if (!Directory.Exists(MODEL_IMAGES_DIRECTORY + selectedCoinName))
                    {
                        Directory.CreateDirectory(MODEL_IMAGES_DIRECTORY + selectedCoinName);
                    }
                    // if coin Name directory exist create subdirectory
                    if (Directory.Exists(MODEL_IMAGES_DIRECTORY + selectedCoinName))
                    {
                        Directory.CreateDirectory(MODEL_IMAGES_DIRECTORY + selectedCoinName + "\\Edges");
                        Directory.CreateDirectory(MODEL_IMAGES_DIRECTORY + selectedCoinName + "\\Side1");
                        Directory.CreateDirectory(MODEL_IMAGES_DIRECTORY + selectedCoinName + "\\Side2");
                        Directory.CreateDirectory(MODEL_IMAGES_DIRECTORY + selectedCoinName + "\\Master");
                        Directory.CreateDirectory(MODEL_IMAGES_DIRECTORY + selectedCoinName + "\\Detail");
                        Directory.CreateDirectory(MODEL_IMAGES_DIRECTORY + selectedCoinName + "\\Negative");
                    }
                }

                // sperad image files into folders
                // delete files from SavedImages folder 
                foreach (var indexer in coinIndexes)
                {
                    string filePathName = cs_FileNames[indexer];
                    string[] fileNameSplit = filePathName.Split('_');
                    string lastPart = fileNameSplit[fileNameSplit.Length - 1];
                    if (lastPart.IndexOf("F") == 0)
                    {
                        if (File.Exists(filePathName))
                        {
                            string newFilePathName = Path.GetFileName(filePathName);
                            if (File.Exists(MODEL_IMAGES_DIRECTORY + selectedCoinName + "\\Side1\\" + newFilePathName))
                                File.Delete(MODEL_IMAGES_DIRECTORY + selectedCoinName + "\\Side1\\" + newFilePathName);
                            File.Move(filePathName, MODEL_IMAGES_DIRECTORY + selectedCoinName + "\\Side1\\" + newFilePathName);
                        }
                    }
                    if (lastPart.IndexOf("B") == 0)
                    {
                        if (File.Exists(filePathName))
                        {
                            string newFilePathName = Path.GetFileName(filePathName);
                            if (File.Exists(MODEL_IMAGES_DIRECTORY + selectedCoinName + "\\Side2\\" + newFilePathName))
                                File.Delete(MODEL_IMAGES_DIRECTORY + selectedCoinName + "\\Side2\\" + newFilePathName);
                            File.Move(filePathName, MODEL_IMAGES_DIRECTORY + selectedCoinName + "\\Side2\\" + newFilePathName);
                        }
                    }
                    if (lastPart.IndexOf("T") == 0)
                    {
                        if (File.Exists(filePathName))
                        {
                            string newFilePathName = Path.GetFileName(filePathName);
                            if (File.Exists(MODEL_IMAGES_DIRECTORY + selectedCoinName + "\\Edges\\" + newFilePathName))
                                File.Delete(MODEL_IMAGES_DIRECTORY + selectedCoinName + "\\Edges\\" + newFilePathName);
                            File.Move(filePathName, MODEL_IMAGES_DIRECTORY + selectedCoinName + "\\Edges\\" + newFilePathName);
                        }
                    }
                }

                // update the list
                Update_SavedImages_List();
                return true;

            }
            else
            {
                // error : number of files does not multiple 3
                return false;
            }


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void buttonSpreadSelected_Click(object sender, EventArgs e)
        {
            if (listBoxSImageFiles.SelectedItem != null)
            {
                string selected_CoinName = listBoxSImageFiles.SelectedItem.ToString();
                SpreadSelectedCoinImages(selected_CoinName);

            }

        }

        private void buttonSpreadAll_Click(object sender, EventArgs e)
        {

            int numberOfList = listBoxSImageFiles.Items.Count;
            int badIndex = 0;
            for (int i = 0; i < numberOfList; i++)
            {
                var uniqueCoin = listBoxSImageFiles.Items[badIndex];
                if (uniqueCoin.ToString() is not null)
                {
                    if (!SpreadSelectedCoinImages(uniqueCoin.ToString()))
                    {
                        badIndex++;
                    }
                }
                Update_SavedImages_List();
            }
        }

        private void listBoxSImageFiles_SelectedValueChanged(object sender, EventArgs e)
        {
            selectedCoinName = listBoxSImageFiles.SelectedItem.ToString();
        }

        private void buttonListRef_Click(object sender, EventArgs e)
        {
            Update_SavedImages_List();
        }

        private void buttonModelsRef_Click(object sender, EventArgs e)
        {
            Update_Models_List();

        }

        private void buttonSetRunListAll_Click(object sender, EventArgs e)
        {
            Update_Models_List();
            changeRunList = true;

        }

        private void buttonSetRunList_Click(object sender, EventArgs e)
        {
            if (listBoxModels.SelectedItem is not null)
            {
                string m_selectedCoinName = listBoxModels.SelectedItem.ToString();
                m_selectedCoinName = m_selectedCoinName.Replace("_", "-");
                Update_Models_List(MODEL_IMAGES_DIRECTORY + m_selectedCoinName);
                changeRunList = true;
            }

        }

        private void listBoxSImageFiles_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBoxModels_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBoxSearchModels_TextChanged(object sender, EventArgs e)
        {
            Update_Selected_Models_List(textBoxSearchModels.Text);
            changeRunList = true;
        }
    }
}
