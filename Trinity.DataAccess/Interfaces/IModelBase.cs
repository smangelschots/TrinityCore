﻿using System.Collections.Generic;
using System.ComponentModel;

namespace Trinity.DataAccess.Interfaces
{
    public interface IModelBase : INotifyPropertyChanging, INotifyPropertyChanged, IEditableObject, IDataErrorInfo, IRevertibleChangeTracking
    {
        new event PropertyChangingEventHandler PropertyChanging;

        new event PropertyChangedEventHandler PropertyChanged;
        void SetColumnError(string columnName, string error);

        void SendPropertyChanging(string propertyName);

        void SendPropertyChanged(string propertyName);
        Dictionary<string, string> Errors { get; set; }
        Dictionary<string, object> OldValues { get; set; }

        IModelConfiguration Configuration { get; set; }


        bool HasErrors();
        void ClearErrors();
        List<string> GetProperties();
    }
}
