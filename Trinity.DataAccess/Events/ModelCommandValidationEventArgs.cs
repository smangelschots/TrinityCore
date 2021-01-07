using System;
using Trinity.DataAccess.Interfaces;

namespace Trinity.DataAccess.Events
{
    public class ModelCommandValidationEventArgs<T> : EventArgs
        where T : class
    {
        public IDataCommand<T> ModelCommand { get; set; }
    }
}