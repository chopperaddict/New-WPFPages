﻿using System;
using System . Globalization;
using System . Windows;
using System . Windows . Data;

namespace WPFPages . Converts
{
        // ALL CONVERTERS ARE ACTUALLY WORKING 10/7/21
        public class DateTimeToShortStringConverter : IValueConverter
	{
		public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( value == DependencyProperty . UnsetValue )
				return DependencyProperty . UnsetValue;
			// Receives a FULL date with time = "01/01/1933 12:13:54"
			// Returns just the date part = "01/01/1933"
			string date = value . ToString ( );
			char [ ] ch = { ' ' };
			string [ ] dateonly = date . Split ( ch [ 0 ] );
			return ( string ) dateonly [ 0 ];
		}

		public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return null as object;
		}
	}
}
