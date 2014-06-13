using System.IO;
using System.Windows;


namespace Vinco.Silverlight.Framework.TriggerActions
{
    public class MapDragDropEventToCommand : MapEventToCommandBase<DragEventArgs>
    {
        protected override void Invoke(object parameter)
        {
            if (this.Command == null)
            {
                return;
            }
            DragEventArgs args = parameter as DragEventArgs;
            FileInfo[] files = (FileInfo[])args.Data.GetData(DataFormats.FileDrop);
            if (this.Command.CanExecute(files))
            {
                this.Command.Execute(files);
            }
        }
    }
}