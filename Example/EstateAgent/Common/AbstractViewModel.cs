using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using log4net;

namespace EstateAgent.Common
{
    public abstract class AbstractViewModel : DependencyObject,  INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string displayName;
        protected ILog logger;

        protected AbstractViewModel(string theDisplayName)
        {
            logger = LogManager.GetLogger(this.GetType());
            displayName = theDisplayName;
        }

        public string DisplayName
        {
            get { return this.displayName; }
            set 
            { 
                this.displayName= value;
                RaisePropertyChangedEvent(() => this.DisplayName);
            }
        }

        protected void RaisePropertyChangedEvent<TValue>(Expression<Func<TValue>> propertySelector)
        {
            if (PropertyChanged == null)
                return;

            var memberExpression = propertySelector.Body as MemberExpression;
            if (memberExpression == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(memberExpression.Member.Name));
        }
    }
}
