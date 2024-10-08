using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Trinity.DataAccess.Attributes;
using Trinity.DataAccess.Events;
using Trinity.DataAccess.Interfaces;

namespace Trinity.DataAccess.Models
{
    public delegate void EditEventHandler(object sender, EditEventHandlerArgs args);

    [Serializable]
    public abstract class ModelBase : IModelBase, IModelConfigurationManager
    {
        [Bindable(false)]
        [Browsable(false)]
        [Ignore]
        public Dictionary<string, object> OldValues { get; set; }

        [Bindable(false)]
        [Browsable(false)]
        [Ignore]
        public Dictionary<string, string> Errors { get; set; }

        [Bindable(false)]
        [Browsable(false)]
        [Ignore]
        public string RowError { get; set; }

        [Bindable(false)]
        [Browsable(false)]
        [Ignore]
        public string Error { get; set; }


        protected ModelBase()
        {
            Errors = new Dictionary<string, string>();
            OldValues = new Dictionary<string, object>();
        }

        public string this[string columnName]
        {
            get
            {
                if (Errors.ContainsKey(columnName))
                    return Errors[columnName];
                return string.Empty;
            }
        }

        public bool HasErrors()
        {
            if (Errors.Count > 0) return true;
            return false;
        }

        public void ClearErrors()
        {
            Errors.Clear();
            RowError = string.Empty;
        }

        public virtual void SetColumnError(string columnName, string error)
        {
            if (Errors.ContainsKey(columnName) == false)
                Errors.Add(columnName, error);
            else
                Errors[columnName] = error;
        }

        [field: NonSerialized]
        public event PropertyChangingEventHandler PropertyChanging;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized]
        public event EditEventHandler EditObject;


        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual void SendPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual void SendPropertyChanging([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanging != null)
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));

        }






        public void BeginEdit()
        {
            if (EditObject != null)
                EditObject(this, new EditEventHandlerArgs(ModelEditType.Begin));

        }

        public void EndEdit()
        {
            if (EditObject != null)
                EditObject(this, new EditEventHandlerArgs(ModelEditType.End));
        }

        public void CancelEdit()
        {
            if (EditObject != null)
                EditObject(this, new EditEventHandlerArgs(ModelEditType.Cancel));
        }

        public void AcceptChanges()
        {
            OldValues.Clear();
        }

        [Bindable(false)]
        [Browsable(false)]
        [Ignore]
        public bool IsChanged { get; private set; }

        public void RejectChanges()
        {
            throw new NotImplementedException("Implement ChangeTracking");
            //TODO Implement ChangeTracking
        }

        public abstract List<string> GetProperties();


        [Bindable(false)]
        [Browsable(false)]
        [Ignore]
        public IModelConfiguration Configuration { get; set; }
    }
}