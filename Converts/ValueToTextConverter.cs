using System;
using System . Collections . Generic;
using System . Globalization;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Data;

namespace WPFPages . Converts
{
	public class ValueToTextConverter : IValueConverter
	{
		public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
		{
                        string input = "";
                        input = value . ToString ( );
                        return input ;
		}

		public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
		{
                        string input = value . ToString ( );
                        if(input.Contains("."))
                                return (object)System . Convert . ToDouble( value );
                        else
                                return ( object ) System . Convert.ToInt32 ( value );
		}
	}

}
