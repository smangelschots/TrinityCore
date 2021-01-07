using System;
using System.Reflection;
using Trinity.DataAccess.Logging;

namespace Trinity.DataAccess.Interfaces
{
    public interface IMapper
    {
        void GetTableInfo(Type type, TableInfo tableInfo);
        bool MapPropertyToColumn(PropertyInfo pi, ref string columnName, ref bool resultColumn);
        Func<object, object> GetFromDbConverter(PropertyInfo propertyInfo, Type SourceType);
        Func<object, object> GetToDbConverter(Type SourceType);
    }
}