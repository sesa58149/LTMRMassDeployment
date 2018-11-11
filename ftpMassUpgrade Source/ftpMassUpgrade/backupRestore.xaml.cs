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
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;

namespace ftpMassUpgrade
{
    /// <summary>
    /// Interaction logic for backupRestore.xaml
    /// </summary>
    public partial class backupRestore : Window
    {
        public backupRestore bnrWin { get; set; }
        public TeSysTBackupNRestore ltmrBnR { get; set; }
        public backupRestore()
        {
            InitializeComponent();
            bnrWin = this;
            rdBackup.IsChecked = true;
            ltmrBnR = new TeSysTBackupNRestore(callback_ProcessProgress);

            // update gride
            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Header = "IP Address";
            col5.Binding = new Binding("deviceIP");
            col5.IsReadOnly = true;
            grdBnRDeviceList.Columns.Add(col5);
            
            DataGridTextColumn col6 = new DataGridTextColumn();
            col6.Header = "Com Reference";
            col6.Binding = new Binding("comRef");
            col6.IsReadOnly = true;
            col6.Visibility = 0;
            grdBnRDeviceList.Columns.Add(col6);

            DataGridTextColumn col7 = new DataGridTextColumn();
            col7.Header = "Controller FW Version";
            col7.Binding = new Binding("kuVer");
            col7.IsReadOnly = true;
            grdBnRDeviceList.Columns.Add(col7);

            DataGridTextColumn col8 = new DataGridTextColumn();
            col8.Header = "Netwrok FW Version";
            col8.Binding = new Binding("kcVer");
            col8.IsReadOnly = true;
            grdBnRDeviceList.Columns.Add(col8);
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void rdBackup_Checked(object sender, RoutedEventArgs e)
        {
            txtRef8Amp.Visibility = Visibility.Hidden;
            txtRef27Amp.Visibility = Visibility.Hidden;
            txtRef100Amp.Visibility = Visibility.Hidden;
            btRef8Amp.Visibility = Visibility.Hidden;
            btRef27Amp.Visibility = Visibility.Hidden;
            btRef100Amp.Visibility = Visibility.Hidden;
            chComRef.Visibility = Visibility.Hidden;
            chKuVer.Visibility = Visibility.Hidden;
            chKCVer.Visibility = Visibility.Hidden;
            chSerialN.Visibility = Visibility.Hidden;
            grpComp.Visibility = Visibility.Hidden;
            grpFileSel.Visibility = Visibility.Hidden;
        }

        private void rdRestore_Checked(object sender, RoutedEventArgs e)
        {
            txtRef8Amp.Visibility = Visibility.Visible;
            txtRef27Amp.Visibility = Visibility.Visible;
            txtRef100Amp.Visibility = Visibility.Visible;
            btRef8Amp.Visibility = Visibility.Visible;
            btRef27Amp.Visibility = Visibility.Visible;
            btRef100Amp.Visibility = Visibility.Visible;
            chComRef.Visibility = Visibility.Visible;
            chKuVer.Visibility = Visibility.Visible;
            chKCVer.Visibility = Visibility.Visible;
            chSerialN.Visibility = Visibility.Visible;
            grpFileSel.Visibility = Visibility.Visible;
            grpComp.Visibility = Visibility.Visible;
        }
        public void callback_ProcessProgress(backupNRestoreProgressInfo pInfo)
        {
            Action cmd;
            try
            {
                if (pInfo.actOn == actionOnControl.NO_ACTION)
                {
                    return;
                }

                
                if (pInfo.actOn == actionOnControl.PROGRESS_BAR)
                {
                    if (pInfo.pbVal == 0)
                    {
                        
                        cmd = () => bnrWin.pbBnR.Maximum = pInfo.pbMax;
                        Application.Current.Dispatcher.Invoke(cmd);

                        cmd = () => bnrWin.pbBnR.Minimum = pInfo.pbMin;
                        Application.Current.Dispatcher.Invoke(cmd);

                        // IP address lebel clean up
                        cmd = () => bnrWin.lblDeviceId.Content = " ";
                        Application.Current.Dispatcher.Invoke(cmd);

                    }                 

                    cmd = () => bnrWin.pbBnR.Value = pInfo.pbVal;
                    Application.Current.Dispatcher.Invoke(cmd);
                }
                else if(pInfo.actOn==actionOnControl.LEBEL)
                {
                    cmd = () => bnrWin.lblDeviceId.Content = pInfo.dEntry.deviceIP;
                    Application.Current.Dispatcher.Invoke(cmd);
                }
                else if(pInfo.actOn==actionOnControl.PROCESS_COMPLETED)
                {
                    cmd = () => bnrWin.pbBnR.Value = 0;
                    Application.Current.Dispatcher.Invoke(cmd);
                    // IP address lebel clean up
                    cmd = () => bnrWin.lblDeviceId.Content = " ";
                    Application.Current.Dispatcher.Invoke(cmd);
                                        
                    cmd = () => bnrWin.lblDeviceId.Content = "Backup or Restore Process Completed";
                    Application.Current.Dispatcher.Invoke(cmd);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            int selectedLst = grdBnRDeviceList.SelectedItems.Count; 
            deviceEntry[] dEntry= new deviceEntry[selectedLst];
            bool isFileSelected = false;
            TeSysTConfFileCompatibilityParam compatibilityParam = new TeSysTConfFileCompatibilityParam();

            try
            {
                if (selectedLst < 1)
                {
                    MessageBox.Show("Please select at leat one device to be Backed-up or Restored");
                    return;
                }
                

                for (int i = 0; i < selectedLst; i++)
                {
                    dEntry[i]= (deviceEntry)(grdBnRDeviceList.SelectedItems[i]);
                }
            }
            catch(excepetion ex)
            {
                Console.WriteLine(ex.Message);
            }
            if(rdBackup.IsChecked == true)
            {
                ltmrBnR.backupDeviceConfiguration(dEntry, selectedLst);
            }
            else if(rdRestore.IsChecked==true)
            {
                TeSysTConfiFile confFiles = new TeSysTConfiFile();
                if (txtRef100Amp.Text != "")
                {
                    confFiles.fileName100Amp = txtRef100Amp.Text.ToString();
                    isFileSelected = true;
                }
                else
                {
                    confFiles.fileName100Amp = "";
                }

                if (txtRef27Amp.Text != "")
                {
                    confFiles.fileName27Amp = txtRef27Amp.Text.ToString();
                    isFileSelected = true;
                }
                else
                {
                    confFiles.fileName27Amp = "";
                }
                if (txtRef8Amp.Text != "")
                {
                    confFiles.fileName8Amp = txtRef8Amp.Text.ToString();
                    isFileSelected = true;
                }
                else
                {
                    confFiles.fileName8Amp = "";
                }


                if (isFileSelected == true)
                {
                    compatibilityParam.comercialReference = (bool)chComRef.IsChecked;
                    compatibilityParam.kuFirmwareVer = (bool)chKuVer.IsChecked;
                    compatibilityParam.kcFirmwareVer = (bool)chKCVer.IsChecked;
                    compatibilityParam.deviceSerialNumber = (bool)chSerialN.IsChecked;
                    confFiles.cParam = compatibilityParam;

                    ltmrBnR.restoreDeviceConfiguration(dEntry, selectedLst, confFiles);
                }
                else
                {
                    MessageBox.Show("please select at least one configuration file");
                }
            }
        }

        private void btRef8Amp_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            try
            {
                if (dlg.ShowDialog() == true)
                {
                    string filePath = dlg.FileName;

                    string srcFilePath = dlg.FileName;

                    string fileName = System.IO.Path.GetFileName(filePath);
                    txtRef8Amp.Text = fileName;                   
                    int retCode = ltmrBnR.loadDeviceConfigurationFile(srcFilePath, LTRM_ComercialRef.LTMR_08);
                    if (retCode== -2 )
                    {
                        MessageBox.Show("please select 08 Amp reference configuration file");

                    }
                    else if (retCode == -1)
                    {
                        MessageBox.Show("File doesn't exist");
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btRef27Amp_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            try
            {
                if (dlg.ShowDialog() == true)
                {
                    string filePath = dlg.FileName;

                    string srcFilePath = dlg.FileName;

                    string fileName = System.IO.Path.GetFileName(filePath);
                    txtRef27Amp.Text = fileName;
                    string srcFileName = fileName;
                    int retCode = ltmrBnR.loadDeviceConfigurationFile(srcFileName, LTRM_ComercialRef.LTMR_27);
                    if (retCode == -2)
                    {
                        MessageBox.Show("please select 27 Amp reference configuration file");

                    }
                    else if (retCode == -1)
                    {
                        MessageBox.Show("File doesn't exist");
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btRef100Amp_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            try
            {
                if (dlg.ShowDialog() == true)
                {
                    string filePath = dlg.FileName;

                    string srcFilePath = dlg.FileName;

                    string fileName = System.IO.Path.GetFileName(filePath);
                    txtRef100Amp.Text = fileName;
                    string srcFileName = fileName;
                    int retCode = ltmrBnR.loadDeviceConfigurationFile(srcFileName, LTRM_ComercialRef.LTMR_100);
                    if (retCode == -2)
                    {
                        MessageBox.Show("please select 100 Amp reference configuration file");

                    }
                    else if (retCode == -1)
                    {
                        MessageBox.Show("File doesn't exist");
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

  
    }
}
