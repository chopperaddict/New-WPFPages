using System;
using System .Collections .Generic;
using System .Linq;
using System .Text;
using System .Threading .Tasks;
using System .Windows;
using System .Windows .Media;

namespace WPFPages .Views
{
      public class MenuAttachedProperties : DependencyObject
      {

            #region MenuBackground
            public static readonly DependencyProperty MenuBackgroundProperty =
                      DependencyProperty.RegisterAttached("MenuBackground", typeof(Brush), typeof(MenuAttachedProperties), 
                            new PropertyMetadata(Brushes.AliceBlue));
            public static Brush GetMenuBackground ( DependencyObject obj )
            {
                  return ( Brush ) obj .GetValue ( MenuBackgroundProperty );
            }
            public static void SetMenuBackground ( DependencyObject obj , Brush value )
            {
                  obj .SetValue ( MenuBackgroundProperty , value );
            }
            #endregion MenuBackground

            #region MenuItemBackground 
            public static readonly DependencyProperty MenuItemBackgroundProperty =
                  DependencyProperty .RegisterAttached("MenuItemBackground", typeof(Brush), typeof(MenuAttachedProperties), 
                        new PropertyMetadata(Brushes.AliceBlue));
            public static Brush MenuItemBackground ( DependencyObject obj )
            {
                  return ( Brush ) obj .GetValue ( MenuItemBackgroundProperty );
            }
            public static void SetMenuItemBackground( DependencyObject obj , Brush value )
            {
                  obj .SetValue ( MenuItemBackgroundProperty , value );
            }
            #endregion MenuItemBackground 

            #region MenuItemBorderColor
            public static readonly DependencyProperty MenuItemBorderColorProperty =
                  DependencyProperty.RegisterAttached("MenuItemBorderColor", typeof(Brush), typeof(MenuAttachedProperties), 
                        new PropertyMetadata((Brush)Brushes.AliceBlue));
            public static Brush GetMenuItemBorderColor ( DependencyObject obj )
            {
                  return ( Brush ) obj .GetValue ( MenuItemBorderColorProperty );
            }
            public static void SetMenuItemBorderColor ( DependencyObject obj , Brush value )
            {
                  obj .SetValue ( MenuItemBorderColorProperty , value );
            }

            #endregion MenuItemBorderColor

            #region MenuItemBorderThicknesss
            public static readonly DependencyProperty MenuItemBorderThicknessProperty =
                  DependencyProperty.RegisterAttached("MenuItemBorderThickness", typeof(Thickness), typeof(MenuAttachedProperties), 
                        new PropertyMetadata((Thickness)default));
            public static Thickness GetMenuItemBorderThickness ( DependencyObject obj )
            {
                  return ( Thickness ) obj .GetValue ( MenuItemBorderThicknessProperty );
            }

            public static void SetMenuItemBorderThickness ( DependencyObject obj , Thickness value )
            {
                  obj .SetValue ( MenuItemBorderThicknessProperty , value );
            }

		#endregion MenuItemBorderThicknesss

            #region MenuFontSize
            public static readonly DependencyProperty MenuFontSizeProperty =
                      DependencyProperty.RegisterAttached("MenuFontSize", typeof(double), typeof(MenuAttachedProperties), 
                            new PropertyMetadata((double)12));
            public static double GetMenuFontSize ( DependencyObject obj )
            {
                  return ( double ) obj .GetValue ( MenuFontSizeProperty );
            }
            public static void SetMenuFontSize ( DependencyObject obj , double value )
            {
                  obj .SetValue ( MenuFontSizeProperty , value );
            }

            #endregion MenuFontSize

            #region MenuDropdownWidth 
            public static double GetMenuDropdownWidth ( DependencyObject obj )
		{
			return ( double ) obj . GetValue ( MenuDropdownWidthProperty );
		}
		public static void SetMenuDropdownWidth ( DependencyObject obj , double value )
		{
			obj . SetValue ( MenuDropdownWidthProperty , value );
		}

		// Using a DependencyProperty as the backing store for MenuDropdownWidth.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MenuDropdownWidthProperty =
                  DependencyProperty.RegisterAttached("MenuDropdownWidth", typeof(double), typeof(MenuAttachedProperties), new PropertyMetadata((double)200));
            #endregion MenuDropdownWidth 

            #region MenuFontWeight
            public static readonly DependencyProperty MenuFontWeightProperty =
                  DependencyProperty.RegisterAttached("MenuFontWeight", typeof(string), typeof(MenuAttachedProperties), 
                        new PropertyMetadata("Normal"));
            public static string  GetMenuFontWeight ( DependencyObject obj )
            {
                  return  (string) obj .GetValue ( MenuFontWeightProperty );
            }
            public static void SetMenuFontWeight ( DependencyObject obj , string  value )
            {
                  obj .SetValue ( MenuFontWeightProperty , value );
            }

            #endregion MenuFontWeight

            #region MenuItemForeground 
            public static readonly DependencyProperty MenuItemForegroundProperty =
                  DependencyProperty.RegisterAttached("MenuItemForeground", typeof(Brush), typeof(MenuAttachedProperties), 
                        new PropertyMetadata((Brush)Brushes.AliceBlue));
            public static Brush GetMenuItemForeground ( DependencyObject obj )
            {
                  return ( Brush ) obj .GetValue ( MenuItemForegroundProperty );
            }
            public static void SetMenuItemForeground ( DependencyObject obj , Brush value )
            {
                  obj .SetValue ( MenuItemForegroundProperty , value );
                  Console .WriteLine ( $"MenuItemForeground set to  {value}" );
            }

            #endregion MenuItemForeground 

            #region MenuItemHeight
            public static readonly DependencyProperty MenuItemHeightProperty =
                      DependencyProperty.RegisterAttached("MenuItemHeight", typeof(double), typeof(MenuAttachedProperties), 
                            new PropertyMetadata((double)20));
            public static double GetMenuItemHeight ( DependencyObject obj )
            {
                  return ( double ) obj .GetValue ( MenuItemHeightProperty );
            }
            public static void SetMenuItemHeight ( DependencyObject obj , double value )
            {
                  obj .SetValue ( MenuItemHeightProperty , value );
            }

            #endregion MenuItemHeight

            #region MenuItemMargin
            public static readonly DependencyProperty MenuItemMarginProperty =
                      DependencyProperty.RegisterAttached("MenuItemMargin", typeof(double), typeof(MenuAttachedProperties), 
                            new PropertyMetadata((double)0));
            public static double GetMenuItemMargin ( DependencyObject obj )
            {
                  return ( double ) obj .GetValue ( MenuItemMarginProperty );
            }
            public static void SetMenuItemMargin ( DependencyObject obj , double value )
            {
                  obj .SetValue ( MenuItemMarginProperty , value );
            }

            #endregion MenuItemMargin

            #region MenuItemSelectedBackground 
            public static readonly DependencyProperty MenuItemSelectedBackgroundProperty =
                  DependencyProperty.RegisterAttached("MenuItemSelectedBackground", typeof(Brush ), typeof(MenuAttachedProperties), 
                        new PropertyMetadata(Brushes.Yellow));
            public static Brush GetMenuItemSelectedBackground ( DependencyObject obj )
            {
                  return ( Brush ) obj .GetValue ( MenuItemSelectedBackgroundProperty );
            }
            public static void SetMenuItemSelectedBackground ( DependencyObject obj , Brush value )
            {
                  obj .SetValue ( MenuItemSelectedBackgroundProperty , value );
            }

            #endregion MenuItemSelectedBackground 

            #region MenuItemSelectedForeground
            public static readonly DependencyProperty MenuItemSelectedForegroundProperty =
                  DependencyProperty.RegisterAttached("MenuItemSelectedForeground", typeof(Brush), typeof(MenuAttachedProperties), 
                        new PropertyMetadata(Brushes.AliceBlue));
            public static Brush GetMenuItemSelectedForeground ( DependencyObject obj )
            {
                  return ( Brush ) obj .GetValue ( MenuItemSelectedForegroundProperty );
            }
            public static void SetMenuItemSelectedForeground ( DependencyObject obj , Brush value )
            {
                  obj .SetValue ( MenuItemSelectedForegroundProperty , value );
            }

            #endregion MenuItemSelectedForeground
            
      }
}
