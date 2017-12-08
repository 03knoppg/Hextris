using System.Collections.Generic;

public class StateEnd : HextrisStateMachineBehaviour
{

    protected override void OnEnter()
    {
        UIStates.SetGroupState(UIStates.Group.EndGame, UIStates.State.Active);
        UIStates.SetGroupState(UIStates.Group.PieceControls, UIStates.State.Disabled);
        UIStates.SetGroupState(UIStates.Group.Undo, UIStates.State.Disabled);

        AddListeners(new List<ESignalType> { ESignalType.StartPuzzle, ESignalType.SelectBoard, ESignalType.Quit });

        Game.currentGame.SetPieceModeEnd();
        foreach (Player player in Game.currentGame.players)
        {
            player.SetActivePlayer(false);
        }
    }
    
    protected override void OnSignal(ESignalType signalType)
    {
        switch (signalType)
        {
            case ESignalType.StartPuzzle:
                UIStates.SetGroupState(UIStates.Group.EndGame, UIStates.State.Hidden);
                Game.SelectGame();
                SMTransition(signalType);
                break;
            case ESignalType.SelectBoard:
                UIStates.SetGroupState(UIStates.Group.EndGame, UIStates.State.Hidden);
                SMTransition(signalType);
                break;

            case ESignalType.Quit:
                SMTransition(signalType);
                break;
        }
    }
}