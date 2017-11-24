using DTAnimatorStateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HextrisStateMachineBehaviour : DTStateMachineBehaviour<HextrisStateMachine>
{
    List<ESignalType> SignalTypes;
    List<ESignalType> SignalTypesOne;
    List<ESignalType> SignalTypesTwo;

    [SerializeField] string stateName;

    protected void AddListeners(List<ESignalType> signalTypes)
    {
        SignalTypes = signalTypes;
        Signals.AddListeners(OnSignal, signalTypes);
    }
    protected void AddListenersOne(List<ESignalType> signalTypes)
    {
        SignalTypesOne = signalTypes;
        Signals.AddListeners(OnSignalOne, signalTypes);
    }
    protected void AddListenersTwo(List<ESignalType> signalTypes)
    {
        SignalTypesTwo = signalTypes;
        Signals.AddListeners(OnSignalTwo, signalTypes);
    }
    
    //Prevent overriding to ensure listeners are cleaned up
    sealed protected override void OnStateExited()
    {
        if (SignalTypes != null) Signals.RemoveListeners(OnSignal, SignalTypes);
        if (SignalTypesOne != null) Signals.RemoveListeners(OnSignalOne, SignalTypesOne);
        if (SignalTypesTwo != null) Signals.RemoveListeners(OnSignalTwo, SignalTypesTwo);

        Debug.Log("Exit " +  GetType().Name + ": " + (stateName));

        OnExit();
    }
    sealed protected override void OnInitialized()
    {
        Debug.Log("Initialize " + GetType().Name + ": " + (stateName));

        OnInitialize();
    }

    sealed protected override void OnStateEntered()
    {
        Debug.Log("Enter " + GetType().Name + ": " + (stateName));

        OnEnter();
    }

    sealed protected override void OnStateUpdated()
    {
        Debug.Log("Update " + GetType().Name + ": " + (stateName));

        OnUpdate();
    }
    
    protected virtual void OnSignal(ESignalType signalType) { /*stub*/ }
    protected virtual void OnSignalOne(ESignalType signalType, object arg0) { /*stub*/ }
    protected virtual void OnSignalTwo(ESignalType signalType, object arg0, object arg1) { /*stub*/ }
    protected virtual void OnInitialize() { /*stub*/ }
    protected virtual void OnUpdate() { /*stub*/ }
    protected virtual void OnEnter() { /*stub*/ }
    protected virtual void OnExit() { /*stub*/ }

    protected virtual void SMTransition(ESignalType signal)
    {
        if (Animator.parameters.Any(param => param.name == signal.ToString()))
            Animator.SetTrigger(signal.ToString());
        else
            throw new ArgumentException("No animator trigger: " + signal.ToString());
    }
}
