   M S B A M L   `   `   `  ���� I  EftpMassUpgrade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:ftpMassUpgradeftpMassUpgrade      ftpMassUpgrade.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     .localclr-namespace:ftpMassUpgrade       � Title$!  TeSysT Mass Upgrade--- V6.0��$	��810��$
��1280��$��15(�
�������.��� -    �Name$ mainWin�� J� RenderTransformOrigin$ 0.498,0.515&�$	��Top=����	#FF158597$
��Left=�$	��993��$
��1656��$��0,2,-385,-3q�.+��� $
��Left=�$��
118,47,0,0q�$	��Top=�&   System.Windows.Controls.DataGrid  -     Name$ grdDeviceList�����	#FFCDCFD1$
��Left=�$��
684,21,0,0q�$	��Top=�$	��652��$	��542��$y�Visible=�|�.     FrozenColumnCount$ 3��   GridLinesVisibility$ Vertical=��� -    ��Name$ 	btUpgrade��$��LTMR Upgrade��$
��Left=�$��68��$��348,394,0,0q�$	��Top=�$	��280��$��24(��� -   $ btTest��$��Close��$
��Left=�$��1105,682,0,0q�$	��Top=�$	��121��$��45��$��20(��� -   $ btBackup��$��LTMR Backup & Restore��$
��Left=�$��
50,394,0,0q�$	��Top=�$	��280��$��24(�$��68���� -    ��Name$ grpList��$��Network Scan��$
��Left=�$��	35,13,0,0q�$	��Top=�$��20(�$	��321��$	��624���� -   $ grpConfi��$��Device Managment��$
��Left=�$��
35,354,0,0q�$	��Top=�$��20(�$	��148��$	��625���� -   $ btTestProgrssBar��$��TestPB��$
��Left=�$��684,682,0,0q�$	��Top=�$	��102��$��45��$��20(��� $��Local Interface��$
��Left=�$��43��$	��Top=�$	��226��$��24(�$��	51,47,0,0q��� -	    ��Name$ lbIPAddress��$��Local IPAddress��$
��Left=�$��43��$	��Top=�$	��193��$��24(�$��
435,47,0,0q��� -
   	 ��Name$	 cbIPAddress��$
��Left=�$	��Top=�$	��196��$��38��$��
432,90,0,0q����� 
 �� EndPoint$	
 0,1&� �� 
StartPoint$	 0,0&�.���  � Color$ 	#FFF0F0F0�� � Offset$ 0Q�� $ 	#FFCDCFD1��$ 1Q��� -   $	 cbInterface��$
��Left=�$��38��$	��Top=�$	��225��$��	51,90,0,0q����� $	
 0,1&�$	 0,0&�.��� $ 	#FFF0F0F0��$ 0Q�� $ 	#FFCDCFD1��$ 1Q��� -    ��Name$ 
txtIPStart��$
��Left=�$��36�� �� TextWrapping$
 Wrap=�$	��Top=�$	��226��$��
51,177,0,0q����	#FFCDCFD1�� -   $ 
txtIPRange��$
��Left=�$��35��$
 Wrap=�$	��Top=�$	��196��$��432,178,0,0q����	#FFCDCFD1�� -   $ btScan��$
��Scan��$
��Left=�$	��Top=�$	��136��$��38��$��24(�$��492,276,0,0q�{�.  �� -   $ lbIPAddress_Copy��$��Start Address��$
��Left=�$��40��$	��Top=�$	��226��$��24(�$��
50,132,0,0q��� -   $ lbIPAddress_Copy1��$��Range��$
��Left=�$��40��$	��Top=�$	��193��$��24(�$��435,132,0,0q�� -    �Name$ 
pbScanning��$
��Left=�$��35��$	��Top=�$	��577��$��
51,235,0,0q����	#FFCDCFD1�� -   $ lbIConStatus��$�� ��$
��Left=�$	��Top=�$	��226�����	#FFCDCFD1$��38��$��
51,276,0,0q��� -   $ 
btMBAccess��$��Modbus Register Access��$
��Left=�$��
50,568,0,0q�$	��Top=�$	��280��$��67��$��24(��� -   $ grpMbAccess��$��Modbus Register Access��$
��Left=�$��
35,525,0,0q�$	��Top=�$��20(�$	��148��$	��625��