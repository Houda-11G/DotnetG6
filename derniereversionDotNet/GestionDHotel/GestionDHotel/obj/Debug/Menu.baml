   M S B A M L   `   `   `  ����  GestionDHotel    GestionDHotel.Menu   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  Menu Principal��5   	   $	��600��6    $	��800��6-   .��5      ]� .+�5   
   c� $	��Top=�6   
�������$��50��6>   .@�5	      �� -   5        $��Gestion des Chambres��5	      $	��150��64   $��40��6@   $��10q�6L   6X   �� 5
      -   5        $��Liste des Employés��5
      $	��150��62   $��40��6>   $��10q�6J   6V   �� 5      -   5        $$��Mise à jour des données Empl��5      $	��150��6<   $��40��6H   $��10q�6T   6`   �� 5      -   5         ��Name$ NotificationIcon��5      $��40��6/   $��40��6:   $��10q�6F   .��5      �� $��notification_icon.png��6   $��20��67   $��20��6B    � ToolTip$ Nouvelles réservations!��6N   5      5      � 5   
   $��10q�6   .+�5      � $��Center=�6   $��Center=�60    � RowDefinitions 5      �� 5      $
��Auto �6$   �� 5      $��* �6$    � ColumnDefinitions 5      �� 5      $��* �6'   �� 5      $��* �6'   .+�5      �� -   5         ��Name$ TypeChambreComboBox��5      $��Center=�5      $	��200��5       .@�5"      �� $��Single��6#   �� 5#      $��Double��6#   �� 5$      $��Suite��6#   �� 5%      $��Deluxe��6#   5&      &   System.Windows.Controls.DataGrid  5)      -   5          Name$ ChambresDataGrid��5)         AutoGenerateColumns .  $��Stretch=�6Q   $	��300��6o   $	��Top=�6|   $��0,50,0,0q�6�   $��1��6�   $��0��6�      Columns 5*      0  *System.Windows.Controls.DataGridTextColumn  5+      ,  &System.Windows.Controls.DataGridColumn	   Header$	 Numéro��61   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 6A   �� *
NumChambre+  5,      $
	 Type��61   
 6?   �� *TypeChambre+  5-      $	 	Capacité��61   
 6C   �� *
Capacite+  5.      $
	 Prix��61   
 6?   �� *PrixParNuit+  5/      $	 Description��61   
 6F   �� *Description+  50      $	 Étage��61   
 6@   �� *Etage+  51      $	 Statut��61   
 6A   �� *Statut+4  .System.Windows.Controls.DataGridTemplateColumn  52      $	 Image��65      CellTemplate 53   "   �� 54   &   .R�55   *   �� $	��100��6I   $	��100��6U   ��60   �� *Image+6U   56   '   58      5;      5>      �� 5?      -   5        $��+��5?      $
��Left=�6!   $��
121,77,0,0q�6<   $	��Top=�6P   ���	#FF3D8F0E$��31��6�   $��69��6�   $
��Bold"�6�   $��20(�6�   $��Arial Rounded MT Bold)�6�   5@      5A      5B      