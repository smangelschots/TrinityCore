using System;
using Trinity.DataAccess.Interfaces;

namespace Trinity.DataAccess.Orm
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PrimaryKeyAttribute : Attribute, IPrimaryKeyAttribute
    {
        public PrimaryKeyAttribute(string primaryKey)
        {
            this.Name = primaryKey;
            this.IsAutoIncrement = true;
        }

        public string Name { get; private set; }
        public string SequenceName { get; set; }

        public bool IsAutoIncrement { get; set; }
    }
}