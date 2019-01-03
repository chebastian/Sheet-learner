using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MVVMHelpers
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged; 

        protected void OnPropertyChanged( [CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged; 
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class LocationThingyEventArg
    {
        public string PropName;
        public bool HasError;
    }

    public class ValidationViewModelBase : ViewModelBase, INotifyDataErrorInfo
    { 
        protected readonly Dictionary<string, ICollection<string>> validationErrors; 
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged; 

        public delegate void OnErrorChanged(INotifyDataErrorInfo info); 
        public OnErrorChanged NotifyOnErrorChanged;

        public ValidationViewModelBase()
            :base()
        {
            validationErrors = new Dictionary<string, ICollection<string>>();
        }

        protected void UpdateError(ICollection<string> errors,[CallerMemberName]string propertyName = null)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
                return;
 
            if(validationErrors.ContainsKey(propertyName))
                validationErrors.Remove(propertyName);

            if(errors.Count > 0)
                validationErrors.Add(propertyName,errors.ToList());
        }

        protected void SetErrorChanged([CallerMemberName] string propertyName = null)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this,new DataErrorsChangedEventArgs(propertyName));

            if (NotifyOnErrorChanged != null)
                NotifyOnErrorChanged(this);
        }

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (String.IsNullOrWhiteSpace(propertyName) || !validationErrors.ContainsKey(propertyName))
                return null;

            return validationErrors[propertyName];
        }

        public bool HasErrors
        {
            get
            {
                return validationErrors.Values.Where(x => x.Count > 0).Any();
            }
        }
    }
}
