using Microsoft . SqlServer . Management . Smo;

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
using System . Windows . Navigation;
using System . Windows . Shapes;

using static System . Windows . Forms . VisualStyles . VisualStyleElement . Button;

namespace WPFPages . UserControls
{
	/// <summary>
	/// Interaction logic for MsgBox.xaml
	/// </summary>
	public partial class MsgBox : UserControl
	{
#region rowdata
		int row1Height { get; set; }
		int row21Height { get; set; }
		int row3Height { get; set; }
		int row4Height { get; set; }
		int row1Width { get; set; }
		int row2Width { get; set; }
		int row3Width { get; set; }
		int row4Width { get; set; }

		string Row1String { get; set; }
		string Row2String { get; set; }
		string Row3String { get; set; }
		#endregion rowdata
		
#region buttondata

		Button OKButton  = new Button();
		Button YESButton  = new Button();
		Button NOButton  = new Button();
		Button CANCELButton  = new Button();
		Button UserButton1  = new Button();
		Button UserButton2  = new Button();
		bool[] InUse ={false, false, false, false, false , false , false };
		Image image { set; get; }

		List<buttons> Buttons= new List<buttons>();
		Dictionary<int, Button> BtnDict = new     Dictionary<int, Button>();
		enum buttons
		{
			OKBUTTON = 0,
			YESBUTTON,
			NOBUTTON,
			CANCELBUTTON,
			USERBUTTON1,
			USERBUTTON2
		}
		#endregion buttondata

		public MsgBox ( )
		{
			InitializeComponent ( );
			Window_Loading ( );
		}
		
		#region Rowdata Handlers
		
		public string GetRow1Data ( ){return Row1 . Text;}
		public void  SetRow1Data ( ){Row1String = Row1 . Text;}
		public string GetRow2Data ( ) { return Row2 . Text; }
		public void SetRow2Data ( ) { Row1String = Row2 . Text; }
		public string GetRow3Data ( ) { return Row3 . Text; }
		public void SetRow3Data ( ) { Row1String = Row3 . Text; }

		#endregion Rowdata Handlers

		#region Button Handlers
		public void SetBtnActive ( int btn , bool mode ) { InUse [ btn ] = mode; }
		public void ClearBtnsActive ( ){int count=0;foreach ( var item in InUse ){InUse [ count++ ] = false;}InUse [ count ] = false; }

		private void Window_Loading ( )
		{
			Buttons . Add (buttons.OKBUTTON );
			Buttons . Add ( buttons . YESBUTTON );
			Buttons . Add ( buttons . NOBUTTON );
			Buttons . Add ( buttons . CANCELBUTTON );
			Buttons . Add ( buttons . USERBUTTON1 );
			Buttons . Add ( buttons . USERBUTTON2 );
			BtnDict . Add ( 1 , OKButton );
			BtnDict . Add ( 2 , YESButton );
			BtnDict . Add ( 3 , NOButton );
			BtnDict . Add ( 4 , CANCELButton );
			BtnDict . Add ( 5 , UserButton1 );
			BtnDict . Add ( 6 , UserButton2 );
		}

		/// <summary>
		///  accepts a variabe no. of params and uses these t set Buton array as active/Non actve
		/// </summary>
		/// <param name="args"></param>
		private void SetActveButtons (params  int [ ] args )
		{
			int count = 0;
			ClearAllInuse ( );
			foreach ( var item in args )
			{
				InUse [ count++ ] = true;
			}
		}
		
		/// <summary>
		/// Makes selected buttons Visible based on int[] InUse set by caller
		/// </summary>
		/// <param name="args"></param>
		private void ShowActveButtons ( int [ ] args )
		{
			for(int x = 0 ; x < args.Length ; x++) 
			{
				if ( InUse [ x ] )
				{
					KeyValuePair<int, Button> b = BtnDict.ElementAt(args[x]);
					b . Value . Visibility = Visibility . Visible;
				}
			}
		}
		private void ClearAllInuse (){
			int count = 0;
			foreach ( var item in InUse )
			{
				InUse[count++] = false;
			}
		}
		#endregion Button Handlers

		private void UserControl_Loaded ( object sender , RoutedEventArgs e )
		{
			SetActveButtons ( 2,3,4);	// Yes/No/Cancel
		}
		
		public  void ShowMsgbox(
			string string1 ,
			string string2 ,
			string string3 ,
			int[] btns,
			Image icon
			)
		{
			Row1String = string1;
			Row2String = string2;
			Row3String = string3;
			SetActveButtons ( btns );
			image = icon;  // Need to load it here ...
		}
	}
}
