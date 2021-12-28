using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;

using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Windows . Shapes;

namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for Grabviewer.xaml
	/// </summary>
	public partial class Grabviewer : Window
	{
		private static double imgheight = 0;
		private static double imgwidth = 0;
		private static double WindowExtraSize = 0;
		private static double scrnwidth= 0;
		private static double scrnheight= 0;
		Window caller;
		Control ctrl;
		public Grabviewer ( Window parent,Control ctrl, RenderTargetBitmap bmp )
		{
			if ( bmp == null )
				return;
			InitializeComponent ( );
			Utils . SetupWindowDrag ( this );
			caller = parent;
			ctrl = ctrl;
			//scrnheight = SystemParameters . PrimaryScreenHeight;
			//scrnwidth= SystemParameters . PrimaryScreenWidth;
			//WindowExtraSize = SystemParameters . CaptionHeight;

			//Grabimage . Refresh ( );
			Grabimage . Height = bmp . Height;
			Grabimage . Width = bmp . Width;
			Grabimage . Stretch = Stretch .None;
			GrabGrid.Height= bmp . Height;
			GrabGrid. Width = bmp . Width;
			Grabimage . HorizontalAlignment = HorizontalAlignment . Center;
			Grabimage . VerticalAlignment = VerticalAlignment . Center;

			this . Height = Grabimage . Height;
			this . Width= Grabimage . Width;
			if ( this . Height < this . MinHeight )
				this . Height = this . MinHeight;
			if ( this . Width < this . MinWidth )
				this . Width = this . MinWidth;

			if ( this . Height <= GrabGrid. Height )
				this . Height = GrabGrid . Height * 1.2;
			if ( this . Width <= this . GrabGrid.Width )
				this . Width = GrabGrid.Width * 1.2;


			//this . Height = bmp . Height;// + WindowExtraSize;
			//this . Width= bmp . Width ;

			//this . SizeToContent = SizeToContent.Height;
			//this . Height = this . Height * 2;
			//this . Width= this . Width * 2;
			//this . Height = SystemParameters . PrimaryScreenHeight * 0.95;
			//this . Width = SystemParameters . PrimaryScreenWidth * 0.95;
			this . UpdateLayout ( );
			this . Refresh ( );
			imgheight = this . Height;
			imgwidth=this . Width;
		}

		private void adjustToDPI ( )
		{
		}
		private void GrabWin_SizeChanged ( object sender , SizeChangedEventArgs e )
		{
			if ( this . Height < imgheight )
			{
				this . Height = imgheight;
				this . MinHeight = imgheight + 50;
//				GrabGrid . Height = this . Height;
			}
			if ( this. Width < imgwidth )
			{
				this . Width = imgwidth;
				this . MinWidth= imgwidth;
			}
			//GrabViewBox . Height = this . Height;
			//GrabViewBox . Width = this . Width;
			Grabimage. Height = GrabViewBox . Height;
			Grabimage . Width = GrabViewBox . Width;
			this . UpdateLayout ( );
			e . Handled = true;
		}

		private void closebtn_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
		{
			this . Close ( );
			caller . Focus ( );
			ctrl ?. Focus ( );
		}

		private void GrabWin_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
		{
			this . Close ( );
			caller . Focus ( );
			ctrl ?. Focus ( );
		}

		private void GrabWin_PreviewKeyDown ( object sender , KeyEventArgs e )
		{
			e . Handled = true;
			if ( e . Key == Key . F12 )
				this . Close ( );
			caller . Focus ( );
			ctrl ?. Focus ( );
		}

	}

}

