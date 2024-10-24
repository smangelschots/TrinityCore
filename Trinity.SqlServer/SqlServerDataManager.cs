using FastMember;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Trinity.DataAccess;
using Trinity.DataAccess.Events;
using Trinity.DataAccess.Extentions;
using Trinity.DataAccess.Interfaces;
using Trinity.DataAccess.Logging;
using Trinity.DataAccess.Models;
using Trinity.DataAccess.Orm;
using Trinity.DataAccess.Results;
using Westwind.Utilities;
using Westwind.Utilities.Properties;
using DataAccessProviderTypes = Trinity.DataAccess.DataAccessProviderTypes;

namespace Trinity.SqlServer
{
    public class SqlServerDataManager<T> : DataManager<SqlModelCommand<T>>, IModelCommand<T>
        where T : class
    {
        public ModelConfiguration<T> ModelConfiguration { get; set; }

        public SqlServerDataManager(string connectionString)
            : this(connectionString, "System.Data.SqlClient")
        {
        }

        public SqlServerDataManager(string connectionString, string providerName)
            : base(connectionString, providerName)
        {
            if (DataContext.TableMaps == null)
            {
                this.TableMaps = new Dictionary<string, TableMap>();
                DataContext.TableMaps = this.TableMaps;
            }
            else
            {
                this.TableMaps = DataContext.TableMaps;
            }
            TableMapFromDatabase = true;
        }

        public void CreateDatabaseIfNotExist()
        {
            //// function to create database if not exist
            //if (this.TableMaps == null) { return; }
            //foreach (var tableMap in this.TableMaps)
            //{
            //    var map = tableMap.Value;
            //    if (map.TableType == "TABLE")
            //    {
            //        var sql = string.Format("IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{0}' and xtype='U') CREATE TABLE {0} (", map.TableName);
            //        foreach (var column in map.ColumnMaps)
            //        {
            //            sql += string.Format("{0} {1},", column.ColumnName, column.);
            //        }
            //        sql = sql.TrimEnd(',');
            //        sql += ")";
            //        this.Execute(sql);
            //    }
            //}

        }

        public IDataCommand CreateUpdateTable(TableMap tableMap)
        {

            var dataCommand = this.CreateUpdateTable();
            dataCommand.TableMap = tableMap;
            return dataCommand;
        }

        public override void CreateTransaction()
        {
            var conn = Connection as SqlConnection;


            if (conn != null)
                this.Transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public IDataCommand<T> Track(T model)
        {
            var dataCommand = this.Update();
            dataCommand.Model = model;
            dataCommand.Track = true;
            var errorInfo = model as IDataErrorInfo;
            if (errorInfo != null)
            {
                dataCommand.OnSetValidation(new ModelCommandValidationEventArgs<T> { ModelCommand = dataCommand });
            }


            if (model is IModelBase)
            {
                var item = model as IModelBase;
                item.PropertyChanging += (sender, args) =>
                {

                    if (model is IObjectDataManager)
                    {
                        var manager = model as IObjectDataManager;
                        var value = manager.GetData(args.PropertyName);

                        if (item.OldValues.ContainsKey(args.PropertyName))
                            item.OldValues[args.PropertyName] = value;
                        else
                        {
                            item.OldValues.Add(args.PropertyName, value);
                        }
                    }

                    //TODO imp 

                };


            }

            if (model is INotifyPropertyChanged)
            {
                var item = model as INotifyPropertyChanged;
                item.PropertyChanged += (e, r) =>
                {
                    var property = item.GetType().GetProperty(r.PropertyName);
                    dataCommand.AddPropertyChange(property);
                };
            }

            return dataCommand;
        }

        public IDataCommand<T> Insert(T model)
        {
            var dataCommand = this.Insert();
            dataCommand.Model = model;
            var errorInfo = model;
            if (errorInfo != null)
            {
                dataCommand.OnSetValidation(new ModelCommandValidationEventArgs<T> { ModelCommand = dataCommand });
            }
            return dataCommand;
        }

        public IDataCommand<T> Update(T model)
        {
            var dataCommand = this.Update();
            dataCommand.Model = model;
            var errorInfo = model;
            if (errorInfo != null)
            {
                dataCommand.OnSetValidation(new ModelCommandValidationEventArgs<T> { ModelCommand = dataCommand });
            }
            return dataCommand;
        }

        protected override ICommandResult ExecuteDeleteCommand(SqlModelCommand<T> dataCommand, IDbCommand command)
        {
            var result = new ModelCommandResult<T>();
            if (TableMapFromDatabase)
                dataCommand.GetTableMap();

            dataCommand.BuildSqlParameters(command);
            dataCommand.BuildSqlCommand();
            command.CommandText = dataCommand.SqlCommandText;

            OnBeforeCommandExecute(this, new ModelCommandBeforeExecuteEventArgs(dataCommand, command, DataCommandType.Delete));

            if (command.CommandText.ToUpper().Contains("WHERE"))
            {
                int records = command.ExecuteNonQuery();
                result.RecordsAffected = records;
                result.AddMessage(string.Format("{0} executed with {1} rows affected", dataCommand.SqlCommandText,
                    records));
                dataCommand.ResetCommand();
            }
            else
            {
                result.AddError(LogType.Information, "No where in delete " + command.CommandText);
            }
            result.DataCommand = dataCommand;

            return result;
        }



        protected override ICommandResult ExecuteUpdateCommand(SqlModelCommand<T> dataCommand, IDbCommand command)
        {

            var result = new ModelCommandResult<T>();
            var item = dataCommand.Model as IModelBase;
            //TODO beforeExectute
            if (item != null)
            {
                if (item.Error == null)
                    if (item.HasErrors())
                    {
                        result.AddError(LogType.Error, "Model has validation error");
                        return result;
                    }
            }
            if (TableMapFromDatabase)
                dataCommand.GetTableMap();


            if (dataCommand.TableMap.TableType == "VIEW")
            {

                if (item.Configuration == null)
                {
                    result.AddMessage(string.Format("The command is type of View and has no merge configuration"));
                    return result;
                }
            }
            else
            {
                if (dataCommand.Track == false)
                {
                    dataCommand.AddChanges();
                }
                dataCommand.BuildKeys();
                foreach (var change in dataCommand.Changes)
                {
                    if (dataCommand.TableMap != null)
                    {
                        var column = dataCommand.TableMap.ColumnMaps.FirstOrDefault(m => m.PropertyName == change);

                        if (column == null)
                            column = dataCommand.TableMap.ColumnMaps.FirstOrDefault(m => m.ColumnName == change);

                        if (column?.IsIdentity == false)
                        {
                            dataCommand.Value(column.ColumnName, dataCommand.GetValue(change));
                        }
                    }
                    else
                    {
                        dataCommand.Value(change, dataCommand.GetValue(change));
                    }
                }

                OnBeforeCommandExecute(this, new ModelCommandBeforeExecuteEventArgs(dataCommand, command, DataCommandType.Update));

                if (dataCommand.Columns.Count > 0 && dataCommand.Changes.Count > 0)
                {
                    dataCommand.BuildSqlParameters(command);
                    dataCommand.BuildSqlCommand();
                    command.CommandText = dataCommand.SqlCommandText;



                    if (command.CommandText.Contains("WHERE"))
                    {
                        int resultIndex = command.ExecuteNonQuery();
                        result.RecordsAffected = resultIndex;
                        if (resultIndex == 0)
                        {
                            result.AddError(LogType.Information, "No rows affected");
                        }
                        else
                        {
                            dataCommand.ResetCommand();
                        }
                    }
                    else
                    {
                        result.AddError(LogType.Information, "No where in update");
                    }
                    result.AddMessage(string.Format("{0} executed with {1} rows affected", command.CommandText,
                        result.RecordsAffected));
                }
                result.DataCommand = dataCommand;

            }
            return result;

        }


        protected override ICommandResult ExecuteInsertCommand(SqlModelCommand<T> dataCommand, IDbCommand command)
        {
            var result = new ModelCommandResult<T>();
            var item = dataCommand.Model as IModelBase;

            if (item != null)
            {
                if (item.Errors != null)
                    if (item.HasErrors())
                    {
                        result.AddError(LogType.Error, "Model has validation error");
                        return result;
                    }
            }
            if (TableMapFromDatabase)
                dataCommand.GetTableMap();


            var select = string.Empty;
            bool identity = false;
            var retunColums = new Dictionary<string, string>();
            dataCommand.AddChanges();
            foreach (var key in dataCommand.PrimaryKeys)
            {
                var column = dataCommand.TableMap.ColumnMaps.FirstOrDefault(m => m.PropertyName == key);

                if (column == null)
                    column = dataCommand.TableMap.ColumnMaps.FirstOrDefault(m => m.ColumnName == key);

                if (column != null)
                {
                    if (column.IsIdentity)
                    {
                        select += string.Format(" {0},", column.ColumnName);
                        identity = true;
                        retunColums.Add(column.ColumnName, column.PropertyName);
                        if (dataCommand.Changes.Contains(column.PropertyName))
                        {
                            dataCommand.Changes.Remove(column.PropertyName);
                        }
                    }
                }
            }

            var guidIdColumns =
                dataCommand.TableMap.ColumnMaps.Where(map => !string.IsNullOrEmpty(map.Default))
                    .Where(
                        map =>
                            map.Default.ToUpper().Contains("NEWSEQUENTIALID")
                            || map.Default.ToUpper().ToUpper().Contains("NEWID"))
                    .ToList();
            foreach (var guidIdColumn in guidIdColumns)
            {
                select += string.Format(" {0},", guidIdColumn.ColumnName);
                retunColums.Add(guidIdColumn.ColumnName, guidIdColumn.PropertyName);
            }

            if (identity)
            {
                dataCommand.SetWhereText(string.Format(
                    " SELECT SCOPE_IDENTITY() as [{0}]", retunColums.FirstOrDefault().Key
                ));
            }


            foreach (var change in dataCommand.Changes)
            {
                var column = dataCommand.TableMap.ColumnMaps.FirstOrDefault(m => m.PropertyName == change);
                if (column != null)
                {
                    dataCommand.Value(column.ColumnName, dataCommand.GetValue(change));
                }
            }

            dataCommand.BuildSqlParameters(command);
            dataCommand.BuildSqlCommand();
            command.CommandText = dataCommand.SqlCommandText;

            OnBeforeCommandExecute(this, new ModelCommandBeforeExecuteEventArgs(dataCommand, command, DataCommandType.Insert));


            using (var dataReader = command.ExecuteReader())
            {


                result.RecordsAffected = dataReader.RecordsAffected;
                if (dataReader.RecordsAffected == 0)
                {
                    result.AddError(LogType.Information, "No rows affected");
                }
                else
                {

                    dataCommand.SetWhereText(string.Empty);
                    dataCommand.CommandType = DataCommandType.Update;


                }
                while (dataReader.Read())
                {

                    for (int i = 0; i < 1; ++i)
                    {
                        var name = dataReader.GetName(i);
                        var property = retunColums[name];
                        dataCommand.SetValue(property, dataReader.GetValue(i).ToInt());
                    }
                }

            }
            //TODO error when transaction is full 
            result.AddMessage(string.Format("{0} executed with {1} rows affected", dataCommand.SqlCommandText,
                result.RecordsAffected));
            dataCommand.ResetCommand();
            result.DataCommand = dataCommand;
            return result;
        }

        protected override async Task<ICommandResult> ExecuteSelectCommandAsync(SqlModelCommand<T> dataCommand,
            IDbCommand command)
        {

            if (TableMapFromDatabase)
                dataCommand.GetTableMap();

            dataCommand.BuildSqlCommand();
            dataCommand.BuildSqlParameters(command);
            command.CommandText = dataCommand.SqlCommandText;
            var items = new List<T>();
            ICommandResult result = null;

            try
            {
                int rowsIndex = 0;
                var sqlCommand = command as SqlCommand;

                OnBeforeCommandExecute(this, new ModelCommandBeforeExecuteEventArgs(dataCommand, command, DataCommandType.Select));

                using (SqlDataReader r = await sqlCommand.ExecuteReaderAsync())
                {


                    Type objectType = typeof(T);

                    if (objectType == typeof(DataTable))
                    {

                        result = new DataTableCommandResult();
                        var dt = new DataTable(dataCommand.TableName);
                        dt.Load(r);
                        ((DataTableCommandResult)result).Data = dt;
                        result.RecordsAffected = dt.Rows.Count;

                        return result;
                    }
                    result = new ModelCommandResult<T>();

                    var tablename = dataCommand.GetTableAttribute();
                    while (await r.ReadAsync())
                    {
                        bool userDataManager = false;
                        var newObject = (T)Activator.CreateInstance(objectType);
                        var dataManager = newObject as IObjectDataManager;

                        if (dataManager != null)
                            userDataManager = true;

                        if (dataCommand.TableMap != null)
                            if (dataCommand.TableName != objectType.Name)
                            {
                                if (tablename == dataCommand.TableName)
                                    if (dataManager != null)
                                        userDataManager = true;
                                    else
                                        userDataManager = false;
                            }

                        if (userDataManager)
                        {
                            dataManager.SetData(r);
                        }
                        else
                        {

                            //TODO Change code to use Reflection.Emit 
                            int counter = r.FieldCount;
                            for (int i = 0; i < counter; i++)
                            {
                                var name = r.GetName(i);
                                try
                                {
                                    var fieldType = r.GetFieldType(i);
                                    var value = r.GetValue(i);
                                    if (dataCommand.TableMap != null)
                                    {
                                        var column =
                                            dataCommand.TableMap.ColumnMaps.FirstOrDefault(m => m.ColumnName == name);

                                        if (column != null)
                                        {
                                            if (!string.IsNullOrEmpty(column.PropertyName))
                                            {
                                                var prop = objectType.GetProperty(column.PropertyName);
                                                if (prop != null)
                                                {
                                                    if (value != DBNull.Value)
                                                    {
                                                        prop.SetValue(newObject, value, null);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                TrySetValue(dataCommand, newObject, objectType, name, value);
                                            }

                                        }
                                        else
                                        {
                                            TrySetValue(dataCommand, newObject, objectType, name, value);
                                        }
                                    }
                                    else
                                    {
                                        var prop = objectType.GetProperty(name);
                                        if (prop != null)
                                        {
                                            if (value != DBNull.Value)
                                            {
                                                prop.SetValue(newObject, value, null);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    result.AddError(LogType.Error,
                                        string.Format("{1} {0} ", name, dataCommand.TableName), ex);
                                }
                            }
                        }
                        items.Add(newObject);
                        rowsIndex++;
                    }
                }
                result.RecordsAffected = rowsIndex;
            }
            catch (Exception exception)
            {
                result.AddError(LogType.Error, dataCommand.TableName + " " + typeof(T).FullName, exception);
            }

            ((ModelCommandResult<T>)result).Data = items;
            result.AddMessage(string.Format("{0} executed with {1} rows affected", dataCommand.SqlCommandText,
                result.RecordsAffected));
            //TODO change class to use base type
            dataCommand.OnCommandExecuted(new ModelCommandExecutedEventArgs<T> { Result = (ModelCommandResult<T>)result });
            dataCommand.ResetCommand();
            result.DataCommand = dataCommand;
            return result;
        }



        protected override ICommandResult ExecuteSelectCommand(SqlModelCommand<T> dataCommand, IDbCommand command)
        {

            if (TableMapFromDatabase)
            {

                var map = dataCommand.GetTableMap();
                if (dataCommand.SelectAll == false)
                {

                    if (!string.IsNullOrEmpty(map))
                    {
                        var columns = dataCommand.GetColumnAttributes();
                        foreach (var columnMap in columns)
                        {
                            bool addColumn = true;
                            if (dataCommand.TableMap == null)
                                addColumn = false;

                            var column = dataCommand.TableMap.ColumnMaps.FirstOrDefault(m => m.ColumnName == columnMap.ColumnName);
                            if (column == null)
                                addColumn = false;


                            if (addColumn)
                                dataCommand.Column(columnMap.ColumnName);
                        }
                        dataCommand.SelectAll = false;
                    }

                }

            }


            dataCommand.BuildSqlCommand();
            dataCommand.BuildSqlParameters(command);
            command.CommandText = dataCommand.SqlCommandText;
            var items = new List<T>();
            ICommandResult result = null;
            try
            {
                int rowsIndex = 0;


                OnBeforeCommandExecute(this, new ModelCommandBeforeExecuteEventArgs(dataCommand, command, DataCommandType.Select));
                using (SqlDataReader r = command.ExecuteReader() as SqlDataReader)
                {

                    Type objectType = typeof(T);

                    if (objectType == typeof(DataTable))
                    {
                        result = new DataTableCommandResult();
                        var dt = new DataTable(dataCommand.TableName);
                        dt.Load(r);


                        ((DataTableCommandResult)result).Data = dt;
                        result.RecordsAffected = dt.Rows.Count;

                        foreach (var item in dataCommand.TableMap.ColumnMaps)
                        {
                            if (item.ColumnName != item.PropertyName)
                            {

                                foreach (System.Data.DataColumn column in dt.Columns)
                                {
                                    if (item.ColumnName == column.ColumnName)
                                    {
                                        if (!string.IsNullOrEmpty(item.PropertyName))
                                            if (item.IsMapped)
                                                column.ColumnName = item.PropertyName;
                                    }
                                }
                            }
                        }

                        return result;
                    }
                    result = new ModelCommandResult<T>();

                    var tablename = dataCommand.GetTableAttribute();

                    var accessor = TypeAccessor.Create(objectType);

                    ConstructorInfo ctor = objectType.GetConstructor(Type.EmptyTypes);
                    TrinityActivator.ObjectActivator<T> createdActivator = TrinityActivator.GetActivator<T>(ctor);

                    while (r.Read())
                    {
                        bool userDataManager = false;

                        //https://vagifabilov.wordpress.com/2010/04/02/dont-use-activator-createinstance-or-constructorinfo-invoke-use-compiled-lambda-expressions/

                        var newObject = createdActivator();
                        //var newObject = (T)Activator.CreateInstance(objectType);
                        var dataManager = newObject as IObjectDataManager;


                        if (dataCommand.TableMap != null)
                            if (dataCommand.TableName != objectType.Name)
                            {
                                if (tablename == dataCommand.TableName)
                                    if (dataManager != null)
                                        userDataManager = true;
                                    else
                                        userDataManager = false;
                            }
                            else
                            {

                                if (dataManager != null)
                                    userDataManager = true;
                            }

                        if (userDataManager)
                        {
                            dataManager.SetData(r);

                        }
                        else
                        {

                            //TODO Change code to use Reflection.Emit  
                            int counter = r.FieldCount;
                            for (int i = 0; i < counter; i++)
                            {
                                var name = r.GetName(i);
                                try
                                {
                                    var fieldType = r.GetFieldType(i);
                                    var value = r.GetValue(i);
                                    if (dataCommand.TableMap != null)
                                    {
                                        var column =
                                            dataCommand.TableMap.ColumnMaps.FirstOrDefault(m => m.ColumnName == name);

                                        if (column != null)
                                        {
                                            try
                                            {
                                                if (!accessor.TrySetValue(result, newObject, column, value))
                                                {
                                                    TrySetValue(dataCommand, newObject, objectType, name, value);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                TrySetValue(dataCommand, newObject, objectType, name, value);
                                                throw new ApplicationException("error in manager", ex);
                                            }
                                        }
                                        else
                                        {
                                            if (!accessor.TrySetValue(result, newObject, column, value, name))
                                            {
                                                TrySetValue(dataCommand, newObject, objectType, name, value);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var prop = objectType.GetProperty(name);
                                        if (prop != null)
                                        {
                                            if (value != DBNull.Value)
                                            {
                                                prop.SetValue(newObject, value, null);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    result.AddWaring(string.Format("{1} {0} ", name, dataCommand.TableName), ex);
                                }
                            }
                        }
                        items.Add(newObject);
                        rowsIndex++;
                    }
                }
                result.RecordsAffected = rowsIndex;
            }
            catch (Exception exception)
            {
                result.AddError(LogType.Error, $"{dataCommand.TableName} {dataCommand.SqlCommandText} {typeof(T).FullName}", exception);
            }
            ((ModelCommandResult<T>)result).Data = items;
            result.AddMessage(string.Format("{0} executed with {1} rows affected", dataCommand.SqlCommandText, result.RecordsAffected));
            //TODO change class to use base type


            dataCommand.OnCommandExecuted(new ModelCommandExecutedEventArgs<T> { Result = (ModelCommandResult<T>)result });

            dataCommand.ResetCommand();
            result.DataCommand = dataCommand;
            return result;
        }




        private void TrySetValue(IDataCommand dataCommand, object newObject, Type objectType, string name, object value)
        {
            var prop = objectType.GetProperty(name);
            if (prop != null)
            {
                if (value != DBNull.Value)
                {
                    prop.SetValue(newObject, value, null);
                }
            }
            else
            {
                PropertyInfo[] properties = objectType.GetProperties();
                var index = properties.Count();
                for (int j = 0; j < index; j++)
                {
                    var props = properties[j];
                    if (props.CanWrite)
                    {
                        var propName = props.Name.ToLower();
                        var columnName = name.ToLower();
                        if (propName == columnName)
                        {

                            if (value != DBNull.Value)
                                props.SetValue(newObject, value, null);

                            var column = dataCommand.TableMap.ColumnMaps.FirstOrDefault(m => m.ColumnName == name);
                            if (column == null)
                            {
                                dataCommand.TableMap.ColumnMaps.Add(new SqlColumnMap()
                                {
                                    ColumnName = name,
                                    PropertyName = props.Name
                                });
                            }

                            else
                            {
                                column.PropertyName = props.Name;
                            }
                            break;
                        }
                    }
                }
            }
        }

        public override DbProviderFactory GetDbProviderFactory(string providerName)
        {

            return GetDbProviderFactoryBase(providerName);
            return DbProviderFactories.GetFactory(providerName);
        }



        public DbProviderFactory GetDbProviderFactory(string dbProviderFactoryTypename, string assemblyName)
        {

            //https://weblog.west-wind.com/posts/2017/nov/27/working-around-the-lack-of-dynamic-dbproviderfactory-loading-in-net-core
            var instance = ReflectionUtils.GetStaticProperty(dbProviderFactoryTypename, "Instance");
            if (instance == null)
            {
                var a = ReflectionUtils.LoadAssembly(assemblyName);
                if (a != null)
                    instance = ReflectionUtils.GetStaticProperty(dbProviderFactoryTypename, "Instance");
            }

            if (instance == null)
                throw new InvalidOperationException(string.Format(Resources.UnableToRetrieveDbProviderFactoryForm, dbProviderFactoryTypename));

            return instance as DbProviderFactory;
        }

        public DbProviderFactory GetDbProviderFactory(DataAccessProviderTypes type)
        {

            //TODO change or put all in one
            if (type == DataAccessProviderTypes.SqlServer)
                return SqlClientFactory.Instance; // this library has a ref to SqlClient so this works

            if (type == DataAccessProviderTypes.SqLite)
            {

                return GetDbProviderFactory("Microsoft.Data.Sqlite.SqliteFactory", "Microsoft.Data.Sqlite");
            }
            if (type == DataAccessProviderTypes.MySql)
                return GetDbProviderFactory("MySql.Data.MySqlClient.MySqlClientFactory", "MySql.Data");
            if (type == DataAccessProviderTypes.PostgreSql)
                return GetDbProviderFactory("Npgsql.NpgsqlFactory", "Npgsql");


            throw new NotSupportedException(string.Format(Resources.UnsupportedProviderFactory, type.ToString()));
        }

        public DbProviderFactory GetDbProviderFactoryBase(string providerName)
        {

            var providername = providerName.ToLower();

            if (providername == "system.data.sqlclient")
                return GetDbProviderFactory(DataAccessProviderTypes.SqlServer);
            if (providername == "system.data.sqlite" || providername == "microsoft.data.sqlite")
                return GetDbProviderFactory(DataAccessProviderTypes.SqLite);
            if (providername == "mysql.data.mysqlclient" || providername == "mysql.data")
                return GetDbProviderFactory(DataAccessProviderTypes.MySql);
            if (providername == "npgsql")
                return GetDbProviderFactory(DataAccessProviderTypes.PostgreSql);

            throw new NotSupportedException(string.Format("", providerName));
        }

        protected override Task<ICommandResult> ExecuteDeleteCommandAsync(SqlModelCommand<T> dataCommand, IDbCommand command)
        {
            throw new NotImplementedException();
        }

        protected override Task<ICommandResult> ExecuteUpdateCommandAsync(SqlModelCommand<T> dataCommand, IDbCommand command)
        {
            throw new NotImplementedException();
        }

        protected override Task<ICommandResult> ExecuteInsertCommandAsync(SqlModelCommand<T> dataCommand, IDbCommand command)
        {
            throw new NotImplementedException();
        }

        protected override ICommandResult ExecuteTableCreateUpdate(SqlModelCommand<T> dataCommand, IDbCommand command)
        {
            throw new NotImplementedException();
        }
    }
}