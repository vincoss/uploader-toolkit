using System;


namespace Vinco.Silverlight.Framework.TriggerActions
{
    public class EventDescriptor<TEventArgs> where TEventArgs : EventArgs
    {
        public object Sender { get; set; }
        public TEventArgs EventArgs { get; set; }
        public object CommandArgument { get; set; }
    }
}
