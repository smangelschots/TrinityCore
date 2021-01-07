using System;
using Trinity.DataAccess;
using Trinity.DataAccess.Orm;
using Trinity.SqlServer;

namespace Trinity.SqlServer
{
    public class SqlServerDataContext : DataContext
    {

        public SqlServerDataContext(string connectionString)
        {
            conn = connectionString;
        }

        private string conn;

        public void Init()
        {
            //var manager = new SqlServerDataManager<ApplicationModel>(conn);
            //manager.CreateUpdateTable(CreateMap<ApplicationModel>().AddPrimaryKey(m => m.Id));
            //manager.CreateUpdateTable(CreateMap<ObjectModel>().AddPrimaryKey(m => m.Id));

            //OnModelCreating(manager);

        }

        //public virtual void OnModelCreating(SqlServerDataManager<ApplicationModel> manager)
        //{



        //}


        private void CommitTableMap()
        {


            //dataCommand.SaveChanges();

        }


        public SqlTableMap<T> CreateMap<T>() where T : class
        {
            var tableMap = GetTableMap<T>();
            return tableMap;
        }


        public SqlTableMap<T> CreateMap<T>(string name)
        {
            var tableMap = new SqlTableMap<T>();
            tableMap.TableName = name;
            

            return tableMap;
        }



        public virtual SqlTableMap<T> GetTableMap<T>() where T : class
        {
            var map = new SqlTableMap<T>();
            map.TableName = GetTableAttribute<T>();
            var propertyMap = this.GetColumnAttributes<T>();

            foreach (ColumnMap column in propertyMap)
            {
                map.ColumnMaps.Add(new SqlColumnMap()
                {
                    ColumnName = column.ColumnName,
                    PropertyName = column.PropertyName,
                    DbType = column.SqlDbType,
                    Size = column.Size
                });
            }

            return map;

        }


      


    }
}