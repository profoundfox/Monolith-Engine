using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Microsoft.Xna.Framework;
using Monolith.Geometry;
using Monolith.Nodes;

public static class NodeFactory
{
    /// <summary>
    /// Creates a Node dynamically by type name, sets the IRegionShape2D in the config,
    /// and applies any extra config properties (node-specific).
    /// </summary>
    public static Node2D CreateNode(
        SpatialNodeConfig nodeConfig,
        IRegionShape2D shape,
        System.Collections.Generic.Dictionary<string, object> extraConfigProperties = null)
    {
        Type nodeType = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.FullName == nodeConfig.Name);

        if (nodeType == null)
            throw new Exception($"Node type '{nodeConfig.Name}' not found.");

        ConstructorInfo constructor = nodeType.GetConstructors().FirstOrDefault();
        if (constructor == null)
            throw new Exception($"Node type '{nodeConfig.Name}' has no public constructors.");

        Type configType = constructor.GetParameters()[0].ParameterType;

        object configInstance = Activator.CreateInstance(configType);

        PropertyInfo regionProp = configType.GetProperties()
            .FirstOrDefault(p => typeof(IRegionShape2D).IsAssignableFrom(p.PropertyType) && p.CanWrite);

        if (regionProp != null)
            regionProp.SetValue(configInstance, shape);

        PropertyInfo nameProp = configType.GetProperty("Name");
        if (nameProp != null && nameProp.CanWrite)
            nameProp.SetValue(configInstance, nodeType.Name);
        
        
        
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
