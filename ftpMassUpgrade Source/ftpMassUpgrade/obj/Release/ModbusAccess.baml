   M S B A M L   `   `   `  ���� I  EftpMassUpgrade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:ftpMassUpgradeftpMassUpgrade  !    ftpMassUpgrade.ModbusAccess   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     .localclr-namespace:ftpMassUpgrade       � Title$  ModbusAccess��$��666.947��$	��800�����	#FF158597.��� $��	0,0,-8,-9q� J� RenderTransformOrigin$ 0.527,0.527&�.+��� -    ��Name$ grpCmd��$��Modbus Command��$
��Left=�$��
17,156,0,0q�$	��Top=�$��97��$	��399�����	#FF55520C$��3q�$��25(�$
��Bold"�$��Italic$�� $��14,253,384,80q�.+��� $��
Query From��$
��Left=�$	��Top=�$	��402��$	��312��$��25(�$
��Bold"�$��Italic$����	#FF55520C$��3q��� -    ��Name$ btMainBkWindow��$��Close��$
��Left=�$��
10,575,0,0q�$	��Top=�$��77��$��39��$��20(�$��Italic$��� -   $ btSendModbus��$��Send Request��$
��Left=�$��131,575,0,0q�$	��Top=�$	��285��$��39��$��2q�$��20(�$��Italic$�$
��Bold"�&   System.Windows.Controls.DataGrid  -     Name$ 	grdModbus��   AutoGenerateColumns .  $
��Left=�$	��Top=�$	��535��$	��307�����	#FFCDCFD1$��Segoe UI Black)�$��
438,35,0,0q����	#FF55520C$��2q��� -   $ 	btDGClear��$��ClearAll��$
��Left=�$	��Top=�$	��127��$��20(�$
��Bold"�$��Italic$�$��39��$��617,575,0,0q��� -   $ 	btSaveAll��$��Save All��$
��Left=�$	��Top=�$	��127��$��20(�$
��Bold"�$��Italic$�$��39��$��438,575,0,0q�	� -    	�Name$ 
rdFromFile��$��	From File��$
��Left=�$	��Top=� d� 	IsChecked$ FalseY�$��
53,456,0,0q��� -    ��Name$ txtStartAddress��$
��Left=�$��20��	 �� TextWrapping$
	 Wrap=�$	��Top=�$	��141�����	#FFCDCFD1$��217,340,0,0q��� -	   
 ��Name$
 lblStartAddress��$��Start Address��$��15(�$
��Left=�$	��Top=�$	��107��$��27��$��
93,337,0,0q��� -
   $ txtNReg��$
��Left=�$��20��$
	 Wrap=�$	��Top=�$	��141�����	#FFCDCFD1$��217,372,0,0q��� -   $
 lblNReg��$��	Registers��$��15(�$
��Left=�$	��Top=�$	��107��$��32��$��
93,366,0,0q��� -   $ btBrowse��$��Browse��$
��Left=�$	��Top=�$��93��$��265,498,0,0q��� -   $ txtFileName��$
��Left=�$��20��$
	 Wrap=�$
��.xls��$	��Top=�$��93�����	#FFCDCFD1$��
93,499,0,0q�	� -   $ rdMbAdd��$��Modbus Address��$��15(�$
��Left=�$	��Top=�$	��141��$
 TrueY�$��
53,304,0,0q��� -   $
 lblMBException��$�� ��$
��Left=�$	��Top=�$	��141��$��29��$��217,456,0,0q��� -   $ txtValue��$
��Left=�$��23��$
	 Wrap=�$	��Top=�$	��141�����	#FFCDCFD1$��217,408,0,0q��� $��Value��$
��Left=�$	��Top=�$��93��$��
93,406,0,0q��� $��Request Type��$
��Left=�$	��140��$	��Top=�$��25(�$��Italic$�$
��Bold"�$	��402��$��3q����	#FF55520C$��	14,14,0,0q�� $��50,61,409,491q�.+��� -   $ 
txtSlaveIP��$
��Left=�$��20��$
	 Wrap=�$	��Top=�$	��150��$ 0.514,1.083&�$��
148,39,0,0q����	#FFCDCFD1�� -   $
 
lblSlaveIP��$
��Left=�$��15(�$��ServerIP��$	��Top=�$��78��$��29��$��0,30,0,0q�	� -   $ rdAll��$	��All��$
��Left=�$��15(�$	��Top=�$
 TrueY�$��3,0,0,0q�	� -   $ rdInd��$��
Individual��$
��Left=�$��15(�$	��Top=�$	��105��$��	193,4,0,0q�� $��50,202,409,392q�.+�	� -   $ rdMbRead��$
��Read��$
��Left=�$
 TrueY�$	��Top=�$��20(�$
��Bold"�$��Italic$�	� -   $ 	rdMbWrite��$��Write��$
��Left=�$��	197,2,0,0q�$ FalseY�$	��Top=�$��20(�$
��Bold"�$��Italic$�