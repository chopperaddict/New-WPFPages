using System;
using System . Globalization;
using System . Windows;
using System . Windows . Data;

namespace WPFPages . Converts
{
        public class Actype2Name : IValueConverter
	{
		public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			// Receives an int value 1-4
			// Returns the AC type as a word "Checking A/c"
			if ( value == DependencyProperty . UnsetValue )
				return DependencyProperty . UnsetValue;
			try
			{
				int val = System . Convert . ToInt32 ( value );
				if ( val == 0 )
					return "Unknown";
				else if ( val == 1 )
					return "Checking A/C";
				else if ( val == 2 )
					return "Deposit A/C";
				else if ( val == 3 )
					return "Savings A/C";
				else if ( val == 4 )
					return "Business A/C";
				else
					return ( object ) null;
			}
			catch ( Exception ex )
			{
				return ( object ) null;
			}
		}

		public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return null as object;
		}
	}
}
