#pragma warning disable CS0234

using System;
using System . CodeDom;
using System . Collections;
using System . Collections . Generic;
using System . ComponentModel;
using System . Data;
using System . Data . Linq;
using System . Globalization;
using System . Security . AccessControl;
using System . Security . Cryptography;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Markup;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using System . Xml . Linq;

using WPFPages . Commands;
using WPFPages . Converts;
using WPFPages . UserControls;

using static WPFPages . Views . ButtonTesting;

namespace WPFPages . Views
{
        /// <summary>
        /// Interaction logic for ButtonTesting.xaml
        /// </summary>
        public partial class ButtonTesting : Window
        {
                public static bool IdentifyMouseOver = false;

                public delegate bool MouseCheck ( MouseEventArgs e, HitControl Hitctrl, string argname, object argobject );

        #region FULL Properties

                BreakCommand Mynewcommand = new BreakCommand ( );

                 public BreakCommand MynewCommand
                {
                        get
                        {
                                return Mynewcommand;
                        }
                        set
                        {
                                Mynewcommand = value;
                        }
                }

                private DependencyObject objectmembersname;
                public DependencyObject ObjectMembersName
                {
                        get
                        {
                                return objectmembersname;
                        }
                        set
                        {
                                objectmembersname = value as DependencyObject;
                                OnPropertyChanged ( ObjectMembersName . ToString ( ) );
                        }
                }

                private string controlHit;
                public string ActiveHitName
                {
                        get
                        {
                                return controlHit;
                        }
                        set
                        {
                                controlHit = value;
                                OnPropertyChanged ( ActiveHitName . ToString ( ) );
                        }
                }
                #endregion

                /// <summary>
                /// Structure to control hit testing of objects in any window
                /// IsHitActive = Find / identify control specified by : string ActiveHitName
                /// GertObject = Return breakdown of entire object specified
                /// </summary>
                /// <param name="IsHitActive"></param>
                /// <param name="GetObject"></param>
                /// <returns>Controls access to the Object find systemr</returns>
                public static HitControl Hitcontrol = new HitControl ( );
                public string commandtest
                {
                        get; set;
                }
                public bool LockOutput = false;
                /// <summary>
                /// Constructor
                /// </summary>
                public ButtonTesting ( )
                {
                        this . DataContext = this;
                        InitializeComponent ( );
                        Utils . SetupWindowDrag ( this );

                        // initialize the hits class
                        Hitcontrol = HitControl . _Hitcontrol;
                        Hitcontrol . names = new List<string> ( );
                        Hitcontrol . IsHitActive = false;
                        Hitcontrol . ActiveHitName = "";
                        Hitcontrol . ObjectMembersName = null;
                        Hitcontrol . ActiveHitsObject = null;
                        Hitcontrol . GetObject = false;
                        Hitcontrol . ShowAll = true;
                        // Examples of using color converter from C#
                        //Converter name is StringtoColorConverter 
                        StringtoColorConverter sc = new StringtoColorConverter ( );
                        var c = sc . Convert ( null, typeof ( SolidColorBrush ), "Magenta1", CultureInfo . CurrentCulture );
                        SolidColorBrush colr = ( SolidColorBrush ) c;
                        var l = sc . Convert ( null, typeof ( LinearGradientBrush ), "HeaderBrushGreen", CultureInfo . CurrentCulture );
                        LinearGradientBrush lgb = ( LinearGradientBrush ) l;
                        var clr = sc . Convert ( null, typeof ( Color ), "ClrGreen1", CultureInfo . CurrentCulture );
                        Color cl = ( Color ) clr;

                        BitmapImage bi = new BitmapImage ( );
                        bi . Rotation = Rotation . Rotate90;
                        Image im = new Image ( );
                        var v = new Uri ( "/icons.right-arrow.png", UriKind . Relative );
                        var bitmap = new BitmapImage ( v );

                        TestWin2 . Children . Add ( im );
                        im . Source = bitmap;
                        im . Width = 45;
                        im . Height = 45;
                        im . Name = "NewImage";
                        im . UpdateLayout ( );

                        // Subclass the MouseMove event
                        MouseMove += ButtonTesting_MouseMove;
                        //Declare a new delegate for our mouse handlers

                        commandtest = "12345 67890";
                }

                private void Window_MouseEnter ( object sender, MouseEventArgs e )
                {
                }
                public static Point GetActiveHitName ( FrameworkElement control, FrameworkElement Host, MouseEventArgs e, out bool hit )
                {
                        Point result = new Point ( 0, 0 );
                        hit = false;
                        result = e . GetPosition ( Host );
                        double left = control . Margin . Left;
                        double right = control . Margin . Left + control . ActualWidth;
                        double top = Host . Margin . Top;
                        double bottom = control . Margin . Top + control . ActualHeight;
                        if ( result . X >= left )
                        {
                                if ( result . X <= right )
                                {
                                        if ( result . Y >= top )
                                        {
                                                if ( result . Y <= bottom )
                                                {
                                                        hit = true;
                                                }
                                        }
                                }
                        }
                        return result;
                }


            /// <summary>
            /// This is a delegate that is passed from MouseMove method 
            ///*************************************************************************************************************//
            /// ================= This is the main logic  for the mousemove system====================================//
            ///*************************************************************************************************************//
            /// </summary>
            /// <param name="e"></param>
            /// <param name="hitcontrol"></param>
            /// <returns></returns>
            public bool DoMouseCheck ( MouseEventArgs e, HitControl hitcontrol, string argname = "", object argobject = null )
                {
                        string controltarget = "";
                        if ( !LockOutput )
                                currentpoint . Items . Clear ( );
                        if ( Hitcontrol . IsHitActive )
                        {
                                bool x = false;
                                controltarget = Hitcontrol . ActiveHitName;
                                Hitcontrol . names . Clear ( );

                                // Get list of parent controls
                                Hitcontrol . names = MousoverHandlers . GetHierarchy ( e );
                                if ( Hitcontrol . ShowAll )
                                        ListAllMouseoverTargets ( Hitcontrol . ActiveHitName . ToUpper ( ) );
                                //Check if the control specified is hit (Test IS CASE Sensitive)
                                string s = MousoverHandlers . FindHierarchyParent ( Hitcontrol . names, Hitcontrol . ActiveHitName );
                                //output to screen TextBlock if we are over  the requeted control
                                if ( s != "" )
                                {
                                        if ( s . Contains ( ":" ) )
                                        {
                                                argname = argname . Trim ( );
                                                string temp = s . Substring ( 0, s . IndexOf ( ':' ) );
                                                if ( temp != "" )
                                                {
                                                        bool result = false;
                                                        string [ ] items = s . Split ( '.' );
                                                        foreach ( var item in items )
                                                        {
                                                                if ( item . Contains ( ":" ) )
                                                                {
                                                                        string [ ] inner = item . Split ( ':' );
                                                                        foreach ( var item2 in inner )
                                                                        {
                                                                                if ( item2 . Contains ( ":" ) )
                                                                                {
                                                                                        string [ ] items3 = s . Split ( ':' );
                                                                                        foreach ( var item3 in items3 )
                                                                                        {
                                                                                                if ( item3.Trim() == argname )
                                                                                                {
                                                                                                        result = true;
                                                                                                        break;
                                                                                                }
                                                                                        }
                                                                                        if ( item2 . Trim ( ) == argname )
                                                                                        {
                                                                                                result = true;
                                                                                                break;
                                                                                        }
                                                                                }
                                                                                else
                                                                                {
                                                                                        if ( item2.Trim() == argname )
                                                                                        {
                                                                                                result = true;
                                                                                                break;
                                                                                        }
                                                                                }
                                                                                if ( result && item . Trim ( ) == argname )
                                                                                {
                                                                                        result = true;
                                                                                        break;
                                                                                }
                                                                        }
                                                                        return result;
                                                                }
                                                                else
                                                                {
                                                                        if ( item == argname )
                                                                        {
                                                                                result = true;
                                                                                break;
                                                                        }
                                                                }
                                                        }
                                                        return result;
                                                }
                                                else
                                                {
                                                        bool result = false;
                                                        string [ ] items = s . Split ( '.' );
                                                        foreach ( var item in items )
                                                        {
                                                                if ( item . ToLower ( ) == argname . ToLower ( ) )
                                                                {
                                                                        result = true;
                                                                        break;
                                                                }
                                                        }
                                                        if ( result == false )
                                                                return false;
                                                }
                                                if ( !LockOutput )
                                                        currentpoint . Items . Add ( s );
                                                if ( controltarget != "" )
                                                {
                                                        if ( MousoverHandlers . IsActiveHitName ( Hitcontrol . names, controltarget ) )
                                                        {
                                                                if ( !LockOutput )
                                                                        currentpoint . Items . Add ( "CONTROL HIT..." );

                                                                string currentitem = Hitcontrol . ActiveHitName;

                                                                if ( Hitcontrol . ShowAll == false )
                                                                        ListAllMouseoverTargets ( Hitcontrol . ActiveHitName . ToUpper ( ) );

                                                        }
                                                        else
                                                        {
                                                                if ( !LockOutput )
                                                                        currentpoint . Items . Clear ( );
                                                        }
                                                }
                                                else
                                                {
                                                        if ( Hitcontrol . ShowAll )
                                                        {
                                                                //List all controls identified under current mouse position
                                                                ListAllMouseoverTargets ( Hitcontrol . ActiveHitName . ToUpper ( ) );
                                                        }
                                                }
                                        }
                                        else
                                        {
                                                //List all controls identified under current mouse position
                                                if ( Hitcontrol . ShowAll )
                                                        ListAllMouseoverTargets ( );
                                                else
                                                {
                                                        if ( !LockOutput )
                                                                currentpoint . Items . Clear ( );
                                                }
                                                if ( controltarget != "" )
                                                {
                                                        if ( MousoverHandlers . IsActiveHitName ( Hitcontrol . names, controltarget ) )
                                                        {
                                                                if ( !LockOutput )
                                                                        currentpoint . Items . Add ( "CONTROL HIT..." );
                                                        }
                                                }
                                        }
                                        return true;
                                }
                        }
                        else
                        {
                                if ( Hitcontrol . ObjectMembersName != null && Hitcontrol . GetObject )
                                {
                                        MousoverHandlers . GetParentObjects ( ( FrameworkElement ) Hitcontrol . ObjectMembersName, 9 );
                                        if ( !LockOutput )
                                                currentpoint . Items . Clear ( );
                                        // List all controls identified under current mouse position
                                        if ( Hitcontrol . names . Count > 0 )
                                                ListAllMouseoverTargets ( Hitcontrol . ObjectMemberString . ToUpper ( ) );
                                        else
                                                if ( !LockOutput )
                                                currentpoint . Items . Add ( "No item identified...." );
                                }
                                else if ( Hitcontrol . ObjectStatsString != "" && Hitcontrol . ObjectStats )
                                {
                                        if ( Hitcontrol . ObjectStatsDp == null )
                                                Hitcontrol . ObjectStatsDp = MousoverHandlers . FindDescendant ( this, Hitcontrol . ObjectStatsString );
                                        //                                        if ( Hitcontrol . ObjectStatsDp == null )
                                        //                                             MousoverHandlers . GetParent( ( FrameworkElement ) Hitcontrol . ObjectStatsDp, 9 );
                                        Hitcontrol . names = MousoverHandlers . GetHierarchy ( e );

                                        if ( MousoverHandlers . IsActiveHitName ( HitControl . _Hitcontrol . names, Hitcontrol . ObjectStatsString ) )
                                        {
                                                int found = 0;
                                                // We are over a control of the specified type
                                                foreach ( var item in Hitcontrol . names )
                                                {
                                                        if ( item . ToLower ( ) . Contains ( Hitcontrol . ObjectStatsString . ToLower ( ) ) )
                                                        {
                                                                break;
                                                        }
                                                        found++;
                                                }
                                                if ( found > 0 )
                                                {
                                                        if ( !LockOutput )
                                                                currentpoint . Items . Add ( $"Object {Hitcontrol . ObjectStatsString . ToUpper ( )} contains the following objects:" );

                                                        for ( int y = found ; y >= 0 ; y-- )
                                                        {
                                                                if ( !LockOutput )
                                                                        currentpoint . Items . Add ( Hitcontrol . names [ y ] );
                                                        }
                                                }
                                                //currentpoint . Text = "";
                                        }
                                        // List all controls identified under current mouse position
                                        //if ( Hitcontrol . names . Count > 0 )
                                        //        ListAllMouseoverTargets ( );
                                        //else
                                        //        currentpoint . Text = "No item identified....";
                                }
                                else
                                {
                                        if ( !LockOutput )
                                                currentpoint . Items . Clear ( );
                                        if ( Hitcontrol . ShowAll )
                                        {
                                                //List all controls identified under current mouse position
                                                Hitcontrol . names = MousoverHandlers . GetHierarchy ( e );
                                                foreach ( var item in Hitcontrol . names )
                                                {
                                                        if ( ( bool ) item . Contains ( "Border1" ) == true )
                                                                return false;
                                                }
                                                ListAllMouseoverTargets ( );
                                        }
                                }
                                return true;
                        }
                        return true;
                }

                #region specialist mousemove handlers

                public bool delegatetest ( MouseEventArgs e, HitControl Hitcontrol, MouseCheck mousecheck, string argstring = "", object argobject = null )
                {
                        if ( ( Hitcontrol . IsHitActive == false && Hitcontrol . ActiveHitsObject == null )
                                && ( Hitcontrol . GetObject == false || Hitcontrol . ObjectMemberString == "" )
                                && ( Hitcontrol . ObjectStats == false || Hitcontrol . ObjectStatsString == "" )
                                && ( Hitcontrol . ShowAll == false ) )
                                return false;
                        else
                        {
                                //mousecheck ( e, Hitcontrol, argstring, argobject);
                                return true;
                        }
                }
                /// <summary>
                /// Receives a List of strings that are the sub controls  identified in any control  specified  in Hitcontrol structure
                /// </summary>
                public void ListAllMouseoverTargets ( string caller = "" )
                {
                        if ( !LockOutput )
                        {
                                if ( caller == "" )
                                        currentpoint . Items . Add ( $"Hierarchy : " );
                                else
                                        currentpoint . Items . Add ( $"Hierarchy for : {caller}" );
                        }
                        foreach ( string item in Hitcontrol . names )
                        {
                                if ( item != "" )
                                        if ( !LockOutput )
                                                currentpoint . Items . Add ( item );
                        }
                }

                #region Checkboxes

                private void ObjStats_Checked ( object sender, RoutedEventArgs e )
                {
                        if ( GetObjectStats . IsChecked == true )
                        {
                                if ( ObjectStatsName . Text == "" )
                                {
                                        Hitcontrol . ObjectStatsDp = null;
                                        Hitcontrol . ObjectStatsString = "";
                                        GetObjectStats . IsChecked = false;
                                        if ( !LockOutput )
                                                currentpoint . Items . Add ( "No object specified..." );
                                }
                                else
                                {
                                        Hitcontrol . ObjectStats = true;
                                        var result = MousoverHandlers . FindHierarchyParent ( Hitcontrol . names, ObjectStatsName . Text );

                                        Hitcontrol . ObjectStatsDp = MousoverHandlers . FindDescendant ( this, result );
                                        ResetCheckboxes ( GetObjectStats . Name, true );
                                }
                        }
                        else
                        {
                                Hitcontrol . ObjectStats = false;
                                GetObjectStats . IsChecked = false;
                                if ( !LockOutput )
                                        currentpoint . Items . Clear ( );
                                ShowAll . IsEnabled = true;
                                ShowAll . Opacity = 1;
                                ResetCheckboxes ( GetObjectStats . Name, false);
                        }
                }
                private void CheckForHits_Checked ( object sender, RoutedEventArgs e )
                {
                        if ( CheckForHits . IsChecked == true )
                        {
                                if ( Hitcontrol . ActiveHitName == "" )
                                {
                                        Hitcontrol . IsHitActive = false;
                                        CheckForHits . IsChecked = false;
                                        Hitcontrol . ActiveHitsObject = null;
                                        if ( !LockOutput )
                                                currentpoint . Items . Add ( "No object specified..." );
                                        ResetCheckboxes ( CheckForHits . Name, false );
                                }
                                else
                                {
                                        CheckForHits . IsChecked = true;
                                        Hitcontrol . IsHitActive = true;
                                        Hitcontrol . ActiveHitsObject = MousoverHandlers . FindDescendant ( this, Hitcontrol . ActiveHitName );
                                        if ( !LockOutput )
                                                currentpoint . Items . Clear ( );
                                        ResetCheckboxes ( CheckForHits . Name, true );
                                }
                        }
                        else
                        {
                                Hitcontrol . IsHitActive = false;
                                if ( !LockOutput )
                                        currentpoint . Items . Clear ( );
                        }
                }
                private void ObjMembers_Checked ( object sender, RoutedEventArgs e )
                {
                        if ( ObjMembers . IsChecked == true )
                        {
                                if ( objmembersname . Text != "" )
                                {
                                        Hitcontrol . GetObject = true;
                                        Hitcontrol . ObjectMemberString = objmembersname . Text;
                                        Hitcontrol . ObjectMembersName = MousoverHandlers . FindDescendant ( this, objmembersname . Text );
                                        if ( Hitcontrol . ObjectMembersName == null )
                                        {
                                                ObjMembers . IsChecked = true;
                                                Hitcontrol . ObjectStatsDp = ( FrameworkElement ) e . Source;
                                                ResetCheckboxes ( ObjMembers. Name, true);
                                           }
                                }
                                else
                                {
                                        Hitcontrol . GetObject = false;
                                        Hitcontrol . ObjectMemberString = objmembersname . Text;
                                        Hitcontrol . ObjectMembersName = null;
                                        ObjMembers . IsChecked = false;
                                        if ( objmembersname . Text != "" )
                                        {
                                                if ( !LockOutput )
                                                        currentpoint . Items . Add ( "Pllease enter Object name ..." );
                                        }
                                        ResetCheckboxes ( ObjMembers . Name, false );
                                }
                        }
                        else
                        {
                                Hitcontrol . GetObject = false;
                                Hitcontrol . ObjectMemberString = objmembersname . Text;
                                ObjMembers . IsChecked = false;
                                if ( !LockOutput )
                                        currentpoint . Items . Clear ( );
                                ResetCheckboxes ( ObjMembers . Name, false );
                        }
                }
                private void objectmembersname_KeyDown ( object sender, KeyEventArgs e )
                {
                        if ( e . Key == Key . Enter )
                        {
                                if ( objmembersname . Text != "" )
                                {
                                        Hitcontrol . ObjectMemberString = objmembersname . Text;
                                        GetObjectStats . IsChecked = true;
                                        // Convert from string to DependencyObject
                                        Hitcontrol . ObjectMembersName = MousoverHandlers . FindDescendant ( this, objmembersname . Text );
                                        ResetCheckboxes ( ObjMembers . Name, true);
                                }
                                else
                                {
                                        Hitcontrol . ObjectMemberString = objmembersname . Text;
                                        GetObjectStats . IsChecked = false;
                                        ResetCheckboxes ( ObjMembers . Name, false );
                                }
                        }
                }

                 private void ShowAll_Checked ( object sender, RoutedEventArgs e )
                {
                        if ( ShowAll . IsChecked == true )
                        {
                                Hitcontrol . ShowAll = true;
                                ResetCheckboxes ( ShowAll. Name, true );
                        }
                        else
                        {
                                Hitcontrol . ShowAll = false;
                                ResetCheckboxes ( ShowAll . Name, false);
                        }
                        if ( !LockOutput )
                                currentpoint . Items . Clear ( );
                }
                private void HitsName_KeyDown ( object sender, KeyEventArgs e )
                {
                        if ( e . Key == Key . Enter )
                        {
                                Hitcontrol . ActiveHitName = HitsName . Text;
                                if ( Hitcontrol . ActiveHitName != "" )
                                {
                                        // Convert from string to DependencyObject
                                        // Usually FAILS
                                        Hitcontrol . ActiveHitsObject = MousoverHandlers . FindDescendant ( this, Hitcontrol . ActiveHitName );
                                        Hitcontrol . IsHitActive = true;
                                        CheckForHits . IsChecked = true;
                                        if ( !LockOutput )
                                                currentpoint . Items . Add ( "Mouse over the selected object(s)..." );
                                        ResetCheckboxes ( HitsName. Name, true);
                                }
                                else
                                {
                                        Hitcontrol . IsHitActive = false;
                                        CheckForHits . IsChecked = false;
                                        if ( !LockOutput )
                                                currentpoint . Items . Add ( "No control object entered..." );
                                        ResetCheckboxes ( HitsName . Name, false );
                                }
                        }
                        //                        currentpoint . Text = "";
                }
                 private void objectstatsname_KeyDown ( object sender, KeyEventArgs e )
                {
                        if ( e . Key == Key . Enter )
                        {
                                if ( ObjectStatsName . Text != "" )
                                {
                                        Hitcontrol . ObjectStatsString = ObjectStatsName . Text;
                                        Hitcontrol . ObjectStats = true;
                                        //GetObjectStats . IsChecked = true;
                                        // find the specifed control so we get the DP of it to use  to break ist content down
                                        foreach ( var tb in MousoverHandlers . FindVisualChildren<Control> ( this ) )
                                        {
                                                if ( tb . Name . ToLower ( ) == ObjectStatsName . Text . ToLower ( ) )
                                                {
                                                        Console . WriteLine ( $"{tb . Name}" );
                                                        Hitcontrol . ObjectStatsDp = tb;
                                                        break;
                                                }

                                        }
                                        if ( !LockOutput )
                                                currentpoint . Items . Clear ( );
                                        if ( !LockOutput )
                                                currentpoint . Items . Add ( $"Object : {Hitcontrol . ObjectStatsString . ToUpper ( )}" );
                                        if ( Hitcontrol . ObjectStatsDp != null )
                                        {
                                                foreach ( var tb in MousoverHandlers . FindVisualChildren<FrameworkElement> ( Hitcontrol . ObjectStatsDp ) )
                                                {
                                                        if ( tb . Name != "" )
                                                        {
                                                                if ( !LockOutput )
                                                                        currentpoint . Items . Add ( $"x:Name = {tb . Name}, ActualHeight/Width = {( int ) tb . ActualHeight} / {( int ) tb . ActualWidth}" );
                                                        }
                                                }
                                        }
                                        ResetCheckboxes ( GetObjectStats . Name, true);
                                }
                                else
                                {
                                        Hitcontrol . ObjectMemberString = objmembersname . Text;
                                        GetObjectStats . IsChecked = false;
                                        ResetCheckboxes ( GetObjectStats . Name, false );

                                }
                        }
                }

                #endregion
                public bool IsControlhit ( string controlname, bool testmode = false )
                {
                        bool result = false;
                        if ( testmode )
                        {
                                if ( Hitcontrol . names [ 0 ] . Contains ( controlname ) )
                                        return result;
                        }
                        foreach ( var item in Hitcontrol . names )
                        {
                                if ( item . ToLower ( ) . Contains ( controlname . ToLower ( ) ) )
                                {
                                        result = true;
                                        break;
                                }
                        }
                        return result;
                }
                public void DisplayControlStats ( DependencyObject dp, string name )
                {
                        MousoverHandlers . GetObjectSize ( ( DependencyObject ) dp, out double dpHeight, out double dpWidth );
                        if ( !LockOutput )
                                currentpoint . Items . Add ( $"Object Statistics for : {name}" );
                        if ( !LockOutput )
                                currentpoint . Items . Add ( $"Height = {dpHeight}, Width = {dpWidth}" );
                        MousoverHandlers . GetObjectActualSize ( ( DependencyObject ) dp, out double dpHeight2, out double dpWidth2 );
                        if ( !LockOutput )
                                currentpoint . Items . Add ( $"ActualHeight = {dpHeight2}, ActualWidth = {dpWidth2}" );
                }

                private void ButtonTesting_MouseMove ( object sender, MouseEventArgs e )
                {
                        if ( DesignerProperties . GetIsInDesignMode ( this ) == false )
                        {
                                if ( Hitcontrol . IsHitActive )// && Hitcontrol . ActiveHitsObject != null )
                                {
                                        // Hits Active option is Active
                                        var Mousecheck = new MouseCheck ( DoMouseCheck );

                                        // call delegate
                                        if ( delegatetest ( e, Hitcontrol, Mousecheck, "", null ) )
                                        {
                                                if ( Hitcontrol . IsHitActive && Hitcontrol . ActiveHitName != "" )
                                                        if ( Mousecheck ( e, Hitcontrol, Hitcontrol . ActiveHitName, Hitcontrol . ActiveHitsObject ) == false )
                                                        {
                                                                if ( !LockOutput )
                                                                        currentpoint . Items . Clear ( );

                                                        }
                                                        else
                                                        {
                                                                //object obj = new ShadowLabelControl ( );
                                                                //MousoverHandlers.GetObjectHierarchy ( obj,  "" );
                                                                 ListAllMouseoverTargets ( Hitcontrol . ActiveHitName );
                                                        }
                                        }
                                }
                                else if ( Hitcontrol . ObjectStats && Hitcontrol . ObjectStatsString != "" )
                                {
                                        if ( !LockOutput )
                                                currentpoint . Items . Clear ( );
                                        // Hits Active option is Active
                                        var Mousecheck = new MouseCheck ( DoMouseCheck );
                                        // call delegate
                                        if ( delegatetest ( e, Hitcontrol, Mousecheck, "", null ) )
                                                Mousecheck ( e, Hitcontrol, "", null );

                                        if ( IsControlhit ( Hitcontrol . ObjectStatsString ) == false )
                                                return;
                                        string currentitem = Hitcontrol . ObjectStatsString;
                                        Hitcontrol . ObjectStatsDp = ( FrameworkElement ) e . Source;
                                        if ( currentitem . Contains ( Hitcontrol . ObjectStatsString ) )
                                                DisplayControlStats ( Hitcontrol . ObjectStatsDp, Hitcontrol . ObjectStatsString );
                                }
                                else
                                {
                                        if ( Hitcontrol . ShowAll )
                                        {
                                                var Mousecheck = new MouseCheck ( DoMouseCheck );
                                                // call delegate
                                                if ( delegatetest ( e, Hitcontrol, Mousecheck, "", null ) )
                                                        Mousecheck ( e, Hitcontrol, "", null );
                                        }
                                }
                        }
                }

                #endregion

                #region ICOMMANDS && PropertyChanged
                //========================================================================


                public event PropertyChangedEventHandler PropertyChanged;
                // logic for the ICOMMAND in Command.cs
                public bool canExecuteMethod ( object parameter )
                {
                        return true;
                }
                //ACTION for the ICommand               
                public void ExecuteMethod ( object parameter )
                {
                        MessageBox . Show ( "Command executed..." );
                }

                #endregion

                protected void OnPropertyChanged ( string propertyname )
                {
                        if ( PropertyChanged != null )
                                PropertyChanged ( this, new PropertyChangedEventArgs ( propertyname ) );
                }

                private void HitsName_TextChanged ( object sender, TextChangedEventArgs e )
                {

                }
     
                private void ResetCheckboxes ( string Obj, bool mode=false)
                {
                        if ( Obj == null )
                        {
                                CheckForHits . IsChecked = true;
                                CheckForHits . Opacity = 1.0;
                                ObjMembers . IsChecked = true;
                                ObjMembers . Opacity = 1.0;
                                GetObjectStats . IsChecked = true;
                                GetObjectStats . Opacity = 1.0;
                                ShowAll . IsChecked = true;
                                ShowAll . Opacity = 1.0;
                                return;
                        }
                        if ( mode == false )
                        {
//                                CheckForHits . IsChecked = true;
                                CheckForHits . Opacity = 1.0;
//                                ObjMembers . IsChecked = true;
                                ObjMembers . Opacity = 1.0;
//                                GetObjectStats . IsChecked = true;
                                GetObjectStats . Opacity = 1.0;
//                                ShowAll . IsChecked = true;
                                ShowAll . Opacity = 1.0;
                                if ( Obj == "GetObjectStats" )
                                {
                                        GetObjectStats . IsChecked = false;
         //                               GetObjectStats . Opacity = 0.6;
                                }
                                else if ( Obj == "ObjMembers" )
                                {
                                        ObjMembers . IsChecked = false;
      //                                  ObjMembers . Opacity = 0.6;
                                }
                                else if ( Obj == "ShowAll" )
                                {
                                        ShowAll . IsChecked = false;
//                                        ShowAll . Opacity = 0.6;
                                }
                                else if ( Obj == "CheckForHits" )
                                {
                                        CheckForHits . IsChecked = false;
   //                                     CheckForHits . Opacity = 0.6;
                                }
                        }
                        else
                        {
                                CheckForHits . IsChecked = false;
                                CheckForHits . Opacity = 0.6;
                                ObjMembers . IsChecked = false;
                                ObjMembers . Opacity = 0.6;
                                GetObjectStats . IsChecked = false;
                                GetObjectStats . Opacity = 0.6;
                                ShowAll . IsChecked = false;
                                ShowAll . Opacity = 0.6;
                                if ( Obj == "GetObjectStats" )
                                {
                                        GetObjectStats . IsChecked = true;
                                        GetObjectStats . Opacity = 1.0;
                                }
                                else if ( Obj == "ObjMembers" )
                                {
                                        ObjMembers . IsChecked = true;
                                        ObjMembers . Opacity = 1.0;
                                }
                                else if ( Obj == "ShowAll" )
                                {
                                        ShowAll . IsChecked = true;
                                        ShowAll . Opacity = 1.0;
                                }
                                else if ( Obj == "CheckForHits" )
                                {
                                        CheckForHits . IsChecked = true;
                                        CheckForHits . Opacity = 1.0;
                                }
                        }


                }
                private void lockOutput_Checked ( object sender, KeyEventArgs e )
                {
                        if ( e . Key == Key . F2 )
                                LockOutput = !LockOutput;
                        if ( LockOutput )
                                lockOutput . Text = "Press F2 to UNLOCK Output Window";
                        else
                                lockOutput . Text = "Press F2 to LOCK Output Window";
                }

                private void dummyMove ( object sender, MouseEventArgs e )
                {
                        // do nothing on purpose
                }
        }

  

}
