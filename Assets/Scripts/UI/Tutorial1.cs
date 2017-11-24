using System.Collections.Generic;

public class Tutorial1 : UIThing {

    public float level;
    
    new void Start()
    {
        base.Start();
        Signals.AddListeners(OnBoardSelect, new List<ESignalType>() { ESignalType.GameStart, ESignalType.SelectBoard, ESignalType.PlayerWin });
    }

    void OnBoardSelect(ESignalType signal, object arg1)
    {
        if (signal == ESignalType.GameStart)
        {
            if (((int)arg1) == level)
                State = UIStates.State.Active;

            else
                State = UIStates.State.Hidden;
        }
        else
            State = UIStates.State.Hidden;
    }

    public void Hide()
    {
        State = UIStates.State.Hidden;
    }

}
