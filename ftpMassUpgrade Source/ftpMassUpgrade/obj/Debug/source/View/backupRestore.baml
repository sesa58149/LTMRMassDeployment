   M S B A M L   `   `   `  ����I  EftpMassUpgrade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:ftpMassUpgradeftpMassUpgrade  5   	   "    ftpMassUpgrade.backupRestore   5   
   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:ftpMassUpgrade     5   	     � Title$  backupRestoreWin��5   	   $��720.659��6"   $��689.079��63   .��5	      � $��
0,0,-34,-6q�6   .+�5
   
   &   System.Windows.Controls.DataGrid  -   5          Name$ grdBnRDeviceList��5
      $
��Left=�6-   $��	10,45,0,0q�6H   $	��Top=�6[   $	��581��6s   $	��382��6�   �� 5   
   -   5         ��Name$ btClose��5      $��Close Window��6"   $
��Left=�69   $��585,280,0,0q�6T   $	��Top=�6i   $��88��6�   6�   � 5   
   $��10,631,302,-320q�6   .+�5      	� -   5         	�Name$ rdBackup��5      $��Backup��6,   $
��Left=�6=   $	��Top=�6X   $��0,9,0,0q�6p   $��26��6�   $��75��6�   	� 5      -   5        $ 	rdRestore��5      $��Restore��6-   $
��Left=�6?   $��	303,9,0,0q�6Z   $	��Top=�6m   $��26��6�    J� RenderTransformOrigin$ 0.519,0.462&�6�   $��75��6�    � BindingGroup 5      &   System.Windows.Data.BindingGroup  5      5      5      � 5   
   $��413,45,-172,197q�6   .+�5      �� -   5        $ 	btRef8Amp��5      $��8AMP Browse��6(   $
��Left=�6>   $��	142,3,0,0q�6Y   $	��Top=�6l   $��96��6�   �� 5      -   5        $ 
btRef27Amp��5      $��27AMP Browse��6)   $
��Left=�6@   $��
142,36,0,0q�6[   $	��Top=�6o   $��96��6�   �� 5      -   5        $ btRef100Amp��5      $��100AMP Browse��6*   $
��Left=�6B   $��
142,70,0,0q�6]   $	��Top=�6q   $��96��6�   �� 5      -   5         ��Name$ 
txtRef8Amp��5      $
��Left=�6*   $��23��6E    �� TextWrapping$
 Wrap=�6Q   $	��Top=�6e   $	��120��6}   �� 5      -	   5        $ txtRef27Amp��5      $
��Left=�6+   $��23��6F   $��0,33,0,0q�6R   $
 Wrap=�6d   $	��Top=�6x   $	��120��6�   �� 5      -
   5        $ txtRef100Amp��5      $
��Left=�6,   $��23��6G   $��0,67,0,0q�6S   $
 Wrap=�6e   $	��Top=�6y   $	��120��6�   5      �� 5   
   -   5        $ btStart��5      $��Start��6"   $
��Left=�62   $��413,280,0,0q�6M   $	��Top=�6b   $��75��6z   6�   � 5   
   -   5         �Name$ pbBnR��5      $
��Left=�6%   $��19��6@   $��413,202,0,0q�6L   $	��Top=�6a   $	��236��6y   �� 5    
   -   5        	 ��Name$	 lblDeviceId��5       $�� ��6%   $
��Left=�60   
��� � �$��413,168,0,0q�6^   $	��Top=�6s   $	��223��6�   5"      5#      