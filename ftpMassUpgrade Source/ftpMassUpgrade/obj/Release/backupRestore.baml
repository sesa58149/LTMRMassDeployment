   M S B A M L   `   `   `  ���� I  EftpMassUpgrade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:ftpMassUpgradeftpMassUpgrade  "    ftpMassUpgrade.backupRestore   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     .localclr-namespace:ftpMassUpgrade       � Title$  backupRestoreWin��$��754.659��$��731.079�����	#FF158597.��� $��
0,0,-34,-6q�.+��� -    ��Name$ 	grpAction��$��Action��$
��Left=�$��
10,626,0,0q�$	��Top=�$��86��$	��382��$��20(��� -   $ grpProgress��$��Activity Progress��$
��Left=�$��429,475,0,0q�$	��Top=�$	��100��$	��272��$��20(��� -   $ grpComp��$ ��Config Compatibility check��$
��Left=�$��429,208,0,0q�$	��Top=�$	��130��$	��272��$��20(�{�.  �� -   $ 
grpFileSel��$��Configuration Files ��$
��Left=�$��
429,34,0,0q�$	��Top=�$	��145��$	��272��$��20(�{�.  &   System.Windows.Controls.DataGrid  -     Name$ grdBnRDeviceList��$
��Left=�$��	10,45,0,0q�$	��Top=�$	��581��$	��382���� -    ��Name$ btClose��$��Close Window��$
��Left=�$��563,606,0,0q�$	��Top=�$��88��� $��10,656,302,13q�.+�	� -    	�Name$ rdBackup��$��Backup��$
��Left=�$	��Top=�$��	10,10,0,0q�$��25��$	��101��$��20(�	� -   $ 	rdRestore��$��Restore��$
��Left=�$��
287,10,0,0q�$	��Top=�$��25�� J� RenderTransformOrigin$ 0.519,0.462&�$��97��$��20(� � BindingGroup &   System.Windows.Data.BindingGroup  �� -	   $ 	btRef8Amp��$��8AMP Browse��$
��Left=�$��96��$��20��$��Bottom=�$��581,0,0,639q��� -
   $ 
btRef27Amp��$��27AMP Browse��$
��Left=�$	��Top=�$��96��$��581,104,0,0q��� -   $ btRef100Amp��$��100AMP Browse��$
��Left=�$	��Top=�$��96��$��581,141,0,0q��� -    ��Name$ 
txtRef8Amp��$
��Left=�$��23�� �� TextWrapping$
 Wrap=�$	��Top=�$	��120��$��
439,71,0,0q����	#FFCDCFD1�� -   $ txtRef27Amp��$
��Left=�$��23��$
 Wrap=�$	��Top=�$	��120��$��439,104,0,0q����	#FFCDCFD1�� -   $ txtRef100Amp��$
��Left=�$��23��$
 Wrap=�$	��Top=�$	��120��$��439,138,0,0q����	#FFCDCFD1�� -   $ btStart��$��Start��$
��Left=�$��413,606,0,0q�$	��Top=�$��75��� -   	 �Name$	 pbBnR��$
��Left=�$��19��$��439,541,0,0q�$	��Top=�$	��236���� -   
 ��Name$
 lblDeviceId��$�� ��$
��Left=�
��� � �$��439,507,0,0q�$	��Top=�$	��223���� -    ��Name$ chComRef��$��ComRef��$
��Left=�$��437,250,0,0q�$	��Top=� d� 	IsChecked$
 TrueY��� -   $ chKuVer��$��KU Ver��$
��Left=�$��557,270,0,0q�$	��Top=��� -   $ chKCVer��$��KC Ver��$
��Left=�$��479,270,0,0q�$	��Top=��� -   $ 	chSerialN��$��Serial Number��$
��Left=�$��437,305,0,0q�$	��Top=�