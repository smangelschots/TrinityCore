using System;
using Trinity.DataAccess.Attributes;

namespace Trinity.DataAccess.LobModels
{

    [TableConfiguration("Z_Object")]
    public class ObjectModel
    {
        public Guid Id { get; set; }

        public Guid ApplicationId { get; set; }

        public string Name { get; set; }

        public string ObjectType { get; set; }

        public string Document { get; set; }

    }
}
