using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ftpMassUpgrade
{
    /// <summary>
    /// Interaction logic for Upgrade.xaml
    /// </summary>
    public partial class Upgrade : Window
    {
        ProgressBar[] pb = new ProgressBar[8];
        Label[] lb = new Label[8];
        public string srcFilePath;
        public string srcFileName;
        Upgrade updateWin { get; set; }
        upgradeTask fUpgradeTask { get; set; }
        deviceEntry[] upgradedDeviceList;
        int upgradedDeviceListCnt;
        public Upgrade()
        {
            InitializeComponent();

            updateWin = this;
            updateControlInit();
        }
        public Upgrade(ref deviceEntry [] deveList, int lstCnt)
        {
            InitializeComponent();

            updateWin = this;
            updateControlInit();
            for(int i=0;i<lstCnt;i++)
            {
                grdUpdateList.Items.Add(deveList[i]);
            }
        }

        private void updateControlInit()
        {

            fUpgradeTask = new upgradeTask(updateProgressBar_Callback);
            upgradedDeviceListCnt = 0;
            /* initialize progress bar for update process*/

            pb[0] = pbUpdate_0;
            lb[0] = lbIPAddress_0;
            pb[1] = pbUpdate_1;
            lb[1] = lbIPAddress_1;
            pb[2] = pbUpdate_2;
            lb[2] = lbIPAddress_2;
            pb[3] = pbUpdate_3;
            lb[3] = lbIPAddress_3;
            pb[4] = pbUpdate_4;
            lb[4] = lbIPAddress_4;
            pb[5] = pbUpdate_5;
            lb[5] = lbIPAddress_5;
            pb[6] = pbUpdate_6;
            lb[6] = lbIPAddress_6;
            pb[7] = pbUpdate_7;
            lb[7] = lbIPAddress_7;

            /* init device list data grid*/

            // update gride
            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Header = "IP Address";
            col5.Binding = new Binding("deviceIP");
            col5.IsReadOnly = true;
            grdUpdateList.Columns.Add(col5);

            DataGridTextColumn col7 = new DataGridTextColumn();
            col7.Header = "KU FW Version";
            col7.Binding = new Binding("kuVer");
            col7.IsReadOnly = true;
            grdUpdateList.Columns.Add(col7);

            DataGridTextColumn col8 = new DataGridTextColumn();
            col8.Header = "KC FW Version";
            col8.Binding = new Binding("kcVer");
            col8.IsReadOnly = true;
            grdUpdateList.Columns.Add(col8);

            DataGridTextColumn col9 = new DataGridTextColumn();
            col9.Header = "Upgrade Status";
            col9.Binding = new Binding("upgradeStatus");
            col9.IsReadOnly = true;
            grdUpdateList.Columns.Add(col9);
        }
        public void updateProgressBar_Callback(updateInfo pbInfo)
        {
            Action cmd;
            try
            {
                if (pbInfo.actOn == actionOnControl.NO_ACTION)
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
                        cmd = () => updateWin.pb[pbInfo.winControlIndex].Maximum = pbInfo.pbMax;
                        Application.Current.Dispatcher.Invoke(cmd);

                        cmd = () => updateWin.pb[pbInfo.winControlIndex].Minimum = pbInfo.pbMin;
                        Application.Current.Dispatcher.Invoke(cmd);

                        // IP address lebel clean up
                        cmd = () => updateWin.lb[pbInfo.winControlIndex].Content = " ";
                        Application.Current.Dispatcher.Invoke(cmd);

                    }
                    else
                    {
                        cmd = () => updateWin.lb[pbInfo.winControlIndex].Content = pbInfo.serverIpAddress;
                        Application.Current.Dispatcher.Invoke(cmd);
                    }

                    cmd = () => updateWin.pb[pbInfo.winControlIndex].Value = pbInfo.pbVal;
                    Application.Current.Dispatcher.Invoke(cmd);
                }
                else if (pbInfo.actOn == actionOnControl.DATA_GRID)
                {
                    cmd = () => updateWin.grdUpdateList.Items.Add(pbInfo.dEntry);
                    Application.Current.Dispatcher.Invoke(cmd);
                }
                else if (pbInfo.actOn==actionOnControl.PROCESS_COMPLETED)
                {
                    int lstCnt = 0;

                    cmd = () => lstCnt = grdUpdateList.Items.Count;
                    Application.Current.Dispatcher.Invoke(cmd);
                    /* clean old entry*/
                     
                    for(int i=0; i<lstCnt;i++)
                    {
                        cmd = () => grdUpdateList.Items.RemoveAt(0);
                        Application.Current.Dispatcher.Invoke(cmd);
                        
                    }
                    /* add new entry*/
                    int j = 0;
                    for (int i = 0; i < upgradedDeviceListCnt; i++)
                    {
                        cmd = () => grdUpdateList.Items.Add(upgradedDeviceList[i]);
                        Application.Current.Dispatcher.Invoke(cmd);
                    }
                    // enbale start button
                    cmd = () => btStart.IsEnabled = true;
                    Application.Current.Dispatcher.Invoke(cmd);
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
                    // enable upgrade button
                    btStart.IsEnabled = true;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {

            if(txtBrowse.Text=="")
            {
                MessageBox.Show("please select file to be deployed");
            }
            upgradeFileInfo uFileinfo = new upgradeFileInfo();
            uFileinfo.srcFileName = srcFileName;
            uFileinfo.srcFilePath = srcFilePath;

            try
            {
                btStart.IsEnabled = false;

                int noOfDevice = grdUpdateList.Items.Count;
                if(noOfDevice <=0)
                {
                    return;
                }
                upgradedDeviceList = new deviceEntry[noOfDevice];
                for( int i=0; i<noOfDevice;i++)
                {
                    upgradedDeviceList[i] = (deviceEntry)grdUpdateList.Items[i];
                }
                upgradedDeviceListCnt = noOfDevice;
                fUpgradeTask.startUpgradeTask(upgradedDeviceList, noOfDevice, uFileinfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
