﻿
using System.Data;
using Trinity.DataAccess.Interfaces;

namespace Trinity.DataAccess.Orm
{
    public class DataParameter : ColumnMap , IParameter
    {
        public string Name { get; set; }

        public DataRowVersion SourceVersion { get; set; }

        public object Value { get; set; }

     //   public string ParameterName { get; set; }

        public string SourceColumn { get; set; }

        public ParameterDirection Direction { get; set; }

        public bool IsSelectParameter { get; set; }

        public DataParameter()
        {
            this.SqlDbType = SqlDbType.NVarChar;
            this.Direction = ParameterDirection.Input;
        }


    }
}
