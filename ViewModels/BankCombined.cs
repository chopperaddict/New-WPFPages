using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;

namespace WPFPages . ViewModels
{
	public class BankCombinedViewModel : BankAccountViewModel
	{
		public BankCombinedViewModel ( )
		{
		}

		string LName { get; set; }
		string FName { get; set; }
		string Addr1 { get; set; }
		string Addr2  { get; set; }
		string Town { get; set; }
		string County { get; set; }
		string PCode { get; set; }
		string Phone { get; set; }
		string Mobile { get; set; }
	}
}
