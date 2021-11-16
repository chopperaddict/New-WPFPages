using System .Windows .Controls;
using System .Windows;
using WPFPages .Views;
using WPFPages .ViewModels;

namespace WPFPages .DataTemplates
{
	/// <summary>
	/// A Class to let ListBox & ListView (ONLY) switch DataTemplates on the fly
	/// you use the ItemTemplateSelector Property to point to this class
	/// </summary>
	public class BankDataTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate ( object item , DependencyObject container )
		{
			FrameworkElement element = container as FrameworkElement;

			// Task is the particular value to let you select the requiired Template element 
			if ( element != null && item != null && item is BankAccountViewModel )
			{
				BankAccountViewModel dg = item as BankAccountViewModel;

				if ( dg .AcType == 2 )
					return
					    element .FindResource ( "BankDataTemplate1" ) as DataTemplate;
				else
					return
					    element .FindResource ( "BankDataTemplate2" ) as DataTemplate;
			}
			return null;
		}
	}
}


