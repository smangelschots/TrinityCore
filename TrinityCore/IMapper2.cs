using System;

namespace TrinityCore
{
    public interface IMapper2 : IMapper
    {
        Func<object, object> GetFromDbConverter(Type DestType, Type SourceType);
    }
}