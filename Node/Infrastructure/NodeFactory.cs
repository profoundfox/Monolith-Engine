using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Monolith.Geometry;
using Monolith.Nodes;
using Monolith.Managers;
using Monolith;

public static class NodeFactory
{
    private static readonly Dictionary<string, Type> TypeCache = new();

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
            throw new Exception($"Ambiguous node type '{name}':\n{string.Join("\n", matches.Select(t => t.FullName))}");

        TypeCache[name] = matches[0];
        return matches[0];
    }

    /// <summary>
    /// Creates a Node2D using Engine.Node, but also sets up children, parent, and extra config properties.
    /// </summary>
    public static Node2D CreateNode(
        string name,
        IRegionShape2D shape,
        Dictionary<string, object> extraConfigProperties = null)
    {
        Type nodeType = ResolveNodeType(name);

        // Find constructor that takes SpatialNodeConfig
        ConstructorInfo constructor = nodeType
            .GetConstructors()
            .FirstOrDefault(c =>
            {
                var p = c.GetParameters();
                return p.Length == 1 && typeof(SpatialNodeConfig).IsAssignableFrom(p[0].ParameterType);
            });

        if (constructor == null)
            throw new Exception($"Node type '{name}' has no SpatialNodeConfig constructor.");

        Type configType = constructor.GetParameters()[0].ParameterType;
        var configInstance = Activator.CreateInstance(configType);

        // Set basic properties
        var shapeProp = configType.GetProperties().FirstOrDefault(p => typeof(IRegionShape2D).IsAssignableFrom(p.PropertyType) && p.CanWrite);
        shapeProp?.SetValue(configInstance, shape);

        var nameProp = configType.GetProperty("Name");
        if (nameProp != null && nameProp.CanWrite)
            nameProp.SetValue(configInstance, nodeType.FullName);

        var posProp = configType.GetProperty("LocalPosition");
        if (posProp != null && posProp.CanWrite)
            posProp.SetValue(configInstance, shape.Location.ToVector2());

        if (extraConfigProperties != null)
        {
            foreach (var kvp in extraConfigProperties)
            {
                var prop = configType.GetProperty(kvp.Key);
                if (prop != null && prop.CanWrite)
                    prop.SetValue(configInstance, kvp.Value);
            }
        }

        // Automatically create child nodes for Node2D properties in config
        var childNodes = new List<Node2D>();
        var nodeProps = configType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => typeof(Node2D).IsAssignableFrom(p.PropertyType) && p.CanWrite)
            .ToArray();

        foreach (var prop in nodeProps)
        {
            var child = CreateNode(prop.PropertyType.FullName, shape, null);
            prop.SetValue(configInstance, child);
            childNodes.Add(child);
        }

        // Use Engine.Node to create the actual node
        var createMethod = typeof(NodeManager).GetMethod(nameof(NodeManager.Create))
            .MakeGenericMethod(nodeType);
        var node = createMethod.Invoke(Engine.Node, new[] { configInstance }) as Node2D;

        if (node == null)
            throw new Exception($"Failed to create node '{name}' via NodeManager.");

        // Attach children
        foreach (var child in childNodes)
            node.AddChild(child);

        // Attach parent if set in config
        var parentProp = configType.GetProperty("Parent");
        if (parentProp != null)
        {
            var parent = parentProp.GetValue(configInstance) as Node2D;
            parent?.AddChild(node);
        }

        return node;
    }
}
