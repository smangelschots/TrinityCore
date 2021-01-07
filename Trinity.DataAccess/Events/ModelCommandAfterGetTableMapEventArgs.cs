using System;
using System.Collections.Generic;
using System.Text;
using Trinity.DataAccess.Orm;

namespace Trinity.DataAccess.Events
{
    public class ModelCommandAfterGetTableMapEventArgs : EventArgs
    {
        public ModelCommandAfterGetTableMapEventArgs(TableMap tableMap, string modelName)
        {
            TableMap = tableMap;
            ModelName = modelName;
        }

        public TableMap TableMap { get; }
        public string ModelName { get; }
    }
}
