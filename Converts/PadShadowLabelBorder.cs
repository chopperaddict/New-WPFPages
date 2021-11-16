using System;
using System . Globalization;

namespace WPFPages . Converts
{
        public partial class PadShadowLabelBorder : System . Windows . Data . IMultiValueConverter
        {
                #region IValue Converter methods

                public object Convert ( object [ ] values, Type targetType, object parameter, CultureInfo culture )
                {
                        double result = 0;
                        double TextHeight = 0;//			Type t = values[0].GetType();
                                              //double TextHeight = 0;
                                              //string fontstyle = "";
                                              ////			double divisor = 2.0;
                        object obj = values [ 0 ];
                        string name = obj . ToString ( );
                        if ( name != "TextWithShadowAndScaleTransform" )
                                return 0;
                        obj = values [ 1 ];
                        TextHeight = double . Parse ( obj . ToString ( ) );
                        obj = values [ 2 ];
                        double padding = double . Parse ( obj . ToString ( ) );
                        result = TextHeight + padding;
                        //Type t = caller . GetType ( );
                        //ThreeDeeBtnControl tb = caller as ThreeDeeBtnControl;
                        //string Content = tb . BtnText;
                        //double len = ( double ) tb . TextWidth;
                        //fontstyle = tb . FontDecoration;
                        //TextHeight = ( double ) tb . TextHeight;
                        //object obj = tb . FindName ( "ThreeDBtn" );
                        //System . Windows . Controls . Button b = obj as System . Windows . Controls . Button;
                        //TextWidth = b . ActualWidth;
                        //object o2 = b . FindName ( "Btn6Content" );
                        Console . WriteLine ( $"Value going back = {result} TextHeight ={TextHeight}  Padding ={padding} \n" );
                        return ( object ) result;
                }

                public object [ ] ConvertBack ( object value, Type [ ] targetTypes, object parameter, CultureInfo culture )
                {
                        throw new NotImplementedException ( );
                }

                #endregion IValue Converter methods
        }
}
