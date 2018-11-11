using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ftpMassUpgrade
{
    public struct netwrokInterfaceInfo
    {
        public string ifName { get; set; }
        public string ipAdd { get; set; }
        public string macAdd { get; set; }
    }
    public struct modbusSlaveInfo
    {
        public string slaveIPAdd { get; set; }
        public ushort slaveUid { get; set; }
        public ushort tcpPortId { get; set; }        
    }
    public struct modbusTable
    {
        public ushort regAdd { get; set; }
        public ushort regVal { get; set; }
    }
    public struct TeSysTConfiFile
    {
        public string fileName8Amp { get; set; }
        public string fileName27Amp { get; set; }
        public string fileName100Amp { get; set; }
    }
}