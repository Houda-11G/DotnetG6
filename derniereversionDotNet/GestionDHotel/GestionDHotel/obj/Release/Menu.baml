   M S B A M L   `   `   `  ����   GestionDHotel    GestionDHotel.Menu   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  Menu Principal��$	��600��$	��800��.��]� .+�c� $	��Top=�
�������$��50��.@��� -   $��Gestion des Chambres��$	��150��$��40��$��10q��� -   $��Liste des Employés��$	��150��$��40��$��10q��� -   $$��Mise à jour des données Empl��$	��150��$��40��$��10q��� -    ��Name$ NotificationIcon��$��40��$��40��$��10q�.���� $��notification_icon.png��$��20��$��20�� � ToolTip$ Nouvelles réservations!��� $��10q�.+�� $��Center=�$��Center=� � RowDefinitions �� $
��Auto ��� $��* � � ColumnDefinitions �� $��* ��� $��* �.+��� -    ��Name$ TypeChambreComboBox��$��Center=�$	��200��.@��� $��Single���� $��Double���� $��Suite���� $��Deluxe��&   System.Windows.Controls.DataGrid  -     Name$ ChambresDataGrid��   AutoGenerateColumns .  $��Stretch=�$	��300��$	��Top=�$��0,50,0,0q�$��1��$��0��   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn	   Header$	 Numéro��1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 �� *
NumChambre+  $
	 Type��
 �� *TypeChambre+  $	 	Capacité��
 �� *
Capacite+  $
	 Prix��
 �� *PrixParNuit+  $	 Description��
 �� *Description+  $	 Étage��
 �� *Etage+  $	 Statut��
 �� *Statut+4  .System.Windows.Controls.DataGridTemplateColumn  $	 Image��   CellTemplate �� .R��� $	��100��$	��100������ *Image+�� -   $��+��$
��Left=�$��
121,77,0,0q�$	��Top=����	#FF3D8F0E$��31��$��69��$
��Bold"�$��20(�$��Arial Rounded MT Bold)�