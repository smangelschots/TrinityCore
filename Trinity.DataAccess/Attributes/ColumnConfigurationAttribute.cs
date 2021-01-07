using System;
using Trinity.DataAccess.Interfaces;

namespace Trinity.DataAccess.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnConfigurationAttribute : Attribute, IColumnAttribute
    {
        public string Name { get; set; }
        public string TableName { get; set; }
        public ColumnConfigurationAttribute() { }
        
        public ColumnConfigurationAttribute(string mapToColumnName)
        {
            this.Name = mapToColumnName;
        }

        public ColumnConfigurationAttribute(string mapToColumnName, string mapToTableName)
        {
            this.Name = mapToColumnName;
            this.TableName = mapToTableName;
        }
    }
}