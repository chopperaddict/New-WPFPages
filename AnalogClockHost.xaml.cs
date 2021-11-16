using MyAnalogClock;

using System;
using System .Collections .Generic;
using System .Linq;
using System .Text;
using System .Threading .Tasks;
using System .Windows;
using System .Windows .Controls;
using System .Windows .Data;
using System .Windows .Documents;
using System .Windows .Input;
using System .Windows .Media;
using System .Windows .Media .Imaging;
using System .Windows .Shapes;

using static MyAnalogClock .AnalogClock;

namespace WPFPages
{
	/// <summary>
	/// Interaction logic for AnalogClockHost.xaml
	/// </summary>
	public partial class AnalogClockHost : Window
	{
		public AnalogClockHost ( )
		{
			InitializeComponent ( );
		}

		private void AnalogClock_PreviewKeyDown ( object sender , KeyEventArgs e )
		{
			Console .WriteLine ($"Key is {e.Key.ToString()}");
			if ( e .Key == Key .OemPlus
				)
				analogclock .Width--;
			else if ( e .Key == Key .F8
				)
				analogclock .Width--;
			else if ( e .Key == Key .F9)
				analogclock .Width++;
			else
				analogclock .Width++;
		}
	}
}
