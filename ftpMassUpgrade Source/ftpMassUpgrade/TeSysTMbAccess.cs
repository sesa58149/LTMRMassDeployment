﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using System.Threading;

namespace ftpMassUpgrade
{
    public delegate void scanProgressBar_Callback(scanInfo sInfo);
    public class tcpNIpInfoStruct
    {
        public string clientIP;
        public string serverIP;
        public int clientPort;
        public int serverPort;
        public int nDevice;
        public tcpNIpInfoStruct()
        {
            clientIP = " ";
            serverIP = " ";
            clientPort = 0;
            serverPort = 502;
            nDevice = 0;
        }

    }
    public class scanInfo
    {
        public int pvMax { get; set; }
        public int pvMin { get; set; }
        public int pvVal { get; set; }
        public string IPAddress { get; set; }
        public deviceEntry deviceInfo { get; set; }
        public actionOnControl actOn { get; set; }
        public scanInfo()
        {
            pvMax =0;
            pvMin=0;
            pvVal=0;
            IPAddress=" ";
            deviceInfo = new deviceEntry();
            actOn = actionOnControl.PROGRESS_BAR;
        }
    
    }// scanIfo
    public class TeSysTMbAccess
    {
        private scanInfo sInfo { get; set; }
        private mbTCP mbClient { get; set; }
        private toolUitility utility { get; set; }
        public UInt16[] mbReadBuf { get; set; }
        public UInt16[] mbWriteBuf { get; set; }
        private string clientIpAdd { get; set; }
        private int clientPortNumber { get; set; }
               
        private deviceIdentification devIdObj { get; set; }
        public scanProgressBar_Callback callBack;

        private void initToDefault()
        {
            sInfo = new scanInfo();
            mbClient = new mbTCP();
            utility = new toolUitility();
            devIdObj = new deviceIdentification();
            mbReadBuf = new UInt16[120];
            mbWriteBuf = new UInt16[120];
            clientPortNumber = 0;
            clientIpAdd = " ";
        }

        public TeSysTMbAccess(scanProgressBar_Callback cb)
        {
            initToDefault();
            callBack = cb;                                
        }        
        public TeSysTMbAccess()
        {
            initToDefault();
            callBack = null;           
        }

        private void parseComRef(deviceEntry entry)
        {

            char tmp = (char)(mbReadBuf[0] >> 8);// MSB
            entry.comRef = tmp.ToString();
            tmp = (char)(mbReadBuf[0] & 0x00FF);  //LSB
            entry.comRef += tmp.ToString();


            tmp = (char)(mbReadBuf[1] >> 8);// MSB
            entry.comRef += tmp.ToString();
            tmp = (char)((mbReadBuf[1]) & 0x00FF);// LSB
            entry.comRef += tmp.ToString();


            tmp = (char)(mbReadBuf[2] >> 8);// MSB
            entry.comRef += tmp.ToString();
            tmp = (char)((mbReadBuf[2]) & 0x00FF);// LSB
            entry.comRef += tmp.ToString();


            tmp = (char)(mbReadBuf[3] >> 8);// MSB
            entry.comRef += tmp.ToString();
            tmp = (char)((mbReadBuf[3]) & 0x00FF);// LSB
            entry.comRef += tmp.ToString();


            tmp = (char)(mbReadBuf[4] >> 8);// MSB
            entry.comRef += tmp.ToString();
            tmp = (char)((mbReadBuf[4]) & 0x00FF);// LSB
            entry.comRef += tmp.ToString();


        }

        private void parsekuVer(deviceEntry entry )
        {
            UInt16 regVal = mbReadBuf[0];
            int major = regVal / 10000;
            int minor = regVal % 10000;
            int rev = minor % 1000;
            string revStr;

            if (rev > 99)
            {
                revStr = rev.ToString();
            }
            else if (rev > 9)
            {
                revStr = "0" + rev.ToString();
            }
            else
            {
                revStr = "00" + rev.ToString();
            }

            minor /= 1000;

            entry.kuVer = major.ToString() + "." + minor.ToString() + "." + revStr;
        }
        private void parsekcVer( deviceEntry entry)
        {


            UInt16 regVal = mbReadBuf[0];
            int major = regVal / 10000;
            int minor = regVal % 10000;
            int rev = minor % 1000;
            string revStr;
            if (rev > 99)
            {
                revStr = rev.ToString();
            }
            else if (rev > 9)
            {
                revStr = "0" + rev.ToString();
            }
            else
            {
                revStr = "00" + rev.ToString();
            }

            minor /= 1000;
            entry.kcVer = major.ToString() + "." + minor.ToString() + "." + revStr;


        }

        public int scanNetwork(deviceEntry[] deviceList, tcpNIpInfoStruct portInfo)
        {
            scanInfo sInfo = new scanInfo();
            
            int retCode = -1;
            string serverIP= portInfo.serverIP;

            sInfo.pvMax = portInfo.nDevice;
            sInfo.pvMin = 0;
            sInfo.pvVal = 0;
            sInfo.actOn = actionOnControl.PROGRESS_BAR;
            try
            {
                callBack(sInfo);
                for (int i = 0; i < portInfo.nDevice; i++)
                {
                    deviceEntry entry = new deviceEntry();
                    sInfo.IPAddress = serverIP;
                    sInfo.actOn = actionOnControl.LEBEL;
                    callBack(sInfo);

                    if( devIdObj.getDeviceInformation(entry, serverIP) == 0)
                    {
                        // update data grid only if device available in the network else look for next device
                        entry.deviceIP = serverIP;
                        deviceList[i] = entry;
                        sInfo.deviceInfo = entry;
                        sInfo.actOn = actionOnControl.DATA_GRID;
                        callBack(sInfo);
                    }
                    long IPLong = utility.IP2LongEndianLess(serverIP);
                    IPLong++; // go for next IP address
                    serverIP = utility.LongToIP(IPLong);
                    
                    // update only progress bar
                    sInfo.pvVal++;
                    sInfo.actOn = actionOnControl.PROGRESS_BAR;
                    callBack(sInfo);  
                   
                }//For Loop 
                sInfo.actOn = actionOnControl.PROCESS_COMPLETED;
                callBack(sInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);               
            }

            return retCode;
        }

        public int backupDevice(string []fileNames, string startIpAdd, int nDevice)
        {
            int retCode = -1;

            return retCode;

        }
        public int storeDevice(string[] fileNames, string startIpAdd, int nDevice)
        {
            int retCode = -1;

            return retCode;

        }
        public int mbRead(int rStartAddress, int rNReg, string IpAdd, int[] rRegVal )
        {
            int retCode = -1;

            return retCode;

        }
        public int mbWrite(int wStartAddress, int wNReg, string IpAdd, int[] wRegVal)
        {
            int retCode = -1;

            return retCode;

        }
        public int readDeviceIdentity( string ipAdd,string identity)
        {
            int retCode = -1;

            return retCode;
        }

    }
}
