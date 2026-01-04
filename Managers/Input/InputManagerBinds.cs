using System.Collections.Generic;
using System.Linq;
using Monolith.Helpers;
using Monolith.Input;
using Microsoft.Xna.Framework.Input;

namespace Monolith.Managers 
{
    public partial class InputManager
    {
        public void AddBind(params InputAction[] inputActions)
        {
            var name = "InputAction" + (Binds.Count + 1).ToString();

            AddBind(name, inputActions);
        }
        /// <summary>
        /// Adds a bind to the dictionary with a given name.
        /// </summary>
        public void AddBind(string actionName, params InputAction[] inputActions)
        {
            if (Binds.TryGetValue(actionName, out var existing))
                existing.AddRange(inputActions.Select(a => a.Clone()));
            else
                Binds[actionName] = inputActions.Select(a => a.Clone()).ToList();
            
            if (!InitialBinds.TryGetValue(actionName, out _))
            {
                InitialBinds.Add(
                    actionName,
                    inputActions.Select(a => a.Clone()).ToList()
                );

            }
        }

        /// <summary>
        /// Removes a bind by name.
        /// </summary>
        public void RemoveBind(string actionName)
        {
            Binds.Remove(actionName);
        }

        /// <summary>
        /// Clears all binds.
        /// </summary>
        public void ClearBinds()
        {
            Binds.Clear();
        }

        /// <summary>
        /// Resets all binds to their initial states.
        /// </summary>
        public void ResetBinds()
        {
            Binds = InitialBinds.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Select(action => action.Clone()).ToList()
            );
        }

        /// <summary>
        /// Rebinds a key in an action.
        /// Adds a new InputAction if one with a key does not exist.
        /// </summary>
        public void Rebind(string actionName, Keys newKey)
        {
            if (!Binds.TryGetValue(actionName, out var actions))
            {
                Binds[actionName] = new List<InputAction> { new InputAction(newKey) };
                return;
            }

            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i].HasKey)
                {
                    actions[i] = new InputAction(newKey);
                    return;
                }
            }

            actions.Add(new InputAction(newKey));
        }

        /// <summary>
        /// Rebinds a button in an action.
        /// Adds a new InputAction if one with a button does not exist.
        /// </summary>
        public void Rebind(string actionName, Buttons newButton)
        {
            if (!Binds.TryGetValue(actionName, out var actions))
            {
                Binds[actionName] = new List<InputAction> { new InputAction(newButton) };
                return;
            }

            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i].HasKey)
                {
                    actions[i] = new InputAction(newButton);
                    return;
                }
            }

            actions.Add(new InputAction(newButton));
        }

        /// <summary>
        /// Rebinds a mouse button in an action.
        /// Adds a new InputAction if one with a mouse button does not exist.
        /// </summary>
        public void Rebind(string actionName, MouseButton newButton)
        {
            if (!Binds.TryGetValue(actionName, out var actions))
            {
                Binds[actionName] = new List<InputAction> { new InputAction(newButton) };
                return;
            }

            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i].HasKey)
                {
                    actions[i] = new InputAction(newButton);
                    return;
                }
            }

            actions.Add(new InputAction(newButton));
        }


    }
}