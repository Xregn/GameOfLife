   M S B A M L   `   `   `  ����E  AGameOfLife, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:GameOfLife
GameOfLife  5   	       GameOfLofeMVVM.MainView   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:GameOfLife     5   	     � Title$  
GameOfLife��5   	   $	��450��6   $	��500��6)    � DataContext 5
        GameOfLife.MainViewModel  5   
    � 	Resources 5       �� 
TargetType �� Property �� Value��%v   '��       �� 5   
   " ��6   .�5      ��  � ��$ 3q�6'   �� 5       � ��$	 100��6)   5      5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      �� 5      $
��Auto �6   .+�5   
   o� .@�5      m� $��Файл��6   .@�5      m� -   5        $��NewEmptyField��5   /   $��Новое поле��6    m� Command 6D   �� *NewEmptyFieldCommand+m� 5       $��Растановка��6   .@�5!      m� -   5        $��	OpenField��5!   0   $��Открыть��6    6B   �� *OpenFieldCommand+m� 5"      -   5        $��	SaveField��5"   2   $��Сохранить��6    6D   �� *SaveFieldCommand+m� 5#      -   5        $��DelField��5#   /   $��Уалить��6    6@   �� *DelFieldCommand+m� 5$      -   5        $��LoadRandField��5$   <   $+��%Загрузить случайную��6    6R   �� *LoadRandFieldCommand+m� 5%      -   5        $��	FieldRand��5%   ;   $)��#Заполнить случайно��6    6M   �� *FieldRandCommand+5&      m� 5'      $��Сохранения��6   .@�5(      m� -   5        $��OpenSave��5(   0   $��Открыть��6    6A   �� *OpenSaveCommand+m� 5)      -   5        $��SaveSave��5)   2   $��Сохранить��6    6C   �� *SaveSaveCommand+m� 5*      -	   5        $��DelSave��5*   /   $��Уалить��6    6?   �� *DelSaveCommand+5+      m� 5,      -
   5        $	��log��5,   5   $%��Просмотр истории��6    6A   �� *
LogCommand+�� 5-      m� 5.      -   5        $
��Exit��5.   *   $��
Выход��6    67   �� *ExitCommand+5/      m� 50      $��Игра��6   .@�51      m� ��6   �� *StartStopTite+ �� UpdateSourceTrigger$ PropertyChanged=� 6a   �� *StartStopCommand+m� 52      $��Тип вселенной��6   .@�53      	� -   5        $	��RB1��53   9   $��Ограниченная��6"   69   	� 54      -   5        $	��RB2��54   6   $��Замкнутая��6"   	 d� 	IsChecked	 6A   �� *
closedWord+55      57      58      �� 5:   
   $��Visible=�6   $��Visible=�6?   $��1��6e   .��5;      � $��2,2,2,2q�6   .+�5<      �� -   5        $��ContentControlPanel��5<   !   $��Center=�6<   $��Center=�6Y   $��Center=�6t   $��Center=�6�   ��6�   �� *contentControl+
 �� Mode$
 TwoWay=�$ PropertyChanged=�5=      5>      �� 5@   
   $
��Left=�6    �� Orientation$ 
Horizontal=�60   $��2��6I   .+�5A      �� -   5        $��	StartStop��5A      ��6&   �� *StartStopTite+$
 TwoWay=�$ PropertyChanged=���6{   �� *StartStopCommand+�� 5B      -   5        $��textGen��5B      $��Center=�6'   $	��3,0q�6B   ��6O   �� *TextGeneration+$
 TwoWay=�$ PropertyChanged=�Поколение: 05D      5E      5G      5H      