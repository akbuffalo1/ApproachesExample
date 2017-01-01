using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TigerApp
{
    public static class TypeEx
    {
        public static Map GetTypeInheritance(Type type)
        {
            //get all the interfaces for this type
            var interfaces = type.GetInterfaces();

            //get all the interfaces for the ancestor interfaces
            var baseInterfaces = interfaces.SelectMany(i => i.GetInterfaces());

            //filter based on only the direct interfaces
            var directInterfaces = interfaces.Where(i => baseInterfaces.All(b => b != i));

            return new Map
            {
                Node = type,
                Ancestors = directInterfaces.Select(GetTypeInheritance).ToList()
            };
        }

        public class Map
        {
            public Type Node { get; set; }
            public List<Map> Ancestors { get; set; }
        }

        public static IEnumerable<Map> Traverse(this Map root)
        {
            var stack = new Stack<Map>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                yield return current;
                foreach (var child in current.Ancestors)
                    stack.Push(child);
            }
        }

        public static IEnumerable<TypeInfo> GetInterfaceHierarchy(this Type type)
        {
            var root = GetTypeInheritance(type);
            return root.Traverse().Where(n => n.Node.IsInterface).Select(n => n.Node.GetTypeInfo()).Reverse();
        }

        public static IEnumerable<MethodInfo> GetInterfaceMethods(this Type type)
        {
            var root = GetTypeInheritance(type);
            return root.Traverse().SelectMany(n => n.Node.GetMethods());
        }
    }
}