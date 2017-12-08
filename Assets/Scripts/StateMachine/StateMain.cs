using System.Collections.Generic;

public class StateMain : HextrisStateMachineBehaviour {

    protected override void OnEnter()
    {
        UIStates.SetGroupState(UIStates.Group.PieceControls, UIStates.State.Active);

        AddListeners(new List<ESignalType> {
            ESignalType.Quit,
            ESignalType.RotateCCW,
            ESignalType.RotateUndo,
            ESignalType.RotateCW,
            ESignalType.EndTurn,
            ESignalType.Restart
            });

        AddListenersOne(new List<ESignalType> { ESignalType.PuzzleComplete });

        Game.currentGame.StartMain();
    }

    protected override void OnSignal(ESignalType signalType)
    {
        Game game = Game.currentGame;
        switch (signalType)
        {
            case ESignalType.Quit:
                SMTransition(signalType);
                break;
            case ESignalType.EndTurn:
                game.OnMovementFinished();
                break;
            case ESignalType.RotateCCW:
                game.currentSelectedPiece.RotateCCW();
                break;
            case ESignalType.RotateCW:
                game.currentSelectedPiece.RotateCW();
                break;
            case ESignalType.RotateUndo:
                if (game.lastSelectedPiece)
                    game.lastSelectedPiece.UndoRotation();
                else
                    game.currentSelectedPiece.ResetRotation();
                break;
            case ESignalType.Restart:
                Game.SelectGame(Game.currentGameIndex);
                SMTransition(signalType);
                break;
        }
    }

    protected override void OnSignalOne(ESignalType signalType, object arg0)
    {
        switch(signalType)
        {
            case ESignalType.PuzzleComplete:
                SMTransition(signalType);
                break;
        }
    }

    protected override void OnUpdate()
    {
        Game game = Game.currentGame;
        
        game.UpdateCollisions();
        game.UpdateUIStateMain();
    }
}
