using System .Windows .Controls;
using System .Windows;
using System .Windows .Media;
using WPFPages .ViewModels;

namespace WPFPages .DataTemplates
{
	public class BankDataStyleTemplateSelector : StyleSelector
	{
		// This needs to retun a style for specific datagrid Row type
		public override Style SelectStyle ( object item ,
			  DependencyObject container )
		{
			Style st = new Style();
			st .TargetType = typeof ( DataGridRow);
			Setter setter = new Setter();
			setter .Property = DataGridRow .StyleProperty;
//			ListView listView = ItemsControl.ItemsControlFromItemContainer(container) as ListView;
			DataGrid listView = ItemsControl.ItemsControlFromItemContainer(container) as DataGrid;
			BankAccountViewModel bvm =  listView .SelectedItem as BankAccountViewModel ;
//			int index = listView.ItemContainerGenerator.IndexFromContainer(container);
			if ( bvm.AcType  == 2 )
			{
				//setter .Value = FindResource(BankAccountGridStyle)
			}
			else
			{
					//setter .Value = Brushes .Beige;
			}
	//		st .Setters .Add ( setter );
			return st;
		}
	}
}


