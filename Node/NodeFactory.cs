using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Monolith.Geometry;
using Monolith.Nodes;

public static class NodeFactory
{
    private static readonly Dictionary<string, Type> TypeCache = new();
 
    /// <summary>
    /// Resolves a node type by simple name or full name.
    /// Results are cached and ambiguity is detected.
    /// </summary>
    /// <param name="name">The node type name.</param>
    /// <returns>The resolved Type.</returns>
    private static Type ResolveNodeType(string name)
    {
        if (TypeCache.TryGetValue(name, out var cached))
        {
            return cached;
        }

        var allTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    return e.Types.Where(t => t != null);
                }
            });

        var matches = allTypes
            .Where(t => t.Name == name || t.FullName == name)
            .ToList();

        if (matches.Count == 0)
        {
            throw new Exception($"Node type '{name}' not found.");
        }

        if (matches.Count > 1)
        {
            throw new Exception(
                $"Ambiguous node type '{name}'. Found:\n" +
                string.Join("\n", matches.Select(t => t.FullName))
            );
        }

        TypeCache[name] = matches[0];
        return matches[0];
    }

    /// <summary>
    /// Creates a Node2D instance using a SpatialNodeConfig-based constructor.
    /// Automatically injects shape, position, name, and extra config properties.
    /// If a parent is specified in the config, the node is attached to it.
    /// </summary>
    /// <param name="name">The node type name.</param>
    /// <param name="shape">The collision or region shape to associate with the node.</param>
    /// <param name="extraConfigProperties">Optional additional config properties.</param>
    /// <returns>The created Node2D.</returns>
    public static Node2D CreateNode(
        string name,
        IRegionShape2D shape,
        Dictionary<string, object> extraConfigProperties = null)
    {
        Type nodeType = ResolveNodeType(name);

        ConstructorInfo constructor = nodeType
            .GetConstructors()
            .FirstOrDefault(c =>
            {
                var p = c.GetParameters();
                return p.Length == 1 &&
                    typeof(SpatialNodeConfig).IsAssignableFrom(p[0].ParameterType);
            });

        if (constructor == null)
            throw new Exception($"Node type '{name}' has no valid SpatialNodeConfig constructor.");

        Type configType = constructor.GetParameters()[0].ParameterType;
        object configInstance = Activator.CreateInstance(configType);

        PropertyInfo shapeProp = configType.GetProperties()
            .FirstOrDefault(p =>
                typeof(IRegionShape2D).IsAssignableFrom(p.PropertyType) && p.CanWrite);

        if (shapeProp != null)
            shapeProp.SetValue(configInstance, shape);

        PropertyInfo nameProp = configType.GetProperty("Name");
        if (nameProp != null && nameProp.CanWrite)
            nameProp.SetValue(configInstance, nodeType.FullName);

        PropertyInfo posProp = configType.GetProperty("Position");
        if (posProp != null && posProp.CanWrite)
            posProp.SetValue(configInstance, shape.Location.ToVector2());

        if (extraConfigProperties != null)
        {
            foreach (var kvp in extraConfigProperties)
            {
                PropertyInfo prop = configType.GetProperty(kvp.Key);
                if (prop != null && prop.CanWrite)
                    prop.SetValue(configInstance, kvp.Value);
            }
        }

        var childNodes = new List<Node2D>();

        PropertyInfo[] nodeProps = configType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p =>
                typeof(Node2D).IsAssignableFrom(p.PropertyType) &&
                p.CanWrite)
            .ToArray();

        foreach (var prop in nodeProps)
        {
            Node2D child = CreateNode(prop.PropertyType.FullName, shape, null);
            prop.SetValue(configInstance, child);
            childNodes.Add(child);
        }

        Node2D node = constructor.Invoke(new[] { configInstance }) as Node2D;

        if (node == null)
            throw new Exception($"Failed to create node '{name}'.");

        foreach (var child in childNodes)
        {
            node.AddChild(child);
        }

        PropertyInfo parentProp = configType.GetProperty("Parent");
        if (parentProp != null)
        {
            Node2D parent = parentProp.GetValue(configInstance) as Node2D;
            if (parent != null)
                parent.AddChild(node);
        }

        return node;
    }

}