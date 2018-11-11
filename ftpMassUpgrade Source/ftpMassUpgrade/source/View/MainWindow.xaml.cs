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

public class winControl
{
    public ProgressBar pb { get; set; }
    public Label lb { get; set; }
   
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

namespace ftpMassUpgrade// ftpMainWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum TOOLSTATE{
            UNINIT,
            INIT,
            SCAN,
            UPGRADE,
            STATUS_UPDATE,
            ABORT
        };
        public winControl[] winUpdateCntrl=new winControl[8];
        static TOOLSTATE toolState { get; set; }
        public bool upgrdStart { get; set; }
        public MainWindow mWin { get; set; }
        ProgressBar[] pb = new ProgressBar[8];
        Label[] lb = new Label[8];
        mbTCP myClient;
        mbTCP mbtcp;
        string clientipStr;
        StateMachine stateM;
        toolUitility utility = new toolUitility(); 
        public UInt16[] regVal = new UInt16[120];

        public string srcFileName;
        public string srcFilePath;
        public string tmpFolderPath;
        public int grdIndex=0;
        private struct ltmrConfig
        {
            public ushort address;
            public ushort data;
        }
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
            catch
            {

            }

            UInt16[] regVal = new UInt16[120];
            mWin = this;
            /* initialize progress bar for update process*/

            pb[0] = pbUpdate_0;
            lb[0] = lblIPAdd_0;
            pb[1] = pbUpdate_1;
            lb[1] = lblIPAdd_1;
            pb[2] = pbUpdate_2;
            lb[2] = lblIPAdd_2;
            pb[3] = pbUpdate_3;
            lb[3] = lblIPAdd_3;
            pb[4] = pbUpdate_4;
            lb[4] = lblIPAdd_4;
            pb[5] = pbUpdate_5;
            lb[5] = lblIPAdd_5;
            pb[6] = pbUpdate_6;
            lb[6] = lblIPAdd_6;
            pb[7] = pbUpdate_7;
            lb[7] = lblIPAdd_7;
            /* Initialization progress bar End*/


            pbScanning.Maximum = 160;
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



            // update gride
            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Header = "IP Address";
            col5.Binding = new Binding("deviceIP");
            col5.IsReadOnly = true;
            grdUpdateList.Columns.Add(col5);

            DataGridTextColumn col6 = new DataGridTextColumn();
            col6.Header = "Com Reference";
            col6.Binding = new Binding("comRef");
            col6.IsReadOnly = true;
            col6.Visibility = 0;
            grdUpdateList.Columns.Add(col6);

            DataGridTextColumn col7 = new DataGridTextColumn();
            col7.Header = "Controller FW Version";
            col7.Binding = new Binding("kuVer");
            col7.IsReadOnly = true;
            grdUpdateList.Columns.Add(col7);

            DataGridTextColumn col8 = new DataGridTextColumn();
            col8.Header = "Netwrok FW Version";
            col8.Binding = new Binding("kcVer");
            col8.IsReadOnly = true;
            grdUpdateList.Columns.Add(col8);

            //DataGridCheckBoxColumn col5 = new DataGridCheckBoxColumn();
            //col5.Header = "Selection";
            //col5.Binding = new Binding("selection");
            //col5.IsReadOnly = true;
            //grdDeviceList.Columns.Add(col5);

            DataGridTextColumn col9 = new DataGridTextColumn();
            col9.Header = "Upgrade Status";
            col9.Binding = new Binding("upgradeStatus");
            col9.IsReadOnly = true;
            grdUpdateList.Columns.Add(col9);

            //ftpupgrade = new upgradeTask();
            //ftpupgrade.setMainWindowAddress(this);

            // mbtcp = new mbTCP();

            toolState = TOOLSTATE.UNINIT;

            stateM = new StateMachine(scanProgressBar_Callback,dgrUpdateListRead_CallBack,updateProgressBar_Callback);             
            stateM.setMainWindowAddress(this);            
            stateM.startStateMachine();

        }
        /*********************************Main Window load initialization END******************************************************************************/

        public void updateProgressBar_Callback(updateInfo pbInfo)
        {
            Action cmd;
            try
            {
                if(pbInfo.actOn == actionOnControl.NO_ACTION)
                {
                    return;
                }

                if (pbInfo.winControlIndex == 0xFF)
                {
                    Console.WriteLine("Progress bar is uninitialized ");
                }
                if (pbInfo.actOn == actionOnControl.PROGRESS_BAR)
                {
                    if (pbInfo.pbVal == 0)
                    {
                        cmd = () => mWin.pb[pbInfo.winControlIndex].Maximum = pbInfo.pbMax;
                        Application.Current.Dispatcher.Invoke(cmd);

                        cmd = () => mWin.pb[pbInfo.winControlIndex].Minimum = pbInfo.pbMin;
                        Application.Current.Dispatcher.Invoke(cmd);

                        // IP address lebel clean up
                        cmd = () => mWin.lb[pbInfo.winControlIndex].Content = " ";
                        Application.Current.Dispatcher.Invoke(cmd);

                    }
                    else
                    {
                        cmd = () => mWin.lb[pbInfo.winControlIndex].Content = pbInfo.serverIpAddress;
                        Application.Current.Dispatcher.Invoke(cmd);
                    }

                    cmd = () => mWin.pb[pbInfo.winControlIndex].Value = pbInfo.pbVal;
                    Application.Current.Dispatcher.Invoke(cmd);
                }
                else if (pbInfo.actOn == actionOnControl.DATA_GRID)
                {
                    cmd = () => mWin.grdUpdateList.Items.Add(pbInfo.dEntry);
                    Application.Current.Dispatcher.Invoke(cmd);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
          
        }

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

                    cmd = () => mWin.lbICon.Content = "Completed"; 
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
            //for (int i = 0; i < 10; i++)
            //{
            //    grdDeviceList.Items.Add(new deviceEntry() { deviceIP = "192.168.2.1", comRef = "EBD", kuVer = "2.8.000", kcVer = "3.1.000", selection = false, upgradeStatus = "..." });
            //}
            
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
                    lbICon.Content = "";
                    pbScanning.Value = 0;
                    int lstCnt= grdDeviceList.Items.Count;
                    for(int i=0;i<lstCnt;i++)
                    {
                        grdDeviceList.Items.RemoveAt(0);
                    }
                   // btScan.IsEnabled = false;
                    stateM.btScanDisabled = true;
                    //MessageBox.Show("start Scan");
                    tcpNIpInfoStruct portInfo = new tcpNIpInfoStruct();
                    portInfo.clientIP = clientipStr;
                    portInfo.clientPort = 0; // any available port
                    portInfo.serverIP = txtIPStart.Text.ToString();
                    portInfo.serverPort = 502; // always modbus port
                    portInfo.nDevice = range;
                    stateM.setClientServerPortInfo(portInfo);                    
                    toolState = TOOLSTATE.SCAN;
                    lbICon.Content = "Scanning ....";
                    stateM.setState((int)toolState);                    
                }              
            }
            catch
            {
                MessageBox.Show(" Only interger allowed ");
            }
            
        }

        public int dgrUpdateListRead_CallBack(deviceEntry[] deviceList)
        {
            Action cmd;
            int nDevice = mWin.grdUpdateList.Items.Count;
            deviceEntry entry = new deviceEntry();
            try
            {
                for (int i = 0; i < nDevice; i++)
                {
                    entry = (deviceEntry)mWin.grdUpdateList.Items[i];
                    deviceList[i] = entry;
                }
                // clean the to update status of upgraded device
                for (int i = 0; i < nDevice; i++)
                {
                    cmd = () => mWin.grdUpdateList.Items.RemoveAt(0);
                    Application.Current.Dispatcher.Invoke(cmd);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return nDevice;
        }
        /* upgrade button click*/
        private void btUpgrade_Click(object sender, RoutedEventArgs e)
        {
            upgradeFileInfo uFileinfo = new upgradeFileInfo();
            uFileinfo.srcFileName = srcFileName;
            uFileinfo.srcFilePath = srcFilePath;
            
            try
            {
                stateM.setUpgradeFileInfo(uFileinfo);
                stateM.setState((int)TOOLSTATE.UPGRADE);

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

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

            winUpdateCntrl[0].pb.Maximum = 10;
            winUpdateCntrl[0].pb.Minimum = 1;
            winUpdateCntrl[0].pb.Value = 0;

            winUpdateCntrl[0].lb.Content = "192.168.2.101";


        }
       
   
        private void btBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            try
            {
                if (dlg.ShowDialog() == true)
                {
                    string filePath = dlg.FileName;
                    
                    srcFilePath = dlg.FileName;
                    
                    string fileName = System.IO.Path.GetFileName(filePath);
                    txtBrowse.Text = fileName;
                    srcFileName = fileName;
                   
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public Int32 testCmd(string btFile, string path)
        {
            Int32 retCode = -1;
            string add = path + "\\" + btFile;
            try
            {
                //var cmd = new System.Diagnostics.Process();
                ////cmd.StartInfo.FileName = btFile;
                //cmd.StartInfo.FileName = add;
                //cmd.StartInfo.UseShellExecute = false;
                //cmd.StartInfo.RedirectStandardInput = true;
                ////cmd.StartInfo.WorkingDirectory = path;
                //cmd.Start();
                //cmd.WaitForExit();
                //cmd.Close();



                Int32 exitCode = -1;
                ProcessStartInfo processInfo;
                Process process;
                string cmd = "ftp -s:ftpcmd.txt";
                //processInfo = new ProcessStartInfo("cmd.exe", "/c " + cmd);
                processInfo = new ProcessStartInfo("cmd.exe", cmd);
                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = true;
                // *** Redirect the output ***
                processInfo.RedirectStandardError = true;
                processInfo.RedirectStandardOutput = true;
                processInfo.FileName = add;
                processInfo.WorkingDirectory = path;

                process = Process.Start(processInfo);
                process.WaitForExit();

                // *** Read the streams ***
                // Warning: This approach can lead to deadlocks, see Edit #2
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                exitCode = process.ExitCode;

                Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
                Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
                Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
                process.Close();
                return exitCode;

               // ftpClient ftp;

                retCode = 0;
            }
            catch (SystemException e)
            {
                Console.Write(e.Message.ToString());

            }
            return retCode;
        }

        private void frmIpAddress_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void DataGrid_SelectionCellChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            
            //try
            //{               
            //    deviceEntry tmpdenty = new deviceEntry();
                
            //    int lstCnt = grdDeviceList.Items.Count;
            //    int index = grdDeviceList.SelectedIndex;
                
            //    var tmp = grdDeviceList.Items.GetItemAt(index);
            //    tmpdenty = (deviceEntry)tmp;

            //    if (tmpdenty.selection == true)
            //    {
            //        tmpdenty.selection = false;
            //    }
            //    else
            //    {
            //        tmpdenty.selection = true;
            //    }

            //    //grdDeviceList.Items.RemoveAt(index);
            //    grdDeviceList.Items.Insert(index, tmpdenty);

            //}
            //catch
            //{
            //    //MessageBox.Show("exception");
            //}
        }

        private void btEntryDataGrid_Click(object sender, RoutedEventArgs e)
        {

            //ProgressBar pb = pbUpdate_0;
            //pb.Maximum = 10;
            //pb.Minimum = 0;
            //pb.Value = 5;

            //deviceEntry denty = new deviceEntry();

            //denty.deviceIP = "192.168.2.100";
            //denty.comRef = "LBED";
            //denty.kcVer = (grdIndex++.ToString())+ ".1.000";
            //denty.kuVer = "2.2.000";
            //denty.selection = false;
            //denty.upgradeStatus = "...";


            //grdDeviceList.Items.Add(denty);

            //winUpdateCntrl[0].pb.Maximum = 10;
            //winUpdateCntrl[0].pb.Minimum = 1;
            //winUpdateCntrl[0].pb.Value = 0;

            //winUpdateCntrl[0].lb.Content = "192.168.2.101";

            string ip = "192.168.2.1";
            string newIP = ip.Replace('.', '_');
            MessageBox.Show(newIP);



        }
        
      
        /* read custom logic file from LTMR*/
        private void readLTMRLogicFile()
        {
            byte tokenId = 0;
            int pageId = 0;
            int wordRemain = 0;
            int logicFileSize = 0;
            UInt16[] regVal = new UInt16[32];
            byte[] data = new byte[512];
            int failCode = 0;
            int isInConfigMode = 0;
            StreamWriter fWriter = null;
            /* enter in config mode before starting custom logic file access it is mandatory requirment */
            //if (comm.Read(ViewModelConstants.NodeNumber, 601, 1, ref regVal) == true)
            if(myClient.mbRead(601,1, regVal) > 0 )
            {
                isInConfigMode = regVal[0] & 0x0001;
                if (isInConfigMode == 0)
                {
                    regVal[0] |= 0x0001;
                    //if (comm.Write(ViewModelConstants.NodeNumber, 601, 1, regVal) == false)
                    if (myClient.mbWrite(601, 1, regVal) ==-1)
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
            if (myClient.mbRead(1203, 1, regVal) > 0)
            {
                logicFileSize = regVal[0];
                wordRemain = logicFileSize+1;// 1 byte added for len of file
                if (logicFileSize > 0)
                {
                    /* get  the token to access the logic file available in device. */
                    //if (comm.Read(ViewModelConstants.NodeNumber, 1209, 1, ref regVal) == true)
                    if (myClient.mbRead(1209, 1, regVal) > 0)
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

                try
                {
                    fWriter = new StreamWriter("logicFile.csv");
                }
                catch (FileLoadException e)
                {
                    MessageBox.Show(" Fail to open File -- may be used by other application ");
                }
                while (wordRemain > 0)
                {


                    /* Tell device to want access logical memory section (Custom logic) */
                    regVal[0] = 1;
                    //if (comm.Write(ViewModelConstants.NodeNumber, 1207, 1, regVal) == true)
                    if(myClient.mbWrite(1207,1, regVal)==0)
                    {
                        /* Tell device the page number wants to read */
                        regVal[0] = (ushort)pageId;
                        //if (comm.Write(ViewModelConstants.NodeNumber, 1208, 1, regVal) == true)
                        if (myClient.mbWrite(1208,1,regVal)==0)
                        {
                            int cmd = tokenId * 256; // token in MSB
                            cmd += 1; //read command
                            regVal[0] = (ushort) cmd;

                            /*Send command to device telling READ with provided token*/
                            //if (comm.Write(ViewModelConstants.NodeNumber, 1209, 1, regVal) == true)
                            if (myClient.mbWrite(1209, 1, regVal) == 0)

                            {
                                int nReg = (wordRemain > 16) ? 16 : wordRemain;

                                /* read custom logic file of Max size 16 register/Word of asked pageId */
                                // if (comm.Read(ViewModelConstants.NodeNumber, 1210, (ushort)nReg, ref regVal) == true)
                                if (myClient.mbRead(1210, (ushort)nReg, regVal) > 0)
                                {
                                    /* incereament tokenId by one every time for new request*/
                                    tokenId++;
                                    /* incereament pageId by Max Page size every time for new page request*/
                                    pageId+= 16; 
                                    wordRemain -= nReg;
                                    /* dump this page in file and go for next pageId*/
                                    for (int i = 0; i < nReg; i++)
                                    {
                                        fWriter.WriteLine(regVal[i].ToString());
                                    }
                                }
                                if (wordRemain <= 0)
                                {
                                    /* Release the token after uploading complete logic file */
                                    cmd = tokenId * 256; // token in MSB
                                    cmd += 5; //release token command
                                    regVal[0] = (ushort) cmd;
                                    //if (comm.Write(ViewModelConstants.NodeNumber, 1209, 1, regVal) == true)
                                    if (myClient.mbWrite(1209, 1, regVal) == 0)
                                    {
                                        MessageBox.Show("Logical file backup successful ");
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
                fWriter.Close();
                if (failCode == -1)
                {
                    MessageBox.Show("logical file Backup failed");
                }
            }

            /* Exit from config mode if enter in this fuction else nothing to do */
            if (isInConfigMode == 0)
            {
                //if (comm.Read(ViewModelConstants.NodeNumber, 601, 1, ref regVal) == true)
                if (myClient.mbRead(601, 1, regVal) > 0)
                {
                    regVal[0] &= 0xFFFE;
                    //if (comm.Write(ViewModelConstants.NodeNumber, 601, 1, regVal) == false)
                    if(myClient.mbWrite(601,1,regVal)==-1)
                    {
                        MessageBox.Show("Failed to exit from config mode");
                    }

                }
            }
        }

        
        /* Read LTMR configuration including host configuration register and 
         * custom logic file over Modbus serial line(HMI Port) */
        private void readLTMRConfiguration()
        {
            ushort modbusStartAddress;
            ushort numberofRegister;
            byte[] data = new byte[512];
            UInt16[] regVal = new UInt16[120];
            int index = 0;

            //Register Address 540-599, 600-699,800-898,1250-1269,3000-3087,3088-3119)
            StreamWriter confFile = new StreamWriter("conf.csv");
            confFile.Write("Register Address" + ";" + " value " + " \n");

            myClient.mbRead(64, 10, regVal);
           
            modbusStartAddress = 540;
            numberofRegister = 60;
            //if (comm.Read(ViewModelConstants.NodeNumber, modbusStartAddress, numberofRegister, ref data) == true)
            if (myClient.mbRead(modbusStartAddress, numberofRegister, regVal) > 0)
            {
           
                for (int i = 0; i < numberofRegister; i++)
                {
                    confFile.Write((modbusStartAddress + i).ToString() + ";" + regVal[i].ToString() + " \n");
                }
           
                index = index + numberofRegister;
                modbusStartAddress = 600;
                numberofRegister = 100;
                //if (comm.Read(ViewModelConstants.NodeNumber, modbusStartAddress, numberofRegister, ref data) == true)
                if (myClient.mbRead(modbusStartAddress, numberofRegister, regVal) > 0)
                {
                    //confFile.Write("  Main Setting     " + " \n");
                    for (int i = 0; i < numberofRegister; i++)
                    {
                        confFile.Write((modbusStartAddress + i).ToString() + ";" + regVal[i].ToString() + " \n");
                    }
                   
                    index = index + numberofRegister;
                    modbusStartAddress = 800;
                    numberofRegister = 99;
                    //if (comm.Read(ViewModelConstants.NodeNumber, modbusStartAddress, numberofRegister, ref data) == true)
                    if (myClient.mbRead(modbusStartAddress, numberofRegister, regVal) > 0)
                    {
                        for (int i = 0; i < numberofRegister; i++)
                        {
                            confFile.Write((modbusStartAddress + i).ToString() + ";" + regVal[i].ToString() + " \n");
                        }
                        index = index + numberofRegister;
                        modbusStartAddress = 1250;
                        numberofRegister = 20;
                        //if (comm.Read(ViewModelConstants.NodeNumber, modbusStartAddress, numberofRegister, ref data) == true)
                        if (myClient.mbRead(modbusStartAddress, numberofRegister, regVal) > 0)
                        {
                            for (int i = 0; i < numberofRegister; i++)
                            {
                                confFile.Write((modbusStartAddress + i).ToString() + ";" + regVal[i].ToString() + " \n");
                            }
                            index = index + numberofRegister;
                            modbusStartAddress = 3000;
                            numberofRegister = 88;
                            // if (comm.Read(ViewModelConstants.NodeNumber, modbusStartAddress, numberofRegister, ref data) == true)
                            if (myClient.mbRead(modbusStartAddress, numberofRegister, regVal) > 0)
                            {
                                for (int i = 0; i < numberofRegister; i++)
                                {
                                    confFile.Write((modbusStartAddress + i).ToString() + ";" + regVal[i].ToString() + " \n");
                                }
                                index = index + numberofRegister;
                                modbusStartAddress = 3088;
                                numberofRegister = 32;
                                // if (comm.Read(ViewModelConstants.NodeNumber, modbusStartAddress, numberofRegister, ref data) == true)
                                if (myClient.mbRead(modbusStartAddress, numberofRegister, regVal) > 0)
                                {
                                    for (int i = 0; i < numberofRegister; i++)
                                    {
                                        confFile.Write((modbusStartAddress + i).ToString() + ";" + regVal[i].ToString() + " \n");
                                    }
                                    index = index + numberofRegister;

                                }
                            }
                        }
                    }

                }

            }
            confFile.Close();
            /* check Operating mode before starting uploading custom logic file*/
            //if (comm.Read(ViewModelConstants.NodeNumber, 540, 1, ref data) == true)
            if(myClient.mbRead(540, 1, regVal) > 0)
            {
                /* Custom logic file will present if Reg(540)>255 else no custom logic file hance egnor reading it*/
                if (regVal[0] > 255)
                {
                    readLTMRLogicFile();
                }

            }
            MessageBox.Show("backup Completed");

        }

        private void writeReg()
        {
            ushort[] reg = new ushort[10];
            byte[] data = new byte[512];
            ushort[] readVal = new ushort[10];

            reg[0]= (ushort)Convert.ToInt16 ( txtBMWrite.Text);
            if (myClient.mbWrite(600, 1, reg)==0)
            {
                myClient.mbRead(600, 1, readVal);
                //copyByteToRegister(readVal, data,1);
                MessageBox.Show(readVal[0].ToString());

            }
            else
            {
                MessageBox.Show(" MB Write failed");
            }
        }

        private void btBackup_Click(object sender, RoutedEventArgs e)
        {
            backupRestore brWin = new backupRestore();
            try
            {
                int selectedLst = grdDeviceList.SelectedItems.Count;

                if (selectedLst < 1)
                {
                    MessageBox.Show("Please select at leat one device to be Backed-up or Restored");
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


        private void storeLTMRConfiguration()
        {
            ltmrConfig[] conf = new ltmrConfig[4048];
            UInt16[] logicToken = new UInt16[10240];
           
            int i = 0;
            //Register Address 540-599, 600-699,800-898,1250-1269,3000-3087,3088-3119)
            if (File.Exists("conf.csv") == false)
            {
                MessageBox.Show("No Backup Found");
                return;
            }

            StreamReader confFile = new StreamReader("conf.csv");
            //read header line not data from this line
            string strLine = confFile.ReadLine();

            for (i = 0; i < 60; i++)  //60 reg
            {
                strLine = confFile.ReadLine();
                int pos = strLine.IndexOf(";");
                string strAdd = strLine.Substring(0, pos);
                conf[i].address = Convert.ToUInt16(strAdd);
                string strData = strLine.Substring(pos + 1);
                conf[i].data = Convert.ToUInt16(strData);
            }

            for (i = 60; i < 160; i++) //100 reg
            {
                strLine = confFile.ReadLine();
                int pos = strLine.IndexOf(";");
                string strAdd = strLine.Substring(0, pos);
                conf[i].address = Convert.ToUInt16(strAdd);
                string strData = strLine.Substring(pos + 1);
                conf[i].data = Convert.ToUInt16(strData);
            }
            for (i = 160; i < 259; i++) //99 reg
            {
                strLine = confFile.ReadLine();
                int pos = strLine.IndexOf(";");
                string strAdd = strLine.Substring(0, pos);
                conf[i].address = Convert.ToUInt16(strAdd);
                string strData = strLine.Substring(pos + 1);
                conf[i].data = Convert.ToUInt16(strData);
            }

            for (i = 259; i < 279; i++)//20 Reg
            {
                strLine = confFile.ReadLine();
                int pos = strLine.IndexOf(";");
                string strAdd = strLine.Substring(0, pos);
                conf[i].address = Convert.ToUInt16(strAdd);
                string strData = strLine.Substring(pos + 1);
                conf[i].data = Convert.ToUInt16(strData);
            }

            for (i = 279; i < 367; i++) //88 reg
            {
                strLine = confFile.ReadLine();
                int pos = strLine.IndexOf(";");
                string strAdd = strLine.Substring(0, pos);
                conf[i].address = Convert.ToUInt16(strAdd);
                string strData = strLine.Substring(pos + 1);
                conf[i].data = Convert.ToUInt16(strData);
            }
            for (i = 367; i < 399; i++) // 32 reg
            {
                strLine = confFile.ReadLine();
                int pos = strLine.IndexOf(";");
                string strAdd = strLine.Substring(0, pos);
                conf[i].address = Convert.ToUInt16(strAdd);
                string strData = strLine.Substring(pos + 1);
                conf[i].data = Convert.ToUInt16(strData);
            }

            confFile.Close();           

           
            /************************* write to device*************************/
            myClient = new mbTCP();
            clientipStr = "192.168.2.1";
            //string serverIP = "192.168.2.207";
            string serverIP = "192.168.2.100";
            //clientipStr = "85.16.100.0";
            //string serverIP = "85.16.177.249";

            if (myClient.modbusInit(clientipStr, 502) == 0)
            {
                if (myClient.mbOpen(serverIP, 502) != 0)
                {
                    return;
                }
            }

            int j = 0;
            for (i = 0; i < 60; i++)
            {
                regVal[j++] = conf[i].data;
            }
            myClient.mbWrite(540, 60, regVal);

            j = 0;
            for (i = 60; i < 160; i++)
            {
                regVal[j++] = conf[i].data;
            }
            myClient.mbWrite(600, 100, regVal);

            j = 0;
            for (i = 160; i < 259; i++)
            {
                regVal[j++] = conf[i].data;
            }
            myClient.mbWrite(800, 99, regVal);

            j = 0;
            for (i = 259; i < 279; i++)
            {
                regVal[j++] = conf[i].data;
            }
            //myClient.mbWrite(1200, 20, regVal);

            j = 0;
            for (i = 279; i < 367; i++)
            {
                regVal[j++] = conf[i].data;
            }
            myClient.mbWrite(3000, 88, regVal);

            j = 0;
            for (i = 367; i < 399; i++)
            {
                regVal[j++] = conf[i].data;
            }
            myClient.mbWrite(3088, 32, regVal);
            myClient.mbClose();
            /*download custome logic file*/
            if (File.Exists("logicFile.csv") == true)
            {
                confFile = new StreamReader("logicFile.csv");
                strLine = confFile.ReadLine();
                UInt16 nToken = Convert.ToUInt16(strLine);
                logicToken[0] = nToken;
                for (i = 1; i <= nToken; i++)
                {
                    strLine = confFile.ReadLine();                    
                    logicToken[i] = Convert.ToUInt16(strLine) ;  
                }
                if (downloadCustomLogic(logicToken, nToken+1) == -1) // size len is out of file size
                {
                    MessageBox.Show("failed to upload custom logic");
                }
                confFile.Close();

            }

        }

        private int downloadCustomLogic(UInt16 []token, int tokeSize)
        {
            int retCode = -1;            
            byte tokenId = 0;
            int pageId = 0;
            int wordRemain = tokeSize;            
            UInt16[] regVal = new UInt16[32];
            int isInConfigMode = 0;

            myClient = new mbTCP();
            clientipStr = "192.168.2.1";
            //string serverIP = "192.168.2.207";
            string serverIP = "192.168.2.100";
            //clientipStr = "85.16.100.0";
            //string serverIP = "85.16.177.249";

            if (myClient.modbusInit(clientipStr, 502) == 0)
            {
                if (myClient.mbOpen(serverIP, 502) != 0)
                {
                    return retCode;
                }
            }
            /* get  the token to access the logic file available in device. */
            //if (comm.Read(ViewModelConstants.NodeNumber, 1209, 1, ref regVal) == true)
            if (myClient.mbRead(1209, 1, regVal) > 0)
            {
                tokenId = (byte)(regVal[0] / 256); // taken is only MSB
            }
            else
            {
                MessageBox.Show("fail to get valid token from device");
                return  retCode;
            }

            /* enter in config mode before starting custom logic file access it is mandatory requirment */
            //if (comm.Read(ViewModelConstants.NodeNumber, 601, 1, ref regVal) == true)
            if (myClient.mbRead(601, 1, regVal) > 0)
            {
                isInConfigMode = regVal[0] & 0x0001;
                if (isInConfigMode == 0)
                {
                    regVal[0] |= 0x0001;
                    //if (comm.Write(ViewModelConstants.NodeNumber, 601, 1, regVal) == false)
                    if (myClient.mbWrite(601, 1, regVal) == -1)
                    { /* resotre can't start without entring in config mode*/
                        MessageBox.Show("fail to enter in config mode");
                        return retCode;
                    }
                }
            }
          
            /*Check the size of downloading file should be less then the supported by device */
            if (myClient.mbRead(1202, 1, regVal) > 0)
            {
               if(tokeSize> regVal[0])
                {   /* Action can't be completed as file size is larger than the supported */
                    MessageBox.Show("File size is larger then supported");
                    return retCode;
                }
            }

            /* clear all windows register which has old data*/
            regVal[0] = (UInt16) (tokenId << 8); // token
            regVal[0] += 4; //command clear all logic memory
            tokenId++;
            if (myClient.mbWrite(1209, 1, regVal) ==-1)
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
                regVal[2] = (UInt16)( (tokenId * 256)+ 3); // token in MSB + Write command(3) inLSB
                int nReg = (wordRemain > 16) ? 16 : wordRemain;
                /* Write logic token of nReg in logic window */
                int j = 0;
                for (int i = 3; i < (3+nReg); i++)
                {
                    regVal[i] = token[j + pageId];
                    j++;
                }
                //if (comm.Write(ViewModelConstants.NodeNumber, 1207, 1, regVal) == true)
                if (myClient.mbWrite(1207, (UInt16)(3 + nReg), regVal) >= 0)
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
                regVal[3] = (UInt16)(tokeSize - 1); //1210 keep 1 byte out
                tokenId++;
                if (myClient.mbWrite(1207, 4, regVal) >= 0)
                {
                    regVal[0] = (UInt16)((tokenId << 8) + 2); //1209 WRITE TO FRAM  
                    myClient.mbWrite(1209, 1, regVal);
                    tokenId++;
                    /* wait for a second to get data written over the Flash*/
                    System.Threading.Thread.Sleep(1000);
                    /* Release the token after uploading complete logic file */
                    regVal[0] = (UInt16)((tokenId << 8) + 5);
                    //if (comm.Write(ViewModelConstants.NodeNumber, 1209, 1, regVal) == true)
                    if (myClient.mbWrite(1209, 1, regVal) == -1)
                    {                        
                        retCode = -1;
                    }
                }
            }
            //if (comm.Read(ViewModelConstants.NodeNumber, 601, 1, ref regVal) == true)
            if (myClient.mbRead(601, 1, regVal) > 0)
                {
                    regVal[0] &= 0xFFFE;
                    //if (comm.Write(ViewModelConstants.NodeNumber, 601, 1, regVal) == false)
                    if (myClient.mbWrite(601, 1, regVal) == -1)
                    {
                        MessageBox.Show("Failed to exit from config mode after downloading completed");
                    }

                }
            myClient.mbClose();
            return retCode;
        }

        private void btRestore_Click(object sender, RoutedEventArgs e)
        {

             storeLTMRConfiguration();
                    

            //byte[] data = { 0x00, 0x15, 0x06, 0xd7, 0x01, 0x00, 0x00, 0x01, 0x00, 0x01 };
            //ushort[] reg = new ushort[20];
            //copyByteToRegister(reg, data, 5);

            //writeReg();
        }

        private int getFileFromDevice()
        {
            string destinationFile = "macd.dat";
            string serverPath = "ftp://192.168.2.100/"+"fw/mac.dat";

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(serverPath);

            request.KeepAlive = true;
            request.UsePassive = true;
            request.UseBinary = true;

            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential("pcfactory", "pcfactory");

            // Read the file from the server & write to destination                
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) // Error here
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(responseStream))
            using (StreamWriter destination = new StreamWriter(destinationFile))
            {
                destination.Write(reader.ReadToEnd());
                destination.Flush();
            }


            //int retVal = -1;

            //Uri myUri = new Uri("ftp://192.168.2.100/" + "Laxmi-Lab-Paperwork.pdf");
            ////Uri myUri = new Uri("ftp://192.168.2.114/" + srcFileName);
            //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(myUri);


            //try
            //{
            //    request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.CacheIfAvailable);
            //    request.Method = WebRequestMethods.Ftp.DownloadFile;
            //    request.UseBinary = true;
            //    request.Credentials = new NetworkCredential("pcfactory", "pcfactory");
            //    request.UsePassive = false;

            //    /* ftp Operation start*/
            //    Stream requestStream = request.GetRequestStream();
            //    FileStream writeFile = new FileStream("mac.dat", FileMode.Create);
            //    int byteToRead = 12345;
            //    byte[] fileContaint = new byte[byteToRead];

            //    int readByte = requestStream.Read(fileContaint,0, byteToRead);
            //    writeFile.Write(fileContaint, 0, readByte);
            //    writeFile.Close();
            //    /*FTP Close */
            //    requestStream.Close();

            //    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            //    Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);

            //    response.Close();
            //    retVal = 0;
            //}
            //catch (WebException e)
            //{
            //    Console.WriteLine(e.Message.ToString());
            //    String status = ((FtpWebResponse)e.Response).StatusDescription;
            //    Console.WriteLine(status);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message.ToString());

            //}

            return 0;
        }

        private int PostDatatoDevice()
        {
            updateInfo info = new updateInfo();
            int retVal = -1;
            int ftpFrameSize = 1400;
            // Read the source file into a byte array.
            byte[] dataToSend = new byte[ftpFrameSize];
            int bytesToSend;
            info.serverIpAddress = "192.168.2.100";
            info.winControlIndex = 1;
            info.fileName = srcFileName;
            string s1 = "ftp://192.168.2.100/" + "fw/" + srcFileName;
            string s2 = "ftp://" + info.serverIpAddress + "/fw/" + info.fileName;

            Uri myUri = new Uri("ftp://192.168.2.100/"+"fw/"+srcFileName);
            byte[] fileContents = new byte[1400];
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(myUri);
                request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.CacheIfAvailable);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential("pcfactory", "pcfactory");
                // Copy the contents of the file to the request stream.  
                //StreamReader sourceStream = new StreamReader(@srcFilePath);// + "\\" + srcFileName);//  "E:\yourlocation\SampleFile.txt");
                //sourceStream.
                //byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                //sourceStream.Close();
                /* file operation closed*/

                FileInfo fInfo = new FileInfo(srcFilePath);
                int remainbyte = (int)fInfo.Length;
                BinaryReader bread = new BinaryReader(File.Open( srcFilePath,FileMode.Open));
                
                /* progress bar update*/
                info.pbMax = (remainbyte / ftpFrameSize) + 1; // atleast 1 frame
               
                //reset progress bar and other information @ startup 
                info.actOn = actionOnControl.PROGRESS_BAR;
                updateProgressBar_Callback(info);

                /* ftp Operation start*/
                Stream requestStream = request.GetRequestStream();
                while (remainbyte > 0)
                {
                    bytesToSend = ((remainbyte > ftpFrameSize) ? ftpFrameSize : remainbyte);
                    dataToSend = bread.ReadBytes(bytesToSend);
                    //Buffer.BlockCopy(fileContents, (fileContents.Length - remainbyte), dataToSend, 0, bytesToSend);
                    request.ContentLength = bytesToSend;
                    requestStream.Write(dataToSend, 0, bytesToSend);

                    info.pbVal += 1;
                    /* update progress bar */
                    info.actOn = actionOnControl.PROGRESS_BAR;
                    updateProgressBar_Callback(info);
                    remainbyte = remainbyte - bytesToSend;
                    Thread.Sleep(10);
                }
                requestStream.Close();
                bread.Close();
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);

                response.Close();
                retVal = 0;
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message.ToString());
                String status = ((FtpWebResponse)e.Response).StatusDescription;
                Console.WriteLine(status);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            
            return retVal;
        }


        private void btConnect_Click(object sender, RoutedEventArgs e)
        {
            if(cbInterface.Text == "" || cbIPAddress.Text=="")
            {
                MessageBox.Show(" please select the PC interface");
                return;
            }
            // PostDatatoDevice();

            //string serverIP = "192.168.2.114";

            //clientipStr = "192.168.2.1";
            //if (mbtcp.modbusInit(clientipStr, 502) == 0)
            //{
            //    if (mbtcp.mbOpen(serverIP, 502) == 0)
            //    {
            //         readLTMRConfiguration();
            //        //MessageBox.Show("TcpConnection Opend");
            //    }

            //}
            ModbusAccess win2 = new ModbusAccess();
            // Window win2 = new Window();
            win2.setReferenceOfMainWindow(this);
            win2.Show();
           // this.Close();

        }

        private void btDisconnet_Click(object sender, RoutedEventArgs e)
        {
            mbtcp.mbClose();
            MessageBox.Show("TCP connection Close");

        }

        private void btInit_Click(object sender, RoutedEventArgs e)
        {
            UInt16[] mbReadBuf = new UInt16[10];
            string comRef;

            string serverIP = "192.168.2.114";

            clientipStr = "192.168.2.1";
            if (mbtcp.modbusInit(clientipStr, 502) == 0)
            {
                if (mbtcp.mbOpen(serverIP, 502) == 0)
                {
                    if (mbtcp.mbRead(64, 5, mbReadBuf) > 0)
                    {


                        char tmp = (char)(mbReadBuf[0] >> 8);// MSB
                        comRef = tmp.ToString();
                        tmp = (char)(mbReadBuf[0] & 0x00FF);  //LSB
                        comRef += tmp.ToString();

                        
                        tmp = (char)(mbReadBuf[1] >> 8);// MSB
                        comRef += tmp.ToString();
                        tmp = (char)((mbReadBuf[1]) & 0x00FF);// LSB
                        comRef += tmp.ToString();

                        
                        tmp = (char)(mbReadBuf[2] >> 8);// MSB
                        comRef += tmp.ToString();
                        tmp = (char)((mbReadBuf[2]) & 0x00FF);// LSB
                        comRef += tmp.ToString();

                        
                        tmp = (char)(mbReadBuf[3] >> 8);// MSB
                        comRef += tmp.ToString();
                        tmp = (char)((mbReadBuf[3]) & 0x00FF);// LSB
                        comRef += tmp.ToString();

                        
                        tmp = (char)(mbReadBuf[4] >> 8);// MSB
                        comRef += tmp.ToString();
                        tmp = (char)((mbReadBuf[4]) & 0x00FF);// LSB
                        comRef += tmp.ToString();

                        MessageBox.Show(comRef);
                    }
                }
            }
            mbtcp.mbClose();
        }


        private void grdDeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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

        private void btAddEntry_Click(object sender, RoutedEventArgs e)
        {
            //int cnt= grdDeviceList.Items.Count;


            // for (int i = 0; i < cnt; i++)
            // {
            //     deviceEntry ent = new deviceEntry();
            //     ent = (deviceEntry)grdDeviceList.Items[i];
            //     Console.WriteLine(ent.deviceIP + "of Index "+i.ToString());
            //     Thread.Sleep(2000);
            // }

            while (true)
            {
                int index = grdDeviceList.SelectedIndex;
                Console.WriteLine(index.ToString());
                if (index == -1)
                    break;
                try
                {
                    deviceEntry ent = new deviceEntry();
                    grdUpdateList.Items.Add(grdDeviceList.Items[index]);
                    ent = (deviceEntry)grdDeviceList.Items[index];
                    Console.WriteLine(ent.deviceIP);
                    //MessageBox.Show(grdDeviceList.SelectedItem.ToString());                    
                    grdDeviceList.Items.RemoveAt(index);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void btDeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                int index = grdUpdateList.SelectedIndex;
                if (index == -1)
                    break;
                try
                {
                    grdDeviceList.Items.Add(grdUpdateList.Items[index]);
                    grdUpdateList.Items.RemoveAt(index);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void btGetFile_Click(object sender, RoutedEventArgs e)
        {
            getFileFromDevice();
        }

        /*********************************END******************************************************************************/
    }// main window
}//class 

