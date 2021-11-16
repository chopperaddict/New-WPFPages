//#define TESTING

namespace WPFPages . Views
{
	public partial class AnimationTest
	{
		public partial class NativeMethods
		{
			/// Return Type: BOOL->int  
			///X: int  
			///Y: int  
			[System . Runtime . InteropServices . DllImportAttribute ( "user32.dll", EntryPoint = "SetCursorPos" )]
			[return: System . Runtime . InteropServices . MarshalAsAttribute ( System . Runtime . InteropServices . UnmanagedType . Bool )]
			public static extern bool SetCursorPos ( int X, int Y );

			[System .Runtime .InteropServices .DllImportAttribute ( "user32.dll" , EntryPoint = "GetCursorPos" )]
			[return: System .Runtime .InteropServices .MarshalAsAttribute ( System .Runtime .InteropServices .UnmanagedType .Bool )]
			public static extern bool GetCursorPos ( int X , int Y );
		}
	}
}
