using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

public enum Signal
{
    None,
    EndTurn,
    RotateCW,
    RotateUndo,
    RotateCCW,
    SelectBoard,
    ShowBoardSelect,
    Quit,
    PlayerTurn,
    PlayerWin,
    Restart,
    CamPosition,
    GameStart,
    PuzzleComplete
};

public class Signals : MonoBehaviour
{

    public class UIButtonClick : UnityEvent<Signal> { };
    public class UIButtonClickOne : UnityEvent<Signal, object> { };
    public class UIButtonClickTwo : UnityEvent<Signal, object, object> { };
    Dictionary<Signal, UIButtonClick> buttonActions = new Dictionary<Signal, UIButtonClick>();
    Dictionary<Signal, UIButtonClickOne> buttonActionsOne = new Dictionary<Signal, UIButtonClickOne>();
    Dictionary<Signal, UIButtonClickTwo> buttonActionsTwo = new Dictionary<Signal, UIButtonClickTwo>();

    void Awake()
    {
        foreach (Signal button in Enum.GetValues(typeof(Signal)))
        {
            if (button != Signal.None)
            {
                buttonActions[button] = new UIButtonClick();
                buttonActionsOne[button] = new UIButtonClickOne();
                buttonActionsTwo[button] = new UIButtonClickTwo();
            }
        }
    }

    public void AddListeners(UnityAction<Signal> UIClick, List<Signal> list)
    {
        foreach (Signal button in list)
            buttonActions[button].AddListener(UIClick);
    }
    public void AddListeners(UnityAction<Signal, object> UIClick, List<Signal> list)
    {
        foreach (Signal button in list)
            buttonActionsOne[button].AddListener(UIClick);
    }
    public void AddListeners(UnityAction<Signal, object, object> UIClick, List<Signal> list)
    {
        foreach (Signal button in list)
            buttonActionsTwo[button].AddListener(UIClick);
    }

    public void Invoke(Signal button, object arg1 = null, object arg2 = null)
    {
        if (button == Signal.None)
            return;

        if (arg1 == null)
            buttonActions[button].Invoke(button);
        else if (arg2 == null)
            buttonActionsOne[button].Invoke(button, arg1);
        else
            buttonActionsTwo[button].Invoke(button, arg1, arg2);
    }
}
