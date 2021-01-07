using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.DataAccess.Interfaces;

namespace Trinity.DataAccess.Orm
{


    public class TableMap<T> : TableMap where T : IColumnMap
    {



        public TableMap<T> MapColumn(string columnName, string propertyName = "", Action<T> mapFunction = null)
        {

            var map = GetColumnMap(columnName);

            if (map == null)
            {
                map = (T)Activator.CreateInstance(typeof(T));
                map.ColumnName = columnName;
                map.PropertyName = propertyName;
                this.ColumnMaps.Add(map);
            }

            if (mapFunction != null)
            {
                mapFunction.Invoke((T)map);
            }
            return this;
        }



    }
    public class TableMap
    {
        public TableMap()
        {
            this.ColumnMaps = new List<IColumnMap>();
            this.KeyMaps = new List<KeyMap>();
        }
        public string Schema { get; set; }
        public string TableName { get; set; }
        public string TableType { get; set; }
        public string Catalog { get; set; }
        public List<IColumnMap> ColumnMaps { get; set; }
        public List<KeyMap> KeyMaps { get; set; }
        public int Id { get; set; }
        public bool Mapped { get; set; }

        public bool ColumnsMapped
        {
            get
            {

                if (ColumnMaps != null)
                    foreach (var item in ColumnMaps)
                    {
                        if (item.IsMapped == false)
                        {
                            return false;
                        }
                    }
                return true;

            }
        }

        public List<string> GetPrimaryKeys()
        {

            var items = new List<string>();

            foreach (var item in this.KeyMaps)
            {
                if (item.KeyType == KeyMapType.PrimaryKey)
                {
                    items.Add(item.PropertyName);
                }
            }
            return items;
        }

        public TableMap MapColumn(string name, Func<IColumnMap, IColumnMap> mapFunction)
        {
            var tempMap = GetTableMapByColumnName(name);
            if (tempMap == null)
            {
                tempMap = GetTableMapByPropertyName(name);
            }
            if (tempMap == null)
            {
                var tempMapResult = mapFunction.Invoke(null);
                this.ColumnMaps.Add(tempMapResult);
            }
            else
            {
                var tempMapResult = mapFunction.Invoke(tempMap);
            }
            return this;
        }



        public static TableMap Create(string name)
        {
            var map = new TableMap()
            {
                TableName = name
            };

            return map;

        }

        public IColumnMap GetColumnMap(string name)
        {
            var tempMap = GetTableMapByColumnName(name);
            if (tempMap == null)
            {
                tempMap = GetTableMapByPropertyName(name);
            }
            return tempMap;
        }




        public IColumnMap GetTableMapByColumnName(string columnName)
        {
            return this.ColumnMaps.FirstOrDefault(m => m.ColumnName == columnName);
        }

        public IColumnMap GetTableMapByPropertyName(string propertyName)
        {
            return this.ColumnMaps.FirstOrDefault(m => m.PropertyName == propertyName);
        }

    }
}