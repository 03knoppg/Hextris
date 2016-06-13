using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

public enum UISignal
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
    Restart
};

public class UISignals : MonoBehaviour
{
    

    //public class UIButtonClick : UnityEvent<UISignal> { };
    public class UIButtonClick : UnityEvent<UISignal, object> { };
    Dictionary<UISignal, UIButtonClick> buttonActions = new Dictionary<UISignal, UIButtonClick>();

    void Awake()
    {
        foreach (UISignal button in Enum.GetValues(typeof(UISignal)))
        {
            if(button != UISignal.None)
                buttonActions[button] = new UIButtonClick();
        }
    }

    public void AddListeners(UnityAction<UISignal, object> UIClick, List<UISignal> list)
    {
        foreach (UISignal button in list)
            buttonActions[button].AddListener(UIClick);
    }

    public void Click(UISignal button, object arg1 = null)
    {
        buttonActions[button].Invoke(button, arg1);
    }
}
