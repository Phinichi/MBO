   M S B A M L   `   `   `  ���� 	  Hanoi    Hanoi.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  Türme von Hanoi��$	��459��$��798.776������  �� Color$ White��.��� -    �Name$ 	hanoiGrid��$
��Left=�$	��644�� � RowDefinitions �� $��7* ��� $	��10* ��� $
��121* �.+��� -   $��Titel��$��Center=�$��Center=�$
��Bold"�$��14(�$��Türme von Hanoi��$��268,-2,243,23q�$��2��$��30��$	��133���� -    ��Name$ Canvas1�� J� 	AllowDrop . ���	#FFF0F0F0$��Bottom=�$��10,0,434,73q�$	��300��$	��200��$��2���� -   $ Canvas2�� . ���	#FFF0F0F0$��Bottom=�$��222,0,222,73q�$	��300��$	��200��$��2���� -   $ Canvas3�� . ���	#FFF0F0F0$��Bottom=�$��434,0,10,73q�$	��300��$	��200��$��2���� -    ��Name$ DiscAmountSlider��$
��Left=�$��327,348,0,0q�$	��Top=�$	��126�� � Minimum$ 3Q� � Value$ 3Q�	 � SmallChange$	 1Q�
 � Cursor$
  ��$��2�� �� AutoToolTipPlacement$ TopLeft=� �� TickPlacement$ TopLeft=�$��22���� $��Anzahl Scheiben:��$
��Left=�$��227,344,0,0q�$	��Top=�$��2��$��26��$	��100���� -    ��Name$ ButtonReset��$��Neustart��{�. $y�Visible=�$��10,342,537,4q�$��2���� -	   $ ButtonSolve��$��Lösen��{�. $y�Visible=�$��113,342,434,4q�$��2���� -
   $��Legend��$��Center=�$��Center=�$����Klick: Canvas1, 2, 3

Gesten: R. Maustaste 
+ Symbol:  1, 2,  3

Sprache: (Nummer) 1, 2, 3,
Löse, Neustart,

Multipel: 
Schließe, / + dies, \
Bewege + 1, 2, 3 ,dies*
+ 1, 2, 3, dorthin*

* dies, dorthin: Mouseover��$��652,2,-140,199q�$	��132��$	��206�� J� RenderTransformOrigin$ 0.5,0.5&�$
��10.5(�$��1��$��2��z�V� .~��� ��  �� AngleX$ 0.477Q��� T�  T� X$ 0.492Q��� -   $��	LastInput��$��Center=�$��Center=�$��
Input Log:��$��685,170,-119,181q�$��78��$��2��$��24���� -   $��CanvasNumbers��$��Center=�$��Center=�$��Canvas1��$��
10,4,434,2q�$	��200��$��1��$��24���� -   $��InputFeedbackOld��$��9(�$
��Left=�$	��Top=�$�� ��$��652,227,-136,0q�$��2��$ 5.026,0.594&�$	��143��$	��128���� -   $��InputFeedbackNew��$��9(�$
��Left=�$	��Top=�$�� ��$��652,199,-136,0q�$��2��$ 5.026,0.594&�$��23��$	��128���� -    ��Name$ CanvasNumbers_Copy��$��Center=�$��Center=�$��Canvas2��$��222,4,222,2q�$	��200��$��1��$��24���� -   $ CanvasNumbers_Copy1��$��Center=�$��Center=�$��Canvas3��$��
432,4,12,2q�$	��200��$��1��$��24���� -   $ Messages��$��Center=�$��Center=�$�� ��$��10,310,12,37q�$	��622��$��2��$��29���� -   $ LastInput_Copy��$��Center=�$��Center=�$��Eingabemöglichkeiten��$��652,1,-136,28q�$	��128��$��24��$��2��