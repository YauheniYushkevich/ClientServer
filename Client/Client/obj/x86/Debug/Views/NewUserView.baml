   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:Client.ViewModelsClient.ViewModels  5   	       Client.Views.NewUserView   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   /mvmclr-namespace:Client.ViewModels     5   	     � Title$  Create new user��5   	   $	��200��6!   $	��300��6.    � 
ResizeMode$ NoResize=�6:    � 	Resources 5      (  "System.Windows.HorizontalAlignment   CustomAlignment��%'   &	         5   
   	Right=�6=   5      .��5	      �� -   5        $
��main��5	      .+�5
   
   �  � DataContext 6   �� *	NewUser+ �� UpdateSourceTrigger$ PropertyChanged=� � ColumnDefinitions 5   
   �� 5      $
��auto �6   �� 5      $	��100��6    � RowDefinitions 5   
   �� 5      $��25 �6   �� 5      $��25 �6   �� 5      $��25 �6   �� 5      $��25 �6   �� 5      $��25 �6   .+�5      �� $��Email��6   $��0��6$   $��0��64   ��6A   ��  �� Source# [   �� Mode$ OneWay=��� 5      $��1��5      $��0��6&   $��2q�63   ��5      �� *Email+	 �� ValidatesOnDataErrors	 . 
 �� ValidatesOnExceptions
 . $ PropertyChanged=�5   3   �� 5      $
��Name��6   $��0��6#   $��1��63   ��6@   �� # [  $ OneWay=��� 5      $��1��5      $��1��6&   $��2q�63   ��5      �� *Name+	 . 
 . $ PropertyChanged=�5   3   �� 5      $��Surname��6   $��0��6&   $��2��66   ��6C   �� # [  $ OneWay=��� 5      $��1��5      $��2��6&   $��2q�63   ��5      �� *	Surname+	 . 
 . $ PropertyChanged=�5   3   �� 5      $��Password��6   $��0��6'   $��3��67   ��6D   �� # [  $ OneWay=�G� 5       &    Client.ViewModels.PasswordHelper   Attach . $��1��5"      $��3��6*   $��2q�67      Password 5!      �� *
Password+	 . 
 . $ TwoWay=�5"   7   �� 5#      $��Confirm password��6   $��0��6/   $��4��6?   ��6L   �� # [  $ OneWay=�G� 5$       . $��1��5&      $��4��6*   $��2q�67    5%      �� *ConfirmPassword+	 . 
 . $ TwoWay=�5&   7   5'      ]� 5(   
   ��.  $��Center=�6o    6   �� *	NewUser+$ PropertyChanged=�.+�5)      �� -   5        $��5q�5)   q   ��6   �� *CreateCommand+$ PropertyChanged=�.��6|   Ok6�   �� 5*      -   5        $��5q�5*   *   .��65   Cancel6>   5+      5,      5-      