   M S B A M L   `   `   `  ����J  FProyecto grafos, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:Proyecto_grafosProyecto_grafos  5   	        Proyecto_grafos.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   /localclr-namespace:Proyecto_grafos     5   	     � Title$  AirWar��5   	   $	��600��6   $	��900��6%   .��5      �  � ColumnDefinitions 5   
   �� 5	      $��* �6   �� 5      $	��250 �6   .+�5   
   �� -   5        $��
MapaCanvas��5      
����ح�$��0��6:   .+�5      -   'Proyecto_grafos.ControlBateriaAntiaerea  -   5          Name$ BateriaAntiaerea��5   ,    �� Left$	 375��6F   
 �� Top$	 550��6X   5      ]� 5   
   $��1��6   
�������.+�5      �� $��Aviones Derribados��6   $
��Bold"�62   $��16(�6D   
�������$��5q�6e   �� 5      -   5        $��AvionesDerribadosListBox��5      $	��200��66   $��5q�6C   ��5      �� 5   
   .R�5      �� .+�5       �� $
��Bold"�62   ��6   �� *Name+62   �� 5!      $��Combustible: ��6   �� 5"      ��6   �� *Combustible+5#      5$      5&      �� 5)      $��Módulos AI��6   $
��Bold"�6*   $��16(�6<   
�������$��5q�6]   �� 5,      -   5        $��ModulosAIListBox��5,      $	��200��6.   $��5q�6;   ��5-      �� 5.      .R�5/      �� .+�50      �� $��Módulo: {Binding Rol}��6(   $
��Bold"�6E   �� 51      $��ID: {Binding ID}��6(   �� 52      $,��&Horas de Vuelo: {Binding HorasDeVuelo}��6(   53      54      56      57      58      59      