   M S B A M L   `   `   `  ����H  DChemistryProg, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:ChemistryProg.CommandsChemistryProg.Commands  5   	   -clr-namespace:ChemistryProgChemistryProg  6?   M+clr-namespace:ChemistryProg.InformationMenuChemistryProg.InformationMenu  6i       ChemistryProg.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   9commands$clr-namespace:ChemistryProg.Commands     5   	   -localclr-namespace:ChemistryProg     6?   B
processing+clr-namespace:ChemistryProg.InformationMenu     6i   -   5          � Title$  	Crystal3D��5   	   $	��900��6   $
��1280��6(   
�������.��5      � -   5         �Name$ mainGrid��5      $��0,0,0,9q�6    � RowDefinitions 5   
   �� 5      $��20 �6   6*   �� 5      �� 5      �� 5      $��40 �6   �� 5      $��40 �6    � ColumnDefinitions 5   
   �� 5      �� 5      �� 5      �� 5      $��2* �6   .+�5   
   o� $��0��6   $��4��6   $��25��63   $	��Top=�6?   .@�5      m� $��Project��6   .@�5      m� $��Save Screenshot��6    m� Command$- 'commands:CustomCommands.TakeScreenshoot��64   m� 5      -   5        $
��Exit��5      6)   5       m� 5!      $��
Projection��6   .@�5"      m� -   5        $��XY��5"      6'   m� 5#      -   5        $��XZ��5#      6'   5$      5&      � 5'   
   -   5        $��1��5'      $��3��6   $��2��61   
�������.+�5)      '� -   5         '�Name$ monitor��5)      �� 5*      -   5         ��Name$ monitorCanvas��5*      � 5+      -	   5         �Name$ popup1��5+       � 	StaysOpen .  $��Mouse=�66   $	��180��6H   	 � AllowsTransparency	 . .��5-      �� $
��Wrap=�6   $	��180��60   
�������
 J� Opacity$	
 0.8Q�6S   5.      5/      50      �� 51   
   $��1��6   $��2��6"   $��3��63   .@�52      �� $
��Info��6   .��53      /   )ChemistryProg.InformationMenu.InfoControl  54      �� 55      $��Relative��6   .��56      &   System.Windows.Controls.DataGrid     
IsReadOnly .    SelectionMode$ Single=�6d      AutoGenerateColumns .    ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$ * 6�   ��6   ��  �� Path$ RelativeList� �� Mode$ OneWay=�6�   57      �� 58      $
��Real��6   .��59         .  . $ * 6|   ��6   �� $ RealList�$ OneWay=�6|   5:      �� 5;      $��Distance��6   .��5<         .  . $ * 6�   ��6   �� $ DistanceList�$ OneWay=�6�   5=      �� 5>      $��Angle��6   .��5?         .  . $ * 6|   ��6   �� $ 	AngleList�$ OneWay=�6|   5@      �� 5A      $��
FactCharge��6   .��5B         .  . $ * 6�   ��6   �� $ FactChargeList�$ OneWay=�6�   5C      �� 5D      $��All DIstance��6   .��5E         .  . $ * 6�   ��6   �� $ AllDistanceList�$ OneWay=�6�   5F      �� 5G      $��Polyhedra Values��6   .��5H         .  . $ * 6�   ��6   �� $ PolyhedraList�$ OneWay=�6�   5I      5J      �� 5K   
   $��3��6   $��Right=�6   $��Center=�6@   $��Группа��6b   $��40��6s   $��Center=�6   �� 5L   
   -
   5        $��Center=�5L      $��3��6j   $��1��6w   $��0,0,10,0q�6�   $��30��6�   $��Center=�6�   ��65   �� $ 
GroupsList�$ TwoWay=�6�   �� 5O   
   $��4��6   $��Right=�6   $��Center=�6@   $��Соединение��6b   $��0,5,0,0q�6w   �� 5P   
   -   5        $��Center=�5P      $��4��6m   $��1��6z   $��0,5,10,5q�6�   ��65   �� $ CompoundsList�$ TwoWay=�6�   �� 5Q   
   -   5        $��2��5Q   �   $��3��6�   $��30��6�   $��5,0,5,0q�6�   $��Center=�6�   ��6   �� $  DistanceMesurer.ButtonName�$ TwoWay=� �� NotifyOnSourceUpdated .  �� UpdateSourceTrigger$ PropertyChanged=�6�   �� 5R   
   -   5        $��2��5R   �   $��4��6�   $��5,5,5,5q�6�   ��6   �� $ AngleMesurer.ButtonName�$ TwoWay=� . $ PropertyChanged=�6�   �� 5S   
   -   5        $��Button��5S      $��3��6"   $
��Left=�62   $��
227,10,0,0q�6M   $��4��6a   $	��Top=�6n   $��76��6�   $��25��6�   6�   �� 5T   
   -   5         ��Name$ PathTB��5T      $��3��6"    �� 
IsReadOnly .  $
��Left=�6F   $��23��6a   $��	85,10,0,0q�6m   $��3��6�    �� TextWrapping$
 Wrap=�6�   $	��Top=�6�   $	��328��6�   5U      5W      