   M S B A M L   `   `   `  ����H  DCourseProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:CourseProjectCourseProject  5   	   C&clr-namespace:CourseProject.PropertiesCourseProject.Properties  5   	   "    CourseProject.RecoveryWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:CourseProject     5   	   ;language&clr-namespace:CourseProject.Properties     5   	     � WindowStartupLocation$  CenterScreen=�5	   	    � Title$ RecoveryWindow��5
   	   $	��450��6    $	��400��6-    � 
ResizeMode$ NoResize=�69   '  !System.Windows.Shell.WindowChrome   WindowChrome 5        5   
      CaptionHeight$ 5Q�6      UseAeroCaptionButtons .  6)    � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $ Resources/Styles.xaml?�6%   %    5      .��5      �� -   5        	 ��Name$	 
MainCanvas��5         BG_Style#��[  6!   .+�5   
   �� -   5        $	 Step1Canvas��5      $	��400��6&   $	��120��62   $��0, 20, 0, 0q�6?   .+�5      �� -   5        
 ��Name$
 MailTextBox��5        TB_Style#��[ 6+   $�� ��6M   
 �� Top$ 46��6U   $	��250��6e    �� Left$	 104��6q   $
��Left=�6�   $��Center=�6�   �� 5      -   5         ��Name$ ConfirmBtn1��5      (   "CourseProject.Properties.Resources   confirm#��Z 6*     BtnConfirm_Style#��[ 6Z   $	 155��6�   $ 75��6�   $��Center=�6�   $	��Top=�6�   6�   �� 5      -   5         ��Name$ 	MailLable��5         	maillable#��Z 6'     Label_Style#��[ 6Y   $ 10��6~   $ 38��6�   $
��Left=�6�   $��Center=�6�   $��94��6�   5      �� 5   
   -   5        $	 Step2Canvas��5      $	��400��6&   $��90��62   $��0, 140, 0, 0q�6>   $y�Hidden=�6T   .+�5      �� -   5        $
 AccessCodeTextBox��5      #��[ 61   $�� ��6S   $ 12��6[   $	��250��6k   $	 102��6w   $
��Left=�6�   $	��Top=�6�   �� 5      -   5        $ ConfirmBtn2��5      #��Z 6*   #��[ 6Z   $	 153��6�   $ 41��6�   $
��Left=�6�   $	��Top=�6�   6�   �� 5      -	   5        $ AccessCodeLable��5         code#��Z 6-   #��[ 6Z   $ 10��6   $ 10��6�   $
��Left=�6�   $��Center=�6�   $��92��6�   5       �� 5!   
   -
   5        $	 Step3Canvas��5!      $	��400��6&   $	��150��62   $��0, 230, 0, 0q�6?   $y�Hidden=�6U   .+�5"      �� -   5        $ UserLoginLable��5"      $��
User Login��6,   #��[ 6A   $	 152��6f   $ 10��6x   $��Center=�6�   $��Center=�6�   G� 5#      -   5         G�Name$ PasswordTextBox��5#        PB_Style#��[ 63   $	��250��6U   $��24��6a   $��14(�6m   $��Center=�6{   $	 113��6�   $ 48��6�   $
��Left=�6�   $	��Top=�6�   �� 5$      -   5        $ PasswordLable��5$         password#��Z 6+   #��[ 6\   $ 40��6�   $
��Left=�6�   $��Center=�6�   $ 10��6�   $	��103��6�   �� 5%      -   5        $ RepitPasswordLable��5%         
repitlable#��Z 60   #��[ 6c   $ 10��6�   $ 68��6�   $
��Left=�6�   $��Center=�6�   $	��102��6�   G� 5&      -   5        $ RepitPasswordTextBox��5&      #��[ 68   $	��250��6Z   $��24��6f   $��14(�6r   $��Center=�6�   $	 113��6�   $ 76��6�   $
��Left=�6�   $	��Top=�6�   �� 5'      -   5        $ ConfirmBtn3��5'      #��Z 6*   #��[ 6Z   $	 155��6�   $	 105��6�   $
��Left=�6�   $	��Top=�6�   6�   5)      �� 5*   
   -   5        $ RecoveryLable��5*         recovery#��Z 6'   #��[ 6X   $��25(�6}   $
��Left=�6�   $��Center=�6�   $��Center=�6�   $ 20��6�   $	��400��6�   o� 5+   
   $��20��6   $	��400��6   
������ $��Right=�6@   .@�5,      m� -   5        $	��_Ru��5,      
�������$��20��67   $��25��6C   6N   m� 5-      -   5        $	��_En��5-      
�������$��20��67   $��25��6C   6N   m� 5.      -   5         m�Name$ CloseBtn��5.      $��_X��6)   
�������$��20��6H   $��20��6T   $��Right=�6_   $��	330,0,0,0q�6{   6�   5/      50      51      