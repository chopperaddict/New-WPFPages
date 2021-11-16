using System;
using System . Diagnostics;
using System . Globalization;
using System . Windows . Data;

namespace WPFPages . Converts
{
        internal class DummyDebugConverter : IValueConverter
        {
                object IValueConverter.Convert ( object value, Type targetType, object parameter, CultureInfo culture )
                {
                        Debugger . Break ( );
                        return value;
                }

                object IValueConverter.ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
                {
                        Debugger . Break ( );
                        return value;
                }
        }
}
