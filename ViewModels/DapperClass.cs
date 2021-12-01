using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . Data . SqlClient;
using System . Data;
using System . Linq;
using System . Security . Policy;
using System . Text;
using System . Threading . Tasks;
using Dapper;
using System . Security . Cryptography;

namespace WPFPages . ViewModels
{
	public class DapperClass : IEnumerable
	{
		public IEnumerator GetEnumerator ( )
		{
			return this . GetEnumerator ( );
		}
		public DapperClass ( )
		{

		}
		public IEnumerable GetDataViaSp<T> ( T dbtype ,
			string SqlCommand ,
			string parameters="")
		{
			string ConString = ( string ) Properties . Settings . Default [ "BankSysConnectionString" ];
			DynamicParameters Params = ParseParameters ( parameters );
			//Params = ParseParameters ( parameters );
			using ( IDbConnection db = new SqlConnection ( ConString ) )
			{
				IEnumerable enumer = db . Query<T> ( SqlCommand , Params ,null, true, null,CommandType . StoredProcedure );
				return enumer;
			}
		}
		private DynamicParameters ParseParameters ( string parameters )
		{
			DynamicParameters Params = new DynamicParameters();
			string[] temp;
			if ( parameters == "" )
				return null;
			 temp = parameters . Split ( ',' );
			foreach ( var item in temp)
			{
				Params . Add ( item );
			}
			return Params;
		}

	}
}
