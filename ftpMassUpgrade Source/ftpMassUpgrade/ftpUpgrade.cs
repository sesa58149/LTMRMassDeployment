using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.IO;
using Microsoft.Win32;
using System.Threading;
using System.Net;
using System.Net.Cache;

namespace ftpMassUpgrade
{
    public delegate void callback_ProgressBar(updateInfo info);
    public class updateInfo
    {
        public string fileName { get; set; }
        public string filePath { get; set; }
        public string serverIpAddress { get; set; }
        public int winControlIndex { get; set; }
        public int pbMax { get; set; }
        public int pbMin { get; set; }
        public int pbVal { get; set; }
        public actionOnControl actOn { get; set; }
        public deviceEntry dEntry { get; set; }
        
        public updateInfo()
        {
            fileName = " ";
            filePath = " ";
            serverIpAddress = " ";
            winControlIndex = 0xFF;
            pbMax = 0;
            pbMin = 0;
            pbVal = 0xFFFF;
            actOn = actionOnControl.NO_ACTION;
            dEntry = new deviceEntry();
            
        }
    }

    public class upgradeTask
    {
        public deviceEntry[] denty;
        public MainWindow mainW;
        public updateInfo[] info = new updateInfo[8];
        public int listIndex;
        public callback_ProgressBar cbPb;
        public Thread[] threadList { get; set; }
        public int deviceListSize;
        public Semaphore semaUpdate { get; set; }
        private Object thislock = new Object();
        private Object ftpFileLock = new Object();
        public string srcFileName { get; set; }
        public string srcFilePath { get; set; }
        private int numberOfActiveUgradeTask { get; set; }
        private int numberOfFailedUgradeTask { get; set; }
        public void setMainWindowAddress(MainWindow mainT)
        {
            mainW = mainT;
        }
        public void setFtpFileProperty(string fname, string fPath)
        {
            srcFileName = fname;
            srcFilePath = fPath;
        }
        private void intitDeafault()
        {
            listIndex = 0;
            mainW = null;
            numberOfActiveUgradeTask = 0;
            numberOfFailedUgradeTask = 0;
        }
        public upgradeTask()
        {
            intitDeafault();
            for (int i = 0; i < 8; i++)
            {
                info[i] = new updateInfo();
            }
            srcFilePath = null;
            srcFileName = null;
        }
        public upgradeTask(int controlItem, callback_ProgressBar pb,string filePath, string fileName)
        {
            intitDeafault();
            info = new updateInfo[controlItem];
            cbPb = pb;           
            srcFileName = fileName;
            srcFilePath = filePath;
            string st = new string(new char[8]);
        }
        public upgradeTask(callback_ProgressBar pb)
        {
            intitDeafault();
            listIndex = 0;
            mainW = null;
            for (int i = 0; i < 8; i++)
            {
                info[i] = new updateInfo();
            }
            cbPb = pb;
            srcFilePath = null;
            srcFileName = null;

        }
        public void uprgdFinishTaskTrack()
        {
            updateInfo uInfo = new updateInfo();
            while ( (numberOfActiveUgradeTask- numberOfFailedUgradeTask )> 0)
            {
                Thread.Sleep(1000);
            }
            uInfo.actOn = actionOnControl.PROCESS_COMPLETED;
            uInfo.dEntry = null;
            cbPb(uInfo); 
        }
        public void startUpgradeTask(deviceEntry[] entry, int listSize,upgradeFileInfo uFileInfo)
        {
            deviceListSize = listSize;
            denty = new deviceEntry[listSize];
            denty = entry;
            semaUpdate = new Semaphore(8, 8);
            deviceEntry dEntry = new deviceEntry();
            srcFileName = uFileInfo.srcFileName;
            srcFilePath = uFileInfo.srcFilePath;
            Thread myth;

            numberOfActiveUgradeTask = listSize;
            Thread uprgdTaskTrak = new Thread(uprgdFinishTaskTrack);
            uprgdTaskTrak.Start();
            int k = 0;
            for ( k = 0; k < listSize; k++)
            {
                dEntry = denty[k];               
                myth = new Thread(() => updateThread(dEntry));               
                myth.Start();
                Thread.Sleep(1000);
            }
            
        }

       // public void updateThread(int deviceId)
        public void updateThread(deviceEntry dEntry)
        {
            updateInfo uInfo = new updateInfo();
            Console.WriteLine("upgrade task invoked and waiting from sema");
            semaUpdate.WaitOne();
            int i = 0;
            lock (thislock)
            {
                for (i = 0; i < 8; i++)
                {
                    if (info[i].winControlIndex == 0xFF)
                    {
                        uInfo = info[i];
                        break;
                    }
                }
                if (i < 8)
                {
                    uInfo.winControlIndex = i;
                }
            }
            if (i > 8) // somthing is worng in memory allocation process did't able get windows resources
            {
                return;
            }
            uInfo.serverIpAddress = dEntry.deviceIP;
            uInfo.fileName = srcFileName;
            uInfo.filePath = srcFilePath;
            int rTry = 0;
            do
            {   // send file to device and retry for 3 time in case fail 
                if (PostDatatoDevice(uInfo) == -1)
               // if (dumy(uInfo) == -1)
                {
                    uInfo.actOn = actionOnControl.PROGRESS_BAR;
                    uInfo.pbVal = 0;
                    cbPb(uInfo);
                    rTry++;
                    Thread.Sleep(1000);
                }
                else
                {
                    break;
                }
                
            }while(rTry < 3);
            // Successful only within 3 time retry
            if (rTry < 3)
            {
                dEntry.upgradeStatus = "Successful";
            }
            else
            {
                //denty[deviceId].upgradeStatus = "Failed";
                dEntry.upgradeStatus = "Failed";
            }
            uInfo.actOn = actionOnControl.DATA_GRID;
            uInfo.dEntry = dEntry;
            cbPb(uInfo);
            lock (thislock)
            {
                info[i].winControlIndex = 0xFF;
            }

            numberOfActiveUgradeTask--;
            // release the semaphore for other task
            semaUpdate.Release();
        }

        private int dumy(updateInfo info)
        {
            /* progress bar update*/
            info.pbMax = 20;
            //reset progress bar and other information @ startup 
            info.pbMin = 0;
            info.pbVal = 0;
            info.actOn = actionOnControl.PROGRESS_BAR;
            cbPb(info);

            for (int i=0; i<20;i++)
            {
                
                info.pbVal += 1;
                /* update progress bar */
                cbPb(info);
                Thread.Sleep(1000);
            }
            return 0;
        }
        private int PostDatatoDevice(updateInfo info)
        {
           // updateInfo info = new updateInfo();
            int retVal = -1;
            int ftpFrameSize = 1400;
            // Read the source file into a byte array.
            byte[] dataToSend = new byte[ftpFrameSize];
            byte[] fileContain;
            int bytesToSend;
            int remainbyte;            
            Uri myUri = new Uri("ftp://"+info.serverIpAddress+ "/fw/" + info.fileName);
            byte[] fileContents = new byte[1400];
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(myUri);
           

            try
            {               
                request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.CacheIfAvailable);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.UseBinary = true;
                request.Credentials = new NetworkCredential("pcfactory", "pcfactory");
                lock (ftpFileLock)
                {
                    FileInfo fInfo = new FileInfo(info.filePath);
                    remainbyte = (int)fInfo.Length;
                    BinaryReader bread = new BinaryReader(File.Open(info.filePath, FileMode.Open));
                    fileContain  = new byte[remainbyte];
                    /* read complete file same time To give access for other thread*/
                    fileContain = bread.ReadBytes(remainbyte);
                    /* file operation closed*/
                    bread.Close();
                }
                /* progress bar update*/
                info.pbMax = (remainbyte / ftpFrameSize) + 1; // atleast 1 frame
                //reset progress bar and other information @ startup 
                info.pbMin = 0;
                info.pbVal = 0;
                //reset progress bar and other information @ startup 
                info.actOn = actionOnControl.PROGRESS_BAR;
                cbPb(info);
                

                /* ftp Operation start*/
                Stream requestStream = request.GetRequestStream();
                int dataIndex = 0;
                while (remainbyte > 0)
                {
                    bytesToSend = ((remainbyte > ftpFrameSize) ? ftpFrameSize : remainbyte);
                    //dataToSend = bread.ReadBytes(bytesToSend);                   
                    request.ContentLength = bytesToSend;
                    Buffer.BlockCopy(fileContain, dataIndex, dataToSend, 0,bytesToSend);
                    requestStream.Write(dataToSend, 0, bytesToSend);
                    dataIndex += bytesToSend;
                    info.pbVal += 1;
                    Console.WriteLine(info.pbVal.ToString());
                    /* update progress bar */
                    info.actOn = actionOnControl.PROGRESS_BAR;
                    cbPb(info);
                    remainbyte = remainbyte - bytesToSend;
                    Thread.Sleep(10);
                }
                /*FTP Close */
                requestStream.Close();
                                
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);

                response.Close();
                retVal = 0;
                info.dEntry.upgradeStatus = "Successfull";
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message.ToString());
                String status = ((FtpWebResponse)e.Response).StatusDescription;
                Console.WriteLine(status);
                numberOfFailedUgradeTask++;
                info.dEntry.upgradeStatus = "Failed";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                numberOfFailedUgradeTask++;
                info.dEntry.upgradeStatus = "Failed";
            }

            return retVal;
        }


    }//updateTask Class




} // name space
     
