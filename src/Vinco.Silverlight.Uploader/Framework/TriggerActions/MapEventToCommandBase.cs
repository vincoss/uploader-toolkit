using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Interactivity;


namespace Vinco.Silverlight.Framework.TriggerActions
{
    public abstract class MapEventToCommandBase<TEventArgs> : TriggerAction<FrameworkElement> where TEventArgs : EventArgs
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand),
            typeof(MapEventToCommandBase<TEventArgs>), new PropertyMetadata(null, OnCommandPropertyChanged));

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter",
            typeof(object), typeof(MapEventToCommandBase<TEventArgs>), new PropertyMetadata(null, OnCommandParameterPropertyChanged));

        private static void OnCommandParameterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var invokeCommand = d as MapEventToCommand;
            if (invokeCommand != null)
            {
                invokeCommand.SetValue(CommandParameterProperty, e.NewValue);
            }
        }

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var invokeCommand = d as MapEventToCommand;
            if (invokeCommand != null)
            {
                invokeCommand.SetValue(CommandProperty, e.NewValue);
            }
        }

        protected override void Invoke(object parameter)
        {
            if (this.Command == null)
            {
                return;
            }
            if (this.Command.CanExecute(parameter) == false)
            {
                return;
            }
            var eventDescriptor = new EventDescriptor<TEventArgs>
                                      {
                                          Sender = this.AssociatedObject,
                                          EventArgs = (TEventArgs)parameter,
                                          CommandArgument = CommandParameter
                                      };
            this.Command.Execute(eventDescriptor);
        }

        public ICommand Command
        {
            get { return (ICommand)base.GetValue(CommandProperty); }
            set { base.SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return base.GetValue(CommandParameterProperty); }
            set { base.SetValue(CommandParameterProperty, value); }
        }
    }
}
