

using System;
using System . Collections . Generic;
using System . ComponentModel;
using System . Globalization;
using System . Runtime . CompilerServices;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Media . Imaging;
using WPFPages . Converts;
using WPFPages . UserControls;

namespace WPFPages . Views
{
        /// <summary>
        /// Support class for a window that demonstrates how  to access objects currently under the mouse pointer
        /// It is useful for getting hierarchy of any control by calling these methods
        /// </summary>
        public class MousoverHandlers : FrameworkElement
        {
                  /// <summary>
                /// Returns a List of strings containing all parent controls of the control that has the mouseover. Optionally it accepts a control x:Name and will stop the iteration when-If found
                /// </summary>
                /// <param name="e"></param>
                /// <param name="target"></param>
                /// <returns></returns>
                public static List<string> GetHierarchy ( MouseEventArgs e, string target = "" )
                {
                        string temp = "";
                        string temp2 = "";
                        string currname = "";
                        List<string> names = new List<string> ( );
                        DependencyObject parent = null;        

                       // create a dumy if we dont receive a target paraeter so it never matches
                        if ( target == "" )
                                target = "X1X2X3x4x5x6";
                        
                        // Handle checking for something under the mouse
                        DependencyObject dpo = e . OriginalSource as DependencyObject;
                        currname = dpo . GetValue ( (DependencyProperty)NameProperty ) . ToString ( );
                        if ( e . OriginalSource . ToString ( ) == target )
                                return names;
                           names . Add ( "Mouseover object : " + currname + " : " + e . OriginalSource . ToString ( ) );

                        //Now stat the iteration up the Visual tree
                        parent = VisualTreeHelper . GetParent ( e . OriginalSource as DependencyObject );
                        if ( parent . ToString ( ) . Contains ( target ) )
                                return names;

                        // iterate down  the tree first
                        foreach ( var item in FindVisualChildren<FrameworkElement> ( e . OriginalSource as DependencyObject ) )
                        {
                                if ( item == null ) break;
                                DependencyObject d = item;
                                Console . WriteLine (   $"{d}");
                        }


                        names . Add ( parent . ToString ( ) );
                        while ( true )
                        {
                                if ( parent != null )
                                {
                                        temp2 = "";
                                        parent = VisualTreeHelper . GetParent ( parent );
                                        if ( parent == null )
                                                break;
                                        temp = parent . ToString ( );
                                        if ( temp . Contains ( target ) )
                                                break;
                                        temp2 = parent . GetValue ( NameProperty ) . ToString ( );
                                        if ( temp2 . Contains ( target ) )
                                        {
                                                // Reached the specified target level, so STOP here...
                                                names . Add ( "Control Target : " + temp . Substring ( temp . IndexOf ( ':' ) + 1 ) );
                                                break;
                                        }
                                        else
                                        {
                                                if ( temp2 != "" )
                                                        temp += " : " + temp2;
                                        }
                                        names . Add ( temp );
                                }
                        }
                          return names;
                }
                public static List<string> GetObjectHierarchy ( object e, string target = "" )
                {
                        string temp = "";
                        string temp2 = "";
                        string currname = "";
                        List<string> names = new List<string> ( );
                        DependencyObject parent = null;

                        // create a dumy if we dont receive a target paraeter so it never matches
                        if ( target == "" )
                                target = "X1X2X3x4x5x6";

                        // Handle checking for something under the mouse
                        DependencyObject dpo = e  as DependencyObject;
                        currname = dpo . GetValue ( ( DependencyProperty ) NameProperty ) . ToString ( );
                        if ( e  . ToString ( ) == target )
                                return names;
                        names . Add ( "Mouseover object : " + currname + " : " + e  . ToString ( ) );

                        //Now stat the iteration up the Visual tree
                        //parent = VisualTreeHelper . GetParent ( e as DependencyObject );
                        //if ( parent . ToString ( ) . Contains ( target ) )
                        //        return names;

                        // iterate down  the tree first
                        foreach ( var item in FindVisualChildren<DependencyObject> (e as DependencyObject) )
                        {
                                if ( item == null )
                                        break;
                                DependencyObject d = item;
                                Console . WriteLine ( $"{d}" );
                        }

                       if(parent == null)
                               return null;
                        names . Add ( parent . ToString ( ) );
                        while ( true )
                        {
                                if ( parent != null )
                                {
                                        temp2 = "";
                                        parent = VisualTreeHelper . GetParent ( parent );
                                        if ( parent == null )
                                                break;
                                        temp = parent . ToString ( );
                                        if ( temp . Contains ( target ) )
                                                break;
                                        temp2 = parent . GetValue ( NameProperty ) . ToString ( );
                                        if ( temp2 . Contains ( target ) )
                                        {
                                                // Reached the specified target level, so STOP here...
                                                names . Add ( "Control Target : " + temp . Substring ( temp . IndexOf ( ':' ) + 1 ) );
                                                break;
                                        }
                                        else
                                        {
                                                if ( temp2 != "" )
                                                        temp += " : " + temp2;
                                        }
                                        names . Add ( temp );
                                }
                        }
                        return names;
                }

                /// <summary>
                /// Find a specified  control name from a list of mouseover targets
                /// created by Method above ( GetHierarchy )
                /// </summary>
                /// <param name="names"></param>
                /// <param name="target"></param>
                /// <returns>list of all parent objects forming the the specified element</returns>
                public static string FindHierarchyParent ( List<string> names, string target )
                {
                        string result = "";
                        string upperitem = "";
                        string uppertarget = target;
                        if ( target == null )
                                return result;
                        foreach ( var item in names )
                        {
                                if ( item != null )
                                {
                                        upperitem = item;
                                        if ( upperitem . Contains ( uppertarget ) )
                                        {
                                                result = item;
                                                break;
                                        }
                                }
                                else
                                        break;
                        }
                        if ( result == "" )
                                result = names [ names . Count - 1 ];
                        return result;
                }

                /// <summary>
                /// takes a specified  control name and checks t see if it is under the current mousemove position
                /// </summary>
                /// <param name="names"></param>
                /// <param name="target"></param>
                /// <returns>true or false</returns>
                public static bool  IsActiveHitName( List<string> names, string target )
                {                       
                        bool result = false;
                        if( target == null ) return result;
                        foreach ( var item in names )
                        {
                                if ( item.ToUpper() . Contains ( target .ToUpper()) )
                                {
                                        result = true;
                                        break;
                                }
                        }
                        return result;
                }

                #region Wpf Utility methods

                public  static DependencyObject FindDescendant ( DependencyObject parent, string name )
                {
                        // See if this object has the target name.
                        //FrameworkElement element = parent as FrameworkElement;
                        //if ( ( element != null ) && ( element . Name == name ) )
                        //        return parent;

                        // Recursively check the children.
                        int num_children = VisualTreeHelper . GetChildrenCount ( parent );
                        for ( int i = 0 ; i < num_children ; i++ )
                        {
                                // See if this child has the target name.
                                DependencyObject child = VisualTreeHelper . GetChild ( parent, i );
                                DependencyObject descendant = FindDescendant ( child, name );
                                if ( descendant != null )
                                {
                                        string childitem = descendant . ToString ( );
                                        if ( childitem . Contains ( name ) )
                                        {
                                                return descendant;
                                        }
                                }
                        }
                        // We didn't find a descendant with the target name.
                        return null;
                }

                /// <summary>
                /// Returns the PARENT Dependency Object TREE of whatever object is passed in
                /// plus it's name as a string in the  'out' parameter
                /// </summary>
                /// <param name="dp"></param>
                /// <param name="name"></param>
                /// <returns>List of object names in Hierarchical order</returns>
                public static DependencyObject GetParentObjects ( FrameworkElement dp, int levels = 1 )
                {
                        // How to get the parent 's Name and the object of any object received
                        // Expects the dp t be the parent DP
                        FrameworkElement dps = null;
                        string clientname = "";
                        HitControl._Hitcontrol.names = new List<string> ( );
                        int lvl = 0;
                        FrameworkElement dpo = ( FrameworkElement ) dp . Parent;
                        if ( dpo == null )
                                return null;

                        clientname = ( string ) dp . GetValue ( NameProperty );
                        HitControl._Hitcontrol  . names . Add ( clientname );
                        for ( int x = 0 ; x < levels ; x++ )
                        {
                                dps = ( FrameworkElement ) dpo . Parent;
                                if ( dps == null )
                                {
                                        dpo = dps;
                                        break;
                                }
                                else
                                {
                                        HitControl._Hitcontrol  . names . Add ( ( string ) dps . GetValue ( NameProperty ) );
                                        dpo = dps;
                                        if ( HitControl._Hitcontrol  . names [ lvl ] == "" )
                                        {
                                                HitControl._Hitcontrol  . names [ lvl ] = "No further Parents";
                                                break;
                                        }
                                        lvl++;
                                }
                        }
                        return dpo;
                }
                    public static DependencyObject GetObjectHierarchy ( FrameworkElement dp, out List<string> names, int levels = 99 )
                {
                        // How to get the parent 's Name and the object of any object received
                        // Expects the dp to be the parent DP
                        FrameworkElement dps = null;
                        string clientname = "";
                        names = new List<string> ( );
                        int lvl = 0;
                        FrameworkElement dpo = ( FrameworkElement ) dp . Parent;
                        if ( dpo == null )
                                return null;

                        clientname = ( string ) dp . GetValue ( NameProperty );
                        names . Add ( clientname );
                        for ( int x = 0 ; x < levels ; x++ )
                        {
                                dps = ( FrameworkElement ) dpo . Parent;
                                if ( dps == null )
                                {
                                        dpo = dps;
                                        break;
                                }
                                else
                                {
                                        names . Add ( ( string ) dps . GetValue ( NameProperty ) );
                                        dpo = dps;
                                        if ( names [ lvl ] == "" )
                                        {
                                                names [ lvl ] = "No further Parents";
                                                break;
                                        }
                                        lvl++;
                                }
                        }
                        return dpo;
                }

                /// <summary>
                /// returns ALL controls contained within a specified parent 
                /// eg : FindVisualChildren<Control> ( this) 
                /// will return all cntrols within whatever 'this' represents
                /// </summary>
                /// <typeparam name="T"></typeparam>
                /// <param name="depObj"></param>
                /// <returns></returns>
                public static IEnumerable<T> FindVisualChildren<T> ( DependencyObject depObj ) where T : DependencyObject
                {
                        if ( depObj == null )
                                yield return null;

                        for ( int i = 0 ; i < VisualTreeHelper . GetChildrenCount ( depObj ) ; i++ )
                        {
                                var child = VisualTreeHelper . GetChild ( depObj, i );

                                if ( child != null && child is T )
                                        yield return ( T ) child;

                                foreach ( T childOfChild in FindVisualChildren<T> ( child ) )
                                        yield return childOfChild;
                        }
                }

                #endregion

                #region object sizemethods
                /// <summary>
                /// Returns the Height/Width of the received objects PARENT Dependency Object 
                /// </summary>
                /// <param name="dp"></param>
                /// <param name="dpHeight"></param>
                /// <param name="dpWidth"></param>
                public static void GetParentSize ( FrameworkElement dp, out double dpHeight, out double dpWidth, out string parentname )
                {
                        // How to get the size of any "Parent" object
                        // Expects the dp t be the parent DP
                        dpHeight = 0;
                        dpWidth = 0;
                        parentname = "";
                        if ( dp == null )
                        {
                                dpHeight = 0;
                                dpWidth = 0;
                                return;
                        }
                        DependencyObject dpo = dp . Parent;
                        if ( dpo == null )
                        {
                                dpHeight = 0;
                                dpWidth = 0;
                                return;
                        }
                        parentname = ( string ) dpo . GetValue ( NameProperty );
                        dpHeight = ( double ) dpo . GetValue ( ActualHeightProperty );
                        dpWidth = ( double ) dpo . GetValue ( ActualWidthProperty );
                        if ( dpHeight == 0 || dpWidth == 0 )
                        {
                                dpHeight = ( double ) dp . GetValue ( HeightProperty );
                                dpWidth = ( double ) dp . GetValue ( WidthProperty );
                        }
                }
                /// <summary>
                /// Returns the Height/Width of the received object's Dependency Object 
                /// </summary>
                /// <param name="obj"></param>
                /// <param name="dpHeight"></param>
                /// <param name="dpWidth"></param>
                public static void GetObjectSize ( DependencyObject obj, out double dpHeight, out double dpWidth )
                {
                        // How to get the STANDARD Height/Width of any object from its DP settings
                        // Expects the dp to be the object Name 
                        dpHeight = 0;
                        dpWidth = 0;
                        if ( obj == null )
                        {
                                dpHeight = 0;
                                dpWidth = 0;
                                return;
                        }
                        dpHeight = ( double ) obj . GetValue ( ActualHeightProperty );
                        dpWidth = ( double ) obj . GetValue ( ActualWidthProperty );
                }
                /// <summary>
                /// Returns the Height/Width of the received object's Dependency Object 
                /// </summary>
                /// <param name="obj"></param>
                /// <param name="dpHeight"></param>
                /// <param name="dpWidth"></param>
                public static void GetObjectActualSize ( DependencyObject obj, out double dpHeight, out double dpWidth )
                {
                        // How to get the ACTUAL size of any "Parent" object from its DP settings
                        // Expects the dp to be the object Name
                        dpHeight = 0;
                        dpWidth = 0;
                        if ( obj == null )
                        {
                                dpHeight = 0;
                                dpWidth = 0;
                                return;
                        }
                        dpHeight = ( double ) obj . GetValue ( ActualHeightProperty );
                        dpWidth = ( double ) obj . GetValue ( ActualWidthProperty );
                }
                #endregion

        }
}
