using System;

namespace TrinityCore
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableConfigurationAttribute : Attribute, ITableConfigurationAttribute
    {
        public TableConfigurationAttribute(string tableName)
        {
            this.TableName = tableName;
        }
        public string TableName { get; private set; }
    }
}