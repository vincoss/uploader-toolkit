using System;
using Vinco.Silverlight.Framework.Commands;


namespace Vinco.Silverlight.Commands
{
    // TODO:
    public class OptionsCommand : Command
    {
        public override void Execute(object parameter)
        {
            System.Windows.MessageBox.Show("Options here...");
        }
    }
}
