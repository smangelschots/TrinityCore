using System.Collections.Generic;
using Trinity.DataAccess.Orm;
using Trinity.DataAccess.Results;

namespace Trinity.DataAccess.Interfaces
{
    public interface IModelCommand<T>
        where T : class
    {

        bool TableMapFromDatabase { get; set; }

        IDataCommand<T> Track(T model);

        IDataCommand<T> Insert(T model);

        IDataCommand<T> Update(T model);



        ModelConfiguration<T> ModelConfiguration { get; set; }
        void RemoveAt(int index);
        void Remove(IDataCommand command);
        void ClearCommands();

        IEnumerable<IDataCommand> GetCommands();

        IDataCommand GetCommand(int index);


        ResultList Execute();


    }



}
