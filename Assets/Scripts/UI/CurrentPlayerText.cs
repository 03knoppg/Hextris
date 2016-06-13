using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CurrentPlayerText : UIThing {

    new void Start()
    {
        base.Start();
        UISignals.AddListeners(OnPlayerChange, new List<UISignal>(){UISignal.PlayerTurn}); 
    }

    void OnPlayerChange(UISignal Signal, object playerIndexObj)
    {
        if(Signal == global::UISignal.PlayerTurn)
            GetComponentInChildren<Text>().text = "Player " + (((int)playerIndexObj) + 1) + "'s turn";
    }
}
