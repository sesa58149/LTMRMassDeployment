using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Windows;
using System.Threading;

namespace ftpMassUpgrade
{
    
    public struct backupFileHeader
    {
        public string comRef { get; set; }
        public string serialNumber { get; set; }
        public string kuFwVer { get; set; }
        public string kcFwVer { get; set; }
        public int confRegLen { get; set; }
        public int customStartIndex { get; set; }
        public int customRegLen { get; set; }


    }
    public class backupNRestoreProgressInfo
    {
      
        public int pbMax { get; set; }
        public int pbMin { get; set; }
        public int pbVal { get; set; }
        public actionOnControl actOn { get; set; }
        public deviceEntry dEntry { get; set; }
        public backupNRestoreProgressInfo()
        {
            pbMax = 0;
            pbMin = 0;
            pbVal = 0xFFFF;
            actOn = actionOnControl.NO_ACTION;
            dEntry = new deviceEntry();
        }
    }

    public delegate void callback_ProcessProgress(backupNRestoreProgressInfo pInfo);
    public class TeSysTBackupNRestore
    {
        const int MAX_CONF_FILE_SIZE = 20480;
        private deviceEntry[] deviceList { get; set; }
        int deviceLstCnt { get; set; }
       
        private TeSysTConfiFile confileNames { get; set; }

        private modbusTable[] Config8Amp { get;set;}
        private modbusTable[] Config27Amp { get; set; }
        private modbusTable[] Config100Amp { get; set; }
        private bool isConfig8AmpAvailable { get; set; }
        private bool isConfig27AmpAvailable { get; set; }
        private bool isConfig100AmpAvailable { get; set; }
        private callback_ProcessProgress cbPp { get; set; }
        private backupNRestoreProgressInfo progressInfo { get; set; }
        private ModbusRegisterAccess mbAccess { get; set; }

        private void initToDeafult()
        {
            deviceList = null;
            deviceLstCnt = 0;
            confileNames = new TeSysTConfiFile();
            Config8Amp = new modbusTable[MAX_CONF_FILE_SIZE];
            Config27Amp = new modbusTable[MAX_CONF_FILE_SIZE];
            Config100Amp = new modbusTable[MAX_CONF_FILE_SIZE];

            isConfig8AmpAvailable = false;
            isConfig27AmpAvailable = false;
            isConfig100AmpAvailable = false;

            cbPp = null;
            progressInfo = new backupNRestoreProgressInfo();
            mbAccess = new ModbusRegisterAccess();
        }

        public TeSysTBackupNRestore()
        {
            initToDeafult();
        }

        public TeSysTBackupNRestore(callback_ProcessProgress pp)
        {
            initToDeafult();
            cbPp = pp;
        }
        public void addDeviceToList(deviceEntry[] dList, int nDevice)
        {
            if (dList == null || nDevice == 0)
                return;
            if(deviceList==null)
            {
                deviceList = new deviceEntry[nDevice];
            }
            for(int i=0;i<nDevice;i++)
            {
                deviceList[i] = dList[i];
            }
        }
        public int backupDeviceConfiguration(deviceEntry[] dEntry, int nDevice)
        {
            progressInfo.actOn = actionOnControl.PROGRESS_BAR;
            progressInfo.pbVal = 0;
            progressInfo.pbMax = nDevice;
            progressInfo.pbMin = 0;
            cbPp(progressInfo);

            if (nDevice < 1)
                return -1;
            deviceList = dEntry;
            deviceLstCnt = nDevice;


            Thread backupTh;
            backupTh = new Thread(ltmrBackupTask);
            backupTh.Start();           
            return 0;
        }
        private void ltmrBackupTask()
        {
            for (int i = 0; i < deviceLstCnt; i++)
            {
                progressInfo.actOn = actionOnControl.LEBEL;
                progressInfo.dEntry = deviceList[i];
                progressInfo.pbMax = deviceLstCnt;
                progressInfo.pbMin = 0;
                cbPp(progressInfo);


                //readLTMRConfiguration(dEntry[i]);
                Thread.Sleep(1000);

                progressInfo.actOn = actionOnControl.PROGRESS_BAR;
                progressInfo.pbVal += 1;
                cbPp(progressInfo);
            }
            progressInfo.actOn = actionOnControl.PROCESS_COMPLETED;
            cbPp(progressInfo);
        }
        public int restoreDeviceConfiguration(deviceEntry []dEntry, int nDevice, TeSysTConfiFile confFiles)
        {
            return 0;
        }
        public void setProgressCallback(callback_ProcessProgress pp )
        {
            cbPp = pp;
        }

        ///* read custom logic file from LTMR*/
        //private void readLTMRLogicFile()
        //{
        //    byte tokenId = 0;
        //    int pageId = 0;
        //    int wordRemain = 0;
        //    int logicFileSize = 0;
        //    UInt16[] regVal = new UInt16[32];
        //    byte[] data = new byte[512];
        //    int failCode = 0;
        //    int isInConfigMode = 0;
        //    StreamWriter fWriter = null;
        //    /* enter in config mode before starting custom logic file access it is mandatory requirment */
        //    //if (comm.Read(ViewModelConstants.NodeNumber, 601, 1, ref regVal) == true)
        //    if (mbAccess.readModbusRegister( 601, 1, regVal) > 0)
        //    {
        //        isInConfigMode = regVal[0] & 0x0001;
        //        if (isInConfigMode == 0)
        //        {
        //            regVal[0] |= 0x0001;
        //            //if (comm.Write(ViewModelConstants.NodeNumber, 601, 1, regVal) == false)
        //            if (mbAccess.mbWrite(601, 1, regVal) == -1)
        //            {
        //                MessageBox.Show("logic File Back-up failed");
        //                failCode = -1;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        failCode = -1;
        //    }
        //    /* get  the custom logic file size from device. which to be uploaded*/
        //    //if (comm.Read(ViewModelConstants.NodeNumber, 1203, 1, ref regVal) == true)
        //    if (mbAccess.mbRead(1203, 1, regVal) > 0)
        //    {
        //        logicFileSize = regVal[0];
        //        wordRemain = logicFileSize + 1;// 1 byte added for len of file
        //        if (logicFileSize > 0)
        //        {
        //            /* get  the token to access the logic file available in device. */
        //            //if (comm.Read(ViewModelConstants.NodeNumber, 1209, 1, ref regVal) == true)
        //            if (myClient.mbRead(1209, 1, regVal) > 0)
        //            {
        //                tokenId = (byte)(regVal[0] / 256); // taken is only MSB
        //            }
        //            else
        //            {
        //                failCode = -1;
        //            }
        //        }
        //        else
        //        {
        //            failCode = -1;
        //            MessageBox.Show("Logic File Size is zero");
        //        }
        //    }
        //    if (tokenId > 0 && failCode == 0)
        //    {

        //        try
        //        {
        //            fWriter = new StreamWriter("logicFile.csv");
        //        }
        //        catch (FileLoadException e)
        //        {
        //            MessageBox.Show(" Fail to open File -- may be used by other application ");
        //        }
        //        while (wordRemain > 0)
        //        {


        //            /* Tell device to want access logical memory section (Custom logic) */
        //            regVal[0] = 1;
        //            //if (comm.Write(ViewModelConstants.NodeNumber, 1207, 1, regVal) == true)
        //            if (myClient.mbWrite(1207, 1, regVal) == 0)
        //            {
        //                /* Tell device the page number wants to read */
        //                regVal[0] = (ushort)pageId;
        //                //if (comm.Write(ViewModelConstants.NodeNumber, 1208, 1, regVal) == true)
        //                if (myClient.mbWrite(1208, 1, regVal) == 0)
        //                {
        //                    int cmd = tokenId * 256; // token in MSB
        //                    cmd += 1; //read command
        //                    regVal[0] = (ushort)cmd;

        //                    /*Send command to device telling READ with provided token*/
        //                    //if (comm.Write(ViewModelConstants.NodeNumber, 1209, 1, regVal) == true)
        //                    if (myClient.mbWrite(1209, 1, regVal) == 0)

        //                    {
        //                        int nReg = (wordRemain > 16) ? 16 : wordRemain;

        //                        /* read custom logic file of Max size 16 register/Word of asked pageId */
        //                        // if (comm.Read(ViewModelConstants.NodeNumber, 1210, (ushort)nReg, ref regVal) == true)
        //                        if (myClient.mbRead(1210, (ushort)nReg, regVal) > 0)
        //                        {
        //                            /* incereament tokenId by one every time for new request*/
        //                            tokenId++;
        //                            /* incereament pageId by Max Page size every time for new page request*/
        //                            pageId += 16;
        //                            wordRemain -= nReg;
        //                            /* dump this page in file and go for next pageId*/
        //                            for (int i = 0; i < nReg; i++)
        //                            {
        //                                fWriter.WriteLine(regVal[i].ToString());
        //                            }
        //                        }
        //                        if (wordRemain <= 0)
        //                        {
        //                            /* Release the token after uploading complete logic file */
        //                            cmd = tokenId * 256; // token in MSB
        //                            cmd += 5; //release token command
        //                            regVal[0] = (ushort)cmd;
        //                            //if (comm.Write(ViewModelConstants.NodeNumber, 1209, 1, regVal) == true)
        //                            if (myClient.mbWrite(1209, 1, regVal) == 0)
        //                            {
        //                                MessageBox.Show("Logical file backup successful ");
        //                            }

        //                        }

        //                    }
        //                    else
        //                    {
        //                        failCode = -1;
        //                    }
        //                }
        //                else
        //                {
        //                    failCode = -1;
        //                }
        //            }
        //            else
        //            {
        //                failCode = -1;
        //            }
        //        }//while
        //        fWriter.Close();
        //        if (failCode == -1)
        //        {
        //            MessageBox.Show("logical file Backup failed");
        //        }
        //    }

        //    /* Exit from config mode if enter in this fuction else nothing to do */
        //    if (isInConfigMode == 0)
        //    {
        //        //if (comm.Read(ViewModelConstants.NodeNumber, 601, 1, ref regVal) == true)
        //        if (myClient.mbRead(601, 1, regVal) > 0)
        //        {
        //            regVal[0] &= 0xFFFE;
        //            //if (comm.Write(ViewModelConstants.NodeNumber, 601, 1, regVal) == false)
        //            if (myClient.mbWrite(601, 1, regVal) == -1)
        //            {
        //                MessageBox.Show("Failed to exit from config mode");
        //            }

        //        }
        //    }
        //}
        /* Read LTMR configuration including host configuration register and 
       * custom logic file over Modbus serial line(HMI Port) */
        private void readLTMRConfiguration(deviceEntry dEntry)
        {
            ushort modbusStartAddress;
            ushort numberofRegister;
            byte[] data = new byte[512];
            UInt16[] regVal = new UInt16[120];
            int index = 0;


            //Register Address 540-599, 600-699,800-898,1250-1269,3000-3087,3088-3119)            
            backupFileHeader bHeader = new backupFileHeader();
            bHeader.comRef = dEntry.comRef;
            bHeader.kcFwVer = dEntry.kcVer;
            bHeader.kuFwVer = dEntry.kuVer;

            modbusSlaveInfo sInfo= new modbusSlaveInfo();
            sInfo.slaveIPAdd = dEntry.deviceIP;
            sInfo.slaveUid = 1;
            sInfo.tcpPortId = 502;
            mbAccess.setSlaveInfo(sInfo);

            mbAccess.readModbusRegister(70, 5, regVal);

            bHeader.serialNumber = regVal[0].ToString() + regVal[1].ToString() + regVal[2].ToString() + regVal[3].ToString() + regVal[4].ToString();

            modbusStartAddress = 540;
            numberofRegister = 60;
            //if (comm.Read(ViewModelConstants.NodeNumber, modbusStartAddress, numberofRegister, ref data) == true)
            if (mbAccess.readModbusRegister(modbusStartAddress, numberofRegister, regVal) > 0)
            {

                for (int i = 0; i < numberofRegister; i++)
                {

                    Config8Amp[i].regAdd = (ushort)(modbusStartAddress + i);
                    Config8Amp[i].regVal = (ushort)(regVal[i]);
                }

                index = index + numberofRegister;
                modbusStartAddress = 600;
                numberofRegister = 100;
                //if (comm.Read(ViewModelConstants.NodeNumber, modbusStartAddress, numberofRegister, ref data) == true)
                if (mbAccess.readModbusRegister(modbusStartAddress, numberofRegister, regVal) > 0)
                {
                    //confFile.Write("  Main Setting     " + " \n");
                    for (int i = 0; i < numberofRegister; i++)
                    {
                        Config8Amp[index + i].regAdd = (ushort)(modbusStartAddress + i);
                        Config8Amp[index + i].regVal = (ushort)(regVal[i]);
                    }

                    index = index + numberofRegister;
                    modbusStartAddress = 800;
                    numberofRegister = 99;
                    //if (comm.Read(ViewModelConstants.NodeNumber, modbusStartAddress, numberofRegister, ref data) == true)
                    if (mbAccess.readModbusRegister(modbusStartAddress, numberofRegister, regVal) > 0)
                    {
                        for (int i = 0; i < numberofRegister; i++)
                        {
                            Config8Amp[index + i].regAdd = (ushort)(modbusStartAddress + i);
                            Config8Amp[index + i].regVal = (ushort)(regVal[i]);
                        }
                        index = index + numberofRegister;
                        modbusStartAddress = 1250;
                        numberofRegister = 20;
                        //if (comm.Read(ViewModelConstants.NodeNumber, modbusStartAddress, numberofRegister, ref data) == true)
                        if (mbAccess.readModbusRegister(modbusStartAddress, numberofRegister, regVal) > 0)
                        {
                            for (int i = 0; i < numberofRegister; i++)
                            {
                                Config8Amp[index + i].regAdd = (ushort)(modbusStartAddress + i);
                                Config8Amp[index + i].regVal = (ushort)(regVal[i]);
                            }
                            index = index + numberofRegister;
                            modbusStartAddress = 3000;
                            numberofRegister = 88;
                            // if (comm.Read(ViewModelConstants.NodeNumber, modbusStartAddress, numberofRegister, ref data) == true)
                            if (mbAccess.readModbusRegister(modbusStartAddress, numberofRegister, regVal) > 0)
                            {
                                for (int i = 0; i < numberofRegister; i++)
                                {
                                    Config8Amp[index + i].regAdd = (ushort)(modbusStartAddress + i);
                                    Config8Amp[index + i].regVal = (ushort)(regVal[i]);
                                }
                                index = index + numberofRegister;
                                modbusStartAddress = 3088;
                                numberofRegister = 32;
                                // if (comm.Read(ViewModelConstants.NodeNumber, modbusStartAddress, numberofRegister, ref data) == true)
                                if (mbAccess.readModbusRegister(modbusStartAddress, numberofRegister, regVal) > 0)
                                {
                                    for (int i = 0; i < numberofRegister; i++)
                                    {
                                        Config8Amp[index + i].regAdd = (ushort)(modbusStartAddress + i);
                                        Config8Amp[index + i].regVal = (ushort)(regVal[i]);
                                    }
                                    index = index + numberofRegister;

                                }
                            }
                        }
                    }

                }

            }    
                      
            bHeader.confRegLen = index;
            bHeader.customStartIndex = index;

            string confFileName = dEntry.deviceIP.Replace('.', '_');
            confFileName = confFileName + "conf.csv";
            StreamWriter confFile = new StreamWriter(confFileName);
            // First write Backup file header to file then data
            confFile.Write(" Comercial ref" + ";" + bHeader.comRef + " \n");
            confFile.Write(" Serial No" + ";" + bHeader.serialNumber + " \n");
            confFile.Write(" KU FW Version" + ";" + bHeader.kuFwVer + " \n");
            confFile.Write(" KC FW Version" + ";" + bHeader.kcFwVer + " \n");
            confFile.Write(" lenght of Conf" + ";" + bHeader.confRegLen.ToString() + " \n");
            confFile.Write(" StartAdd CL" + ";" + bHeader.customStartIndex.ToString() + " \n");
            confFile.Write(" lenght of CL" + ";" + bHeader.customRegLen.ToString() + " \n");

            //write configuration register to file
            for (int i = 0; i < bHeader.confRegLen; i++)
            {
                confFile.Write(Config8Amp[i].regAdd.ToString() + ";" + Config8Amp[i].regVal.ToString() + " \n");
            }
            confFile.Close();

            /* check Operating mode before starting uploading custom logic file*/
            //if (myClient.mbRead(540, 1, regVal) > 0)
            //{
            //    /* Custom logic file will present if Reg(540)>255 else no custom logic file hance egnor reading it*/
            //    if (regVal[0] > 255)
            //    {
            //        readLTMRLogicFile();
            //    }

            //}
            MessageBox.Show("backup Completed");

        }
    }
}