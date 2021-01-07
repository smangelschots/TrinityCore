using System;

namespace Trinity.DataAccess.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoJoinAttribute : Attribute
    {
        public AutoJoinAttribute() { }
    }
}