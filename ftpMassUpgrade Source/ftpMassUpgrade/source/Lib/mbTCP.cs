using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
namespace ftpMassUpgrade
{
    class mbTCP
    {
        //    char [] readBuf = new char[255];
        //    char[] ipAdd = new char[25];
        public string clientIpStr { get; set; }
        public ushort port { get; set; }
        public byte[] readBuf { get; set; }
        public byte[] writeBuf { get; set; }
        public string serverStartAddress { get; set; }
        public ushort serverRange { get; set; }
        public TcpClient clientConnection { get; set; }
        public NetworkStream networkStream { get; set; }
        public IPEndPoint clientEndPoint { get; set; }
        public IPEndPoint serverEndPoint { get; set; }
        public ftpMassUpgrade.toolUitility myutility { get; set; }
        public bool isMbClietOpen { get; set;}
        private UInt16 mbTransactionId { get; set; }
        private UInt16 mbProtocolId { get; set; }
        public UInt32 mbMaxFrameSize { get; set; }
        public mbTCP()
        {
            port = 0;
            readBuf = new byte[512];
            writeBuf = new byte[512];
            clientIpStr = new string(new char[25]);
            serverStartAddress = new string(new char[25]);
            serverRange = 0;
            myutility = new ftpMassUpgrade.toolUitility();
            isMbClietOpen = false;
            mbProtocolId = 0;
            mbTransactionId = 0;
            mbMaxFrameSize = 256;


        }

        private UInt16 encodeReadRequest(UInt16 startAddress, UInt16 numberOfRegister)
        {
            UInt16 index = 0;
            
            mbTransactionId++;
            index = myutility.copyUint16Endian(writeBuf, mbTransactionId, index);
            index = myutility.copyUint16Endian(writeBuf, mbProtocolId, index);
            index = myutility.copyUint16Endian(writeBuf, (1 + 1 + 2 + 2), index);
            writeBuf[index++] = 0x01;
            writeBuf[index++] = 0x03;
            index = myutility.copyUint16Endian(writeBuf, startAddress, index);
            index = myutility.copyUint16Endian(writeBuf, numberOfRegister, index);
            
            return index;
        }

        private int decodeReadResponse(byte[] src, UInt16[] dst, Int32 len)
        {
            int retCode = 0;
            int mbheader = 9;
            int j = 0;
            byte FC = src[mbheader - 2];

            for (int i = mbheader; i < len;)
            {
                /* copy byte in register and convert big Endian to little Endian*/
                dst[j] = (UInt16)(src[i + 1] + (src[i] << 8));
                i += 2;
                j++;
            }
            if((FC&0x80)==0x80)
            {
                dst[0] = (UInt16)(src[mbheader-1]);
                retCode = -1;
            }
            return retCode;
        }

        private Int32 decodeWriteResponse(byte [] response)
        {
            Int32 retCode = 0;

            int MbHeaderSize = 8;

            if ((response[MbHeaderSize] & 0x80) == 0x80)
                retCode = -1;

            return retCode;
        }
        private UInt16 encodeWriteRequest(UInt16 startAddress, UInt16 numberOfRegister ,UInt16 [] regVal)
        {
            UInt16 index = 0;
            mbTransactionId++;
            index = myutility.copyUint16Endian(writeBuf, mbTransactionId, index);
            index = myutility.copyUint16Endian(writeBuf, mbProtocolId, index);
            index = myutility.copyUint16Endian(writeBuf, (ushort)(1 + 1 + 2 + 2 + 1 + (numberOfRegister * 2)),index);
            writeBuf[index++] = 0x01;
            writeBuf[index++] = 0x10;
            index = myutility.copyUint16Endian(writeBuf, startAddress, index);
            index = myutility.copyUint16Endian(writeBuf, numberOfRegister, index);
            writeBuf[index++] = (byte)(numberOfRegister * 2);
            for(int i=0;i<numberOfRegister;i++)
            {
                index = myutility.copyUint16Endian(writeBuf, regVal[i], index);
            }
            return index;

        }
        public Int32 modbusInit(string ipAddress, UInt16 port)
        {
            Int32 retCode = -1;

            //MessageBox.Show(ipAddress);
            //MessageBox.Show(port.ToString());
            if ( ipAddress!=null &&(ipAddress.Count()) > 0 )
            {
                long ipAdd = myutility.IP2Long(ipAddress);

                clientEndPoint = new IPEndPoint(ipAdd,0);

                if (clientEndPoint != null)
                {                    
                    clientConnection = new TcpClient(clientEndPoint);

                    if (clientConnection != null)
                    {
                        clientConnection.ReceiveTimeout = 2000;// wait 2 sec for response 
                        clientConnection.ReceiveBufferSize = (int)mbMaxFrameSize;
                        clientConnection.SendBufferSize = (int)mbMaxFrameSize;
                        clientConnection.SendTimeout = 1000; //wait1 Sec for tx data
                                                
                        retCode = 0;
                    }
                    else
                    {
                        Console.WriteLine("fail to create client Connection");
                    }
                }
                else
                {
                    Console.WriteLine("fail to create client Endpoint");
                }
            }
            return retCode;
        }
        public Int32 modbusSetServerAddress(string startAdd)
        {
            return 0;
        }
        public Int32 modbusSetServerAddress(string startAdd,UInt16 numberOfServer)
        {

            clientIpStr = String.Copy(startAdd);
            serverRange = numberOfServer;
            return 0;
        }
        public Int32 mbOpen(string IPadd, UInt16 port)
        {
            Int32 retCode = -1;
            try
            {
                if (isMbClietOpen == true)
                {
                    MessageBox.Show("mbClient is already Open");
                }
                else
                {
                    if (IPadd.Count() > 0 && port != 0)
                    {

                        IAsyncResult ar = clientConnection.BeginConnect(IPadd, port, null, null);
                        System.Threading.WaitHandle wh = ar.AsyncWaitHandle;
                        try
                        {
                            if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2), false))
                            {
                                clientConnection.Close();
                                throw new TimeoutException();
                            }
                           
                            networkStream = clientConnection.GetStream();
                            isMbClietOpen = true;
                            retCode = 0;
                            
                        }
                        finally
                        {
                            wh.Close();
                        }

                    }
                } //else
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return retCode;
        }
        public Int32 mbClose()
        {
            Int32 retVal = -1;

            if(isMbClietOpen ==true)
            {
                networkStream.Close();
                
                clientConnection.Close(); 
                
                isMbClietOpen = false;
                retVal = 0;
            }
            return retVal;
        }
       
        private Int32 mbSend( byte[] buffer, int size)
        {
            Int32 retVal = -1;
            int writeSize = size;
            if(isMbClietOpen ==false)
            {
                MessageBox.Show("mbClient is not Connected");
            }
            else
            {
                if (size <= mbMaxFrameSize)
                {
                    //set 10 mSec timeout for sending
                    networkStream.WriteTimeout = 10;
                    if (networkStream.CanWrite)
                    {
                        networkStream.Write(buffer, 0, size);
                        retVal = 0;
                    }
                }
            }

            return retVal;
        }

        private Int32 mbReceive(byte[] buffer, int size)
        {
            Int32 retVal = -1;
            int readSize = size;


            if (isMbClietOpen == false)
            {
                MessageBox.Show("mbClient is not Open");
            }
            else
            {
                // Get the stream used to read the message sent by the server.
               // NetworkStream networkStream = clientConnection.GetStream();
                // Set a 10 millisecond timeout for reading.
                networkStream.ReadTimeout = 1000;
                // Read the server message into a byte buffer.
                //byte[] bytes = new byte[256];
                if (size > mbMaxFrameSize)
                {
                    readSize = size;
                }
                if (networkStream.CanRead)
                {
                    retVal = networkStream.Read(buffer, 0, readSize);
                }
                //Convert the server's message into a string and display it.
                //String data = Encoding.UTF8.GetString(buffer);
                //Console.WriteLine("Server sent message: {0}", data);                
                
            }

            return retVal;
        }

       
        public Int32 mbRead(UInt16 startAddress, UInt16 numOfReg, UInt16[] buff)
        {
            Int32 retCode = -1;
            
            if(!( startAddress==0 || numOfReg==0||buff==null))
            {
                int len = encodeReadRequest(startAddress, numOfReg);
                retCode = mbSend(writeBuf, len);
                if( retCode == 0)
                {
                    retCode = mbReceive(readBuf, (int)mbMaxFrameSize);
                    if (retCode > 0 && retCode <= mbMaxFrameSize)
                    {
                       retCode = decodeReadResponse(readBuf, buff,retCode);
                       // readBuf.CopyTo(buff, 0);
                        
                    }
                }
            }
            return retCode;
        }

        public Int32 mbWrite(UInt16 startAddress, UInt16 nReg, UInt16 []regVal)
        {
            Int32 retCode = -1;

            if (!(startAddress == 0 || nReg == 0 || regVal == null))
            {
                int len = encodeWriteRequest(startAddress, nReg, regVal);
                retCode = mbSend(writeBuf, len);
                if (retCode == 0)
                {
                    retCode = mbReceive(readBuf, (int)mbMaxFrameSize);
                    if (retCode > 0 && retCode <= mbMaxFrameSize)
                    {
                        retCode = decodeWriteResponse(readBuf);
                    }
                }
            }

            return retCode;
        }



        public Int32 pingDevice( string path)
        {
            //string command = "ping 192.168.2.1";
            string command = "mkdir" +""+ path+"pawanTest";
            Int32 retCode = executeWinShallCmd(command);
            if ( retCode!= 0)
            {
                MessageBox.Show(" cmd executed with exception =");
                MessageBox.Show(retCode.ToString());
            }
            return 0;
        }

        private Int32 executeWinShallCmd(string cmd)
        {
            Int32 exitCode = -1;
            ProcessStartInfo processInfo;
            Process process;

            //processInfo = new ProcessStartInfo("cmd.exe", "/c " + cmd);
            processInfo = new ProcessStartInfo("cmd.exe", cmd);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

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
        } 
    }//class mbTCP




}//Name space 
