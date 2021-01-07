using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Trinity.DataAccess.Interfaces;
using Trinity.DataAccess.Orm;

namespace Trinity.DataAccess.Events
{
    public class ModelCommandBeforeExecuteEventArgs : EventArgs
    {
        public ModelCommandBeforeExecuteEventArgs(IDataCommand dataCommand, IDbCommand command, DataCommandType commandType)
        {
            DataCommand = dataCommand;
            Command = command;
            CommandType = commandType;
        }



        public IDbCommand Command { get; }
        public IDataCommand DataCommand { get; }
        public DataCommandType CommandType { get; }
    }
}
