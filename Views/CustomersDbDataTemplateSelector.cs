using System . Windows;
using System . Windows . Controls;



namespace WPFPages . Views
{
	public class CustomersDbDataTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate ( object item , DependencyObject container )
		{
			FrameworkElement elemnt = container as FrameworkElement;
			int actype = ( int ) item;
			if ( actype == 1 )
				return elemnt .FindResource ( "Actype1DataTemplate" ) as DataTemplate;
			else if ( actype == 2 )
				return elemnt .FindResource ( "Actype2DataTemplate" ) as DataTemplate;
			else if ( actype == 3 )
				return elemnt .FindResource ( "Actype3DataTemplate" ) as DataTemplate;
			else if ( actype == 4 )
				return elemnt .FindResource ( "Actype4DataTemplate" ) as DataTemplate;
			return null;
		}
	}

}
