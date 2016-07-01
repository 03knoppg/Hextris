using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CurrentPlayerText : UIThing {

    new void Start()
    {
        base.Start();
        UISignals.AddListeners(OnPlayerChange, new List<Signal>(){Signal.PlayerTurn}); 
    }

    void OnPlayerChange(Signal Signal, object playerIndexObj)
    {
        if(Signal == global::Signal.PlayerTurn)
            GetComponentInChildren<Text>().text = "Player " + (((int)playerIndexObj) + 1) + "'s turn";
    }
}
