using System.Collections.Generic;

public class StateBoardSelect : HextrisStateMachineBehaviour
{
    protected override void OnEnter()
    {
        AddListeners(new List<ESignalType> { ESignalType.StartPuzzle, ESignalType.Quit });
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
        switch(signalType)
        {
            case ESignalType.StartPuzzle:
                Game.SelectGame((int)index);
                SMTransition(signalType);
                break;
        }
    }
}
