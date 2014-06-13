using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading;
using System.Collections.Generic;


namespace Vinco.Silverlight.Framework.Models
{
    public abstract class PropertyChangedBase : INotifyPropertyChanged
    {
        private PropertyChangedEventHandler _propChangedHandler;
        private readonly SynchronizationContext _synchronizationContext;
        private static readonly Dictionary<string, PropertyChangedEventArgs> EventCache = new Dictionary<string, PropertyChangedEventArgs>();

        protected PropertyChangedBase()
        {
            this._synchronizationContext = SynchronizationContext.Current;
        }

        protected void NotifyPropertyChanged<T>(Expression<Func<T>> property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            var expression = (MemberExpression)property.Body;
            var member = expression.Member;
            if (this._synchronizationContext != null)
            {
                this._synchronizationContext.Post(delegate
                                                      {
                                                          if (this._propChangedHandler != null)
                                                          {
                                                              this._propChangedHandler(this, GetEventArgs(member.Name));
                                                          }
                                                      }, null);
            }
            else
            {
                this._propChangedHandler(this, GetEventArgs(member.Name));
            }
        }

        #region Private methods

        private static PropertyChangedEventArgs GetEventArgs(string propertyName)
        {
            PropertyChangedEventArgs propertyEvent;
            if (EventCache.TryGetValue(propertyName, out propertyEvent) == false)
            {
                propertyEvent = new PropertyChangedEventArgs(propertyName);
                EventCache[propertyName] = propertyEvent;
            }
            return propertyEvent;
        }
        
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                this._propChangedHandler = (PropertyChangedEventHandler)Delegate.Combine(this._propChangedHandler, value);
            }
            remove
            {
                if (this._propChangedHandler != null)
                {
                    this._propChangedHandler = (PropertyChangedEventHandler)Delegate.Remove(this._propChangedHandler, value);
                }
            }
        }
        #endregion
    }
}
