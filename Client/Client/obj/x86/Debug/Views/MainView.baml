   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:Client.ViewModelsClient.ViewModels  5   	   5clr-namespace:Client.ConvertersClient.Converters  5   	       Client.Views.MainView   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   /mvmclr-namespace:Client.ViewModels     5   	   0convclr-namespace:Client.Converters     5   	     � Title$  Client Service��5   	    � 	Resources 5      (  "System.Windows.HorizontalAlignment   CustomAlignment1  +Client.Converters.BoolToVisibilityConverter  BoolToVisibilityConverter4  .Client.Converters.NotBoolToVisibilityConverter   NotBoolToVisibilityConverter.  (Client.Converters.BoolToNotBoolConverter  BoolToNotBoolConverter,  &Client.Converters.BoolToBrushConverter  BoolToBrushConverter%  Client.ViewModels.MainViewModel  mainViewModel��%�   &	        &	      &	 0     &	 C     &	 V     &	 i      5   
   	Right=�6=     5	   
   6)     5
   
   6,     5   
   6&     5   
   6$     5   
   6   5      .��5      ]� -   5        $��dpMain��5       � DataContext# [ 6   ��. $��5q�6`   .+�5   
   ]� ��.  $	��Top=�6*   $��0,0,0,5q�6?   .+�5      ]� ��.  $��Right=�6.   $��0,0,0,5q�6E   .+�5      �� $��Delete My Account��6   $��Right=�5      $��5,0,0,0q�60    5   5   �� *MyUser+ �� UpdateSourceTrigger$ PropertyChanged=���5      ��  �� Path$ DeleteUserCommand�$ PropertyChanged=�y�5      ��  �� ElementName$ 	btnLogOut��$ 
Visibility�5   0   �� 5      -   5        $��Create Account��5      $��Right=�5      $��5,0,0,0q�60   y�5   2   �� *	IsLogin+ �� 	Converter# [ 5   A   �� 5      -   5        $��	btnLogOut��5   *   $��LogOut��6   $��Right=�5   j   $��5,0,0,0q�6�   ��5   ;   �� *LogOutCommand+$ PropertyChanged=�y�5      �� *	IsLogin+# [ 6�   �� 5      $��LogIn��6   $��Right=�5   m   $��5,0,0,0q�6�   ��5   )   �� *LogInCommand+$ PropertyChanged=�y�5      �� *	IsLogin+# [ 6�   5      G� 5      &    Client.ViewModels.PasswordHelper   Attach . $	��250��5       G� 	MaxLength$ 20��6)   $��Right=�68   $��2��6O   	   Password	 5   ;   �� *
Password+
 �� Mode$
 TwoWay=�{�5      �� *	IsLogin+# [ $ PropertyChanged=�5   O   �� 5       $��Password��6   $��Right=�6'   $��10,0,0,0q�6>   �� 5!      $	��250��5#      $��Right=�6%   $��0��6<   ��5!      �� *
Username+$ PropertyChanged=�{�5"      �� *	IsLogin+# [ $ PropertyChanged=�5#   <   �� 5$      $��Email��6   $��Right=�6$   5%      �� 5&   
   $��2q�6~   $��Bottom=�6�   $��0,5,0,0q�6�   
������� 6   �� *Logs+��6.   �� *Result+# [ ._�5'      �� $
��Wrap=�6N   ��6   �� *Text+ � ToolTip 6.   �� *Description+6N   5(      �� 5)   
   $��5��6X   {�6   �� *	IsLogin+$ PropertyChanged=�.@�5*      �� $��	Home page��6    6)   �� *MyUser+$ PropertyChanged=� �� 
IsSelected 5+      �� $
 OneWayToSource=�$ 	IsUpdated�.��5,      ]� ��. $��5q�61   .+�5-      �� $��0,0,5,0q�6!   .+�5.      ]� ��. .+�5/      �� $	��100��6$   $
��Name��60   ��6?   ��  �� Source# [  $
 OneWay=��� 50      $	��150��51   &   $��0,5,0,0q�65   $��0��6F   ��50   &   �� *
EditName+ �� ValidatesOnDataErrors .  �� ValidatesOnExceptions . $ PropertyChanged=�.��51   S   $2 ,Enter your name. This field can not be empty��54   3   .��55      56      ]� 57      ��. .+�58      �� $	��100��6$   $��Surname��60   ��6B   �� # [  $
 OneWay=��� 59      $	��150��5:   &   $��0,5,0,0q�65   $��1��6F   ��59   &   �� *EditSurname+ .  . $ PropertyChanged=�.��5:   S   $5 /Enter your surname. This field can not be empty��5=   3   .��5>      5?      ]� 5@      ��. .+�5A      �� $	��100��6$   $
��Role��60   ��6?   �� # [  $
 OneWay=��� 5B       �� 
IsReadOnly . $	��150��6�   $��0,5,0,0q�6�   ��6&   �� *Role+$
 OneWay=�$ PropertyChanged=�6�   5C      ]� 5D      ��. .+�5E      �� $	��100��6$   $��New Password��60   ��6G   �� # [  $
 OneWay=�G� 5F       . $	��150��5G   *   $��0,5,0,0q�69   $��2��6J   	 5F   K   �� *EditPassword+$
 TwoWay=� .  . $L FEnter your new password. The password must be longer than 8 characters��5J   7   5K      5L      ]� 5M      ��. .+�5N      �� $	��100��6$   $��Confirm Password��60   ��6K   �� # [  $
 OneWay=�G� 5O       . $	��150��5P   *   $��0,5,0,0q�69   $��3��6J   	 5O   K   �� *ConfirmEditPassword+$
 TwoWay=� .  .  $ Confirm your password��5S   7   5T      5U      ]� 5V      $��0,5,0,0q�6$   ��.  .+�5W      �� $
��Save��6%   $	��100��6Z   $��0,0,5,0q�6f   $��4��6w   ��64   �� *SaveMyUserCommand+6w   �� 5X      $��Default��6%   $	��100��6`   ��67   �� *DefaultMyUserCommand+6`   5Y      5Z      &   System.Windows.Controls.DataGrid  5[         AutoGenerateColumns .     
IsReadOnly . ��6   �� *UserHistory+   Columns 5\      0	  *System.Windows.Controls.DataGridTextColumn	  5]      ,
  &System.Windows.Controls.DataGridColumn 
  Header$ Document��6K    
  
IsReadOnly .  
  MinWidth$	 250Q�6o   1  +System.Windows.Controls.DataGridBoundColumn   Binding 61   �� *Title+6o   	  5^      $
 Date��6J    
  CanUserResize .   .  
  Width5  /System.Windows.Controls.DataGridLengthConverter$	 100 6�    61   �� *Date+6�   5`      5a      5b      �� 5c      $��	Documents��6    6)   �� *DocumentCollection+ 5d      �� $
 OneWayToSource=�$ 	IsUpdated�.��5e      ]� ��. $��5q�61   .+�5f      �� $	��Top=�6!   $��0,0,0,5q�66   .+�5g      ]� ��. $��0,0,0,5q�69   .+�5h      �� $��Title��6$   �� 5i      $	��150��6e   $��Right=�6t   $��2��6�   ��6&   �� *
FindTags+$ PropertyChanged=�6�   �� 5j      $
��Tags��6$   $��Right=�63   �� 5k      $	��100��5n   '   $��Right=�63   $��0,0,5,0q�6J   $��1��6[   ��5k   '   �� # [ $ 
Publishers�$ PropertyChanged=� �� SelectedValue 5l   '   �� *FindPublisher+$ PropertyChanged=� �� IsDropDownOpen 5m   '   �� # [ $ 	IsUpdated�$
 OneWayToSource=�5n   [   �� 5q      $��	Publisher��6$   $��Right=�68   �� 5r      $��0,0,5,0q�6f   $��0��6w   ��6&   �� *	FindTitle+$ PropertyChanged=�6w   5s      ]� 5t      ��.  .+�5u      �� $��65��6m   $��Right=�6x   $��3��6�   $��5,0,0,0q�6�   ��6%   �� *FindDocCommand+$ PropertyChanged=�.��5v   "   ]� .+�5w   &   �� $��16��6,   $��/Resources/Search.png��68   �� 5x   &   $��Search��60   5y   #   5z      �� 5{      $&  clear all field add result table��6k   $��65��6�   $��Right=�6�   ��6%   �� *ClearCommand+$ PropertyChanged=�.��5|   "   ]� .+�5}   &   �� $��16��6,   $��/Resources/edit-clear.png��68   �� 5~   &   $��Clear��60   5   #   5�      5�      5�      �� 5�      $	��300��5�   ;   $	��150��6K   $��Right=�6Z   $��5,0,0,0q�6q    5�      �� $ dgDocuments��$ SelectedValue�$ PropertyChanged=���5�      �� *ScreenImage+6q   ]� 5�      $��Bottom=�6    ��.  $��0,5,0,0q�6N   .+�5�      �� $��
Delete All��6!   $��75��5�   |   $��Right=�6�   $��5,0,0,0q�6�   ��5�   6   �� *DeleteAllDocCommand+$ PropertyChanged=�y�5�   !   �� *
IsReadOnly+# [ 6�   �� 5�      $��Save All��6!   $��75��5�   |   $��Right=�6�   $��5,0,0,0q�6�   ��5�   4   �� *SaveAllDocCommand+$ PropertyChanged=�y�5�   !   �� *
IsReadOnly+# [ 6�   �� 5�      $��Download All��6!   $��80��5�   |   $��Right=�6�   $��5,0,0,0q�6�   ��5�   8   �� *DownloadAllDocCommand+$ PropertyChanged=�y�5�   !   �� *
IsReadOnly+# [ 6�   �� 5�      $��
Upload All��6!   $��75��5�   |   $��Right=�6�   $��5,0,0,0q�6�   ��5�   6   �� *UploadAllDocCommand+$ PropertyChanged=�y�5�   !   �� *
IsReadOnly+# [ 6�   �� 5�      $ Add documents��6!   $��75��5�   |   $��Right=�6�   $��5,0,0,0q�6�   ��5�   9   �� *AddDocCommand+$ PropertyChanged=�y�5�   !   �� *
IsReadOnly+# [ .��5�      ]� .+�5�   "   �� $��16��6(   $��/Resources/Add file.png��64   �� 5�   "   $	��Add��6,   5�      5�      5�        5�      -   5        $��dgDocuments��5�         CanUserAddRows .   .  ��62   �� *	Documents+$ PropertyChanged=� 5�      �� *
IsReadOnly+$ PropertyChanged=� 5�      4  .System.Windows.Controls.DataGridTemplateColumn  5�       .    CellTemplate 5�   "   �� 5�   &   .R�5�   *   �� $��16��60   $��/Resources/document.png��6<   5�   '   5�      	  5�      $ Title��62   $	 150Q�6�    6A   �� *Title+$ PropertyChanged=�6�   	  5�      $ Description��61   $	 150Q�6�    6F   �� *Description+$ PropertyChanged=�6�     5�      $ Language��65   $	 100 6H    .   5�   "   �� 5�   &   .R�5�   *   �� ��63   �� # [ $ 	Languages�$ PropertyChanged=� 5�   3   �� $ Language�$ PropertyChanged=� �� 
IsReadOnly 5�   3   �� *
IsReadOnly+$ PropertyChanged=�5�   '   5�        5�      $ 	Publisher��65   $	 100 6I    .   5�   "   �� 5�   &   .R�5�   *   ��  �� 
IsEditable . ��63   �� # [ $ 
Publishers�$ PropertyChanged=� 5�   3   �� # [ $ 	IsUpdated�$
 OneWayToSource=�  �� Text  5�   3   �� *	Publisher+$ PropertyChanged=� 5�   3   �� *
IsReadOnly+$ PropertyChanged=�6z   5�   '   5�      	  5�      $
 Tags��61   $	 100Q�6?    6N   �� *Tags+$ PropertyChanged=�  5�      $ Path to screen��65   $	 120Q�6M   ! 
  
Visibility! 6\   �� *
IsReadOnly+# [  5�   "   �� 5�   &   .R�5�   *   ]� ��. .+�5�   .   �� $	��...��6�   $��Right=�6�   ��65   �� *PathToScreenCommand+$ PropertyChanged=�6�   �� 5�   .    . ��66   �� *PathToScreen+$ PropertyChanged=�6y   5�   +   5�   '   5�        5�      $ Path to doc��66   $	 150Q�6K    5�   "   �� 5�   &   .R�5�   *   ]� ��. .+�5�   .   �� $	��...��6   $��Right=�6�   ��65   �� *PathToDocCommand+$ PropertyChanged=�y�5�   5   �� *
IsReadOnly+# [ �� 5�   .    . ��66   �� *	PathToDoc+$ PropertyChanged=�6u   5�   +   5�   '   5�        5�      $ 	Operation��65    5�   "   �� 5�   &   .R�5�   *   ]� ��.  $��1q�6J   .+�5�   .   �� $ Download file��6�   $��1q�6�   ��65   �� *DownloadDocCommand+$ PropertyChanged=�.��5�   2   �� $��16��68   $"��/Resources/Download file.png��6D   5�   /   �� 5�   .   $ Upload file��5�      $��1q�6�   y�5�   5   �� *
IsReadOnly+# [ ��5�   5   �� *UploadDocCommand+$ PropertyChanged=�.��5�   2   �� $��16��68   $ ��/Resources/Upload file.png��6D   5�   /   �� 5�   .   $ 	View file��6}   $��1q�6�   ��65   �� *ViewDocCommand+$ PropertyChanged=�.��5�   2   �� $��16��68   $��/Resources/View.png��6D   5�   /   �� 5�   .   $ Save document info��5�   }   $��1q�6�   y�5�   5   �� *
IsReadOnly+# [ ��5�   5   �� *SaveDocCommand+$ PropertyChanged=�.��5�   2   �� $��16��68   $��/Resources/Save.png��6D   5�   /   �� 5�   .   $ Delete file��5�      $��1q�6�   y�5�   5   �� *
IsReadOnly+# [ ��5�   5   �� *DeleteDocCommand+$ PropertyChanged=�.��5�   2   �� $��16��68   $ ��/Resources/Delete file.png��6D   5�   /   5�   +   5�   '   5�      5�      5�      5�      �� 5�      $��Users��6    6%   �� *UserCollection+ 6L   �� *	IsUpdated+.��5�      ]� ��. $��5q�61   .+�5�      �� $	��Top=�6!   $��0,0,0,5q�66   .+�5�      ]� ��. $��0,0,0,5q�69   .+�5�      �� $��Email��6$   �� 5�      $	��100��5�   '   $��Right=�63   $��3��6J   ��5�   '   �� *Roles+# [ $ PropertyChanged=� 5�   '   �� *
FindRole+$ PropertyChanged=�5�   J   �� 5�      $
��Role��6$   $��Right=�63   �� 5�      $	��100��6h   $��Right=�6t   $��0,0,10,0q�6�   $��2��6�   ��6&   �� *FindSurname+$ PropertyChanged=�6�   �� 5�      $��Surname��6$   $��Right=�66   �� 5�      $	��100��6e   $��Right=�6q   $��0,0,10,0q�6�   $��1��6�   ��6&   �� *
FindName+$ PropertyChanged=�6�   �� 5�      $
��Name��6$   $��Right=�63   �� 5�      $��0,0,10,0q�6f   $��0��6x   ��6&   �� *	FindEmail+$ PropertyChanged=�6x   5�      ]� 5�      ��.  .+�5�      �� $��Right=�6n   $��5,0,0,0q�6�   ��6%   �� *FindUserCommand+$ PropertyChanged=�.��5�   "   ]� .+�5�   &   �� $��16��6,   $��/Resources/Search.png��68   �� 5�   &   $��Search��60   5�   #   5      �� 5     $��Clear��6n   $ 
Clear find��6~   $��Right=�6�   ��6%   �� *ClearUserCommand+$ PropertyChanged=�6�   5     5     ]� 5     $��Right=�6�   y�6    �� *
IsReadOnly+# [ $ PropertyChanged=�.+�5        .   .  5     �� $ dgUsers��$ SelectedValue�$ PropertyChanged=���5     �� *UserHistory+$ PropertyChanged=� 5	       5
      .  5  "   �� 5  &   .R�5  *   �� $��16��60   $��/Resources/document.png��6<   5  '   5     	  5     $ Document��61   $	 250Q�6�    6C   �� *Title+$ PropertyChanged=�6�   	  5     $
 Date��61    .  $	 100 6�    6?   �� *Date+$ PropertyChanged=�6�   5     5       5     -   5        $��dgUsers��5     "   CanUserDeleteRows" .   .  $��0,0,5,0q�5  U    .  ��5  .   �� *Users+$ PropertyChanged=� 5     �� *
IsReadOnly+$ PropertyChanged=� 5       5      .  5  "   �� 5  &   .R�5  *   �� $��16��60   $%��/Resources/user_information.png��6<   5  '   5      	  5!     $ Email��61    . $	 150Q�6l    6@   �� *Email+6l   	  5"     $
 Name��61    . $	 100Q�6j    6?   �� *Name+6j   	  5#     $ Surname��61    . $	 100Q�6p    6B   �� *	Surname+6p     5$     $
 Role��65   $	 100Q�6C    5%  "   �� 5&  &   .R�5'  *   ��  .  ��63   �� # [ $ Roles� .  . $ PropertyChanged=� 5)  3   �� *
EditRole+$ PropertyChanged=� 5*  3   �� *
IsReadOnly+$ PropertyChanged=�5+  3   5,  '   5.       5/      50  "   �� 51  &   .R�52  *   ]� .+�53  .   �� $ delete user��55  5   $��5,0,0,0q�6K   ��53  5   �� *DeleteUserCommand+$ PropertyChanged=�y�54  5   �� *
IsReadOnly+# [ $ PropertyChanged=�.��56  2   �� $��16��68   $ ��/Resources/user_delete.png��6D   57  /   �� 58  .   $ 	Save user��5:  5   ��58  5   �� *SaveUserCommand+$ PropertyChanged=�y�59  4   �� *
IsReadOnly+# [ $ PropertyChanged=�.��5;  2   �� $��16��68   $��/Resources/user_edit.png��6D   5<  /   5=  +   5>  '   5@     5B     5D     5E     5F     5G     5H     