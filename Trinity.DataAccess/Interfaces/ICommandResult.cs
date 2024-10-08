using System;
using System.Collections.Generic;
using System.Data;
using Trinity.DataAccess.Logging;
using Trinity.DataAccess.Orm;

namespace Trinity.DataAccess.Interfaces
{
    public interface ICommandResult
    {
        IDataCommand DataCommand { get; set; }
        int RecordsAffected { get; set; }
        bool HasErrors { get; }
        DataCommandType CommandType { get;}
        List<DataError> CommandErrors { get; set; }
        void AddError(LogType errorType, string message, Exception exception = null);
        List<string> Messages { get; set; }
        long ElapsedMilliseconds { get; set; }
        void AddMessage(string message);

        void AddWaring(string message, Exception ex = null);
        bool IsMapped { get;  }


    }
}