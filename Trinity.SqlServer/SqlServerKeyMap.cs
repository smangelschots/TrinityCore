using System;
using Trinity.DataAccess.Orm;

namespace Trinity.SqlServer
{
    public class SqlServerKeyMap : KeyMap
    {
        public KeyMapType GetSqlKeyType(string keyType)
        {
            if (keyType == "FK")
            {
                return KeyMapType.ForeignKey;
            }
            return KeyMapType.PrimaryKey;
        }
    }
}
