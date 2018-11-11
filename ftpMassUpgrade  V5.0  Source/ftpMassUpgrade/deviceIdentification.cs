using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ftpMassUpgrade
{
   
    class deviceIdentification:toolUitility
    {
        const int devComRefAddress = 64;
        const int devComRefNReg = 5;
        const int controllerFWAddress = 76;
        const int controllerFWNReg = 1;
        const int networkFWAddress = 62;
        const int networkFWNReg = 1;
        ushort[] mbReadBuf { get; set; }
        private deviceEntry deviceInfo { get; set; }
        private ModbusRegisterAccess mbAccess { get; set; }
       
        public deviceIdentification()
        {
            mbReadBuf = new ushort[120];
            deviceInfo = new deviceEntry();
            mbAccess = new ModbusRegisterAccess();
         
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

        private void parsekuVer(deviceEntry entry)
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
        private void parsekcVer(deviceEntry entry)
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

        public int getDeviceInformation(deviceEntry devEntry,string deviceIP)
        {
            int retCode = -1;
            modbusSlaveInfo slaveInfo = new modbusSlaveInfo();
            slaveInfo.slaveIPAdd = deviceIP;
            slaveInfo.slaveUid = 1;
            slaveInfo.tcpPortId = 502;

            mbAccess.setSlaveInfo(slaveInfo);

            if(mbAccess.readModbusRegister(devComRefAddress, devComRefNReg, mbReadBuf)==0)
            {
                parseComRef(devEntry);
                if (mbAccess.readModbusRegister(controllerFWAddress, controllerFWNReg, mbReadBuf) == 0)
                {
                    parsekuVer(devEntry);
                }
                if (mbAccess.readModbusRegister(networkFWAddress, networkFWNReg, mbReadBuf) == 0)
                {
                    parsekcVer(devEntry);
                    retCode = 0;
                }
            }
            return retCode;
        }


    }//class



}//namespace
