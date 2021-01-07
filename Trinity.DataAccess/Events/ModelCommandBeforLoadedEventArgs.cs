using System;
using System.Collections.Generic;
using System.Text;
using Trinity.DataAccess.Interfaces;
using Trinity.DataAccess.Orm;

namespace Trinity.DataAccess.Events
{
    public class ModelCommandBeforeLoadedEventArgs : EventArgs
    {
        public ModelCommandBeforeLoadedEventArgs(IDataCommand[] commands, DataCommandType commandType)
        {
            Commands = commands;
            CommandType = commandType;
        }

        public IDataCommand[] Commands { get; }
        public DataCommandType CommandType { get; }
    }
}
