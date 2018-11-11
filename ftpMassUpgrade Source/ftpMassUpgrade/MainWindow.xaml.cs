using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Net.Cache;
//using ModbusTCP.;

public class Product

{
    public int ProductID { get; set; }
    public string ProductName { get; set; }
    public bool IsAvailable { get; set; }
}

public class deviceEntry
{
    public string deviceIP { get; set; }
    public string comRef { get; set; }
    public string kuVer { get; set; }
    public string kcVer { get; set; }
    public bool selection { get; set; }
    public string upgradeStatus { get; set; }
    
    public deviceEntry()
    {
        deviceIP        = new string(new char[25]);
        comRef          = new string(new char[25]);
        kuVer           = new string(new char[25]);
        kcVer           = new string(new char[25]);
        upgradeStatus   = new string(new char[25]);
        upgradeStatus = "failed";
        selection = false;
    }
}

enum TOOLSTATE
{
    UNINIT,
    INIT,
    SCAN,
    UPGRADE,
    STATUS_UPDATE,
    ABORT
};

namespace ftpMassUpgrade// ftpMainWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
              
        static TOOLSTATE toolState { get; set; }      
        public MainWindow mWin { get; set; }
        string clientipStr;
        StateMachine stateM;
        toolUitility utility = new toolUitility();  
        private NetworkScan networkScan { get; set; }
        netwrokInterfaceInfo pcInterfaceInfo;
        public MainWindow()
        {
            InitializeComponent();

            
            /* Initialize the Ethernet interface to combobox */
            try
            {
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus == OperationalStatus.Up)
                    {
                        cbInterface.Items.Add(nic.Name);
                        pcInterfaceInfo.ifName = nic.Name;
                        PhysicalAddress macAdd = nic.GetPhysicalAddress();
                        pcInterfaceInfo.macAdd = macAdd.ToString();
                    }
                }
            }
            catch (excepetion ex)
            {
                Console.WriteLine(ex.Message);
            }
           
            mWin = this;
           
            /* Initialization progress bar End*/
            pbScanning.Maximum = 64;
            pbScanning.Minimum = 0;
            pbScanning.Value = 0;

            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "IP Address";
            col1.Binding = new Binding("deviceIP");
            col1.IsReadOnly = true;
            grdDeviceList.Columns.Add(col1);
                        
            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Com Reference";
            col2.Binding = new Binding("comRef");
            col2.IsReadOnly = true;
            col2.Visibility = 0;
            grdDeviceList.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "Controller FW Version";
            col3.Binding = new Binding("kuVer");
            col3.IsReadOnly = true;
            grdDeviceList.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Header = "Netwrok FW Version";
            col4.Binding = new Binding("kcVer");
            col4.IsReadOnly = true;
            grdDeviceList.Columns.Add(col4);

            networkScan = new NetworkScan(scanProgressBar_Callback);

        }
        /*********************************Main Window load initialization END******************************************************************************/

        public void scanProgressBar_Callback(scanInfo sInfo)
        {
            Action cmd;

            try
            {
                if (sInfo == null)
                    return;
                if (sInfo.actOn == actionOnControl.PROGRESS_BAR)
                {
                    if (sInfo.pvVal == 0)
                    {
                        /* set progress bar to  */
                        cmd = () => mWin.pbScanning.Maximum = sInfo.pvMax;
                        Application.Current.Dispatcher.Invoke(cmd);
                        cmd = () => mWin.pbScanning.Minimum = sInfo.pvMin;
                        Application.Current.Dispatcher.Invoke(cmd);
                        cmd = () => mWin.pbScanning.Value = sInfo.pvVal;
                        Application.Current.Dispatcher.Invoke(cmd);
                    }
                    else
                    {
                        cmd = () => mWin.pbScanning.Value = sInfo.pvVal;
                        Application.Current.Dispatcher.Invoke(cmd);
                    }
                }
                else if (sInfo.actOn == actionOnControl.LEBEL)
                {
                    cmd = () => mWin.lbIConStatus.Content = sInfo.IPAddress;
                    Application.Current.Dispatcher.Invoke(cmd);
                }
                else if (sInfo.actOn == actionOnControl.DATA_GRID)
                {
                    cmd = () => mWin.grdDeviceList.Items.Add(sInfo.deviceInfo);
                    Application.Current.Dispatcher.Invoke(cmd);
                }
                else if(sInfo.actOn == actionOnControl.PROCESS_COMPLETED)
                {              
                    cmd = () => mWin.lbIConStatus.Content = "";
                    Application.Current.Dispatcher.Invoke(cmd);

                    cmd = () => mWin.pbScanning.Value=0 ;
                    Application.Current.Dispatcher.Invoke(cmd);

                    cmd = () => mWin.lbIConStatus.Content = "Completed"; 
                    Application.Current.Dispatcher.Invoke(cmd);

                    cmd = () => mWin.btScan.IsEnabled = true;
                    Application.Current.Dispatcher.Invoke(cmd);
                }
                else
                {
                    Console.WriteLine(" Worng control selected for update scan status ");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        private void cbIPAddress_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clientipStr = cbIPAddress.SelectedItem.ToString();
            pcInterfaceInfo.ipAdd = clientipStr;
            ConfigManager.setNetwrokInterfaceInfo(pcInterfaceInfo);
            btScan.IsEnabled = true;
            
        }

        private void cbInterface_MouseDoubleClick(object sender, EventArgs  e)
        {
            for (int i = 0; i < cbInterface.Items.Count; i++)
            {
                cbInterface.Items.RemoveAt(i);
            }

            try
            {
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus == OperationalStatus.Up)
                    {
                        cbInterface.Items.Add(nic.Name);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            MessageBox.Show("CB interface clicked");
        }

    /*********************************END******************************************************************************/
        private void cbInterface_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                
                for (int i = 0; i < cbIPAddress.Items.Count; i++)
                {
                    cbIPAddress.Items.RemoveAt(i);
                }
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus == OperationalStatus.Up && nic.Name == cbInterface.SelectedItem.ToString())
                    {

                        foreach (UnicastIPAddressInformation ip in nic.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                cbIPAddress.Items.Add(ip.Address.ToString());
                            }
                        }
                    }
                }
            }

            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /*********************************END******************************************************************************/
      
/*********************************END******************************************************************************/
        private void btScan_Click(object sender, RoutedEventArgs e)
        {
            
            if (txtIPRange.Text == " ")
            {
                MessageBox.Show(" Range value should be 0 to 255");
            }
            if (txtIPStart.Text == " ")
            {
                MessageBox.Show(" Start Address can't be empty  ");
            }

            try
            {
                UInt16 range = Convert.ToUInt16(txtIPRange.Text);
                if (utility.validateIPAddress(txtIPStart.Text) == false)
                {
                    MessageBox.Show(" No a valid IP address ");
                }
                else
                {
                    lbIConStatus.Content = "";
                    pbScanning.Value = 0;
                    int lstCnt= grdDeviceList.Items.Count;
                    for(int i=0;i<lstCnt;i++)
                    {
                        grdDeviceList.Items.RemoveAt(0);
                    }
                    btScan.IsEnabled = false;
                    
                    //MessageBox.Show("start Scan");
                    tcpNIpInfoStruct portInfo = new tcpNIpInfoStruct();
                    portInfo.clientIP = clientipStr;
                    portInfo.clientPort = 0; // any available port
                    portInfo.serverIP = txtIPStart.Text.ToString();
                    portInfo.serverPort = 502; // always modbus port
                    portInfo.nDevice = range;
                    networkScan.startScanNetwork(portInfo);
                    lbIConStatus.Content = "Scanning ....";
                                   
                }              
            }
            catch
            {
                MessageBox.Show(" Only interger allowed ");
            }
            
        }

       
        /* upgrade button click*/
        private void btUpgrade_Click(object sender, RoutedEventArgs e)
        {
            int nDevice = grdDeviceList.SelectedItems.Count;
            deviceEntry[] entry = new deviceEntry[nDevice];
            try
            {
                if (nDevice < 1)
                {
                    MessageBox.Show("Please select at least one device to be upgraded ");
                    return;
                }
                for (int i = 0; i < nDevice; i++)
                {
                    entry[i] = (deviceEntry)grdDeviceList.SelectedItems[i];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            Upgrade updateWin = new Upgrade(ref entry, nDevice);
            updateWin.Show();

        }

        public void btTest_Click(object sender, RoutedEventArgs e)
        {
            //int mod = 255 % 10;
            //int div = 255 / 10;
            //MessageBox.Show(mod.ToString());
            //MessageBox.Show(div.ToString());
            //lbIConStatus.Content += mod.ToString();
            // PostDatatoFTP(0);
            // btTest.IsEnabled = false;
            //ftpupgrade.startUpgradeTask();
            // upgrdThread = new Thread(new ThreadStart(mWin.PostDatatoFTP));
            //upgrdThread.Start();
            //string IP = "192.168.2.114";
            //long IPA;
            //tcpUitility uitility = new tcpUitility();
            //IPA = uitility.IP2LongEndianLess(IP);
            //MessageBox.Show(IPA.ToString());
            //IPA++;
            //IP = uitility.LongToIP(IPA);
            //MessageBox.Show(IP);

            //winUpdateCntrl[0].pb.Maximum = 10;
            //winUpdateCntrl[0].pb.Minimum = 1;
            //winUpdateCntrl[0].pb.Value = 0;

            //winUpdateCntrl[0].lb.Content = "192.168.2.101";


        }


        private void btMbAccess_Click(object sender, RoutedEventArgs e)
        {
            ModbusAccess mbAccessWin = new ModbusAccess();
            mbAccessWin.Show();         
        }


        private void btBackup_Click(object sender, RoutedEventArgs e)
        {
            backupRestore brWin = new backupRestore();
            try
            {
                int selectedLst = grdDeviceList.SelectedItems.Count;

                if (selectedLst < 1)
                {
                    MessageBox.Show("Please select at least one device to be Backed-up or Restored");
                    return;
                }

                for (int i = 0; i < selectedLst; i++)
                {
                    brWin.grdBnRDeviceList.Items.Add(grdDeviceList.SelectedItems[i]);
                }
            }
            catch( excepetion ex)
            {
                Console.WriteLine(ex.Message);
            }         

            brWin.Show();
            
          
        }


        int index;
        private void btTestProgrssBar_Click(object sender, RoutedEventArgs e)
        {
            
            
            for (int i = 0; i < 5; i++)
            {
                deviceEntry entry = new deviceEntry();
                entry.comRef = "LTMR08EBD";
                entry.deviceIP = "122.122.122." + index.ToString();
                entry.kcVer = "4.5.0";
                entry.kuVer = "3.7.0";

                entry.kcVer = "4.5.0" + index.ToString();
                entry.kuVer = "3.7.0" + index.ToString();
                grdDeviceList.Items.Add(entry);
                Console.WriteLine(index);
                index++;
                
            }

        }



        /*********************************END******************************************************************************/
    }// main window
}//class 

