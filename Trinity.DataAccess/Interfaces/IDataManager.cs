using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Trinity.DataAccess.Events;
using Trinity.DataAccess.Orm;
using Trinity.DataAccess.Results;

namespace Trinity.DataAccess.Interfaces
{
    public interface IDataManager<T>  : IDataManager where T   : IDataCommand
    {

        T Insert();

        T Update();

    }

    public interface IDataManager
    {
        IDbConnection Connection { get; }


        bool TableMapFromDatabase { get; set; }

        Dictionary<string, TableMap> TableMaps { get; set; }

        string ConnectionString { get; set; }
        string DatabaseName { get; }

        IDataCommand GetCommand(int index);
        void RemoveAt(int index);

        void Remove(IDataCommand command);

        void ClearCommands();
        ResultList Execute();

        void RemoveCommand(IDataCommand command);
        ICommandResult ExecuteCommand(IDataCommand dataCommand);

        Task<ICommandResult> ExecuteCommandAsync(IDataCommand dataCommand);


        void OnBeforeCommandExecute(object sender, ModelCommandBeforeExecuteEventArgs e);

        void OnAfterTryGetTableMap(object sender, ModelCommandAfterGetTableMapEventArgs e);

        void OnValidating(object sender, ModelCommandPropertyChangedEventArgs e);
    }
}