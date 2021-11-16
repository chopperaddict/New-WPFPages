using System;
using System .Collections .Generic;
using System .Linq;
using System .Text;
using System .Threading .Tasks;
using System .Windows .Controls;
using System .Windows;

namespace WPFPages .DataTemplates
{
	public class CustomerDataTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate ( object item , DependencyObject container )
		{
			FrameworkElement element = container as FrameworkElement;

			// Task is the particular value to let you select the requiired Template element 
			if ( element != null && item != null && item is DataGrid)
			{
				DataGrid dg = item as DataGrid;
			
				if ( dg .Name == "custgrid" )
					return
					    element .FindResource ( "CustomersDbTemplate1" ) as DataTemplate;
				else
					return
					    element .FindResource ( "CustomersDbTemplate2" ) as DataTemplate;
			}
			return null;
		}
	}
}

