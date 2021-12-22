using Microsoft . SqlServer . Management . Sdk . Sfc . Metadata;

using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;

using WPFPages . ViewModels;

namespace WPFPages
{
	public static class Extensions
	{
		public static Brush ToSolidBrush ( this string HexColorString )
		{
			if ( HexColorString . Length < 9 )
			{
//				MessageBox.Show( "The Hex value entered is invalid. It needs to be # + 4 hex pairs\n\neg: [#FF0000FF] = BLUE ");
				return null;
			}
			try
			{
				if ( HexColorString != null && HexColorString != "" )
					return ( Brush ) ( new BrushConverter ( ) . ConvertFrom ( HexColorString ) );
				else
					return null;
			}
			catch(Exception ex)
			{
				Console . WriteLine ($"ToSolidbrush failed - input = {HexColorString}");
				return null;
			}
		}
		public static LinearGradientBrush ToLinearGradientBrush ( this string Colorstring )
		{
			try
			{
				return Application . Current . FindResource ( Colorstring ) as LinearGradientBrush;
			}
			catch ( Exception ex )
			{
				Console . WriteLine ( $"ToLinearGradientbrush failed - input = {Colorstring}" );
				return null;
			}
			//return ( LinearGradientBrush ) ( new BrushConverter ( ) . ConvertFrom ( color ) );
		}
		public static string BrushtoText ( this Brush brush )
		{
			try
			{
				if ( brush != null )
					return ( string ) brush . ToString ( );
				else
					return null;
			}
			catch ( Exception ex )
			{
				Console . WriteLine ( $"BrushtoText failed - input = {brush }" );
				return null;
			}
		}
		public static BankAccountViewModel ToBankRecord ( this string record )
		{
			BankAccountViewModel bvm = new  BankAccountViewModel();
			//string [] fields = record.Split(',');
			//foreach ( var item in fields )
			//{

			//}

			return bvm;
		}
	}

}
