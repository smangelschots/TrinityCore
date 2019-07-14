using System.Data;

namespace TrinityCore
{
    public interface IObjectDataManager
    {

        void SetData(IDataReader reader);
        object GetData(string field);
        void SetData(string field, object value, bool fireEvents);
    }
}
