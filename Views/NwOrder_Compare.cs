using System;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;

namespace WPFPages . Views
{
        public class NwOrder_Compare : IComparer<nworder>
        {
                public int Compare ( nworder x, nworder y )
                {
                        if ( object . ReferenceEquals ( x, y ) )
                              return 0;
                        if ( x == null )
                                return -1;
                        if ( y == null )
                                return 1;
                        return x . OrderId . CompareTo ( y . OrderId );
                }
        }
}
