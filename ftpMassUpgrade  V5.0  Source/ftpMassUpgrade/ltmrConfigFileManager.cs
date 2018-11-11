using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace ftpMassUpgrade
{

    public struct confFileHeader
    {
        public string comRef { get; set; }
        public string serialNumber { get; set; }
        public string kuFwVer { get; set; }
        public string kcFwVer { get; set; }
        public int confRegLen { get; set; }
        public int customStartIndex { get; set; }
        public int customRegLen { get; set; }
      
    }
    public struct confFileFormate
    {
        public confFileHeader fileHeader { get; set; }
        public confData[] fileData { get; set; }
        public bool isConfigAvailable { get; set; }
    }

    public class ltmrConfigFileManager
    {
        public const int MAX_CONF_FILE_SIZE = 20480;
        public const int maxBackupFilesupported = 3;
        protected confFileFormate[] ltmrConfigFile { get; set; }
        protected string[] confFileName { get; set; }

        public ltmrConfigFileManager()
        {
            ltmrConfigFile = new confFileFormate[maxBackupFilesupported];           
            
        }
       
        public confFileHeader getConfHeader(LTRM_ComercialRef Refrence)
        {
            return ltmrConfigFile[(int)Refrence].fileHeader;
        }
        public confData[] getConfData(LTRM_ComercialRef Refrence)
        {
            return ltmrConfigFile[(int)Refrence].fileData;
        }
        protected int parseConfFileHeader(string fileName, LTRM_ComercialRef Refrence)
        {
            int retCode = -1;

            if (File.Exists(fileName) == false)
            {
                //MessageBox.Show("File Doesn't exist");
                return retCode;
            }
            StreamReader confFile = new StreamReader(fileName);
            confFileHeader fileHeader = new confFileHeader();
           
            //read header line not data from this line
            string strLine = confFile.ReadLine();
            int pos = strLine.IndexOf(";");
            if (pos <= 0)
            {
                Console.WriteLine("Wrong File Formate");
                confFile.Close();
                return retCode;
            }
            string strTmpC = strLine.Substring(0, pos);
            string strTmpD = strLine.Substring(pos + 1);

            string cRatings = strTmpD.Substring(4, 2);
            if (cRatings == "08")
            {
                if (Refrence != LTRM_ComercialRef.LTMR_08)
                {
                    confFile.Close();
                    return retCode;
                }
            }
            else if (cRatings == "27")
            {
                if (Refrence != LTRM_ComercialRef.LTMR_27)
                {
                    confFile.Close();
                    return retCode;
                }

            }
            else if (cRatings == "10")//100
            {
                if (Refrence != LTRM_ComercialRef.LTMR_100)
                {
                    confFile.Close();
                    return retCode;
                }
            }
            else
            {
                confFile.Close();
                return retCode;
            }            
            fileHeader.comRef = strTmpD;

            strLine = confFile.ReadLine();
            pos = strLine.IndexOf(";");
            if (pos <= 0)
            {
                Console.WriteLine("Wrong File Formate");
                confFile.Close();
                return retCode;
            }
            strTmpC = strLine.Substring(0, pos);
            strTmpD = strLine.Substring(pos + 1);
            fileHeader.serialNumber = strTmpD;

            strLine = confFile.ReadLine();
            pos = strLine.IndexOf(";");
            if (pos <= 0)
            {
                Console.WriteLine("Wrong File Formate");
                confFile.Close();
                return retCode;
            }
            strTmpC = strLine.Substring(0, pos);
            strTmpD = strLine.Substring(pos + 1);
            fileHeader.kuFwVer = strTmpD;

            strLine = confFile.ReadLine();
            pos = strLine.IndexOf(";");
            if (pos <= 0)
            {
                Console.WriteLine("Wrong File Formate");
                confFile.Close();
                return retCode;
            }
            strTmpC = strLine.Substring(0, pos);
            strTmpD = strLine.Substring(pos + 1);
            fileHeader.kcFwVer = strTmpD;

            strLine = confFile.ReadLine();
            pos = strLine.IndexOf(";");
            if (pos <= 0)
            {
                Console.WriteLine("Wrong File Formate");
                confFile.Close();
                return retCode;
            }
            strTmpC = strLine.Substring(0, pos);
            strTmpD = strLine.Substring(pos + 1);
            fileHeader.confRegLen = Convert.ToInt32(  strTmpD);

            strLine = confFile.ReadLine();
            pos = strLine.IndexOf(";");
            if (pos <= 0)
            {
                Console.WriteLine("Wrong File Formate");
                confFile.Close();
                return retCode;
            }
            strTmpC = strLine.Substring(0, pos);
            strTmpD = strLine.Substring(pos + 1);
            fileHeader.customStartIndex = Convert.ToInt32(strTmpD);

            strLine = confFile.ReadLine();
            pos = strLine.IndexOf(";");
            if (pos <= 0)
            {
                Console.WriteLine("Wrong File Formate");
                confFile.Close();
                return retCode;
            }
            strTmpC = strLine.Substring(0, pos);
            strTmpD = strLine.Substring(pos + 1);
            fileHeader.customRegLen = Convert.ToInt32(strTmpD);         

            ltmrConfigFile[(int)Refrence].fileHeader = fileHeader;
            retCode = 0;
            confFile.Close();

            return retCode;
        }
        /* Parse configuration data from selected file and store in local private variable  
         */
        protected int parseConfFileData(string fileName, LTRM_ComercialRef r)
        {
            int i = 0;
            int retCode = -1;
            //Register Address 540-599, 600-699,800-898,1250-1269,3000-3087,3088-3119)


            StreamReader confFile = new StreamReader(fileName);
            //read header line not data from this line
            //Just to skeep header and start reading from Data block
            string strLine = confFile.ReadLine();//ref
            strLine = confFile.ReadLine();//ser
            strLine = confFile.ReadLine();//fw
            strLine = confFile.ReadLine();//fw
            strLine = confFile.ReadLine();//le
            strLine = confFile.ReadLine();//le
            strLine = confFile.ReadLine();//le

            //ltmrConfigFile[0].
            confData[] conf = new confData[MAX_CONF_FILE_SIZE];

            for (i = 0; i < 60; i++)  //60 reg
            {
                strLine = confFile.ReadLine();
                int pos = strLine.IndexOf(";");
                string strAdd = strLine.Substring(0, pos);
                conf[i].regAdd = Convert.ToUInt16(strAdd);
                string strData = strLine.Substring(pos + 1);
                conf[i].regVal = Convert.ToUInt16(strData);
            }

            for (i = 60; i < 160; i++) //100 reg
            {
                strLine = confFile.ReadLine();
                int pos = strLine.IndexOf(";");
                string strAdd = strLine.Substring(0, pos);
                conf[i].regAdd = Convert.ToUInt16(strAdd);
                string strData = strLine.Substring(pos + 1);
                conf[i].regVal = Convert.ToUInt16(strData);
            }
            for (i = 160; i < 259; i++) //99 reg
            {
                strLine = confFile.ReadLine();
                int pos = strLine.IndexOf(";");
                string strAdd = strLine.Substring(0, pos);
                conf[i].regAdd = Convert.ToUInt16(strAdd);
                string strData = strLine.Substring(pos + 1);
                conf[i].regVal = Convert.ToUInt16(strData);
            }

            for (i = 259; i < 279; i++)//20 Reg
            {
                strLine = confFile.ReadLine();
                int pos = strLine.IndexOf(";");
                string strAdd = strLine.Substring(0, pos);
                conf[i].regAdd = Convert.ToUInt16(strAdd);
                string strData = strLine.Substring(pos + 1);
                conf[i].regVal = Convert.ToUInt16(strData);
            }

            for (i = 279; i < 367; i++) //88 reg
            {
                strLine = confFile.ReadLine();
                int pos = strLine.IndexOf(";");
                string strAdd = strLine.Substring(0, pos);
                conf[i].regAdd = Convert.ToUInt16(strAdd);
                string strData = strLine.Substring(pos + 1);
                conf[i].regVal = Convert.ToUInt16(strData);
            }
            for (i = 367; i < 399; i++) // 32 reg
            {
                strLine = confFile.ReadLine();
                int pos = strLine.IndexOf(";");
                string strAdd = strLine.Substring(0, pos);
                conf[i].regAdd = Convert.ToUInt16(strAdd);
                string strData = strLine.Substring(pos + 1);
                conf[i].regVal = Convert.ToUInt16(strData);
            }

            for(i=399; i< (399+ltmrConfigFile[(int )r].fileHeader.customRegLen);i++)
            {
                strLine = confFile.ReadLine();               
                conf[i].regVal = Convert.ToUInt16(strLine);
            }

            ltmrConfigFile[(int)r].fileData = conf;
            ltmrConfigFile[(int)r].isConfigAvailable = true;

            confFile.Close();
            retCode = 0;

            return retCode;
        }
        public int parserConfFile(string fileName, LTRM_ComercialRef Refrence)
        {
            int retCode = -1;

            if (File.Exists(fileName) == false)
            {
                MessageBox.Show("File Doesn't exist");
                return retCode;
            }

            if (parseConfFileHeader(fileName, Refrence) !=0)
            {
                retCode = -2;
                return retCode;
            }
            
            if(parseConfFileData(fileName,Refrence)!=0)
            {
                retCode = -3;
                return retCode;
            }
           
            retCode = 0;            
            return retCode;
        }
        
        public int writeConfFile(confFileFormate confRowData, string fileName)
        {
            int retCode = -1;
            if(fileName=="")
            {
                Console.WriteLine("Config file name can't be null");
                return retCode;
            }
            try
            {
                StreamWriter confFile = new StreamWriter(fileName);

                // Write header to file    
                confFile.Write("Comercial Reference" + ";" + confRowData.fileHeader.comRef + " \n");
                confFile.Write("Serial Number" + ";" + confRowData.fileHeader.serialNumber + " \n");
                confFile.Write("Controller FW Ver" + ";" + confRowData.fileHeader.kuFwVer + " \n");
                confFile.Write("Network FW Ver" + ";" + confRowData.fileHeader.kcFwVer + " \n");
                confFile.Write("Conf Data Len" + ";" + confRowData.fileHeader.confRegLen.ToString() + " \n");
                confFile.Write("CL Start Index" + ";" + confRowData.fileHeader.customStartIndex.ToString() + " \n");
                confFile.Write("CL Data Len" + ";" + confRowData.fileHeader.customRegLen.ToString() + " \n");
                for (int i = 0; i < confRowData.fileHeader.confRegLen; i++)
                {
                    confFile.Write(confRowData.fileData[i].regAdd.ToString() + ";" + confRowData.fileData[i].regVal.ToString() + " \n");
                }
                if (confRowData.fileHeader.confRegLen == confRowData.fileHeader.customStartIndex)
                {
                    int clLen = confRowData.fileData[confRowData.fileHeader.confRegLen].regVal;
                    for (int i = confRowData.fileHeader.customStartIndex; i < (clLen+ confRowData.fileHeader.customStartIndex); i++)
                    {
                        confFile.Write(confRowData.fileData[i].regVal + " \n");
                    }
                }
                confFile.Close();
                retCode = 0;
            }
            catch (excepetion ex)
            {
                Console.WriteLine(ex.Message);
            }

            return retCode;
        }
    }
   
}
