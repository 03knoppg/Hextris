using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class UIStates : MonoBehaviour {

    public enum State
    {
        Hidden,
        Disabled,
        Active
    }

    public enum Group
    {
        PieceControls,
        EndTurn
    };

    public class UIStateChange : UnityEvent<State> { };
    //public Dictionary<Group, UIState> GroupState = new Dictionary<Group, UIState>();
    Dictionary<Group, UIStateChange> GroupStateChanges = new Dictionary<Group, UIStateChange>();

	// Use this for initialization
	void Start () {
        foreach (Group group in Enum.GetValues(typeof(Group)))
        {
            GroupStateChanges[group] = new UIStateChange();
        }
	}

    public void SetGroupState(Group group, State state)
    {
        GroupStateChanges[group].Invoke(state);
    }

    public UIStateChange GetEvent(Group group)
    {
        return GroupStateChanges[group];
    }
}
