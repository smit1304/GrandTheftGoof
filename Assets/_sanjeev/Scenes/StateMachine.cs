using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    // This class represents a single State in the machine
    public class State
    {
        public string name; // The name of the State (e.g., "Idle", "Chase")

        // This action runs once when the state starts (entering the state)
        public System.Action onEnter;

        // This action runs once when the state ends (exiting the state)
        public System.Action onExit;

        // This action runs every frame while the state is active (staying in the state)
        public System.Action onStay;

        // When converting the State to string, return its name
        public override string ToString()
        {
            return name;
        }
    }

    // Dictionary to hold all states by their name for quick access
    Dictionary<string, State> states = new Dictionary<string, State>();

    public State currentState; // The state currently active
    public State initialState; // The first state to start with when machine runs

    // Creates and adds a new State by name, and returns the new State object
    public State CreateState(string name)
    {
        var newState = new State();
        newState.name = name;

        // Automatically assign the first created state as the initial state
        if (states.Count == 0)
        {
            initialState = newState;
        }

        // Add the new state to the dictionary
        states[name] = newState;
        return newState;
    }

    // Called regularly (e.g., each frame) to update the state machine logic
    public void Update()
    {
        // If there are no states at all or no initial state, warn in the console
        if (states.Count == 0 || initialState == null)
        {
            Debug.Log("*** No states defined in the StateMachine!");
            return;
        }

        // If no current state is set, automatically switch to the initial state
        if (currentState == null)
            TransitionTo(initialState);

        // If the current state has an ongoing action (onStay), run it every frame
        if (currentState.onStay != null)
        {
            currentState.onStay();
        }
    }

    // Switch from the current state to a new given state
    public void TransitionTo(State newState)
    {
        // If the new state is null, display warning and do not switch
        if (newState == null)
        {
            Debug.Log("*** Cannot transition to a null state!");
            return;
        }

        // If there is a current state and it has onExit logic, run it before switching
        if (currentState != null && currentState.onExit != null)
        {
            currentState.onExit();
        }

        // Log the transition for debugging purposes
        Debug.LogFormat("Transitioning from state '{0}' to state '{1}'", currentState, newState);

        // Set the current state to the new state
        currentState = newState;

        // If the new state has onEnter logic, run it right after switching
        if (newState.onEnter != null)
        {
            newState.onEnter();
        }
    }

    // Switch current state using the state's name (string) instead of object reference
    public void TransitionTo(string newStateName)
    {
        // If the state with the given name does not exist, warn and do not switch
        if (!states.ContainsKey(newStateName))
        {
            Debug.Log("*** StateMachine does not contain the state named " + newStateName);
            return;
        }

        // Get the state object by name and transition to it
        var state = states[newStateName];
        TransitionTo(state);
    }
}