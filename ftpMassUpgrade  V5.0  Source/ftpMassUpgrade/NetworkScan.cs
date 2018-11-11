using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ftpMassUpgrade
{
    class NetworkScan
    {
        const int MAX_DEVICE_SUPPORTED = 64;
        TeSysTMbAccess t6tAccess { get; set; }       
        public Thread scanThread { get; set; }      
        public tcpNIpInfoStruct portInfo { get; set; }           
        public deviceEntry[] deviceList { get; set; }   
        public scanProgressBar_Callback scanCallback { get; set; }

        public NetworkScan(scanProgressBar_Callback cb)
        {
            t6tAccess = new TeSysTMbAccess(cb);
            deviceList = new deviceEntry[MAX_DEVICE_SUPPORTED];
            portInfo = new tcpNIpInfoStruct();
        }
        public void startScanNetwork(tcpNIpInfoStruct pInfo )
        {
            portInfo = pInfo;
            scanThread = new Thread(scanNetwork);
            scanThread.Start();
        }
        private void scanNetwork()
        {
            if (t6tAccess.scanNetwork(deviceList, portInfo)==-1)
            {
                Console.WriteLine(" !warning -> Some error encounter during network scan");
            }
        }



    }
}
