using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tutorial1 : UIThing {

    public float level;
    
    new void Start()
    {
        base.Start();
        UISignals.AddListeners(OnBoardSelect, new List<Signal>() { Signal.GameStart, Signal.ShowBoardSelect, Signal.PlayerWin });
    }

    void OnBoardSelect(Signal signal, object arg1)
    {
        if (signal == Signal.GameStart)
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
