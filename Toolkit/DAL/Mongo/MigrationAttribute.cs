using System;

namespace Toolkit.DAL.Mongo
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MigrationAttribute(string name) : Attribute
    {
        public string Name { get; } = name;
    }
}
