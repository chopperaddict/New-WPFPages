﻿using System;
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

namespace WPFPages
{
	/// <summary>
	/// Interaction logic for TemplateforWpfWindow.xaml
	/// </summary>
	public partial class TemplateforWpfWindow : Window
	{
		public TemplateforWpfWindow ( )
		{
			InitializeComponent ( );
		}

		private void Window_Loaded ( object sender , RoutedEventArgs e )
		{
			MouseMove += Grab_MouseMove;
			KeyDown += Window_PreviewKeyDown;
		}
		private void Grab_MouseMove ( object sender , MouseEventArgs e )
		{
			if ( e . LeftButton == MouseButtonState . Pressed )
				Utils . Grab_MouseMove ( sender , e );
			e . Handled = true;
		}

		private void Window_PreviewKeyDown ( object sender , KeyEventArgs e )
		{
			if ( e . Key == Key . F11 )
			{
				var pos = Mouse . GetPosition ( this);
				Utils . Grab_Object ( sender , pos );

				if ( Utils . ControlsHitList . Count == 0 )
					return;
				Utils . Grabscreen ( this , Utils . ControlsHitList [ 0 ] . VisualHit , null, sender as Control );
			}
		}


	}
}
