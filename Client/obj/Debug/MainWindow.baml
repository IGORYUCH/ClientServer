   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient  5   	       Client.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:Client     5   	     � Title$  Client��5   	   $	��420��6   $	��500��6%   .��5	      � $��0,0,2,2q�6   .+�5
   
   �  � RowDefinitions 5      �� 5      $
��auto �6    �� 5      $��8 �6    �� 5      �� 5      $��8 �6    �� 5      $
��auto �6    .+�5      � $��0��6   $
��Left=�6    � ColumnDefinitions 5      �� 5      $
��auto �6'   �� 5      $
��auto �6'   �� 5      $
��auto �6'   �� 5      $
��auto �6'   �� 5      $
��auto �6'   �� 5      $
��auto �6'   �� 5      $
��auto �6'   .+�5      �� $��0��6   $��Статус:��6(   $
��Left=�6:   $��Center=�6U   �� 5      -   5         ��Name$ ConnectionStatusLabel��5   (   $��1��6   $
��Left=�6l   $	��Top=�6�   $��90��6�   ��6G   �� *ConnectionStatus+6�   �� 5      -   5         ��Name$ ConnectDisconnectButton��5   )   $��2��6   $��Center=�6�   $��Center=�6�   $��90��6�   $��22��6�   ��6J   �� *ConnectDisconnectButtonStatus+��6|   �� * ConnectDisconnectButtonCommand+6�   �� 5       $��3��6   $��	Порт:��6)   �� 5!      $��4��6   $��22��6*   $��60��66   $��Center=�6A   $��Center=�6^   ��6y   �� *Port+{�6�   �� *PortTextBoxEnabled+�� 5"      $��5��6   $��Адрес:��6(   �� 5#      $��6��6   $��22��6o   $	��100��6{   ��6*   �� *	Address+{�6C   �� *AddressTextBoxEnabled+6{   5$      �� 5%      $��1��6   $��Center=�6%   �� 5&      $��2��6   $��Сообщения��6$   .��5'      � .+�5(      �� .��5)      �� -   5         ��Name$ MessageBlock��5)   $   $
��Wrap=�6X   ��6:   �� *MessagesText+6X   5*      5+      5,      �� 5-      $��3��6   $��Center=�6%   � 5.      $��4��6    5/      �� 50      $
��auto �6'   �� 51      �� 52      $
��auto �6'   .+�55      �� $��0��6   $(��"Введите сообщение:��6(   $��Center=�6E   $��Center=�6b   �� 56      -   5         ��Name$ SendMessageBox��56   I   $��1��6    �� TextWrapping$
 Wrap=�6a   $��22��6u   $��Center=�6�   $	��5,2q�6�   ��6+   �� *MessageText+6�   �� 57      -   5        $ 
SendButton��57   *   $��2��6   $��Отправить��6>   $��Center=�6R   $��75��6o   $��22��6z   $��Center=�6�   ��6�   �� *SendMessageCommand+��6�   ��  �� ElementName$ SendMessageBox��	 �� Path$
	 Text�{�6  �� *SendMessageButtonEnabled+58      59      5:      5;      