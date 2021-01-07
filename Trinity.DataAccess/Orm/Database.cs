using Trinity.DataAccess.Interfaces;

namespace Trinity.DataAccess.Orm
{
    public class Database
    {
        public static IMapper Mapper
        {
            get;
            set;
        }
    }
}