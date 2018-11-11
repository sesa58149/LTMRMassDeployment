using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ftpMassUpgrade
{
    public class ModbusRegisterAccess
    {
        private const ushort maxFrameSize = 125;
        private const ushort mbTcpPort = 502;
        private  ushort[] rxFrame { get; set; }
        private mbTCP mbTcpClient { get; set; }
        private modbusSlaveInfo slaveInfo { get; set; }

        public ModbusRegisterAccess()
        {
            rxFrame = new ushort[maxFrameSize];
            mbTcpClient = new mbTCP();
            slaveInfo = new modbusSlaveInfo();
            
        }

        /** setSlaveInfo(modbusSlaveInfo sInfo)
         *  Description:    public interface- Application calls to set slave inforamation
         *                  before starting the modbus messaging 
         *  input:          sInfo - slave IP , UintID, and TCP PortID
         *  Output:         None
         *  return:         None
         */
        public void setSlaveInfo(modbusSlaveInfo sInfo)
        {
            slaveInfo = sInfo;
        }

        /** readModbusRegister(ushort startAdd, ushort nReg, ushort []readData)
        *  Description:    public interface -Application calls to read modbus registers
        *                  from slave. SetSlaveInfo first(one time or slave info changes) 
        *                  then call this function. 
        *  input:          startAdd - start register address
        *                  nReg-      number of register to read  
        *  Output:         readData-  Array of ushort of size atleast nReg  
        *  return:         int: -1 on error else number of valid bytes in readData buffer
        */
        public int readModbusRegister(ushort startAdd, ushort nReg, ushort []readData)
        {
            int retCode = -1;
            netwrokInterfaceInfo localInterface = ConfigManager.getNetwrokInterfaceInfo();
            if(localInterface.ipAdd == " ")
            {
                Console.WriteLine("local interface is not selected");
                return retCode;
            }
            if(nReg >120)
            {
                Console.WriteLine( nReg.ToString() + " is more than support 120 Max");
                return retCode;
            }

            try
            {
                if (mbTcpClient.modbusInit(localInterface.ipAdd, 0) == 0)
                {
                    if (mbTcpClient.mbOpen(slaveInfo.slaveIPAdd, slaveInfo.tcpPortId) == 0)
                    {
                        retCode = mbTcpClient.mbRead(startAdd, nReg, readData);
                        mbTcpClient.mbClose();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return retCode;
        }

        /** writeModbusRegister(ushort startAdd, ushort nReg, ushort []writeData)
       *  Description:    public interface -Application calls to write modbus registers
       *                  from slave. SetSlaveInfo first(one time or slave info changes) 
       *                  then call this function. 
       *  input:          startAdd - start register address
       *                  nReg-      number of register to write  
       *  Output:         writeData-  Array of ushort of size atleast nReg with data to be written to device 
       *  return:         int: -1 = error and 0= success
       */
        public int writeModbusRegister(ushort startAdd, ushort nReg, ushort[] writeData)
        {
            int retCode = -1;
            netwrokInterfaceInfo localInterface = ConfigManager.getNetwrokInterfaceInfo();
            if (localInterface.ipAdd == " ")
            {
                Console.WriteLine("local interface is not selected");
                return retCode;
            }

            if (mbTcpClient.modbusInit(localInterface.ipAdd, 0) == 0)
            {
                if (mbTcpClient.mbOpen(slaveInfo.slaveIPAdd, slaveInfo.tcpPortId) == 0)
                {
                    retCode = mbTcpClient.mbWrite(startAdd, nReg, writeData);
                    mbTcpClient.mbClose();
                }
            }

            return retCode;
        }
    }
}