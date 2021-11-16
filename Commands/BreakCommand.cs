using System;
using System . Collections . Generic;
using System . Diagnostics;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Input;
using System . Xml . Linq;

namespace WPFPages . Commands
{
        /// <summary>
        /// Special command to allow me to go into debugger break mode via any  specified key press
        /// </summary>
        public class BreakCommand : ICommand
        {
                public bool CanExecute ( object parameter )
                {return true;}
                public void Execute ( object parameter )
                {Debugger . Break ( );}

                public event EventHandler CanExecuteChanged
                {
                        add{CommandManager . RequerySuggested += value;}
                        remove{CommandManager . RequerySuggested -= value;}
                }

        }
}
