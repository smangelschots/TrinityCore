using System;
using System.Data;
using System.Linq.Expressions;
using Trinity.DataAccess.Interfaces;

namespace Trinity.DataAccess.Orm
{
    public class ColumnMap : IColumnMap
    {
        public ColumnMap()
        {
            Default = string.Empty;
        }

        public string ColumnName { get; set; }
        public SqlDbType SqlDbType { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public int Size { get; set; }
        public int OrdinalPosition { get; set; }
        public string Default { get; set; }
        public bool IsNullable { get; set; }
        public bool IsMapped { get; set; }
        public int Id { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsComputed { get; set; }
        public string PropertyName { get; set; }


    }
}