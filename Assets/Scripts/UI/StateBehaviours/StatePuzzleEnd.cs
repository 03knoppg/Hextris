using System.Collections.Generic;

public class StatePuzzleEnd : HextrisStateMachineBehaviour
{

    protected override void OnEnter()
    {
        UIStates.SetGroupState(UIStates.Group.EndGame, UIStates.State.Active);
        UIStates.SetGroupState(UIStates.Group.PieceControls, UIStates.State.Disabled);
        UIStates.SetGroupState(UIStates.Group.Undo, UIStates.State.Disabled);

        AddListeners(new List<ESignalType> { ESignalType.StartPuzzle, ESignalType.SelectBoard });

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
                Animator.SetTrigger(signalType.ToString());
                break;
            case ESignalType.SelectBoard:
                UIStates.SetGroupState(UIStates.Group.EndGame, UIStates.State.Hidden);
                Animator.SetTrigger(signalType.ToString());
                break;
        }
    }
}