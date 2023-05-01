using System;

namespace Toolkit.DAL.Mongo
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MigrationAttribute : Attribute
    {
        public string Name { get; }

        public MigrationAttribute(string name)
        {
            Name = name;
        }
    }
}
