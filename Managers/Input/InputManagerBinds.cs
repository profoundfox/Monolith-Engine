using System.Collections.Generic;
using System.Linq;
using Monolith.Helpers;
using Monolith.Input;
using Microsoft.Xna.Framework.Input;

namespace Monolith.Managers 
{
    public partial class InputManager
    {
        /// <summary>
        /// Adds binds to the map and clones it.
        /// </summary>
        public void InitializeBinds(Dictionary<string, List<InputAction>> bindsToAdd)
        {
            AddBinds(bindsToAdd);

            InitialBinds = DictionaryHelper.CloneDictionaryOfLists(
                Binds,
                action => action.Clone()
            );
        }

        /// <summary>
        /// Adds a bind to the dictionary.
        /// </summary>
        public void AddBind(string actionName, List<InputAction> inputActions)
        {
            if (Binds.TryGetValue(actionName, out var existing))
                existing.AddRange(inputActions.Select(a => a.Clone()));
            else
                Binds[actionName] = inputActions.Select(a => a.Clone()).ToList();
        }

        /// <summary>
        /// Adds multiple binds at once.
        /// </summary>
        public void AddBinds(Dictionary<string, List<InputAction>> bindsToAdd)
        {
            foreach (var kvp in bindsToAdd)
                AddBind(kvp.Key, kvp.Value);
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