using Trinity.DataAccess.Interfaces;

namespace Trinity.DataAccess.Collections
{
    public class DataBindingList<T> : DataCommandCollection<T>
        where T : class
    {
        public DataBindingList(IModelCommand<T> dataManager) : base(dataManager)
        {
        }
    }
}
