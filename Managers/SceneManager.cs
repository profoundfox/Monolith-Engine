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
    public class SceneManager
    {
        public readonly Stack<IScene> Scenes = new();
        private bool _sceneFrozen;
        private bool _pendingFreeze;

        private static readonly Dictionary<string, Type> _sceneTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IScene).IsAssignableFrom(t) && !t.IsAbstract)
            .ToDictionary(t => t.Name);

        
        public bool SceneFrozen => _sceneFrozen;

        public SceneManager() { }

        /// <summary>
        /// A function that takes an IScene adds it to the stack, loads it and initializes it
        /// </summary>
        /// <param name="scene"></param>
        public void AddScene(IScene scene)
        {
            if (scene == null)
                throw new ArgumentNullException(nameof(scene), "Cannot add a null scene.");

            SceneIntervention();

            //! Must be called before load to ensure new map is loaded.
            Scenes.Push(scene);
            
            scene.Initialize();
            scene.Load();
        }



        /// <summary>
        /// Acts in between scene actions where scenes are removed, added, etc
        /// </summary>
        private void SceneIntervention()
        {
            ClearSceneData();
            _sceneFrozen = false;
        }

        /// <summary>
        /// Takes the type and uses assembly to create a new scene instance from it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IScene GetSceneFromType<T>() where T : IScene, new()
        {
            return new T();
        }

        /// <summary>
        /// Takes the string and uses assembly to create a new scene instance from it.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public IScene GetSceneFromString(string sceneName)
        {
            return _sceneTypes.TryGetValue(sceneName, out var type)
                ? (IScene)Activator.CreateInstance(type)
                : null;
        }


        /// <summary>
        /// Adds scene from string
        /// </summary>
        /// <param name="sceneName"></param>
        public void AddSceneFromString(string sceneName)
        {
            IScene targetScene = GetSceneFromString(sceneName);
            if (targetScene == null)
                throw new InvalidOperationException($"Scene '{sceneName}' not found.");

            AddScene(targetScene);
        }

        /// <summary>
        /// Adds a scene from a type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddSceneFromType<T>() where T : IScene, new()
        {
            IScene targetScene = GetSceneFromType<T>();
            AddScene(targetScene);
        }

        /// <summary>
        /// Removes the current scene
        /// </summary>
        public void RemoveCurrentScene()
        {
            if (Scenes.Count == 0) return;

            var current = Scenes.Pop();
            current?.Unload();

            SceneIntervention();
        }


        /// <summary>
        /// Gets the current scene
        /// </summary>
        /// <returns></returns>
        public IScene GetCurrentScene()
        {
            return Scenes.Count > 0 ? Scenes.Peek() : null;
        }

        /// <summary>
        /// Freezes the current scene at the end of the frame
        /// </summary>
        public void QueueFreezeCurrentScene()
        {
            _pendingFreeze = true;
        }

        /// <summary>
        /// Freezes the current scene immediately
        /// </summary>
        public void FreezeCurrentScene()
        {
            _sceneFrozen = true;
        }

        /// <summary>
        /// Unfreezes the current scene
        /// </summary>
        public void UnfreezeCurrentScene()
        {
            _sceneFrozen = false;
            _pendingFreeze = false;
        }

        /// <summary>
        /// Function that waits until the end of update cycle to freeze
        /// </summary>
        private void ApplyPendingFreeze()
        {
            if (_pendingFreeze)
            {
                _sceneFrozen = true;
                _pendingFreeze = false;
            }
        }

        /// <summary>
        /// Queues freezing the current scene for a period of time
        /// </summary>
        /// <param name="duration"></param>
        public void QueueFreezeCurrentSceneFor(float duration)
        {
            QueueFreezeCurrentScene();
            Engine.Timer.Wait(duration, UnfreezeCurrentScene);
        }

        /// <summary>
        /// Updates the current scene. Calls ApplyFreeze at the end to ensure the scene is frozen
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateCurrentScene(GameTime gameTime)
        {
            if (IsStackEmpty())
                return;

            if (!_sceneFrozen)
            {
                Engine.Tween.Update();
                Engine.Node.UpdateNodes(gameTime);
                GetCurrentScene()?.Update(gameTime);
            }

            ApplyPendingFreeze();
        }


        /// <summary>
        /// Draws the current scene.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawCurrentScene(SpriteBatch spriteBatch)
        {
            if (IsStackEmpty())
                return;

            Engine.Screen.DrawTilemaps(spriteBatch);
            Engine.Node.DrawNodes(spriteBatch);
            GetCurrentScene()?.Draw(spriteBatch);
        }


        /// <summary>
        /// Checks if the stack is empty
        /// </summary>
        /// <returns></returns>
        public bool IsStackEmpty()
        {
            return Scenes.Count == 0;
        }

        /// <summary>
        /// Reloads current scene as a fresh instance
        /// </summary>
        public void ReloadCurrentScene()
        {
            if (Scenes.Count == 0) return;

            var oldScene = Scenes.Pop();
            oldScene.Unload();

            var newScene = (IScene)Activator.CreateInstance(oldScene.GetType());
            AddScene(newScene);
        }

        
        /// <summary>
        /// Clears data related to scenes.
        /// </summary>
        public static void ClearSceneData()
        {
            Engine.Screen.Tilemaps.Clear();
            Engine.Node.DumpAllInstances();
        }
    }
}