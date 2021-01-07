using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Trinity.DataAccess.Attributes;
using Trinity.DataAccess.Interfaces;
using Trinity.DataAccess.Orm;

namespace Trinity.DataAccess
{
    public class DataContext
    {
        public static Dictionary<string, TableMap> TableMaps { get; set; }


        public DataContext()
        {
            if (DataContext.TableMaps == null)
                DataContext.TableMaps = new Dictionary<string, TableMap>();
        }


        public virtual string GetTableAttribute<T>() where T : class
        {
            var modelType = typeof(T);
            var tableAttribute = modelType.GetCustomAttributes(typeof(TableConfigurationAttribute), false);
            if (tableAttribute.Any())
            {
                var table = tableAttribute.FirstOrDefault() as TableConfigurationAttribute;
                if (table != null)
                {
                    return table.TableName;
                }
            }
            return modelType.Name;
        }

        public virtual List<IColumnMap> GetColumnAttributes<T> (List<IColumnMap> columnMaps = null)
        {
            if (columnMaps == null)
                columnMaps = new List<IColumnMap>();
            var modelType = typeof(T);
            var properties = modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propertyInfo in properties)
            {
                var name = string.Empty;
                if (propertyInfo.CanWrite)
                {
                    var atribute = propertyInfo.GetCustomAttributes(typeof(IgnoreAttribute), false);
                    if (!atribute.Any())
                    {
                        var columAttribute = propertyInfo.GetCustomAttributes(
                            typeof(ColumnConfigurationAttribute),
                            false);

                        if (columAttribute.Any())
                        {
                            var colmn = columAttribute.FirstOrDefault() as ColumnConfigurationAttribute;
                            if (colmn != null)
                            {
                                name = colmn.Name;
                            }
                        }
                        else
                        {
                            name = propertyInfo.Name;
                        }
                        var columnMap = new ColumnMap();
                        columnMap.ColumnName = name;
                        columnMap.PropertyName = propertyInfo.Name;
                        if (columnMaps.FirstOrDefault(m => m.ColumnName == name) == null)
                            columnMaps.Add(columnMap);
                    }
                }
            }
            return columnMaps;
        }


        public DataContext CreateMap(string name, TableMap tableMap)
        {
            var existingMap = GetDataTable(name);

            if (existingMap == null)
            {
                DataContext.TableMaps.Add(name, tableMap);
            }
            return this;
        }


        public TableMap GetDataTable(string name)
        {
            if (DataContext.TableMaps.ContainsKey(name))
            {
                return DataContext.TableMaps[name];
            }

            return null;
        }
    }
}
