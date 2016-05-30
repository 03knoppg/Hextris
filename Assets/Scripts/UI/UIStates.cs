using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class UIStates : MonoBehaviour {

    public enum State
    {
        None,
        Hidden,
        Disabled,
        Active
    }

    public enum Group
    {
        None,
        PieceControls,
        EndTurn,
        EndGame,
        PuzzleSelection
    };

    public class UIStateChange : UnityEvent<State> { };
    Dictionary<Group, UIStateChange> GroupStateChanges = new Dictionary<Group, UIStateChange>();

    //public int winner;
    //public int currentPlayer;

	// Use this for initialization
	void Awake () {
        foreach (Group group in Enum.GetValues(typeof(Group)))
        {
            if(group != Group.None)
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
