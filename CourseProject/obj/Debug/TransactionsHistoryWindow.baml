   M S B A M L   `   `   `  ����H  DCourseProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:CourseProjectCourseProject  5   	   C&clr-namespace:CourseProject.PropertiesCourseProject.Properties  5   	   -    'CourseProject.TransactionsHistoryWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:CourseProject     5   	   =
Properties&clr-namespace:CourseProject.Properties     5   	     � Title$  TransactionsHistoryWindow��5	   	   $	��600��6+   $	��900��68    � 
ResizeMode$ NoResize=�6D    � WindowStartupLocation$ CenterScreen=�6Z    � 	Resources 5
       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $ Resources/Styles.xaml?�6%   %    5      '  !System.Windows.Shell.WindowChrome   WindowChrome 5        5   
      CaptionHeight$ 5Q�6      UseAeroCaptionButtons .  6)   .��5      ��    BG_Style#��[  6   .+�5   
   �� -   5        	 ��Name$	 TransactionsList��5      $	��550��6-   $	��880��6:   
 �� Left$
 10��6F   
 �� Top$ 24��6W   $��19(�6g   
�������$
��Left=�6�   $	��Top=�6�   
������  �� View 5      �� 5      .P�5      �� (   "CourseProject.Properties.Resources   transactiontype#��Z 6P    �� Width$	 155��6�    �� DisplayMemberBinding 6%   ��  �� Path$
 Type�6�   �� 5         destination#��Z 6W   $	 185��6�    6%   �� $ Destination�6�   �� 5         date#��Z 6\   $	 185��6�    6%   �� $ Transaction_Date�6�   �� 5         number#��Z 6R   $	 175��6�    6%   �� $ Number�6�   �� 5      
   sum#��Z 6]   $	 170��6�    6%   �� $ Transaction_Value�6�   5      ��5      �� 5        �� 
TargetType" ��6   .�5!      -   5        �� 5#       �� Property � �� �� Value
 ���� 63    �� Triggers 5$      P� 5%       P� Property �@J�IsMouseOver P� Value . . �5&      ��  � ��
 �����6;   �� 5'       � ��
 �����6;   5(      P� 5)       �@J�	IsFocused . . �5*      ��  � ��
 �����6;   �� 5+       � ��
 �����6;   5,      P� 5-       �@��
IsSelected . . �5.      ��  � ��
 �����6;   �� 5/       � ��
 �����6;   50      52      54      �� 55   
   -   5         ��Name$ CloseBtn��55      $��X��6#   $��20��6/   $��20��6;   $	
 880��6F   
������ 
�������$
��Left=�6�   $��Center=�6�   56      57      