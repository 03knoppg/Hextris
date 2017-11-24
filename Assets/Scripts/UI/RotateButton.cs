using System.Collections.Generic;

public class RotateButton : UIButton {

    bool pieceSelected;

    new void Start()
    {
        Signals.AddListeners(OnSignal, new List<ESignalType> { ESignalType.PieceSelected });
        base.Start();
    }

    private void OnSignal(ESignalType signalType, object arg1)
    {
        if(signalType == ESignalType.PieceSelected)
        {
            pieceSelected = arg1 != null;
            if (State != UIStates.State.Hidden)
                State = pieceSelected ? UIStates.State.Active : UIStates.State.Disabled;
        }
    }

    protected override void OnStateChanged(UIStates.State newState)
    {
        base.OnStateChanged(newState);

        if (State == UIStates.State.Active && !pieceSelected)
            State = UIStates.State.Disabled;
    }
}
