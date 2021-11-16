using System;
using System . Globalization;
using System . Windows;
using System . Windows . Data;

namespace WPFPages . Converts
{
	public class NumericString2ShortDateConverter : IValueConverter
	{
		public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( value == DependencyProperty . UnsetValue )
				return DependencyProperty . UnsetValue;

                        string Output = "";
                        string input = value . ToString ( ).Trim();
                        if ( input . Contains ( ":" ) )
                        {
                                string [ ] parts = input . Split ( ':' );
                                if ( parts . Length != 3 )
                                        return value;
                                if ( parts [ 0 ] . Length == 4 )
                                {
                                        // looks like the format is yyyy:mm:dd ?
                                        // return it as dd/mm/yy
                                        Output = parts [ 2 ].Trim() + "/";
                                        Output += parts [ 1 ] . Trim ( ) + "/";
                                        Output += parts [ 0 ] . Trim ( );
                                }
                                else if (parts [ 2 ] . Length == 4 )
                                {
                                        // looks like the format is dd:mm:yyyy ?
                                        // return it as dd/mm/yy
                                        Output = parts [ 0 ] . Trim ( ) + "/";
                                        Output += parts [ 1 ] . Trim ( ) + "/";
                                        Output += parts [ 2 ] . Trim ( );
                                }
                        }
                        else
                        {
                                // Assumes receiving a date as "21041932" or similar
                                string date = value . ToString ( ).Trim ( );
                                Output = date [ 0 ] + date [ 1 ] + "/";
                                Output += date [ 2 ] + date [ 3 ] + "/";
                                Output += date [ 4 ] + date [ 5 ];
                                Output += date [ 6 ] + date [ 7 ];
                        }
			return Output;
		}
		public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
		{
                        // Convert  a DateTime value to a DD/MM/YYYY string but ignore time element
                        string Output = value . ToString ( );
                        if ( value == DependencyProperty . UnsetValue )
                                return DependencyProperty . UnsetValue;
                        string input = value . ToString ( ) . Trim ( );
                        if ( input . Contains ( " " ) )
                        {
                                string [ ] part = input . Split ( ' ' );
                                if ( part [ 0 ] . Length == 10 )
                                        Output = part [ 0 ];
                        }
                        return (object)Output;
		}
	}

}