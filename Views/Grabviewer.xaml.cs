using System;
using System . IO;
using System . Linq;
using System . Runtime . CompilerServices;
using System . Threading;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;

using static System . Net . WebRequestMethods;

namespace WPFPages . Views
{
	/// <summary>
	/// Interaction logic for Grabviewer.xaml
	/// </summary>
	[Serializable]
	public partial class Grabviewer : Window
	{
		private static double imgheight = 0;
		private static double imgwidth = 0;
		private static double WindowExtraSize = 0;
		private static double scrnwidth= 0;
		private static double scrnheight= 0;
		private string Imagepath= "C:\\WPFPages-11nov21\\Icons\\Grabimage.png";
		private RenderTargetBitmap  img;
		Window caller;
		Control ctrl;
		public Grabviewer ( Window parent , Control ctrl , RenderTargetBitmap bmp )
		{
			if ( bmp == null )
				return;
			InitializeComponent ( );
			Utils . SetupWindowDrag ( this );
			caller = parent;
			ctrl = ctrl;
			// just read image frm disk , cos the initial call ALLWAYS saves it to disk as "C:\\WPFPages-11nov21\\Icons\\Grabimage.png"
			// / automatically, overwriting any existing file....
			BitmapImage bmi = new BitmapImage ( new Uri ( "C:\\WPFPages-11nov21\\Icons\\Grabimage.png" ) );
			Grabimage . Source = bmi;
			// Grab the size of the image cos  otherwise it doesnt paint corretly.
			Grabimage . Width = bmi . PixelWidth;
			Grabimage . Height = bmi . PixelHeight;

			Grabimage . HorizontalAlignment = HorizontalAlignment . Center;
			Grabimage . VerticalAlignment = VerticalAlignment . Center;

			this . Height = Grabimage . Height + SystemParameters . CaptionHeight + 75 + SystemParameters . BorderWidth * 2;
			this . Width = Grabimage . Width + 75 + SystemParameters . BorderWidth * 2;
			this . UpdateLayout ( );
			this . Refresh ( );
			//imgheight = this . Height;
			//imgwidth = this . Width;

		}

		//public static ImageSource ByteToImage ( byte [ ] imageData )
		//{
		//	BitmapImage biImg = new BitmapImage();
		//	MemoryStream ms = new MemoryStream(imageData);
		//	biImg . BeginInit ( );
		//	biImg . StreamSource = ms;
		//	biImg . EndInit ( );

		//	ImageSource imgSrc = biImg as ImageSource;

		//	return imgSrc;
		//}


		//private static byte [ ] LoadImage ( string path )
		//{
		//	byte[] bytes;
		//	//using ( FileStream fs = new FileStream ( path ,
		//	//		FileMode . Open , FileAccess . Read , FileShare . None ) )
		//	//{
		//	FileStream fs = new FileStream("C: \\users\\ianch\\documents\\Grabimage . png" , FileMode.Open,FileAccess.Read);

		//	bytes = new byte [ fs . Length ];
		//	fs . Read ( bytes , 0 , System . Convert . ToInt32 ( fs . Length ) );
		//	fs . Close ( );
		//	return bytes;
		//	// sortta wo rks
		//	//bytes = Enumerable.Repeat((byte)0x20, 700000).ToArray();
		//	//int len = (int)fs.Length;
		//	//int bytesread =  fs . Read (bytes, 0, len );
		//	//if ( bytesread == 0 )
		//	//	return null;
		//	//			}
		//	//var image = new BitmapImage();
		//	//using ( var mem = new MemoryStream ( bytes ) )
		//	//{
		//	//	mem . Position = 0;
		//	//	image . BeginInit ( );
		//	//	image . CreateOptions = BitmapCreateOptions . PreservePixelFormat;
		//	//	image . CacheOption = BitmapCacheOption . OnLoad;
		//	//	image . UriSource = null;
		//	//	image . StreamSource = mem;
		//	//	image . EndInit ( );
		//	//}
		//	//image . Freeze ( );
		//	//return image;
		//}
		public static BitmapSource ReadImageFromDisk ( string path )
		{
			BitmapSource bitmapSource=null;
			;
			using ( FileStream fs = new FileStream ( path ,
				FileMode . Open , FileAccess . Read , FileShare . None ) )
			{
				byte[] bytes = Enumerable.Repeat((byte)0x20, 700000).ToArray();
				int len = (int)fs.Length;
				int bytesread =  fs . Read (bytes, 0, len );
				//LoadImage ( bytes );
				bitmapSource = BitmapSource . Create ( 16 , 16 , 96 , 96 , PixelFormats . Pbgra32 , null , bytes , 16 );
				//bitmapSource = BitmapSource . Create ( Width , Height , 300 , 300 , PixelFormats . Indexed8 , BitmapPalettes . Gray256 , bytes , 2 );
			}
			return bitmapSource;
		}
		private void ChecksMouseMove ( object sender , MouseEventArgs e )
		{
			e . Handled = true;
			if ( e . RightButton == MouseButtonState . Pressed )
				return;
		}

		private void GrabWin_SizeChanged ( object sender , SizeChangedEventArgs e )
		{
		}

		private void closebtn_PreviewMouseLeftButtonDown ( object sender , MouseButtonEventArgs e )
		{
			this . Close ( );
			caller . Focus ( );
			ctrl?.Focus ( );
		}

		private void GrabWin_PreviewMouseRightButtonDown ( object sender , MouseButtonEventArgs e )
		{
			this . Close ( );
			caller . Focus ( );
			ctrl?.Focus ( );
		}

		private void GrabWin_PreviewKeyDown ( object sender , KeyEventArgs e )
		{
			e . Handled = true;
			if ( e . Key == Key . F12 )
				this . Close ( );
			caller . Focus ( );
			ctrl?.Focus ( );
		}

		private void GrabWin_MouseDoubleClick ( object sender , MouseButtonEventArgs e )
		{
			bool isfinished=false;
			// Save image to Documents or wherever ?
			do
			{
				string path = Utils . GetExportFileName ( "Screengrab.png" );
				if ( path . Contains ( "\\" ) == false )
					break;
				try
				{
					if ( System . IO . File . Exists ( path ) )
					{
						this . Topmost = false;

						Utils . Mbox ( this , string1: "A file of the same name already exists in this folder !" , string2: "Do you want to overwrite it ?" , caption: "File Overwrite Caution " , iconstring: "\\icons\\Information.png" , Btn1: MB . YES , Btn2: MB . NO , defButton: MB . YES);
						if ( DlgInput .returnint == 0 )
							break;
						System . IO . File . Copy ( Imagepath , path );
						Utils . Mbox ( this , string1: "Image save successfully ....." , string2: "" , caption: "" , iconstring: "\\icons\\Information.png" , Btn1: MB . OK , Btn2: MB . NNULL , defButton: MB . OK, minsize: true );
					}
					else
					{
						System . IO . File . Copy ( Imagepath , path );
						Utils . Mbox ( this , string1: "Image save successfully ....." , string2: "" , caption: "" , iconstring: "\\icons\\Information.png" , Btn1: MB . OK , Btn2: MB . NNULL , defButton: MB . OK , minsize: true );
					}
					break;
				}
				catch ( Exception ex )
				{
					this . Topmost = false;
					Utils . Mbox ( this , string1: "A file of the same name already exists in this folder !" , string2: "Do you want to overwrite it ?" , caption: "File Overwrite Caution " , iconstring: "\\icons\\Information.png" , Btn1: MB . YES , Btn2: MB . NO , defButton: MB . YES);
					if ( DlgInput . returnint == 0 )
						break;
					try
					{
						System . IO . File . Copy ( Imagepath , path );
						Utils . Mbox ( this , string1: "Image save successfully ....." , string2: "" , caption: "" , iconstring: "\\icons\\Information.png" , Btn1: MB . OK, Btn2: MB . NNULL , defButton: MB . OK , minsize: true );
					}
					catch ( Exception ex2 )
					{
						Console . WriteLine ( $"Failed to copy file....." );
						Utils . DoErrorBeep ( repeat: 2 );
						break;
					}
				}
				if ( isfinished )
					break;
			} while ( true );
			// finally, close viewer
			this.Close ( );
		}
	}
}



