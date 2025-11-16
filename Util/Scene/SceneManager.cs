using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConstructEngine.Components;
using ConstructEngine.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConstructEngine.Util
{
    public class SceneManager : Scene
    {
        public readonly Stack<IScene> Scenes = new();
        private bool _sceneFrozen;
        private bool _pendingFreeze;

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

            scene.Initialize();
            scene.Load();

            Scenes.Push(scene);
        }

        /// <summary>
        /// Acts in between scene actions where scenes are removed, added, etc
        /// </summary>
        private void SceneIntervention()
        {
            Engine.ClearSceneLists();
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
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetTypes().Any(t => t.Name == sceneName && typeof(IScene).IsAssignableFrom(t)));

            if (assembly == null) return null;

            var type = assembly.GetTypes()
                .FirstOrDefault(t => t.Name == sceneName && typeof(IScene).IsAssignableFrom(t));

            if (type == null) return null;

            return (IScene)Activator.CreateInstance(type);
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
            Timer.Wait(duration, UnfreezeCurrentScene);
        }

        /// <summary>
        /// Updates the current scene, loops safely through backseat components and updates them. Calls ApplyFreeze at the end to ensure the scene is frozen
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateCurrentScene(GameTime gameTime)
        {
            if (!IsStackEmpty() && !_sceneFrozen)
                GetCurrentScene()?.Update(gameTime);

            for (int i = BackseatComponent.BackseatComponentList.Count - 1; i >= 0; i--)
                BackseatComponent.BackseatComponentList[i].Update(gameTime);

            ApplyPendingFreeze();
        }

        /// <summary>
        /// Draws the current scene, loops safely through backseat components and draws them.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawCurrentScene(SpriteBatch spriteBatch)
        {
            if (!IsStackEmpty())
                GetCurrentScene()?.Draw(spriteBatch);

            for (int i = BackseatComponent.BackseatComponentList.Count - 1; i >= 0; i--)
                BackseatComponent.BackseatComponentList[i].Draw(spriteBatch);
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

            var currentType = Scenes.Peek().GetType();
            var newScene = (IScene)Activator.CreateInstance(currentType);
            AddScene(newScene);
        }
    }
}