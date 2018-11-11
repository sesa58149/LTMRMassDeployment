using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace ftpMassUpgrade
{
    public delegate int dgrUpdateListRead_CallBack(deviceEntry[] devEntry);

    public class  upgradeFileInfo
    {
        public string srcFilePath;
        public string srcFileName;
        public string dstFilePath;
        public string dstFileName;
        public upgradeFileInfo()
        {
            srcFileName = "";
            srcFilePath = "";
            dstFileName = "";
            dstFilePath = "";
        }
    }

    public class StateMachine
    {
        enum TOOLSTATE
        {
            UNINIT,
            INIT,
            SCAN,
            UPGRADE,
            STATUS_UPDATE,
            ABORT,
            IDEL
        };
        TeSysTMbAccess t6tAccess { get; set; }
        upgradeTask ftpupdgr { get; set; } // = new upgradeTask();
        public Thread updateThread { get; set; }
        static TOOLSTATE toolState { get; set; }
        public upgradeFileInfo fileInfo { get; set; }
        public tcpNIpInfoStruct portInfo { get; set; }
        public string clientIP { get; set; }
        public string serverIP { get; set; }
        public UInt16 clientPort { get; set; }
        public UInt16 serverPort { get; set; }
        public UInt16 noOfDevice { get; set; }
        public toolUitility utility { get; set; }
        public deviceEntry [] deviceList { get; set; }
        public deviceEntry [] upgradedDeviceList { get; set; }
        public deviceEntry tmpEntry { get; set; }
        public int deviceListIndex { get; set; }
        public MainWindow mainW { get; set; }
        public bool btScanDisabled { get; set; }
        public dgrUpdateListRead_CallBack upgrade_Callback;

        public StateMachine()
        {

        }

        public StateMachine(scanProgressBar_Callback cb, dgrUpdateListRead_CallBack ucb, callback_ProgressBar cpb )
        {            
            updateThread = new Thread(new ThreadStart(updateTask));
            t6tAccess = new TeSysTMbAccess( cb);
            ftpupdgr = new upgradeTask(cpb);
            toolState = TOOLSTATE.UNINIT;
            upgrade_Callback = ucb;
            utility = new ftpMassUpgrade.toolUitility();           
            clientPort = 0;
            serverPort = 0;
            noOfDevice = 0;          
            deviceList = new deviceEntry[250];
            upgradedDeviceList = new deviceEntry[250];
            tmpEntry = new deviceEntry();         
            btScanDisabled = false;
            portInfo = new tcpNIpInfoStruct();
            fileInfo = new upgradeFileInfo();

        }
        public void setUpgradeFileInfo(upgradeFileInfo ifo)
        {
            fileInfo = ifo;
        }
        public void setMainWindowAddress( MainWindow mptr)
        {
            mainW = mptr;
        }
        public void startStateMachine()
        {
            updateThread.Start();
            toolState = TOOLSTATE.INIT;

        }
        public void setClientServerPortInfo( tcpNIpInfoStruct pInfo)
        {
            portInfo = pInfo;
        }
       
        public void setState(int state)
        {
            toolState = (TOOLSTATE) state;
        }
        /* Create state machine task*/
        
      
        void updateTask()
        {
            int readErr = -1;
            while (toolState == TOOLSTATE.UNINIT)
            {
                MessageBox.Show(" task not init");
            }

            while (toolState != TOOLSTATE.UNINIT)
            {
                switch (toolState)
                {
                    case TOOLSTATE.INIT:
                        
                        Console.WriteLine("In Init state");
                        Thread.Sleep(1000);
                        break;

                    case TOOLSTATE.SCAN:                        
                        
                        if (t6tAccess.scanNetwork(deviceList, portInfo) ==-1)
                        {
                            Console.WriteLine(" !warning -> Some error encounter during network scan");
                        }
                        toolState = TOOLSTATE.IDEL;                            
                        break;

                    case TOOLSTATE.UPGRADE:
                        
                        Console.WriteLine("In Upgrade state");
                        
                        noOfDevice = (ushort) upgrade_Callback(upgradedDeviceList);
                        ftpupdgr.startUpgradeTask(upgradedDeviceList, noOfDevice, fileInfo);

                        toolState = TOOLSTATE.STATUS_UPDATE;
                        Thread.Sleep(1000);
                        break;
                    case TOOLSTATE.STATUS_UPDATE:
                        //MessageBox.Show("in Status update");

                        //ftpupdgr.startUpgrade();
                        Console.WriteLine("in Status update");
                        toolState = TOOLSTATE.INIT;
                        Thread.Sleep(1000);
                        break;

                    case TOOLSTATE.ABORT:
                        //MessageBox.Show("in Abort State");
                        Console.WriteLine("in Abort State");
                        toolState = TOOLSTATE.INIT;
                        Thread.Sleep(1000);
                        break;
                    case TOOLSTATE.IDEL:

                        Console.WriteLine("In Idel state");
                        Thread.Sleep(1000);
                        break;
                }// switch
                Thread.Sleep(1000);
            }//while
        }
        /*********************************Task END********************************************/
    }
}
