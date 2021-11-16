using System . Windows;
using System . Windows . Media;

namespace WPFPages . ViewModels
{

        public  class Ex : DependencyObject
        {
                #region ItemHeight Attached Property
                public static readonly DependencyProperty ItemHeightProperty
                        = DependencyProperty . RegisterAttached (
                        "ItemHeight",
                        typeof ( double ),
                        typeof ( Ex ),
                        new PropertyMetadata ( (double)20 ) );
                public static double GetItemHeight ( DependencyObject d )
                {
                        return ( double ) d . GetValue ( ItemHeightProperty );
                }
                public static void SetItemHeight ( DependencyObject d, double value )
                {
                        d . SetValue ( ItemHeightProperty, value );
                }
                #endregion

                #region Background Attached Property
                public static readonly DependencyProperty BackgroundProperty
                        = DependencyProperty . RegisterAttached (
                        "Background",
                        typeof ( Brush ),
                        typeof ( Ex),
                        new PropertyMetadata ( ( Brush) Brushes.LightGray) );
                public static Brush GetBackground ( DependencyObject d )
                {
                        return ( Brush) d . GetValue ( BackgroundProperty );
                }
                public static void SetBackground ( DependencyObject d, Brush value )
                {
                        d . SetValue ( BackgroundProperty, value );
                }
                #endregion
        }
}
