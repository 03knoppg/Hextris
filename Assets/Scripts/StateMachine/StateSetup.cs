using System.Collections.Generic;

public class StateSetup : HextrisStateMachineBehaviour
{
    protected override void OnEnter()
    {
        UIStates.SetGroupState(UIStates.Group.PieceControls, UIStates.State.Hidden);
        UIStates.SetGroupState(UIStates.Group.EndGame, UIStates.State.Hidden);
        UIStates.SetGroupState(UIStates.Group.PuzzleSelection, UIStates.State.Hidden);

        AddListeners(new List<ESignalType> { ESignalType.SetupComplete });

        Game.currentGame.StartSetup();
    }

    protected override void OnSignal(ESignalType signalType)
    {
        switch (signalType)
        {
            case ESignalType.SetupComplete:
                SMTransition(signalType);
                break;
        }
    }
}
