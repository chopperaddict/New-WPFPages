using System;
using System . Collections;
using System . Collections . Generic;
using System . Collections . ObjectModel;
using System . ComponentModel;
using System . Diagnostics;
using System . Linq;
using System . Runtime . CompilerServices;
using System . Runtime . InteropServices;
using System . Text;
using System . Threading . Tasks;

namespace WPFPages . ViewModels
{
        public class TreeviewDataModel  : INotifyPropertyChanged
        {
                 public static ObservableCollection<TopLevel> Toplevel = new ObservableCollection<TopLevel> ( );
                public static ObservableCollection<MidLevel> Midlevel= new ObservableCollection<MidLevel> ( );
                public static ObservableCollection<BaseLevel> Baselevel= new ObservableCollection<BaseLevel> ( );

                public static TopLevel Level1 = new TopLevel ( );
                public static MidLevel Level2 = new MidLevel ( );
                public static BaseLevel Level3 = new BaseLevel ( );
                public static ObservableCollection<TreeviewDataModel >TreeViewData = new ObservableCollection<TreeviewDataModel>( );

                #region PropertyChanged
                new public event PropertyChangedEventHandler PropertyChanged;

                #region PropertyChanged
                new private void OnPropertyChanged ( string propertyName )
                {
                        if ( Flags . SqlBankActive == false )
                                //				this . VerifyPropertyName ( propertyName );

                                if ( this . PropertyChanged != null )
                                {
                                        var e = new PropertyChangedEventArgs ( propertyName );
                                        this . PropertyChanged ( this, e );
                                }
                }
                /// <summary>
                /// Warns the developer if this object does not have
                /// a public property with the specified name. This
                /// method does not exist in a Release build.
                /// </summary>
                [Conditional ( "DEBUG" )]
                [DebuggerStepThrough]
                public virtual void VerifyPropertyName ( string propertyName )
                {
                        // Verify that the property name matches a real,
                        // public, instance property on this object.
                        if ( TypeDescriptor . GetProperties ( this ) [ propertyName ] == null )
                        {
                                string msg = "Invalid property name: " + propertyName;

                                if ( this . ThrowOnInvalidPropertyName )
                                        throw new Exception ( msg );
                                else
                                        Debug . Fail ( msg );
                        }
                }
                #endregion
                /// <summary>
                /// Returns whether an exception is thrown, or if a Debug.Fail() is used
                /// when an invalid property name is passed to the VerifyPropertyName method.
                /// The default value is false, but subclasses used by unit tests might
                /// override this property's getter to return true.
                /// </summary>
                protected virtual bool ThrowOnInvalidPropertyName
                {
                        get; private set;
                }

                #endregion PropertyChanged

                public TreeviewDataModel ( ){
                        CreateData ( );
                }
                private void CreateData ( )
                {
                        // Top level class
                        Level1 . TopIndex= 1;
                        Level1 . TopLine = "Lorem Ipsum is simply dummy";
                        Toplevel. Add ( Level1 );
                        Level1 . TopIndex = 2;
                        Level1 . TopLine = "of the printing and typesetting";
                        Toplevel . Add ( Level1 );
                        Level1 . TopIndex = 3;
                        Level1 . TopLine = " It has survived not only five";
                        Toplevel . Add ( Level1 );

                        // Mid level class
                        Level2 . MidIndex = 1;
                        Level2 . MidLine= "industry. Lorem Ipsum has been the industry's";
                        Midlevel . Add ( Level2 );

                        // Base level class
                        Level3 . BaseIndex = 1;
                        Level3 . BaseLine= "1500s, when an unknown printer took";
                        Baselevel . Add ( Level3);
                }
        }

        public class TopLevel
        {
                public int TopIndex
                {
                        get; set;
                }
                public string TopLine
                {
                        get; set;
                }
        }
        public class MidLevel
        {
                public int MidIndex
                {
                        get; set;
                }
                public string MidLine
                {
                        get; set;
                }
        }
        public class BaseLevel
        {
                public int BaseIndex
                {
                        get; set;
                }
                public string BaseLine
                {
                        get; set;
                }
        }
}
