using System;
using Trinity.DataAccess.Results;

namespace Trinity.DataAccess.Events
{
    public class ModelCommandExecutedEventArgs<T> : EventArgs
        where T : class
    {
        public ModelCommandResult<T> Result { get; set; }
    }
}