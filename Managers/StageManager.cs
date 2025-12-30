using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Monolith.IO;
using Monolith.Graphics;
using Monolith.Nodes;
using Monolith.Geometry;
using Monolith.Util;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monolith.Managers
{
    public class StageManager
    {
        public readonly Stack<IStage> Stages = new();
        private bool _stageFrozen;
        private bool _pendingFreeze;

        private static readonly Dictionary<string, Type> _stageTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IStage).IsAssignableFrom(t) && !t.IsAbstract)
            .ToDictionary(t => t.Name);

        public bool StageFrozen => _stageFrozen;

        public StageManager() { }

        /// <summary>
        /// Takes a stage, adds it to the stack, initializes and loads it
        /// </summary>
        public void AddStage(IStage stage)
        {
            if (stage == null)
                throw new ArgumentNullException(nameof(stage), "Cannot add a null stage.");

            StageIntervention();

            Stages.Push(stage);

            stage.Initialize();
            stage.Load();
        }

        /// <summary>
        /// Acts in between stage actions where stages are removed, added, etc
        /// </summary>
        private void StageIntervention()
        {
            ClearStageData();
            _stageFrozen = false;
        }

        /// <summary>
        /// Creates a new stage instance from a type
        /// </summary>
        public IStage GetStageFromType<T>() where T : IStage, new()
        {
            return new T();
        }

        /// <summary>
        /// Creates a new stage instance from a string using reflection
        /// </summary>
        public IStage GetStageFromString(string stageName)
        {
            return _stageTypes.TryGetValue(stageName, out var type)
                ? (IStage)Activator.CreateInstance(type)
                : null;
        }

        /// <summary>
        /// Adds a stage from a string
        /// </summary>
        public void AddStageFromString(string stageName)
        {
            IStage targetStage = GetStageFromString(stageName);
            if (targetStage == null)
                throw new InvalidOperationException($"Stage '{stageName}' not found.");

            AddStage(targetStage);
        }

        /// <summary>
        /// Adds a stage from a type
        /// </summary>
        public void AddStageFromType<T>() where T : IStage, new()
        {
            IStage targetStage = GetStageFromType<T>();
            AddStage(targetStage);
        }

        /// <summary>
        /// Removes the current stage
        /// </summary>
        public void RemoveCurrentStage()
        {
            if (Stages.Count == 0) return;

            var current = Stages.Pop();
            current?.Unload();

            StageIntervention();
        }

        /// <summary>
        /// Gets the current stage
        /// </summary>
        public IStage GetCurrentStage()
        {
            return Stages.Count > 0 ? Stages.Peek() : null;
        }

        /// <summary>
        /// Freezes the current stage at the end of the frame
        /// </summary>
        public void QueueFreezeCurrentStage()
        {
            _pendingFreeze = true;
        }

        /// <summary>
        /// Freezes the current stage immediately
        /// </summary>
        public void FreezeCurrentStage()
        {
            _stageFrozen = true;
        }

        /// <summary>
        /// Unfreezes the current stage
        /// </summary>
        public void UnfreezeCurrentStage()
        {
            _stageFrozen = false;
            _pendingFreeze = false;
        }

        /// <summary>
        /// Applies pending freeze at the end of the update cycle
        /// </summary>
        private void ApplyPendingFreeze()
        {
            if (_pendingFreeze)
            {
                _stageFrozen = true;
                _pendingFreeze = false;
            }
        }

        /// <summary>
        /// Queues freezing the current stage for a duration
        /// </summary>
        public void QueueFreezeCurrentStageFor(float duration)
        {
            QueueFreezeCurrentStage();
            Engine.Timer.Wait(duration, UnfreezeCurrentStage);
        }

        /// <summary>
        /// Updates the current stage
        /// </summary>
        public void UpdateCurrentStage(GameTime gameTime)
        {
            if (IsStackEmpty())
                return;

            if (!_stageFrozen)
                GetCurrentStage()?.Update(gameTime);
            
            ApplyPendingFreeze();
        }

        /// <summary>
        /// Draws the current stage
        /// </summary>
        public void DrawCurrentStage(SpriteBatch spriteBatch)
        {
            if (IsStackEmpty())
                return;
            
            GetCurrentStage()?.Draw(spriteBatch);
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
            oldStage.Unload();

            var newStage = (IStage)Activator.CreateInstance(oldStage.GetType());
            AddStage(newStage);
        }

        /// <summary>
        /// Clears data related to stages
        /// </summary>
        public static void ClearStageData()
        {
            Engine.Node.DumpAllInstances();
        }
    }
}
