﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Trinity.DataAccess.Attributes;
using Trinity.DataAccess.Events;
using Trinity.DataAccess.Interfaces;

namespace Trinity.DataAccess.LobModels
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
            this.Errors = new Dictionary<string, string>();
            this.OldValues = new Dictionary<string, object>();
        }

        public string this[string columnName]
        {
            get
            {
                if (this.Errors.ContainsKey(columnName))
                    return this.Errors[columnName];
                return string.Empty;
            }
        }

        public bool HasErrors()
        {
            if (this.Errors.Count > 0) return true;
            return false;
        }

        public void ClearErrors()
        {
            this.Errors.Clear();
            this.RowError = string.Empty;
        }

        public virtual void SetColumnError(string columnName, string error)
        {
            if (this.Errors.ContainsKey(columnName) == false)
                this.Errors.Add(columnName, error);
            else
                this.Errors[columnName] = error;
        }

        [field: NonSerialized]
        public event PropertyChangingEventHandler PropertyChanging;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized]
        public event EditEventHandler EditObject;


        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual void SendPropertyChanged([CallerMemberName]  string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual void SendPropertyChanging([CallerMemberName]  string propertyName = "")
        {
            if (this.PropertyChanging != null)
                this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));

        }






        public void BeginEdit()
        {
            if (this.EditObject != null)
                this.EditObject(this, new EditEventHandlerArgs(ModelEditType.Begin));

        }

        public void EndEdit()
        {
            if (this.EditObject != null)
                this.EditObject(this, new EditEventHandlerArgs(ModelEditType.End));
        }

        public void CancelEdit()
        {
            if (this.EditObject != null)
                this.EditObject(this, new EditEventHandlerArgs(ModelEditType.Cancel));
        }

        public void AcceptChanges()
        {
            this.OldValues.Clear();
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