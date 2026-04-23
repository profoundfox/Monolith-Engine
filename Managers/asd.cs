using System;
using System.Collections.Generic;

using Monolith.Hierarchy;

namespace Monolith.Managers
{
    public class StageMdanager
    {
        private readonly Node _root;

        private Node _scene;
        private readonly Stack<Node> _overlays = new();

        public StageMdanager(Node root)
        {
            _root = root ?? throw new ArgumentNullException(nameof(root));
        }

        /// <summary>
        /// Current active scene (the "world")
        /// </summary>
        public Node CurrentScene => _scene;

        /// <summary>
        /// Top-most overlay (if any)
        /// </summary>
        public Node CurrentOverlay => _overlays.Count > 0 ? _overlays.Peek() : null;

        // =========================
        // SCENE CONTROL
        // =========================

        public void SetScene(Node scene)
        {
            if (scene == null)
                throw new ArgumentNullException(nameof(scene));

            // Clear overlays first (they belong to previous scene)
            ClearOverlays();

            // Exit old scene
            if (_scene != null)
            {
                _scene.OnExit();
                _root.RemoveChild(_scene);
            }

            _scene = scene;

            _root.AddChild(scene);
            scene.OnEnter();
        }

        public void ReloadScene()
        {
            if (_scene == null)
                return;

            var type = _scene.GetType();
            var newScene = (Node)Activator.CreateInstance(type);

            SetScene(newScene);
        }

        // =========================
        // OVERLAY CONTROL
        // =========================

        public void PushOverlay(Node overlay)
        {
            if (overlay == null)
                throw new ArgumentNullException(nameof(overlay));

            _overlays.Push(overlay);

            _root.AddChild(overlay);
            overlay.OnEnter();
        }

        public void PopOverlay()
        {
            if (_overlays.Count == 0)
                return;

            var overlay = _overlays.Pop();

            overlay.OnExit();
            _root.RemoveChild(overlay);
        }

        public void ClearOverlays()
        {
            while (_overlays.Count > 0)
                PopOverlay();
        }
    }
}
