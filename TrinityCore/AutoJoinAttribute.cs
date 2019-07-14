using System;

namespace TrinityCore
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoJoinAttribute : Attribute
    {
        public AutoJoinAttribute() { }
    }
}