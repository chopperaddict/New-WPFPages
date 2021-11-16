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
            // Using a DependencyProperty as the backing store for MenuBackground.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty MenuBackgroundProperty =
                      DependencyProperty.RegisterAttached("MenuBackground", typeof(Brush), typeof(MenuAttachedProperties), new PropertyMetadata((Brush)default));

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
            public static Brush MenuItemBackground ( DependencyObject obj )
            {
                  return ( Brush ) obj .GetValue ( MenuItemBackgroundProperty );
            }
            public static void SetMenuItemBackground( DependencyObject obj , Brush value )
            {
                  obj .SetValue ( MenuItemBackgroundProperty , value );
                  Console .WriteLine ($"MenuItemBackground set to  {value}");
            }
            public static readonly DependencyProperty MenuItemBackgroundProperty =
                  DependencyProperty .RegisterAttached("MenuItemBackground", typeof(Brush), typeof(MenuAttachedProperties), new PropertyMetadata((Brush)default));
            #endregion MenuItemBackground 

            #region MenuItemBorderColor
            public static Brush GetMenuItemBorderColor ( DependencyObject obj )
            {
                  return ( Brush ) obj .GetValue ( MenuItemBorderColorProperty );
            }

            public static void SetMenuItemBorderColor ( DependencyObject obj , Brush value )
            {
                  obj .SetValue ( MenuItemBorderColorProperty , value );
            }

            // Using a DependencyProperty as the backing store for MenuItemBorderColor.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty MenuItemBorderColorProperty =
                  DependencyProperty.RegisterAttached("MenuItemBorderColor", typeof(Brush), typeof(MenuAttachedProperties), new PropertyMetadata(default));
            #endregion MenuItemBorderColor

            #region MenuItemBorderThicknesss
            public static Thickness GetMenuItemBorderThickness ( DependencyObject obj )
            {
                  return ( Thickness ) obj .GetValue ( MenuItemBorderThicknessProperty );
            }

            public static void SetMenuItemBorderThickness ( DependencyObject obj , Thickness value )
            {
                  obj .SetValue ( MenuItemBorderThicknessProperty , value );
            }

            // Using a DependencyProperty as the backing store for MenuItemBorderThickness.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty MenuItemBorderThicknessProperty =
                  DependencyProperty.RegisterAttached("MenuItemBorderThickness", typeof(Thickness), typeof(MenuAttachedProperties), new PropertyMetadata((Thickness)default));
		#endregion MenuItemBorderThicknesss

            #region MenuFontSize
            public static double GetMenuFontSize ( DependencyObject obj )
            {
                  return ( double ) obj .GetValue ( MenuFontSizeProperty );
            }

            public static void SetMenuFontSize ( DependencyObject obj , double value )
            {
                  obj .SetValue ( MenuFontSizeProperty , value );
                  Console .WriteLine ( $"MenuFontSize set to  {value}" );
            }

            // Using a DependencyProperty as the backing store for MenuFontSize.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty MenuFontSizeProperty =
                      DependencyProperty.RegisterAttached("MenuFontSize", typeof(double), typeof(MenuAttachedProperties), new PropertyMetadata((double)12));
            #endregion MenuFontSize

            #region MenuFontWeight
            public static string  GetMenuFontWeight ( DependencyObject obj )
            {
                  return  (string) obj .GetValue ( MenuFontWeightProperty );
            }

            public static void SetMenuFontWeight ( DependencyObject obj , string  value )
            {
                  obj .SetValue ( MenuFontWeightProperty , value );
                  Console .WriteLine ( $"MenuFontWeight set to  {value}" );
            }

            // Using a DependencyProperty as the backing store for MenuFontWeight.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty MenuFontWeightProperty =
                  DependencyProperty.RegisterAttached("MenuFontWeight", typeof(string), typeof(MenuAttachedProperties), new PropertyMetadata("Normal"));
            #endregion MenuFontWeight

            #region MenuItemForeground 
            public static Brush GetMenuItemForeground ( DependencyObject obj )
            {
                  return ( Brush ) obj .GetValue ( MenuItemForegroundProperty );
            }

            public static void SetMenuItemForeground ( DependencyObject obj , Brush value )
            {
                  obj .SetValue ( MenuItemForegroundProperty , value );
                  Console .WriteLine ( $"MenuItemForeground set to  {value}" );
            }

            // Using a DependencyProperty as the backing store for MenuForeground.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty MenuItemForegroundProperty =
                  DependencyProperty.RegisterAttached("MenuItemForeground", typeof(Brush), typeof(MenuAttachedProperties), new PropertyMetadata((Brush)default));
            #endregion MenuItemForeground 

            #region MenuItemHeight
            public static double GetMenuItemHeight ( DependencyObject obj )
            {
                  return ( double ) obj .GetValue ( MenuItemHeightProperty );
            }

            public static void SetMenuItemHeight ( DependencyObject obj , double value )
            {
                  obj .SetValue ( MenuItemHeightProperty , value );
            }

            // Using a DependencyProperty as the backing store for MenuItemHeight.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty MenuItemHeightProperty =
                      DependencyProperty.RegisterAttached("MenuItemHeight", typeof(double), typeof(MenuAttachedProperties), new PropertyMetadata((double)14));
            #endregion MenuItemHeight

            #region MenuItemMargin
            public static double GetMenuItemMargin ( DependencyObject obj )
            {
                  return ( double ) obj .GetValue ( MenuItemMarginProperty );
            }

            public static void SetMenuItemMargin ( DependencyObject obj , double value )
            {
                  obj .SetValue ( MenuItemMarginProperty , value );
            }

            // Using a DependencyProperty as the backing store for MenuItemMargin.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty MenuItemMarginProperty =
                      DependencyProperty.RegisterAttached("MenuItemMargin", typeof(double), typeof(MenuAttachedProperties), new PropertyMetadata((double)0));
            #endregion MenuItemMargin

            #region MenuItemSelectedBackground 
            public static Brush GetMenuItemSelectedBackground ( DependencyObject obj )
            {
                  return ( Brush ) obj .GetValue ( MenuItemSelectedBackgroundProperty );
            }

            public static void SetMenuItemSelectedBackground ( DependencyObject d , double value )
            {
                  d .SetValue ( MenuItemSelectedBackgroundProperty , value );
                  Console .WriteLine ( $"MenuSelectedItemBackground set to  {value}" );
            }

            // Using a DependencyProperty as the backing store for MenuItemSelectedBackground.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty MenuItemSelectedBackgroundProperty =
                  DependencyProperty.RegisterAttached("MenuItemSelectedBackground", typeof(Brush ), typeof(MenuAttachedProperties), new PropertyMetadata((Brush)Brushes.Yellow));
            #endregion MenuItemSelectedBackground 

            #region MenuSelectedForeground
            public static Brush GetMenuSelectedForeground ( DependencyObject obj )
            {
                  return ( Brush ) obj .GetValue ( MenuSelectedForegroundProperty );
            }

            public static void SetMenuSelectedForeground ( DependencyObject obj , Brush value )
            {
                  obj .SetValue ( MenuSelectedForegroundProperty , value );
                  Console .WriteLine ( $"MenuSelectedForeground set to  {value}" );
            }

            // Using a DependencyProperty as the backing store for MenuSelectedForeground.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty MenuSelectedForegroundProperty =
                  DependencyProperty.RegisterAttached("MenuSelectedForeground", typeof(Brush), typeof(MenuAttachedProperties), new PropertyMetadata((Brush)default));
            #endregion MenuSelectedForeground
            
      }
}
