   M S B A M L   `   `   `  ����I  EftpMassUpgrade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:ftpMassUpgradeftpMassUpgrade  5   	   !    ftpMassUpgrade.ModbusAccess   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:ftpMassUpgrade     5   	     � Title$  ModbusAccess��5   	   $��592.008��6   $��597.541��6/   .��5	      � $��	0,0,-8,-9q�6    J� RenderTransformOrigin$ 0.527,0.527&�6   .+�5
   
   �� -   5         ��Name$ btMainBkWindow��5
      $��<< MainWindow��6)   $
��Left=�6G   $��
10,424,0,0q�6b   $	��Top=�6v   $	��121��6�   6�   �� 5   
   -   5         ��Name$ 
txtSlaveIP��5      $
��Left=�6&   $��27��6A   $��	87,26,0,0q�6M    �� TextWrapping$
 Wrap=�6`   $	��Top=�6t   $	��120��6�   �� 5   
   -   5        $ txtStartAddress��5      $
��Left=�6+   $��27��6F   $��
113,58,0,0q�6R   $
 Wrap=�6f   $	��Top=�6z   $��94��6�   �� 5   
   $��SlaveIP��6   $
��Left=�6"   $��	10,26,0,0q�6=   $	��Top=�6P   $��60��6h   $��27��6s   �� 5   
   $��Start Address��6   $
��Left=�6(   $��	10,58,0,0q�6C   $	��Top=�6V   $��98��6n   $��27��6y   �� 5   
   -   5        $ txtNReg��5      $
��Left=�6#   $��27��6>   $��
113,91,0,0q�6J   $
 Wrap=�6^   $	��Top=�6r   $��94��6�   �� 5   
   $��	Registers��6   $
��Left=�6$   $��	10,91,0,0q�6?   $	��Top=�6R   $��98��6j   $��27��6u   �� 5   
   -   5        $ btSendModbus��5      $��
SendModbus��6'   $
��Left=�6<   $��113,258,0,0q�6W   $	��Top=�6l   $��86��6�   6�   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ 	grdModbus��5         AutoGenerateColumns .  $
��Left=�6D   $��
222,33,0,0q�6_   $	��Top=�6s   $	��454��6�   $	��356��6�   
������$��Segoe UI Black)�6�   6�   �� 5   
   -   5        $ 	btDGClear��5      $��ClearAll��6$   $
��Left=�67   $��503,492,0,0q�6R   $	��Top=�6g   $��75��6   6�   �� 5   
   -   5        $ btAdd��5      $	��Add��6    $
��Left=�6.   $��
99,370,0,0q�6I   $	��Top=�6]   $��75��6u   6�   �� 5   
   -	   5        $ btBrowse��5      $��Browse��6#   $
��Left=�64   $��124,188,0,0q�6O   $	��Top=�6d   $��75��6|   6�   �� 5   
   -
   5         ��Name$ lblMBException��5      $�� ��6(   $
��Left=�63   $��
10,283,0,0q�6N   $	��Top=�6b   $	��197��6z   $��55��6�   �� 5   
   -   5         ��Name$ 
chReqInput��5      $��Request From File��6'   $
��Left=�6C   $��
17,145,0,0q�6^   $	��Top=�6r   6�   �� 5   
   -   5        $ txtFileName��5      $
��Left=�6'   $��23��6B   $��
17,185,0,0q�6N   $
 Wrap=�6b   $
��.xls��6v   $	��Top=�6�   $��91��6�   �� 5   
   -   5        $ btOpenExcel��5      $
��Open��6&   $
��Left=�65   $��108,512,0,0q�6P   $	��Top=�6e   $��75��6}   6�   5      5      