using System;
using System . Collections . Generic;
using System . Configuration;
using System . Data;
using System . Diagnostics;
using System . IO;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Controls;
using System . Windows . Media . Imaging;
using System . Windows . Media;
using System . Windows . Shapes;
using System . Windows;

using WPFPages . ViewModels;

using WPFPages . Views;
using System . Windows . Input;
using WPFPages . Properties;
using Microsoft . Win32;

namespace WPFPages
{
    public  class Library1
    {
        public static string trace ( string prompt = "" )
        {
            // logs all the calls made upwards in a tree
            string output = "", tmp = "";
            int indx = 1;
            var v = new StackTrace ( 0 );
            if ( prompt != "" )
                output = prompt + "\nStackTrace :\n";
            while ( true )
            {
                try
                {

                    tmp = v . GetFrame ( indx++ ) . GetMethod ( ) . Name + '\n';
                    if ( tmp . Contains ( "Invoke" ) || tmp . Contains ( "RaiseEvent" ) )
                        break;
                    else
                        output += tmp;
                }
                catch ( Exception ex )
                {
                    Console . WriteLine ( "Crashed...\n" );
                    output += "\nCrashed...\n";
                    break;
                }
            }
            Console . WriteLine ( $"\n{output}\n" );
            return $"\n{output}\n";
        }
        public static void Mbox ( Window win , string string1 = "" , string caption = "" , string string2 = "" , string iconstring = "" , int Btn1 = 1 , int Btn2 = 0 , int defButton = 1 )
        {
            // TODO  - reset this up
            // We NEED to remove any \r as part of \r\n as textboxes ONLY accept \n on its own for Cr/Lf
            //string1 = ParseforCR ( string1 );
            //Msgboxs m = new Msgboxs ( string1: string1 , string2: string2 , caption: caption , Btn1: Btn1 , Btn2: Btn2 , defButton: defButton , iconstring: iconstring );
            ////			m . Owner = win;
            //m . ShowDialog ( );
        }

        public static void Mssg (
                string caption = "" ,
                string string1 = "" ,
                string string2 = "" ,
                string string3 = "" ,
                string title = "" ,
                string iconstring = "" ,
                int defButton = 1 ,
                int Btn1 = 1 ,
                int Btn2 = 2 ,
                int Btn3 = 3 ,
                int Btn4 = 4 ,
                string btn1Text = "" ,
                string btn2Text = "" ,
                string btn3Text = "" ,
                string btn4Text = "" ,
                bool usedialog = true
                 )
        {
            Msgbox msg = new Msgbox (
                caption: caption ,
                string1: string1 ,
                string2: string2 ,
                string3: string3 ,
                title: title ,
                Btn1: Btn1 ,
                Btn2: Btn2 ,
                Btn3: Btn3 ,
                Btn4: Btn4 ,
                defButton: defButton ,
                iconstring: iconstring ,
                btn1Text: btn1Text ,
                btn2Text: btn2Text ,
                btn3Text: btn3Text ,
                btn4Text: btn4Text );
            //msg . Owner = win;
            if ( usedialog )
                msg . ShowDialog ( );
            else
                msg . Show ( );
        }

        private static string ParseforCR ( string input )
        {
            string output = "";
            if ( input . Length == 0 )
                return input;
            if ( input . Contains ( "\r\n" ) )
            {
                do
                {
                    string [ ] fields = input . Split ( '\r' );
                    foreach ( var item in fields )
                    {
                        output += item;
                    }
                    if ( output . Contains ( "\r" ) == false )
                        break;
                } while ( true );
            }
            else
                return input;
            return output;
        }
        public static Brush GetNewBrush ( string color )
        {
            if ( color [ 0 ] != '#' )
                color = "#" + color;
            return ( Brush ) new BrushConverter ( ) . ConvertFrom ( color );
        }

        public static Brush BrushFromColors ( Color color )
        {
            Brush brush = new SolidColorBrush ( color );
            return brush;
        }

        public static BankAccountViewModel CreateBankRecordFromString ( string type , string input )
        {
            int index = 0;
            BankAccountViewModel bvm = new BankAccountViewModel ( );
            char [ ] s = { ',' };
            string [ ] data = input . Split ( s );
            string donor = data [ 0 ];
            try
            {
                DateTime dt;
                if ( type == "BANK" || type == "DETAILS" )
                {
                    // This WORKS CORRECTLY 12/6/21 when called from n SQLDbViewer DETAILS grid entry && BANK grid entry					
                    // this test confirms the data layout by finding the Odate field correctly
                    // else it drops thru to the Catch branch
                    dt = Convert . ToDateTime ( data [ 7 ] );
                    //We can have any type of record in the string recvd
                    index = 1;  // jump the data type string
                    bvm . Id = int . Parse ( data [ index++ ] );
                    bvm . CustNo = data [ index++ ];
                    bvm . BankNo = data [ index++ ];
                    bvm . AcType = int . Parse ( data [ index++ ] );
                    bvm . IntRate = decimal . Parse ( data [ index++ ] );
                    bvm . Balance = decimal . Parse ( data [ index++ ] );
                    bvm . ODate = Convert . ToDateTime ( data [ index++ ] );
                    bvm . CDate = Convert . ToDateTime ( data [ index ] );
                    return bvm;
                }
                else if ( type == "CUSTOMER" )
                {
                    // this test confirms the data layout by finding the Odate field correctly
                    // else it drops thru to the Catch branch
                    dt = Convert . ToDateTime ( data [ 5 ] );
                    // We have a customer record !!
                    //Check to see if the data includes the data type in it
                    //As we have to parse it diffrently if not - see index....
                    index = 1;
                    bvm . Id = int . Parse ( data [ index++ ] );
                    bvm . CustNo = data [ index++ ];
                    bvm . BankNo = data [ index++ ];
                    bvm . AcType = int . Parse ( data [ index++ ] );
                    bvm . ODate = Convert . ToDateTime ( data [ index++ ] );
                    bvm . CDate = Convert . ToDateTime ( data [ index ] );
                }
                return bvm;
            }
            catch
            {
                //Check to see if the data includes the data type in it
                //As we have to parse it diffrently if not - see index....
                index = 0;
                try
                {
                    int x = int . Parse ( donor );
                    // if we get here, it IS a NUMERIC VALUE
                    index = 0;
                }
                catch
                {
                    //its probably the Data Type string, so ignore it for our Data creation processing
                    index = 1;
                }
                //We have a CUSTOMER record
                bvm . Id = int . Parse ( data [ index++ ] );
                bvm . CustNo = data [ index++ ];
                bvm . BankNo = data [ index++ ];
                bvm . AcType = int . Parse ( data [ index++ ] );
                bvm . ODate = Convert . ToDateTime ( data [ index++ ] );
                bvm . CDate = Convert . ToDateTime ( data [ index ] );
                return bvm;
            }
        }
        public static BankDragviewModel CreateBankGridRecordFromString ( string input )
        {
            int index = 1;
            string type = "";
            //			BankAccountViewModel bvm = new BankAccountViewModel ( );
            BankDragviewModel bvm = new BankDragviewModel ( );
            CustomerDragviewModel cvm = new CustomerDragviewModel ( );

            char [ ] s = { ',' };
            string [ ] data = input . Split ( s );
            string donor = data [ 0 ];
            try
            {
                DateTime dt;
                type = data [ 0 ];
                if ( type == "BANKACCOUNT" || type == "BANK" || type == "DETAILS" )
                {
                    // This WORKS CORRECTLY 12/6/21 when called from n SQLDbViewer DETAILS grid entry && BANK grid entry					
                    // this test confirms the data layout by finding the Odate field correctly
                    // else it drops thru to the Catch branch
                    dt = Convert . ToDateTime ( data [ 7 ] );
                    //We can have any type of record in the string recvd
                    index = 1;  // jump the data type string
                    bvm . RecordType = type;
                    bvm . Id = int . Parse ( data [ index++ ] );
                    bvm . CustNo = data [ index++ ];
                    bvm . BankNo = data [ index++ ];
                    bvm . AcType = int . Parse ( data [ index++ ] );
                    bvm . IntRate = decimal . Parse ( data [ index++ ] );
                    bvm . Balance = decimal . Parse ( data [ index++ ] );
                    bvm . ODate = Convert . ToDateTime ( data [ index++ ] );
                    bvm . CDate = Convert . ToDateTime ( data [ index ] );
                    return bvm;
                }
            }
            catch
            {
                //Check to see if the data includes the data type in it
                //As we have to parse it diffrently if not - see index....
                index = 0;
                try
                {
                    int x = int . Parse ( donor );
                    // if we get here, it IS a NUMERIC VALUE
                    index = 0;
                }
                catch ( Exception ex )
                {
                    //its probably the Data Type string, so ignore it for our Data creation processing
                    index = 1;
                }
                //We have a CUSTOMER record
                bvm . RecordType = type;
                bvm . Id = int . Parse ( data [ index++ ] );
                bvm . CustNo = data [ index++ ];
                bvm . BankNo = data [ index++ ];
                bvm . AcType = int . Parse ( data [ index++ ] );
                bvm . ODate = Convert . ToDateTime ( data [ index++ ] );
                bvm . CDate = Convert . ToDateTime ( data [ index ] );
                return bvm;
            }
            return bvm;
        }

        public static CustomerDragviewModel CreateCustGridRecordFromString ( string input )
        {
            int index = 0;
            string type = "";
            //			BankAccountViewModel bvm = new BankAccountViewModel ( );
            CustomerDragviewModel cvm = new CustomerDragviewModel ( );

            char [ ] s = { ',' };
            string [ ] data = input . Split ( s );
            string donor = data [ 0 ];
            try
            {
                DateTime dt;
                type = data [ 0 ];
                // this test confirms the data layout by finding the Dob field correctly
                // else it drops thru to the Catch branch
                dt = Convert . ToDateTime ( data [ 10 ] );
                // We have a customer record !!
                //Check to see if the data includes the data type in it
                //As we have to parse it diffrently if not - see index....
                index = 0;
                cvm . RecordType = type;
                cvm . Id = int . Parse ( data [ index++ ] );
                cvm . CustNo = data [ index++ ];
                cvm . BankNo = data [ index++ ];
                cvm . AcType = int . Parse ( data [ index++ ] );

                cvm . FName = data [ index++ ];
                cvm . LName = data [ index++ ];
                cvm . Town = data [ index++ ];
                cvm . County = data [ index++ ];
                cvm . PCode = data [ index++ ];

                cvm . Dob = Convert . ToDateTime ( data [ index++ ] );
                cvm . ODate = Convert . ToDateTime ( data [ index++ ] );
                cvm . CDate = Convert . ToDateTime ( data [ index ] );
                return cvm;
            }
            catch
            {
                //Check to see if the data includes the data type in it
                //As we have to parse it diffrently if not - see index....
                index = 0;
                try
                {
                    int x = int . Parse ( donor );
                    // if we get here, it IS a NUMERIC VALUE
                    index = 0;
                }
                catch ( Exception ex )
                {
                    //its probably the Data Type string, so ignore it for our Data creation processing
                    index = 1;
                }
            }
            return cvm;
        }

        public static CustomerViewModel CreateCustomerRecordFromString ( string input )
        {
            int index = 1;
            CustomerViewModel cvm = new CustomerViewModel ( );
            char [ ] s = { ',' };
            string [ ] data = input . Split ( s );
            string donor = data [ 0 ];
            //Check to see if the data includes the data type in it
            //As we have to parse it diffrently if not - see index....
            //if ( donor . Length > 3 )
            //        index =0 ;
            //We have the sender type in the string recvd
            cvm . Id = int . Parse ( data [ index++ ] );
            cvm . CustNo = data [ index++ ];
            cvm . BankNo = data [ index++ ];
            cvm . AcType = int . Parse ( data [ index++ ] );
            cvm . FName = data [ index++ ];
            cvm . LName = data [ index++ ];
            cvm . Addr1 = data [ index++ ];
            cvm . Addr2 = data [ index++ ];
            cvm . Town = data [ index++ ];
            cvm . County = data [ index++ ];
            cvm . PCode = data [ index++ ];
            cvm . Phone = data [ index++ ];
            cvm . Mobile = data [ index++ ];
            cvm . Dob = DateTime . Parse ( data [ index++ ] );
            cvm . ODate = DateTime . Parse ( data [ index++ ] );
            cvm . CDate = DateTime . Parse ( data [ index ] );
            return cvm;
        }
        public static DetailsViewModel CreateDetailsRecordFromString ( string input )
        {
            int index = 0;
            DetailsViewModel bvm = new DetailsViewModel ( );
            char [ ] s = { ',' };
            string [ ] data = input . Split ( s );
            string donor = data [ 0 ];
            //Check to see if the data includes the data type in it
            //As we have to parse it diffrently if not - see index....
            if ( donor . Length > 3 )
                index = 1;
            bvm . Id = int . Parse ( data [ index++ ] );
            bvm . CustNo = data [ index++ ];
            bvm . BankNo = data [ index++ ];
            bvm . AcType = int . Parse ( data [ index++ ] );
            bvm . IntRate = decimal . Parse ( data [ index++ ] );
            bvm . Balance = decimal . Parse ( data [ index++ ] );
            bvm . ODate = DateTime . Parse ( data [ index++ ] );
            bvm . CDate = DateTime . Parse ( data [ index ] );
            return bvm;
        }
        public static string CreateFullCsvTextFromRecord ( BankAccountViewModel bvm , DetailsViewModel dvm , CustomerViewModel cvm = null , bool IncludeType = true )
        {
            if ( bvm == null && cvm == null && dvm == null )
                return "";
            string datastring = "";
            if ( bvm != null )
            {
                // Handle a BANK Record
                if ( IncludeType )
                    datastring = "BANKACCOUNT";
                datastring += bvm . Id + ",";
                datastring += bvm . CustNo + ",";
                datastring += bvm . BankNo + ",";
                datastring += bvm . AcType . ToString ( ) + ",";
                datastring += bvm . IntRate . ToString ( ) + ",";
                datastring += bvm . Balance . ToString ( ) + ",";
                datastring += "'" + bvm . CDate . ToString ( ) + "',";
                datastring += "'" + bvm . ODate . ToString ( ) + "',";
            }
            else if ( dvm != null )
            {
                if ( IncludeType )
                    datastring = "DETAILS,";
                datastring += dvm . Id + ",";
                datastring += dvm . CustNo + ",";
                datastring += dvm . BankNo + ",";
                datastring += dvm . AcType . ToString ( ) + ",";
                datastring += dvm . IntRate . ToString ( ) + ",";
                datastring += dvm . Balance . ToString ( ) + ",";
                datastring += "'" + dvm . CDate . ToString ( ) + "',";
                datastring += dvm . ODate . ToString ( ) + ",";
            }
            else if ( cvm != null )
            {
                if ( IncludeType )
                    datastring = "CUSTOMER,";
                datastring += cvm . Id + ",";
                datastring += cvm . CustNo + ",";
                datastring += cvm . BankNo + ",";
                datastring += cvm . AcType . ToString ( ) + ",";
                datastring += "'" + cvm . CDate . ToString ( ) + "',";
                datastring += cvm . ODate . ToString ( ) + ",";
            }
            return datastring;
        }
        public static string CreateDragDataFromRecord ( BankDragviewModel bvm )
        {
            if ( bvm == null )
                return "";
            string datastring = "";
            datastring = bvm . RecordType + ",";
            datastring += bvm . Id + ",";
            datastring += bvm . CustNo + ",";
            datastring += bvm . BankNo + ",";
            datastring += bvm . AcType . ToString ( ) + ",";
            datastring += bvm . IntRate . ToString ( ) + ",";
            datastring += bvm . Balance . ToString ( ) + ",";
            datastring += "'" + bvm . CDate . ToString ( ) + "',";
            datastring += "'" + bvm . ODate . ToString ( ) + "',";
            return datastring;
        }



        #region System Settings helpers
        public static void SaveProperty ( string setting , string value )
        {
            try
            {
                if ( value . ToUpper ( ) . Contains ( "TRUE" ) )
                    Settings . Default [ setting ] = true;
                else if ( value . ToUpper ( ) . Contains ( "FALSE" ) )
                    Settings . Default [ setting ] = false;
                else
                    Settings . Default [ setting ] = value;
                Settings . Default . Save ( );
                Settings . Default . Upgrade ( );
                ConfigurationManager . RefreshSection ( setting );
            }
            catch ( Exception ex )
            {
                Debug . WriteLine ( $"Unable to save property {setting} of [{value}]\nError was {ex . Data}, {ex . Message}, Stack trace = \n{ex . StackTrace}" );
            }
        }

        public static void AddUpdateAppSettings ( string key , string value )
        {
            try
            {
                var configFile = ConfigurationManager . OpenExeConfiguration ( ConfigurationUserLevel . None );
                var settings = configFile . AppSettings . Settings;
                if ( settings [ key ] == null )
                {
                    settings . Add ( key , value );
                }
                else
                {
                    settings [ key ] . Value = value;
                }
                configFile . Save ( ConfigurationSaveMode . Full );
                ConfigurationManager . RefreshSection ( configFile . AppSettings . SectionInformation . Name );
            }
            catch ( ConfigurationErrorsException )
            {
                Console . WriteLine ( "Error writing app settings" );
            }
        }

        public static string OpenFileDlg ( string filespec = "" )
        {
            OpenFileDialog ofd = new OpenFileDialog ( );
            ofd . InitialDirectory = @"C:\";
            ofd . CheckFileExists = true;
            ofd . Filter = "All Files (*.*) | *.*";
            ofd . AddExtension = true;
            //if ( filespec == "" )
            //	ofd . DefaultExt = ".XLS*" ;
            //else
            //	ofd . DefaultExt = $".{filespec . ToUpper ( )}" ;
            ofd . ShowDialog ( );
            return ofd . FileName;
        }

        public static void HandleCtrlFnKeys ( bool key1 , System.Windows.Input.KeyEventArgs e )
        {
            if ( key1 && e . Key == Key . F5 )
            {
                // list Flags in Console
                Utils . GetWindowHandles ( );
                e . Handled = true;
                key1 = false;
                return;
            }
            else if ( key1 && e . Key == Key . F6 )  // CTRL + F6
            {
                // list various Flags in Console
                Debug . WriteLine ( $"\nCTRL + F6 pressed..." );
                Flags . UseBeeps = !Flags . UseBeeps;
                e . Handled = true;
                key1 = false;
                Debug . WriteLine ( $"Flags.UseBeeps reset to  {Flags . UseBeeps}" );
                return;
            }
            else if ( key1 && e . Key == Key . F7 )  // CTRL + F7
            {
                // list various Flags in Console
                Debug . WriteLine ( $"\nCTRL + F7 pressed..." );
                Flags . PrintDbInfo ( );
                e . Handled = true;
                key1 = false;
                return;
            }
            else if ( key1 && e . Key == Key . F8 )     // CTRL + F8
            {
                Debug . WriteLine ( $"\nCTRL + F8 pressed..." );
                EventHandlers . ShowSubscribersCount ( );
                e . Handled = true;
                key1 = false;
                return;
            }
            else if ( key1 && e . Key == Key . F9 )     // CTRL + F9
            {
                Debug . WriteLine ( "\nCtrl + F9 NOT Implemented" );
                key1 = false;
                return;

            }
            else if ( key1 && e . Key == Key . System )     // CTRL + F10
            {
                // Major  listof GV[] variables (Guids etc]
                Debug . WriteLine ( $"\nCTRL + F10 pressed..." );
                Flags . ListGridviewControlFlags ( 1 );
                key1 = false;
                e . Handled = true;
                return;
            }
            else if ( key1 && e . Key == Key . F11 )  // CTRL + F11
            {
                // list various Flags in Console
                Debug . WriteLine ( $"\nCTRL + F11 pressed..." );
                Flags . PrintSundryVariables ( );
                e . Handled = true;
                key1 = false;
                return;
            }
        }

        /// <summary>
        /// A Func that takes ANY 2 (of 3 [Bank,Customer,Details] Db type records and returns true if the CustNo and Bankno match
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool CompareDbRecords ( object obj1 , object obj2 )
        {
            bool result = false;
            BankAccountViewModel bvm = new BankAccountViewModel ( );
            CustomerViewModel cvm = new CustomerViewModel ( );
            DetailsViewModel dvm = new DetailsViewModel ( );
            //bvm = null;
            //cvm = null;
            //dvm = null;
            if ( obj1 == null || obj2 == null )
                return result;
            if ( obj1 . GetType ( ) == bvm . GetType ( ) )
                bvm = obj1 as BankAccountViewModel;
            if ( obj1 . GetType ( ) == cvm . GetType ( ) )
                cvm = obj1 as CustomerViewModel;
            if ( obj1 . GetType ( ) == dvm . GetType ( ) )
                dvm = obj1 as DetailsViewModel;

            if ( obj2 . GetType ( ) == bvm . GetType ( ) )
                bvm = obj2 as BankAccountViewModel;
            if ( obj2 . GetType ( ) == cvm . GetType ( ) )
                cvm = obj2 as CustomerViewModel;
            if ( obj2 . GetType ( ) == dvm . GetType ( ) )
                dvm = obj2 as DetailsViewModel;

            if ( bvm != null && cvm != null )
            {
                if ( bvm . CustNo == cvm . CustNo )
                    result = true;
            }
            else if ( bvm != null && dvm != null )
            {
                if ( bvm . CustNo == dvm . CustNo )
                    result = true;
            }
            else if ( cvm != null && dvm != null )
            {
                if ( cvm . CustNo == dvm . CustNo )
                    result = true;
            }
            result = false;
            return result;
        }

        public static bool CheckRecordMatch (
              BankAccountViewModel bvm ,
              CustomerViewModel cvm ,
              DetailsViewModel dvm )
        {
            bool result = false;
            if ( bvm != null && cvm != null )
            {
                if ( bvm . CustNo == cvm . CustNo )
                    result = true;
            }
            else if ( bvm != null && dvm != null )
            {
                if ( bvm . CustNo == dvm . CustNo )
                    result = true;
            }
            else if ( cvm != null && dvm != null )
            {
                if ( cvm . CustNo == dvm . CustNo )
                    result = true;
            }
            return result;
        }

        public static void SetUpGridSelection ( DataGrid grid , int row = 0 )
        {
            //			bool inprogress = false;
            //			int scrollrow = 0;
            if ( row == -1 )
                row = 0;
            // This triggers the selection changed event
            grid . SelectedIndex = row;
            grid . SelectedItem = row;
            //			grid . SetDetailsVisibilityForItem ( grid . SelectedItem, Visibility . Visible );
            grid . SelectedIndex = row;
            grid . SelectedItem = row;
            Utils . ScrollRecordIntoView ( grid , row );
            grid . UpdateLayout ( );
            grid . Refresh ( );
            //			var v = grid .VerticalAlignment;
        }
        public static void SetUpGListboxSelection ( ListBox grid , int row = 0 )
        {
            //			bool inprogress = false;
            //			int scrollrow = 0;
            if ( row == -1 )
                row = 0;
            // This triggers the selection changed event
            grid . SelectedIndex = row;
            grid . SelectedItem = row;
            //			grid . SetDetailsVisibilityForItem ( grid . SelectedItem, Visibility . Visible );
            grid . SelectedIndex = row;
            grid . SelectedItem = row;
            Utils . ScrollLBRecordIntoView ( grid , row );
            grid . UpdateLayout ( );
            grid . Refresh ( );
            //			var v = grid .VerticalAlignment;
        }

        /// <summary>
        /// Metohd that almost GUARANTESS ot force a record into view in any DataGrid
        /// /// This is called by method above - MASTER Updater Method
        /// </summary>
        /// <param name="dGrid"></param>
        /// <param name="row"></param>
        public static void ScrollRecordInGrid ( DataGrid dGrid , int row )
        {
            if ( dGrid . CurrentItem == null )
                return;
            dGrid . UpdateLayout ( );
            dGrid . ScrollIntoView ( dGrid . Items . Count - 1 );
            dGrid . UpdateLayout ( );
            dGrid . ScrollIntoView ( row );
            dGrid . UpdateLayout ( );
            Utils . ScrollRecordIntoView ( dGrid , row );
        }
        public static int FindMatchingRecord ( string Custno , string Bankno , DataGrid Grid , string currentDb = "" )
        {
            int index = 0;
            if ( currentDb == "BANKACCOUNT" )
            {
                foreach ( var item in Grid . Items )
                {
                    BankAccountViewModel cvm = item as BankAccountViewModel;
                    if ( cvm == null )
                        break;
                    if ( cvm . CustNo == Custno && cvm . BankNo == Bankno )
                    {
                        break;
                    }
                    index++;
                }
                if ( index == Grid . Items . Count )
                    index = -1;
                return index;
            }
            else if ( currentDb == "CUSTOMER" )
            {
                foreach ( var item in Grid . Items )
                {
                    CustomerViewModel cvm = item as CustomerViewModel;
                    if ( cvm == null )
                        break;
                    if ( cvm . CustNo == Custno && cvm . BankNo == Bankno )
                    {
                        break;
                    }
                    index++;
                }
                if ( index == Grid . Items . Count )
                    index = -1;
                return index;
            }
            else if ( currentDb == "DETAILS" )
            {
                foreach ( var item in Grid . Items )
                {
                    DetailsViewModel dvm = item as DetailsViewModel;
                    if ( dvm == null )
                        break;
                    if ( dvm . CustNo == Custno && dvm . BankNo == Bankno )
                    {
                        break;
                    }
                    index++;
                }
                if ( index == Grid . Items . Count )
                    index = -1;
                return index;
            }
            return -1;
        }

        public static void ScrollRecordIntoView ( DataGrid Dgrid , int CurrentRecord )
        {
            // Works well 26/5/21
            double currentTop = 0;
            double currentBottom = 0;
            if ( CurrentRecord == -1 )
                return;
            if ( Dgrid . Name == "CustomerGrid" || Dgrid . Name == "DataGrid1" )
            {
                currentTop = Flags . TopVisibleBankGridRow;
                currentBottom = Flags . BottomVisibleBankGridRow;
            }
            else if ( Dgrid . Name == "BankGrid" || Dgrid . Name == "DataGrid2" )
            {
                currentTop = Flags . TopVisibleCustGridRow;
                currentBottom = Flags . BottomVisibleCustGridRow;
            }
            else if ( Dgrid . Name == "DetailsGrid" || Dgrid . Name == "DetailsGrid" )
            {
                currentTop = Flags . TopVisibleDetGridRow;
                currentBottom = Flags . BottomVisibleDetGridRow;
            }     // Believe it or not, it takes all this to force a scrollinto view correctly

            if ( Dgrid == null || Dgrid . Items . Count == 0 || Dgrid . SelectedItem == null )
                return;

            //update and scroll to bottom first
            Dgrid . SelectedIndex = ( int ) CurrentRecord;
            Dgrid . SelectedItem = ( int ) CurrentRecord;
            Dgrid . UpdateLayout ( );
            Dgrid . ScrollIntoView ( Dgrid . Items . Count - 1 );
            Dgrid . UpdateLayout ( );
            Dgrid . ScrollIntoView ( Dgrid . SelectedItem );
            Dgrid . UpdateLayout ( );
            Flags . CurrentSqlViewer?.SetScrollVariables ( Dgrid );
        }

        public static void ScrollLBRecordIntoView ( ListBox Dgrid , int CurrentRecord )
        {
            // Works well 26/5/21

            //update and scroll to bottom first
            Dgrid . SelectedIndex = ( int ) CurrentRecord;
            Dgrid . SelectedItem = ( int ) CurrentRecord;
            Dgrid . UpdateLayout ( );
            Dgrid . ScrollIntoView ( Dgrid . Items . Count - 1 );
            Dgrid . UpdateLayout ( );
            Dgrid . ScrollIntoView ( Dgrid . SelectedItem );
            Dgrid . UpdateLayout ( );
        }

        public static string GetDataSortOrder ( string commandline )
        {
            if ( Flags . SortOrderRequested == ( int ) Flags . SortOrderEnum . DEFAULT )
                commandline += "Custno, BankNo";
            else if ( Flags . SortOrderRequested == ( int ) Flags . SortOrderEnum . ID )
                commandline += "ID";
            else if ( Flags . SortOrderRequested == ( int ) Flags . SortOrderEnum . BANKNO )
                commandline += "BankNo, CustNo";
            else if ( Flags . SortOrderRequested == ( int ) Flags . SortOrderEnum . CUSTNO )
                commandline += "CustNo";
            else if ( Flags . SortOrderRequested == ( int ) Flags . SortOrderEnum . ACTYPE )
                commandline += "AcType";
            else if ( Flags . SortOrderRequested == ( int ) Flags . SortOrderEnum . DOB )
                commandline += "Dob";
            else if ( Flags . SortOrderRequested == ( int ) Flags . SortOrderEnum . ODATE )
                commandline += "Odate";
            else if ( Flags . SortOrderRequested == ( int ) Flags . SortOrderEnum . CDATE )
                commandline += "Cdate";
            return commandline;
        }

        public static bool CheckForExistingGuid ( Guid guid )
        {
            bool retval = false;
            for ( int x = 0 ; x < Flags . DbSelectorOpen . ViewersList . Items . Count ; x++ )
            {
                ListBoxItem lbi = new ListBoxItem ( );
                //lbi.Tag = viewer.Tag;
                lbi = Flags . DbSelectorOpen . ViewersList . Items [ x ] as ListBoxItem;
                if ( lbi . Tag == null )
                    return retval;
                Guid g = ( Guid ) lbi . Tag;
                if ( g == guid )
                {
                    retval = true;
                    break;
                }
            }
            return retval;
        }

        public static void Grab_MouseMove ( object sender , MouseEventArgs e )
        {
            Point pt = e . GetPosition ( ( UIElement ) sender );
            HitTestResult hit = VisualTreeHelper . HitTest ( ( Visual ) sender , pt );
            if ( hit . VisualHit != null )
            {
                if ( Utils.ControlsHitList . Count != 0 )
                {
                    if ( hit . VisualHit == Utils . ControlsHitList [ 0 ] . VisualHit )
                        return;
                }
                Utils . ControlsHitList . Clear ( );
                Utils . ControlsHitList . Add ( hit );
            }
        }
        public static void Grabscreen ( Window parent , object obj , GrabImageArgs args , Control ctrl = null )
        {
            UIElement ui = obj as UIElement;
            UIElement OBJ = new UIElement ( );
            int indx = 0;
            bool success = false;
            // try to step up the visual tree ?
            do
            {
                indx++;
                if ( indx > 30 || indx < 0 )
                    break;
                switch ( indx )
                {
                    case 1:
                        OBJ = FindVisualParent<DataGrid> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 2:
                        OBJ = FindVisualParent<Button> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 3:
                        OBJ = FindVisualParent<Slider> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 4:
                        OBJ = FindVisualParent<ListView> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 5:
                        OBJ = FindVisualParent<ListBox> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 6:
                        OBJ = FindVisualParent<ComboBox> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 7:
                        OBJ = FindVisualParent<WrapPanel> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 8:
                        OBJ = FindVisualParent<CheckBox> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 9:
                        OBJ = FindVisualParent<DataGridRow> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 10:
                        OBJ = FindVisualParent<DataGridCell> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 11:
                        OBJ = FindVisualParent<Canvas> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 12:
                        OBJ = FindVisualParent<GroupBox> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 13:
                        OBJ = FindVisualParent<ProgressBar> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 14:
                        OBJ = FindVisualParent<Ellipse> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 15:
                        OBJ = FindVisualParent<RichTextBox> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 16:
                        OBJ = FindVisualParent<TextBlock> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 17:
                        //if(ui Equals TextBox)
                        if ( ui == null )
                            break;
                        DependencyObject v = new DependencyObject ( );
                        DependencyObject prev = new DependencyObject ( );
                        //OBJ = FindVisualParent<TextBox> ( ui );
                        do
                        {
                            v = VisualTreeHelper . GetParent ( ui );
                            if ( v == null )
                                break;
                            prev = v;
                            TextBox tb = v as TextBox;
                            if ( tb != null && tb . Text . Length > 0 )
                            {
                                Console . WriteLine ( $"UI = {tb . Text}" );
                                OBJ = tb as UIElement;
                                success = true;
                                break;
                            }
                            Console . WriteLine ( $"UI = {v . ToString ( )}" );
                            //							if ( v . ToString ( ) . Contains ( ".TextBox" ) )
                            //if ( v . ToString ( ) . Contains ( ".TextBox" ) )
                            //{
                            //	OBJ = prev as UIElement;
                            //	success = true;
                            //	break;
                            //}
                            //else
                            ui = v as UIElement;
                        } while ( true );
                        break;
                    case 18:
                        OBJ = FindVisualParent<ContentPresenter> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 19:
                        OBJ = FindVisualParent<Grid> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 20:
                        OBJ = FindVisualParent<Window> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 21:
                        OBJ = FindVisualParent<ScrollContentPresenter> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    case 22:
                        OBJ = FindVisualParent<Rectangle> ( ui );
                        if ( OBJ != null )
                            success = true;
                        break;
                    //case 23:
                    //	OBJ = FindVisualParent<TextBoxLineDrawingVisual> ( ui );
                    //	if ( OBJ != null )
                    //		success = true;
                    //	break;
                    default:
                        OBJ = ui;
                        success = false;
                        break;
                }
                if ( success == true && OBJ != null )
                    break;
            } while ( true );
            if ( success == false )
                return;
            Console . WriteLine ( $"Element Identified for display = : [{OBJ . ToString ( )}]" );
            //string str = OBJ . ToString ( );
            //if ( str . Contains ( ".TextBox" ) )
            //{
            //	var v  = OBJ.GetType();
            //	Console . WriteLine ( $"Type is {v}" );
            //}
            ////OBJ = OBJ . Text;
            var bmp = Utils . CreateControlImage ( OBJ as FrameworkElement , @"J:\\users\ianch\Documents\capturedimage.png" );
            if ( bmp == null )
                return;
            Grabviewer gv = new Grabviewer ( parent , ctrl , bmp );
            //Setup the  image in our viewer
            gv . Grabimage . Source = bmp;
            gv . Title = @"C:\users\ianch\documents\Grabimage.png";
            gv . Title += $"  :  RESIZE window to magnify the contents ...";
            gv . Show ( );
            // Save to disk file
            Utils . SaveImageToFile ( ( RenderTargetBitmap ) bmp , @"C:\users\ianch\documents\Grabimage.png" , "PNG" );
        }
        public static T FindVisualParent<T> ( UIElement element ) where T : UIElement
        {
            UIElement parent = element;
            while ( parent != null )
            {
                var correctlyTyped = parent as T;
                if ( correctlyTyped != null )
                {
                    return correctlyTyped;
                }
                parent = VisualTreeHelper . GetParent ( parent ) as UIElement;
            }
            return null;
        }

        public static parentItem FindVisualParent<parentItem> ( DependencyObject obj ) where parentItem : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper . GetParent ( obj );
            while ( parent != null && !parent . GetType ( ) . Equals ( typeof ( parentItem ) ) )
            {
                parent = VisualTreeHelper . GetParent ( parent );
            }
            return parent as parentItem;
        }

        public static RenderTargetBitmap CreateControlImage ( FrameworkElement control , string filename = "" , bool savetodisk = false , GrabImageArgs ga = null )
        {
            if ( control == null )
                return null;
            // Get the Visual (Control) itself and the size of the Visual and its descendants.
            // This is the clever bit that gets the requested control, not the full window
            Rect rect = VisualTreeHelper . GetDescendantBounds ( control );

            // Make a DrawingVisual to make a screen
            // representation of the control.
            DrawingVisual dv = new DrawingVisual ( );

            // Fill a rectangle the same size as the control
            // with a brush containing images of the control.
            using ( DrawingContext ctx = dv . RenderOpen ( ) )
            {
                VisualBrush brush = new VisualBrush ( control );
                ctx . DrawRectangle ( brush , null , new Rect ( rect . Size ) );
            }

            // Make a bitmap and draw on it.
            int width = ( int ) control . ActualWidth;
            int height = ( int ) control . ActualHeight;
            if ( height == 0 || width == 0 )
                return null;
            RenderTargetBitmap rtb = new RenderTargetBitmap ( width , height , 96 , 96 , PixelFormats . Pbgra32 );
            rtb . Render ( dv );
            if ( savetodisk && filename != "" )
                SaveImageToFile ( rtb , filename );
            return rtb;
        }

        // allows any image to be saved as PNG/GIF/JPG format, defaullt is PNG
        public static void SaveImageToFile ( RenderTargetBitmap bmp , string file , string imagetype = "PNG" )
        {
            string [ ] items;
            // Make a PNG encoder.
            if ( bmp == null )
                return;
            if ( file == "" && imagetype != "" )
                file = @"J:\users\ianch\pictures\defaultimage";
            items = file . Split ( '.' );
            file = items [ 0 ];
            if ( imagetype == "PNG" )
                file += ".png";
            else if ( imagetype == "GIF" )
                file += ".gif";
            else if ( imagetype == "JPG" )
                file += ".jpg";
            using ( FileStream fs = new FileStream ( file ,
                        FileMode . Create , FileAccess . Write , FileShare . None ) )
            {
                if ( imagetype == "PNG" )
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder ( );
                    encoder . Frames . Add ( BitmapFrame . Create ( bmp ) );
                    encoder . Save ( fs );
                }
                else if ( imagetype == "GIF" )
                {
                    GifBitmapEncoder encoder = new GifBitmapEncoder ( );
                    encoder . Frames . Add ( BitmapFrame . Create ( bmp ) );
                    encoder . Save ( fs );
                }
                else if ( imagetype == "JPG" || imagetype == "JPEG" )
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder ( );
                    encoder . Frames . Add ( BitmapFrame . Create ( bmp ) );
                    encoder . Save ( fs );
                }
            }
        }
        #endregion Bitmap creation/Save methods
        public static void LoadBankDbGeneric ( BankCollection bvm , string caller = "" , bool Notify = false , int lowvalue = -1 , int highvalue = -1 , int maxrecords = -1 )
        {
            if ( maxrecords == -1 )
                BankCollection . LoadBank ( cc: bvm , caller: "BankAccount" , ViewerType: 99 , NotifyAll: Notify );
            else
            {
                DataTable dtBank = new DataTable ( );
                dtBank = BankCollection . LoadSelectedBankData ( Min: lowvalue , Max: highvalue , Tot: maxrecords );
                bvm = BankCollection . LoadSelectedCollection ( bankCollection: bvm , max: -1 , dtBank: dtBank , Notify: Notify );
            }

        }
        public static void SelectTextBoxText ( TextBox txtbox )
        {
            txtbox . SelectionLength = txtbox . Text . Length;
            txtbox . SelectionStart = 0;
            txtbox . SelectAll ( );
        }

        public static void SetupWindowDrag ( Window inst )
        {
            try
            {
                //Handle the button NOT being the left mouse button
                // which will crash the DragMove Fn.....
                MouseButtonState mbs = Mouse . RightButton;
                if ( mbs == MouseButtonState . Pressed )
                    return;
                inst . MouseDown += delegate
                {
                    {
                        try
                        {
                            inst?.DragMove ( );
                        }
                        catch ( Exception ex )
                        {
                            return;
                        }
                    }
                };
            }
            catch ( Exception ex )
            {
                return;
            }
        }
        public static void SetGridRowSelectionOn ( DataGrid dgrid , int index )
        {
            if ( dgrid . Items . Count > 0 && index != -1 )
            {
                try
                {
                    //Setup new selected index
                    dgrid . SelectedIndex = index;
                    dgrid . SelectedItem = index;
                    dgrid . UpdateLayout ( );
                    dgrid . BringIntoView ( );
                    object obj = dgrid . Items [ index ];
                    if ( obj . GetType ( ) == typeof ( BankAccountViewModel ) )
                    {
                        BankAccountViewModel item = dgrid . Items [ index ] as BankAccountViewModel;
                        dgrid . ScrollIntoView ( item );
                    }
                    else if ( obj . GetType ( ) == typeof ( CustomerViewModel ) )
                    {
                        CustomerViewModel item = dgrid . Items [ index ] as CustomerViewModel;
                        dgrid . ScrollIntoView ( item );
                    }
                    else if ( obj . GetType ( ) == typeof ( GenericClass ) )
                    {
                        GenericClass item = dgrid . Items [ index ] as GenericClass;
                        dgrid . ScrollIntoView ( item );
                    }
                }
                catch ( Exception ex )
                {
                    Debug . WriteLine ( $"{ex . Message}" );
                }
            }
        }


    }
}
