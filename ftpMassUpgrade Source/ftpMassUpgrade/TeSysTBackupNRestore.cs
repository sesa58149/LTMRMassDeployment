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
  
    public class TeSysTBackupNRestore : ltmrConfigFileManager
    {
        
        private deviceEntry[] deviceList { get; set; }
        int deviceLstCnt { get; set; }
        private callback_ProcessProgress cbPp { get; set; }
        private backupNRestoreProgressInfo progressInfo { get; set; }
        private ModbusRegisterAccess mbAccess { get; set; }
        private deviceIdentification devIdObj { get; set; }
        private TeSysTConfFileCompatibilityParam cParam { get; set; }
        


        private void initToDeafult()
        {
            deviceList = null;
            deviceLstCnt = 0;           
            cbPp = null;
            progressInfo = new backupNRestoreProgressInfo();
            mbAccess = new ModbusRegisterAccess();
            devIdObj = new deviceIdentification();
            cParam = new TeSysTConfFileCompatibilityParam();
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
        public int restoreDeviceConfiguration(deviceEntry[] dEntry, int nDevice, TeSysTConfiFile confFiles)
        {
            int retCode = -1;

            progressInfo.actOn = actionOnControl.PROGRESS_BAR;
            progressInfo.pbVal = 0;
            progressInfo.pbMax = nDevice;
            progressInfo.pbMin = 0;
            cbPp(progressInfo);

            if (nDevice < 1)
            {
                Console.WriteLine("device list is 0 can't be restored");
                return retCode;
            }

            deviceList = dEntry;
            deviceLstCnt = nDevice;
            cParam = confFiles.cParam;

            Thread restoreTh = new Thread(ltmrRestoreTask);
            restoreTh.Start();
            retCode = 0;
           
            return retCode;
        }
        private void ltmrRestoreTask()
        {
            int retCode = -1;
            bool isValidRestore = false;
            LTRM_ComercialRef comRef = LTRM_ComercialRef.NONE;

            for (int i = 0; i < deviceLstCnt; i++)
            {
                progressInfo.actOn = actionOnControl.LEBEL;
                progressInfo.dEntry = deviceList[i];
                progressInfo.pbMax = deviceLstCnt;
                progressInfo.pbMin = 0;
                cbPp(progressInfo);

                isValidRestore = false;
                if (cParam.comercialReference == true) // check for configuration compatibility 
                {
                    comRef = getDeviceComRef(deviceList[i]);
                    if (comRef != LTRM_ComercialRef.NONE)
                    {
                        if (ltmrConfigFile[(int)comRef].isConfigAvailable == true)
                        {
                            isValidRestore = true; // comercial reference mathched 
                            if (cParam.kcFirmwareVer == true) // is KC check required 
                            {
                                if (deviceList[i].kcVer == ltmrConfigFile[(int)comRef].fileHeader.kcFwVer)
                                {
                                    isValidRestore = true;
                                }
                                else
                                {
                                    isValidRestore = false;
                                }
                            }

                            if (cParam.kuFirmwareVer == true) // is ku check required 
                            {
                                if (deviceList[i].kuVer == ltmrConfigFile[(int)comRef].fileHeader.kuFwVer)
                                {
                                    isValidRestore = true;
                                }
                                else // in case paased in KC ver test need to stop
                                {
                                    isValidRestore = false;
                                }
                            }
                        }
                    }
                }
                else // no compatibility check required 
                {
                    isValidRestore = true;
                    if (ltmrConfigFile[0].isConfigAvailable == true)
                        comRef = LTRM_ComercialRef.LTMR_08;
                    else if (ltmrConfigFile[1].isConfigAvailable == true)
                        comRef = LTRM_ComercialRef.LTMR_27;
                    else if (ltmrConfigFile[2].isConfigAvailable == true)
                        comRef = LTRM_ComercialRef.LTMR_100;
                    else
                        comRef = LTRM_ComercialRef.NONE;
                }

                if (isValidRestore == true)
                {
                    if (comRef != LTRM_ComercialRef.NONE)
                    {
                        retCode = startRestoreProcess(ltmrConfigFile[(int)comRef], deviceList[i].deviceIP);
                    }
                }
                
                progressInfo.actOn = actionOnControl.PROGRESS_BAR;
                progressInfo.pbVal += 1;
                cbPp(progressInfo);
            }//While 
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


                readLTMRConfiguration(deviceList[i]);
                Thread.Sleep(100);

                progressInfo.actOn = actionOnControl.PROGRESS_BAR;
                progressInfo.pbVal += 1;
                cbPp(progressInfo);
            }
            progressInfo.actOn = actionOnControl.PROCESS_COMPLETED;
            cbPp(progressInfo);
        }
         
        public int loadDeviceConfigurationFile(string fileName, LTRM_ComercialRef r)
        {
            return parserConfFile(fileName, r);
        }
        private int restoreCustomLogic(confFileFormate conf)
        {
            int retCode = -1;

            int tokenSize = conf.fileHeader.customRegLen;
            byte tokenId = 0;
            int pageId = 0;
            int wordRemain = tokenSize;
            ushort[] clToken = new ushort[tokenSize];
            UInt16[] regVal = new UInt16[32];
          
            for(int i=0;i<tokenSize;i++)
            {
                clToken[i] = conf.fileData[i + conf.fileHeader.customStartIndex].regVal;
            }
            /* get  the token to access the logic file available in device. */
            //if (comm.Read(ViewModelConstants.NodeNumber, 1209, 1, ref regVal) == true)
            if (mbAccess.readModbusRegister(1209, 1, regVal) == 0)
            {
                tokenId = (byte)(regVal[0] / 256); // taken is only MSB
            }
            else
            {
                MessageBox.Show("fail to get valid token from device");
                return retCode;
            }

           
            /*Check the size of downloading file should be less then the supported by device */
            if (mbAccess.readModbusRegister(1202, 1, regVal) ==0)
            {
                if (tokenSize > regVal[0])
                {   /* Action can't be completed as file size is larger than the supported */
                    MessageBox.Show("File size is larger then supported");
                    return retCode;
                }
            }

            /* clear all windows register which has old data*/
            regVal[0] = (UInt16)(tokenId << 8); // token
            regVal[0] += 4; //command clear all logic memory
            tokenId++;
            if (mbAccess.writeModbusRegister(1209, 1, regVal) !=0)
            {
                MessageBox.Show("Fail to clear Logic memory old data");
                return retCode;
            }

            while (wordRemain > 0)
            {
                /* Tell device to want access logical memory section (Custom logic) */
                regVal[0] = 1; //1207
                /* Tell device the page number wants to read */
                regVal[1] = (ushort)pageId; //1208
                /* send command to write data on window*/
                regVal[2] = (UInt16)((tokenId * 256) + 3); // token in MSB + Write command(3) inLSB
                int nReg = (wordRemain > 16) ? 16 : wordRemain;
                /* Write logic token of nReg in logic window */
                int j = 0;
                for (int i = 3; i < (3 + nReg); i++)
                {
                    regVal[i] = clToken[j + pageId];
                    j++;
                }
                //if (comm.Write(ViewModelConstants.NodeNumber, 1207, 1, regVal) == true)
                if (mbAccess.writeModbusRegister(1207, (UInt16)(3 + nReg), regVal) == 0)
                {
                    /* incereament tokenId by one every time for new request*/
                    tokenId++;
                    /* incereament pageId by Max Page size every time for new page request*/
                    pageId += 16;
                    wordRemain -= nReg;
                    if (wordRemain <= 0)
                        retCode = 0;
                }
                else
                {
                    MessageBox.Show("fail to write data to window ");
                    break;
                }

            }//while

            /* to the final work of download */
            if (retCode == 0)
            {
                /* write the file size downloaded to device */
                regVal[0] = 1;  // 1207
                regVal[1] = 0; //1208
                regVal[2] = (UInt16)((tokenId << 8) + 3); //1209
                regVal[3] = (UInt16)(tokenSize - 1); //1210 keep 1 byte out
                tokenId++;
                if (mbAccess.writeModbusRegister(1207, 4, regVal) == 0)
                {
                    regVal[0] = (UInt16)((tokenId << 8) + 2); //1209 WRITE TO FRAM  
                    mbAccess.writeModbusRegister(1209, 1, regVal);
                    tokenId++;
                    /* wait for a second to get data written over the Flash*/
                    System.Threading.Thread.Sleep(1000);
                    /* Release the token after uploading complete logic file */
                    regVal[0] = (UInt16)((tokenId << 8) + 5);
                    //if (comm.Write(ViewModelConstants.NodeNumber, 1209, 1, regVal) == true)
                    if (mbAccess.writeModbusRegister(1209, 1, regVal) == -1)
                    {
                        retCode = -1;
                    }
                }
            }
           
            return retCode;
        }

        private int restoreDeviceConfiguration(confFileFormate conf)
        {
            int retCode = -1;
            ushort []reg601 = new ushort[1];
            int i = 0,j=0;
            ushort[] regVal = new ushort[100]; 

            for (i = 0; i < 60; i++)
            {
                regVal[j++] = conf.fileData[i].regVal;
            }
            if(mbAccess.writeModbusRegister(540, 60, regVal)!=0)
            {
                Console.WriteLine("faile to resoter 540 range ");
            }
            
            // skip 601 
            regVal[0] = conf.fileData[i].regVal; //600
            reg601[0] = conf.fileData[i+1].regVal;
            if ( mbAccess.writeModbusRegister(600, 1, regVal)!=0)
            {
                Console.WriteLine("faile to resoter 600 ");
            }

            j = 0;
            for (i = 62; i < 160; i++)
            {
                regVal[j++] = conf.fileData[i].regVal;
            }
            if(mbAccess.writeModbusRegister(602, 98, regVal)!=0)
            {
                Console.WriteLine("faile to resoter 602 range ");
            }

            j = 0;
            for (i = 160; i < 259; i++)
            {
                regVal[j++] = conf.fileData[i].regVal; 
            }
            //myClient.mbWrite(800, 99, regVal);
            if(mbAccess.writeModbusRegister(800, 99, regVal)!=0)
            {
                Console.WriteLine("faile to resoter 800 range ");
            }

            j = 0;
            for (i = 259; i < 279; i++)
            {
                regVal[j++] = conf.fileData[i].regVal;
            }
            //myClient.mbWrite(1200, 20, regVal);
            if(mbAccess.writeModbusRegister(1250, 20, regVal)!=0)
            {
                Console.WriteLine("faile to resoter 1200 range ");
            }

            j = 0;
            for (i = 279; i < 367; i++)
            {
                regVal[j++] = conf.fileData[i].regVal;
            }
            // myClient.mbWrite(3000, 88, regVal);
            if(mbAccess.writeModbusRegister(3000, 88, regVal)!=0)
            {
                Console.WriteLine("faile to resoter 3000 range ");
            }

            j = 0;
            for (i = 367; i < 399; i++)
            {
                regVal[j++] = conf.fileData[i].regVal;
            }
           // myClient.mbWrite(3088, 32, regVal);
           if( mbAccess.writeModbusRegister(3088, 32, regVal)!=0)
            {
                Console.WriteLine("faile to resoter 3088 range ");
            }
            //myClient.mbClose();
            if(mbAccess.writeModbusRegister(601,1,reg601)!=0)
            {
                Console.WriteLine("faile to resoter 601 ");
            }
            retCode = 0;

            return retCode;
        }
        private int startRestoreProcess( confFileFormate conf, string deviceIP)
        {
            int retCode = -1;

            modbusSlaveInfo sInfo = new modbusSlaveInfo();
            sInfo.slaveIPAdd = deviceIP;
            sInfo.slaveUid = 1;
            sInfo.tcpPortId = 502;
            mbAccess.setSlaveInfo(sInfo);
            ushort[] regVal = new ushort[1]; 
            /* enter in config mode before starting custom logic file access it is mandatory requirment */
            //if (comm.Read(ViewModelConstants.NodeNumber, 601, 1, ref regVal) == true)
            if (mbAccess.readModbusRegister(601, 1, regVal) == 0)
            {
                int isInConfigMode = regVal[0] & 0x0001;
                if (isInConfigMode == 0)
                {
                    regVal[0] |= 0x0001;
                    //if (comm.Write(ViewModelConstants.NodeNumber, 601, 1, regVal) == false)
                    if (mbAccess.writeModbusRegister(601, 1, regVal) != 0)
                    { /* resotre can't start without entring in config mode*/
                        MessageBox.Show("fail to enter in config mode");
                        return retCode;
                    }
                }
            }

            if ( restoreDeviceConfiguration(conf)==0)
            {
                if (conf.fileHeader.customRegLen != 0)
                {
                    retCode = restoreCustomLogic(conf);
                }
            }


            

            return retCode;
        }
        private LTRM_ComercialRef getDeviceComRef(deviceEntry device)
        {
            LTRM_ComercialRef cRef = LTRM_ComercialRef.NONE;            

            string cRatings = device.comRef.Substring(4, 2);
            if (cRatings == "08")
            {
                cRef = LTRM_ComercialRef.LTMR_08;
            }
            else if (cRatings == "27")
            {
                cRef = LTRM_ComercialRef.LTMR_27;

            }
            else if (cRatings == "10")//100
            {
                cRef = LTRM_ComercialRef.LTMR_100;
            }

            return cRef;
        }
     
        public void setProgressCallback(callback_ProcessProgress pp )
        {
            cbPp = pp;
        }

        ///* read custom logic file from LTMR*/
        private int readLTMRLogicFile(ref confFileFormate confdata)
        {
            byte tokenId = 0;
            int pageId = 0;
            int wordRemain = 0;
            int logicFileSize = 0;
            UInt16[] regVal = new UInt16[32];
            byte[] data = new byte[512];
            int failCode = 0;
            int isInConfigMode = 0;
            int startAdd = confdata.fileHeader.customStartIndex;
            
            /* enter in config mode before starting custom logic file access it is mandatory requirment */
            //if (comm.Read(ViewModelConstants.NodeNumber, 601, 1, ref regVal) == true)
            if (mbAccess.readModbusRegister(601, 1, regVal) == 0)
            {
                isInConfigMode = regVal[0] & 0x0001;
                if (isInConfigMode == 0)
                {
                    regVal[0] |= 0x0001;
                    //if (comm.Write(ViewModelConstants.NodeNumber, 601, 1, regVal) == false)
                    if (mbAccess.writeModbusRegister(601, 1, regVal) == -1)
                    {
                        MessageBox.Show("logic File Back-up failed");
                        failCode = -1;
                    }
                }
            }
            else
            {
                failCode = -1;
            }
            /* get  the custom logic file size from device. which to be uploaded*/
            //if (comm.Read(ViewModelConstants.NodeNumber, 1203, 1, ref regVal) == true)
            if (mbAccess.readModbusRegister(1203, 1, regVal) == 0)
            {
                logicFileSize = regVal[0];                
                wordRemain = logicFileSize + 1;// 1 byte added for len of file
                if (logicFileSize > 0)
                {
                    /* get  the token to access the logic file available in device. */
                    //if (comm.Read(ViewModelConstants.NodeNumber, 1209, 1, ref regVal) == true)
                    if (mbAccess.readModbusRegister(1209, 1, regVal) == 0)
                    {
                        tokenId = (byte)(regVal[0] / 256); // taken is only MSB
                    }
                    else
                    {
                        failCode = -1;
                    }
                }
                else
                {
                    failCode = -1;
                    MessageBox.Show("Logic File Size is zero");
                }
            }
            if (tokenId > 0 && failCode == 0)
            {
                                
                while (wordRemain > 0)
                {


                    /* Tell device to want access logical memory section (Custom logic) */
                    regVal[0] = 1;
                    //if (comm.Write(ViewModelConstants.NodeNumber, 1207, 1, regVal) == true)
                    if (mbAccess.writeModbusRegister(1207, 1, regVal) == 0)
                    {
                        /* Tell device the page number wants to read */
                        regVal[0] = (ushort)pageId;
                        //if (comm.Write(ViewModelConstants.NodeNumber, 1208, 1, regVal) == true)
                        if (mbAccess.writeModbusRegister(1208, 1, regVal) == 0)
                        {
                            int cmd = tokenId * 256; // token in MSB
                            cmd += 1; //read command
                            regVal[0] = (ushort)cmd;

                            /*Send command to device telling READ with provided token*/
                            //if (comm.Write(ViewModelConstants.NodeNumber, 1209, 1, regVal) == true)
                            if (mbAccess.writeModbusRegister(1209, 1, regVal) == 0)

                            {
                                int nReg = (wordRemain > 16) ? 16 : wordRemain;

                                /* read custom logic file of Max size 16 register/Word of asked pageId */
                                // if (comm.Read(ViewModelConstants.NodeNumber, 1210, (ushort)nReg, ref regVal) == true)
                                if (mbAccess.readModbusRegister(1210, (ushort)nReg, regVal) == 0)
                                {
                                    /* incereament tokenId by one every time for new request*/
                                    tokenId++;
                                    /* incereament pageId by Max Page size every time for new page request*/
                                    pageId += 16;
                                    wordRemain -= nReg;
                                    /* dump this page in file and go for next pageId*/
                                    for (int i = 0; i < nReg; i++)
                                    {
                                        confdata.fileData[startAdd + i].regVal = regVal[i];                                       
                                    }
                                    startAdd += nReg;
                                }
                                if (wordRemain <= 0)
                                {
                                    /* Release the token after uploading complete logic file */
                                    cmd = tokenId * 256; // token in MSB
                                    cmd += 5; //release token command
                                    regVal[0] = (ushort)cmd;
                                    //if (comm.Write(ViewModelConstants.NodeNumber, 1209, 1, regVal) == true)
                                    if (mbAccess.writeModbusRegister(1209, 1, regVal) == 0)
                                    {                                       
                                        //MessageBox.Show("Logical file backup successful ");
                                        Console.WriteLine("Logical file backup successful ");
                                    }

                                }

                            }
                            else
                            {
                                failCode = -1;
                            }
                        }
                        else
                        {
                            failCode = -1;
                        }
                    }
                    else
                    {
                        failCode = -1;
                    }
                }//while
              
                if (failCode == -1)
                {
                    MessageBox.Show("logical file Backup failed");
                }
            }

            /* Exit from config mode if enter in this fuction else nothing to do */
            if (isInConfigMode == 0)
            {
                //if (comm.Read(ViewModelConstants.NodeNumber, 601, 1, ref regVal) == true)
                if (mbAccess.readModbusRegister(601, 1, regVal) == 0)
                {
                    regVal[0] &= 0xFFFE;
                    //if (comm.Write(ViewModelConstants.NodeNumber, 601, 1, regVal) == false)
                    if (mbAccess.writeModbusRegister(601, 1, regVal) ==-1)
                    {
                        MessageBox.Show("Failed to exit from config mode");
                    }

                }
            }
            return logicFileSize;
        }

        /* Read LTMR configuration including host configuration register and 
       * custom logic file over Modbus serial line(HMI Port) */
        private void readLTMRConfiguration(deviceEntry dEntry)
        {
            ushort modbusStartAddress;
            ushort numberofRegister;
            byte[] data = new byte[512];
            UInt16[] regVal = new UInt16[120];
            int index = 0;
            confFileFormate confData = new confFileFormate();

            //Register Address 540-599, 600-699,800-898,1250-1269,3000-3087,3088-3119)            
            confFileHeader bHeader = new confFileHeader();
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
           // confData.fileHeader = bHeader;
            confData.fileData = new confData[20480];
            
            modbusStartAddress = 540;
            numberofRegister = 60;
            
            if (mbAccess.readModbusRegister(modbusStartAddress, numberofRegister, regVal) == 0)
            {

                for (int i = 0; i < numberofRegister; i++)
                {

                    confData.fileData[i].regAdd = (ushort)(modbusStartAddress + i);
                    confData.fileData[i].regVal = (ushort)(regVal[i]);
                }

                index = index + numberofRegister;
                modbusStartAddress = 600;
                numberofRegister = 100;
                //if (comm.Read(ViewModelConstants.NodeNumber, modbusStartAddress, numberofRegister, ref data) == true)
                if (mbAccess.readModbusRegister(modbusStartAddress, numberofRegister, regVal) == 0)
                {
                    //confFile.Write("  Main Setting     " + " \n");
                    for (int i = 0; i < numberofRegister; i++)
                    {
                        confData.fileData[index + i].regAdd = (ushort)(modbusStartAddress + i);
                        confData.fileData[index + i].regVal = (ushort)(regVal[i]);
                    }

                    index = index + numberofRegister;
                    modbusStartAddress = 800;
                    numberofRegister = 99;
                    //if (comm.Read(ViewModelConstants.NodeNumber, modbusStartAddress, numberofRegister, ref data) == true)
                    if (mbAccess.readModbusRegister(modbusStartAddress, numberofRegister, regVal) == 0)
                    {
                        for (int i = 0; i < numberofRegister; i++)
                        {
                            confData.fileData[index + i].regAdd = (ushort)(modbusStartAddress + i);
                            confData.fileData[index + i].regVal = (ushort)(regVal[i]);
                        }
                        index = index + numberofRegister;
                        modbusStartAddress = 1250;
                        numberofRegister = 20;
                        //if (comm.Read(ViewModelConstants.NodeNumber, modbusStartAddress, numberofRegister, ref data) == true)
                        if (mbAccess.readModbusRegister(modbusStartAddress, numberofRegister, regVal) == 0)
                        {
                            for (int i = 0; i < numberofRegister; i++)
                            {
                                confData.fileData[index + i].regAdd = (ushort)(modbusStartAddress + i);
                                confData.fileData[index + i].regVal = (ushort)(regVal[i]);
                            }
                            index = index + numberofRegister;
                            modbusStartAddress = 3000;
                            numberofRegister = 88;
                            // if (comm.Read(ViewModelConstants.NodeNumber, modbusStartAddress, numberofRegister, ref data) == true)
                            if (mbAccess.readModbusRegister(modbusStartAddress, numberofRegister, regVal) == 0)
                            {
                                for (int i = 0; i < numberofRegister; i++)
                                {
                                    confData.fileData[index + i].regAdd = (ushort)(modbusStartAddress + i);
                                    confData.fileData[index + i].regVal = (ushort)(regVal[i]);
                                }
                                index = index + numberofRegister;
                                modbusStartAddress = 3088;
                                numberofRegister = 32;
                                // if (comm.Read(ViewModelConstants.NodeNumber, modbusStartAddress, numberofRegister, ref data) == true)
                                if (mbAccess.readModbusRegister(modbusStartAddress, numberofRegister, regVal) == 0)
                                {
                                    for (int i = 0; i < numberofRegister; i++)
                                    {
                                        confData.fileData[index + i].regAdd = (ushort)(modbusStartAddress + i);
                                        confData.fileData[index + i].regVal = (ushort)(regVal[i]);
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
            confData.fileHeader = bHeader;

            /* check Operating mode before starting uploading custom logic file*/
            //((modbusStartAddress, numberofRegister, regVal) == 0)
            if (mbAccess.readModbusRegister(540, 1, regVal) == 0)
            {
                /* Custom logic file will present if Reg(540)>255 else no custom logic file hance ignore reading it*/
                if (regVal[0] > 255)
                {
                   bHeader.customRegLen= readLTMRLogicFile(ref confData);
                }
                
            }

            confData.fileHeader = bHeader;
            string confFileName = dEntry.deviceIP.Replace('.', '_');
            confFileName = confFileName + "_conf.csv";
            writeConfFile(confData, confFileName);
            //MessageBox.Show("backup Completed");

        }
    }
}