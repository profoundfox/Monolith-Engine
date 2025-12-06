using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Monolith.Geometry;
using Monolith.Nodes;

public static class NodeFactory
{
    private static readonly Dictionary<string, Type> TypeCache = new();

    /// <summary>
    /// Resolves a type by simple name or full name, with caching and conflict detection.
    /// </summary>
    private static Type ResolveNodeType(string name)
    {
        if (TypeCache.TryGetValue(name, out var cached))
            return cached;

        var allTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
            });

        var matches = allTypes
            .Where(t => t.Name == name || t.FullName == name)
            .ToList();

        if (matches.Count == 0)
            throw new Exception($"Node type '{name}' not found.");

        if (matches.Count > 1)
            throw new Exception(
                $"Ambiguous node type '{name}'. Found:\n" +
                string.Join("\n", matches.Select(t => t.FullName))
            );

        TypeCache[name] = matches[0];
        return matches[0];
    }

    /// <summary>
    /// Creates a Node dynamically by type name, sets the IRegionShape2D in the config,
    /// and applies any extra config properties (node-specific).
    /// </summary>
    public static Node2D CreateNode(
        string name,
        IRegionShape2D shape,
        Dictionary<string, object> extraConfigProperties = null)
    {
        Type nodeType = ResolveNodeType(name);

        ConstructorInfo constructor = nodeType.GetConstructors().FirstOrDefault();

        if (constructor == null)
            throw new Exception($"Node type '{name}' has no public constructors.");

        Type configType = constructor.GetParameters()[0].ParameterType;

        object configInstance = Activator.CreateInstance(configType);

        PropertyInfo regionProp = configType.GetProperties()
            .FirstOrDefault(p => typeof(IRegionShape2D).IsAssignableFrom(p.PropertyType) && p.CanWrite);

        if (regionProp != null)
            regionProp.SetValue(configInstance, shape);

        PropertyInfo nameProp = configType.GetProperty("Name");
        if (nameProp != null && nameProp.CanWrite)
            nameProp.SetValue(configInstance, nodeType.FullName);

        PropertyInfo posProp = configType.GetProperty("Position");
        if (posProp != null && posProp.CanWrite)
            posProp.SetValue(configInstance, shape.Location.ToVector2());

        PropertyInfo[] nodeProps = configType
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Where(p => typeof(Node2D).IsAssignableFrom(p.PropertyType))
            .ToArray();

        foreach (var n in nodeProps)
        {
            if (!n.CanWrite)
                continue;

            Type nodePropType = n.PropertyType;

            Node2D childNode = CreateNode(nodePropType.FullName, shape);

            n.SetValue(configInstance, childNode);
        }

        if (extraConfigProperties != null)
        {
            foreach (var kvp in extraConfigProperties)
            {
                PropertyInfo prop = configType.GetProperty(kvp.Key); 
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(configInstance, kvp.Value);
                }
                else
                {
                    Console.WriteLine($"Warning: Property '{kvp.Key}' does not exist on config type '{configType.Name}'");
                }
            }
        }

        Node2D node = constructor.Invoke(new object[] { configInstance }) as Node2D;

        Console.WriteLine($"Created node: {node}, Node information: {node.Config}");

        return node;
    }
}
