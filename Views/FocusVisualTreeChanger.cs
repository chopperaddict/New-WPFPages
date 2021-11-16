using System . Windows;



namespace WPFPages . Views
{

	#region VisualTree
	public class FocusVisualTreeChanger
	{
		public static bool GetIsChanged ( DependencyObject obj )
		{
			return ( bool ) obj . GetValue ( IsChangedProperty );
		}

		public static void SetIsChanged ( DependencyObject obj , bool value )
		{
			obj . SetValue ( IsChangedProperty , value );
		}

		public static readonly DependencyProperty IsChangedProperty =
		DependencyProperty.RegisterAttached("IsChanged", typeof(bool), typeof(FocusVisualTreeChanger), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, IsChangedCallback));

		private static void IsChangedCallback ( DependencyObject d , DependencyPropertyChangedEventArgs e )
		{
			if ( true . Equals ( e . NewValue ) )
			{
				FrameworkContentElement contentElement = d as FrameworkContentElement;
				if ( contentElement != null )
				{
					contentElement . FocusVisualStyle = null;
					return;
				}

				FrameworkElement element = d as FrameworkElement;
				if ( element != null )
				{
					element . FocusVisualStyle = null;
				}
			}
		}
		#endregion VisualTree

	}

}
