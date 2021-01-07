using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Trinity.DataAccess.Interfaces;
using Trinity.DataAccess.Orm;

namespace Trinity.SqlServer
{
    public class SqlColumnMap : ColumnMap,  IColumnMap<SqlDbType>
    {
        public SqlColumnMap()
        {
            Default = string.Empty;
        }
        public SqlDbType DbType { get; set; }
    }
}
