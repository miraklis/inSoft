using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace inCRM
{
    public class ValidatedViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        protected Dictionary<string, List<string>> validationErrors = new Dictionary<string, List<string>>();

        protected void SetErrors(string propertyName, List<string> propertyErrors)
        {
            ClearErrors(propertyName);
            if (propertyErrors.Count > 0)
                validationErrors.Add(propertyName, propertyErrors);
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected void ClearErrors(string propertyName)
        {
            validationErrors.Remove(propertyName);
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public bool HasErrors => validationErrors.Count > 0;
        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !validationErrors.ContainsKey(propertyName))
                return null;
            return validationErrors[propertyName];
        }
    }
}
