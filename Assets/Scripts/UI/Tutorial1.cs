using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tutorial1 : UIThing {

    public float level;
    
    new void Start()
    {
        base.Start();
        UISignals.AddListeners(OnBoardSelect, new List<UISignal>() { UISignal.GameStart, UISignal.ShowBoardSelect, UISignal.PlayerWin });
    }

    void OnBoardSelect(UISignal signal, object arg1)
    {
        if (signal == UISignal.GameStart)
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
