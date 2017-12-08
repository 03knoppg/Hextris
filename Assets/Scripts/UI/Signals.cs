using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ESignalType
{
    None,
    EndTurn,
    RotateCW,
    RotateUndo,
    RotateCCW,
    StartPuzzle,
    SelectBoard,
    Quit,
    PlayerTurn,
    PlayerWin,
    Restart,
    CamPosition, // arg1 => board bounds
    GameStart,
    PuzzleComplete, // arg1 => stars
    PuzzleMode,
    PvPMode,
    SetupComplete,
    PieceSelected,
    ClearProgression
};


public class Signals
{
    public class UIButtonClick : UnityEvent<ESignalType> { };
    public class UIButtonClickOne : UnityEvent<ESignalType, object> { };
    public class UIButtonClickTwo : UnityEvent<ESignalType, object, object> { };
    static Dictionary<ESignalType, UIButtonClick> buttonActions = new Dictionary<ESignalType, UIButtonClick>();
    static Dictionary<ESignalType, UIButtonClickOne> buttonActionsOne = new Dictionary<ESignalType, UIButtonClickOne>();
    static Dictionary<ESignalType, UIButtonClickTwo> buttonActionsTwo = new Dictionary<ESignalType, UIButtonClickTwo>();


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        foreach (ESignalType button in Enum.GetValues(typeof(ESignalType)))
        {
            if (button != ESignalType.None)
            {
                buttonActions[button] = new UIButtonClick();
                buttonActionsOne[button] = new UIButtonClickOne();
                buttonActionsTwo[button] = new UIButtonClickTwo();
            }
        }
    }

    public static void AddListeners(UnityAction<ESignalType> UIClick, List<ESignalType> list)
    {
        foreach (ESignalType button in list)
            buttonActions[button].AddListener(UIClick);
    }
    public static void AddListeners(UnityAction<ESignalType, object> UIClick, List<ESignalType> list)
    {
        foreach (ESignalType button in list)
            buttonActionsOne[button].AddListener(UIClick);
    }
    public static void AddListeners(UnityAction<ESignalType, object, object> UIClick, List<ESignalType> list)
    {
        foreach (ESignalType button in list)
            buttonActionsTwo[button].AddListener(UIClick);
    }
    public static void RemoveListeners(UnityAction<ESignalType> UIClick, List<ESignalType> list)
    {
        foreach (ESignalType button in list)
            buttonActions[button].RemoveListener(UIClick);
    }
    public static void RemoveListeners(UnityAction<ESignalType, object> UIClick, List<ESignalType> list)
    {
        foreach (ESignalType button in list)
            buttonActionsOne[button].RemoveListener(UIClick);
    }
    public static void RemoveListeners(UnityAction<ESignalType, object, object> UIClick, List<ESignalType> list)
    {
        foreach (ESignalType button in list)
            buttonActionsTwo[button].RemoveListener(UIClick);
    }

    public static void Invoke(ESignalType button, params object[] args)
    {
        string debug = button.ToString();

        foreach (object arg in args)
        {
            if (arg != null)
                debug += " " + arg.ToString();
            else
                debug += " NULL"; 
        }

        Debug.Log(debug);

        if (button == ESignalType.None)
            return;

        if (args.Length == 0)
            buttonActions[button].Invoke(button);
        else if (args.Length == 1)
            buttonActionsOne[button].Invoke(button, args[0]);
        else
            buttonActionsTwo[button].Invoke(button, args[0], args[1]);
    }
}
