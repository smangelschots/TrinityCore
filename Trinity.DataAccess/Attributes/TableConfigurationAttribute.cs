using System;
using Trinity.DataAccess.Interfaces;

namespace Trinity.DataAccess.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableConfigurationAttribute : Attribute, ITableConfigurationAttribute
    {
        public TableConfigurationAttribute(string mapToTableName)
        {
            this.TableName = mapToTableName;
        }
        public string TableName { get; private set; }
    }
}