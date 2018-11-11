using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ftpMassUpgrade
{
    public class ConfigManager
    {
        static netwrokInterfaceInfo nIface { get; set; }
        public  ConfigManager()
        {
            nIface = new netwrokInterfaceInfo();
            
        }
        static public void setNetwrokInterfaceInfo(netwrokInterfaceInfo ifInfo)
        {
            nIface = ifInfo;
        }
        static public netwrokInterfaceInfo getNetwrokInterfaceInfo()
        {
            return nIface;
        }
    }
}