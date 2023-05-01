using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Toolkit.Extention
{
    public static class AssemblyExtensions
    {
        public static IReadOnlyCollection<Type> GetAssignableTypes<TParent>(this Assembly entryPoint)
        {
            var prefix = typeof(AssemblyExtensions).Namespace?.Split('.')[0] ?? string.Empty;

            return entryPoint
                .GetReferencedAssemblies()
                .Select(Assembly.Load)
                .SelectMany(x => x.GetReferencedAssemblies().Append(x.GetName()))
                .DistinctByParam(x => x.FullName)
                .Select(Assembly.Load)
                .Concat(new[] { entryPoint })
                .Where(a => a.FullName?.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ?? false)
                .SelectMany(x => x.GetTypes())
                .Where(type => type.IsClass && !type.IsInterface && typeof(TParent).IsAssignableFrom(type))
                .Distinct()
                .ToArray();
        }

        public static IReadOnlyCollection<Assembly> GetAllReferencedAssemblies(this Assembly entryPoint)
        {
            var prefix = typeof(AssemblyExtensions).Namespace?.Split('.')[0] ?? string.Empty;

            return entryPoint
                .GetReferencedAssemblies()
                .Select(Assembly.Load)
                .SelectMany(x => x.GetReferencedAssemblies().Append(x.GetName()))
                .DistinctByParam(x => x.FullName)
                .Select(Assembly.Load)
                .Concat(new[] { entryPoint })
                .Where(a => a.FullName?.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ?? false)
                .Distinct()
                .ToArray();
        }
    }
}