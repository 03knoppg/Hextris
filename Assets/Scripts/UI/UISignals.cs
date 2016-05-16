using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

public class UISignals : MonoBehaviour
{
    public enum UISignal
    {
        EndTurn,
        RotateCW,
        RotateCCW
    };

    public class UIButtonClick : UnityEvent<UISignal> { };
    Dictionary<UISignal, UIButtonClick> buttonActions = new Dictionary<UISignal, UIButtonClick>();

    void Awake()
    {
        foreach (UISignal button in Enum.GetValues(typeof(UISignal)))
        {
            buttonActions[button] = new UIButtonClick();
        }
    }

    public void AddListeners(UnityAction<UISignal> UIClick, List<UISignal> list)
    {
        foreach (UISignal button in list)
            buttonActions[button].AddListener(UIClick);
    }

    public void Click(UISignal button) 
    {
        buttonActions[button].Invoke(button);
    }
}
