using System;
using System.Collections.Generic;
using System.Linq;
using Monolith.Tools;
using Monolith.Hierarchy;
using Monolith.Util;

namespace Monolith.Managers
{
  public class TreeServer2D
  {
    private readonly List<Instance> instances = new();
    private readonly Dictionary<string, List<Instance>> byName = new();


    private readonly List<Instance> pendingAdd = new();
    private readonly List<Instance> pendingRemove = new();

    private readonly Queue<Action> continuations = new();

    /// <summary>
    /// Creates a new node and adds it to the tree.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Create<T>()
        where T : Instance, new()
    {
      var inst = new T();

      Flush();
      inst.OnEnter();

      return inst;
    }

    public Tween<T> CreateTween<T>
    (
        Action<T> setter,
        T start,
        T end,
        float duration,
        Func<T, T, float, T> lerpFunc,
        Func<float, float> easingFunction = null)
    {
      if (easingFunction == null)
        easingFunction = EasingFunctions.Linear;

      var tween = new Tween<T>(
          start,
          end,
          duration,
          lerpFunc,
          setter,
          easingFunction
      );

      return tween;
    }


    /// <summary>
    /// Queues an instance to be added to this tree.
    /// </summary>
    /// <param name="instance"></param>
    internal void QueueAdd(Instance instance) => pendingAdd.Add(instance);
    /// <summary>
    /// Queues an intance to be removed from this tree.
    /// </summary>
    /// <param name="instance"></param>
    internal void QueueRemove(Instance instance) => pendingRemove.Add(instance);

    /// <summary>
    /// Flushes all instances.
    /// </summary>
    private void Flush()
    {
      if (pendingAdd.Count == 0 && pendingRemove.Count == 0)
        return;

      foreach (var n in pendingAdd)
        AddInternal(n);

      pendingAdd.Clear();

      foreach (var n in pendingRemove.ToList())
        RemoveInternal(n);

      pendingRemove.Clear();
    }

    /// <summary>
    /// Adds an instance.
    /// </summary>
    /// <param name="instance"></param>
    private void AddInternal(Instance instance)
    {
      instances.Add(instance);

      if (!string.IsNullOrEmpty(instance.GetName()))
      {
        if (!byName.TryGetValue(instance.GetName(), out var list))
        {
          list = new List<Instance>();
          byName[instance.GetName()] = list;
        }
        list.Add(instance);
      }
    }

    /// <summary>
    /// Removes an instance.
    /// </summary>
    /// <param name="instance"></param>
    private void RemoveInternal(Instance instance)
    {
      instance.OnExit();

      instances.Remove(instance);

      if (!string.IsNullOrEmpty(instance.GetName())
          && byName.TryGetValue(instance.GetName(), out var list))
      {
        list.Remove(instance);
        if (list.Count == 0)
          byName.Remove(instance.GetName());
      }

      instance.OnExit();

      instance.ClearData();
    }

    /// <summary>
    /// Removes an instance without queueing.
    /// </summary>
    /// <param name="instance"></param>
    internal void RemoveNow(Instance instance) => RemoveInternal(instance);

    /// <summary>
    /// Frees and removes all nodes immediately.
    /// </summary>
    public void DumpAllInstances()
    {
      foreach (var instance in instances.ToList())
      {
        RemoveNow(instance);
      }

      instances.Clear();
      byName.Clear();
    }


    internal void Update(TimeContext context, int steps)
    {
      for (int i = 0; i < steps; i++)
      {
        Engine.Stage.PhysicsUpdate((float)context.FixedDelta.TotalSeconds);
      }

      Engine.Stage.ProcessUpdate((float)context.FrameDelta.TotalSeconds);
      Engine.Stage.SubmitCallCurrentStage();

      while (continuations.Count > 0)
      {
        continuations.Dequeue()?.Invoke();
      }
    }

    /// <summary>
    /// Updates the instances with a fixed framerate.
    /// </summary>
    /// <param name="delta"></param>
    internal void ProcesssUpdate(float delta)
    {
      Flush();
      foreach (var i in instances.ToList())
      {
        i.ProcessUpdate(delta);
        Flush();
      }
    }

    /// <summary>
    /// Updates the instance with a dynamic framerate.
    /// </summary>
    /// <param name="delta"></param>
    internal void PhysicsUpdate(float delta)
    {
      Flush();
      foreach (var i in instances.ToList())
      {
        i.PhysicsUpdate(delta);
        Flush();
      }
    }

    /// <summary>
    /// Submits calls to the Canvas manager.
    /// </summary>
    public void SubmitCalls()
    {
      foreach (var i in instances)
        i.SubmitCall();
    }

    internal void Post(Action action)
    {
      if (action == null) return;
      continuations.Enqueue(action);
    }

    /// <summary>
    /// Gets the first instance by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Instance Get(string name)
        => GetAll(name).FirstOrDefault();

    /// <summary>
    /// Gets all instances by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IReadOnlyList<Instance> GetAll(string name)
        => string.IsNullOrEmpty(name)
            ? Array.Empty<Instance>()
            : byName.TryGetValue(name, out var list)
                ? list
                : Array.Empty<Instance>();

    /// <summary>
    /// Gets the first instance by type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Get<T>() where T : Instance
        => GetAll<T>().FirstOrDefault();

    /// <summary>
    /// Gets all instances by type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IReadOnlyList<T> GetAll<T>() where T : Instance
        => instances.OfType<T>().ToList();

    /// <summary>
    /// Gets all instacnes
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<Instance> GetAll()
    {
      return instances.AsReadOnly();
    }

  }
}
