   M S B A M L   `   `   `  ����  GestionDHotel    GestionDHotel.Chambre   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Ajouter une Chambre��5   	   $	��600��6%   $	��600��62   .��5      �  � ColumnDefinitions 5   
   �� 5      $��2* �6   �� 5	      $��3* �6   .+�5   
   �� $��0��6   $��20q�6%   .+�5      ��  �� Orientation$ 
Horizontal=�6   $
��0,10q�62   .+�5      �� $��Numéro Chambre:��6   $	��150��62   $��Center=�6>   �� 5      -   5        $��NumChambreTextBox��5      $	��200��63   5      �� 5      $ 
Horizontal=�6   $
��0,10q�62   .+�5      �� $��Type de Chambre:��6   $	��150��63   $��Center=�6?   �� 5      -   5        $��TypeChambreComboBox��5      $	��200��66   .@�5      �� $��Single��6#   �� 5      $��Double��6#   �� 5      $��Suite��6#   �� 5      $��Deluxe��6#   5      5      �� 5       $ 
Horizontal=�6   $
��0,10q�62   .+�5!      �� $��
Capacité:��6   $	��150��6,   $��Center=�68   �� 5"      -   5        $��CapaciteTextBox��5"      $	��200��61   5#      �� 5&      $ 
Horizontal=�6   $
��0,10q�62   .+�5'      �� $��Prix par Nuit (en €):��6   $	��150��68   $��Center=�6D   �� 5(      -   5        $��PrixParNuitTextBox��5(      $	��200��64   5)      �� 5,      $ 
Horizontal=�6   $
��0,10q�62   .+�5-      �� $��Description:��6   $	��150��6/   $	��Top=�6;   �� 5.      -   5        $��DescriptionTextBox��5.      $	��200��64   $��60��6@   5/      �� 52      -   5        $��Choisir Image��52      $	��120��6-   $
��0,10q�69   $
��Left=�6G   6b   �� 55      $ 
Horizontal=�6   $
��0,10q�62   .+�56      �� $��Étage:��6   $	��150��6)   $��Center=�65   �� 57      -   5        $��EtageTextBox��57      $	��200��6.   58      �� 5;      $ 
Horizontal=�6   $
��0,10q�62   .+�5<      �� $��Statut:��6   $	��150��6*   $��Center=�66   �� 5=      -   5        $��StatutComboBox��5=      $	��200��61   .@�5>      �� $��Libre��6#   �� 5?      $��
Réservée��6#   �� 5@      $��En maintenance��6#   5A      5B      �� 5F      -	   5        $��Ajouter Chambre��5F      $	��120��6/   $��30��6;   $��Right=�6G   $
��0,20q�6c   6q   5G      �� 5J   
   $��1��6   $��20q�6%   .+�5K      �� $��Image de la Chambre��6   $
��Bold"�63   $��0,0,0,10q�6E   �� 5L      -
   5        $��ChambreImage��5L      $	��250��6(   $	��200��64   $
��0,10q�6A   $��Center=�6O   �� 5N      $��Visualisation��6   $
��Bold"�6-   $
��0,10q�6?   �� 5O      $��
Voir Image��6   $	��120��6*   $��Center=�66   $
��0,10q�6S   5P      5Q      5R      