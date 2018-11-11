using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
namespace ftpMassUpgrade
{
   public class toolUitility
    {
        public string LongToIP(long longIP)
        {
            string ip = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                int num = (int)(longIP / Math.Pow(256, (3 - i)));
                longIP = longIP - (long)(num * Math.Pow(256, (3 - i)));
                if (i == 0)
                    ip = num.ToString();
                else
                    ip = ip + "." + num.ToString();
            }//for
            return ip;
        }//longToIP

        public long IP2Long(string ip)
        {
            string[] ipBytes;
            double num = 0;
            UInt32 ipAdd;
            if (!string.IsNullOrEmpty(ip))
            {
                ipBytes = ip.Split('.');
                for (int i = ipBytes.Length - 1; i >= 0; i--)
                {
                    num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
                }//for
            }//if
          
            byte[] tmp = new byte[10];
            copyUint32Endian(tmp, (UInt32)num, 0);
            //ipAdd=Convert.ToUInt32(tmp);
            ipAdd=BitConverter.ToUInt32(tmp, 0);
            return ipAdd;
        }// IP2Long

        public long IP2LongEndianLess(string ip)
        {
            string[] ipBytes;
            double num = 0;
            long ipAdd;
            if (!string.IsNullOrEmpty(ip))
            {
                ipBytes = ip.Split('.');
                for (int i = ipBytes.Length - 1; i >= 0; i--)
                {
                    num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
                }//for
            }//if

            // byte[] tmp = new byte[10];
            //copyUint32Endian(tmp, (UInt32)num, 0);
            //ipAdd=Convert.ToUInt32(tmp);
            //ipAdd = BitConverter.ToUInt32(tmp, 0);
            ipAdd = (long)num;
            return ipAdd;
        }// IP2Long

        public UInt16 copyUint16Endian(byte[]dst, UInt16 val, UInt16 startIndex)
        {
            byte[] tmp  = BitConverter.GetBytes(val);
            
            dst[startIndex] = tmp[1];
            startIndex++;
            dst[startIndex] = tmp[0];
            startIndex++;            
            return startIndex; 
        }
        public UInt16 copyUint32Endian(byte[] dst, UInt32 val, UInt16 startIndex)
        {            
            byte[] tmp = BitConverter.GetBytes(val);

            dst[startIndex] = tmp[3];
            startIndex++;
            dst[startIndex] = tmp[2];
            startIndex++;
            dst[startIndex] = tmp[1];
            startIndex++;
            dst[startIndex] = tmp[0];
            startIndex++;
            return startIndex;
        }
        public bool validateIPAddress(String ipAddress)
        {
            bool retVal = true;
            UInt16 all0cnt = 0, all255cnt = 0;

            try
            {
                //  Split string by ".", check that array length is 3
                char chrFullStop = '.';
                string[] arrOctets = ipAddress.Split(chrFullStop);
                if (arrOctets.Length != 4)
                {
                    retVal = false;
                }
                else
                {
                    //  Check each substring checking that the int value is less than 255 and that is char[] length is !> 2
                    Int16 MAXVALUE = 255;
                    Int32 temp; // Parse returns Int32
                    foreach (String strOctet in arrOctets)
                    {
                        if (strOctet.Length > 3)
                        {
                            retVal = false;
                            break;
                        }
                        else
                        {

                            temp = int.Parse(strOctet);
                            if (temp > MAXVALUE)
                            {
                                retVal = false;
                                break;
                            }
                            if (temp == 0)
                            {
                                all0cnt++;
                            }
                            if (temp == MAXVALUE)
                            {
                                all255cnt++;
                            }
                        }//else
                    }//foreach

                }//else

                // all field zero of 255 
                if (all255cnt == 4 || all0cnt == 4)
                {
                    retVal = false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Only integer and . is allowed");
            }
            return retVal;
        }

        public string U16ToHex(ushort val)
        {
            string tmp = "";
            byte hexByte;

            for(int i=3; i>=0;i--)
            {
                hexByte = (byte)((val >> (4 * i)) & 0x000F);
                if (hexByte > 9)
                {
                    switch (hexByte)
                    {
                        case 10:
                            tmp += "A";
                            break;
                        case 11:
                            tmp += "B";
                            break;
                        case 12:
                            tmp += "C";
                            break;
                        case 13:
                            tmp += "D";
                            break;
                        case 14:
                            tmp += "E";
                            break;
                        case 15:
                            tmp += "F";
                            break;
                    }
                }
                else
                {
                    tmp += hexByte.ToString();
                }
            }

            return tmp;
        }

        public bool validateIntegerTextBox(string val)
        {
            int len = val.Length;
            bool retVal = true;
            char[] byteArray = val.ToCharArray(0, len);
            
            for(int i=0;i<len;i++)
            {
                if (byteArray[i] < 0x30 || byteArray[i] > 0x39)
                {
                    retVal = false;
                    break;
                }
            }
            return retVal;
        }
        public ushort HexToU16(string val)
        {
            ushort []tmp = { 0, 0, 0, 0 };
            ushort uVal = 0;
            int strLen = val.Length;

            if(strLen>4)
            {
                Console.WriteLine("invaild hex value-> value overflow with ushort data type");
            }  
            else if(strLen==0)
            {
                Console.WriteLine("text box can't be empty");                
            }       
            else
            {
                if(val.Length==1)
                {
                    val = "000" + val;
                }
                else if (val.Length == 2)
                {
                    val = "00" + val;
                }
                else if (val.Length == 3)
                {
                    val = "0" + val;
                }

                char[] hexVal = val.ToCharArray(0, val.Length);
                int subVal;
                for (int i = 0; i < 4; i++)
                {
                    // 0 to 9 -- 30 to 39
                    if (hexVal[i] >= 0x30 && hexVal[i] <= 0x39)
                    {
                        subVal = 0x30;
                    }
                    // a to f --61 to 66 
                    else if (hexVal[i] >= 0x61 && hexVal[i] <= 0x66)
                    {
                        subVal = 0x57;
                    }
                    // A to F--41 to 46
                    else if (hexVal[i] >= 0x41 && hexVal[i] <= 0x46)
                    {
                        subVal = 0x37;
                    }
                    else
                    {
                        subVal = 0;
                    }
                     tmp[i] = (ushort)((hexVal[i] - (ushort)(subVal)));
                }

                
                tmp[0]=   (ushort)(tmp[0]<< (4 * (val.Length - 1)));
                tmp[1] =  (ushort)(tmp[1] << (4 * (val.Length - 2)));
                tmp[2] =  (ushort)(tmp[2] << (4 * (val.Length - 3)));
                tmp[3] =  (ushort)(tmp[3] << (4 * (val.Length - 4)));

                uVal = (ushort) (tmp[0] + tmp[1] + tmp[2] + tmp[3]);
            }
            
            return uVal;
        }


    }//tcpUitility




}//namespace
