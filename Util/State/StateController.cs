using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Monolith.Util
{
    public class StateController
    {
        public Dictionary<string, IState> _states = new Dictionary<string, IState>(StringComparer.OrdinalIgnoreCase);
        private IState _currentState;
        private IState _initialState;
        private IState _globalState;

        public State CurrentState { get; private set; }

        public StateController(IState initialState)
        {
            _initialState = initialState;
            InitializeStates([initialState]);
        }

        public StateController(IState initialState, IEnumerable<IState> childStates)
        {
            _initialState = initialState;
            InitializeStates(childStates);
        }

        public StateController(IState initialState, IState globalState, IEnumerable<IState> childStates)
        {
            _initialState = initialState;
            _globalState = globalState;
            InitializeStates(childStates);
        }

        private void InitializeStates(IEnumerable<IState> states)
        {
            foreach (var state in states)
            {
                string stateName = state.GetType().Name;
                _states[stateName] = state;

                state.TransitionRequested += OnChildTransition;
            }

            if (_initialState != null)
            {
                _currentState = _initialState;
                CurrentState = _currentState as State;
                _currentState.OnEnter();
                _globalState?.OnEnter();
            }

        }

        public void RequestTransition(string newStateName)
        {
            OnChildTransition(_currentState, newStateName);
        }


        public void Update(GameTime gameTime)
        {
            _currentState?.Update(gameTime);
            _globalState?.Update(gameTime);
        }

        private void OnChildTransition(IState state, string newStateName)
        {
            if (state != _currentState)
                return;

            if (_states.TryGetValue(newStateName, out IState new_state))
            {
                _currentState?.OnExit();

                _currentState = new_state;
                _currentState.OnEnter();

                CurrentState = _currentState as State;
            }
            else
            {
                Console.WriteLine($"Could not find state {newStateName}");
            }
        }

    }
}