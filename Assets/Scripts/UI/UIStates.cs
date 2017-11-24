using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIStates {

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
        Undo,
        EndTurn,
        EndGame,
        PuzzleSelection
    };

    public class UIStateChange : UnityEvent<State> { };
    static Dictionary<Group, UIStateChange> GroupStateChanges = new Dictionary<Group, UIStateChange>();


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init () {
        foreach (Group group in Enum.GetValues(typeof(Group)))
        {
            if(group != Group.None)
                GroupStateChanges[group] = new UIStateChange();
        }
	}

    public static void SetGroupState(Group group, State state)
    {
        GroupStateChanges[group].Invoke(state);
    }

    public static UIStateChange GetEvent(Group group)
    {
        return GroupStateChanges[group];
    }
}
