using System.Collections.Generic;
using UnityEngine;

public class BoardSelect : HextrisStateMachineBehaviour
{
    protected override void OnEnter()
    {
        AddListeners(new List<ESignalType> { ESignalType.StartPuzzle });
        AddListenersOne(new List<ESignalType> { ESignalType.StartPuzzle });

        UIStates.SetGroupState(UIStates.Group.EndGame, UIStates.State.Hidden);
        UIStates.SetGroupState(UIStates.Group.EndTurn, UIStates.State.Hidden);
        UIStates.SetGroupState(UIStates.Group.PuzzleSelection, UIStates.State.Active);
    }

    protected override void OnSignal(ESignalType signalType)
    {
        switch (signalType)
        {
            case ESignalType.StartPuzzle:
                Game.SelectGame();
                SMTransition(signalType);
                break;
            case ESignalType.Quit:
                SMTransition(signalType);
                break;
        }
    }

    protected override void OnSignalOne(ESignalType signalType, object index)
    {
        Game.SelectGame((int)index);
        Animator.SetTrigger("StartPuzzle");
    }
}
