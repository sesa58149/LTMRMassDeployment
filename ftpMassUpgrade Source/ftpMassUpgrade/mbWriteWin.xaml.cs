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

namespace ftpMassUpgrade
{
    /// <summary>
    /// Interaction logic for mbWriteWin.xaml
    /// </summary>
    public partial class mbWriteWin : Window
    {
        toolUitility utility;
        ModbusRegisterAccess mbAccess;
        public modbusSlaveInfo slaveInfo;
        
        public mbWriteWin()
        {
            InitializeComponent();
            utility = new toolUitility();
            mbAccess = new ModbusRegisterAccess();
            slaveInfo = new modbusSlaveInfo();
           
        }

        private void btModbusAccessWin_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtEnterValDone_KBLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (utility.validateIntegerTextBox(txtVal.Text.ToString()) == false)
            {
                MessageBox.Show("please enter only 0-9 integer value");
                txtVal.Text = "0";
            }
            else
            {
                if (txtVal.Text.ToString() != "")
                {
                    ushort val = Convert.ToUInt16(txtVal.Text);
                    txtValHex.Text = utility.U16ToHex(val).ToString();
                }
            }
        }

        private void txtHexEnterValDone_KBLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            string strVal = txtValHex.Text.ToString();
            if(strVal.Length==0)
            {
                strVal = "0";
                txtValHex.Text = strVal;
            }
            else if(strVal.Length > 4)
            {
                MessageBox.Show("value can't be more than 16 bit");
                strVal = "0";
                txtValHex.Text = strVal;
            }
            
            ushort val = utility.HexToU16(strVal);
            txtVal.Text = val.ToString();
        }

        private void btSendCmd_Click(object sender, RoutedEventArgs e)
        {
            ushort[] txBuf = new ushort[120];            
            ushort startAdd = Convert.ToUInt16(txtmbAdd.Text);
            txBuf[0] = Convert.ToUInt16(txtVal.Text);
            mbAccess.setSlaveInfo(slaveInfo);
            if (mbAccess.writeModbusRegister(startAdd, 1, txBuf) == 0)
            {
                lblSuccessCode.Content = "Success!";
            }
        }

        private void txtEnterChar_KBCharChange(object sender, KeyEventArgs e)
        {
            
            //if (utility.validateIntegerTextBox(txtVal.Text.ToString())== false)
            //{
            //    MessageBox.Show("please enter only 0-9 integer value");
            //    txtVal.Text = "";
            //}
        }
    }
}
