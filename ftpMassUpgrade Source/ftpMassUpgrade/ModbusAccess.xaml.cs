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
using System.Data;
using System.ComponentModel;
using Microsoft.Win32;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace ftpMassUpgrade
{
    public struct modbusReadReq
    {
        public ushort regAdd;
        public ushort nReg;
    }

    class modbusResponseGrid
    {
        public string regAdd { get; set; }
        public string val { get; set; }
        public string valHex { get; set; }
        
        public modbusResponseGrid()
        {
            regAdd = new string (new char[32]);
            val = new string(new char[32]);
            valHex = new string(new char[32]);

        }
    }
    
    /// <summary>
    /// Interaction logic for ModbusAccess.xaml
    /// </summary>
    public partial class ModbusAccess : Window
    {
        const int listSize = 64;
        mbTCP mbClient = new mbTCP();
        toolUitility utility = new toolUitility();
        static int rgdIndex = 0;
        ModbusRegisterAccess mrAccess { get; set; }
        public object ExcelReaderFactory { get; private set; }
        public modbusReadReq [] readReqArray = new modbusReadReq[32];
        public int nRequestInArray = 0;
        string srcFileName = " ";
        string srcFilePath = " ";
        MainWindow mWin;
        public deviceEntry[] deviceList = new deviceEntry[listSize];
        public int nDevice = 0;
        public ModbusAccess()
        {
            InitializeComponent();

            DataGridTextColumn col0 = new DataGridTextColumn();
            col0.Header = "Register Address";
            col0.Binding = new Binding("regAdd");
            col0.IsReadOnly = true;
            grdModbus.Columns.Add(col0);

            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Register Value";
            col1.Binding = new Binding("val");
            col1.IsReadOnly = false;
            grdModbus.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Register Hex";
            col2.Binding = new Binding("valHex");
            col2.IsReadOnly = false;
            grdModbus.Columns.Add(col2);

            mrAccess = new ModbusRegisterAccess();
            
            txtFileName.Visibility = Visibility.Hidden;
            btBrowse.Visibility = Visibility.Hidden;

            txtNReg.Visibility = Visibility.Visible;
            txtStartAddress.Visibility = Visibility.Visible;


            mWin = new MainWindow();

        }
        public int getDeviceListSize()
        {
            return listSize;
        }
        private void btSaveAll_Click(object sender, RoutedEventArgs e)
        {
            modbusResponseGrid MrGrid = new modbusResponseGrid();
            btSaveAll.IsEnabled = false;

            int lstCnt = grdModbus.Items.Count;
            StreamWriter confFile = new StreamWriter("MBResponse.csv");
            // write Header
            confFile.Write("Register Address" + ";" + " Value in Decimal" + ";" + "Value in Hex" + " \n");

            for ( int i=0; i< lstCnt; i++)
            {
                MrGrid = (modbusResponseGrid)grdModbus.Items.GetItemAt(i);
                
                try
                {                       
                    confFile.Write(MrGrid.regAdd + ";" + MrGrid.val + ";" + MrGrid.valHex +" \n");                                    
                }
                catch (excepetion ex)
                {
                    Console.WriteLine(ex.Message);
                }                               
            }
            confFile.Close();
            btSaveAll.IsEnabled = true;
            MessageBox.Show(" data grid saved in .csv file in parenet folder of exe file ");
        }
    public void setReferenceOfMainWindow(MainWindow mW)
        {
            mWin = mW;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {            
            this.Close();
        }
        private void handleModbusReadRequest(ushort startAdd, ushort nReg, ushort []rxBuf)
        {
            int rxReg = -1;
            try
            {
                
                rxReg = mrAccess.readModbusRegister(startAdd, nReg, rxBuf);
                if (rxReg == -1)
                {
                    switch (rxBuf[0])
                    {
                        case 1:
                            lblMBException.Content = "ILLEGAL FUNCTION";
                            break;
                        case 2:
                            lblMBException.Content = "ILLEGAL DATA ADDRESS";
                            break;
                        case 3:
                            lblMBException.Content = "ILLEGAL DATA VALUE";
                            break;
                        case 4:
                            lblMBException.Content = "SLAVE DEVICE FAILURE";
                            break;
                        case 6:
                            lblMBException.Content = "SLAVE DEVICE BUSY";
                            break;
                        default:
                            lblMBException.Content = rxBuf[0].ToString();
                            break;
                    }
                }
                else
                {
                    lblMBException.Content = "Success !";
                    modbusResponseGrid[] msGrid = new modbusResponseGrid[nReg];
                    for (int i = 0; i < nReg; i++)
                    {
                        msGrid[i] = new modbusResponseGrid();
                        msGrid[i].regAdd = (startAdd + i).ToString();
                        msGrid[i].val = rxBuf[i].ToString();
                        msGrid[i].valHex = utility.U16ToHex(rxBuf[i]);
                        grdModbus.Items.Add(msGrid[i]);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void singleDeviceModbusRequest()
        {
            ushort[] rxBuf = new ushort[250];
            if (utility.validateIPAddress(txtSlaveIP.Text) == false)
                return;
            if (txtNReg.Text == " ")
            {
                Console.WriteLine("Number of register must be in range of 1 to 120");
                return;
            }
            if (txtStartAddress.Text == " ")
            {
                Console.WriteLine("Number of register must be in range of 1 to 120");
                return;
            }
            modbusSlaveInfo slaveInfo = new modbusSlaveInfo();
            slaveInfo.slaveIPAdd = txtSlaveIP.Text.ToString();
            slaveInfo.tcpPortId = 502;
            slaveInfo.slaveUid = 1;
            mrAccess.setSlaveInfo(slaveInfo);
            try
            {
                btSendModbus.IsEnabled = false;

                if (rdFromFile.IsChecked == false)
                {
                    ushort startAdd = Convert.ToUInt16(txtStartAddress.Text);
                    ushort nReg = Convert.ToUInt16(txtNReg.Text);
                    handleModbusReadRequest(startAdd, nReg, rxBuf);
                }
                else
                {

                    if (nRequestInArray >= 0)
                    {
                        for (int i = 0; i < nRequestInArray - 1; i++)
                        {
                            handleModbusReadRequest(readReqArray[i].regAdd, readReqArray[i].nReg, rxBuf);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Fail to read Modbus request from Excel file");
                    }
                }
                btSendModbus.IsEnabled = true;

            }
            catch (excepetion ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void multipleDeviceModbusRequest()
        {
            ushort[] rxBuf = new ushort[250];

            if(nDevice <= 0)
            {
                Console.WriteLine("no device presented in list");
                return;
            }
            btSendModbus.IsEnabled = false;

            for (int dCnt = 0; dCnt < nDevice; dCnt++)
            {

                modbusSlaveInfo slaveInfo = new modbusSlaveInfo();
                slaveInfo.slaveIPAdd = deviceList[dCnt].deviceIP;
                slaveInfo.tcpPortId = 502;
                slaveInfo.slaveUid = 1;
                mrAccess.setSlaveInfo(slaveInfo);
                try
                {
                    

                    if (rdFromFile.IsChecked == false)
                    {
                        ushort startAdd = Convert.ToUInt16(txtStartAddress.Text);
                        ushort nReg = Convert.ToUInt16(txtNReg.Text);
                        handleModbusReadRequest(startAdd, nReg, rxBuf);
                    }
                    else
                    {

                        if (nRequestInArray >= 0)
                        {
                            for (int i = 0; i < nRequestInArray - 1; i++)
                            {
                                handleModbusReadRequest(readReqArray[i].regAdd, readReqArray[i].nReg, rxBuf);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Fail to read Modbus request from Excel file");
                        }
                    }            

                }
                catch (excepetion ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            btSendModbus.IsEnabled = true;
        }
        private void btSendModbus_Click(object sender, RoutedEventArgs e)
        {
            if(rdAll.IsChecked==true)
            {
                multipleDeviceModbusRequest();
            }
            else
            {
                singleDeviceModbusRequest();
            }
        }
       
        private int readMBReqFromExcel()
        {
            int retCode = -1;
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;

            int rCnt;
            //int cCnt;
            int rw = 0;
            int cl = 0;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(srcFilePath, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            range = xlWorkSheet.UsedRange;
            rw = range.Rows.Count;
            if (rw > 32)
            {
                rw = 32;
            }
            cl = range.Columns.Count;
            if(cl>2)
            {
                Console.WriteLine("File corrupted -- No of column MUST be 2 ");
                return retCode;
            }


            for (rCnt = 1; rCnt <= rw; rCnt++)
            {

                readReqArray[rCnt-1].regAdd = (ushort)(range.Cells[rCnt, 1] as Excel.Range).Value;
                readReqArray[rCnt-1].nReg = (ushort)(range.Cells[rCnt, 2] as Excel.Range).Value;               
            }
            retCode = rCnt;
            xlWorkBook.Close(true, null, null);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);
            return retCode;
        }

        private void btDGClear_Click(object sender, RoutedEventArgs e)
        {
            int nItem = grdModbus.Items.Count;

            for (int i = 0; i < nItem; i++)
            {
                grdModbus.Items.RemoveAt(0);
            }
            rgdIndex = 0;
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            modbusResponseGrid msGrid = new modbusResponseGrid();
            msGrid.regAdd = 500.ToString();
            msGrid.val = 100.ToString();
            msGrid.valHex = utility.U16ToHex(100);
            grdModbus.Items.Add(msGrid);         

        }

        private void btCreateNewMBWin_Click(object sender, RoutedEventArgs e)
        {
           // Window mb = new Window();
            
        }

        private DataRowView rowBeingEdited = null;
        private void dgModbusCellEditEnd(object sender, DataGridCellEditEndingEventArgs e)
        {
            //if (e.EditAction == DataGridEditAction.Commit)
            //{
            //    var column = e.Column as DataGridBoundColumn;
            //    if (column != null)
            //    {
            //        var bindingPath = (column.Binding as Binding).Path.Path;
            //        if (bindingPath == "val")
            //        {
            //            int rowIndex = e.Row.GetIndex();
            //            var el = e.EditingElement as TextBox;
            //            // rowIndex has the row index
            //            // bindingPath has the column's binding
            //            // el.Text has the new, user-entered value
            //            MessageBox.Show(el.ToString());
            //        }
            //    }
            //}
            DataRowView rowView = e.Row.Item as DataRowView;
            rowBeingEdited = rowView;

        }

        

        private void dgModbusEventHandler(object sender, DataTransferEventArgs e)
        {

        }

        private void dgModbusCellChanged(object sender, EventArgs e)
        {
            if (rowBeingEdited != null)
            {
                rowBeingEdited.EndEdit();
            }
        }

        private void chReqInput_Checked(object sender, RoutedEventArgs e)
        {
            txtFileName.Visibility = Visibility.Visible;
            btBrowse.Visibility = Visibility.Visible;
            

            txtStartAddress.Visibility = Visibility.Hidden;
            txtNReg.Visibility = Visibility.Hidden;
        }

        private void chReqInput_unChecked(object sender, RoutedEventArgs e)
        {

            txtFileName.Visibility = Visibility.Hidden;
            btBrowse.Visibility = Visibility.Hidden;

            txtNReg.Visibility = Visibility.Visible;
            txtStartAddress.Visibility = Visibility.Visible;
            
        }

        private void btBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.FileName = "MbQuery";
            dlg.DefaultExt = ".xls";
            dlg.Filter = "Excel doc (.xls)|*.xlsx";
            try
            {
                btBrowse.IsEnabled = false;
                if (dlg.ShowDialog() == true)
                {
                    string filePath = dlg.FileName;

                    srcFilePath = dlg.FileName;

                    string fileName = System.IO.Path.GetFileName(filePath);
                    txtFileName.Text = fileName;
                    srcFileName = fileName;

                }
                nRequestInArray = readMBReqFromExcel();
                btBrowse.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void handleDGChnage_DBClick(object sender, MouseButtonEventArgs e)
        {
            int index = grdModbus.SelectedIndex;
            if(index>=0)
            {
                modbusResponseGrid entry = (modbusResponseGrid)grdModbus.Items[index];
                mbWriteWin win2 = new mbWriteWin();
                win2.txtmbAdd.Text = entry.regAdd.ToString();
                win2.txtVal.Text = entry.val.ToString();
                win2.txtValHex.Text = entry.valHex.ToString();
                win2.slaveInfo.slaveIPAdd = txtSlaveIP.Text.ToString();
                win2.slaveInfo.tcpPortId = 502;
                win2.slaveInfo.slaveUid = 1;                
                win2.Show();
            }
        }

        private void rdAll_Selected(object sender, RoutedEventArgs e)
        {
            lblSlaveIP.Visibility = Visibility.Hidden;
            txtSlaveIP.Visibility = Visibility.Hidden;

        }
        private void rdInd_Selected(object sender, RoutedEventArgs e)
        {
            lblSlaveIP.Visibility = Visibility.Visible;
            txtSlaveIP.Visibility = Visibility.Visible;
        }

        private void rdFromFile_Selected(object sender, RoutedEventArgs e)
        {
            txtFileName.Visibility = Visibility.Visible;
            btBrowse.Visibility = Visibility.Visible;

            txtStartAddress.Visibility = Visibility.Hidden;
            lblStartAddress.Visibility = Visibility.Hidden;
            txtNReg.Visibility = Visibility.Hidden;
            lblNReg.Visibility = Visibility.Hidden;

        }

        private void rdMbAdd_Selected(object sender, RoutedEventArgs e)
        {
            txtFileName.Visibility = Visibility.Hidden;
            btBrowse.Visibility = Visibility.Hidden;

            txtStartAddress.Visibility = Visibility.Visible;
            lblStartAddress.Visibility = Visibility.Visible;
            txtNReg.Visibility = Visibility.Visible;
            lblNReg.Visibility = Visibility.Visible;
        }

       
    }// class end

    [Serializable]
    internal class excepetion : Exception
    {
        public excepetion()
        {
        }

        public excepetion(string message) : base(message)
        {
        }

        public excepetion(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected excepetion(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    internal interface IExcelDataReader
    {
    }
}// name space end
