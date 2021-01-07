
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Trinity.DataAccess.Events;
using Trinity.DataAccess.Interfaces;
using Trinity.DataAccess.Logging;
using Trinity.DataAccess.Orm;
using Trinity.DataAccess.Results;

namespace Trinity.DataAccess
{
    public enum DataAccessProviderTypes
    {
        SqlServer,
        SqLite,
        MySql,
        PostgreSql
    }

    public abstract class DataManager<T> : IDataManager<T> where T : IDataCommand
    {
        private DbProviderFactory _factory;


        public event EventHandler<ModelCommandPropertyChangedEventArgs> Validating;
        public event EventHandler<ModelCommandAfterGetTableMapEventArgs> AfterGetTableMap;
        public event EventHandler<ModelCommandBeforeExecuteEventArgs> BeforeCommandExecute;

        public event EventHandler<ModelCommandBeforeLoadedEventArgs> BeforeCommandsLoaded;

        public virtual void OnAfterTryGetTableMap(object sender, ModelCommandAfterGetTableMapEventArgs e)
        {
            AfterGetTableMap?.Invoke(sender, e);
        }

        public virtual void OnValidating(object sender, ModelCommandPropertyChangedEventArgs e)
        {
            Validating?.Invoke(sender, e);
        }
        public virtual void OnBeforeCommandExecute(object sender, ModelCommandBeforeExecuteEventArgs e)
        {
            BeforeCommandExecute?.Invoke(sender, e);
        }
        protected virtual void OnBeforeCommandsLoaded(object sender, ModelCommandBeforeLoadedEventArgs e)
        {
            BeforeCommandsLoaded?.Invoke(sender, e);
        }
        public bool TableMapFromDatabase { get; set; }

        public Dictionary<string, TableMap> TableMaps { get; set; }

        private DbConnection _connection;
        public DbTransaction Transaction { get; set; }
        private List<DataError> Errors { get; set; }

        protected List<IDataCommand> Commands { get; set; }
        public IDbConnection Connection
        {
            get
            {
                return this._connection;
            }
        }
        public string ConnectionString { get; set; }
        public string ProviderName { get; }
        protected void OnException(Exception x)
        {
            System.Diagnostics.Debug.WriteLine(x.ToString());
        }

        protected DataManager(string connectionString, string providerName) : this()
        {
            this.ConnectionString = connectionString;
            ProviderName = providerName;
            this._factory = GetDbProviderFactory(providerName);
        }

        public abstract DbProviderFactory GetDbProviderFactory(string providerName);

        private DataManager()
        {
            this.Commands = new List<IDataCommand>();
            this.Errors = new List<DataError>();
            this.TableMaps = new Dictionary<string, TableMap>();
        }

        //TODO Check if this works
        protected DataManager(DbConnection connection)
            : this()
        {
            this._connection = connection;
        }

        private void OpenSharedConnection()
        {

            try
            {
                if (this._connection == null)
                {
                    if (_factory == null)
                    {
                        this._factory = System.Data.Common.DbProviderFactories.GetFactory(ProviderName);
                    }
                    this._connection = this._factory.CreateConnection();
                }
                if (this._connection != null)
                {

                    switch (this._connection.State)
                    {
                        case ConnectionState.Closed:
                            this._connection.ConnectionString = this.ConnectionString;
                            this._connection.Open();
                            break;
                        case ConnectionState.Open:

                            break;
                        case ConnectionState.Connecting:
                            break;
                        case ConnectionState.Executing:
                            break;
                        case ConnectionState.Fetching:
                            break;
                        case ConnectionState.Broken:

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

            }
            catch (Exception e)
            {
                LoggingService.SendErrorToLog(e);
            }

        }

        //TODO Implement transaction
        public virtual void CreateTransaction()
        {

            this.OpenSharedConnection();

            if (this.Connection != null)
                if (this.Connection.State != ConnectionState.Open)
                    this.Connection.Open();

            if (this.Transaction == null)
            {
                this.Transaction = this._connection.BeginTransaction();

            }
        }



        public virtual TableMap<T> CreateTableMap<T>(string name) where T : IColumnMap
        {
            var existingMap = GetTableMap(name);

            if (existingMap == null)
            {
                existingMap = new TableMap<T>()
                {
                    TableName = name
                };

                TableMaps.Add(name, existingMap);
            }
            return (TableMap<T>)existingMap;
        }

        public TableMap GetTableMap(string name)
        {
            if (TableMaps.ContainsKey(name))
            {
                return TableMaps[name];
            }

            return null;
        }






        private T AddNew()
        {
            var command = (T)Activator.CreateInstance(typeof(T), this);
            this.Commands.Add(command);
            return command;
        }

        public string DatabaseName
        {
            get
            {

                if (string.IsNullOrEmpty(ConnectionString))
                {
                    return string.Empty;
                }

                return GetDatabaseName(ConnectionString);

            }
        }


        public IDataCommand GetCommand(int index)
        {

            if (Commands.Count > 0)
                if (this.Commands.Count >= index)
                    return (T)this.Commands[index];

            return null;
        }

        public void RemoveAt(int index)
        {
            if (this.Commands.Count() >= index)
            {
                this.Commands.RemoveAt(index);
            }

        }

        public void Remove(IDataCommand command)
        {
            this.Commands.Remove(command);
        }

        public void ClearCommands()
        {
            this.Commands.Clear();

        }

        public IEnumerable<IDataCommand> GetCommands()
        {
            return this.Commands;
        }

        public void RemoveCommand(IDataCommand command)
        {
            this.Commands.Remove(command);
        }


        private IEnumerable<ICommandResult> ExecuteTableCreateUpdate()
        {
            var commands = this.Commands.Where(m => m.CommandType == DataCommandType.TableCreateUpdate).ToArray();
            this.OnBeforeCommandsLoaded(this, new ModelCommandBeforeLoadedEventArgs(commands, DataCommandType.TableCreateUpdate));
            return this.ExecuteCommands(commands);
        }

        private IEnumerable<ICommandResult> InsertCommands()
        {
            var commands = this.Commands.Where(m => m.CommandType == DataCommandType.Insert).ToArray();
            this.OnBeforeCommandsLoaded(this, new ModelCommandBeforeLoadedEventArgs(commands, DataCommandType.Insert));
            return this.ExecuteCommands(commands);
        }
        private IEnumerable<ICommandResult> UpdateCommands()
        {
            var commands = this.Commands.Where(m => m.CommandType == DataCommandType.Update).ToArray();
            this.OnBeforeCommandsLoaded(this, new ModelCommandBeforeLoadedEventArgs(commands, DataCommandType.Update));
            return this.ExecuteCommands(commands); ;
        }
        private IEnumerable<ICommandResult> DeleteCommands()
        {
            var deletes = this.Commands.Where(m => m.CommandType == DataCommandType.Delete).ToArray();
            this.OnBeforeCommandsLoaded(this, new ModelCommandBeforeLoadedEventArgs(deletes, DataCommandType.Delete));
            var restult = this.ExecuteCommands(deletes);

            if (Transaction == null)
                for (int i = deletes.Count() - 1; i >= 0; i--)
                {
                    this.Commands.Remove(deletes[i]);
                }
            return restult;
        }
        private IEnumerable<ICommandResult> SelectCommands()
        {
            var commands = this.Commands.Where(m => m.CommandType == DataCommandType.Select).ToArray();
            this.OnBeforeCommandsLoaded(this, new ModelCommandBeforeLoadedEventArgs(commands, DataCommandType.Select));
            return this.ExecuteCommands(commands);
        }

        public virtual string GetDatabaseName(string connectionString)
        {
            return string.Empty;
        }

        public virtual ResultList Execute()
        {
            var results = new ResultList();

            results.AddRange(this.ExecuteTableCreateUpdate());
            results.AddRange(this.DeleteCommands());
            results.AddRange(this.UpdateCommands());
            results.AddRange(this.InsertCommands());
            results.AddRange(this.SelectCommands());
          


            return results;
        }

        protected abstract ICommandResult ExecuteDeleteCommand(T dataCommand, IDbCommand command);
        protected abstract ICommandResult ExecuteUpdateCommand(T dataCommand, IDbCommand command);
        protected abstract ICommandResult ExecuteInsertCommand(T dataCommand, IDbCommand command);
        protected abstract ICommandResult ExecuteSelectCommand(T dataCommand, IDbCommand command);
        protected abstract ICommandResult ExecuteTableCreateUpdate(T dataCommand, IDbCommand command);

        protected abstract Task<ICommandResult> ExecuteDeleteCommandAsync(T dataCommand, IDbCommand command);
        protected abstract Task<ICommandResult> ExecuteUpdateCommandAsync(T dataCommand, IDbCommand command);
        protected abstract Task<ICommandResult> ExecuteInsertCommandAsync(T dataCommand, IDbCommand command);
        protected abstract Task<ICommandResult> ExecuteSelectCommandAsync(T dataCommand, IDbCommand command);



        public ICommandResult ExecuteCommand(IDataCommand dataCommand)
        {
            ICommandResult result = null;
            var timer = new Stopwatch();
            timer.Start();

            try
            {
                try
                {
                    if (dataCommand.Validate() == false) return null;
                }
                catch (Exception exception)
                {
                    result.AddError(LogType.Error, "Validate exeption", exception);
                }
                if (Connection == null)
                    this.OpenSharedConnection();
                //try
                //{
               
                var command = this.Connection.CreateCommand();

                command.CommandTimeout = 0;
                command.Connection = this.Connection;
                if (this.Transaction != null)
                {
                    command.Transaction = this.Transaction;
                }
                switch (dataCommand.CommandType)
                {
                    case DataCommandType.Select:
                        result = ExecuteSelectCommand((T)dataCommand, command);
                        break;
                    case DataCommandType.Insert:
                        result = ExecuteInsertCommand((T)dataCommand, command);
                        break;
                    case DataCommandType.Update:
                        result = ExecuteUpdateCommand((T)dataCommand, command);
                        break;
                    case DataCommandType.Delete:
                        result = ExecuteDeleteCommand((T)dataCommand, command);
                        break;
                    case DataCommandType.TableCreateUpdate:
                        result = ExecuteTableCreateUpdate((T)dataCommand, command);
                        break;
                }
                result.DataCommand = dataCommand;

                //}
                //catch (SqlException exception)
                //{
                //    result.AddError(LogType.Error, "Sql exeption", exception);
                //}
                //catch (Exception ex)
                //{
                //    result.AddError(LogType.Error, "exeption", ex);
                //}
            }
            catch (Exception ex1)
            {
                this.Errors.Add(new DataError
                {
                    StackTrace = ex1.StackTrace,
                    Exception = ex1,
                    HasError = true
                });
            }
            finally
            {
                if (this.Transaction == null)
                    this.Connection.Close();

                timer.Stop();
                if(result == null)
                {
                    result = new CommandResult();

                }
                result.ElapsedMilliseconds = timer.ElapsedMilliseconds;
             

                result.AddMessage($"Executed {dataCommand.CommandType} in {timer.ElapsedMilliseconds}ms with {result.RecordsAffected} rows affected");

            }
            return result;
        }



        private IEnumerable<ICommandResult> ExecuteCommands(IDataCommand[] commandsList)
        {

            var results = new List<ICommandResult>();
            if (!commandsList.Any()) return results;
            try
            {
                this.OpenSharedConnection();

                this.CreateTransaction();
                foreach (var dataCommand in commandsList)
                {
                    var result = this.ExecuteCommand(dataCommand);
                    results.Add(result);
                }
                try
                {
                    this.Transaction.Commit();
                }
                catch (Exception exception)
                {
                    this.Errors.Add(item: new DataError { Exception = exception, HasError = true });
                    this.Transaction.Rollback();
                }
                finally
                {
                    if (Transaction != null)
                        this.Transaction.Dispose();
                }
            }
            catch (Exception ex1)
            {
                this.Errors.Add(new DataError { Exception = ex1, HasError = true });
                Console.WriteLine("Commit Exception Type: {0}", ex1.GetType());
                Console.WriteLine("  Message: {0}", ex1.Message);
            }
            finally
            {
                if (this.Connection != null)
                    this.Connection.Close();
            }
            return results;
        }


        public async Task<ICommandResult> ExecuteCommandAsync(IDataCommand dataCommand)
        {
            var result = new CommandResult();

            try
            {
                if (dataCommand.Validate() == false) return null;
                this.OpenSharedConnection();
                try
                {
                    using (var command = this.Connection.CreateCommand())
                    {
                        command.Connection = this.Connection;
                        if (this.Transaction != null) command.Transaction = this.Transaction;
                        switch (dataCommand.CommandType)
                        {
                            case DataCommandType.Select:
                                return await this.ExecuteSelectCommandAsync((T)dataCommand, command);
                            case DataCommandType.Insert:
                                return await ExecuteInsertCommandAsync((T)dataCommand, command);
                            case DataCommandType.Update:
                                return await ExecuteUpdateCommandAsync((T)dataCommand, command);
                            case DataCommandType.Delete:
                                return await ExecuteDeleteCommandAsync((T)dataCommand, command);
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.AddError(LogType.Error, "exeption", ex);
                }
            }
            catch (Exception ex1)
            {
                this.Errors.Add(new DataError
                {
                    StackTrace = ex1.StackTrace,
                    Exception = ex1,
                    HasError = true
                });
            }
            finally
            {
                if (this.Transaction == null)
                    this.Connection.Close();
            }

            return result;
        }


        public virtual T Select()
        {
            var dataCommand = AddNew();
            dataCommand.SelectAll = false;
            dataCommand.CommandType = DataCommandType.Select;

            return dataCommand;
        }
        public T Delete()
        {
            var dataCommand = AddNew();
            dataCommand.CommandType = DataCommandType.Delete;
            return dataCommand;
        }
        public T Insert()
        {
            var dataCommand = AddNew();
            dataCommand.CommandType = DataCommandType.Insert;
            return dataCommand;
        }
        public T Update()
        {
            var dataCommand = AddNew();
            dataCommand.CommandType = DataCommandType.Update;
            return dataCommand;
        }

        public T CreateUpdateTable()
        {
            var dataCommand = AddNew();
            dataCommand.CommandType = DataCommandType.TableCreateUpdate;
            return dataCommand;
        }


        public void Dispose()
        {
            if (this._connection == null)
                return;
            this._connection.Dispose();
        }
    }
}