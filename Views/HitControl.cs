#pragma warning disable CS0234

using System;
using System . Collections . Generic;
using System . ComponentModel;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
namespace WPFPages . Views
{
        public class HitControl : INotifyPropertyChanged
        {
                public static HitControl _Hitcontrol = new HitControl ( );
                public  HitControl ( )
                {
                }
                #region FULL properties

                //======================================================//
                private bool isHitActive;
                public bool IsHitActive
                {
                        get{return isHitActive;
                        }
                        set{
                                isHitActive = value; OnPropertyChanged ( nameof ( IsHitActive ) );
                        }
                }
                private string activeHitName;
                public string ActiveHitName
                {
                        get
                        {
                                return activeHitName;
                        }
                        set
                        {
                                activeHitName = value;
                                OnPropertyChanged ( nameof ( ActiveHitName ) );
                        }
                }
                private DependencyObject activeHitsObject;
                public DependencyObject ActiveHitsObject
                {
                        get
                        {
                                return activeHitsObject;
                        }
                        set
                        {
                                activeHitsObject = value;
                                OnPropertyChanged ( nameof ( activeHitsObject ) );
                        }
                }

                //======================================================//
                // Get Object Members
                private bool getObject;
                public bool GetObject
                {
                        get{return getObject;}
                        set{getObject = value; OnPropertyChanged ( nameof ( GetObject ) );
                        }
                }
                private string objectmemberstring;
                public string ObjectMemberString
                {
                        get
                        {
                                return objectmemberstring;
                        }
                        set
                        {
                                objectmemberstring = value;
                                OnPropertyChanged ( nameof ( ObjectMemberString ) );
                        }
                }
                private DependencyObject objectMembersName;
                public DependencyObject ObjectMembersName
                {
                        get
                        {
                                return objectMembersName;
                        }
                        set
                        {
                                objectMembersName = value;
                                OnPropertyChanged ( nameof ( ObjectMembersName ) );
                        }
                }
                //======================================================//
                // Get Object Statistics
                private bool objectStats;
                public bool ObjectStats
                {
                        get{return objectStats;}
                        set{objectStats = value; OnPropertyChanged ( nameof ( ObjectStats ) );
                        }
                }
                private string objectStatsString;
                public string ObjectStatsString
                {
                        get
                        {
                                return objectStatsString;
                        }
                        set
                        {
                                objectStatsString = value;
                                OnPropertyChanged ( nameof ( ObjectStatsString ) );
                        }
                }
                private DependencyObject objectStatsDp;
                public DependencyObject ObjectStatsDp
                {
                        get
                        {
                                return objectStatsDp;
                        }
                        set
                        {
                                objectStatsDp = value;
                                OnPropertyChanged ( nameof ( ObjectStatsDp ) );
                        }
                }
                //======================================================//
                 // Show data on everything
                private bool showall;
                public bool ShowAll
                {
                        get
                        {
                                return showall;
                        }
                        set
                        {
                                showall = value;
                                OnPropertyChanged ( nameof ( ShowAll ) );
                        }
                }
                //======================================================//
                private List<string> _names;  
                public List<string> names
                {
                        get{return _names;}
                        set{_names = value; OnPropertyChanged ( nameof ( names ) );
                        }
                }
                //======================================================//
                #endregion 
        }
}
