using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Monolith.IO;
using Monolith.Graphics;
using Monolith.Hierarchy;
using Monolith.Geometry;
using Monolith.Util;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monolith.Tools;

namespace Monolith.Managers
{
  public partial class StageManager
  {
    public readonly Stack<Stage> Stages = new();

    private static readonly Dictionary<string, Type> _stageTypes = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(a => a.GetTypes())
        .Where(t => typeof(Stage).IsAssignableFrom(t) && !t.IsAbstract)
        .ToDictionary(t => t.Name);

    public StageManager() { }

    /// <summary>
    /// Takes a stage, adds it to the stack, initializes and loads it
    /// </summary>
    public void AddStage(Stage stage)
    {
      if (stage == null)
        throw new ArgumentNullException(nameof(stage), "Cannot add a null stage.");

      StageIntervention();

      Stages.Push(stage);

      stage.OnEnter();
    }

    /// <summary>
    /// Acts in between stage actions where stages are removed, added, etc
    /// </summary>
    private void StageIntervention()
    {
      ClearStageData();
    }

    /// <summary>
    /// Creates a new stage instance from a type
    /// </summary>
    public Stage GetStageFromType<T>() where T : Stage, new()
    {
      return new T();
    }

    /// <summary>
    /// Creates a new stage instance from a string using reflection
    /// </summary>
    public Stage GetStageFromString(string stageName)
    {
      return _stageTypes.TryGetValue(stageName, out var type)
          ? (Stage)Activator.CreateInstance(type)
          : null;
    }

    /// <summary>
    /// Adds a stage from a string
    /// </summary>
    public void AddStageFromString(string stageName)
    {
      Stage targetStage = GetStageFromString(stageName);
      if (targetStage == null)
        throw new InvalidOperationException($"Stage '{stageName}' not found.");

      AddStage(targetStage);
    }

    /// <summary>
    /// Adds a stage from a type
    /// </summary>
    public void AddStageFromType<T>() where T : Stage, new()
    {
      Stage targetStage = GetStageFromType<T>();
      AddStage(targetStage);
    }

    /// <summary>
    /// Removes the current stage
    /// </summary>
    public void RemoveCurrentStage()
    {
      if (Stages.Count == 0) return;

      var current = Stages.Pop();
      current?.OnExit();

      StageIntervention();
    }

    /// <summary>
    /// Gets the current stage
    /// </summary>
    public Stage GetCurrentStage()
    {
      return Stages.Count > 0 ? Stages.Peek() : null;
    }


    public void ProcessUpdate(float deltaTime)
    {
      if (IsStackEmpty())
        return;

      GetCurrentStage()?.ProcessUpdate(deltaTime);
    }

    /// <summary>
    /// Updates the current stage with a fixed framerate.
    /// </summary>
    public void PhysicsUpdate(float deltaTime)
    {
      if (IsStackEmpty())
        return;

      GetCurrentStage()?.PhysicsUpdate(deltaTime);
    }

    public void SubmitCallCurrentStage()
    {
      if (IsStackEmpty())
        return;

      GetCurrentStage()?.SubmitCall();
    }

    /// <summary>
    /// Checks if the stack is empty
    /// </summary>
    public bool IsStackEmpty()
    {
      return Stages.Count == 0;
    }

    /// <summary>
    /// Reloads the current stage as a fresh instance
    /// </summary>
    public void ReloadCurrentStage()
    {
      if (Stages.Count == 0) return;

      var oldStage = Stages.Pop();
      oldStage.OnExit();

      var newStage = (Stage)Activator.CreateInstance(oldStage.GetType());
      AddStage(newStage);
    }

    /// <summary>
    /// Clears data related to stages
    /// </summary>
    public static void ClearStageData()
    {
      Engine.Tree.DumpAllInstances();
    }
  }
}
