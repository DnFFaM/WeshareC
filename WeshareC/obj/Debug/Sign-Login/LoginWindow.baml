   M S B A M L   `   `   `  ����C  ?WeshareC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:WeshareCWeshareC  5   	       WeshareC.LoginWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:WeshareC     5   	     � Title$  LoginWindow��5   	    � WindowStartupLocation$ CenterScreen=�6   .��5	      � ���#F3F4F6 � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��1��6   $��Center=�6"   $��Center=�6?   $
��0 40q�6Z   .+�5      �� $��Login��6   $��34(�6%   $
��Bold"�63   $��0 0 0 20q�6E   $��Center=�6W   �� 5      $
��0 10q�6    �� Orientation$ 
Horizontal=�6'   .+�5      �� $ ��\Icons\Signature-solid.png��6   $��20��6<   $��20��6G   $��0 0 5 0q�6S   �� 5      $��Username��6   $��16(�6,   5      �� 5      -   5         ��Name$ txtUsername��5      $	��300��6+   $��40��67   $	��0 5q�6C   $��14(�6P   $��1q�6^   ���#D1D5DB6r   �� 5      $
��0 10q�6   $ 
Horizontal=�6'   .+�5      �� $��\Icons\Key-solid.png��6   $��20��66   $��20��6A   $��0 0 5 0q�6M   �� 5      $��Password��6   $��16(�6,   5      G� 5      -   5         G�Name$ txtPassword��5      $	��300��6/   $��40��6;   $	��0 5q�6G   $��14(�6T   $��1q�6b   ���#D1D5DB6v   �� 5      $ 
Horizontal=�6   $��Center=�62   $
��0 20q�6O   .+�5       (� .��5!      �� -   5        $	��150��5!      $��40��6)   $��14(�65   ���#2E86C1
����������#3498DB$��1q�6�   $��5q�6�   .��5"      �� $ 
Horizontal=�6%   .+�5#      �� $-��'\Icons\arrow-Right-to-bracket-solid.png��6$   $��20��6U   $��20��6`   $	��5 0q�6l   �� 5$      $��1��6(   $��Login��68   $��14(�6E   $��Center=�6S   5%      5&      5'      (� 5(      .��5)      �� -   5        $	��150��5)      $��40��6)   $��14(�65   ���#E74C3C
����������#3498DB$��1q�6�   $��5q�6�   .��5*      �� $ 
Horizontal=�6%   .+�5+      �� $)��#\Icons\Right-from-bracket-solid.png��6$   $��20��6Q   $��20��6\   $	��5 0q�6h   �� 5,      $��1��6(   $��Go Back��68   $��14(�6G   $��Center=�6U   5-      5.      5/      50      51      52      53      